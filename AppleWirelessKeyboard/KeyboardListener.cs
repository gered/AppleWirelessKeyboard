using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;

namespace AppleWirelessKeyboard
{
    /// <summary>
    /// A class that manages a global low level keyboard hook
    /// </summary>
    public static class KeyboardListener
    {
        static KeyboardListener()
        {
            HookedKeys = new List<Key>();
            Hook = IntPtr.Zero;
        }
        #region PInvoke Structures
        public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);

        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;
        #endregion

        #region Public declarations

        public static List<Key> HookedKeys { get; set; }
        internal static IntPtr Hook { get; set; }

        public static event KeyHookEventHandler KeyDown;
        public static event KeyHookEventHandler KeyUp;

        public static bool ModifierLeftAlt { get; set; }
        public static bool ModifierRightAlt { get; set; }
        public static bool ModifierLeftCtrl { get; set; }
        public static bool ModifierRightCtrl { get; set; }
        public static bool ModifierLeftShift { get; set; }
        public static bool ModifierRightShift { get; set; }
        public static bool ModifierLeftWin { get; set; }
        public static bool ModifierRightWin { get; set; }
        public static bool ModifierCapsLock { get; set; }
        public static bool ModifierFn { get; set; }

        public class KeyHookEventArgs
        {
            public KeyHookEventArgs()
            {
            }
            public Key Key { get; set; }
            public bool ModifierLeftAlt { get; set; }
            public bool ModifierRightAlt { get; set; }
            public bool ModifierLeftCtrl { get; set; }
            public bool ModifierRightCtrl { get; set; }
            public bool ModifierLeftShift { get; set; }
            public bool ModifierRightShift { get; set; }
            public bool ModifierLeftWin { get; set; }
            public bool ModifierRightWin { get; set; }
            public bool ModifierCapsLock { get; set; }
            public bool ModifierFn { get; set; }

            public bool ModifierAnyAlt { get { return ModifierLeftAlt || ModifierRightAlt; } set { ModifierLeftAlt = value; } }
            public bool ModifierAnyCtrl { get { return ModifierLeftCtrl || ModifierRightCtrl; } set { ModifierLeftCtrl = value; } }
            public bool ModifierAnyShift { get { return ModifierLeftShift || ModifierRightShift; } set { ModifierLeftShift = value; } }
            public bool ModifierAnyWin { get { return ModifierLeftWin || ModifierRightWin; } set { ModifierLeftWin = value; } }

            public bool ModifierAnyNative { get { return (((ModifierAnyAlt) || (ModifierAnyWin)) || ((ModifierAnyCtrl) || (ModifierAnyShift))); } }
            public bool ModifierAny { get { return ((ModifierAny) || (ModifierFn)); } }
        }

        public delegate bool KeyHookEventHandler(KeyHookEventArgs e);
        #endregion

        #region Public Methods

        internal static void SetModifiers(Key key, bool IsPressed, int VKey)
        {
            switch (key)
            {
                case Key.LeftAlt:
                    ModifierLeftAlt = IsPressed;
                    break;
                case Key.RightAlt:
                    ModifierRightAlt = IsPressed;
                    break;
                case Key.RightCtrl:
                    ModifierRightCtrl = IsPressed;
                    break;
                case Key.LeftCtrl:
                    ModifierLeftCtrl = IsPressed;
                    break;
                case Key.LeftShift:
                    ModifierLeftShift = IsPressed;
                    break;
                case Key.RightShift:
                    ModifierRightShift = IsPressed;
                    break;
                case Key.CapsLock:
                    ModifierCapsLock = IsPressed;
                    break;
                case Key.LWin:
                    ModifierLeftWin = IsPressed;
                    break;
                case Key.RWin:
                    ModifierRightWin = IsPressed;
                    break;
            }
            if (!AppleKeyboardHID2.Registered)
                AppleKeyboardHID2.Start();
        }
        internal static KeyHookEventArgs CreateEventArgs(Key key)
        {
            return new KeyHookEventArgs()
            {
                Key = key,
                ModifierCapsLock = ModifierCapsLock,
                ModifierLeftAlt = ModifierLeftAlt,
                ModifierRightAlt = ModifierRightAlt,
                ModifierLeftCtrl = ModifierLeftCtrl,
                ModifierRightCtrl = ModifierRightCtrl,
                ModifierLeftShift = ModifierLeftShift,
                ModifierRightShift = ModifierRightShift,
                ModifierLeftWin = ModifierLeftWin,
                ModifierRightWin = ModifierRightWin,
                ModifierFn = ModifierFn
            };
        }

        public static keyboardHookProc HookProcessor { get; set; }

        public static void Register()
        {
            HookProcessor = Hook_Callback;
            IntPtr hInstance = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
            Hook = SetWindowsHookEx(WH_KEYBOARD_LL, HookProcessor, hInstance, 0);
        }

        public static void UnRegister()
        {
            UnhookWindowsHookEx(Hook);
        }

        public static bool TriggerKeyDown(KeyHookEventArgs args)
        {
            if (KeyDown != null)
                return KeyDown(args);
            else return false;
        }
        public static bool TriggerKeyUp(KeyHookEventArgs args)
        {
            if (KeyUp != null)
                return KeyUp(args);
            else return false;
        }
        public static int Hook_Callback(int code, int wParam, ref keyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                Key key = (Key)System.Windows.Input.KeyInterop.KeyFromVirtualKey(lParam.vkCode);
                bool IsPressed = (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN);

                SetModifiers(key, IsPressed, lParam.vkCode);
                if (HookedKeys.Contains(key))
                {
                    KeyHookEventArgs kea = CreateEventArgs(key);

                    bool Handled = false;
                    if (IsPressed)
                        Handled = TriggerKeyDown(kea);
                    else
                        Handled = TriggerKeyUp(kea);

                    if (Handled)
                        return 1;
                }
            }
            return CallNextHookEx(Hook, code, wParam, ref lParam);
        }
        #endregion

        #region DLL imports
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}
