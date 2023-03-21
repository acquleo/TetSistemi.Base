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
    public class NamedPipeDataEndpoint : DataEndpoint
    {
        /// <summary>
        /// pipe name
        /// </summary>
        public string name { get; internal set; } = string.Empty;
        /// <summary>
        /// server name 
        /// </summary>
        public string server { get; internal set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = obj as NamedPipeDataEndpoint;
            if (other == null) return false;

            if(this.name != other.name) return false;
            if (this.server!= other.server) return false;

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
                hash = hash * 23 + name.GetHashCode();
                hash = hash * 23 + server.GetHashCode();
                return hash;
            }
        }
    }
}
