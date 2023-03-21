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
    public class XmlStringMessageSerializer : IMessageSerializer<string>
    {
        IXmlMessageSerializerInfo info;
        public XmlStringMessageSerializer(IXmlMessageSerializerInfo info) 
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
            
            var serializer = this.info.GetXmlSerializer(payload);
            if (serializer == null)
            {
                throw new SerializerNotFoundException($@"Serializer not found: {contentType}");
            }

            try
            {
                using (var sread = new StringReader(data))
                {
                    obj = serializer.Deserialize(sread) as IMessage;
                }
            }
            catch (Exception ex)
            {
                throw new DeSerializeException($@"Xml Deserialize Exception  {contentType}: ", ex);

            }

            return obj;
        }

        public string Serialize(IMessage msg, string contentType = null)
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
              XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
              {
                  // If set to true XmlWriter would close MemoryStream automatically and using would then do double dispose
                  // Code analysis does not understand that. That's why there is a suppress message.
                  CloseOutput = false,
                  Encoding = Encoding,
                  OmitXmlDeclaration = false,
                  Indent = true
              };
            using (var swrite = new StringWriterWithEncoding(this.Encoding))
            {
                using (XmlWriter xw = XmlWriter.Create(swrite, xmlWriterSettings))
                {
                    serializer.Serialize(xw, msg, namespaces);
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
