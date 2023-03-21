using ICCS.DataBroker.MQTT;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TetSistemi.Base.Interfaces;
using TetSistemi.Protocol.Transport.Data.Endpoint;
using static TetSistemi.Protocol.Transport.Mqtt.Memory.InMemoryMqttBroker;

namespace TetSistemi.Protocol.Transport.Byte.Memory
{
    

    public class InMemoryByteClient : IDataTransport<byte[]>, IEnabler, IDisposable
    {
        public enum Sides
        {
            Client,
            Server
        }

        Sides side;
        public InMemoryByteClient(Sides side)
        {
            this.side = side;
        }

        public bool Available => true;

        public event dDataReceivedAsync<byte[]> DataReceivedAsync;
        public event dDataTransportAsync<byte[]> DataTransportAvailable;
        public event dDataTransportAsync<byte[]> DataTransportUnavailable;
        public event dDataTransportTraceAsync<byte[]> DataTransportTraceAsync;

        public DataEndpoint GetRemoteEndpoint()
        {
            return new EmptyDataEndpoint();
        }

        public Task<bool> SendAsync(byte[] data)
        {
            return SendAsync(data, new EmptyDataEndpoint());
        }

        public Task<bool> SendAsync(byte[] data, DataEndpoint to)
        {
            InMemoryByte.Instance.Publish(data, side);

            return Task.FromResult(true);
        }

        public void Enable()
        {
            InMemoryByte.Instance.SetAs(this, side);
        }

        public void Disable()
        {
            
        }

        public bool IsEnabled()
        {
            return true;
        }

        public async void Pushdata(byte[] data)
        {
            await Task.WhenAll(Array.ConvertAll(
                    DataReceivedAsync.GetInvocationList(),
                    e => ((dDataReceivedAsync<byte[]>)e)(this, data, new EmptyDataEndpoint()))).ConfigureAwait(false);
        }

        public void Dispose()
        {
            
        }
    }
}
