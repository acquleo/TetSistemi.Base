using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TetSistemi.Protocol
{
    public enum ChunkTypes
    {
        Disabled = 0,
        ByService = 1,
        ElementPerMessage = 2,
        NumOfMessage = 3,
    }

    /// <summary>
    /// Generic message definition
    /// </summary>
    public interface IMessageChunkRequest : IMessageRequest
    {
        
        /// <summary>
        /// Returns che request chunk type
        /// </summary>
        /// <returns></returns>
        ChunkTypes GetChunkType();
        
        /// <summary>
        /// Returns the request chunk size
        /// </summary>
        /// <returns></returns>
        uint GetChunkSize();
        
    }
}
