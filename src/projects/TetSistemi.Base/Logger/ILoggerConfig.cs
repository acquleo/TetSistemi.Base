// ------------------------------------------------------------------------
//Società: T&TSistemi s.r.l.
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: TetSistemi.Commons.dll
//Data ultima modifica: $LastChangedDate: 2015-12-14 15:47:15 +0100 (Mon, 14 Dec 2015) $
//Versione: $Rev: 425 $
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
    public interface ILoggerConfig
    {
        /// <summary>
        /// Nome del logger.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Implementa l'interfaccia per creare un instanza dell'oggetto logger.
        /// </summary>
        /// <returns>Oggetto logger</returns>
        object Create();

    }
}
