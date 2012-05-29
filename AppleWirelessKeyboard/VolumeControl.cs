using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using CoreAudioApi;
namespace AppleWirelessKeyboard
{
    public static class VolumeControl
    {
        static VolumeControl()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDevice device = enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            Controller = device.AudioEndpointVolume;
        }
        private static AudioEndpointVolume Controller = null;
        public static void ToggleMute()
        {
            Controller.Mute = !Controller.Mute;
            if (Controller.Mute)
                NotificationCenter.NotifyMuteOff();
            else
                NotificationCenter.NotifyMuteOn();

        }
        public static void VolumeUp()
        {
            if (Controller.MasterVolumeLevelScalar > 0.9375f)
                Controller.MasterVolumeLevelScalar = 1.0f;
            else Controller.MasterVolumeLevelScalar += 0.0625f;
            NotificationCenter.NotifyVolumeLevel((int)(Controller.MasterVolumeLevelScalar / 0.0625));
        }
        public static void VolumeDown()
        {
            if (Controller.MasterVolumeLevelScalar < 0.0625)
            {
                Controller.MasterVolumeLevelScalar = 0;
                NotificationCenter.NotifyNoVolume();
            }
            else
            {
                Controller.MasterVolumeLevelScalar -= 0.0625f;
                NotificationCenter.NotifyVolumeLevel((int)(Controller.MasterVolumeLevelScalar / 0.0625));
            }
        }
    }
}
