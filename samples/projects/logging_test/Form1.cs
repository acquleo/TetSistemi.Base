using System.Reflection;
using TetSistemi.Base.Logging;
using TetSistemi.Base.Logging.Nlog;

namespace LoggingTest
{
    public partial class Form1 : Form
    {
        IApplicationLog log;
        ITraceLog trace;
        IFlowLog flow;
        IPerfLog perf1;
        IPerfLog perf2;


        public Form1()
        {
            TetSistemi.Base.IO.Directory.SetCurrentDirectory(TetSistemi.Base.IO.Directory.GetCurrentDirectory());

            InitializeComponent();

            IApplicationLogFactory factory = new NLogApplicationLogFactory();


            IApplicationLogBuilder builder = factory.GetBuilder();
            IApplicationLog logger = builder.WithObject(this).Build();
            log = factory.GetBuilder().WithObject(this).Build();
            log.Log(LogLevels.Info, "test message");
            ITraceLogFactory factory1 = new NLogTraceLogFactory();
            trace = factory1.GetBuilder().WithObject(this).Build();

            IFlowLogFactory factory2 = new NLogFlowLogFactory();
            flow = factory2.GetBuilder().Build();

            IPerfLogFactory factory3 = new NLogPerfLogFactory();
            perf1 = factory3.GetBuilder().WithCustomName("SNMP_GET").Build();
            perf2 = factory3.GetBuilder().WithCustomName("SNMP_TRAP").Build();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           for(int i=0;i<1000000000;i++)
            {
                writelogs();
                Thread.Sleep(1);
            }
        }

        void writelogs()
        {
            log.Log(LogLevels.Fatal, "--------------------------- SIMPLE TEST ---------------------------");

            log.Log(LogLevels.Fatal, "FATAL simple");
            log.Log(LogLevels.Error, "ERROR simple");
            log.Log(LogLevels.Warning, "WARNING simple");
            log.Log(LogLevels.Info, "INFO simple");
            log.Log(LogLevels.Debug, "DEBUG simple");
            log.Log(LogLevels.Trace, "TRACE simple");

            log.Log(LogLevels.Info, "--------------------------- NEW LINE TEST ---------------------------");

            log.Log(LogLevels.Fatal, $@"FATAL 
test");
            log.Log(LogLevels.Fatal, $@"<tag>
    <subtag/>
</tag>");

            log.Log(LogLevels.Info, "--------------------------- SIMPLE TEST ---------------------------");

            log.Log(LogLevels.Fatal, "FATAL no exception");
            log.Log(LogLevels.Fatal, "FATAL exception", new ArgumentException("nooooo!!", new ArgumentException("inner exception")));

            try
            {
                StartForm(null);
            }
            catch (Exception ex)
            {
                log.Log(LogLevels.Error, "REAL ERROR exception", ex);
            }


            log.Log(LogLevels.Info, "--------------------------- QUOTE TEST ---------------------------");

            log.Log(LogLevels.Info, "Lorem ipsum dolor sit amet, \"consectetur\" adipiscing elit.");


            log.Log(LogLevels.Info, "--------------------------- PARAMETERS TEST ---------------------------");

            log.Log(LogLevels.Debug, string.Format("Lorem ipsum dolor sit {0}, adipiscing {1} elit.", "param1", "param2"));



            trace.Log("127.0.0.1:1234", "10.145.34.21:765", new byte[30], TraceDirections.Output, string.Empty, TetSistemi.Base.Logger.PrintTypeByteArray.Hexadecimal);
            trace.Log("127.0.0.1:1234", "10.145.34.21:765", new byte[30], TraceDirections.Input, string.Empty, TetSistemi.Base.Logger.PrintTypeByteArray.Hexadecimal);

            flow.Log(LogLevels.Info, null, FlowVerbose.V01, "LOGCOMMAND0000001", "PIS-ENGINE", "OPERATOR01", "SEND MESSAGE TO PID", FlowResult.R01, "Lorem ipsum dolor sit amet, consectetur adipiscing elit.");


            perf1.Warning(this, @"SNMP GET TO DEVICE 10.100.34.56 with OID 1.5.6.7.8.9 TIMEOUT");
            perf2.Info(this, @"RECEIVED SNMP TRAP FROM DEVICE 10.100.34.56 with TRAP-OID 1.5.6.7.8.9");
        }

        void StartForm(Form f)
        {
            f.ShowDialog(this);
        }
    }
}