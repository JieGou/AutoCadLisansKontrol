

using MaterialDesignDemo.autocad.masterkey.ws;
using MaterialDesignDemo.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;
using System.Management;
using MaterialDesignDemo.Controller;

namespace AutoCadLisansKontrol.Controller
{
    public class LicenseDetection
    {

        public static CheckLicenseModel ExecutePowerShell(CheckLicenseModel chc, string username, string password, int opreationid)
        {
            //Environment.SetEnvironmentVariable("clientname", chc.Ip);
            //Environment.SetEnvironmentVariable("username", username);
            //Environment.SetEnvironmentVariable("password", password);
            //ProcessStartInfo info = new ProcessStartInfo();
            //info.FileName = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\checklicense.bat";
            //var arg = @"-c -f " + chc.Ip + " -u " + username + " -p " + password + " ";
            //info.RedirectStandardOutput = false;
            //info.UseShellExecute = true;
            //info.Verb = "runas";
            //Process p = Process.Start(info);
            //string output = p.StandardOutput.ReadToEnd();
            //p.WaitForExit();
            //Thread.Sleep(5000);

            var scripts = new List<string>();
            var FileName = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\checklicense.bat";
            var command = System.IO.File.ReadAllText(FileName);


            var createfile = "$s=\" " + command + "\"\n" +
            //"$s >> \".\\checklicense.bat\" -Encoding \"UTF8\""+
            "out-file -filepath \".\\checklicense.bat\" -inputobject $s -encoding UTF8";

            scripts.Add("New-Item -Force -ItemType directory -Path C:\\$env:computername");
            // scripts.Add("DEL \\F \\S \\Q \\A \"C:\\%computername%\\*");
            scripts.Add("cd C:\\$env:computername");
            scripts.Add(createfile);
            scripts.Add("cmd.exe /c checklicense.bat");
            scripts.Add("Get-Content C:\\$env:computername\\$env:computername.txt -Raw");
            scripts.Add("Remove-Item C:\\$env:computername\\$env:computername.txt");
            scripts.Add("Remove-Item C:\\$env:computername\\log.txt");
            scripts.Add("Remove-Item C:\\$env:computername\\checklicense.bat");
            var securepassword = ConvertToSecureString(password);
            string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
            PSCredential remoteCredential = new PSCredential(username, securepassword);
            WSManConnectionInfo connectionInfo = new WSManConnectionInfo(false, chc.Name, 5985, "/wsman", shellUri, remoteCredential);

            string output = "";
            try
            {
                using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
                {
                    runspace.Open();
                    foreach (var script in scripts)
                    {

                        Pipeline pipeline = runspace.CreatePipeline();
                        pipeline.Commands.AddScript(script);
                        var results = pipeline.Invoke();
                        if (script.Contains("Get-Content"))
                        {
                            output = results[0].ToString();
                        }
                    }
                    chc.Success = true;

                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
                chc.Fail = true;
            }
            chc.Output = output;
            chc.IsUnlicensed = true;
            chc.CheckDate = DateTime.Now;
            chc.OperationId = opreationid;
            chc.UpdateDate = DateTime.Now;
            return chc;
        }
        private static SecureString ConvertToSecureString(string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            var securePassword = new SecureString();

            foreach (char c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }
        public static CheckLicenseModel ExecutePsexec(CheckLicenseModel chc, string username, string password, int opreationid)
        {
            var timeout = 1000*60;
            var scripts = new List<RemoteCommand>();


            var ciler = @"-c -f  \\cilerturkmen -u adminciler -p ciler471 cmd.exe /c  ""C:\Users\hikmet\Desktop\checklicense.bat""";
            var hikmet = @"-c -f  \\hikmetyarbasi -u YARBASI\adminhikmet -p hikmet67 C:\Users\hikmet.yarbasi\Desktop\checklicense.bat ";
            var readcontent = @"-c -f  \\hikmetyarbasi -u YARBASI\adminhikmet -p hikmet67  C:\Users\hikmet.yarbasi\Desktop\readlicense.bat 2>C:\Users\hikmet.yarbasi\Desktop\output.txt";
            var readcontent2 = @"-c -f  \\cilerturkmen -u adminciler -p ciler471  cmd.exe /c  ""C:\Users\hikmet\Desktop\readlicense.bat""";
            scripts.Add(new RemoteCommand { command= ciler ,timeout=1000*40});
            scripts.Add(new RemoteCommand { command=readcontent2,timeout=1000*10});
            var filename = @"C:\PSTools\psexec.exe";
            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            try
            {
                foreach (var script in scripts)
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = filename;
                        process.StartInfo.Arguments = script.command;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.Verb = "runas";
                        

                        using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                        using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                        {
                            process.OutputDataReceived += (sender, e) =>
                            {
                                if (e.Data != null)
                                {
                                    output.AppendLine(e.Data);
                                }
                            };
                            process.ErrorDataReceived += (sender, e) =>
                            {
                                if (e.Data != null)
                                {
                                    error.AppendLine(e.Data);
                                }
                            };

                            process.Start();

                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();

                            //if (process.WaitForExit(script.timeout) &&
                            //    outputWaitHandle.WaitOne(script.timeout) &&
                            //    errorWaitHandle.WaitOne(script.timeout))
                            //{
                            //    outputcontent += output.ToString();

                            //}
                            //else
                            //{
                            //    errorcontent += error.ToString();
                            //}
                        }
                    }
                }
                chc.Success = true;
            }
            catch (Exception ex)
            {
                output.Append(ex.Message);
                chc.Fail = true;
            }
            chc.Error = error.ToString();
            chc.Output = output.ToString();
            return chc;
        }

        public static CheckLicenseModel ExecuteWMI(CheckLicenseModel chc, string username, string password, int opreationid)
        {
            var sBatFile = @"C:\Users\hikmet\Desktop\checklicense.bat";
            try
            {
                ProcessWMI process = new ProcessWMI();
                process.ExecuteRemoteProcessWMI(chc.Name, username, password, sBatFile, 1000);
            }
            catch (Exception ex)
            {
                chc.Output = ex.Message;
            }
        
            return chc;
        }

        internal class RemoteCommand {
            public string command { get; set; }
            public int timeout { get; set; }
        }
    }
}
