using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TetSistemi.Protocol
{
    /// <summary>
    /// Generic message definition
    /// </summary>
    public interface IMessageRef : IMessage
    {
        /// <summary>
        /// Returns the data used as request respone reference
        /// </summary>
        /// <returns></returns>
        object GetMsgRef();
        

    }
}
