using System;
using System.Collections.Generic;
using System.Text;

namespace TetSistemi.Base.Logging
{
    /// <summary>
    /// IApplicationLogBuilder interface
    /// </summary>
    public interface IApplicationLogBuilder
    {
        /// <summary>
        /// Returns the IApplicationLog
        /// </summary>
        /// <returns></returns>
        IApplicationLog Build();
        /// <summary>
        /// Set a custom logger Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        IApplicationLogBuilder WithCustomName(string name);
        /// <summary>
        /// Set the class of the object using the logger
        /// </summary>
        /// <param name="clientType"></param>
        /// <returns></returns>
        IApplicationLogBuilder WithClass(Type clientType);
        /// <summary>
        /// Set the object using the logger
        /// </summary>
        /// <param name="clientObject"></param>
        /// <returns></returns>
        IApplicationLogBuilder WithObject(object clientObject);
    }
}
