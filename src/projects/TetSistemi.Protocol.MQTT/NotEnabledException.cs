using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCS.DataBroker.MQTT
{
    public class NotEnabledException: Exception
    {
        public NotEnabledException(string message):base(message)
        {

        }
    }
}
