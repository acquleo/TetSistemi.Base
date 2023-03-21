using MQTTnet;
using MQTTnet.Internal;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TetSistemi.Protocol.Exceptions;

namespace TetSistemi.Protocol
{
    /// <summary>
    /// Implements IRpcMessageEndpoint
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TEnvelope"></typeparam>
    public class RpcMessageEndpoint<TData, TEnvelope>         
        : MessageEndpoint<TData, TEnvelope>,
        IRpcMessageEndpoint<TEnvelope>
        where TEnvelope : IMessageEnvelope
    {
        /// <summary>
        /// define the request context
        /// </summary>
        class RequestContext
        {

            public RequestContext(IMessageEnvelope Req)
            {
                ReplyMsg = new List<TEnvelope> ();
                Event = new  AutoResetEvent(false);
                this.Req = Req;
            }
            public AutoResetEvent Event { get; set; }
            public List<TEnvelope> ReplyMsg { get; set; }
            public IMessageEnvelope Req { get; set; }
        }

        Dictionary<object, RequestContext> pendingRequests;

        /// <summary>
        /// RpcMessageEndpoint constructor
        /// </summary>
        /// <param name="datatransport">underlying transport</param>
        /// <param name="enveloper">IMessage enveloper</param>
        /// <param name="aggregator">data transport aggregator</param>
        public RpcMessageEndpoint(IDataTransport<TData> datatransport,
            IMessageEnveloper<TData, TEnvelope> enveloper, 
            IDataAggregator<TData> aggregator):base(datatransport, enveloper, aggregator)
        {
            pendingRequests = new Dictionary<object, RequestContext>();

            this.MessageReceivedAsync += RpcMessageEndpoint_MessageReceivedAsync;
        }


        /// <summary>
        /// RpcMessageEndpoint constructor
        /// </summary>
        /// <param name="datatransport">underlying transport</param>
        /// <param name="enveloper">IMessage enveloper</param>
        public RpcMessageEndpoint(IDataTransport<TData> datatransport,
           IMessageEnveloper<TData, TEnvelope> enveloper) : this(datatransport, enveloper, null)
        {

        }

        private Task RpcMessageEndpoint_MessageReceivedAsync(IMessageEndpoint<TEnvelope> endpoint, TEnvelope msg, DataEndpoint sender)
        {
            try
            {
                if (!(msg.Payload.Is<IMessageResponse>()))
                {
                    return Task.CompletedTask;
                }
                
                var seq = msg.Payload.As<IMessageResponse>().GetMsgRef();
                RequestContext context;

                lock (pendingRequests)
                {
                    //finding the context using the reference of the message
                    if (!pendingRequests.ContainsKey(seq))
                    {
                        //context not found
                        return Task.CompletedTask;
                    }

                    context = pendingRequests[seq];
                }
                //adds the response to the context
                context.ReplyMsg.Add(msg);

                IMessageEnvelope reqmsg = context.Req;

                if (!(reqmsg.Payload.Is<IMessageRequest>()))
                {
                    return Task.CompletedTask;
                }

                if (!(msg.Payload.Is<IMessageChunkResponse>()))
                {
                    return Task.CompletedTask;
                }
                if (!(reqmsg.Payload.Is<IMessageChunkRequest>()))
                {
                    return Task.CompletedTask;
                }

                //chunk management
                //rpc call ends when
                //chunk is disabled or we have received the last respons message
                if (reqmsg.Payload.As<IMessageChunkRequest>().GetChunkType() == ChunkTypes.Disabled 
                    || msg.Payload.As<IMessageChunkResponse>().GetMsgNum() == msg.Payload.As<IMessageChunkResponse>().GetMsgTot())
                {
                    Debug.WriteLine($@"Request filled {seq} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");

                    context.Event.Set();
                }
            }
            catch (Exception ex)
            {
                OnInnerException(new InternalProtocolException($@"Datatransport_DataReceivedAsync {msg.GetType().Name} exception", ex));
            }
            return Task.CompletedTask;
        }

        public Task<CallResult<TEnvelope>> Call(TEnvelope req)
        {
            return this.Call(req, TimeSpan.FromSeconds(5));
        }

        public async Task<CallResult<TEnvelope>> Call(TEnvelope req, TimeSpan timeout)
        {
            if (!this.Available)
                return new CallResult<TEnvelope>() { Result = CallResults.NotAvailable };

            CallResults result = CallResults.Ok;

            if (!(req.Payload.Is<IMessageRequest>()))
            {
                throw new ArgumentProtocolException($@"Message must implement {nameof(IMessageRequest)}");
            }

            var requestContext = new RequestContext(req);

            try
            {
                lock (pendingRequests)
                {
                    pendingRequests.Add(req.Payload.As<IMessageRef>().GetMsgRef(), requestContext);
                }

                await this.SendAsync(req).ConfigureAwait(false);
                Debug.WriteLine($@"Request cli sent {req.Payload.As<IMessageRef>().GetMsgRef()} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");

                bool istimeout = !requestContext.Event.WaitOne(timeout);
                if (istimeout) result = CallResults.Timeout;
            }
            catch (ThreadInterruptedException)
            {
                
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                lock (pendingRequests)
                {
                    pendingRequests.Remove(req.Payload.As<IMessageRef>().GetMsgRef());
                }

            }

            return new CallResult<TEnvelope>() { ResponseList = requestContext.ReplyMsg, Result = result };
        }

    }
}
