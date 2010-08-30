using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicatorHistory
{
    public interface ICommunicator : IDisposable
    {
        IEnumerable<IConversation> ActiveConversations { get; }
        event EventHandler<EventArgs<IConversation>> ConversationStarted;
        event EventHandler<EventArgs<IConversation>> ConversationEnded;
    }
}
