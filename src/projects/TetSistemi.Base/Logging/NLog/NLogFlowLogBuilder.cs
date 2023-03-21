using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// Nlog implementation of a FlowLogBuilder
    /// </summary>
    internal class NLogFlowLogBuilder : FlowLogBuilder
    {
        /// <summary>
        /// ctor
        /// </summary>
        public NLogFlowLogBuilder() {

            
        }

        /// <summary>
        /// Returns the Nlog implementation of an IFlowLog
        /// </summary>
        /// <returns></returns>
        public override IFlowLog Build()
        {
            string cName = $@"{this.type}_{this.name}";
            return new NLogFlowLog(NLog.LogManager.GetLogger(cName), cName, this.type, this.system);
        }

    }
}
