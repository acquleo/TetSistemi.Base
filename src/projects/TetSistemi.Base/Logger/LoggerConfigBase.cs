using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TetSistemi.Base.Logger
{
    /// <summary>
    /// Implementa la classe base di configurazione di un Logger
    /// </summary>
    public abstract class LoggerConfigBase
    {
        #region Private Field

        private string name;
        #endregion

        #region Constructor
        
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_name">Nome del logger</param>
        public LoggerConfigBase(string _name)
        {
            this.name = _name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Ritorna il nome del logger
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        #endregion
    }
}
