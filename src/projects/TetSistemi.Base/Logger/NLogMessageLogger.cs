// ------------------------------------------------------------------------
//Società: T&TSistemi s.r.l.
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: TetSistemi.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using System.Diagnostics;
using System.Reflection;

namespace TetSistemi.Base.Logger
{
    /// <summary>
    /// Implementa un Logger su un file con interfaccia NLog
    /// </summary>
    public class NLogMessageLogger : NLogLogger, IMessageLog
    {
        #region Constants
        const string DefaultLoggerName = "TetSistemi.Commons.Logger";
        #endregion

        #region Default Singleton

        /// <summary>
        /// Ritorna un NLogMessageLogger dato il nome specificato
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IMessageLog GetLogger(string name)
        {
            string cName = "MESSAGE_" + name;
            NLogMessageLoggerConfig config = new NLogMessageLoggerConfig(cName, cName, false);
            return config.Create() as IMessageLog;
        }

        /// <summary>
        /// Ritorna un NLogMessageLogger dato il nome specificato
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IMessageLog GetLogger(string prefix, string name)
        {
            string cName = string.IsNullOrEmpty(prefix) ? name : prefix + "_" + name;
            NLogMessageLoggerConfig config = new NLogMessageLoggerConfig(cName, cName, false);
            return config.Create() as IMessageLog;
        }

        /// <summary>
        /// Ritorna un NLogMessageLogger dato il nome specificato
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IMessageLog GetLogger(string prefix, object obj)
        {
            Type myType = obj.GetType();
            string name = myType.FullName;
            string cName = string.IsNullOrEmpty(prefix) ? name : prefix + "_" + name;
            NLogMessageLoggerConfig config = new NLogMessageLoggerConfig(cName, cName, false);
            return config.Create() as IMessageLog;

        }
        /// <summary>
        /// Returns an IMessageLog using the object namespace
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IMessageLog GetLogger(object obj)
        {
            Type myType = obj.GetType();
            string name = myType.FullName;
            string cName = "MESSAGE_" + name;
            NLogMessageLoggerConfig config = new NLogMessageLoggerConfig(cName, cName, false);
            return config.Create() as IMessageLog;
        }

        #endregion

        #region Field

        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_config">Configurazione</param>
        public NLogMessageLogger(NLogLoggerConfig _config)
            : base(_config)
        {
         
        }

        #endregion
  
        #region Property

        #endregion

        #region ITraceLog Members

        #endregion

        #region IMessageLog Members

        /// <summary>
        /// Inserisce un messaggio di log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="caller">Oggetto chiamante la funzione di log</param>
        /// <param name="message">Messaggio di log</param>
        public void Log(LogLevels level, object caller, string message)
        {
            if (!CanLog(level))
            {
                return;
            }

            //Called Method
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase method = stackFrame.GetMethod();
            string methodName = method.Name;
            if (methodName.Equals("Log"))
            {
                stackFrame = stackTrace.GetFrame(2);
                method = stackFrame.GetMethod();
                methodName = method.Name;
            }

            string nm = string.Empty;
            string sn = string.Empty;

            if (caller != null)
            {
                nm=caller.GetType().Namespace;
                sn = caller.GetType().Name;
            }


            NLog.LogEventInfo entryLog = new NLog.LogEventInfo(base.GetNlogLevel(level), base.Config.NLogTargetName, message);

            entryLog.Properties["SenderNamespace"] = nm;
            entryLog.Properties["SenderName"] = sn;
            entryLog.Properties["MethodName"] = methodName;
            entryLog.Properties["TetLoggerName"] = base.Config.Name;
            
            base.Logger.Log( entryLog);
        }

        /// <summary>
        /// Inserisce un messaggio di log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="message">Messaggio di log</param>
        public void Log(LogLevels level, string message)
        {
            Log(level, null, message);
        }

        /// <summary>
        /// Inserisce un messaggio di log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="caller">Oggetto chiamante la funzione di log</param>
        /// <param name="message">Messaggio di log</param>
        /// <param name="args">Elementi da formattare sul message</param>
        public void Log(LogLevels level, object caller, string message, params object[] args)
        {
            string text = string.Format(message, args);
            Log(level, caller, text);
        }

        /// <summary>
        /// Livello di log
        /// </summary>
        public LogLevels LogLevel
        {
            get
            {
                return base.GetTetlogLevel(base.GetCurrentNLogLevel());                
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}