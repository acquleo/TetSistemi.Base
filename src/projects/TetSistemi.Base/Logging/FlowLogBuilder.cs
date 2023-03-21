using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TetSistemi.Base.Logging.Nlog
{
    /// <summary>
    /// IFlowLogBuilder implementation
    /// </summary>
    public abstract class FlowLogBuilder : IFlowLogBuilder
    {
        internal const string TYPE = "FLOW";

        internal string system = string.Empty;
        internal string type = string.Empty;
        internal string name = string.Empty;
        
        public FlowLogBuilder()
        {
            this.type = TYPE;
        }

        /// <summary>
        /// Returns the IFlowLog
        /// </summary>
        /// <returns></returns>
        public abstract IFlowLog Build();

        /// <summary>
        /// Set a custom logger Name
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IFlowLogBuilder WithSystem(string system)
        {
            if(string.IsNullOrWhiteSpace(system)) { throw new ArgumentNullException("system"); }

            this.system = system;

            return this;
        }
    }
}
