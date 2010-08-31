using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicatorHistory
{
    public interface ISaveConversation
    {
        void Save(IConversation conversation);
    }
}
