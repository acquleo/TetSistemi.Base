
namespace TetSistemi.Protocol
{
    /// <summary>
    /// messsage envelope interface
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TEnvelope"></typeparam>
    public interface IMessageEnveloper<TData,TEnvelope> 
        where TEnvelope : IMessageEnvelope
    {
        /// <summary>
        /// unwrap the transport data from the envelope
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        TData Unwrap(TEnvelope msg);

        /// <summary>
        /// wrap the transport data to the envelope
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        TEnvelope Wrap(TData data);
        
    }
}