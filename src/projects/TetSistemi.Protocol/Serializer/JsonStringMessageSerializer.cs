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
    public class JsonStringMessageSerializer : IMessageSerializer<string>
    {
        JsonSerializer serializer = new JsonSerializer();
        IJsonMessageSerializerInfo info;
        public JsonStringMessageSerializer(IJsonMessageSerializerInfo info) 
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

        public IMessage Deserialize(string data, string contentType = null)
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
                using (var sread = new StringReader(data))
                {
                    using (var jread = new JsonTextReader(sread))
                    {
                        obj = serializer.Deserialize(jread, contract) as IMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DeSerializeException($@"Xml Deserialize Exception {contentType}: ", ex);

            }
            return obj;
        }

        public string Serialize(IMessage msg, string contentType = null)
        {
            string serializerName = contentType;
            Type type = info.GetJsonType(serializerName);
            if (type == null)
            {
                throw new SerializerNotFoundException($@"Serializer not found: {serializerName}");
            }
            try
            {
                using (var swrite = new StringWriterWithEncoding(this.Encoding))
                {
                    using (var jwrite = new JsonTextWriter(swrite))
                    {
                        this.serializer.Serialize(jwrite, msg, type);
                        jwrite.Flush();
                    }

                    return swrite.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new SerializeException($@"Xml Serialize Exception {contentType}: ", ex);

            }

           
        }
    }
}
