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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NLog; 

namespace TetSistemi.Base.Logger
{
    /// <summary>
    /// Implementa un Logger su un file con interfaccia NLog
    /// </summary>
    public abstract class NLogLogger
    {
        #region Field

        NLog.Logger logger;
        NLogLoggerConfig config;

        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_config">Configurazione</param>
        public NLogLogger(NLogLoggerConfig _config)
        {
            config = _config;
            logger = NLog.LogManager.GetLogger(config.NLogTargetName);

            if (config.DisableAtStartup)
            {
                setLogLevel();
            }
        }

        private void setLogLevel()
        {
            //logger.Factory.DisableLogging();

            IList<NLog.Config.LoggingRule> rules = NLog.LogManager.Configuration.LoggingRules;
            Regex validator = new Regex("(" + config.NLogTargetName + ")");
            foreach (var rule in rules.Where(x => validator.IsMatch(x.LoggerNamePattern)))
            {                
                if (rule.IsLoggingEnabledForLevel(LogLevel.Debug))
                {
                    rule.DisableLoggingForLevel(LogLevel.Debug);
                }
                if (rule.IsLoggingEnabledForLevel(LogLevel.Error))
                {
                    rule.DisableLoggingForLevel(LogLevel.Error);
                }
                if (rule.IsLoggingEnabledForLevel(LogLevel.Fatal))
                {
                    rule.DisableLoggingForLevel(LogLevel.Fatal);
                }
                if (rule.IsLoggingEnabledForLevel(LogLevel.Info))
                {
                    rule.DisableLoggingForLevel(LogLevel.Info);
                }
                if (rule.IsLoggingEnabledForLevel(LogLevel.Trace))
                {
                    rule.DisableLoggingForLevel(LogLevel.Trace);
                }
                if (rule.IsLoggingEnabledForLevel(LogLevel.Warn))
                {
                    rule.DisableLoggingForLevel(LogLevel.Warn);
                }
            }

            //logger.Factory.EnableLogging();
        } 


        #endregion
  
        #region Property

        /// <summary>
        /// Ritorna il logger NLog
        /// </summary>
        protected NLog.Logger Logger
        {
            get
            {
                return this.logger;
            }            
        }

        /// <summary>
        /// Ritorna la configurazione NLog
        /// </summary>
        protected NLogLoggerConfig Config
        {
            get
            {
                return this.config;
            }
        }
        /// <summary>
        /// Lettura del livello NLog.
        /// </summary>
        /// <returns>Restituisce il livello di NLog.</returns>
        protected NLog.LogLevel GetCurrentNLogLevel()
        {
            if (logger.IsDebugEnabled) return NLog.LogLevel.Debug;
            if (logger.IsTraceEnabled) return NLog.LogLevel.Trace;
            if (logger.IsInfoEnabled) return NLog.LogLevel.Info;
            if (logger.IsWarnEnabled) return NLog.LogLevel.Warn;
            if (logger.IsErrorEnabled) return NLog.LogLevel.Error;
            return NLog.LogLevel.Off;
        }
        /// <summary>
        /// Lettura del livello di NLog
        /// </summary>
        /// <param name="tetLevel">Livello di log</param>
        /// <returns>Ritorna il livello di NLog</returns>
        protected NLog.LogLevel GetNlogLevel(LogLevels tetLevel)
        {
            switch (tetLevel)
            {
                case LogLevels.Disabled:
                    {
                        return NLog.LogLevel.Off;
                    }
                case LogLevels.Debug:
                    {
                        return NLog.LogLevel.Debug;
                    }
                case LogLevels.Error:
                    {
                        return NLog.LogLevel.Error;
                    }
                case LogLevels.Info:
                    {
                        return NLog.LogLevel.Info;
                    }
                case LogLevels.Trace:
                    {
                        return NLog.LogLevel.Trace;
                    }
                case LogLevels.Warning:
                    {
                        return NLog.LogLevel.Warn;
                    }
            }

            return NLog.LogLevel.Off;
        }

        protected LogLevels GetTetlogLevel(NLog.LogLevel nlogLevel)
        {
            if (nlogLevel == NLog.LogLevel.Off)
                return LogLevels.Disabled;
            if (nlogLevel == NLog.LogLevel.Debug)
                return LogLevels.Debug;
            if (nlogLevel == NLog.LogLevel.Error)
                return LogLevels.Error;
            if (nlogLevel == NLog.LogLevel.Info)
                return LogLevels.Info;
            if (nlogLevel == NLog.LogLevel.Trace)
                return LogLevels.Trace;
            if (nlogLevel == NLog.LogLevel.Warn)
                return LogLevels.Warning;
            return LogLevels.Disabled;
        }

        /// <summary>
        /// Ritorna se il livello di log specificato è abilitato
        /// </summary>
        /// <param name="level">Livello di Log</param>
        /// <returns>Ritorna lo stato abilitato oppure no.</returns>
        public bool CanLog(LogLevels level)
        {
            switch (level)
            {
                case LogLevels.Disabled:
                    {
                        return false;
                    }
                case LogLevels.Debug:
                    {
                        return logger.IsDebugEnabled;
                    }
                case LogLevels.Error:
                    {
                        return logger.IsErrorEnabled;
                    }
                case LogLevels.Info:
                    {
                        return logger.IsInfoEnabled;
                    }
                case LogLevels.Trace:
                    {
                        return logger.IsTraceEnabled;
                    }
                case LogLevels.Warning:
                    {
                        return logger.IsWarnEnabled;
                    }
            }
            return false;
        }

        #endregion

    }
}