﻿

using MaterialDesignDemo.autocad.masterkey.ws;
using MaterialDesignDemo.Model;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace AutoCadLisansKontrol.Controller
{
    public class LicenseDetection
    {

        public static CheckLicenseModel Execute(CheckLicenseModel chc,string username, string password, int opreationid)
        {
            //    ProcessStartInfo info = new ProcessStartInfo("C:\\PsTools");
            //    info.FileName = @"C:\Users\hikmet\Desktop\PSTools\psexec.exe";
            //    var arg = @"-c -f " + ip + " -u " + username + " -p " + password + " " + System.IO.Directory.GetCurrentDirectory() + @"\BatFile\checklicense.bat";
            //    info.Arguments = arg;
            //    info.RedirectStandardOutput = false;
            //    info.UseShellExecute = true;
            //    info.Verb = "runas";
            //    Process p = Process.Start(info);
            // string output = p.StandardOutput.ReadToEnd();          
            //  p.WaitForExit();
            Thread.Sleep(5000);
            chc.Output = "Konu üzerinden çalışmalarımız devam etmektedir şuan için bir output verememekteyiz.";
            chc.IsUnlicensed = true;
            chc.CheckDate = DateTime.Now;
            chc.OperationId = opreationid;
            chc.UpdateDate = DateTime.Now;
            return chc;
        }

    }
}
