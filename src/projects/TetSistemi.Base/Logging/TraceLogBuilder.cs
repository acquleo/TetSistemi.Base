using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// IFlowLogBuilder implementation
    /// </summary>
    public abstract class TraceLogBuilder : ITraceLogBuilder
    {
        internal const string TYPE = "TRACE";

        internal string type = string.Empty;
        internal string name = string.Empty;

        public TraceLogBuilder() {
            this.type = TYPE;
        }

        /// <summary>
        /// Returns the ITraceLog
        /// </summary>
        /// <returns></returns>
        public abstract ITraceLog Build();

        /// <summary>
        /// Set the class of the object using the logger
        /// </summary>
        /// <param name="clientType"></param>
        /// <returns></returns>
        public ITraceLogBuilder WithClass(Type clientType)
        {
            this.name= clientType.FullName;
            return this;
        }

        /// <summary>
        /// Set a custom logger Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ITraceLogBuilder WithCustomName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            this.name = name;
            return this;
        }

        /// <summary>
        /// Set a custom logger type (default MESSAGE)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ITraceLogBuilder WithCustomType(string type)
        {
            if(string.IsNullOrEmpty(type)) throw new ArgumentNullException("type");

            this.type = type;
            return this;
        }

        /// <summary>
        /// Set the object using the logger
        /// </summary>
        /// <param name="clientObject"></param>
        /// <returns></returns>
        public ITraceLogBuilder WithObject(object clientObject)
        {
            return this.WithClass(clientObject.GetType());
        }
    }
}
