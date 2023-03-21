using ICCS.DataBroker.MQTT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetSistemi.Protocol.Transport.Mqtt
{
    public class MqttMessage
    {
        public bool Retain { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Responsetopic { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public byte[] PayloadRaw { get; set; }
        public byte Qos { get; set; }
        public List<UserPropertyInfo> Userproperties { get; set; }=new List<UserPropertyInfo>();
    }
}
