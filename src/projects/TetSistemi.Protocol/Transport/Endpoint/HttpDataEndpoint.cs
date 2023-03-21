using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace TetSistemi.Protocol.Transport.Data.Endpoint
{

    /// <summary>
    /// http data endpoint
    /// </summary>
    public class HttpDataEndpoint : DataEndpoint
    {
        /// <summary>
        /// HTTP url
        /// </summary>
        public string url { get; internal set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = obj as HttpDataEndpoint;
            if (other == null) return false;

            return this.url == other.url;
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
                hash = hash * 23 + url.GetHashCode();
                return hash;
            }
        }
    }
}