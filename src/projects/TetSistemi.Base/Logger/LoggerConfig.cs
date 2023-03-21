// ------------------------------------------------------------------------
//Società: T&TSistemi s.r.l.
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: TetSistemi.Commons.dll
//Data ultima modifica: $LastChangedDate: 2015-01-20 15:37:29 +0100 (Tue, 20 Jan 2015) $
//Versione: $Rev: 259 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TetSistemi.Base.Logger
{
    /// <summary>
    /// Implementa la classe base intermedia di configurazione di un Logger
    /// </summary>
    public abstract class LoggerConfig : LoggerConfigBase
    {
        #region Private Field
        private LogLevels initialLevel;
        #endregion

        #region Constructor

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_name">Nome del logger</param>
        /// <param name="_initialLevel">Filtro iniziale del livello di log</param>
        public LoggerConfig(string _name, LogLevels _initialLevel): base(_name)
        {
            this.initialLevel = _initialLevel;
        }

        #endregion
               

        #region Properties

       
        /// <summary>
        /// Ritorna il livello di filtro iniziale di Log
        /// </summary>
        public LogLevels InitialLevel
        {
            get
            {
                return this.initialLevel;
            }
        }

        #endregion
    }
}
