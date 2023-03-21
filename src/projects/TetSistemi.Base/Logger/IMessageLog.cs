// ------------------------------------------------------------------------
//Società: T&TSistemi s.r.l.
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: TetSistemi.Commons.dll
//Data ultima modifica: $LastChangedDate: 2019-01-06 11:09:58 +0100 (Sun, 06 Jan 2019) $
//Versione: $Rev: 675 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TetSistemi.Base.Logger
{    

    /// <summary>
    /// Interfaccia generica di Log di Debug
    /// </summary>
    public interface IMessageLog
    {
        /// <summary>
        /// Inserisce un messaggio di log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="caller">Oggetto chiamante la funzione di log</param>
        /// <param name="message">Messaggio di log</param>
        /// <param name="args">Elementi da formattare sul message</param>
        void Log(LogLevels level, object caller, string message, params object[] args);

        /// <summary>
        /// Inserisce un messaggio di log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="caller">Oggetto chiamante la funzione di log</param>
        /// <param name="message">Messaggio di log</param>
        void Log(LogLevels level, object caller, string message);

        /// <summary>
        /// Inserisce un messaggio di log
        /// </summary>
        /// <param name="level">Livello del log</param>
        /// <param name="message">Messaggio di log</param>
        void Log(LogLevels level, string message);

        /// <summary>
        /// Imposta o Ritorna il livello di Log corrente
        /// </summary>
        LogLevels LogLevel { get; set; }

        /// <summary>
        /// Ritorna se il livello di log specificato è abilitato
        /// </summary>
        /// <param name="level">Livello di Log</param>
        /// <returns>Ritorna lo stato abilitato oppure no.</returns>
        bool CanLog(LogLevels level);
    }
}
