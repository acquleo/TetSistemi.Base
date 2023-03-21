using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TetSistemi.Protocol.Converter.BYTE;

namespace TetSistemi.Protocol.Aggregator.Byte
{
    /// <summary>
    /// Implements a byte transport aggregator
    /// </summary>
    public class ByteArrayAggregator : IDataAggregator<byte[]>         
    {
        IByteArrayMessageEnveloper serializer;
        object syncDataReceived = new object();
        private byte[] leaningData = new byte[0];
        private bool leaning = false;
        Queue<byte[]> aggregatedmessagequeue = new Queue<byte[]>();

        Dictionary<DataEndpoint, ByteArrayAggregatorEndPoint> aggregators = new Dictionary<DataEndpoint, ByteArrayAggregatorEndPoint>();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serializer"></param>
        public ByteArrayAggregator(IByteArrayMessageEnveloper serializer)
        {
            this.serializer = serializer;
        }

        ByteArrayAggregatorEndPoint GetAggregator(DataEndpoint endpoint)
        {
            lock (aggregators)
            {
                if (!aggregators.ContainsKey(endpoint))
                {
                    aggregators.Add(endpoint, new ByteArrayAggregatorEndPoint(this.serializer));
                }

                return aggregators[endpoint];
            }
        }

        /// <summary>
        /// Aggregate received data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="endpoint">endpoint</param>
        public void Aggregate(byte[] data, DataEndpoint endpoint)
        {
            GetAggregator(endpoint).Aggregate(data);
        }

        /// <summary>
        /// returns true when aggregated data is available
        /// </summary>
        /// <param name="endpoint">endpoint</param>
        /// <returns></returns>
        public bool AggregatedDataAvailable(DataEndpoint endpoint)
        {
            return GetAggregator(endpoint).AggregatedDataAvailable();
        }

        /// <summary>
        /// Reset the aggregation buffer
        /// </summary>
        /// <param name="endpoint">endpoint</param>
        public void Clear(DataEndpoint endpoint)
        {
            GetAggregator(endpoint).Clear();
        }

        /// <summary>
        /// returns the aggregated data
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public byte[] GetAggregatedData(DataEndpoint endpoint)
        {
            return GetAggregator(endpoint).GetAggregatedData();
        }

    }
}
