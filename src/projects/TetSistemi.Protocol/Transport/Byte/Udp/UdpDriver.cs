// ------------------------------------------------------------------------
//Società: T&TSistemi s.r.l.
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: TetSistemi.Commons.dll
//Data ultima modifica: $LastChangedDate: 2020-01-09 14:21:54 +0100 (Thu, 09 Jan 2020) $
//Versione: $Rev: 706 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Pipes;
using TetSistemi.Protocol.Transport.Data.Endpoint;
using TetSistemi.Protocol.Exceptions;

namespace TetSistemi.Protocol.Transport.Byte.Udp
{
    /// <summary>
    /// Implementa un Driver Udp
    /// </summary>
    public class UdpDriver : IDataTransport<byte[]>, IDisposable
    {
        #region Private Field

        private System.Net.Sockets.Socket log_socket;        
        private static ManualResetEvent allDone = new ManualResetEvent(false);       
        Thread readthread;
        private IPEndPoint local;
        private EndPoint localEndPoint;
        private IPEndPoint remote;
        private EndPoint remoteEndPoint;
        bool isRunning;
        private byte[] DATA;
        int inputBufferSize = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_localPort">Porta locale di ricezione</param>
        /// <param name="_remotePort">Porta remota di ricezione</param>
        /// <param name="_remoteIpAddress">Indirizzo Ip remoto</param>
        /// <param name="_inputBufferSize">Lunghezza in bytes del buffer di ricezione</param>
        public UdpDriver(int _localPort, int _remotePort, string _remoteIpAddress, int _inputBufferSize)
        {
            this.local = new IPEndPoint(IPAddress.Any, _localPort);
            this.localEndPoint = (EndPoint)local;
            this.remote = new IPEndPoint(IPAddress.Parse(_remoteIpAddress), _remotePort);
            this.remoteEndPoint = (EndPoint)remote;
            inputBufferSize = _inputBufferSize;
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_localPort">Porta locale di ricezione</param>
        /// <param name="_remotePort">Porta remota di ricezione</param>
        /// <param name="_remoteIpAddress">Indirizzo Ip remoto</param>
        public UdpDriver(int _localPort, int _remotePort, string _remoteIpAddress)
        {
            this.local = new IPEndPoint(IPAddress.Any, _localPort);
            this.localEndPoint = (EndPoint)local;
            this.remote = new IPEndPoint(IPAddress.Parse(_remoteIpAddress), _remotePort);
            this.remoteEndPoint = (EndPoint)remote;
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_localPort">Porta locale di ricezione</param>
        /// <param name="localIpAddress">Indirizzo Ip locale</param>
        /// <param name="_remotePort">Porta remota di ricezione</param>
        /// <param name="_remoteIpAddress">Indirizzo Ip remoto</param>
        public UdpDriver(int _localPort, string localIpAddress, int _remotePort, string _remoteIpAddress)
        {
            this.local = new IPEndPoint(IPAddress.Parse(localIpAddress), _localPort);
            this.localEndPoint = (EndPoint)local;
            this.remote = new IPEndPoint(IPAddress.Parse(_remoteIpAddress), _remotePort);
            this.remoteEndPoint = (EndPoint)remote;
        }

        #endregion

        #region Properties

        /// <summary>
        /// EndPoint locale
        /// </summary>
        public EndPoint LocalEndPoint
        {
            get
            {
                return localEndPoint;
            }
        }

        /// <summary>
        /// EndPoint remoto
        /// </summary>
        public EndPoint RemoteEndPoint
        {
            get
            {
                return remoteEndPoint;
            }
        }

        /// <summary>
        /// Indica se il Driver è in esecuzione
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }

        /// <summary>
        /// Ritorna la lunghezza in bytes del buffer di ricezione
        /// </summary>
        public int InputBufferSize
        {
            get
            {
                return inputBufferSize;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Avvia il driver Udp
        /// </summary>
        public void Start()
        {
            if (!isRunning)
            {
                isRunning = true;

                this.log_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this.log_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                if (inputBufferSize != 0)
                {
                    this.log_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, inputBufferSize);
                }
                this.log_socket.Bind(localEndPoint);

                //start listening thread
                this.readthread = new Thread(new ThreadStart(this.WaitRead));
                this.readthread.Start();

                
            }
        }

        /// <summary>
        /// ferma il driver Udp
        /// </summary>
        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;

                this.readthread.Interrupt();
                this.readthread.Join();
                this.log_socket.Shutdown(SocketShutdown.Both);
                this.log_socket.Close();
                
            }
        }

        /// <summary>
        /// Modifica la porta di ricezione locale
        /// </summary>
        /// <param name="_localaddress">Indirizzo ip locale.</param>
        /// <param name="_localPort">Porta locale.</param>
        public void ChangeLocalAddress(string _localaddress,int _localPort)
        {
            bool lastisrunning = this.isRunning;
            if (lastisrunning) Stop();

            //restart listening with new port            
            this.local = new IPEndPoint(IPAddress.Parse(_localaddress), _localPort);
            this.localEndPoint = (EndPoint)local;

            if (lastisrunning) Start();

        }

        /// <summary>
        /// Modifica la porta di ricezione locale
        /// </summary>
        /// <param name="_localPort">Porta locale.</param>
        public void ChangeLocalPort(int _localPort)
        {
            bool lastisrunning = this.isRunning;
            if (lastisrunning) Stop();

            //restart listening with new port            
            this.local = new IPEndPoint(IPAddress.Any, _localPort);
            this.localEndPoint = (EndPoint)local;

            if (lastisrunning) Start();
            
        }

        /// <summary>
        /// Modifica l'indirizzo Ip Remoto
        /// </summary>
        /// <param name="_remotePort">Porta remota.</param>
        /// <param name="_remoteIpAddress">Indirizzo ip remoto.</param>
        public void ChangeRemoteAddress(int _remotePort, string _remoteIpAddress)
        {
            this.remote = new IPEndPoint(IPAddress.Parse(_remoteIpAddress), _remotePort);
            this.remoteEndPoint = (EndPoint)remote;
        }


        /// <summary>
        /// Invia un array di bytes sul socket
        /// </summary>
        /// <param name="msg">Array di bytes</param>
        /// <param name="endpoint">Endpoint di destinazione</param>
        /// <returns>Ritorna il numero di bytes inviati</returns>
        public int SendDataTo(byte[] msg, IPEndPoint endpoint)
        {
            int dataSend = this.log_socket.SendTo(msg, msg.Length, SocketFlags.None, endpoint);


            return dataSend;
        }

        #endregion

        #region Private Methods


        private void WaitRead()
        {
            try
            {
                this.DATA = new byte[short.MaxValue];
                //setting socket connection                
                do
                {
                    if (this.log_socket.Available > 0)
                    {
                        //read data from buffer
                        EndPoint ipEndPoint = new IPEndPoint(IPAddress.None,0);
                        int dataRcvSize = 0;
                        try
                        {
                            dataRcvSize = this.log_socket.ReceiveFrom(DATA, 0, DATA.Length, SocketFlags.None, ref ipEndPoint);
                        }
                        catch (SocketException ex1)
                        {
                            Console.WriteLine(ex1.Message);
                        }
                        //process data
                        byte[] dataRcv = new byte[dataRcvSize];
                        Array.Copy(DATA, 0, dataRcv, 0, dataRcvSize);
                        
                        FireOnDataReceived(dataRcv, (IPEndPoint)ipEndPoint);
                        //clear memory data                        
                        for (int i = 0; i < this.DATA.Length; i++)
                        {
                            DATA[i] = 0;
                        }
                    }
                    else
                        Thread.Sleep(1);
                }
                while (isRunning);
            }
            catch (ThreadInterruptedException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private async void FireOnDataReceived(byte[] dataRcv, IPEndPoint endpoint )
        {
            if (DataReceivedAsync != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                await Task.WhenAll(Array.ConvertAll(
                         DataReceivedAsync.GetInvocationList(),
                         e => ((dDataReceivedAsync<byte[]>)e)(this, dataRcv, new IpDataEndpoint()
                         {
                              address=endpoint.Address.ToString(),
                               port=endpoint.Port,
                         }))).ConfigureAwait(false);

            }
        }

        #endregion

        #region IStream Members


        public Task<bool> SendAsync(byte[] data)
        {
            return SendAsync(data, GetRemoteEndpoint());
        }

        public async Task<bool> SendAsync(byte[] data, DataEndpoint to)
        {
            if(to== null)
            {
                to = GetRemoteEndpoint();
            }

            if (!(to is IpDataEndpoint)) throw new ArgumentProtocolException("invalid data endpoint type");

            IpDataEndpoint endpoint = (IpDataEndpoint)to;
            var target = new IPEndPoint(IPAddress.Parse(endpoint.address), endpoint.port);


            int dataSend = await this.log_socket.SendToAsync(new ArraySegment<byte>(data), SocketFlags.None, target).ConfigureAwait(false);

            return dataSend>0;
        }

        IpDataEndpoint endpoint;
        public DataEndpoint GetRemoteEndpoint()
        {
            if (endpoint == null)
                endpoint = new IpDataEndpoint()
                {
                    address = this.remote.Address.ToString(),
                    port = this.remote.Port
                };

            return endpoint;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Ferma il driver e libera le risorse
        /// </summary>
        public void Dispose()
        {
            Stop();
        }


        #endregion

        #region IStream Members
        /// <summary>
        /// Returns if the stream is available
        /// </summary>
        public bool Available
        {
            get
            {
                return this.IsRunning;
            }
        }
        /// <summary>
        /// Implementa l'evento di Trace
        /// </summary>
        public event dDataReceivedAsync<byte[]> DataReceivedAsync;
        public event dDataTransportAsync<byte[]> DataTransportAvailable;
        public event dDataTransportAsync<byte[]> DataTransportUnavailable;
        public event dDataTransportTraceAsync<byte[]> DataTransportTraceAsync;

        #endregion
    }
}
