using System;
using System.Collections.Generic;
using System.Text;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// Nlog implementation of ITraceLogFactory
    /// </summary>
    public class NLogTraceLogFactory : ITraceLogFactory
    {
        /// <summary>
        /// Returns an Nlog implementation of ITraceLogBuilder
        /// </summary>
        /// <returns></returns>
        public ITraceLogBuilder GetBuilder()
        {
            return new NLogTraceLogBuilder();
        }
    }
}
