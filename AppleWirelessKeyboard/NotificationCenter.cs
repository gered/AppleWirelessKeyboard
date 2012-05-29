using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppleWirelessKeyboard
{
    public static class NotificationCenter
    {
        static NotificationCenter()
        {

        }
        public static void NotifyMuteOn()
        {
            App.Window.Dispatcher.Invoke(new Action(delegate
            {
                App.Window.ShowOff(new Glyphs.MuteOn());
            }));
        }
        public static void NotifyMuteOff()
        {
            App.Window.Dispatcher.Invoke(new Action(delegate
            {
                App.Window.ShowOff(new Glyphs.MuteOff());
            }));
        }
        public static void NotifyNoVolume()
        {
            App.Window.Dispatcher.Invoke(new Action(delegate
            {
                App.Window.ShowOff(new Glyphs.VolumeOff());
            }));
        }
        public static void NotifyVolumeLevel(int level)//1-16
        {
            App.Window.Dispatcher.Invoke(new Action(delegate
            {
                App.Window.ShowOff(new Glyphs.VolumeOn(), true, level);
            }));
        }
        public static void NotifyTaskManager()
        {
            App.Window.Dispatcher.Invoke((Action)delegate
            {
                App.Window.ShowOff(new Glyphs.TaskManager());
            });
        }
        public static void NotifyPrintScreen()
        {
            App.Window.Dispatcher.Invoke((Action)delegate
            {
                App.Window.ShowOff(new Glyphs.PrintScreen());
            });
        }
        public static void NotifyOn()
        {
            App.Window.Dispatcher.Invoke((Action)delegate
            {
                App.Window.ShowOff(new Glyphs.On());
            });
        }
        public static void NotifyOff()
        {
            App.Window.Dispatcher.Invoke((Action)delegate
            {
                App.Window.ShowOff(new Glyphs.Off());
            });
        }
        public static void NotifyPlayPause()
        {
            App.Window.Dispatcher.Invoke((Action)delegate
            {
                App.Window.ShowOff(new Glyphs.PlayPause());
            });
        }
        public static void NotifyPrevious()
        {
            App.Window.Dispatcher.Invoke((Action)delegate
            {
                App.Window.ShowOff(new Glyphs.Previous());
            });
        }
        public static void NotifyNext()
        {
            App.Window.Dispatcher.Invoke((Action)delegate
            {
                App.Window.ShowOff(new Glyphs.Next());
            });
        }

        internal static void NotifyEject()
        {
            App.Window.Dispatcher.Invoke((Action)delegate
            {
                App.Window.ShowOff(new Glyphs.Eject());
            });
        }
    }
}
