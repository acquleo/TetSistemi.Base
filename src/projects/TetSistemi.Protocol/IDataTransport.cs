using System.Threading.Tasks;

namespace TetSistemi.Protocol
{
    /// <summary>
    /// delegates the reception of data from the remote endpoint
    /// </summary>
    /// <typeparam name="TData">data type</typeparam>
    /// <param name="endpoint">this data transport</param>
    /// <param name="data">received data</param>
    /// <param name="sender">remote endpoint</param>
    /// <returns></returns>
    public delegate Task dDataReceivedAsync<TData>(IDataTransport<TData> endpoint, TData data, DataEndpoint sender);

    /// <summary>
    /// delegates generic trasnport events
    /// </summary>
    /// <typeparam name="TData">data type</typeparam>
    /// <param name="endpoint">this data transport</param>
    /// <returns></returns>
    public delegate Task dDataTransportAsync<TData>(IDataTransport<TData> endpoint);

    /// <summary>
    /// delegates a data in/Out trace 
    /// </summary>
    /// <typeparam name="TData">data type</typeparam>
    /// <param name="endpoint">this data transport</param>
    /// <param name="data">received data</param>
    /// <param name="sender">remote endpoint</param>
    /// <param name="direction">data direction</param>
    /// <returns></returns>
    public delegate Task dDataTransportTraceAsync<TData>(IDataTransport<TData> endpoint, DataDirection direction, TData data, DataEndpoint sender);

    /// <summary>
    /// data transport interface
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IDataTransport<TData>
    {
        /// <summary>
        /// send data to the remote endpoint
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> SendAsync(TData data);

        /// <summary>
        /// send data to the specified remote endpoint
        /// </summary>
        /// <param name="data"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<bool> SendAsync(TData data, DataEndpoint to);

        /// <summary>
        /// notify data receive from the remote endpoint
        /// </summary>
        event dDataReceivedAsync<TData> DataReceivedAsync;

        /// <summary>
        /// returns the avalability of the transport
        /// </summary>
        bool Available { get; }

        /// <summary>
        /// notify the avalability of the transport
        /// </summary>
        event dDataTransportAsync<TData> DataTransportAvailable;

        /// <summary>
        /// notify the unavalability of the transport
        /// </summary>
        event dDataTransportAsync<TData> DataTransportUnavailable;

        /// <summary>
        /// notify the input/output data though the transport
        /// </summary>
        event dDataTransportTraceAsync<TData> DataTransportTraceAsync;

        /// <summary>
        /// returns the current remote endpoint
        /// </summary>
        /// <returns></returns>
        DataEndpoint GetRemoteEndpoint();
    }
}