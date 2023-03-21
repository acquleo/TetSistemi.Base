using System;
using System.Collections.Generic;
using System.Text;

namespace TetSistemi.Protocol.Transport.Mqtt
{
    public interface IMqttDataTransport : IPubSubDataTransport<MqttMessage,List<MqttTopic>>
    {

    }
}
