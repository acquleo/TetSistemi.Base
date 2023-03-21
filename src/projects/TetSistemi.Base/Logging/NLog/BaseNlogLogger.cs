using System;
using System.Collections.Generic;
using System.Text;
using TetSistemi.Base.Logger;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// Nlog logger base class
    /// </summary>
    public abstract class BaseNLogLogger
    {
        protected NLog.ILogger logger;
        protected string targetName;
        protected string type;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger">Nlog logger object</param>
        /// <param name="targetName">Log target</param>
        /// <param name="type">Log type</param>
        public BaseNLogLogger(NLog.ILogger logger, string targetName, string type)
        {
            this.logger = logger;
            this.targetName = targetName;
            this.type = type;
        }

        /// <summary>
        /// Converts the library log level to the Nlog level
        /// </summary>
        /// <param name="tetLevel"></param>
        /// <returns></returns>
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
                case LogLevels.Fatal:
                    {
                        return NLog.LogLevel.Fatal;
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
    }
}
