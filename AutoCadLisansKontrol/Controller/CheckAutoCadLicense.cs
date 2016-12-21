
using AutoCadLisansKontrol.DAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace AutoCadLisansKontrol.Controller
{
    public class CheckAutoCadLicense
    {

        public void ManageCommandPrompt()
        {
            ProcessStartInfo info = new ProcessStartInfo("C:\\PsTools");
            info.FileName = @"C:\Users\hikmet\Desktop\PSTools\psexec.exe";
            info.Arguments = @"-c -f @c:\users\hikmet\desktop\client.txt -u admin -p select c:\users\hikmet\desktop\testkontroller.bat";
            info.RedirectStandardOutput = false;
            info.UseShellExecute = true;
            info.Verb = "runas";
            Process p = Process.Start(info);
            p.WaitForExit();
        }
        public List<Computer> GetComputerInfoOnNetwork()
        {
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\detectcomputer.bat";
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();

            string[] lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Where(x => x != "").ToArray() ;

            Computer root = GetRootMachine(lines);
            List<Computer> networkmachine = GetNetworkMachine(lines);
            networkmachine.Add(root);

            p.WaitForExit();

            return networkmachine.Take(2).ToList();
        }
        private List<Computer> GetNetworkMachine(string[] lines)
        {
            var startpoint = Array.FindIndex(lines, x => x.Contains("Internet Address")) + 1;

            List<Computer> networkmachine = new List<Computer>();
            for (int i = startpoint; i < lines.Length; i++)
            {
                var machineinfo = lines[i].Split(' ').Where(x => x != "").ToArray();
                networkmachine.Add(new Computer
                {
                    Ip = machineinfo[0],
                    PyshicalAddress = machineinfo[1],
                    Type = machineinfo[2],
                    IsRootMachine = false
                });
            }

            return networkmachine;

        }
        private Computer GetRootMachine(string[] lines)
        {
            var rootmachineline = lines.Where(x => x.Contains("Interface:")).FirstOrDefault();

            return new Computer
            {
                Ip = rootmachineline,
                IsRootMachine = true,
                Name = GetMachineNameFromIPAddress(rootmachineline),
                PyshicalAddress = "",
            };

        }
        public void ExecuteComputer(Computer comp)
        {
            comp.Ip = "xx";
            comp.Name = GetMachineNameFromIPAddress(comp.Ip);
            comp.IsComputer = true;
            comp.IsRootMachine = false;
            comp.PyshicalAddress = "xx";
            comp.Visibility = PingHost(comp.Ip);
        }
        private static string GetMachineNameFromIPAddress(string ipAdress)
        {
            string machineName = string.Empty;
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAdress);

                machineName = hostEntry.HostName;
            }
            catch (Exception ex)
            {
                // Machine not found...
            }
            return machineName;
        }
        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            return pingable;
        }
    }
}
