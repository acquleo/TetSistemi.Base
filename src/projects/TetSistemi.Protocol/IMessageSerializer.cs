
using System;
using System.Data.SqlTypes;

namespace TetSistemi.Protocol
{
    /// <summary>
    /// messsage serializer interface
    /// </summary>
    /// <typeparam name="Tdata"></typeparam>
    public interface IMessageSerializer<Tdata> 
    {
        /// <summary>
        /// convert a message type to data type
        /// </summary>
        /// <param name="msg">Imessage</param>
        /// <param name="contentType">contentType</param>
        /// <returns></returns>
        Tdata Serialize(IMessage msg, string contentType=null);

        /// <summary>
        /// convert a data type to message type
        /// </summary>
        /// <param name="data">trasnport data</param>
        /// <param name="contentType">contentType</param>
        /// <returns></returns>
        IMessage Deserialize(Tdata data, string contentType = null);

        

    }
}