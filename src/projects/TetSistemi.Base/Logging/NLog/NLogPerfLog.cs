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
    /// Nlog implementation of an IApplicationLog
    /// </summary>
    internal class NLogPerfLog : BaseNLogLogger, IPerfLog
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger">Nlog logger object</param>
        /// <param name="targetName">Log target</param>
        /// <param name="type">Log type</param>
        public NLogPerfLog(NLog.ILogger logger, string targetName, string type) :base(logger,targetName, type)
        { 

        }

        /// <summary>
        /// Writes a Debug log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        public void Debug(object caller, string message)
        {
            this.Log( LogLevels.Debug, caller, message);
        }

        /// <summary>
        /// Writes an error log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        /// <param name="exception">optional exception object</param>
        public void Error(object caller, string message, Exception exception = null)
        {
            this.Log(LogLevels.Error, caller, message, exception);
        }

        /// <summary>
        /// Writes Fatal log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        /// <param name="exception">optional exception object</param>
        public void Fatal(object caller, string message, Exception exception = null)
        {
            this.Log(LogLevels.Fatal, caller, message, exception);
        }

        /// <summary>
        /// Writes an Info log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        public void Info(object caller, string message)
        {
            this.Log(LogLevels.Info, caller, message);
        }

        /// <summary>
        /// Writes a Trace log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        public void Trace(object caller, string message)
        {
            this.Log(LogLevels.Trace, caller, message);
        }

        /// <summary>
        /// Writes a warning log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        /// <param name="exception">optional exception object</param>
        public void Warning(object caller, string message, Exception exception = null)
        {
            this.Log(LogLevels.Warning, caller, message, exception);
        }

        /// <summary>
        /// Writes a log
        /// </summary>
        /// <param name="level">log level</param>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        /// <param name="exception">optional exception object</param>
        public void Log(LogLevels level, object caller, string message, Exception exception = null)
        {
            NLog.LogEventInfo entryLog = new NLog.LogEventInfo(base.GetNlogLevel(level), this.targetName, message);
            //entryLog.
            entryLog.Exception = exception;
            entryLog.Properties["TetLoggerName"] = this.targetName;

            logger.Log(typeof(NLogApplicationLog),entryLog);
        }

        /// <summary>
        /// Writes a log
        /// </summary>
        /// <param name="level">log level</param>
        /// <param name="message">log message</param>
        /// <param name="exception">optional exception object</param>
        public void Log(LogLevels level, string message, Exception exception = null)
        {
            this.Log(level, null, message, exception);
        }        
    }
}
