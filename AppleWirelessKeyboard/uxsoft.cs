using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Windows;
using System.Security.Cryptography;
using System.Reflection;
using System.IO;

namespace AppleWirelessKeyboard
{
    public static class uxsoft
    {
        public const string ProgramDownloadUrl = "http://uxsoft.cz/projects/applewirelesskeyboard/AppleWirelessKeyboard.exe";

        public static void VersionCheck()
        {
            const string ProgramVersionUrl = "http://uxsoft.cz/projects/applewirelesskeyboard/version.php";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    string hash = ExecutingHash.GetExecutingFileHash();

                    if (string.IsNullOrEmpty(Properties.Settings.Default.Hash) || (Properties.Settings.Default.Hash != hash))
                    {
                        Properties.Settings.Default.Version = "";
                        Properties.Settings.Default.Hash = hash;
                        Properties.Settings.Default.Save();
                    }

                    WebClient wc = new WebClient();
                    string webVersion = wc.DownloadString(ProgramVersionUrl);
                    if (string.IsNullOrEmpty(webVersion))
                        if (string.IsNullOrEmpty(Properties.Settings.Default.Version))
                        {
                            Properties.Settings.Default.Version = webVersion;
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            if (Properties.Settings.Default.Version != webVersion)
                                if (MessageBox.Show("An update is available, download now?", "Update", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                    Process.Start(ProgramDownloadUrl);
                        }
                }
                catch { }
            });
        }
    }
    internal static class ExecutingHash
    {
        public static string GetExecutingFileHash()
        {
            return MD5(GetSelfBytes());
        }

        public static string MD5(byte[] input)
        {
            return MD5(ASCIIEncoding.ASCII.GetString(input));
        }

        public static string MD5(string input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] originalBytes = ASCIIEncoding.Default.GetBytes(input);
            byte[] encodedBytes = md5.ComputeHash(originalBytes);

            return BitConverter.ToString(encodedBytes).Replace("-", "");
        }

        public static byte[] GetSelfBytes()
        {
            string path = Assembly.GetExecutingAssembly().Location;

            FileStream running = File.OpenRead(path);

            byte[] exeBytes = new byte[running.Length];
            running.Read(exeBytes, 0, exeBytes.Length);

            running.Close();

            return exeBytes;
        }
    }
}
