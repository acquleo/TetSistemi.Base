using System;
using System.Collections.Generic;
using System.Text;

namespace TetSistemi.Base.Logging
{
    /// <summary>
    /// IFlowLogBuilder interface
    /// </summary>
    public interface IFlowLogBuilder
    {
        /// <summary>
        /// Returns the instance of IFlowLog
        /// </summary>
        /// <returns></returns>
        IFlowLog Build();

        /// <summary>
        /// Set a custom logger Name
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        IFlowLogBuilder WithSystem(string system);
    }
}
