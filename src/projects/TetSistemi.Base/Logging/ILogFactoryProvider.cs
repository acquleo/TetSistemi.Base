using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TetSistemi.Base.Logger;

namespace TetSistemi.Base.Logging
{
    /// <summary>
    /// ILogFactoryProvider interface
    /// </summary>
    public interface ILogFactoryProvider
    {

        IApplicationLogFactory GetApplicationLogFactory();

        IFlowLogFactory GetFlowLogFactory();

        ITraceLogFactory GetTraceLogFactory();
    }
}
