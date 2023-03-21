using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TetSistemi.Base.Xml
{
    /// <summary>
    /// Utility di serializazione
    /// </summary>
    public static class XmlSerializationHelper
    {
        static Dictionary<string, XmlSerializer> map = new Dictionary<string, XmlSerializer>();

        public static void ClearSerializerCache()
        {
            map.Clear();
        }

        /// <summary>
        /// Restituisce l'XmlSerializer del tipo specificato
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>XmlSerializer</returns>
        public static XmlSerializer GetSerializer<T>() where T : class
        {
            Type t = typeof(T);
            return GetSerializer(t);
        }

        /// <summary>
        /// Restituisce l'XmlSerializer del tipo specificato
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>XmlSerializer</returns>
        public static XmlSerializer GetSerializer(Type t)
        {
            lock (map)
            {
                if (!map.ContainsKey(t.FullName))
                {
                    map.Add(t.FullName, new XmlSerializer(t));
                }
                return map[t.FullName];
            }
        }

        /// <summary>
        /// Deserializza un oggetto da file xml
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da deserializzare</typeparam>
        /// <param name="data">Stringa sorgente</param>
        /// <returns>Oggetto deserializzato</returns>
        public static T Deserialize<T>(string data) where T : class
        {
            Type t = typeof(T);
            return Deserialize(t, data) as T;
        }
        /// <summary>
        /// Deserializza un oggetto da file xml come estensione di string
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da deserializzare</typeparam>
        /// <param name="data">Stringa sorgente</param>
        /// <returns>Oggetto deserializzato</returns>
        public static T XmlDeserializeFromString<T>(this string data) where T : class
        {
            return Deserialize<T>(data);
        }
        /// <summary>
        /// Deserializza un oggetto da file xml
        /// </summary>
        /// <param name="type">Tipo dell'oggetto da deserializzare</typeparam>
        /// <param name="data">Stringa sorgente</param>
        /// <returns>Oggetto deserializzato</returns>
        public static object Deserialize(Type type, string data)
        {
            
            XmlSerializer serializer = GetSerializer(type);
            using (StringReader reader = new StringReader(data))
            {
                return serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Deserializza un oggetto da file xml
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da deserializzare</typeparam>
        /// <param name="path">Percorso completo del file</param>
        /// <returns>Oggetto deserializzato</returns>
        public static T DeserializeFromFile<T>(string path) where T : class
        {
            Type t = typeof(T);
            return DeserializeFromFile(t,path) as T;
        }

        /// <summary>
        /// Deserializza un oggetto da file xml
        /// </summary>
        /// <param name="type">Tipo dell'oggetto da deserializzare</typeparam>
        /// <param name="path">Percorso completo del file</param>
        /// <returns>Oggetto deserializzato</returns>
        public static object DeserializeFromFile(Type type, string path)
        {
            XmlSerializer serializer = GetSerializer(type);
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    return serializer.Deserialize(reader);
                }
            }
        }
        /// <summary>
        /// Serializza un oggetto su string
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da serializzare</typeparam>
        /// <param name="data">Oggetto da serializzare</param>
        public static string Serialize<T>(T data) where T : class
        {
            Type t = typeof(T);
            return Serialize(t, data);
        }

         /// <summary>
        /// Serializza un oggetto su string
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da serializzare</typeparam>
        /// <param name="data">Oggetto da serializzare</param>
        public static string XmlSerializeToString<T>(this T data) where T : class
        {
            return Serialize<T>(data);
        }
        /// <summary>
        /// Serializza un oggetto su string
        /// </summary>
        /// <param name="type">Tipo dell'oggetto da serializzare</typeparam>
        /// <param name="data">Oggetto da serializzare</param>
        public static string Serialize(Type type, object data)
        {
            return Serialize(type, data, null);
        }

        public static string Serialize(Type type, object data, Encoding encoding)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            if(encoding!=null) settings.Encoding = encoding;
            XmlSerializerNamespaces names = new XmlSerializerNamespaces();
            names.Add(string.Empty, string.Empty);
            XmlSerializer serializer = GetSerializer(type);
            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
//                serializer.Serialize(writer, data, names);
                serializer.Serialize(writer, data, names);
                return builder.ToString();
            }
        }

        /// <summary>
        /// Serializza un oggetto su file xml
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da serializzare</typeparam>
        /// <param name="data">Oggetto da serializzare</param>
        /// <param name="path">Percorso completo del file</param>
        public static void SerializeToFile<T>(T data,string path) where T : class
        {            
            SerializeToFile<T>(data, path,true);
        }

        /// <summary>
        /// Serializza un oggetto su file xml
        /// </summary>
        /// <typeparam name="T">Tipo dell'oggetto da serializzare</typeparam>
        /// <param name="data">Oggetto da serializzare</param>
        /// <param name="path">Percorso completo del file</param>
        public static void SerializeToFile<T>(T data, string path, bool excludeNamespace) where T : class
        {
            Type t = typeof(T);
            SerializeToFile(t, data, path, excludeNamespace);
        }

        /// <summary>
        /// Serializza un oggetto su file xml
        /// </summary>
        /// <param name="type">Tipo dell'oggetto da serializzare</typeparam>
        /// <param name="data">Oggetto da serializzare</param>
        /// <param name="path">Percorso completo del file</param>
        public static void SerializeToFile(Type type, object data, string path,bool excludeNamespace)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            if (excludeNamespace)
            {
                settings.OmitXmlDeclaration = true;
            }
            settings.Indent = true;
            
            XmlSerializerNamespaces names = new XmlSerializerNamespaces();
            names.Add(string.Empty, string.Empty);
            XmlSerializer serializer = GetSerializer(type);
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter sWriter = new StreamWriter(fs))
                {
                    using (XmlWriter writer = XmlWriter.Create(sWriter, settings))
                    {
                        if (excludeNamespace)
                            serializer.Serialize(writer, data, names);
                        else
                            serializer.Serialize(writer, data);
                    }
                }
            }
        }
    }
}
