using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Timers;
using System.Threading;

namespace AppleWirelessKeyboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow Window { get; set; }

		private TrayIcon _trayIcon = null;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
			_trayIcon = new TrayIcon(this);
			_trayIcon.Show();

			// Version check updating disabled because this is a fork and it would obviously 
			// be bad of us to update this forked version with the original...
            //uxsoft.VersionCheck();

            if (!StartupShortcut.IsRegistered)
                StartupShortcut.Register();

            if (Process.GetProcessesByName(Application.ResourceAssembly.GetName().Name).Length > 1)
            {
                var processes = Process.GetProcessesByName(Application.ResourceAssembly.GetName().Name);
                foreach (Process p in processes)
                    if (p != Process.GetCurrentProcess())
                        p.Kill();
            }

            Window = new MainWindow();

            AppleKeyboardHID2.Start();
            AppleKeyboardHID2.KeyDown += new AppleKeyboardHID2.AppleHIDKeyboardEventHandler(AppleKeyboardHID_KeyDown);
            AppleKeyboardHID2.KeyUp += new AppleKeyboardHID2.AppleHIDKeyboardEventHandler(AppleKeyboardHID_KeyUp);

            KeyboardListener.HookedKeys.Add(Key.F2);
            KeyboardListener.HookedKeys.Add(Key.F3);
            KeyboardListener.HookedKeys.Add(Key.F4);
            KeyboardListener.HookedKeys.Add(Key.F7);
            KeyboardListener.HookedKeys.Add(Key.F8);
            KeyboardListener.HookedKeys.Add(Key.F9);
            KeyboardListener.HookedKeys.Add(Key.F10);
            KeyboardListener.HookedKeys.Add(Key.F11);
            KeyboardListener.HookedKeys.Add(Key.F12);
            KeyboardListener.HookedKeys.Add(Key.Back);
            KeyboardListener.HookedKeys.Add(Key.Up);
            KeyboardListener.HookedKeys.Add(Key.Left);
            KeyboardListener.HookedKeys.Add(Key.Right);
            KeyboardListener.HookedKeys.Add(Key.Down);
            KeyboardListener.HookedKeys.Add(Key.Enter);
            KeyboardListener.Register();
            KeyboardListener.KeyDown += new KeyboardListener.KeyHookEventHandler(KeyboardListener_KeyDown);

            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
        }

        void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                Thread.Sleep(16000);
                AppleKeyboardHID2.Shutdown();
                AppleKeyboardHID2.Start();
            }
        }

        void AppleKeyboardHID_KeyUp(AppleKeyboardSpecialKeys key)
        {
            if (key == AppleKeyboardSpecialKeys.Fn)
                KeyboardListener.ModifierFn = false;
        }

        void AppleKeyboardHID_KeyDown(AppleKeyboardSpecialKeys key)
        {
            switch (key)
            {
                case AppleKeyboardSpecialKeys.Fn:
                    KeyboardListener.ModifierFn = true;
                    break;
                case AppleKeyboardSpecialKeys.Eject:
                    KeyboardHandler.HandleEject();
                    break;
            }
        }

        bool KeyboardListener_KeyDown(KeyboardListener.KeyHookEventArgs e)
        {
            return KeyboardHandler.HandleKeyDown(e);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
			_trayIcon.Close();
            KeyboardListener.UnRegister();
            AppleKeyboardHID2.Shutdown();
        }
    }
}
