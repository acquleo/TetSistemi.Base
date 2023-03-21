using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TetSistemi.Base.Interfaces;
using TetSistemi.Protocol.Transport.Data.Endpoint;

namespace TetSistemi.Protocol.Transport.Byte.Pipe
{

    /// <summary>
    /// Client NamedPipe
    /// </summary>
    public class NamedPipeStreamClient : IDataTransport<byte[]>, IEnabler,
        IDisposable
    {
        #region Private fields
        NamedPipeClientStream pipeStream;
        string pipeName;
        string serverName;
        bool isEnabled;
        PipeStateObject stateObj;
        AsyncCallback m_pfnCallBack;
        bool isConnected;
        Thread checkConnectionThread;
        ManualResetEvent firstConnectEvent;
        TimeSpan checkConnectionTime = TimeSpan.FromSeconds(1);
        TimeSpan connectTimeout = TimeSpan.FromSeconds(1);
        int bufferSize = 1024;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverName">Server Name</param>
        /// <param name="name">Pipe Name</param>
        public NamedPipeStreamClient(string serverName, string name)
            :this(name)
        {
            firstConnectEvent = new ManualResetEvent(false);
            this.pipeName = name;
            this.serverName = serverName;
            m_pfnCallBack = new AsyncCallback(OnDataReceived);

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Pipe Name</param>
        public NamedPipeStreamClient(string name)
        {
            firstConnectEvent = new ManualResetEvent(false);
            this.pipeName = name;
            this.serverName = ".";
            m_pfnCallBack = new AsyncCallback(OnDataReceived);

        }
        #endregion

        #region IConnectionStatus

        /// <summary>
        /// Returns the connection status
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }

        /// <summary>
        /// Sets or gets the Check connection time
        /// </summary>
        public TimeSpan CheckConnectionTime
        {
            get
            {
                return checkConnectionTime;
            }

            set
            {
                checkConnectionTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the buffer size
        /// </summary>
        public int BufferSize
        {
            get
            {
                return bufferSize;
            }

            set
            {
                bufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the connect timeout
        /// </summary>
        public TimeSpan ConnectTimeout
        {
            get
            {
                return connectTimeout;
            }

            set
            {
                connectTimeout = value;
            }
        }
        #endregion

        #region Events
        public event dDataReceivedAsync<byte[]> DataReceivedAsync;
        public event dDataTransportAsync<byte[]> DataTransportAvailable;
        public event dDataTransportAsync<byte[]> DataTransportUnavailable;
        public event dDataTransportTraceAsync<byte[]> DataTransportTraceAsync;
        #endregion

        #region Private method
        private void CheckConnectionThread()
        {
            try
            {
                while (true)
                {
                    if (!this.IsConnected)
                    {
                        try
                        {
                            this.InitPipe();

                            pipeStream.Connect((int)this.ConnectTimeout.TotalMilliseconds);

                            OnConnected();
                            
                            this.WaitForData();
                        }
                        catch(IOException)
                        {
                            OnDisconnected();
                        }
                        catch(TimeoutException)
                        {
                            OnDisconnected();
                        }                        
                    }

                    Thread.Sleep(checkConnectionTime);
                }
            }
            catch (ThreadInterruptedException) { }
        }

        private void OnDataReceived(IAsyncResult asyn)
        {
            PipeStateObject theSSO = (PipeStateObject)asyn.AsyncState;

            try
            {                
                //  Determina il numero di byte ricevuti.
                int iRx = theSSO.pipe.EndRead(asyn);
                if (iRx > 0)
                {
                    byte[] dataReceive = new byte[iRx];
                    Array.Copy(theSSO.buffer, dataReceive, iRx);

                    FireOnReceive(dataReceive);

                    this.WaitForData();
                }
                else
                {
                    this.OnDisconnected();
                }
            }
            catch(IOException ioEx)
            {
                this.OnDisconnected();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void InitPipe()
        {
            if (pipeStream != null)
                pipeStream.Dispose();

            pipeStream = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut,
                         PipeOptions.Asynchronous, System.Security.Principal.TokenImpersonationLevel.None,
                          System.IO.HandleInheritability.Inheritable);

            stateObj = new PipeStateObject(pipeStream, this.BufferSize);

        }

        private void WaitForData()
        {
            pipeStream.BeginRead(stateObj.buffer, 0, stateObj.BufferSize, m_pfnCallBack, stateObj);
        }

        private async void FireOnReceive(byte[] _data)
        {

            if (DataReceivedAsync != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                await Task.WhenAll(Array.ConvertAll(
                         DataReceivedAsync.GetInvocationList(),
                         e => ((dDataReceivedAsync<byte[]>)e)(this, _data, GetRemoteEndpoint()))).ConfigureAwait(false);

            }
        }

        private async void OnDisconnected()
        {
            if (isConnected)
            {
                isConnected = false;

                firstConnectEvent.Reset();

                if (DataTransportUnavailable != null)
                {
                    //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                    await Task.WhenAll(Array.ConvertAll(
                             DataTransportUnavailable.GetInvocationList(),
                             e => ((dDataTransportAsync<byte[]>)e)(this))).ConfigureAwait(false);

                }
            }
        }

        private async void OnConnected()
        {
            if (!isConnected)
            {
                isConnected = true;

                firstConnectEvent.Set();

                if (DataTransportAvailable != null)
                {
                    //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                    await Task.WhenAll(Array.ConvertAll(
                             DataTransportAvailable.GetInvocationList(),
                             e => ((dDataTransportAsync<byte[]>)e)(this))).ConfigureAwait(false);

                }
            }
        }

        #endregion

        #region IEnabler
        /// <summary>
        /// NOT USED
        /// </summary>
        public void Initialize()
        {
            
        }

        /// <summary>
        /// Disable the server
        /// </summary>
        public void Disable()
        {
            if (isEnabled)
            {
                pipeStream.Dispose();

                checkConnectionThread.Interrupt();

                isEnabled = false;
            }
        }
        /// <summary>
        /// Enable the server
        /// </summary>
        public void Enable()
        {
            if (!isEnabled)
            {
                this.InitPipe();

                checkConnectionThread = new Thread(new ThreadStart(CheckConnectionThread));
                checkConnectionThread.Start();

                isEnabled = true;
            }
        }
        /// <summary>
        /// Returns the enable status
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled()
        {
            return isEnabled;
        }
        /// <summary>
        /// Disapose the server
        /// </summary>
        public void Dispose()
        {
            this.Disable();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Returns if the stream is available
        /// </summary>
        public bool Available
        {
            get
            {
                return this.IsConnected;
            }
        }
        /// <summary>
        /// Wait for a connected status
        /// </summary>
        /// <param name="connectTimeout"></param>
        /// <returns></returns>
        public bool WaitForConnected(TimeSpan connectTimeout)
        {
            return firstConnectEvent.WaitOne(connectTimeout);
        }

        public async Task<bool> SendAsync(byte[] data)
        {
            if (!isConnected) return false;

            await pipeStream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);

            return false;
        }

        public Task<bool> SendAsync(byte[] data, DataEndpoint to)
        {
            return SendAsync(data);
        }

        NamedPipeDataEndpoint endpoint;
        public DataEndpoint GetRemoteEndpoint()
        {
            if (endpoint == null)
                endpoint = new NamedPipeDataEndpoint()
                {
                    name = this.pipeName,
                    server = this.serverName
                };

            return endpoint;
        }

        #endregion

        #region Classe interna
        //  Questa classe serve a generare l'oggetto che contiene le informazioni circa 
        //  l'operazione asincrona di ricezione.
        internal class PipeStateObject
        {
            //  Client socket.
            public PipeStream pipe;
            //  Size of receive buffer.
            public int BufferSize;
            //  Receive buffer.
            public byte[] buffer;

            public PipeStateObject(PipeStream _pipe, int _bufSize)
            {
                pipe = _pipe; //  Serve per riconoscere il giusto socket quando ci sono più client.
                BufferSize = _bufSize;
                buffer = new byte[BufferSize];
            }
        }
        #endregion
    }
}
