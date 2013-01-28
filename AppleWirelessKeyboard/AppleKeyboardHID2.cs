using System;
using System.IO;
using AppleWirelessKeyboard;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace AppleWirelessKeyboard
{
    public class AppleKeyboardHID2
    {
        // Fields
        static Stream _stream;
        static bool CurrentPowerButtonIsDown;
        public const int VIDApple = 0x5ac;

        // Events
        public static event EventHandler Disconnected;
        public delegate void AppleHIDKeyboardEventHandler(AppleKeyboardSpecialKeys key);
        public static event AppleHIDKeyboardEventHandler KeyUp;
        public static event AppleHIDKeyboardEventHandler KeyDown;

        //Properties
        public static bool FnDown { get; set; }
        public static bool EjectDown { get; set; }
        public static bool PowerButtonDown { get; set; }
        public static bool Registered
        {
            get { return _stream != null; }
        }

        public static void Shutdown()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }
        }

        private static void SpecialKeyStateChanged(IAsyncResult ar)
        {
            if ((_stream != null) && ar.IsCompleted)
            {
                try
                {
                    _stream.EndRead(ar);
                }
                catch (OperationCanceledException)
                {
                }
                catch (IOException)
                {
                    if (Disconnected != null)
                    {
                        Disconnected(null, EventArgs.Empty);
                    }
                    return;
                }
                byte[] asyncState = ar.AsyncState as byte[];
                byte[] buffer2 = asyncState;
                for (int i = 0; i < buffer2.Length; i++)
                {
                    byte num1 = buffer2[i];
                }

				// TODO: would be nice if we could replace this property with some auto-detection to figure out
				//       if the user is using a wired or wireless keyboard based on the device information...
				//       not sure how to do this currently because I don't have a wireless keyboard to test with.
				if (AppleWirelessKeyboard.Properties.Settings.Default.WiredKeyboard)
				{
					// This is probably a big ol' ugly hack... but it seems to work well so far.
					//
					// With the wired Apple Keyboard I have (http://store.apple.com/us/product/MB110LL/B), this whole method
					// _only_ does anything when the Eject key is pressed/released. No other keys or combination of keys
					// that I tried causes the SpecialKeyStateChanged event to fire. This is using the default driver that
					// Windows 7 installs when I first plugged in my keyboard. (FWIW, the Apple driver from Lion's BootCamp
					// files didn't change anything).
					//
					// With the wired keyboard, the bytes in asyncState will be different then the original code here is
					// expecting (which is for the wireless keyboard).
					//
					// Basically, when using the wired keyboard:
					// * when the Eject key is pressed:   (asyncState[0] == 0x0 && asyncState[1] == 0x8) == true
					// * when the Eject key is released:  (asyncState[0] == 0x0 && asyncState[1] == 0x0) == true
					//
					// This _appears_ to be safe for use with the wired keyboard because, as noted above, no other keys
					// or combinations of keys cause this method to be fired in the first place.


					Debug.WriteLine(String.Format("{0}, {1}", asyncState[0], asyncState[1]));
					if (asyncState[0] == 0x0 && asyncState[1] == 0x8)
					{
						if (AppleWirelessKeyboard.Properties.Settings.Default.WiredHoldEjectForFn)
						{
							FnDown = true;
							KeyDown(AppleKeyboardSpecialKeys.Fn);
						}
						else
						{
							EjectDown = true;
							KeyDown(AppleKeyboardSpecialKeys.Eject);
						}
					}
					else if (asyncState[0] == 0x0 && asyncState[1] == 0x0)
					{
						if (AppleWirelessKeyboard.Properties.Settings.Default.WiredHoldEjectForFn)
						{
							FnDown = false;
							KeyUp(AppleKeyboardSpecialKeys.Fn);
						}
						else
						{
							EjectDown = false;
							KeyUp(AppleKeyboardSpecialKeys.Eject);
						}
					}
				}
				else
				{
					// Original code to handle the wireless keyboard left completely intact.

					if (asyncState[0] == 0x11)
					{
						switch (asyncState[1])
						{
							case 24:
								{
									EjectDown = true;
									FnDown = true;
									if (KeyDown != null)
										KeyDown(AppleKeyboardSpecialKeys.Eject);
								}
								break;
							case 16:
								{
									if (EjectDown)
									{
										EjectDown = false;
										if (KeyUp != null)
											KeyUp(AppleKeyboardSpecialKeys.Eject);
									}
									FnDown = true;
									if (KeyDown != null)
										KeyDown(AppleKeyboardSpecialKeys.Fn);
								}
								break;
							case 8:
								{
									if (FnDown)
									{
										FnDown = false;
										if (KeyUp != null)
											KeyUp(AppleKeyboardSpecialKeys.Fn);
									}
									EjectDown = true;
									if (KeyDown != null)
										KeyDown(AppleKeyboardSpecialKeys.Eject);
								}
								break;
							case 0:
								{
									if (EjectDown)
									{
										EjectDown = false;
										if (KeyUp != null)
											KeyUp(AppleKeyboardSpecialKeys.Eject);
									}
									if (FnDown)
									{
										FnDown = false;
										if (KeyUp != null)
											KeyUp(AppleKeyboardSpecialKeys.Fn);
									}
								}
								break;
						}
					}
					else if (asyncState[0] == 0x13)
					{
						CurrentPowerButtonIsDown = asyncState[1] == 1;
					}
				}
                _stream.BeginRead(asyncState, 0, asyncState.Length, new AsyncCallback(SpecialKeyStateChanged), asyncState);
            }
        }

        public static bool Start()
        {
            Guid guid;
            HIDImports.SP_DEVICE_INTERFACE_DATA sp_device_interface_data = new HIDImports.SP_DEVICE_INTERFACE_DATA() { cbSize = Marshal.SizeOf(typeof(HIDImports.SP_DEVICE_INTERFACE_DATA)) };

            if (_stream != null)
            {
                throw new InvalidOperationException("No Stream!");
            }

            HIDImports.HidD_GetHidGuid(out guid);
            IntPtr hDevInfo = HIDImports.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, 0x10);

            int num = 0;
            while (HIDImports.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guid, num++, ref sp_device_interface_data))
            {
                uint num2;
                HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = new HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA { cbSize = (IntPtr.Size == 8) ? (uint)8 : (uint)5 };

                HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref sp_device_interface_data, IntPtr.Zero, 0, out num2, IntPtr.Zero);
                if (HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref sp_device_interface_data, ref deviceInterfaceDetailData, num2, out num2, IntPtr.Zero))
                {
                    HIDImports.HIDD_ATTRIBUTES hidd_attributes = new HIDImports.HIDD_ATTRIBUTES() { Size = Marshal.SizeOf(typeof(HIDImports.HIDD_ATTRIBUTES)) };

                    SafeFileHandle handle = HIDImports.CreateFile(deviceInterfaceDetailData.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

                    if (HIDImports.HidD_GetAttributes(handle.DangerousGetHandle(), ref hidd_attributes))
                    {
                        if (IsAppleWirelessKeyboard(hidd_attributes.VendorID, hidd_attributes.ProductID))
                        {
                            _stream = new FileStream(handle, FileAccess.ReadWrite, 0x16, true);
                            //break;
                        }
                        else
                        {
                            handle.Close();
                        }
                    }
                }
            }

            HIDImports.SetupDiDestroyDeviceInfoList(hDevInfo);

            if (_stream != null)
            {
                byte[] buffer = new byte[0x16];
                _stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(SpecialKeyStateChanged), buffer);
                return true;
            }
            return false;
        }

        private static bool IsAppleWirelessKeyboard(int vid, int pid)
        {
            if (vid == VIDApple)
            {
                Guid HIDGUID;
                HIDImports.HidD_GetHidGuid(out HIDGUID);

                IntPtr DeviceInfo = HIDImports.SetupDiGetClassDevs(ref HIDGUID, null, IntPtr.Zero, 16);

                uint MemberIndex = 0;

                HIDImports.SP_DEVINFO_DATA DID = new HIDImports.SP_DEVINFO_DATA() { cbSize = (uint)Marshal.SizeOf(typeof(HIDImports.SP_DEVINFO_DATA)) };

                while (HIDImports.SetupDiEnumDeviceInfo(DeviceInfo, MemberIndex++, ref DID))
                {
                    uint RequiredSize = 0;
                    uint PropertyDataType = 0;
                    IntPtr buffer = Marshal.AllocHGlobal(512);
                    string CLASS = "";


                    if (HIDImports.SetupDiGetDeviceRegistryProperty(DeviceInfo, ref DID, (uint)HIDImports.SPDRP.SPDRP_CLASS, out PropertyDataType, buffer, 512, out RequiredSize))
                        CLASS = Marshal.PtrToStringAuto(buffer);

                    if (CLASS.Equals("Keyboard", StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }

                HIDImports.SetupDiDestroyDeviceInfoList(DeviceInfo);

                return true;
            }
            else return false;
        }
    }
    public enum AppleKeyboardSpecialKeys : byte
    {
        //Fn + Eject = 24
        Fn = 16,
        Eject = 8,
        PowerButton = 3
        //None = 0
    }
}