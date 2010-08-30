using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace CommunicatorHistory.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var communicator = new Communicator(2);
            communicator.ConversationEnded += OnConversationEnded;

            System.Console.ReadLine();
        }

        static void OnConversationEnded(object sender, EventArgs<IConversation> e)
        {
            var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fileName = Path.Combine(workingDirectory, "History.txt");
            
            using (TextWriter file = File.CreateText(fileName))
            {
                foreach (var communication in e.EventData.Communications)
                {
                    file.WriteLine("{0} ({1})", communication.Sender, communication.TimeStamp);
                    foreach (var message in communication.Messages)
                        file.WriteLine("\t- {0}", message);
                    file.WriteLine();
                }
            }

            Process.Start(fileName);
        }
    }
}
