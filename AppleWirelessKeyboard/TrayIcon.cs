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
    public class TrayIcon
    {
		private System.Windows.Application _application = null;
		private NotifyIcon _icon = null;

		public TrayIcon(System.Windows.Application application)
		{
			_application = application;
		}

        public void Show()
		{
			_icon = new NotifyIcon();
			_icon.Text = "AppleWirelessKeyboard";
			_icon.Icon = new Icon(App.GetResourceStream(new Uri("pack://application:,,,/Gnome-Preferences-Desktop-Keyboard-Shortcuts.ico")).Stream);
			_icon.Visible = true;

			MenuItem[] menuItems = new[] { 
                new MenuItem("Configure", TriggerConfigure),
                new MenuItem("Refresh", TriggerRefresh),
                new MenuItem("Exit", TriggerExit)
            };

			ContextMenu menu = new ContextMenu(menuItems);
			_icon.ContextMenu = menu;
		}

		public void Close()
		{
			_icon.Visible = false;
			_icon = null;
		}

		private void TriggerConfigure(object sender, EventArgs e)
        {
            (new Configuration()).Show();
        }
        private void TriggerExit(object sender, EventArgs e)
        {
			_application.Shutdown();
        }

        public void TriggerRefresh(object sender, EventArgs e)
        {
            if (AppleKeyboardHID2.Registered)
                AppleKeyboardHID2.Shutdown();
            AppleKeyboardHID2.Start();
        }
    }
}
