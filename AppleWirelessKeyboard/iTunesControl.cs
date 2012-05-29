using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using iTunesLib;
using System.Windows;

namespace AppleWirelessKeyboard
{
	// TODO: get this "iTunesLib" (probably by installing iTunes?) and uncomment this code

    public static class iTunesControl
    {
        public static bool iTunesRunning
        {
            get
            {
                //var processes = Process.GetProcesses();
                //return Process.GetProcessesByName("iTunes").Any();

				// might as well just do this for now, since the iTunes functionality below
				// has been commented out
				return false;
            }
        }
        public static void NextSong()
        {
            //iTunesApp app = new iTunesApp();
            //app.NextTrack();
            //app = null;
        }
        public static void PreviousSong()
        {
            //iTunesApp app = new iTunesApp();
            //app.PreviousTrack();
            //app = null;
        }
        public static void PlayPause()
        {
            //iTunesApp app = new iTunesApp();
            //app.PlayPause();
            //app = null;
        }
    }
}
