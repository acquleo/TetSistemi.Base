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
    /// Implementa un LoggerConfig per il logger CircularFileLogger
    /// </summary>
    public class NLogMessageLoggerConfig : NLogLoggerConfig , ILoggerConfig
    {
        #region Field
       
        #endregion

        #region Constructor
        /// <summary>
        /// Configurazione NLogLoggerConfig.
        /// </summary>
        /// <param name="_loggerName">Nome del logger</param>
        /// <param name="_nlogTargetName">Nome del logger NLog</param>
        /// <param name="_disableAtStartup">Impostazione di stato disabilitato all'avvio.</param>
        public NLogMessageLoggerConfig(string _loggerName, string _nlogTargetName,bool _disableAtStartup)
            : base(_loggerName, _nlogTargetName, _disableAtStartup)
        {
        
        }

        #endregion

        #region Property

        #endregion

        #region  Members

        /// <summary>
        /// Crea un'istanza di NLogTraceLoggerConfig relativa al LoggerConfig corrente
        /// </summary>
        /// <returns>Restituisce l'oggetto NLogMessageLogger</returns>
        public override object Create()
        {            
            return new NLogMessageLogger(this);
        }
        #endregion
    }
}
