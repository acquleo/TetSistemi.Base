using System;
using System.Collections.Generic;
using System.Text;

namespace TetSistemi.Base.Logging
{
    /// <summary>
    /// ITraceLogBuilder interface
    /// </summary>
    public interface ITraceLogBuilder
    {
        /// <summary>
        /// Returns the ITraceLog
        /// </summary>
        /// <returns></returns>
        ITraceLog Build();
        /// <summary>
        /// Set a custom logger Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        ITraceLogBuilder WithCustomName(string name);
        /// <summary>
        /// Set the class of the object using the logger
        /// </summary>
        /// <param name="clientType"></param>
        /// <returns></returns>
        ITraceLogBuilder WithClass(Type clientType);
        /// <summary>
        /// Set the object using the logger
        /// </summary>
        /// <param name="clientObject"></param>
        /// <returns></returns>
        ITraceLogBuilder WithObject(object clientObject);
    }
}
