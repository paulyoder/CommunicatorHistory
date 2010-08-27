using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicatorHistory
{
    public class Communication
    {
        public Communication()
        {
            Messages = new List<string>();
        }

        public DateTime TimeStamp { get; set; }
        public string Sender { get; set; }
        public List<string> Messages { get; set; }
    }
}
