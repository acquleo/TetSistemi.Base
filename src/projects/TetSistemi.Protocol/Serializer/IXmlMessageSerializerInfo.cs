﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TetSistemi.Protocol.Serializer
{
    public interface IXmlMessageSerializerInfo
    {
        XmlSerializer GetXmlSerializer(string type);
    }
}
