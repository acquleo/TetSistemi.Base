using NLog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using TetSistemi.Base.Logger;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// Nlog implementation of an IFlowLog
    /// </summary>
    internal class NLogFlowLog : BaseNLogLogger, IFlowLog
    {
        string system;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger">Nlog logger object</param>
        /// <param name="targetName">Log target</param>
        /// <param name="type">Log type</param>
        public NLogFlowLog(NLog.ILogger logger, string targetName, string type,string system) :base(logger,targetName, type)
        { 
            this.system = system;
        }

        /// <summary>
        /// Writes a flow log
        /// </summary>
        /// <param name="level">nlog level</param>
        /// <param name="caller">object caller</param>
        /// <param name="system">flow system</param>
        /// <param name="verbose">flow verbose level</param>
        /// <param name="logid">flow log id</param>
        /// <param name="logsource">flow log source</param>
        /// <param name="user">flow user</param>
        /// <param name="action">flow action</param>
        /// <param name="result">flow result</param>
        /// <param name="message">flow message</param>
        public void Log(LogLevels level, object caller, 
            FlowVerbose verbose, string logid, string logsource, 
            string user, string action, FlowResult result, string message)
        {
            NLog.LogEventInfo entryLog = new NLog.LogEventInfo(base.GetNlogLevel(level), this.targetName, message);

            entryLog.Properties["System"] = this.system; 
            entryLog.Properties["Verbose"] = verbose;
            entryLog.Properties["LogId"] = logid;
            entryLog.Properties["LogSource"] = logsource;
            entryLog.Properties["User"] = user;
            entryLog.Properties["Action"] = action;
            entryLog.Properties["Result"] = result.ToString();

            logger.Log(typeof(NLogApplicationLog), entryLog);
        }
    }
}
