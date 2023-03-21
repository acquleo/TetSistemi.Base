// ------------------------------------------------------------------------
//Società: T&TSistemi s.r.l.
//Anno: 2008 
//Progetto: AMIL5
//Autore: Papi Rudy
//Nome modulo software: TetSistemi.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TetSistemi.Base.Logger
{
    /// <summary>
    /// Implementa un LoggerConfig per il logger NLogLogger
    /// </summary>
    public abstract class NLogLoggerConfig : LoggerConfig , ILoggerConfig
    {
        #region Field

        private string nlogTargetName;
        bool disableAtStartup = false;

        #endregion

        #region Constructor
        /// <summary>
        /// Configurazione NLogLoggerConfig.
        /// </summary>
        /// <param name="_loggerName">Nome del logger</param>
        /// <param name="_nlogTargetName">Nome del logger NLog</param>
        /// <param name="_disableAtStartup">Impostazione di stato disabilitato all'avvio.</param>
        public NLogLoggerConfig(string _loggerName, string _nlogTargetName, bool _disableAtStartup)
            : base(_loggerName,  LogLevels.Disabled)
        {
            this.nlogTargetName = _nlogTargetName;
            this.disableAtStartup = _disableAtStartup;
        }

        #endregion

        #region Property

        /// <summary>
        /// Ritorna se il log si auto disabilita alla prima creazione
        /// </summary>
        public bool DisableAtStartup
        {
            get
            {
                return disableAtStartup;
            }
        }

        /// <summary>
        /// Ritorna il nome del target NLog
        /// </summary>
        public string NLogTargetName
        {
            get
            {
                return this.nlogTargetName;
            }
        }

        #endregion

        #region  Members

        /// <summary>
        /// Crea un'istanza di NLogTraceLoggerConfig relativa al LoggerConfig corrente
        /// </summary>
        /// <returns>Restituisce null</returns>
        public virtual object Create()
        {
            return null;
            //return new CircularFileMessageLogger(this);
        }
        #endregion
    }
}
