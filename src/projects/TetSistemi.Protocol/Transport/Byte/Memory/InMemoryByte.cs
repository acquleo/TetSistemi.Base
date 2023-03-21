using ICCS.DataBroker.MQTT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TetSistemi.Protocol.Transport.Byte.Memory.InMemoryByteClient;

namespace TetSistemi.Protocol.Transport.Byte.Memory
{
    public class InMemoryByte
    {
        static object intancelock = new object();
        static InMemoryByte instance;
        public static InMemoryByte Instance
        {
            get
            {
                lock(intancelock)
                {
                    if(instance == null)
                        instance = new InMemoryByte();
                    return instance;
                }
            }
        }

        InMemoryByteClient client;
        InMemoryByteClient server;

        internal InMemoryByte()
        {

        }

        public void SetAs(InMemoryByteClient client, Sides side)
        {
            if(side== Sides.Client)
                this.client= client;
            if (side == Sides.Server)
                this.server = client;
        }


        public void Publish(byte[] data, Sides side)
        {
            if (side == Sides.Client)
                this.server?.Pushdata(data);
            if (side == Sides.Server)
                this.client?.Pushdata(data);
        }


    }
}
