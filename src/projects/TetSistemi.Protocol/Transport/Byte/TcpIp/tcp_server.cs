using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TetSistemi.Base.Interfaces;
using TetSistemi.Base.Logger;
using TetSistemi.Protocol.Transport.Data.Endpoint;
using TetSistemi.Protocol.Transport.Mqtt;
//using TetSistemi.Commons.Logger;

namespace TetSistemi.Protocol.Transport.Byte.TcpIp
{
    /// <summary>
    /// Implementa le funzionalità della classe TCPEventArgs.
    /// </summary>
    public class TCPEventArgs : EventArgs
    {
        #region Variable Declaration
        private DataEndpoint mintSocket;
        private byte[] mData;
        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="data">Array di byte.</param>
        /// <param name="socketkey">Socket key</param>
        public TCPEventArgs(byte[] data, DataEndpoint socketkey)
        {
            mData = data;
            mintSocket = socketkey;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Proprietà Data.
        /// </summary>
        public byte[] Data
        {
            get
            {
                return mData;
            }
        }

        /// <summary>
        /// Proprietà SocketKey.
        /// </summary>
        public DataEndpoint SocketKey
        {
            get
            {
                return mintSocket;
            }
        }

        #endregion
    }

    /// <summary>
    /// Implementa un Server TCP.
    /// </summary>
    public class TcpServer : IDataTransport<byte[]>, IEnabler, IDisposable
    {

        #region Variable Declaration
        private int mintPort = 0;
        private IPAddress mIPAddress = IPAddress.Any;
        private int mintConnections = 0;
        private int mintBuffer = 1000000;
        private Socket listener;
        private int defaultBackLogSize = 10000;
        Dictionary<DataEndpoint, IDataTransport<byte[]>> connectionsMap = new Dictionary<DataEndpoint, IDataTransport<byte[]>>();
        IMessageLog logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="port">server port</param>
        public TcpServer(int port)
        {
            this.logger = NLogMessageLogger.GetLogger(this);
            this.mintPort = port;
        }


        #endregion

        #region Properties

        /// <summary>
        /// Porta di ricezione TCP
        /// </summary>
		public int LocalPort
        {
            get { return mintPort; }
            set { mintPort = value; }
        }

        /// <summary>
        /// Dimensione del buffer di ricezione
        /// </summary>
        public int InputBuffer
        {
            get { return mintBuffer; }
            set { mintBuffer = value; }
        }

        /// <summary>
        /// Connessioni Correnti
        /// </summary>
        public List<IDataTransport<byte[]>> ActiveConnections
        {
            get
            {
                lock (connectionsMap)
                {
                    List<IDataTransport<byte[]>> list = new List<IDataTransport<byte[]>>();
                    foreach(var el in connectionsMap.Values)
                    {
                        list.Add(el);
                    }
                    return list;
                }                
            }
        }

        /// <summary>
        /// Numero di Connessioni Correnti
        /// </summary>
		public int ActiveConnectionsCount
        {
            get { return mintConnections; }
        }

        /// <summary>
        /// Indirizzo sul quale viene effettuato il Bind
        /// </summary>
		public IPAddress LocalIpAddress
        {
            get { return mIPAddress; }
            set { mIPAddress = value; }
        }

        public bool Available => true;

        #endregion

        #region Events.

        /// <summary>
        /// Delega l'evento BeginConnect
        /// </summary>
        /// <param name="sender">Oggetto TcpServerConnection</param>
        /// <param name="e">Args</param>
        public delegate void ConnectEventHandler(TcpServerConnection sender, TCPEventArgs e);

        /// <summary>
        /// Implementa l'evento di connessione
        /// </summary>
		public event ConnectEventHandler BeginConnect;

        /// <summary>
        /// Delega l'evento BeginConnect
        /// </summary>
        /// <param name="sender">Oggetto TcpServerConnection</param>
        /// <param name="e">Args</param>
		public delegate void DisconnectEventHandler(object sender, TCPEventArgs e);
        /// <summary>
        /// Implementa l'evento di disconnessione
        /// </summary>
		public event DisconnectEventHandler BeginDisconnect;

        public event dDataReceivedAsync<byte[]> DataReceivedAsync;
        public event dDataTransportAsync<byte[]> DataTransportAvailable;
        public event dDataTransportAsync<byte[]> DataTransportUnavailable;
        public event dDataTransportTraceAsync<byte[]> DataTransportTraceAsync;
        bool enabled;
        #endregion

        #region Public Method
        /// <summary>Abilita il Server</summary>
        public void Enable()
        {
            if (enabled) return;
            if (mintPort < 1)
            {
                this.logger.Log(LogLevels.Error,this, $@"Invalid server listening port: {mintPort}");
                throw new Exception("Invalid port.");
            }

            try
            {
                // Initialize socket objects.
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(new IPEndPoint(mIPAddress, mintPort));
                listener.Listen(defaultBackLogSize);

                listener.BeginAccept(new AsyncCallback(BeginAccept), listener);
                enabled = true;

                this.logger.Log(LogLevels.Info, this, $@"Start listening on port: {mintPort}");
            }
            catch (Exception ex)
            {
                this.logger.Log(LogLevels.Error, this, $@"Server listening on port: {mintPort} exception {ex.ToString()}");
                throw ex;
            }
        }

        /// <summary>Disabilita il Server</summary>
        public void Disable()
        {
            if (!enabled) return;
            try
            {
                if (listener != null)
                {
                    enabled = false;
                    listener.Close();

                    Disconnect();  // Diconnette tutti i Clients

                    this.logger.Log(LogLevels.Info, this, $@"Server STOP listening on port: {mintPort}");
                }
            }
            catch (Exception ex)
            {
                this.logger.Log(LogLevels.Error, this, $@"Server listening on port: {mintPort} exception {ex.ToString()}");
                throw ex;
            }
        }

        /// <summary>Disconnette un Client</summary>
        /// <param name="connectionId">Identificativo della connessione</param>
        private void Disconnect(DataEndpoint connectionId)
        {
            try
            {
                if (!connectionsMap.ContainsKey(connectionId)) return;
                if (connectionsMap[connectionId] != null && ((TcpServerConnection)connectionsMap[connectionId]).Connected)
                {
                    ((TcpServerConnection)connectionsMap[connectionId]).DisconnectSocket(false);

                    this.logger.Log(LogLevels.Info, this, $@"Disconnected socket server: {mintPort} connection {connectionId.ToString()}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Disconnette tutti i Clients</summary>
        public void Disconnect()
        {
            List<DataEndpoint> strings= new List<DataEndpoint>();   
            lock (connectionsMap)
            {
                foreach (var el in connectionsMap.Keys)
                {
                    strings.Add(el);
                }
            }
            foreach (DataEndpoint key in strings) { Disconnect(key); }
        }

        #endregion

        #region Private Method

        /// <summary>Evento Connessione Avvenuta</summary>
        private void BeginAccept(IAsyncResult ar)
        {
            Socket listener = ar.AsyncState as Socket;

            if (listener == null || listener.Handle.ToInt32() == -1) return;

            try
            {

                Socket client = listener.EndAccept(ar);

                // search for empty client node.
                if (client.RemoteEndPoint == null) return;

                TcpServerConnection conn = new TcpServerConnection(client, InputBuffer);
                DataEndpoint key = conn.GetRemoteEndpoint();

                if (key == null) return;

                if (!connectionsMap.ContainsKey(key))
                {
                    lock (connectionsMap)
                    {
                        //this.Log(LogLevels.Trace, "BeginAccept connection: " + key);

                        conn.OnDisconnect += new TcpServerConnection.DisconnectEventHandler(OnDisconnectSocket);
                        conn.DataReceivedAsync += Conn_DataReceivedAsync;
                        conn.DataTransportAvailable += Conn_DataTransportAvailable;
                        conn.DataTransportTraceAsync += Conn_DataTransportTraceAsync;
                        conn.DataTransportUnavailable += Conn_DataTransportUnavailable;

                        connectionsMap.Add(key, conn);
                        mintConnections++;
                    }

                    this.logger.Log(LogLevels.Info, this, $@"OnDisconnectSocket connection added from connection list: {mintPort} socket {key} num connections: {mintConnections}");
                    
                    if (BeginConnect != null) BeginConnect(conn, new TCPEventArgs(null, key));

                    conn.BeginReceive();
                }

            }
            catch (ObjectDisposedException ex)
            {
                this.logger.Log(LogLevels.Error, this, $@"Server listening on port: {mintPort} exception {ex.ToString()}");
            }
            catch (Exception e)
            {
                this.logger.Log(LogLevels.Error, this, $@"Server listening on port: {mintPort} exception {e.ToString()}");
            }

            try
            {
                listener.BeginAccept(new AsyncCallback(BeginAccept), listener);

            }
            catch (Exception ex)
            {
                this.logger.Log(LogLevels.Error, this, $@"Server listening on port: {mintPort} exception {ex.ToString()}");
            }

        }

        private async Task Conn_DataTransportUnavailable(IDataTransport<byte[]> endpoint)
        {
            if(DataTransportUnavailable != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataTransportUnavailable.GetInvocationList(),
                     e => ((dDataTransportAsync<byte[]>)e)(endpoint))).ConfigureAwait(false);
            }
        }

        private async Task Conn_DataTransportTraceAsync(IDataTransport<byte[]> endpoint, DataDirection direction, byte[] data, DataEndpoint sender)
        {
            if (DataTransportTraceAsync != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataTransportTraceAsync.GetInvocationList(),
                     e => ((dDataTransportTraceAsync<byte[]>)e)(endpoint, direction, data, sender))).ConfigureAwait(false);
            }
        }

        private async Task Conn_DataTransportAvailable(IDataTransport<byte[]> endpoint)
        {
            if (DataTransportAvailable != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataTransportAvailable.GetInvocationList(),
                     e => ((dDataTransportAsync<byte[]>)e)(endpoint))).ConfigureAwait(false);
            }
        }

        private async  Task Conn_DataReceivedAsync(IDataTransport<byte[]> endpoint, byte[] data, DataEndpoint sender)
        {
            if (DataReceivedAsync != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataReceivedAsync.GetInvocationList(),
                     e => ((dDataReceivedAsync<byte[]>)e)(endpoint, data, sender))).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Client Disconnesso
        /// </summary>
        /// <param name="sender">Client</param>
        /// <param name="e">Parametri</param>
        /// <param name="DisconnectByServer">Indica se il socket è stato disconnesso dall'implementatore</param>
        private void OnDisconnectSocket(object sender, TCPEventArgs e, bool DisconnectByServer)
        {
            if (connectionsMap.ContainsKey(e.SocketKey))
            {
                lock (connectionsMap)
                {
                    connectionsMap.Remove(e.SocketKey);
                    --mintConnections;
                }

                this.logger.Log(LogLevels.Info, this, $@"OnDisconnectSocket connection removed from connection list: {mintPort} socket {e.SocketKey}");
                
                if (!DisconnectByServer)
                {
                    if (BeginDisconnect != null) BeginDisconnect(sender, e);
                }
            }
        }

        public async Task<bool> SendAsync(byte[] data)
        {
            var conns = this.ActiveConnections;
            foreach (var conn in conns) 
            {
                await conn.SendAsync(data).ConfigureAwait(false);
            }
            return true;
        }

        public async Task<bool> SendAsync(byte[] data, DataEndpoint to)
        {
            if(to== null) return await SendAsync(data).ConfigureAwait(false);

            if (!connectionsMap.ContainsKey(to)) return false;

            return await connectionsMap[to].SendAsync(data).ConfigureAwait(false);
        }

        EmptyDataEndpoint empty = new EmptyDataEndpoint();
        public DataEndpoint GetRemoteEndpoint()
        {
            return empty;
        }

        public void Dispose()
        {
            this.Disable();
        }

        public bool IsEnabled()
        {
            return enabled;
        }
        #endregion
    }
}