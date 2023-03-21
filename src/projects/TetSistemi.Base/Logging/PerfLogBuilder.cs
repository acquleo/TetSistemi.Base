using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// IApplicationLogBuilder implementation
    /// </summary>
    public abstract class PerfLogBuilder : IPerfLogBuilder
    {
        internal const string TYPE = "PERF";

        internal string type = string.Empty;
        internal string name = string.Empty;

        public PerfLogBuilder() {
            this.type = TYPE;
        }

        /// <summary>
        /// Returns the IApplicationLog
        /// </summary>
        /// <returns></returns>
        public abstract IPerfLog Build();

        /// <summary>
        /// Set the class of the object using the logger
        /// </summary>
        /// <param name="clientType"></param>
        /// <returns></returns>
        public IPerfLogBuilder WithClass(Type clientType)
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
        public IPerfLogBuilder WithCustomName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            this.name = name;
            return this;
        }

        /// <summary>
        /// Set the object using the logger
        /// </summary>
        /// <param name="clientObject"></param>
        /// <returns></returns>
        public IPerfLogBuilder WithObject(object clientObject)
        {
            return this.WithClass(clientObject.GetType());
        }
    }
}
