// ------------------------------------------------------------------------
//Società: T&TSistemi s.r.l.
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: TetSistemi.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using System.Diagnostics;
using System.Reflection;

namespace TetSistemi.Base.Logger
{
    /// <summary>
    /// Implementa un Logger su un file con interfaccia NLog
    /// </summary>
    public class NLogTraceLogger : NLogLogger, ITraceLog
    {
        /// <summary>
        /// Ritorna un ITraceLog dato il nome specificato
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ITraceLog GetLogger(string name)
        {
            string cName = "TRACE_" + name;
            NLogTraceLoggerConfig config = new NLogTraceLoggerConfig(cName, cName, false);
            return config.Create() as ITraceLog;
        }

        /// <summary>
        /// Returns an ITraceLog using the object namespace
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ITraceLog GetLogger(object obj)
        {
            Type myType = obj.GetType();
            string name = myType.FullName;
            string cName = "TRACE_" + name;
            NLogTraceLoggerConfig config = new NLogTraceLoggerConfig(cName, cName, false);
            return config.Create() as ITraceLog;
        }

        /// <summary>
        /// Ritorna un ITraceLog dato il nome specificato
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ITraceLog GetLogger(string prefix, object obj)
        {
            Type myType = obj.GetType();
            string name = myType.FullName;
            string cName = string.IsNullOrEmpty(prefix) ? name : prefix + "_" + name;
            NLogTraceLoggerConfig config = new NLogTraceLoggerConfig(cName, cName, false);
            return config.Create() as ITraceLog;
        }

        /// <summary>
        /// Ritorna un ITraceLog dato il nome specificato
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ITraceLog GetLogger(string prefix, string name)
        {
            string cName = string.IsNullOrEmpty(prefix) ? name : prefix + "_" + name;
            NLogTraceLoggerConfig config = new NLogTraceLoggerConfig(cName, cName, false);
            return config.Create() as ITraceLog;
        }

        #region Field

        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_config">Configurazione</param>
        public NLogTraceLogger(NLogLoggerConfig _config)
            : base(_config)
        {
            
        }

        #endregion
  
        #region Property       

        #endregion


        #region ITraceLog Members
        /// <summary>
        /// Metodo per la scrittura del log tracer
        /// </summary>
        /// <param name="_currentDevice">Dispositivo locale</param>
        /// <param name="_remoteDevice">Dispositivo remoto</param>
        /// <param name="_data">Array di byte</param>
        /// <param name="_direction">Direzione del messaggio</param>
        /// <param name="_description">Descrizione del messaggio</param>
        /// <param name="_printTypeByteArray">Formattazione di stampa</param>
        public void Log(string _currentDevice, string _remoteDevice, byte[] _data, TraceDirections _direction, string _description, PrintTypeByteArray _printTypeByteArray)
        {
            NLog.LogEventInfo entryLog = new NLog.LogEventInfo(base.GetNlogLevel(LogLevels.Trace), base.Config.NLogTargetName, _description);

            entryLog.Properties["TetLoggerName"] = base.Config.Name;
            entryLog.Properties["CurrentDevice"] = _currentDevice;
            entryLog.Properties["RemoteDevice"] = _remoteDevice;
            entryLog.Properties["Direction"] = _direction.ToString();
            entryLog.Properties["Array"] = new ByteArrayFormatter(_data , _printTypeByteArray);

            base.Logger.Log(entryLog);
        }

        #endregion

        #region Internal Class
        
        /// <summary>
        /// Implementa le funzionalità della classe ByteArrayFormatter
        /// </summary>
        public class ByteArrayFormatter
        {
            byte[] data;
            PrintTypeByteArray printTypeByteArray;
            /// <summary>
            /// Costruttore.
            /// </summary>
            /// <param name="_data">Byte array da formattare</param>
            /// <param name="_printTypeByteArray">Formattazione</param>
            public ByteArrayFormatter(byte[] _data, PrintTypeByteArray _printTypeByteArray)
            {
                printTypeByteArray = _printTypeByteArray;
                data = _data;
            }
            /// <summary>
            /// Ovverride del metodo ToString
            /// </summary>
            /// <returns>Ritorna la conversione in stringa</returns>
            public override string ToString()
            {
                switch (printTypeByteArray)
                {
                    case PrintTypeByteArray.UTF8:
                        return System.Text.ASCIIEncoding.UTF8.GetString(data);
                }

                StringBuilder byteArrayStr = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    byte b = data[i];

                    switch (printTypeByteArray)
                    {
                        case PrintTypeByteArray.Ascii:
                            byteArrayStr.Append(Convert.ToChar(b));
                            break;
                        case PrintTypeByteArray.Hexadecimal:
                            byteArrayStr.AppendFormat("{0:x2}", b);
                            break;
                        case PrintTypeByteArray.Decimal:
                            byteArrayStr.AppendFormat("{0:D}", b);
                            break;
                        default:
                            break;
                    }

                    if (i < (data.Length-1))
                    {
                        byteArrayStr.Append("-");
                    }
                }

                return byteArrayStr.ToString();
            }
        }

        #endregion
    }
}