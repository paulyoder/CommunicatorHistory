using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicatorAPI;
using System.Runtime.InteropServices;
using System.Timers;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;

namespace CommunicatorHistory
{
    class Program
    {
        static Messenger _messenger;
        static List<IMessengerConversationWndAdvanced> _conversations;
        static Dictionary<IMessengerConversationWndAdvanced, string> _conversationsHistory;
        static Timer _timer;

        static void Main(string[] args)
        {
            _conversations = new List<IMessengerConversationWndAdvanced>();
            _conversationsHistory = new Dictionary<IMessengerConversationWndAdvanced, string>();
            
            _messenger = new Messenger();
            _messenger.OnIMWindowCreated += messenger_OnIMWindowCreated;
            _messenger.OnIMWindowDestroyed += _messenger_OnIMWindowDestroyed;
            
            _timer = new Timer(2000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            Console.ReadLine();
            
            _messenger.OnIMWindowCreated -= messenger_OnIMWindowCreated;
            _messenger.OnIMWindowDestroyed -= _messenger_OnIMWindowDestroyed;
            Marshal.ReleaseComObject(_messenger);
        }

        static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var conversation in _conversations)
            {
                _conversationsHistory[conversation] = conversation.History;
            }
        }

        static void _messenger_OnIMWindowDestroyed(object pIMWindow)
        {
            _timer.Stop();
            var conversation = _conversations.Find(x => object.ReferenceEquals((object)x, pIMWindow));
            if (conversation != null)
            {
                SaveConversation(conversation);
                
                _conversations.Remove(conversation);
                Marshal.ReleaseComObject(conversation);
                conversation = null;
            }
        }

        private static void SaveConversation(IMessengerConversationWndAdvanced conversation)
        {
            var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fileName = Path.Combine(workingDirectory, "History.txt");
            
            var history = _conversationsHistory[conversation];
            using (var file = File.CreateText(fileName))
            {
                foreach (var communication in new Spike().GetCommunications(history))
                {
                    file.WriteLine("Person: {0}", communication.Sender);
                    foreach (var message in communication.Messages)
                        file.WriteLine("\t- {0}", message);
                }
            }
            
            Process.Start(fileName);

            
        }

        static void messenger_OnIMWindowCreated(object pIMWindow)
        {
            _conversations.Add((IMessengerConversationWndAdvanced)pIMWindow);
            Console.WriteLine("New Conversation Created!");
        }

        static List<Communication> ConvertToCommunications(string history)
        {
            var list = new List<Communication>();
            var fixedHistory = FixHtmlConversation(history);
            var xmlHistory = XElement.Parse(fixedHistory);
            foreach (var xmlCommunication in xmlHistory.Descendants().Where(x => x.Attributes().Any(a => a.Name == "id" && a.Value == "imsendname")))
            {
                var com = new Communication();
                com.Sender = xmlCommunication.Descendants().First().Value;

                list.Add(com);
            }
            return null;
        }

        static string FixHtmlConversation(string conversation)
        {
            conversation = conversation.Replace("nbsp;", " ");
            var fixedHtml = new StringBuilder("<Conversation>");
            var currentIndex = 0;
            var currentCharacter = "";
            var inElement = false;
            var addApostrophe = false;
            while (currentIndex < conversation.Length)
            {
                currentCharacter = conversation.Substring(currentIndex, 1);
                if (((currentIndex + 7) < conversation.Length) && (conversation.Substring(currentIndex, 7) == "<OBJECT"))
                {
                    currentIndex = conversation.IndexOf("</OBJECT>", currentIndex) + 9;
                    continue;
                }
                if (currentCharacter == "<")
                    inElement = true;
                else if (currentCharacter == ">")
                    inElement = false;
                if (addApostrophe && (currentCharacter == " " || currentCharacter == ">"))
                {
                    fixedHtml.Append("\"");
                    addApostrophe = false;
                }
                fixedHtml.Append(conversation.Substring(currentIndex, 1));
                if (inElement && currentCharacter == "=" && conversation.Substring(currentIndex + 1, 1) != "\"")
                {
                    fixedHtml.Append("\"");
                    addApostrophe = true;
                }
                currentIndex++;
            }
            fixedHtml.Append("</Conversation>");
            return fixedHtml.ToString();
        }
    }
}
