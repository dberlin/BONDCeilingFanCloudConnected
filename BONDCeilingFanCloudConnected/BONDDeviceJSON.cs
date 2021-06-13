namespace BONDCeilingFanCloudConnected
{
    namespace BONDDevice
    {
        using Newtonsoft.Json;

        public class DeviceInfo
        {
            [JsonProperty("name")] public string Name { get; set; }

            [JsonProperty("location")] public string Location { get; set; }

            [JsonProperty("type")] public string Type { get; set; }

            [JsonProperty("actions")] public string[] Actions { get; set; }

            [JsonProperty("_")] public string Empty { get; set; }

            [JsonProperty("state")] public NodeHash State { get; set; }

            [JsonProperty("properties")] public NodeHash Properties { get; set; }

            [JsonProperty("skeds")] public NodeHash Skeds { get; set; }

            [JsonProperty("addr")] public NodeHash NodeHash { get; set; }
        }

        public class NodeHash
        {
            [JsonProperty("_")] public string HashVal { get; set; }
        }

        public class DeviceProperties
        {
            [JsonProperty("max_speed")] public long MaxSpeed { get; set; }

            [JsonProperty("feature_light")] public bool FeatureLight { get; set; }

            [JsonProperty("feature_brightness")] public bool FeatureBrightness { get; set; }

            [JsonProperty("breeze_max_speed")] public long BreezeMaxSpeed { get; set; }

            [JsonProperty("breeze_min_speed")] public long BreezeMinSpeed { get; set; }

            [JsonProperty("breeze_period")] public long BreezePeriod { get; set; }

            [JsonProperty("_")] public string HashVal { get; set; }
        }

        public class DeviceState
        {
            [JsonProperty("power")] public bool Power { get; set; }

            [JsonProperty("speed")] public long Speed { get; set; }

            [JsonProperty("light")] public bool Light { get; set; }

            [JsonProperty("brightness")] public long Brightness { get; set; }

            [JsonProperty("brightness_cycle_phase")]
            public long BrightnessCyclePhase { get; set; }

            [JsonProperty("timer")] public long Timer { get; set; }

            [JsonProperty("breeze")] public long[] Breeze { get; set; }

            [JsonProperty("direction")] public long Direction { get; set; }

            [JsonProperty("_")] public string HashVal { get; set; }
        }
    }
}