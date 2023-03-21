using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TetSistemi.Base.Class;
using TetSistemi.Base.Logger;

namespace TetSistemi.Base.Logging
{
    /// <summary>
    /// IApplicationLog interface
    /// </summary>
    public class FactoryProvider: ILogFactoryProvider
    {
        IApplicationLogFactory applicationFactory;
        IFlowLogFactory flowFactory;
        ITraceLogFactory traceFactory;


        public void SetApplicationLogFactory(IApplicationLogFactory applicationFactory)
        {
            if (this.applicationFactory != null) throw new InvalidOperationException("factory cannot changed");

            this.applicationFactory = applicationFactory;
        }

        public void SetFlowLogFactory(IFlowLogFactory flowFactory)
        {
            if (this.flowFactory != null) throw new InvalidOperationException("factory cannot changed");

            this.flowFactory = flowFactory;
        }

        public void SetTraceLogFactory(ITraceLogFactory traceFactory)
        {
            if (this.traceFactory != null) throw new InvalidOperationException("factory cannot changed");

            this.traceFactory = traceFactory;
        }

        public IApplicationLogFactory GetApplicationLogFactory()
        {
            return this.applicationFactory;
        }

        public IFlowLogFactory GetFlowLogFactory()
        {
            return this.flowFactory;
        }

        public ITraceLogFactory GetTraceLogFactory()
        {
            return this.traceFactory;
        }
    }
}
