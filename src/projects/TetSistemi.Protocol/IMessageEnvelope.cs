using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TetSistemi.Protocol
{
    /// <summary>
    /// Generic message envelope definition
    /// </summary>
    public interface IMessageEnvelope
    {
        /// <summary>
        /// Envelope payload
        /// </summary>
        IMessage Payload { get; }        
    }
}
