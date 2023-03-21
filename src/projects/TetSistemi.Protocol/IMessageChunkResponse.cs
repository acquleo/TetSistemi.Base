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
    public interface IMessageChunkResponse : IMessageResponse
    {
                
        /// <summary>
        /// Returns the response message number
        /// </summary>
        /// <returns></returns>
        uint GetMsgNum();

        /// <summary>
        /// Returns the total response message
        /// </summary>
        /// <returns></returns>
        uint GetMsgTot();

    }
}
