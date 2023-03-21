using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using TetSistemi.Protocol.Exceptions;

namespace TetSistemi.Protocol.Serializer
{
    public class XmlByteMessageSerializer : IMessageSerializer<byte[]>
    {
        IXmlMessageSerializerInfo info;
        public XmlByteMessageSerializer(IXmlMessageSerializerInfo info) 
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
            
            var serializer = this.info.GetXmlSerializer(payload);
            if (serializer == null)
            {
                throw new SerializerNotFoundException($@"Serializer not found: {contentType}");
            }

            try
            {
                using (var mread = new MemoryStream(data))
                {
                    using (var sread = new StreamReader(mread, this.Encoding, true))
                    {
                        obj = serializer.Deserialize(sread) as IMessage;
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

            //rimuovo il namespace dall'xml
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            //ricavo il serializer
            XmlSerializer serializer = info.GetXmlSerializer(serializerName);
            if (serializer == null)
            {
                throw new SerializerNotFoundException($@"Serializer not found: {serializerName}");
            }

            try
            {
                using (var mwrite = new MemoryStream())
                {
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                    {
                        // If set to true XmlWriter would close MemoryStream automatically and using would then do double dispose
                        // Code analysis does not understand that. That's why there is a suppress message.
                        CloseOutput = false,
                        Encoding = Encoding,
                        OmitXmlDeclaration = false,
                        Indent = true
                    };
                    using (XmlWriter xw = XmlWriter.Create(mwrite, xmlWriterSettings))
                    {
                        serializer.Serialize(xw, msg, namespaces);
                    }

                    return mwrite.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new SerializeException($@"Xml Serialize Exception {contentType}: ", ex);
            }
        }
    }
}
