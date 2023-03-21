using ICCS.DataBroker.MQTT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetSistemi.Protocol.Transport.Mqtt
{
    public class MqttTopic
    {
        public string Topic { get; set; } = string.Empty;
        public byte Qos { get; set; }
    }
}
