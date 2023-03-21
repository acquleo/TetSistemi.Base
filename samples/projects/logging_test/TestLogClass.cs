using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetSistemi.Base.Logging;
using TetSistemi.Base.Logging.Nlog;

namespace LoggingTest
{
    
    public class TestLogClass
    {
        IApplicationLog? log;
        ITraceLog? trace;
        // CONSTRUCTOR
        public TestLogClass(ILogFactoryProvider appLogFactory)
        {
            log = appLogFactory?.GetApplicationLogFactory().GetBuilder()?.WithObject(this)?.Build();
            trace = appLogFactory?.GetTraceLogFactory().GetBuilder()?.WithObject(this)?.Build();
        }        
    }



    public class StartupClass
    {
        public StartupClass()
        {
            SingletonFactoryProvider.Provider.SetApplicationLogFactory(new NLogApplicationLogFactory());
        }
    }
}
