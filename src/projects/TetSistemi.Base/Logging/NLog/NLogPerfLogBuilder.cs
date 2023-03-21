using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// Nlog implementation of an ApplicationLogBuilder
    /// </summary>
    internal class NLogPerfLogBuilder : PerfLogBuilder
    {
        /// <summary>
        /// ctor
        /// </summary>
        public NLogPerfLogBuilder() {

            
        }

        /// <summary>
        /// Returns the Nlog implementation of an IApplicationLog
        /// </summary>
        /// <returns></returns>
        public override IPerfLog Build()
        {
            string cName = $@"{this.type}_{this.name}";
            return new NLogPerfLog(NLog.LogManager.GetLogger(cName), cName, this.type);
        }

    }
}
