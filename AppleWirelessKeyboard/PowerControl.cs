using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace AppleWirelessKeyboard
{
    public class PowerControl
    {
        public static void Hibernate()
        {
            Task.Factory.StartNew(() =>
            {
                if (PowerStatusBox.PowerAction("Hibernate", 10))
                    SetSuspendState(true, true, true);
            });
        }

        public static void Shutdown()
        {
            Task.Factory.StartNew(() =>
            {
                if (PowerStatusBox.PowerAction("Shut Down", 10))
                {
                    ProcessStartInfo si = new ProcessStartInfo("shutdown", "/s /t 0");
                    si.CreateNoWindow = true;
                    si.WindowStyle = ProcessWindowStyle.Hidden;
                    Process.Start(si);
                }
            });
        }

        [DllImport("powrprof.dll", SetLastError = true)]
        static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
    }
}
