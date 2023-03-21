using System;
using System.Collections.Generic;
using System.Text;

namespace TetSistemi.Base.Logging
{
    /// <summary>
    /// IFlowLogFactory interface
    /// </summary>
    public interface IFlowLogFactory
    {
        /// <summary>
        /// Returns an instance of IFlowLogBuilder
        /// </summary>
        /// <returns></returns>
        IFlowLogBuilder GetBuilder();
    }
}
