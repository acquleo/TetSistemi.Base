using Newtonsoft.Json;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using TetSistemi.Protocol.Converter.BYTE;
using TetSistemi.Protocol.Exceptions;
using TetSistemi.Protocol.Serializer;

namespace TetSistemi.Protocol.Enveloper.Byte
{

    /// <summary>
    /// Definisce i possibili header del serializzatore XML
    /// </summary>
    public enum MessageLengthCountTypes
    {
        /// <summary>
        /// Il valore del campo Message Length include la sua dimensione
        /// </summary>
        IncludeMessageLength,
        /// <summary>
        /// Il valore del campo Message Length non include la sua dimensione
        /// </summary>
        ExcludeMessageLength
    }

    /// <summary>
    /// Implemena l'interfaccia IMessageParser per la serializzazione xml di messaggi su uno stream
    /// </summary>
    public class ByteArrayMessageEnveloper : IByteArrayMessageEnveloper
    {
        #region Const

        private const int HeaderLength = 4;

        #endregion

        #region Private Variables

        IMessageSerializer<byte[]> serializer;
        #endregion

        #region Constructor

        /// <summary>
        /// Costruttore.
        /// </summary>
        /// <param name="_xmlMessageSerializerInfo">IXmlMessageSerializerInfo</param>
        public ByteArrayMessageEnveloper(IMessageSerializer<byte[]> serializer)
        {
            this.serializer=serializer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns or Sets the header Message Length field in Big Endian
        /// </summary>
        public bool BigEndianMessageLength
        {
            get;
            set;
        }

        /// <summary>
        /// Returns or Sets the header Message Length content type
        /// </summary>
        public MessageLengthCountTypes MessageLengthCountType
        {
            get;
            set;
        }

        #endregion

        #region IMessageParser Members
        /// <summary>
        /// Ritorna se dai dati a disposizione è possibile calcolare la lunghezza COMPLESSIVA del messaggio.
        /// </summary>
        /// <param name="data">Byte array per il calcolo della lunghezza dei dati</param>
        /// <returns>Ritorna se dai dati a disposizione è possibile calcolare la lunghezza COMPLESSIVA del messaggio.</returns>
        public bool CanReadLength(byte[] data)
        {
            //servono 4 bytes x la lunghezza del messaggio
            if (data.Length >= HeaderLength)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Ritorna la lunghezza COMPLESSIVA del messaggio
        /// </summary>
        /// <param name="data">Byte array per il calcolo della lunghezza COMPLESSIVA del messaggio</param>
        /// <returns>Ritorna la lunghezza COMPLESSIVA del messaggio</returns>
        public uint GetLength(byte[] data)
        {
            //lunghezza del messaggio
            byte[] field = new byte[4];
            Array.Copy(data, field, 4);
            if (BigEndianMessageLength) Array.Reverse(field);
            uint lenght = BitConverter.ToUInt32(field, 0);
            if (MessageLengthCountType == MessageLengthCountTypes.ExcludeMessageLength) lenght = lenght + sizeof(int);
            return lenght;
        }
        /// <summary>
        /// Effettua la deserializzazione di un messaggio da un array di byte.
        /// </summary>
        /// <param name="data">Byte array per la deserializzazione.</param>
        /// <returns>ritorna la deserializzazione di un messaggio da un array di byte.</returns>
        /// <remarks>
        /// Effettua la deserializzazione di un messaggio da un array di byte.
        /// Errori di parsing devono essere generati ereditando da MessageParseException.
        /// </remarks>
        public EmptyMessageEnvelope Wrap(byte[] data)
        {
            string messageName = string.Empty;

            //estraggo solo l'array xml
            int index = 0;
            if (BigEndianMessageLength) Array.Reverse(data, index, sizeof(int));
            int fulllenght = BitConverter.ToInt32(data, index);
            if (MessageLengthCountType == MessageLengthCountTypes.ExcludeMessageLength) fulllenght = fulllenght + sizeof(int);

            int headerlength = HeaderLength;
            int xmlLength = fulllenght - HeaderLength;
            index += sizeof(int);

            ushort messageNameLen = BitConverter.ToUInt16(data, index);
            headerlength = sizeof(ushort) + messageNameLen;
            index += sizeof(short);

            messageName = Encoding.UTF8.GetString(data, index, messageNameLen);

            index += messageNameLen;
            xmlLength = xmlLength - sizeof(short) - messageNameLen;

            byte[] body = new byte[xmlLength];
            Array.Copy(data, index, body, 0, xmlLength);

            //deserializzo l'XML
            object obj = null;
            try
            {
                // read the json from a stream
                // json size doesn't matter because only a small piece is read at a time from the HTTP request
                obj = serializer.Deserialize(body, messageName);

            }
            catch (Exception ex)
            {
                throw new SerializeException($@"error serialize {messageName}", ex);

            }

            if (obj is IMessage) return new EmptyMessageEnvelope() { Payload = (IMessage)obj };
            else return null;
            
        }

        /// <summary>
        /// Effettua la serializzazione di un messaggio in un array di byte
        /// </summary>
        /// <param name="data">Array di byte per la serializzazione di un messaggio.</param>
        /// <returns>Ritorna la serializzazione di un messaggio.</returns>
        public byte[] Unwrap(EmptyMessageEnvelope msg)
        {
            return SerializeMessage(msg, true);
        }
        /// <summary>
        /// Effettua la serializzazione di un messaggio in un array di byte
        /// </summary>
        /// <param name="data">Array di byte per la serializzazione di un messaggio.</param>
        /// <param name="includeHeader">Parametro per definire se includere Header del messaggio nella serializzazione.</param>
        /// <returns>Ritorna la serializzazione di un messaggio.</returns>
        public byte[] SerializeMessage(EmptyMessageEnvelope data, bool includeHeader)
        {
            //ricavo il serializer
            string serializerName = data.Payload.GetType().Name;

            //aggiungo l'header
            int index = 0;
            int fullmessagelength = 0;
            byte[] body = this.serializer.Serialize(data.Payload, serializerName);
            byte[] header = new byte[4];
            byte[] header2 = null;

            fullmessagelength += body.Length + HeaderLength;

            byte[] messageNameArray;

            messageNameArray = Encoding.UTF8.GetBytes(serializerName);

            ushort messageNameLength = (ushort)messageNameArray.Length;
            byte[] messageNameLengthArray = BitConverter.GetBytes(messageNameLength);
            fullmessagelength += sizeof(ushort);
            fullmessagelength += messageNameLength;
            header2 = new byte[sizeof(ushort) + messageNameLength];
            Array.Copy(messageNameLengthArray, 0, header2, 0, messageNameLengthArray.Length);
            Array.Copy(messageNameArray, 0, header2, 2, messageNameArray.Length);

            int fullmessagelengthTowrite = fullmessagelength;
            if (MessageLengthCountType == MessageLengthCountTypes.ExcludeMessageLength) fullmessagelengthTowrite = fullmessagelength - sizeof(int);
            Array.Copy(BitConverter.GetBytes(fullmessagelengthTowrite), 0, header, 0, 4);
            if (BigEndianMessageLength) Array.Reverse(header, 0, sizeof(int));

            //aggiungo l'xml
            byte[] message = new byte[fullmessagelength];
            Array.Copy(header, 0, message, index, header.Length);
            index += sizeof(int);
            if (header2 != null)
            {
                Array.Copy(header2, 0, message, index, header2.Length);
                index += header2.Length;
            }
            Array.Copy(body, 0, message, index, body.Length);
            if (includeHeader)
                return message;
            else
                return body;
        }



        #endregion
    }
}
