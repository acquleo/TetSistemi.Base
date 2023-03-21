using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TetSistemi.Protocol.Serializer
{
    /// <summary>
    /// Definisce l'interfaccia che restituisce un serializer xml a partire dal tipo di messaggio
    /// </summary>
    public interface IJsonMessageSerializerInfo
    {

        /// <summary>
        /// Ritorna il serializer del messaggio corrispondente
        /// </summary>
        /// <returns>Restituisce l'oggetto XmlSerializer.</returns>
        Type GetJsonType(string type);
    }
}
