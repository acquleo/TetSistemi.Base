
using System.Threading.Tasks;
using TetSistemi.Protocol.Exceptions;

namespace TetSistemi.Protocol
{
    /// <summary>
    /// delegates message in/out trace
    /// </summary>
    /// <param name="endpoint">this endpoint</param>
    /// <param name="msg">message</param>
    /// <param name="direction">message direction</param>
    /// <param name="arg">endpoint</param>
    /// <returns></returns>
    public delegate Task dMessageTraceAsync<TEnvelope>(IMessageEndpoint<TEnvelope> endpoint, TEnvelope msg, DataDirection direction, DataEndpoint arg)
        where TEnvelope : IMessageEnvelope;

    /// <summary>
    /// delegates message receive
    /// </summary>
    /// <param name="endpoint">this endpoint</param>
    /// <param name="msg">message</param>
    /// <param name="sender">endpoint</param>
    /// <returns></returns>
    public delegate Task dMessageReceivedAsync<TEnvelope>(IMessageEndpoint<TEnvelope> endpoint, TEnvelope msg, DataEndpoint sender)
        where TEnvelope : IMessageEnvelope;

    /// <summary>
    /// generic delegate
    /// </summary>
    /// <param name="endpoint">this endpoint</param>
    /// <returns></returns>
    public delegate Task dMessageTransportAsync<TEnvelope>(IMessageEndpoint<TEnvelope> endpoint)
        where TEnvelope : IMessageEnvelope;

    /// <summary>
    /// delegates internal exception throw
    /// </summary>
    /// <param name="endpoint">this endpoint</param>
    /// <param name="exception">generated exception</param>
    /// <returns></returns>
    public delegate Task dMessageExceptionAsync<TEnvelope>(IMessageEndpoint<TEnvelope> endpoint, ProtocolException exception)
        where TEnvelope : IMessageEnvelope;

    /// <summary>
    /// message endpoint interface
    /// </summary>
    public interface IMessageEndpoint<TEnvelope> 
        where TEnvelope : IMessageEnvelope
    {
        

        /// <summary>
        /// send a message through the data transport
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task<bool> SendAsync(TEnvelope msg);

        /// <summary>
        /// send a message through the data transport to the specified endpoint
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<bool> SendAsync(TEnvelope msg, DataEndpoint to);

        /// <summary>
        /// message received event
        /// </summary>
        event dMessageReceivedAsync<TEnvelope> MessageReceivedAsync;

        /// <summary>
        /// message trace event
        /// </summary>
        event dMessageTraceAsync<TEnvelope> MessageTraceAsync;

        /// <summary>
        /// returns the availabnility of the transport
        /// </summary>
        bool Available { get; }

        /// <summary>
        /// notify the avalability of the transport
        /// </summary>
        event dMessageTransportAsync<TEnvelope> MessageTransportAvailableAsync;

        /// <summary>
        /// notify the unavalability of the transport
        /// </summary>
        event dMessageTransportAsync<TEnvelope> MessageTransportUnavailableAsync;

        /// <summary>
        /// notify internal exception
        /// </summary>
        event dMessageExceptionAsync<TEnvelope> MessageExceptionAsync;
    }
}