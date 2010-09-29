using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicatorAPI;
using System.Runtime.InteropServices;
using System.Timers;

namespace CommunicatorHistory
{
    public class Communicator : ICommunicator
    {
        private Messenger _messenger;
        private List<Conversation> _activeConversations;
        private Timer _timer;
        public event EventHandler<EventArgs<IConversation>> ConversationStarted;
        public event EventHandler<EventArgs<IConversation>> ConversationEnded;

        public Communicator(int recordHistoryInterval)
        {
            _activeConversations = new List<Conversation>();

            _timer = new Timer(recordHistoryInterval * 1000);
            _timer.Elapsed += TimerElapsed;
            _timer.Start();

            _messenger = new Messenger();
            _messenger.OnIMWindowCreated += MessengerOnIMWindowCreated;
            _messenger.OnIMWindowDestroyed += MessengerOnIMWindowDestroyed;
        }

        public IEnumerable<IConversation> ActiveConversations
        {
            get { return _activeConversations.Select(x => (IConversation)x); }
        }

        public void Dispose()
        {
            _timer.Dispose();

            foreach (var conversation in _activeConversations)
                conversation.Dispose();
            _activeConversations.Clear();

            _messenger.OnIMWindowCreated -= MessengerOnIMWindowCreated;
            _messenger.OnIMWindowDestroyed -= MessengerOnIMWindowDestroyed;
            Marshal.ReleaseComObject(_messenger);
            _messenger = null;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            foreach (var conversation in _activeConversations)
                conversation.RecordCommunications();

            _timer.Start();
        }

        private void MessengerOnIMWindowCreated(object pIMWindow)
        {
            var window = (IMessengerConversationWndAdvanced)pIMWindow;
            var newConversation = new Conversation(window);
            _activeConversations.Add(newConversation);

            if (ConversationStarted != null)
                ConversationStarted(this, new EventArgs<IConversation>(newConversation));
        }

        private void MessengerOnIMWindowDestroyed(object pIMWindow)
        {
            var window = (IMessengerConversationWndAdvanced)pIMWindow;
            var windowInList = _activeConversations.SingleOrDefault(x => x.ConversationWindow.Equals(window));
            if (windowInList != null)
            {
                _activeConversations.Remove(windowInList);

                if (windowInList.Communications.Count() > 0 && ConversationEnded != null)
                    ConversationEnded(this, new EventArgs<IConversation>(windowInList));
            }
        }
    }
}
