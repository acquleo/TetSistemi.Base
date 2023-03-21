using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TetSistemi.Protocol
{
    
    /// <summary>
    /// Generic message definition
    /// </summary>
    public static class MessageExtension
    {
        public static bool Is<Tinterface>(this IMessage msg) 
            where Tinterface : IMessage
        {
            return msg is Tinterface;
        }

        public static Tinterface As<Tinterface>(this IMessage msg)
            where Tinterface : IMessage
        {
            return (Tinterface)msg;
        }
    }
}
