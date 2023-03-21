using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetSistemi.Protocol.Exceptions
{
    /// <summary>
    /// base protocol exception
    /// </summary>
    public abstract class ProtocolException : Exception
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="message"></param>
        public ProtocolException(string message) : base(message) { }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerexception"></param>
        public ProtocolException(string message, Exception innerexception) : base(message, innerexception) { }
    }

    /// <summary>
    /// ArgumentProtocolException
    /// </summary>
    public class ArgumentProtocolException : ProtocolException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ArgumentProtocolException(string message):base(message){}
    }

    /// <summary>
    /// InternalProtocolException
    /// </summary>
    public class InternalProtocolException : ProtocolException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerexception"></param>
        public InternalProtocolException(string message, Exception innerexception) : base(message, innerexception) { }
    }

    /// <summary>
    /// SerializerNotFoundException
    /// </summary>
    public class SerializerNotFoundException : ProtocolException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SerializerNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// SerializeException
    /// </summary>
    public class SerializeException : ProtocolException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerexception"></param>
        public SerializeException(string message, Exception innerexception) : base(message, innerexception) { }
    }

    /// <summary>
    /// DeSerializeException
    /// </summary>
    public class DeSerializeException : ProtocolException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerexception"></param>
        public DeSerializeException(string message, Exception innerexception) : base(message, innerexception) { }
    }

}
