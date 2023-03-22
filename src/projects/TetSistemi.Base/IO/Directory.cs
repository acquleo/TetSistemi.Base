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

namespace TetSistemi.Base.IO
{
    /// <summary>
    /// Implementa metodi di utility con le directory
    /// </summary>
    public class Directory
    {

        #region Static Members

        /// <summary>
        /// Imposta il path passato come working directory corrente
        /// </summary>
        /// <returns>Path</returns>
        public static void SetCurrentDirectory(string path)
        {
            System.IO.Directory.SetCurrentDirectory(path);
        }

        /// <summary>
        /// Imposta il path dell'assembly di esecuzione come working directory corrente
        /// </summary>
        public static void SetCurrentDirectory()
        {
            System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
        }

        /// <summary>
        /// Ritorna il path dell'assembly di esecuzione
        /// </summary>
        /// <returns>Ritorna la stringa con il path della directory corrente.</returns>
        public static string GetCurrentDirectory()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Ritorna il path dell'assembly di esecuzione
        /// </summary>
        /// <returns>Ritorna la stringa con il path della directory corrente.</returns>
        public static string GetTempDirectory()
        {
            return System.IO.Path.GetTempPath();
        }

        #endregion

    }
}
