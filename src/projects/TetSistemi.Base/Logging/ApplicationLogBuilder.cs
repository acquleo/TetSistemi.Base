using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// IApplicationLogBuilder implementation
    /// </summary>
    public abstract class ApplicationLogBuilder : IApplicationLogBuilder
    {
        internal const string TYPE = "MESSAGE";

        internal string type = string.Empty;
        internal string name = string.Empty;

        public ApplicationLogBuilder() {
            this.type = TYPE;
        }

        /// <summary>
        /// Returns the IApplicationLog
        /// </summary>
        /// <returns></returns>
        public abstract IApplicationLog Build();

        /// <summary>
        /// Set the class of the object using the logger
        /// </summary>
        /// <param name="clientType"></param>
        /// <returns></returns>
        public IApplicationLogBuilder WithClass(Type clientType)
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
        public IApplicationLogBuilder WithCustomName(string name)
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
        public IApplicationLogBuilder WithObject(object clientObject)
        {
            return this.WithClass(clientObject.GetType());
        }
    }
}
