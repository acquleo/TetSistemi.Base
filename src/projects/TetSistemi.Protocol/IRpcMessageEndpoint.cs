
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TetSistemi.Protocol
{
    /// <summary>
    /// RPC message endpoint interface
    /// </summary>
    public interface IRpcMessageEndpoint<TEnvelope> : IMessageEndpoint<TEnvelope>
        where TEnvelope : IMessageEnvelope
    { 
        /// <summary>
        /// execute an RPC call
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<CallResult<TEnvelope>> Call(TEnvelope req);

        /// <summary>
        /// execute an RPC call using the specified tiemout
        /// </summary>
        /// <param name="req"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<CallResult<TEnvelope>> Call(TEnvelope req, TimeSpan timeout);

    }

    /// <summary>
    /// Call result enum
    /// </summary>
    public enum CallResults
    {
        /// <summary>
        /// Call executed
        /// </summary>
        Ok=0,
        /// <summary>
        /// transport not available
        /// </summary>
        NotAvailable=1,
        /// <summary>
        /// call timeout
        /// </summary>
        Timeout=2
    }

    /// <summary>
    /// call result
    /// </summary>
    /// <typeparam name="TEnvelope"></typeparam>
    public class CallResult<TEnvelope>
        where TEnvelope : IMessageEnvelope
    {
        /// <summary>
        /// ctr
        /// </summary>
        public CallResult() {
            ResponseList = new List<TEnvelope>(); 
        }

        /// <summary>
        /// result
        /// </summary>
        public CallResults Result { get; set; }

        /// <summary>
        /// response messages
        /// </summary>
        public List<TEnvelope> ResponseList { get; set; }
    }
}