using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace TetSistemi.Protocol.Transport.Data.Endpoint
{
    /// <summary>
    /// Mqtt data endpoint
    /// </summary>
    public class MqttDataEndpoint : DataEndpoint
    {
        /// <summary>
        /// broker ip address
        /// </summary>
        public string brokerip { get; internal set; }
        /// <summary>
        /// broker port
        /// </summary>
        public int port { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = obj as MqttDataEndpoint;
            if (other == null) return false;

            if (this.brokerip != other.brokerip) return false;
            if (this.port != other.port) return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + brokerip.GetHashCode();
                hash = hash * 23 + port.GetHashCode();
                return hash;
            }
        }
    }
}