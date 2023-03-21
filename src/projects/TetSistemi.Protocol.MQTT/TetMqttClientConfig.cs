namespace ICCS.DataBroker.MQTT
{
    public class TetMqttClientConfig
    {
        public string clientid { get; set; } = string.Empty;
        public string host { get; set; } = "127.0.0.1";
        public int port { get; set; } = 1883;
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string encoding { get; set; } = "utf-8";
        public bool encode_payload { get; set; } = true;
    }
}