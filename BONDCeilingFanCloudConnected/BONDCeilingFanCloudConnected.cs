namespace BONDCeilingFanCloudConnected
{
    using System.Collections.Generic;
    using BONDDevice;
    using Crestron.RAD.Common.Enums;
    using Crestron.RAD.Common.Interfaces;
    using Crestron.RAD.Common.Interfaces.ExtensionDevice;
    using Crestron.RAD.DeviceTypes.ExtensionDevice;
    using Newtonsoft.Json;
    using static BONDCeilingFanConstants;

    public class BONDCeilingFanCloudConnected : AExtensionDevice, ICloudConnected
    {
        private readonly List<IPropertyAvailableValue<int>> fanDirectionAvailableValues =
            new List<IPropertyAvailableValue<int>>();

        private readonly BONDProtocol protocol;

        private PropertyValue<int> fanDirectionValue;
        private PropertyValue<int> fanLightSliderValue;
        private PropertyValue<bool> fanLightToggleValue;
        private PropertyValue<bool> fanLightToggleVisible;
        private PropertyValue<bool> fanPowerToggleValue;
        private PropertyValue<int> fanSpeedSliderValue;

        public BONDCeilingFanCloudConnected()
        {
            this.ConnectionTransport = new BONDTransport();
            this.DeviceProtocol = this.protocol = new BONDProtocol(this.ConnectionTransport, this.Id);
            this.DeviceProtocol.Initialize(this.DriverData);
            this.protocol.StateReceivedEvent -= this.OnStateReceived;
            this.protocol.StateReceivedEvent += this.OnStateReceived;
            this.CreateDeviceDefinition();
            this.Initialize();
        }

        public void Initialize()
        {
            this.Connected = true;
            this.protocol.Start();
            this.fanLightToggleVisible.Value = true;
            this.Commit();
        }

        public override void Reconnect()
        {
            base.Reconnect();
            this.protocol.Stop();
            this.protocol.Start();
        }

        public override void Disconnect()
        {
            base.Disconnect();
            this.protocol.Stop();
        }

        public override void Connect()
        {
            base.Connect();
            this.protocol.Start();
        }


        private void OnStateReceived(object sender, DeviceState deviceState)
        {
            var receivedState = JsonConvert.SerializeObject(deviceState, Formatting.Indented);
            BONDLogging.TraceMessage(this.EnableLogging, $"State received {receivedState}");
            this.fanLightSliderValue.Value = (int) deviceState.Brightness;
            this.fanLightToggleValue.Value = deviceState.Light;
            this.fanPowerToggleValue.Value = deviceState.Power;
            this.fanSpeedSliderValue.Value = (int) deviceState.Speed;
            this.fanDirectionValue.Value = (int) deviceState.Direction;
            this.Commit();
        }

        private void CreateDeviceDefinition()
        {
            BONDLogging.TraceMessage(this.EnableLogging, "Creating device definition");
            this.fanPowerToggleValue = this.CreateProperty<bool>(
                new PropertyDefinition(FanPowerToggleValueKey, null,
                    DevicePropertyType.Boolean));
            this.fanLightToggleValue = this.CreateProperty<bool>(
                new PropertyDefinition(FanLightToggleValueKey, null,
                    DevicePropertyType.Boolean));
            this.fanLightToggleVisible = this.CreateProperty<bool>(
                new PropertyDefinition(FanLightToggleVisibleKey, null,
                    DevicePropertyType.Boolean));
            this.fanLightSliderValue = this.CreateProperty<int>(new PropertyDefinition(FanLightSliderValueKey, null,
                DevicePropertyType.Int32, 1, 100, 1));
            this.fanSpeedSliderValue = this.CreateProperty<int>(new PropertyDefinition(FanSpeedSliderValueKey, null,
                DevicePropertyType.Int32, 1, 6, 1));
            this.fanDirectionAvailableValues.Add(new PropertyAvailableValue<int>(0, DevicePropertyType.Int32, "Winter",
                "Winter"));
            this.fanDirectionAvailableValues.Add(new PropertyAvailableValue<int>(1, DevicePropertyType.Int32, "Summer",
                "Summer"));
            this.fanDirectionValue = this.CreateProperty<int>(new PropertyDefinition(FanDirectionValueKey, null,
                DevicePropertyType.Int32, this.fanDirectionAvailableValues));
        }

        protected override IOperationResult DoCommand(string command, string[] parameters)
        {
            return new OperationResult(OperationResultCode.Success);
        }

        protected override IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value)
        {
            var succeeded = true;
            switch (propertyKey)
            {
                case FanPowerToggleValueKey:
                {
                    if (value is bool boolVal)
                    {
                        succeeded = boolVal ? this.protocol.TurnFanOn() : this.protocol.TurnFanOff();
                        if (succeeded) this.fanPowerToggleValue.Value = boolVal;
                    }

                    break;
                }
                case FanSpeedSliderValueKey:
                {
                    if (value is int intVal)
                    {
                        succeeded = this.protocol.SetSpeed(intVal);
                        if (succeeded) this.fanSpeedSliderValue.Value = intVal;
                    }

                    break;
                }

                case FanDirectionValueKey:
                {
                    if (value is int intVal)
                    {
                        succeeded = this.protocol.SetDirection(intVal);
                        if (succeeded) this.fanDirectionValue.Value = intVal;
                    }

                    break;
                }
                case FanLightToggleValueKey:
                {
                    if (value is bool boolVal)
                    {
                        succeeded = boolVal ? this.protocol.TurnLightOn() : this.protocol.TurnLightOff();
                        if (succeeded) this.fanLightToggleValue.Value = boolVal;
                    }

                    break;
                }
                case FanLightSliderValueKey:
                {
                    if (value is int intVal)
                    {
                        succeeded = this.protocol.SetBrightness(intVal);
                        if (succeeded) this.fanLightSliderValue.Value = intVal;
                    }

                    break;
                }
            }

            if (!succeeded) return new OperationResult(OperationResultCode.Error);
            this.Commit();
            return new OperationResult(OperationResultCode.Success);
        }

        protected override IOperationResult SetDriverPropertyValue<T>(string objectId, string propertyKey, T value)
        {
            return new OperationResult(OperationResultCode.Success);
        }
    }
}