using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicatorHistory
{
    public interface IConversation : IDisposable
    {
        DateTime StartTime { get; }
        DateTime EndTime { get; }
        IEnumerable<Communication> Communications { get; }
        IEnumerable<string> Contacts { get; }
    }
}
