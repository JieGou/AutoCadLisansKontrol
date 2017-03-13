using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MaterialDesignDemo.Controller
{
    public class ProcessWMI
    {
        public uint ProcessId;
        public int ExitCode;
        public bool EventArrived;
        public ManualResetEvent mre = new ManualResetEvent(false);
        public void ProcessStoptEventArrived(object sender, EventArrivedEventArgs e)
        {
            if ((uint)e.NewEvent.Properties["ProcessId"].Value == ProcessId)
            {
                Console.WriteLine("Process: {0}, Stopped with Code: {1}", (int)(uint)e.NewEvent.Properties["ProcessId"].Value, (int)(uint)e.NewEvent.Properties["ExitStatus"].Value);
                ExitCode = (int)(uint)e.NewEvent.Properties["ExitStatus"].Value;
                EventArrived = true;
                mre.Set();
            }
        }
        public ProcessWMI()
        {
            this.ProcessId = 0;
            ExitCode = -1;
            EventArrived = false;
        }
        public void ExecuteRemoteProcessWMI(string remoteComputerName, string username, string password, string arguments, int WaitTimePerCommand)
        {

            try
            {
                ConnectionOptions connOptions = new ConnectionOptions();
                connOptions.Impersonation = ImpersonationLevel.Impersonate;
                connOptions.EnablePrivileges = true;
                connOptions.Username = username;
                connOptions.Password = password;

                ManagementScope manScope = new ManagementScope(String.Format(@"\\{0}\ROOT\CIMV2", remoteComputerName), connOptions);

                try
                {
                    manScope.Connect();
                }
                catch (Exception e)
                {
                    throw new Exception("Management Connect to remote machine " + remoteComputerName + " as user " + username + " failed with the following error " + e.Message);
                }
                ObjectGetOptions objectGetOptions = new ObjectGetOptions();
                ManagementPath managementPath = new ManagementPath("Win32_Process");
                using (ManagementClass processClass = new ManagementClass(manScope, managementPath, objectGetOptions))
                {
                    using (ManagementBaseObject inParams = processClass.GetMethodParameters("Create"))
                    {
                        inParams["CommandLine"] = arguments;
                        using (ManagementBaseObject outParams = processClass.InvokeMethod("Create", inParams, null))
                        {

                            if ((uint)outParams["returnValue"] != 0)
                            {
                                throw new Exception("Error while starting process " + arguments + " creation returned an exit code of " + outParams["returnValue"] + ". It was launched as " + username + " on " + remoteComputerName);
                            }
                            this.ProcessId = (uint)outParams["processId"];
                        }
                    }
                }

                SelectQuery CheckProcess = new SelectQuery("Select * from Win32_Process Where ProcessId = " + ProcessId);
                using (ManagementObjectSearcher ProcessSearcher = new ManagementObjectSearcher(manScope, CheckProcess))
                {
                    using (ManagementObjectCollection MoC = ProcessSearcher.Get())
                    {
                        if (MoC.Count == 0)
                        {
                            throw new Exception("ERROR AS WARNING: Process " + arguments + " terminated before it could be tracked on " + remoteComputerName);
                        }
                    }
                }

                WqlEventQuery q = new WqlEventQuery("Win32_ProcessStopTrace");
                using (ManagementEventWatcher w = new ManagementEventWatcher(manScope, q))
                {
                    w.EventArrived += new EventArrivedEventHandler(this.ProcessStoptEventArrived);
                    w.Start();
                    if (!mre.WaitOne(WaitTimePerCommand, false))
                    {
                        w.Stop();
                        this.EventArrived = false;
                    }
                    else
                        w.Stop();
                }
                if (!this.EventArrived)
                {
                    SelectQuery sq = new SelectQuery("Select * from Win32_Process Where ProcessId = " + ProcessId);
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(manScope, sq))
                    {
                        foreach (ManagementObject queryObj in searcher.Get())
                        {
                            queryObj.InvokeMethod("Terminate", null);
                            queryObj.Dispose();
                            throw new Exception("Process " + arguments + " timed out and was killed on " + remoteComputerName);
                        }
                    }
                }
                else
                {
                    if (this.ExitCode != 0)
                        throw new Exception("Process " + arguments + "exited with exit code " + this.ExitCode + " on " + remoteComputerName + " run as " + username);
                    else
                        Console.WriteLine("process exited with Exit code 0");
                }

            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Execute process failed Machinename {0}, ProcessName {1}, RunAs {2}, Error is {3}, Stack trace {4}", remoteComputerName, arguments, username, e.Message, e.StackTrace), e);
            }
        }

        public void GetProductWithWMI(string[] software, string remoteComputerName, string username, string password)
        {
            List<Win32_Product> programs = new List<Win32_Product>();

            ConnectionOptions connOptions = new ConnectionOptions();
            connOptions.Impersonation = ImpersonationLevel.Impersonate;
            connOptions.EnablePrivileges = true;
            connOptions.Username = username;
            connOptions.Password = password;

            ManagementScope scope = new ManagementScope(String.Format(@"\\{0}\ROOT\CIMV2", remoteComputerName), connOptions);

            try
            {
                scope.Connect();
            }
            catch (Exception e)
            {
                throw new Exception("Management Connect to remote machine " + remoteComputerName + " as user " + username + " failed with the following error " + e.Message);
            }
            SelectQuery CheckProcess = new SelectQuery("SELECT * FROM Win32_Product");
            using (ManagementObjectSearcher ProcessSearcher = new ManagementObjectSearcher(scope, CheckProcess))
            {
                foreach (ManagementObject mo in ProcessSearcher.Get())
                {
                    Win32_Product product = new Win32_Product();

                    product.Name = mo["Name"].ToString();

                    var param = software.Where(x => product.Name.Contains(x)).FirstOrDefault();
                    if (param == null) continue;

                    product.AssignmentType = Convert.ToUInt16(mo["AssignmentType"]);
                    product.Caption = mo["Caption"].ToString();
                    product.Description = mo["Description"].ToString();
                    product.HelpLink = mo["HelpLink"].ToString();
                    product.HelpTelephone = mo["HelpTelephone"].ToString();
                    product.IdentifyingNumber = mo["IdentifyingNumber"].ToString();
                    product.InstallDate = mo["InstallDate"].ToString();
                    product.InstallDate2 = Convert.ToDateTime(mo["InstallDate"].ToString());
                    product.InstallLocation = mo["InstallLocation"].ToString();
                    product.InstallSource = mo["InstallSource"].ToString();
                    product.InstallState = Convert.ToInt16(mo["InstallState"].ToString());
                    product.Language = mo["Language"].ToString();
                    product.LocalPackage = mo["LocalPackage"].ToString();
                    product.PackageCache = mo["PackageCache"].ToString();
                    product.PackageCode = mo["PackageCode"].ToString();
                    product.PackageName = mo["PackageName"].ToString();
                    product.ProductID = mo["ProductID"].ToString();
                    product.RegCompany = mo["RegCompany"].ToString();
                    product.RegOwner = mo["RegOwner"].ToString();
                    product.SKUNumber = mo["SKUNumber"].ToString();
                    product.Transforms = mo["Transforms"].ToString();
                    product.URLInfoAbout = mo["URLInfoAbout"].ToString();
                    product.URLUpdateInfo = mo["URLUpdateInfo"].ToString();
                    product.Vendor = mo["Vendor"].ToString();
                    product.Version = mo["Version"].ToString();




                }
            }
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            foreach (ManagementObject mo in mos.Get())
            {
                Console.WriteLine(mo["Name"]);
            }
        }

        public List<Software> ReadRegisteryusingWMI(string[] software, string remoteComputerName, string username, string password)
        {

            var list32 = ReadRegistryusingWMICore(software, @"Software\Microsoft\Windows\CurrentVersion\Uninstall", "NBHIKMETY01", "AYGAZNET\\hikmet.yarbasi", "Computer11.");
            var list64 = ReadRegistryusingWMICore(software, @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall", "NBHIKMETY01", "AYGAZNET\\hikmet.yarbasi", "Computer11.");

            var uniqueList = list32.Concat(list64)
                                   .Where(x => x.DisplayName != null)
                                   .GroupBy(item => item.DisplayName)
                                   .Select(group => group.First())
                                   .ToList();

            return uniqueList;
        }
        private List<Software> ReadRegistryusingWMICore(string[] software, string softwareRegLoc, string remoteComputerName, string username, string password)
        {
            List<Software> programs = new List<Software>();

            ConnectionOptions connOptions = new ConnectionOptions();
            connOptions.Impersonation = ImpersonationLevel.Impersonate;
            connOptions.EnablePrivileges = true;
            connOptions.Username = username;
            connOptions.Password = password;

            ManagementScope scope = new ManagementScope(String.Format(@"\\{0}\ROOT\CIMV2", remoteComputerName), connOptions);

            try
            {
                scope.Connect();
            }
            catch (Exception e)
            {
                throw new Exception("Management Connect to remote machine " + remoteComputerName + " as user " + username + " failed with the following error " + e.Message);
            }

            ManagementClass registry = new ManagementClass(scope, new ManagementPath("StdRegProv"), null);
            ManagementBaseObject inParams = registry.GetMethodParameters("EnumKey");
            inParams["hDefKey"] = 0x80000002;//HKEY_LOCAL_MACHINE
            inParams["sSubKeyName"] = softwareRegLoc;

            // Read Registry Key Names 
            ManagementBaseObject outParams = registry.InvokeMethod("EnumKey", inParams, null);
            string[] programGuids = outParams["sNames"] as string[];


            foreach (string subKeyName in programGuids)
            {
                var item = new Software();
                inParams = registry.GetMethodParameters("GetStringValue");
                inParams["sSubKeyName"] = softwareRegLoc + @"\" + subKeyName;

                inParams["sValueName"] = "DisplayName";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.DisplayName = outParams.Properties["sValue"].Value.ToString();

                    var param = software.Where(x => item.DisplayName.Contains(x)).FirstOrDefault();
                    if (param == null) continue;
                }
                else
                {
                    continue;
                }


                inParams["sValueName"] = "InstallDate";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.InstallDate = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "Contact";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.Contact = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "DisplayIcon";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.DisplayIcon = outParams.Properties["sValue"].Value.ToString();
                }



                inParams["sValueName"] = "DisplayVersion";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.DisplayVersion = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "EstimatedSize";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.EstimatedSize = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "HelpLink";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.HelpLink = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "HelpTelephone";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.HelpTelephone = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "InstallLocation";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.InstallLocation = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "InstallSource ";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.InstallSource = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "Language";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.Language = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "ModifyPath_Hidden";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.ModifyPath_Hidden = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "NoModify";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.NoModify = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "NoRemove";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.NoRemove = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "NoRepair";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.NoRepair = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "Publisher";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.Publisher = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "Readme";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.Readme = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "UninstallString";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.UninstallString = outParams.Properties["sValue"].Value.ToString();
                }
                programs.Add(item);
            }

            return programs;
        }
    }
    public class Software
    {
        public string Comments { get; set; }
        public string Contact { get; set; }
        public string DisplayIcon { get; set; }
        public string DisplayName { get; set; }
        public string DisplayVersion { get; set; }
        public string EstimatedSize { get; set; }
        public string HelpLink { get; set; }
        public string HelpTelephone { get; set; }
        public string InstallDate { get; set; }
        public string InstallLocation { get; set; }
        public string InstallSource { get; set; }
        public string Language { get; set; }
        public string ModifyPath_Hidden { get; set; }
        public string NoModify { get; set; }
        public string NoRemove { get; set; }
        public string NoRepair { get; set; }
        public string Publisher { get; set; }
        public string Readme { get; set; }
        public string UninstallString { get; set; }

    }
    public class Win32_Product
    {
        public UInt16 AssignmentType;
        public string Caption;
        public string Description;
        public string IdentifyingNumber;
        public string InstallDate;
        public DateTime InstallDate2;
        public string InstallLocation;
        public Int16 InstallState;
        public string HelpLink;
        public string HelpTelephone;
        public string InstallSource;
        public string Language;
        public string LocalPackage;
        public string Name;
        public string PackageCache;
        public string PackageCode;
        public string PackageName;
        public string ProductID;
        public string RegOwner;
        public string RegCompany;
        public string SKUNumber;
        public string Transforms;
        public string URLInfoAbout;
        public string URLUpdateInfo;
        public string Vendor;
        public UInt32 WordCount;
        public string Version;
    };
}
