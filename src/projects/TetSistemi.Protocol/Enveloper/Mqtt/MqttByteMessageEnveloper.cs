using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TetSistemi.Protocol.Exceptions;
using TetSistemi.Protocol.Transport.Mqtt;

namespace TetSistemi.Protocol.Enveloper.Mqtt
{
    public class MqttByteMessageEnveloper : IMessageEnveloper<MqttMessage,MqttMessageEnvelope>
    {
        IMessageSerializer<byte[]> serializer;
        public MqttByteMessageEnveloper(IMessageSerializer<byte[]> serializer)
        {
            this.serializer = serializer;
        }

        public MqttMessageEnvelope Wrap(MqttMessage data)
        {
            MqttMessageEnvelope topic = new MqttMessageEnvelope();

            //deserializzo l'XML
            IMessage obj = this.serializer.Deserialize(data.PayloadRaw, data.ContentType);

            topic.Topic = data.Topic;
            topic.Responsetopic = data.Responsetopic;
            topic.Payload = obj;
            topic.Retain = data.Retain;
            topic.Qos = data.Qos;

            return topic;
        }

        public MqttMessage Unwrap(MqttMessageEnvelope msg)
        {
            if (!(msg is MqttMessageEnvelope)) return null;

            MqttMessageEnvelope castmsg = (MqttMessageEnvelope)msg;

            MqttMessage data = new MqttMessage();

            string serializerName = msg.Payload.GetType().Name;

            data.PayloadRaw = this.serializer.Serialize(msg.Payload, serializerName);
            data.Topic = castmsg.Topic;
            data.Responsetopic = castmsg.Responsetopic;
            data.ContentType = serializerName;
            data.Qos = castmsg.Qos;
            data.Retain = castmsg.Retain;
            return data;

        }
    }
}
