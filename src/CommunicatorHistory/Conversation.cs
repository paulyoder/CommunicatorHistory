using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicatorHistory
{
    public class Conversation
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Communication> Communications { get; set; }
    }
}
