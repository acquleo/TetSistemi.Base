using System.Threading.Tasks;

namespace TetSistemi.Protocol
{
    /// <summary>
    /// data transport interface
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TSub"></typeparam>
    public interface IPubSubDataTransport<TData,TSub> : IDataTransport<TData>
    {
        /// <summary>
        /// execute a data transport subscribe
        /// </summary>
        /// <param name="subscribe"></param>
        /// <returns></returns>
        Task<bool> SubscribeAsync(TSub subscribe);
    }
}