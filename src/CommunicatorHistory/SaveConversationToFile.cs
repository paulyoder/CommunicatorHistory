using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommunicatorHistory
{
    public class SaveConversationToFile : ISaveConversation
    {
        private string _fileName;

        public SaveConversationToFile(string fileName)
        {
            _fileName = fileName;
        }
        
        public void Save(IConversation conversation)
        {
            using (var file = File.AppendText(_fileName))
            {
                WriteHeader(conversation, file);

                foreach (var communication in conversation.Communications)
                {
                    file.WriteLine("{0} ({1})", communication.Sender, communication.TimeStamp);
                    foreach (var message in communication.Messages)
                        file.WriteLine("\t- {0}", message);
                    file.WriteLine();
                }

                WriteFooter(conversation, file);
            }
        }

        private static void WriteHeader(IConversation conversation, StreamWriter file)
        {
            file.WriteLine("**********************************");
            file.WriteLine("* From: {0}", conversation.StartTime);
            file.WriteLine("*   To: {0}", conversation.EndTime);
            file.WriteLine("*");
            file.WriteLine("* Attendance:");

            foreach (var sender in conversation.Contacts.OrderBy(x => x))
                file.WriteLine("*   {0}", sender);

            file.WriteLine("**********************************");
            file.WriteLine();
        }

        private void WriteFooter(IConversation conversation, StreamWriter file)
        {
            file.WriteLine("");
        }
    }
}
