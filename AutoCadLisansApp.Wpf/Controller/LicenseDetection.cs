

using MaterialDesignDemo.autocad.masterkey.ws;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace AutoCadLisansKontrol.Controller
{
    public class LicenseDetection
    {

        public static CheckLicense Execute(string ip, string username, string password)
        {
            ProcessStartInfo info = new ProcessStartInfo("C:\\PsTools");
            info.FileName = @"C:\Users\hikmet\Desktop\PSTools\psexec.exe";
            info.Arguments = @"-c -f " + ip + " -u " + username + " -p " + password + " " + System.IO.Directory.GetCurrentDirectory() + @"\BatFile\checklicense.bat";
            info.RedirectStandardOutput = false;
            info.UseShellExecute = true;
            info.Verb = "runas";
            Process p = Process.Start(info);


            string output = p.StandardOutput.ReadToEnd();

            p.WaitForExit();

            var item = new CheckLicense();

            return item;
        }

    }
}
