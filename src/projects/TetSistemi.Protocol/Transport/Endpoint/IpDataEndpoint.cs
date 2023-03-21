using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TetSistemi.Protocol.Transport.Data.Endpoint
{
    /// <summary>
    /// Ip data enpoint information
    /// </summary>
    public class IpDataEndpoint : DataEndpoint
    {
        /// <summary>
        /// Ip address
        /// </summary>
        public string address { get; internal set; } = string.Empty;
        /// <summary>
        /// Port
        /// </summary>
        public int port { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return address+":"+port;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = obj as IpDataEndpoint;
            if (other == null) return false;

            if(this.address != other.address) return false;
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
                hash = hash * 23 + address.GetHashCode();
                hash = hash * 23 + port.GetHashCode();
                return hash;
            }
        }
    }
}
