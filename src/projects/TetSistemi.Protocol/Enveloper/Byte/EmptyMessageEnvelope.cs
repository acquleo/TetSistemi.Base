using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TetSistemi.Protocol.Enveloper.Byte
{
    /// <summary>
    /// Generic message envelope definition
    /// </summary>
    public class EmptyMessageEnvelope : IMessageEnvelope
    {
        /// <summary>
        /// envelope message
        /// </summary>
        public IMessage Payload { get; set; }
    }
}
