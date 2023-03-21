using System;
using System.Collections.Generic;
using System.Text;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// Nlog implementation of IApplicationLogFactory
    /// </summary>
    public class NLogApplicationLogFactory : IApplicationLogFactory
    {
        /// <summary>
        /// Returns an Nlog implementation of IApplicationLogBuilder
        /// </summary>
        /// <returns></returns>
        public IApplicationLogBuilder GetBuilder()
        {
            return new NLogApplicationLogBuilder();
        }
    }
}
