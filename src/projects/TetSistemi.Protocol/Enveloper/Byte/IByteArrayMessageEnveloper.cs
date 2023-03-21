using TetSistemi.Protocol.Enveloper.Byte;

namespace TetSistemi.Protocol.Converter.BYTE
{
    /// <summary>
    /// IMessageEnveloper specialized in byte stream
    /// </summary>
    public interface IByteArrayMessageEnveloper : IMessageEnveloper<byte[],EmptyMessageEnvelope> 
    {
        /// <summary>
        /// Returns true if can read length from the input data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool CanReadLength(byte[] data);
        /// <summary>
        /// Returns the message length from the input data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        uint GetLength(byte[] data);
    }
}