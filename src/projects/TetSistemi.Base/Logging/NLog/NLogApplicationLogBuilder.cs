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
    internal class NLogApplicationLogBuilder: ApplicationLogBuilder
    {
        /// <summary>
        /// ctor
        /// </summary>
        public NLogApplicationLogBuilder() {

            
        }

        /// <summary>
        /// Returns the Nlog implementation of an IApplicationLog
        /// </summary>
        /// <returns></returns>
        public override IApplicationLog Build()
        {
            string cName = $@"{this.type}_{this.name}";
            return new NLogApplicationLog(NLog.LogManager.GetLogger(cName), cName, this.type);
        }

    }
}
