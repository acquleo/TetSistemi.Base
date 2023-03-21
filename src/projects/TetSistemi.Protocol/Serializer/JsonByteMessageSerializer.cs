using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading;
using TetSistemi.Protocol.Exceptions;

namespace TetSistemi.Protocol.Serializer
{
    public class JsonByteMessageSerializer : IMessageSerializer<byte[]>
    {
        JsonSerializer serializer = new JsonSerializer();
        IJsonMessageSerializerInfo info;
        public JsonByteMessageSerializer(IJsonMessageSerializerInfo info) 
        { 
            this.info = info;
            this.Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Returns or Sets the XML encoding
        /// </summary>
        public Encoding Encoding
        {
            get;
            set;
        }

        public IMessage Deserialize(byte[] data, string contentType = null)
        {
            //deserializzo l'XML
            IMessage obj = null;

            var payload = contentType;
            if (payload == null)
            {
                throw new SerializerNotFoundException($@"Serializer not found: {contentType}");
            }
            var contract = this.info.GetJsonType(payload);

            try
            {
                using (var mread = new MemoryStream(data))
                {
                    using (var sread = new StreamReader(mread, this.Encoding, true))
                    {
                        using (var jread = new JsonTextReader(sread))
                        {
                            obj = serializer.Deserialize(jread, contract) as IMessage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DeSerializeException($@"Xml Deserialize Exception  {contentType}: ", ex);

            }

            return obj;
        }

        public byte[] Serialize(IMessage msg, string contentType = null)
        {
            string serializerName = contentType;
            Type type = info.GetJsonType(serializerName);
            if (type == null)
            {
                throw new SerializerNotFoundException($@"Serializer not found: {serializerName}");
            }

            try
            {
                using (var mwrite = new MemoryStream())
                {
                    using (var swrite = new StreamWriter(mwrite))
                    {

                        using (var jwrite = new JsonTextWriter(swrite))
                        {
                            this.serializer.Serialize(jwrite, msg, type);
                            jwrite.Flush();
                            return mwrite.ToArray();
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                throw new SerializeException($@"Xml Serialize Exception {contentType}: ", ex);
            }
        }
    }
}
