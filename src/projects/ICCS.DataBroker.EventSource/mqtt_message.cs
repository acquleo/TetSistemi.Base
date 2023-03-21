using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCS.DataBroker.EventSource
{
    

    /// <summary>
    /// Define a data value message
    /// </summary>
    public class mqtt_message
    {
        /// <summary>
        /// data type of the event
        /// </summary>
        public string content_type { get; set; }
        /// <summary>
        /// data type of the event
        /// </summary>
        public byte qos { get; set; }
        /// <summary>
        /// data type of the event
        /// </summary>
        public string responsetopic { get; set; }
        /// <summary>
        /// data type of the event
        /// </summary>
        public string topic { get; set; }
        /// <summary>
        /// retain
        /// </summary>
        public bool retain { get; set; }
        /// <summary>
        /// dava value
        /// </summary>
        public string data { get; set; }

    }

}
