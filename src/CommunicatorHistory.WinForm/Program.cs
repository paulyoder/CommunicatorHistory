using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using log4net;
using System.Configuration;

namespace CommunicatorHistory.WinForm
{
    static class Program
    {
        static ILog _log = LogManager.GetLogger("WinForm");

        static string _historyFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
            "IM History.txt");
        
        [STAThread]
        static void Main()
        {
            try
            {
                _log.InfoFormat("User: {0}", Environment.UserName);
                var communicator = new Communicator(GetRecordHistoryInterval());
                communicator.ConversationEnded += (s, e) => new SaveConversationToFile(_historyFile).Save(e.EventData);

                var notifyIcon = MakeNotifyIcon();
                notifyIcon.Visible = true;
                Application.Run();
                notifyIcon.Visible = false;
            }
            catch (Exception e)
            {
                _log.Error("Exception caught", e);
            }
        }

        static int GetRecordHistoryInterval()
        {
            return Int32.Parse(ConfigurationManager.AppSettings["RecordHistoryInterval"]);
        }

        static NotifyIcon MakeNotifyIcon()
        {
            var notifyIcon = new NotifyIcon();
            notifyIcon.ContextMenu = MakeContextMenu();
            notifyIcon.Text = "Communicator History";
            notifyIcon.Icon = new Icon("app-icon.ico");
            return notifyIcon;
        }

        static ContextMenu MakeContextMenu()
        {
            var contextMenu = new ContextMenu();
            
            var exitMenuItem = new MenuItem();
            exitMenuItem.Text = "Exit";
            exitMenuItem.Click += (s, e) => Application.Exit();

            var viewHistoryMenuItem = new MenuItem();
            viewHistoryMenuItem.Text = "View History";
            viewHistoryMenuItem.Click += ViewHistory;

            contextMenu.MenuItems.Add(viewHistoryMenuItem);
            contextMenu.MenuItems.Add(exitMenuItem);

            return contextMenu;
        }

        static void ViewHistory(object sender, EventArgs e)
        {
            if (File.Exists(_historyFile))
            {
                try
                {
                    Process.Start(_historyFile);
                }
                catch (Exception ex)
                {
                    _log.Error("Exception caught while viewing history file", ex);
                    MessageBox.Show("Ah Snap!\n\nThere was a problem viewing the history file. Email Paul Yoder");
                }
            }
            else
            {
                MessageBox.Show("History file does not exist");
            }
        }
    }
}
