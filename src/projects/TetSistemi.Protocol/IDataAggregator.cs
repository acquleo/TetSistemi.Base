using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TetSistemi.Protocol
{
    /// <summary>
    /// data aggregation interface
    /// </summary>
    public interface IDataAggregator<TData>
    {
        /// <summary>
        /// append data to the aggregator
        /// </summary>
        /// <param name="data"></param>
        void Aggregate(TData data, DataEndpoint endpoint);

        /// <summary>
        /// returns the availability of aggregated data
        /// </summary>
        /// <returns></returns>
        bool AggregatedDataAvailable(DataEndpoint endpoint);

        /// <summary>
        /// returns a chunk of aggregate data
        /// </summary>
        /// <returns></returns>
        TData GetAggregatedData(DataEndpoint endpoint);

        /// <summary>
        /// clear the aggregation buffer
        /// </summary>
        /// <returns></returns>
        void Clear(DataEndpoint endpoint);
    }
}
