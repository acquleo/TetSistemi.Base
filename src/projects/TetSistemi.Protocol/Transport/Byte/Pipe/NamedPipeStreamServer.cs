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
    /// Server NamedPipe
    /// </summary>
    public class NamedPipeStreamServer : IDataTransport<byte[]>, IEnabler,
        IDisposable
    {
        #region Private fields
        NamedPipeServerStream pipeStream;
        string pipeName;
        bool isEnabled;
        PipeStateObject stateObj;
        AsyncCallback m_pfnCallBack;
        AsyncCallback m_connCallBack;
        bool isConnected;
        bool shutdown;
        int bufferSize = 1024;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Pipe Name</param>
        public NamedPipeStreamServer(string name)
        {
            this.pipeName = name;
            
            m_pfnCallBack = new AsyncCallback(OnDataReceived);
            m_connCallBack = new AsyncCallback(OnConnectReceived);
        }
        #endregion

        #region IConnectionStatus
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }

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
        #endregion

        #region Events
        public event dDataReceivedAsync<byte[]> DataReceivedAsync;
        public event dDataTransportAsync<byte[]> DataTransportAvailable;
        public event dDataTransportAsync<byte[]> DataTransportUnavailable;
        public event dDataTransportTraceAsync<byte[]> DataTransportTraceAsync;
        #endregion

        #region Private method

        private async void OnDisconnected()
        {
            if (isConnected)
            {
                isConnected = false;

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

                if (DataTransportAvailable != null)
                {
                    //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                    await Task.WhenAll(Array.ConvertAll(
                             DataTransportAvailable.GetInvocationList(),
                             e => ((dDataTransportAsync<byte[]>)e)(this))).ConfigureAwait(false);

                }
            }
        }

        private void ReinitializePipe()
        {
            this.ShutdownPipe();
            this.InitPipe();            
        }

        private void InitPipe()
        {
            if (pipeStream != null)
            {
                pipeStream.Close();
                pipeStream.Dispose();
            }

            if (!shutdown)
            {
                pipeStream = new System.IO.Pipes.NamedPipeServerStream(pipeName, PipeDirection.InOut, 1,
                    PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                stateObj = new Pipe.NamedPipeStreamServer.PipeStateObject(pipeStream, 
                    this.bufferSize);
                

                this.WaitForConnection();
            }
        }

        private void ShutdownPipe()
        {
            if (pipeStream != null)
            {
                pipeStream.Close();
                pipeStream.Dispose();
            }            
        }


        private void OnConnectReceived(IAsyncResult asyn)
        {
            try
            {
                NamedPipeServerStream stream = ((PipeStateObject)asyn.AsyncState).pipe;
                stream.EndWaitForConnection(asyn);                

                if (!shutdown)
                {

                    WaitForData();

                    OnConnected();
                }

            }
            catch (ObjectDisposedException ex)
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        private void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                PipeStateObject theSSO = (PipeStateObject)asyn.AsyncState;
                int iRx = theSSO.pipe.EndRead(asyn);              

                if (!shutdown)
                {
                    //  Determina il numero di byte ricevuti.

                    if (iRx > 0)
                    {
                        byte[] dataReceive = new byte[iRx];
                        Array.Copy(theSSO.buffer, dataReceive, iRx);

                        FireOnReceive(dataReceive);

                        this.WaitForData();
                    }
                    else
                    {
                        OnDisconnected();
                        ReinitializePipe();
                    }

                }
                

            }
            catch(IOException)
            {
                if (!shutdown)
                {
                    OnDisconnected();
                    ReinitializePipe();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            
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

        private void WaitForConnection()
        {
            pipeStream.BeginWaitForConnection(m_connCallBack, stateObj);
        }

        #endregion
        
        #region IEnabler
        public void Disable()
        {
            if (isEnabled)
            {


                shutdown = true;
                              

                pipeStream.Close();
                pipeStream.Dispose();
                

                isEnabled = false;

                pipeStream = null;
                
            }
        }

        public void Enable()
        {
            if (!isEnabled)
            {
                shutdown = false;

                this.InitPipe();

                isEnabled = true;
            }


        }

        public void Initialize()
        {

        }

        public bool IsEnabled()
        {
            return isEnabled;
        }
        #endregion

        #region IStream
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
        
        public virtual void Dispose()
        {
            this.Disable();
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
                    name = this.pipeName
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
            public NamedPipeServerStream pipe;
            //  Size of receive buffer.
            public int BufferSize;
            //  Receive buffer.
            public byte[] buffer;

            public PipeStateObject(NamedPipeServerStream _pipe, int _bufSize)
            {
                pipe = _pipe; //  Serve per riconoscere il giusto socket quando ci sono più client.
                BufferSize = _bufSize;
                buffer = new byte[BufferSize];
            }
        }
        #endregion
    }
}
