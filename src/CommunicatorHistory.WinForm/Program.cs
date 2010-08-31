using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace CommunicatorHistory.WinForm
{
    static class Program
    {
        static string _historyFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
            "IM History.txt");
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var communicator = new Communicator(3);
            communicator.ConversationEnded += (s, e) => new SaveConversationToFile(_historyFile).Save(e.EventData);

            var notifyIcon = MakeNotifyIcon();
            notifyIcon.Visible = true;
            Application.Run();
            notifyIcon.Visible = false;
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
                Process.Start(_historyFile);
            else
                MessageBox.Show("History file does not exist");
        }
    }
}
