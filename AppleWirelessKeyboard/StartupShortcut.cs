using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace AppleWirelessKeyboard
{
    public static class StartupShortcut
    {
        public static void Register()
        {
            //string file = GetShortcutPath();
            //string shortcut = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.ActivationUri.ToString();
            //StreamWriter sw = new StreamWriter(file);
            //sw.Write(shortcut);
            //sw.Close();
        }
        public static void UnRegister()
        {
            File.Delete(GetShortcutPath());
        }
        public static string GetShortcutPath()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            return Path.Combine(folder, Path.ChangeExtension(Assembly.GetEntryAssembly().GetName().Name, ".appref-ms"));
        }
        public static bool IsRegistered
        {
            get
            {
                return File.Exists(GetShortcutPath());
            }
        }
    }
}
