using NLog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using TetSistemi.Base.Logger;
using static TetSistemi.Base.Logger.NLogTraceLogger;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// Nlog implementation of a ITraceLog
    /// </summary>
    internal class NLogTraceLog : BaseNLogLogger, ITraceLog
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger">Nlog logger object</param>
        /// <param name="targetName">Log target</param>
        /// <param name="type">Log type</param>
        public NLogTraceLog(NLog.ILogger logger, string targetName, string type) :base(logger,targetName, type)
        { 
            
        }

        /// <summary>
        /// Writes a data trace log
        /// </summary>
        /// <param name="_currentDevice">Local endpoint</param>
        /// <param name="_remoteDevice">Remote endpoint</param>
        /// <param name="_data">Data log</param>
        /// <param name="_direction">Trace direction</param>
        /// <param name="_description">optional description</param>
        /// <param name="_printTypeByteArray">byte array format type</param>
        public void Log(string _currentDevice, string _remoteDevice, byte[] _data, TraceDirections _direction, string _description, PrintTypeByteArray _printTypeByteArray)
        {
            this.Log(_currentDevice, _remoteDevice,
                new ByteArrayFormatter(_data, _printTypeByteArray).ToString(),
                _direction, _description);
        }

        /// <summary>
        /// Writes a data trace log
        /// </summary>
        /// <param name="_currentDevice">Local endpoint</param>
        /// <param name="_remoteDevice">Remote endpoint</param>
        /// <param name="_data">Data log</param>
        /// <param name="_direction">Trace direction</param>
        /// <param name="_description">Optional description</param>
        public void Log(string _currentDevice, string _remoteDevice, string _data, TraceDirections _direction, string _description)
        {
            NLog.LogEventInfo entryLog = new NLog.LogEventInfo(base.GetNlogLevel(LogLevels.Trace), this.targetName, _description);

            entryLog.Properties["TetLoggerName"] = this.targetName;
            entryLog.Properties["CurrentDevice"] = _currentDevice;
            entryLog.Properties["RemoteDevice"] = _remoteDevice;
            entryLog.Properties["Direction"] = _direction.ToString();
            entryLog.Properties["Array"] = _data;

            this.logger.Log(entryLog);
        }
    }
}
