using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// Nlog implementation of a TraceLogBuilder
    /// </summary>
    internal class NLogTraceLogBuilder : TraceLogBuilder
    {
        /// <summary>
        /// ctor
        /// </summary>
        public NLogTraceLogBuilder() {

            
        }

        /// <summary>
        /// Returns the Nlog implementation of a ITraceLog
        /// </summary>
        /// <returns></returns>
        public override ITraceLog Build()
        {
            string cName = $@"{this.type}_{this.name}";
            return new NLogTraceLog(NLog.LogManager.GetLogger(cName), cName, this.type);
        }

    }
}
