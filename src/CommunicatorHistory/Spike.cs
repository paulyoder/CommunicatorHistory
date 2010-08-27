using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicatorHistory
{
    public class Spike
    {
        public List<Communication> GetCommunications(string history)
        {
            var communications = new List<Communication>();

            var indexes = new List<Tuple<string, int, string>>();
            indexes.AddRange(GetIdIndexes(history, "imsendtimestamp"));
            indexes.AddRange(GetIdIndexes(history, "imsendname"));
            indexes.AddRange(GetIdIndexes(history, "imcontent"));

            Communication currentCommunication = null;

            foreach (var index in indexes.OrderBy(x => x.Item2))
            {
                if (index.Item1 == "imsendtimestamp")
                {
                    if (currentCommunication != null)
                        communications.Add(currentCommunication);
                    currentCommunication = new Communication();
                    currentCommunication.TimeStamp = DateTime.Now;
                }
                if (index.Item1 == "imsendname")
                    currentCommunication.Sender = index.Item3;
                if (index.Item1 == "imcontent")
                    currentCommunication.Messages.Add(index.Item3);
            }

            return communications;
        }

        private List<Tuple<string, int, string>> GetIdIndexes(string history, string idValue)
        {
            var idString = string.Format("id={0}", idValue);
            var indexes = new List<Tuple<string, int, string>>();
            var index = 0;
            while (index < history.Length)
            {
                index = history.IndexOf(idString, index);
                if (index == -1)
                    break;
                else 
                    indexes.Add(new Tuple<string,int,string>(idValue, index, GetInnerText(history, index)));
                index++;
            }

            return indexes;
        }

        private string GetInnerText(string history, int index)
        {
            var openBracketIndex = history.LastIndexOf("<", index);
            return new XmlUtilities().InnerText(history, openBracketIndex);
        }
    }
}
