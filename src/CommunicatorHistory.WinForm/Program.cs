﻿using System;
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
            ConfigurationManager.AppSettings["FileName"]);
        
        [STAThread]
        static void Main()
        {
            try
            {
                _log.InfoFormat("User: {0}", Environment.UserName);

                while (CommunicatorNotRunning())
                {
                    _log.Info("Communicator is not running. Will try again in 5 seconds");
                    System.Threading.Thread.Sleep(5000);
                }

                var communicator = new Communicator(GetRecordHistoryInterval());
                communicator.ConversationEnded += (s, e) => new SaveConversationToFile(_historyFile).Save(e.EventData);

                var notifyIcon = MakeNotifyIcon();
                notifyIcon.Visible = true;
                Application.Run();
                notifyIcon.Visible = false;
                communicator.Dispose();
            }
            catch (Exception e)
            {
                _log.Error("Exception caught", e);
            }
        }

        static bool CommunicatorNotRunning()
        {
            return Convert.ToInt32(Microsoft.Win32.Registry.CurrentUser
                                    .OpenSubKey("Software").OpenSubKey("IM Providers")
                                    .OpenSubKey("Communicator").GetValue("UpAndRunning", 1)) != 2;
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
            notifyIcon.Icon = Properties.Resources.app_icon;
            notifyIcon.DoubleClick += ViewHistory;
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
