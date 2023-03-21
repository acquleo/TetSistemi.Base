using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TetSistemi.Protocol.Serializer
{
    public class StringWriterWithEncoding : StringWriter
    {
        public StringWriterWithEncoding(Encoding encoding)
        {
            Encoding = encoding;
        }

        public override Encoding Encoding { get; }
    }
}
