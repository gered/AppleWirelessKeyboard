using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace AppleWirelessKeyboard
{
    public static class KeyboardHandler
    {
        static KeyboardHandler()
        {
            PowerButtonPressCount = 0;
        }
        public static bool FMode
        {
            get
            {
				return AppleWirelessKeyboard.Properties.Settings.Default.FMode;
            }
            set
            {
                if (AppleWirelessKeyboard.Properties.Settings.Default.FMode != value)
                {
                    AppleWirelessKeyboard.Properties.Settings.Default.FMode = value;
                    if (value)
                        NotificationCenter.NotifyOn();
                    else NotificationCenter.NotifyOff();

					AppleWirelessKeyboard.Properties.Settings.Default.Save();
                }
            }
        }

        public static void HandleEject()
        {
            if (!AppleKeyboardHID2.FnDown)
                FMode = !FMode;
            else IoControl.EjectAllMedia();
        }

        public static bool HandleKeyDown(KeyboardListener.KeyHookEventArgs e)
        {
			bool F;
			if (AppleWirelessKeyboard.Properties.Settings.Default.WiredKeyboard && 
				AppleWirelessKeyboard.Properties.Settings.Default.WiredHoldEjectForFn)
				F = e.ModifierFn && !e.ModifierAnyNative;
			else
				F = (FMode || e.ModifierFn) && !e.ModifierAnyNative;

            switch (e.Key)
            {
                case Key.F3:
                    if (F)
                    {
                        KeyboardControl.SendPrintScreen();
                        return true;
                    }
                    break;
                case Key.F4:
                    if (F)
                    {
                        KeyboardControl.OpenTaskManager();
                        return true;
                    }
                    break;
                case Key.F2:
                    if (F)
                    {
                        KeyboardControl.SendF3();
                        return true;
                    }
                    break;
                case Key.F7:
                    if (F)
                    {
                        KeyboardControl.SendPreviousTrack();
                        return true;
                    }
                    break;
                case Key.F8:
                    if (F)
                    {
                        KeyboardControl.SendPlayPause();
                        return true;
                    }
                    break;
                case Key.F9:
                    if (F)
                    {
                        KeyboardControl.SendNextTrack();
                        return true;
                    }
                    break;
                case Key.F10:
                    if (F)
                    {
                        VolumeControl.ToggleMute();
                        return true;
                    }
                    break;
                case Key.F11:
                    if (F)
                    {
                        VolumeControl.VolumeDown();
                        return true;
                    }
                    break;
                case Key.F12:
                    if (F)
                    {
                        VolumeControl.VolumeUp();
                        return true;
                    }
                    break;
                case Key.Back:
                    if (e.ModifierFn)
                    {
                        KeyboardControl.SendDelete();
                        return true;
                    }
                    break;
                case Key.Up:
                    if (e.ModifierFn)
                    {
                        KeyboardControl.SendPageUp();
                        return true;
                    }
                    break;
                case Key.Down:
                    if (e.ModifierFn)
                    {
                        KeyboardControl.SendPageDown();
                        return true;
                    }
                    break;
                case Key.Left:
                    if(e.ModifierFn)
                    {
                        KeyboardControl.SendHome();
                        return true;
                    }
                    break;
                case Key.Right:
                    if (e.ModifierFn)
                    {
                        KeyboardControl.SendEnd();
                        return true;
                    } 
                    break;
                case Key.Enter:
                    if (e.ModifierFn)
                    {
                        KeyboardControl.SendInsert();
                        return true;
                    }
                    break;
            }

            if (e.ModifierAnyAlt && (FMode || e.ModifierFn))
                if (e.Key == Key.F3)
                {
                    KeyboardControl.SendPrintScreen();
                    return true;
                }

            return false;
        }
        public static int PowerButtonPressCount { get; set; }
        
    }
}
