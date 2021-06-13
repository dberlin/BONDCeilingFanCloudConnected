namespace BONDCeilingFanCloudConnected
{
    using System;
    using BONDDevice;
    using Crestron.RAD.Common.BasicDriver;
    using Crestron.RAD.Common.Transports;
    using Flurl;
    using Flurl.Http;
    using Newtonsoft.Json;

    public class BONDProtocol : ABaseDriverProtocol
    {
        private const string TokenAttribute = "Token";
        private const string HostnameAttribute = "Hostname";
        private static readonly TimeSpan ActionTimeoutLength = TimeSpan.FromSeconds(5);
        private readonly object updateLock = new object();
        public DeviceProperties DeviceProperties = null;
        private string deviceToken;
        private string deviceUrl;

        public BONDProtocol(ISerialTransport transport, byte id) : base(transport, id)
        {
            this.EnableLogging = true;
            this.PollingInterval = 10000;
        }


        public event EventHandler<DeviceState> StateReceivedEvent;

        public void Start()
        {
            this.EnableAutoPolling = true;
        }

        public void Stop()
        {
            this.EnableAutoPolling = false;
        }

        protected override void ConnectionChangedEvent(bool connection)
        {
        }

        public override void SetUserAttribute(string attributeId, string attributeValue)
        {
            BONDLogging.TraceMessage(this.EnableLogging, $"Setting user attribute {attributeId} to {attributeValue}!");
            switch (attributeId)
            {
                case HostnameAttribute:
                    this.deviceUrl = $"http://{attributeValue}";
                    break;
                case TokenAttribute:
                    this.deviceToken = attributeValue;
                    break;
            }
        }

        protected override void ChooseDeconstructMethod(ValidatedRxData validatedData)
        {
        }

        private Url MakeDeviceURL()
        {
            return this.deviceUrl.AppendPathSegments("/v2", "/devices", "/1");
        }

        private Url MakeDeviceURL(string segment)
        {
            return this.MakeDeviceURL().AppendPathSegment(segment);
        }

        private Url MakeActionURL(string action)
        {
            return this.MakeDeviceURL().AppendPathSegments("/actions", $"/{action}");
        }

        protected DeviceInfo GetInfo()
        {
            if (!this.CheckURLAndToken()) return null;
            try
            {
                return this.MakeDeviceURL()
                    .WithHeader("BOND-Token", this.deviceToken)
                    .GetJsonAsync<DeviceInfo>()
                    .Result;
            }
            catch (Exception e)
            {
                BONDLogging.TraceMessage(this.EnableLogging, $"Exception while retrieving device info:{e}");
                return null;
            }
        }

        protected DeviceProperties GetProperties()
        {
            if (!this.CheckURLAndToken()) return null;

            try
            {
                return this.MakeDeviceURL("/properties")
                    .WithHeader("BOND-Token", this.deviceToken)
                    .GetJsonAsync<DeviceProperties>()
                    .Result;
            }
            catch (Exception e)
            {
                BONDLogging.TraceMessage(this.EnableLogging, $"Exception while retrieving properties:{e}");

                return null;
            }
        }

        private bool CheckURLAndToken()
        {
            if (string.IsNullOrEmpty(this.deviceUrl))
            {
                BONDLogging.TraceMessage(this.EnableLogging, "URL is empty");
                return false;
            }

            if (string.IsNullOrEmpty(this.deviceToken))
            {
                BONDLogging.TraceMessage(this.EnableLogging, "Token is empty");
                return false;
            }

            return true;
        }

        private bool RunAction(string actionName)
        {
            if (!this.CheckURLAndToken()) return false;
            try
            {
                lock (this.updateLock)
                {
                    return this.MakeActionURL(actionName)
                        .WithHeader("BOND-Token", this.deviceToken)
                        .WithTimeout(ActionTimeoutLength)
                        .PutAsync()
                        .Result
                        .StatusCode == 200;
                }
            }
            catch (Exception e)
            {
                BONDLogging.TraceMessage(this.EnableLogging, $"Error running action:{e}");
                return false;
            }
        }

        private bool RunAction<T>(string actionName, T value)
        {
            if (!this.CheckURLAndToken()) return false;
            try
            {
                lock (this.updateLock)
                {
                    var serializedContent = JsonConvert.SerializeObject(new {argument = value});
                    return this.MakeActionURL(actionName)
                        .WithHeader("BOND-Token", this.deviceToken)
                        .WithTimeout(ActionTimeoutLength)
                        .PutStringAsync(serializedContent)
                        .Result.StatusCode == 200;
                }
            }
            catch (Exception e)
            {
                BONDLogging.TraceMessage(this.EnableLogging, $"Error running action:{e}");
                return false;
            }
        }


        public bool SetDirection(int value)
        {
            return this.RunAction("SetDirection", value);
        }

        public bool SetSpeed(int value)
        {
            return this.RunAction("SetSpeed", value);
        }

        public bool SetBrightness(int value)
        {
            return this.RunAction("SetSpeed", value);
        }

        public bool TurnFanOn()
        {
            return this.RunAction("TurnOn");
        }

        public bool TurnFanOff()
        {
            return this.RunAction("TurnOff");
        }

        public bool TurnLightOn()
        {
            return this.RunAction("TurnLightOn");
        }

        public bool TurnLightOff()
        {
            return this.RunAction("TurnLightOff");
        }

        protected override void Poll()
        {
            BONDLogging.TraceMessage(this.EnableLogging, "Polling fan");
            if (!this.CheckURLAndToken()) return;
            try

            {
                lock (this.updateLock)
                {
                    var deviceState = this.MakeDeviceURL("/state")
                        .WithHeader("BOND-Token", this.deviceToken)
                        .GetJsonAsync<DeviceState>().Result;
                    this.StateReceivedEvent?.Invoke(this, deviceState);
                }
            }
            catch (Exception e)
            {
                BONDLogging.TraceMessage(this.EnableLogging, $"Exception while polling:{e}");
            }
        }
    }
}