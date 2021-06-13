namespace BONDCeilingFanCloudConnected
{
    using Crestron.RAD.Common.Transports;

    public class BONDTransport : ATransportDriver
    {
        public BONDTransport()
        {
            this.IsConnected = true;
            this.IsEthernetTransport = true;
        }

        public override void SendMethod(string message, object[] paramaters)
        {
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    }
}