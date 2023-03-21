using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using TetSistemi.Commons.Protocol;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using TetSistemi.Protocol.Transport.Data.Endpoint;
using System.Threading.Tasks;
using TetSistemi.Base.Interfaces;
using TetSistemi.Base.Logger;

namespace TetSistemi.Protocol.Transport.Byte.TcpIp
{
    /// <summary>
    /// Implemenat un Client TCP/IP
    /// </summary>
    public class TcpClient : IDataTransport<byte[]>, IEnabler, IDisposable
    {

        #region Delegati

        /// <summary>
        /// Delega l'evento di connessione del Socket
        /// </summary>
        public delegate void ConnectEvent();
        /// <summary>
        /// Delega l'evento di disconnessione del Socket
        /// </summary>
        public delegate void DisconnectEvent();

        #endregion

        #region Campi privati
        IMessageLog logger;
        private ManualResetEvent firstConnectEvent;
        /// <summary>
        /// TRUE se il client prevede il tentativo di riconnessione automatica 
        /// in caso di disconnessione. False, altrimenti.
        /// </summary>
        private bool autoreconnect;

        //  Socket di comunicazione.
        private Socket m_clientSocket;
        //  EndPoint
        private IPEndPoint server_ipEndPoint;
        //  Flag di stato del thread di controllo della connessione.  
        private bool m_running;

        //  Flag di controllo della connessione.
        private bool _checkConnection;
        private TimeSpan _reconnectscantime = TimeSpan.FromSeconds(10);
        private TimeSpan _connectdelay = TimeSpan.FromSeconds(0);
        //  Thread di controllo della connessione.
        private Thread _checkConnectionThread;
        //  AsyncCallback provides a way for client applications to complete an asynchronous operation. 
        //  This callback delegate is supplied to the client when the asynchronous operation is initiated. 
        //  The event handler referenced by AsyncCallback contains program logic to finish processing the 
        //  asynchronous task for the client.
        //  AsyncCallback uses the IAsyncResult interface to obtain the status of the asynchronous operation.
        //  The IAsyncResult interface is implemented by classes containing methods that can operate asynchronously. 
        //  It is the return type of methods that initiate an asynchronous operation, such as FileStream.BeginRead, 
        //  and is the type of the third parameter of methods that conclude an asynchronous operation, such as 
        //  FileStream.EndRead. IAsyncResult objects are also passed to methods invoked by AsyncCallback delegates 
        //  when an asynchronous operation completes.
        //  An object that supports the IAsyncResult interface stores state information for an asynchronous operation, 
        //  and provides a synchronization object to allow threads to be signaled when the operation completes.
        //  For a detailed description of how the IAsyncResult interface is used, see the Calling Synchronous Methods 
        //  Asynchronously topic.
        //  Vedi Calling Synchronous Methods Asynchronously. 
        private AsyncCallback m_pfnCallBack;
        //  Oggetto necessario per la ricezione asincrona dei pacchetti.
        SocketStateObject theSocPkt;
        int inputBufferLength;
        #endregion

        #region Eventi

        /// <summary>
        /// Implementa l'evento di avvenuta ricezione dati
        /// </summary>
        public event dDataReceivedAsync<byte[]> DataReceivedAsync;

        /// <summary>
        /// Implementa l'evento di avvenuta connessione
        /// </summary>
        public event ConnectEvent OnConnect;

        /// <summary>
        /// Implementa l'evento di avvenuta disconnessione
        /// </summary>
        public event DisconnectEvent OnDisconnect;
        /// <summary>
        /// Implementa l'evento di connessione fallita
        /// </summary>
        public event DisconnectEvent ConnectFailure;
        public event dDataTransportAsync<byte[]> DataTransportAvailable;
        public event dDataTransportAsync<byte[]> DataTransportUnavailable;
        public event dDataTransportTraceAsync<byte[]> DataTransportTraceAsync;
        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="address">Indirizzo Ip del server</param>
        /// <param name="port">Porta di ascolto del server</param>
        /// <param name="reconnectscantime">Tempo di riconnessione automatica</param>
        /// <param name="autoreconnect"> True per abilitare il tentativo di riconnessione al server.</param>
        /// <param name="inputBufferLength"> Lunghezza in bytes del buffer di input.</param>
        public TcpClient(string address, int port, TimeSpan reconnectscantime, bool autoreconnect, int inputBufferLength)
        {
            this.logger = NLogMessageLogger.GetLogger(this);
            this.firstConnectEvent = new ManualResetEvent(false);
            this.inputBufferLength = inputBufferLength;
            this._reconnectscantime = reconnectscantime;
            this.autoreconnect = autoreconnect;
            IPAddress ip;
            try
            {
                ip = IPAddress.Parse(address);
            }
            catch (Exception e)
            {
                throw e;
                // LOG
            }
            //  Create the end point.

            //  Crea un oggetto di tipo IPEndPoint.
            server_ipEndPoint = new IPEndPoint(ip, port);
            //  Crea un oggetto di tipo Socket.
            m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

            //  Inzializza tutti gli altri campi della classe.
            m_pfnCallBack = new AsyncCallback(OnDataReceived);
            theSocPkt = new SocketStateObject(m_clientSocket, inputBufferLength);

            _checkConnection = false;

        }

        /// <summary>
        /// Costruttore AutoReconnect custom -  - Buffer di input a 1024
        /// </summary>
        /// <param name="address">Indirizzo ip del client.</param>
        /// <param name="port">Porta del client.</param>
        /// <param name="reconnectscantime">Tempo di riconnessione.</param>
        /// <param name="_autoreconnect">Impostazione stato di riconnessione automatica.</param>
        public TcpClient(string address, int port, TimeSpan reconnectscantime, bool _autoreconnect) :
            this(address, port, reconnectscantime, _autoreconnect, 1000000)
        {

        }

        /// <summary>
        /// Costruttore AutoReconnect abilitato - Buffer di input a 1024
        /// </summary>
        /// <param name="address">Indirizzo Ip del server</param>
        /// <param name="port">Porta di ascolto del server</param>
        /// <param name="reconnectscantime">Tempo di riconnessione automatica</param>
        public TcpClient(string address, int port, TimeSpan reconnectscantime) :
            this(address, port, reconnectscantime, true)
        {

        }

        #endregion

        #region Metodi Privati
        private async void FireOnTrace(DataDirection direction, byte[] data)
        {
            if (DataTransportTraceAsync != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataTransportTraceAsync.GetInvocationList(),
                     e => ((dDataTransportTraceAsync<byte[]>)e)(this, direction, data, this.GetRemoteEndpoint()))).ConfigureAwait(false);
            }
        }

        private void InternalConnection()
        {
            //  CONDIZIONE:
            //  Se, è necessario eseguire il controllo della connessione e il socket non è connesso,
            if (_checkConnection && !m_clientSocket.Connected)
            {
                //  Allora:
                try
                {
                    //  Esegue la connessione del socket.
                    //							m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //							m_pfnCallBack = new AsyncCallback(OnDataReceived);
                    //							theSocPkt = new SocketStateObject(this.m_clientSocket, 1024);
                    m_clientSocket.Connect(server_ipEndPoint);
                    //  Controlla che la connessione abbia avuto successo.
                    if (m_clientSocket.Connected)
                    {
                        //  Lancia l'evento di OnConnect.
                        FireOnConnect();
                        //  Attesa dei dati.
                        WaitForData();
                    }
                }
                catch (Exception e)
                {
                    if (ConnectFailure != null)
                    {
                        ConnectFailure();
                    }

                    ReinizializeSocket();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckConnectionThread()
        {
            try
            {
                Thread.Sleep(_connectdelay);

                while (m_running)
                {
                    InternalConnection();
                    //  Attesa di 10 secondi.
                    Thread.Sleep(_reconnectscantime);
                }
            }
            catch (ThreadInterruptedException)
            {

            }

        }
        /// <summary>
        /// Metodo per la reinizializzazione del socket.
        /// </summary>
        public void ReinizializeSocket()
        {
            try
            {
                //  Mette a falso il flag per il controllo della connessione; quando il socket è disconesso non serve
                //  controllare la connessione.
                _checkConnection = false;
                //  Esegue lo shutdown del socket.
                //  Il metodo Shutdown(SocketShutdown.Both) disabilita l'invio e la ricezione sul socket (parametro SocketShutdown.Both). 
                m_clientSocket.Shutdown(SocketShutdown.Both);
                //  Chiude il socket e rilascia le risorse ad esso associate.
                m_clientSocket.Close();
                //  Lancia il metodo che gestisce la disconnessione.
                FireOnDisconnect();
                //  Inizializza il socket e tutti gli oggetti preposti e effettuare la connessione.
                m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                theSocPkt = new SocketStateObject(m_clientSocket, inputBufferLength);
                //  Mette a true il flag per il controllo della connessione.
                _checkConnection = true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                _checkConnection = true;
            }

        }
        #endregion

        #region Metodi Pubblici

        /// <summary>
        /// Returns if the stream is available
        /// </summary>
        public bool Available
        {
            get
            {
                return IsConnected;
            }
        }

        /// <summary>
        /// Ritorna lo stato di connessione del socket.
        /// </summary>
        public bool IsConnected
        {
            get { return m_clientSocket.Connected; }
        }

        /// <summary>
        /// Ritorna l'end point locale del socket (null se socket non connesso)
        /// </summary>
        public EndPoint LocalEndPoint
        {
            get { return m_clientSocket.LocalEndPoint; }
        }

        /// <summary>
        /// Ritorna l'end point remoto del socket (null se socket non connesso)
        /// </summary>
        public EndPoint RemoteEndPoint
        {
            get { return m_clientSocket.RemoteEndPoint; }
        }

        /// <summary>
        /// Chiude la connessione ma non disabilita il Driver
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                //  Chiude il socket e rilascia le risorse ad esso associate.
                m_clientSocket.Disconnect(false);
            }
            catch (SocketException) { }
        }

        /// <summary>
        /// Abilita il Client TcpIp e attende la prima connessione
        /// </summary>
        /// <param name="connectTimeout">Imposta il timeout di attesa alla prima connessione</param>
        /// <returns>Ritorna lo stato.</returns>
        public bool ConnectWait(TimeSpan connectTimeout)
        {
            Connect(TimeSpan.FromSeconds(0));

            return WaitForConnected(connectTimeout);
        }

        /// <summary>
        /// Modifica indirizzo IP e porta del server TCP/IP
        /// </summary>
        /// <param name="ipaddress">Indirizzo IP</param>
        /// <param name="port">Porta</param>
        public void ChangeServer(string ipaddress, int port)
        {
            IPAddress ip;
            try
            {
                ip = IPAddress.Parse(ipaddress);
            }
            catch (Exception e)
            {
                throw e;
                // LOG
            }

            server_ipEndPoint = new IPEndPoint(ip, port);

            CloseConnection();
        }

        /// <summary>
        /// Attende lo stato di connessione del socket
        /// </summary>
        /// <param name="connectTimeout">Imposta il timeout di attesa dello stato di connesso</param>
        /// <returns>Ritorna lo stato.</returns>
        public bool WaitForConnected(TimeSpan connectTimeout)
        {
            return firstConnectEvent.WaitOne(connectTimeout);
        }

        /// <summary>
        /// Abilita il Client TcpIp
        /// </summary>
        /// <param name="connectdelay">Imposta un ritardo sul primo tentativo di connessione</param>
        public void Connect(TimeSpan connectdelay)
        {
            _connectdelay = connectdelay;
            // Imposto a true il flag di Running
            m_running = true;
            //  Mette solo a true il flag di controllo della connessione perchè è il thread di controllo della 
            //  conessione che in base a questa condizione esegue la connessione del socket.
            _checkConnection = true;

            if (autoreconnect)
            {
                //Eseguo il Thread di Controllo
                _checkConnectionThread = new Thread(new ThreadStart(CheckConnectionThread));
                _checkConnectionThread.Name = "TCPClient_ConnectionThread_" + server_ipEndPoint.ToString();
                _checkConnectionThread.Start();
            }
            else InternalConnection();
        }

        /// <summary>
        /// Disabilita il Client TcpIp
        /// </summary>
        public void Disconnect()
        {
            if (autoreconnect)
            {
                //  Mette a false il flag che specifica lo stato di esecuzione.
                m_running = false;
                //  Esegue l'abort del thread di controllo della connessione.
                if (_checkConnectionThread != null && _checkConnectionThread.IsAlive)
                    _checkConnectionThread.Interrupt();

                //while(this._checkConnectionThread.IsAlive)
                //{
                //    Thread.Sleep(10);
                //}
                //  Mette a falso il flag per il controllo della connessione; quando il socket è disconesso non serve
                //  controllare la connessione.
                _checkConnection = false;
            }
            //  Esegue lo shutdown del socket.
            //  Il metodo Shutdown(SocketShutdown.Both) disabilita l'invio e la ricezione sul socket (parametro SocketShutdown.Both). 
            if (m_clientSocket.Connected)
            {
                m_clientSocket.Shutdown(SocketShutdown.Both);
            }
            //  Chiude il socket e rilascia le risorse ad esso associate.
            m_clientSocket.Close();
            //  Lancia il metodo che gestisce la disconnessione.            
            FireOnDisconnect();
            //FireOnDisconnect();
            //  Inizializza il socket e tutti gli oggetti preposti e effettuare la connessione.
            m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            theSocPkt = new SocketStateObject(m_clientSocket, inputBufferLength);
            //  Mette a true il flag per il controllo della connessione.
            _checkConnection = true;
        }


        /// <summary>
        /// Invia un array di bytes sul Socket
        /// </summary>
        /// <param name="byteToSend">Dati da inviare</param>
        public async Task<bool> SendAsync(byte[] byteToSend)
        {
            int packetNumber = await SendData(byteToSend).ConfigureAwait(false);

            return packetNumber > 0;
        }

        /// <summary>
        /// Invia un array di bytes sul Socket
        /// </summary>
        /// <param name="byteToSend">Dati da inviare</param>
        public Task<bool> SendAsync(byte[] byteToSend, DataEndpoint arg)
        {
            return SendAsync(byteToSend);
        }

        /// <summary>
        /// Invia un array di bytes sul Socket
        /// </summary>
        /// <param name="byteToSend">Dati da inviare</param>
        /// <returns>Ritorna il numero di bytes da inviare</returns>
        private async Task<int> SendData(byte[] byteToSend)
        {
            try
            {

                //  Controlla se il socket è connesso.
                if (m_clientSocket.Connected)
                {
                    //  Invia il pacchetto come un array di byte e ritorna il numero di byte inviati.
                    int dataSend = await m_clientSocket.SendAsync(new ArraySegment<byte>(byteToSend), SocketFlags.None).ConfigureAwait(false);

                    FireOnTrace(DataDirection.Out, byteToSend);

                    return dataSend;
                }
                else
                {
                    //  Se il socket è disconnesso ritorna 0.
                    return 0;
                }
            }
            catch (Exception e)
            {
                this.logger.Log(LogLevels.Error, this, $@"SendData : {this.endpoint?.ToString()} exception {e.ToString()}");
                return 0;
            }
        }

        private void WaitForData()
        {
            try
            {
                m_clientSocket.BeginReceive(theSocPkt.buffer,        //  Buffer di ricezione dei dati.
                                            0,                       //  Offset 0.
                                            theSocPkt.buffer.Length, //  Dimensione del buffer. 
                                            SocketFlags.None,        //  Nessun flag.
                                            m_pfnCallBack,           //  CallBack.
                                            theSocPkt                //  Oggetto che contiene le informazioni circa l'operazione di ricezione.
                                            );
            }
            catch (SocketException se)
            {
                this.logger.Log(LogLevels.Error, this, $@"WaitForData : {this.endpoint?.ToString()} exception {se.ToString()}");
            }
        }

        //  Gestione dei dati ricevuti.
        // The following method is called when each asynchronous operation completes.
        private void OnDataReceived(IAsyncResult asyn)
        {
            try
            {

                if (m_running)
                {
                    //  Crea l'oggetto che deve contenere le informazioni circa l'operazione di ricezione.
                    SocketStateObject theSSO = asyn.AsyncState as SocketStateObject;
                    //  Determina il numero di byte ricevuti.
                    if (theSSO != null)
                    {
                        int iRx = theSSO.workSocket.EndReceive(asyn);
                        if (iRx > 0)
                        {
                            //  Crea un array dimensionato con il numero di byte ricevuti.
                            byte[] dataReceive = new byte[iRx];
                            //  Copia in dataReceive i dati ricevuti che sono stati passati attraverso l'oggetto preposto.
                            Array.Copy(theSSO.buffer, dataReceive, iRx);
                            //  Notifica dell'evento trace.

                            FireOnTrace(DataDirection.In, dataReceive);
                            //  Notifica dell'evento di avvenuta ricezione.
                            FireOnReceive(/*this,*/ dataReceive);
                            //  Si rimette in attesa per una nuova ricezione.
                            WaitForData();
                        }
                        else
                        {
                            ReinizializeSocket();
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                //Console.WriteLine(ex.Message);
                ReinizializeSocket();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                ReinizializeSocket();
            }
        }
        #endregion

        #region Gestori eventi
        //  Gestione dell'evento di ricezione dei dati.
        private async void FireOnReceive(/*object _sender,*/ byte[] _data)
        {
            if (DataReceivedAsync != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                await Task.WhenAll(Array.ConvertAll(
                         DataReceivedAsync.GetInvocationList(),
                         e => ((dDataReceivedAsync<byte[]>)e)(this, _data, GetRemoteEndpoint()))).ConfigureAwait(false);

            }
        }

        //  Gestione dell'evento di connessione.
        private async void FireOnConnect()
        {
            firstConnectEvent.Set();

            //  Controlla se è avvenuto il cablaggio da parte di un gestore di eventi.
            if (OnConnect != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                Thread tDisc = new Thread(new ThreadStart(OnConnect));
                tDisc.Start();
            }

            if (DataTransportAvailable != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                await Task.WhenAll(Array.ConvertAll(
                         DataTransportAvailable.GetInvocationList(),
                         e => ((dDataTransportAsync<byte[]>)e)(this))).ConfigureAwait(false);

            }

            this.logger.Log(LogLevels.Info, this, $@"Connected : {this.endpoint?.ToString()}");
        }

        //  Gestione dell'evento di disconnessione.
        private async void FireOnDisconnect()
        {
            firstConnectEvent.Reset();

            //  Controlla se è avvenuto il cablaggio da parte di un gestore di eventi.
            if (OnDisconnect != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                Thread tDisc = new Thread(new ThreadStart(OnDisconnect));
                tDisc.Start();

            }

            if (DataTransportUnavailable != null)
            {
                //  Esegue la comunicazione dell'evento attraverso l'apposito delegate.
                await Task.WhenAll(Array.ConvertAll(
                         DataTransportUnavailable.GetInvocationList(),
                         e => ((dDataTransportAsync<byte[]>)e)(this))).ConfigureAwait(false);

            }

            this.logger.Log(LogLevels.Info, this, $@"Disconnected : {this.endpoint?.ToString()}");
        }
        #endregion

        #region Classe interna
        //  Questa classe serve a generare l'oggetto che contiene le informazioni circa 
        //  l'operazione asincrona di ricezione.
        internal class SocketStateObject
        {
            //  Client socket.
            public Socket workSocket;
            //  Size of receive buffer.
            public int BufferSize;
            //  Receive buffer.
            public byte[] buffer;

            public SocketStateObject(Socket _s, int _bufSize)
            {
                workSocket = _s; //  Serve per riconoscere il giusto socket quando ci sono più client.
                BufferSize = _bufSize;
                buffer = new byte[BufferSize];
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disabilita il Client Tcp e libera le risorse allocate
        /// </summary>
        public void Dispose()
        {
            try
            {
                m_running = false;

                Disconnect();

                m_clientSocket.Close();
            }
            catch (Exception ex)
            {
                this.logger.Log(LogLevels.Error, this, $@"Dispose : {this.endpoint?.ToString()} exception {ex.ToString()}");
            }
        }

        IpDataEndpoint endpoint;
        public DataEndpoint GetRemoteEndpoint()
        {
            if (endpoint == null) endpoint = new IpDataEndpoint();
            if (RemoteEndPoint == null) return new EmptyDataEndpoint();
            string[] data = RemoteEndPoint.ToString().Split(':');
            endpoint.address= data[0];
            endpoint.port = int.Parse(data[1]);
            return endpoint;
        }

        bool enabled;
        public void Enable()
        {
            if (enabled) return;
            enabled = true;
            this.Connect(TimeSpan.Zero);
        }

        public void Disable()
        {
            if (!enabled) return;
            enabled = false;
            this.Disconnect();
        }

        public bool IsEnabled()
        {
            return enabled;
        }

        #endregion

    }
}
