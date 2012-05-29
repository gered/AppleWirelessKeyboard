using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using AppleWirelessKeyboard.Views;

namespace AppleWirelessKeyboard
{
    public static class TrayIcon
    {
        static TrayIcon()
        {
            NotifyIcon icon = new NotifyIcon();
            icon.Text = "AppleWirelessKeyboard";
            icon.Icon = new Icon(App.GetResourceStream(new Uri("pack://application:,,,/Gnome-Preferences-Desktop-Keyboard-Shortcuts.ico")).Stream);
            icon.Visible = true;

            MenuItem[] menuItems = new[] { 
                new MenuItem("Configure", TriggerConfigure),
                new MenuItem("Restart", TriggerRestart),
                new MenuItem("Refresh", TriggerRefresh),
                new MenuItem("Exit", TriggerExit)
            };

            ContextMenu menu = new ContextMenu(menuItems);
            icon.ContextMenu = menu;
        }

        public static void Show() { }

        private static void TriggerRestart(object sender, EventArgs e)
        {
            Application.Restart();
        }
        private static void TriggerConfigure(object sender, EventArgs e)
        {
            (new Configuration()).Show();
        }
        private static void TriggerExit(object sender, EventArgs e)
        {
            //Application.Exit();
            Environment.Exit(0);
        }

        public static void TriggerRefresh(object sender, EventArgs e)
        {
            if (AppleKeyboardHID2.Registered)
                AppleKeyboardHID2.Shutdown();
            AppleKeyboardHID2.Start();
        }
    }
}
