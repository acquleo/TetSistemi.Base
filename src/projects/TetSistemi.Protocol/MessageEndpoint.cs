using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TetSistemi.Protocol.Exceptions;

namespace TetSistemi.Protocol
{
    /// <summary>
    /// implements the standard IMessageEndpoint
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class MessageEndpoint<TData,TEnvelope> : IMessageEndpoint<TEnvelope>
        where TEnvelope : IMessageEnvelope
    {
        IDataTransport<TData> datatransport;
        IMessageEnveloper<TData, TEnvelope> enveloper;
        IDataAggregator<TData> aggregator;

        public event dMessageReceivedAsync<TEnvelope> MessageReceivedAsync;
        public event dMessageTraceAsync<TEnvelope> MessageTraceAsync;
        public event dMessageTransportAsync<TEnvelope> MessageTransportAvailableAsync;
        public event dMessageTransportAsync<TEnvelope> MessageTransportUnavailableAsync;
        public event dMessageExceptionAsync<TEnvelope> MessageExceptionAsync;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="datatransport"></param>
        /// <param name="converter"></param>
        /// <param name="aggregator"></param>
        /// <exception cref="ArgumentProtocolException"></exception>
        public MessageEndpoint(IDataTransport<TData> datatransport,
            IMessageEnveloper<TData, TEnvelope> converter, 
            IDataAggregator<TData> aggregator)
        {
            if (datatransport == null) throw new ArgumentProtocolException($@"datatransport argument cannot be null");
            if (converter == null) throw new ArgumentProtocolException($@"converter argument cannot be null");
            this.datatransport = datatransport;
            this.enveloper= converter;
            this.aggregator = aggregator;
            this.datatransport.DataReceivedAsync += Datatransport_DataReceivedAsync;
            this.datatransport.DataTransportUnavailable += Datatransport_DataTransportUnavailable;
            this.datatransport.DataTransportAvailable += Datatransport_DataTransportAvailable;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="datatransport"></param>
        /// <param name="enveloperr"></param>
        public MessageEndpoint(IDataTransport<TData> datatransport,
            IMessageEnveloper<TData,TEnvelope> enveloperr):
            this(datatransport, enveloperr, null)
        {
            
        }

        /// <summary>
        /// returns the avaliability of the data trasnport
        /// </summary>
        public bool Available => this.datatransport.Available;

        /// <summary>
        /// send a message through the data transport
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task<bool> SendAsync(TEnvelope msg)
        {
            if (msg == null) throw new ArgumentProtocolException($@"msg parameter cannot be null");

            return this.SendAsync(msg, null);
        }

        /// <summary>
        /// send a message through the data transport to the specified endpoint
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<bool> SendAsync(TEnvelope msg, DataEndpoint to)
        {
            try
            {
                if (msg == null) throw new ArgumentProtocolException($@"msg parameter cannot be null");

                if (!this.datatransport.Available) return false;

                //convert message to data
                var data = this.enveloper.Unwrap(msg);

                //sends data through the trasnport layer
                var result = await this.datatransport.SendAsync(data, to).ConfigureAwait(false);

                if (result)
                {
                    OnMessageTrace(msg, to, DataDirection.Out);
                }

                return result;
            }
            catch(Exception ex)
            {
                OnInnerException(new InternalProtocolException($@"SendAsync {msg.GetType().Name} exception", ex));

                return false;
            }
        }

        private async Task Datatransport_DataTransportAvailable(IDataTransport<TData> endpoint)
        {
            try
            {
                if (MessageTransportAvailableAsync != null)
                {
                    await Task.WhenAll(Array.ConvertAll(
                      MessageTransportAvailableAsync.GetInvocationList(),
                      e => ((dMessageTransportAsync<TEnvelope>)e)(this))).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                OnInnerException(new InternalProtocolException($@"Datatransport_DataTransportAvailable MessageTransportAvailableAsync exception", ex));
            }
           
            return;
        }

        private async Task Datatransport_DataTransportUnavailable(IDataTransport<TData> endpoint)
        {
            try
            {
                if (this.aggregator != null)
                {
                    //clear the data of the aggregator when the transport became unavailable
                    this.aggregator.Clear(endpoint.GetRemoteEndpoint());
                }
            }
            catch(Exception ex) 
            {
                OnInnerException(new InternalProtocolException($@"Datatransport_DataTransportUnavailable aggregator Clear exception", ex));                
            }

            try
            {
                if (MessageTransportUnavailableAsync != null)
                {
                    await Task.WhenAll(Array.ConvertAll(
                      MessageTransportUnavailableAsync.GetInvocationList(),
                      e => ((dMessageTransportAsync<TEnvelope>)e)(this))).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                OnInnerException(new InternalProtocolException($@"Datatransport_DataTransportUnavailable MessageTransportUnavailableAsync exception", ex));
            }
            return;
        }

        private Task Datatransport_DataReceivedAsync(IDataTransport<TData> endpoint, TData data, DataEndpoint sender)
        {
            try
            {
                if (this.aggregator == null)
                {
                    //no aggregator available, process all data received
                    ProcessData(data, sender);
                    return Task.CompletedTask;
                }

                //append data to the aggregator
                this.aggregator.Aggregate(data, sender);

                //loop while aggregated data is available on the aggregator
                while (this.aggregator.AggregatedDataAvailable(sender))
                {
                    //process aggregated data
                    ProcessData(this.aggregator.GetAggregatedData(sender), sender);
                }                
            }
            catch (Exception ex) {
                OnInnerException(new InternalProtocolException($@"Datatransport_DataReceivedAsync exception", ex));
            }
            return Task.CompletedTask;
        }

        void ProcessData(TData data, DataEndpoint sender)
        {
            TEnvelope msg = default(TEnvelope);
            try
            {
                //convert aggregated data to message
                msg = this.enveloper.Wrap(data);
            }
            catch (Exception ex)
            {
                OnInnerException(new InternalProtocolException($@"ProcessData Deserialize exception", ex));
            }

            try
            {
                if(msg != null) OnMessageReceive(msg, sender);
            }
            catch (Exception ex)
            {
                OnInnerException(new InternalProtocolException($@"ProcessData OnMessageReceive exception", ex));
            }

            
        }

        async protected void OnInnerException(ProtocolException exception)
        {
            if (MessageExceptionAsync != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                  MessageExceptionAsync.GetInvocationList(),
                  e => ((dMessageExceptionAsync<TEnvelope>)e)(this, exception))).ConfigureAwait(false);

            }
        }
        
        async void OnMessageReceive(TEnvelope msg, DataEndpoint endp)
        {
            if (MessageReceivedAsync != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                  MessageReceivedAsync.GetInvocationList(),
                  e => ((dMessageReceivedAsync<TEnvelope>)e)(this, msg, endp))).ConfigureAwait(false);

            }
        }

        async void OnMessageTrace(TEnvelope msg, DataEndpoint endp, DataDirection direction)
        {
            if (MessageTraceAsync != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                  MessageTraceAsync.GetInvocationList(),
                  e => ((dMessageTraceAsync<TEnvelope>)e)(this, msg, direction, endp))).ConfigureAwait(false);

            }
        }

     
    }
}
