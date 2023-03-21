using System;
using System.Collections.Generic;
using System.Text;

namespace TetSistemi.Base.Logging
{
    /// <summary>
    /// ITraceLogFactory interface
    /// </summary>
    public interface ITraceLogFactory
    {
        /// <summary>
        /// Returns an instance of ITraceLogBuilder
        /// </summary>
        /// <returns></returns>
        ITraceLogBuilder GetBuilder();
    }
}
