using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicatorAPI;
using System.Runtime.InteropServices;

namespace CommunicatorHistory
{
    public class Conversation : IConversation
    {
        public IMessengerConversationWndAdvanced ConversationWindow { get; private set; }
        private List<Communication> _communications;

        internal Conversation(IMessengerConversationWndAdvanced conversationWindow)
        {
            ConversationWindow = conversationWindow;
            _communications = new List<Communication>();
        }

        public DateTime StartTime
        {
            get { return _communications.Min(x => x.TimeStamp); }
        }
        
        public DateTime EndTime
        {
            get { return _communications.Max(x => x.TimeStamp); }
        }

        public IEnumerable<Communication> Communications
        {
            get { return _communications; }
        }

        public void RecordCommunications()
        {
            if (ConversationWindow.IsClosed)
                return;

            var history = ConversationWindow.History;
            if (history != null)
                _communications = GetCommunications(history);
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject(ConversationWindow);
            ConversationWindow = null;
        }

        private List<Communication> GetCommunications(string history)
        {
            var communications = new List<Communication>();

            var indexes = new List<Tuple<int, string, string>>();
            indexes.AddRange(GetIdIndexes(history, "imsendtimestamp"));
            indexes.AddRange(GetIdIndexes(history, "imsendname"));
            indexes.AddRange(GetIdIndexes(history, "imcontent"));

            Communication currentCommunication = null;

            foreach (var index in indexes.OrderBy(x => x.Item1))
            {
                if (index.Item2 == "imsendtimestamp")
                {
                    if (currentCommunication != null)
                        communications.Add(currentCommunication);
                    currentCommunication = new Communication();
                    currentCommunication.TimeStamp = DateTime.Parse(string.Format("{0} {1}", DateTime.Now.ToShortDateString(), index.Item3));
                }
                else if (index.Item2 == "imsendname")
                    currentCommunication.Sender = index.Item3;
                else if (index.Item2 == "imcontent")
                    currentCommunication.Messages.Add(index.Item3);
            }
            if (currentCommunication != null)
                communications.Add(currentCommunication);

            return communications;
        }

        private List<Tuple<int, string, string>> GetIdIndexes(string history, string idValue)
        {
            var idString = string.Format("id={0}", idValue);
            var indexes = new List<Tuple<int, string, string>>();
            var index = 0;
            while (index < history.Length)
            {
                index = history.IndexOf(idString, index);
                if (index == -1)
                    break;
                else
                    indexes.Add(new Tuple<int, string, string>(index, idValue, GetInnerText(history, index)));
                index++;
            }

            return indexes;
        }

        private string GetInnerText(string history, int index)
        {
            var openBracketIndex = history.LastIndexOf("<", index);
            return history.InnerText(openBracketIndex);
        }
    }
}
