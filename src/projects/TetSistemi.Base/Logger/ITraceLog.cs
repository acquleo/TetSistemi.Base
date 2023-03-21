// ------------------------------------------------------------------------
//Società: T&TSistemi s.r.l.
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: TetSistemi.Commons.dll
//Data ultima modifica: $LastChangedDate: 2019-04-05 10:34:41 +0200 (Fri, 05 Apr 2019) $
//Versione: $Rev: 687 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TetSistemi.Base.Logger
{
    #region Enum

    /// <summary>
    /// Enumera le formattazioni di stampa
    /// </summary>
    public enum PrintTypeByteArray { 
        /// <summary>
        /// ASCII
        /// </summary>
        Ascii,
        /// <summary>
        /// Esadecimale
        /// </summary>
        Hexadecimal, 
        /// <summary>
        /// Decimale senza segno
        /// </summary>
        Decimal,
        /// <summary>
        /// UTF8
        /// </summary>
        UTF8
    };

    #endregion

    /// <summary>
    /// Enumera le direzioni dei dati in uno stream
    /// </summary>
    public enum TraceDirections
    {
        /// <summary>
        /// Dati in ingresso
        /// </summary>
        Input = 0,
        /// <summary>
        /// Dati in uscita
        /// </summary>
        Output = 1,        

    }

    /// <summary>
    /// Interfaccia generica di Log di uno stream di dati
    /// </summary>
    public interface ITraceLog
    {

        /// <summary>
        /// Inserisce un log di un flusso di dati
        /// </summary>
        /// <param name="_currentDevice">Dispositivo locale</param>
        /// <param name="_remoteDevice">Dispositivo remoto</param>
        /// <param name="_data">Dati da storicizzare</param>
        /// <param name="_direction">Direzione dei dati rispetto al Dispositivo locale</param>
        /// <param name="_description">Descrizione aggiuntiva</param>
        /// <param name="_printTypeByteArray">Formattazione di stampa</param>
        void Log(string _currentDevice, string _remoteDevice, byte[] _data, TraceDirections _direction,
            string _description, PrintTypeByteArray _printTypeByteArray);
        
    }
}
