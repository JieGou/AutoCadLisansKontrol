
using AutoCadLisansKontrol.DAL;
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

            List<string> lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Where(x => x != "" && x.StartsWith("\\")).Select(x => x.Replace("\\", "").TrimEnd()).ToList();


            List<string> multiple = lines.Where(x => x.Contains(" ")).ToList();

            foreach (var item in multiple)
            {
                List<string> child = item.Split(' ').ToList().Where(x => x != "").ToList();
                lines.AddRange(child);
            }
            lines = lines.Where(x => !multiple.Contains(x)).ToList();

            List<Computer> networkmachine = GetNetworkMachine(lines);

            p.WaitForExit();

            return networkmachine.ToList();
        }

        private List<Computer> GetNetworkMachine(List<string> lines)
        {
            List<Computer> networkmachine = new List<Computer>();

            for (int i = 0; i < lines.Count; i++)
            {
                networkmachine.Add(new Computer
                {
                    Name = lines[i],
                    // Type = machineinfo[2],
                    IsRootMachine = false,
                    IsComputer = true
                });
            }


            return networkmachine;

        }
        public string GetMacAddress(string ipAddress)
        {
            string macAddress = string.Empty;
            //ADDED THIS
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            ProcessStartInfo pi = new ProcessStartInfo()
            {
                //CHANGED THIS LINE
                FileName = filePath + "\\nbtstat.exe",
                Arguments = "-A " + ipAddress,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };
            var strOutput = "";
            using (Process p = Process.Start(pi))
            {
                p.WaitForExit();
                strOutput = p.StandardOutput.ReadToEnd();
            }
           
                


                string[] lines = strOutput.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                var line = lines.Where(x => x.Contains(ipAddress)).First();
                if (line != null)
                {
                    var substrings = line.Split(' ').Where(x => x != "").ToList();
                    return substrings[1];
                }
                else
                {
                    return "not found";
                }

            }
        public string GetIpAddressFromName(string name)
        {
            try
            {
                IPAddress[] ips;
                ips = Dns.GetHostAddresses(name);

                if (ips.Length > 0)
                    return ips[0].ToString();

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }
        public IPAddress GetDefaultGateway()
        {

            foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
                if (f.OperationalStatus == OperationalStatus.Up)
                    foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
                    {
                        return d.Address;
                    }
            return null;
        }

        public IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }
        public string GetSubnetMask(byte subnet)
        {
            long mask = (0xffffffffL << (32 - subnet)) & 0xffffffffL;
            mask = IPAddress.HostToNetworkOrder((int)mask);
            return new IPAddress((UInt32)mask).ToString();
        }
        public void ExecuteComputer(Computer comp)
        {

            comp.Ip = GetIpAddressFromName(comp.Name);
            comp.PyshicalAddress = GetMacAddress(comp.Ip);
            comp.IsVisible = PingHost(comp.Ip);
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
