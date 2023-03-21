using System;
using System.Collections.Generic;
using System.Text;

namespace ICCS.DataBroker.EventSource
{
    public static class SystemEvents
    {
        public const string keepalive = "keepalive";
        public const string mqtt_disconnected = "mqtt_disconnected";
        public const string mqtt_connected = "mqtt_connected";
        public const string mqtt_message = "mqtt_message";
    }
}
