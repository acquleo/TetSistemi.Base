using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TetSistemi.Protocol.Enveloper.Mqtt
{
    /// <summary>
    /// MQTT message envelope definition
    /// </summary>
    public class MqttMessageEnvelope : IMessageEnvelope
    {
        /// <summary>
        /// MQTT QOS
        /// </summary>
        public byte Qos { get; set; }
        /// <summary>
        /// MQTT retain flag
        /// </summary>
        public bool Retain { get; set; }
        /// <summary>
        /// MQTT message topic
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// MQTT message respone topic
        /// </summary>
        public string Responsetopic { get; set; }
        /// <summary>
        /// Envelope payload
        /// </summary>
        public IMessage Payload { get; set; }
    }
}
