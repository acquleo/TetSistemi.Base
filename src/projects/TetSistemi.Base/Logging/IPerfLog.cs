using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TetSistemi.Base.Logger;

namespace TetSistemi.Base.Logging
{
    /// <summary>
    /// IApplicationLog interface
    /// </summary>
    public interface IPerfLog
    {

        /// <summary>
        /// Writes a log
        /// </summary>
        /// <param name="level">log level</param>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        /// <param name="exception">optional exception object</param>
        void Log(LogLevels level, object caller, string message, Exception exception = null);

        /// <summary>
        /// Writes a log
        /// </summary>
        /// <param name="level">log level</param>
        /// <param name="message">log message</param>
        /// <param name="exception">optional exception object</param>
        void Log(LogLevels level, string message, Exception exception = null);

        /// <summary>
        /// Writes an error log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        /// <param name="exception">optional exception object</param>
        void Error(object caller, string message, Exception exception = null);

        /// <summary>
        /// Writes a warning log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        /// <param name="exception">optional exception object</param>
        void Warning(object caller, string message, Exception exception = null);

        /// <summary>
        /// Writes Fatal log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        /// <param name="exception">optional exception object</param>
        void Fatal(object caller, string message, Exception exception = null);

        /// <summary>
        /// Writes an Info log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        void Info(object caller, string message);

        /// <summary>
        /// Writes a Debug log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        void Debug(object caller, string message);

        /// <summary>
        /// Writes a Trace log
        /// </summary>
        /// <param name="caller">object caller (may be null)</param>
        /// <param name="message">log message</param>
        void Trace(object caller, string message);
    }
}
