using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using baileySoft.Wmi.Registry;
namespace MaterialDesignDemo.Controller
{
    public class ProcessWMI
    {
        public uint ProcessId;
        public int ExitCode;
        public bool EventArrived;
        public ManualResetEvent mre = new ManualResetEvent(false);
        static string RemoteComputerName;
        static string Username;
        static string Password;
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
        public ProcessWMI(string remoteComputerName, string username, string password)
        {
            RemoteComputerName = remoteComputerName;
            Username = username;
            Password = password;
            this.ProcessId = 0;
            ExitCode = -1;
            EventArrived = false;
        }
        public void ExecuteRemoteProcessWMI(string arguments, int WaitTimePerCommand)
        {

            try
            {
                ConnectionOptions connOptions = new ConnectionOptions();
                connOptions.Impersonation = ImpersonationLevel.Impersonate;
                connOptions.EnablePrivileges = true;
                connOptions.Username = Username;
                connOptions.Password = Password;

                ManagementScope manScope = new ManagementScope(String.Format(@"\\{0}\ROOT\CIMV2", RemoteComputerName), connOptions);

                try
                {
                    manScope.Connect();
                }
                catch (Exception e)
                {
                    throw new Exception("Management Connect to remote machine " + RemoteComputerName + " as user " + Username + " failed with the following error " + e.Message);
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
                                throw new Exception("Error while starting process " + arguments + " creation returned an exit code of " + outParams["returnValue"] + ". It was launched as " + Username + " on " + RemoteComputerName);
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
                            throw new Exception("ERROR AS WARNING: Process " + arguments + " terminated before it could be tracked on " + RemoteComputerName);
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
                            throw new Exception("Process " + arguments + " timed out and was killed on " + RemoteComputerName);
                        }
                    }
                }
                else
                {
                    if (this.ExitCode != 0)
                        throw new Exception("Process " + arguments + "exited with exit code " + this.ExitCode + " on " + RemoteComputerName + " run as " + Username);
                    else
                        Console.WriteLine("process exited with Exit code 0");
                }

            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Execute process failed Machinename {0}, ProcessName {1}, RunAs {2}, Error is {3}, Stack trace {4}", RemoteComputerName, arguments, Username, e.Message, e.StackTrace), e);
            }
        }


        public List<ApplicationEvent> GetApplicationEvent(string[] software)
        {
            List<ApplicationEvent> list = new List<ApplicationEvent>();

            ManagementScope scope = new ManagementScope("\\\\.\\root\\CIMV2");
            scope.Connect();

            SelectQuery query = new SelectQuery("Select * from Win32_NTLogEvent Where EventCode = 11724 ");
            using (ManagementObjectSearcher ProcessSearcher = new ManagementObjectSearcher(scope, query))
            {
                foreach (ManagementBaseObject log in ProcessSearcher.Get())
                {
                    ApplicationEvent item = new ApplicationEvent();

                    item.Message = log["Message"] == null ? null : log["Message"].ToString();
                    var param = software.Where(x => item.Message.Contains(x)).FirstOrDefault();
                    if (param == null) continue;

                    item.ComputerName = log["ComputerName"] == null ? null : log["ComputerName"].ToString();
                    item.Type = log["Type"] == null ? null : log["Type"].ToString();
                    item.User = log["User"] == null ? null : log["User"].ToString();
                    item.EventCode = log["EventCode"] == null ? null : log["EventCode"].ToString();
                    item.Category = log["Category"] == null ? null : log["Category"].ToString();
                    item.SourceName = log["SourceName"] == null ? null : log["SourceName"].ToString();
                    item.RecordNumber = log["RecordNumber"] == null ? null : log["RecordNumber"].ToString();
                    item.TimeWritten = getDateTimeFromDmtfDate(log["TimeWritten"] == null ? null : log["TimeWritten"].ToString());
                    list.Add(item);
                }
            }
            return list;
        }
        private static string getDmtfFromDateTime(DateTime dateTime)
        {
            return ManagementDateTimeConverter.ToDmtfDateTime(dateTime);
        }

        private static string getDmtfFromDateTime(string dateTime)
        {
            DateTime dateTimeValue = Convert.ToDateTime(dateTime);
            return getDmtfFromDateTime(dateTimeValue);
        }

        private static string getDateTimeFromDmtfDate(string dateTime)
        {
            return ManagementDateTimeConverter.ToDateTime(dateTime).ToString();
        }

        public List<Win32_Product> GetProductWithWMI(string[] software)
        {
            List<Win32_Product> products = new List<Win32_Product>();

            //ConnectionOptions connOptions = new ConnectionOptions();
            //connOptions.Impersonation = ImpersonationLevel.Impersonate;
            //connOptions.EnablePrivileges = true;
            //connOptions.Username = username;
            //connOptions.Password = password;

            //ManagementScope scope = new ManagementScope(String.Format(@"\\{0}\ROOT\CIMV2", remoteComputerName), connOptions);

            //try
            //{
            //    scope.Connect();
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Management Connect to remote machine " + remoteComputerName + " as user " + username + " failed with the following error " + e.Message);
            //}
            ManagementScope scope = new ManagementScope("\\\\.\\root\\CIMV2");
            scope.Connect();

            SelectQuery CheckProcess = new SelectQuery("SELECT * FROM Win32_Product");
            using (ManagementObjectSearcher ProcessSearcher = new ManagementObjectSearcher(scope, CheckProcess))
            {
                var WMIproducts = ProcessSearcher.Get();
                foreach (ManagementObject mo in WMIproducts)
                {
                    Win32_Product product = new Win32_Product();

                    product.Name = mo["Name"] == null ? null : mo["Name"].ToString();
                    if (product.Name == null) continue;

                    var param = software.Where(x => product.Name.Contains(x)).FirstOrDefault();
                    if (param == null) continue;

                    product.AssignmentType = mo["AssignmentType"].ToString();
                    product.Caption = mo["Caption"] == null ? null : mo["Caption"].ToString();
                    product.Description = mo["Description"] == null ? null : mo["Description"].ToString();
                    product.HelpLink = mo["HelpLink"] == null ? null : mo["HelpLink"].ToString();
                    product.HelpTelephone = mo["HelpTelephone"] == null ? null : mo["HelpTelephone"].ToString();
                    product.IdentifyingNumber = mo["IdentifyingNumber"] == null ? null : mo["IdentifyingNumber"].ToString();
                    product.InstallDate = mo["InstallDate"] == null ? null : mo["InstallDate"].ToString();
                    product.InstallDate2 = mo["InstallDate2"] == null ? null : mo["InstallDate2"].ToString();
                    product.InstallLocation = mo["InstallLocation"] == null ? null : mo["InstallLocation"].ToString();
                    product.InstallSource = mo["InstallSource"] == null ? null : mo["InstallSource"].ToString();
                    product.InstallState = mo["InstallState"] == null ? null : mo["InstallState"].ToString();
                    product.Language = mo["Language"] == null ? null : mo["Language"].ToString();
                    product.LocalPackage = mo["LocalPackage"] == null ? null : mo["LocalPackage"].ToString();
                    product.PackageCache = mo["PackageCache"] == null ? null : mo["PackageCache"].ToString();
                    product.PackageCode = mo["PackageCode"] == null ? null : mo["PackageCode"].ToString();
                    product.PackageName = mo["PackageName"] == null ? null : mo["PackageName"].ToString();
                    product.ProductID = mo["ProductID"] == null ? null : mo["ProductID"].ToString();
                    product.RegCompany = mo["RegCompany"] == null ? null : mo["RegCompany"].ToString();
                    product.RegOwner = mo["RegOwner"] == null ? null : mo["RegOwner"].ToString();
                    product.SKUNumber = mo["SKUNumber"] == null ? null : mo["SKUNumber"].ToString();
                    product.Transforms = mo["Transforms"] == null ? null : mo["Transforms"].ToString();
                    product.URLInfoAbout = mo["URLInfoAbout"] == null ? null : mo["URLInfoAbout"].ToString();
                    product.URLUpdateInfo = mo["URLUpdateInfo"] == null ? null : mo["URLUpdateInfo"].ToString();
                    product.Vendor = mo["Vendor"] == null ? null : mo["Vendor"].ToString();
                    product.Version = mo["Version"] == null ? null : mo["Version"].ToString();



                    products.Add(product);
                }
            }
            return products;
        }

        public List<Software> ReadRegisteryusingWMI(string[] software)
        {

            var list32 = ReadUnInstallRegistryusingWMICore(software, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            var list64 = ReadUnInstallRegistryusingWMICore(software, @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            var registryautodeskautodesk = ReadRegistrySubkeys(software, @"SOFTWARE\Autodesk");
            var registryautodeskautodesk64 = ReadRegistrySubkeys(software, @"SOFTWARE\Wow6432Node\Autodesk");
            var registryflexlmlicensemanager = ReadRegistrySubkeys(software, @"SOFTWARE\flexlm license manager\");
            var registryflexlmlicensemanager64 = ReadRegistrySubkeys(software, @"SOFTWARE\Wow6432Node\flexlm license manager\");



            var uniqueList = list32.Concat(list64)
                                   .Where(x => x.DisplayName != null)
                                   .GroupBy(item => item.DisplayName)
                                   .Select(group => group.First())
                                   .ToList();

            return uniqueList;
        }
        private List<Software> ReadUnInstallRegistryusingWMICore(string[] software, string softwareRegLoc)
        {
            List<Software> programs = new List<Software>();

            ConnectionOptions connOptions = new ConnectionOptions();
            connOptions.Impersonation = ImpersonationLevel.Impersonate;
            connOptions.EnablePrivileges = true;
            connOptions.Username = Username;
            connOptions.Password = Password;

            ManagementScope scope = new ManagementScope(String.Format(@"\\{0}\ROOT\CIMV2", RemoteComputerName), connOptions);
            scope.Options.Context.Add("__ProviderArchitecture", 64);
            scope.Options.Context.Add("__RequiredArchitecture", true);
            try
            {
                scope.Connect();
            }
            catch (Exception e)
            {
                throw new Exception("Management Connect to remote machine " + RemoteComputerName + " as user " + Username + " failed with the following error " + e.Message);
            }
            //ManagementScope scope = new ManagementScope("\\\\.\\root\\CIMV2");
            //scope.Connect();

            ManagementClass registry = new ManagementClass(scope, new ManagementPath("StdRegProv"), null);
            ManagementBaseObject inParams = registry.GetMethodParameters("EnumKey");
            inParams["hDefKey"] = 0x80000002;//HKEY_LOCAL_MACHINE
            inParams["sSubKeyName"] = softwareRegLoc;

            // Read Registry Key Names 
            ManagementBaseObject outParams = registry.InvokeMethod("EnumKey", inParams, null);
            string[] programGuids = outParams["sNames"] as string[];

            if (programGuids == null) return programs;

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
        private List<Software> ReadRegistrySubkeys(string[] software, string softwareRegLoc)
        {

            List<Software> list = new List<Software>();
            RegistryObject SysRegistry =
                 new RegistryRemote(Username,
                                             Password,
                                             "",
                                             RemoteComputerName,
                                             "64");

            //Instantiate Local Client
            //RegistryObject SysRegistry = new RegistryLocal();
            string registryKey = softwareRegLoc;

            try
            {
                foreach (string subKey in SysRegistry.EnumerateKeys(
                            baileySoft.Wmi.Registry.baseKey.HKEY_LOCAL_MACHINE, registryKey))
                {
                    list.Add(new Software { DisplayName = softwareRegLoc + "\\" + subKey });
                    //var sublist = ReadRegistrySubkeys(software, softwareRegLoc + "\\" + subKey, remoteComputerName, username, password);
                    //list = list.Concat(sublist).ToList();
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return list;
        }

        public FileExplorerModel DirectoryChecklist()
        {

            List<MyDirectory> directories = new List<MyDirectory>();
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\autode~1\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\autode~1\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\autoca~1\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\autoca~1\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\docume~1\\\\alluse~1\\\\applic~1\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\docume~1\\\\alluse~1\\\\applic~1\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\Users\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\Users\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\Users\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\Users\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\docume~1\\\\alluse~1\\\\applic~1\\\\autodesk\\\\*.html", Type = "file" });//html
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\docume~1\\\\alluse~1\\\\applic~1\\\\autodesk\\\\*.err", Type = "dir" });//error 
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\Users\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\*.html", Type = "file" });//html
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\Users\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\*.err", Type = "file" });//error
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\Users\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\*.html", Type = "file" });//html
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\Users\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\*.err", Type = "file" });//erorr
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\Users\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\*.html", Type = "file" });//html
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\Users\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\*.err", Type = "file" });//erorr
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\programdata\\\\autodesk\\\\*.html", Type = "file" });//html
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\programdata\\\\autodesk\\\\*.html", Type = "file" });//html
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\autoca~1\\\\*.lic", Type = "file" });//lic
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\autoca~1\\\\*.lic", Type = "file" });//lic
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\autode~1\\\\*.lic", Type = "file" });//lic
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\autode~1\\\\*.lic", Type = "file" });//lic
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\autodesk\\\\revit\\\\*.lic", Type = "file" });//lic
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\autodesk\\\\revit\\\\*.lic", Type = "file" });//lic
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\revit\\\\*.lic", Type = "file" });//lic
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\revit\\\\*.lic", Type = "file" });//lic
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\autodesk\\\\AutoCA~1\\\\*.lic", Type = "file" });//lic
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\autodesk\\\\AutoCA~1\\\\*.lic", Type = "file" });//lic
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kulla~1\\\\applic~1\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\docume~1\\\\alluse~1\\\\applic~1\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kulla~1\\\\applic~1\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\docume~1\\\\alluse~1\\\\applic~1\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kulla~1\\\\applic~1\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kullan~1\\\\applic~1\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\kulla~1\\\\applic~1\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\kullan~1\\\\applic~1\\\\autodesk\\\\", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kulla~1\\\\applic~1\\\\autodesk\\\\*.html", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kulla~1\\\\applic~1\\\\autodesk\\\\*.err", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\docume~1\\\\alluse~1\\\\applic~1\\\\autodesk\\\\*.html", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\docume~1\\\\alluse~1\\\\applic~1\\\\autodesk\\\\*.err", Type = "dir" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\*.html", Type = "file" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\*.err", Type = "file" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\*.html", Type = "file" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\roaming\\\\autodesk\\\\*.err", Type = "file" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\*.html", Type = "file" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\*.err", Type = "file" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\*.html", Type = "file" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\kullanıcılar\\\\Admini~1\\\\appdata\\\\local\\\\autodesk\\\\*.err", Type = "file" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\programdata\\\\autodesk\\\\*.html", Type = "file" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\programdata\\\\autodesk\\\\*.html", Type = "file" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\\autoca~1\\\\*.lic", Type = "file" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\autoca~1\\\\*.lic", Type = "file" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\\autode~1\\\\*.lic", Type = "file" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\autode~1\\\\*.lic", Type = "file" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\autodesk\\\\revit\\\\*.lic", Type = "file" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\autodesk\\\\revit\\\\*.lic", Type = "file" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\revit\\\\*.lic", Type = "file" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\revit\\\\*.lic", Type = "file" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\progra~1\\\\autodesk\\\\AutoCA~1\\\\*.lic", Type = "file" });
            directories.Add(new MyDirectory { Drive = "d:", Path = "\\\\progra~1\\\\autodesk\\\\AutoCA~1\\\\*.lic", Type = "file" });
            directories.Add(new MyDirectory { Drive = "c:", Path = "\\\\NBHIKMETY01\\\\", Type = "dir" });


            FileExplorerModel model = new FileExplorerModel();

            List<Win32_Directory> list = new List<Win32_Directory>();
            foreach (var item in directories)
            {
                if (item.Type == "dir")
                    model.directories=model.directories.Concat(GetDirectory(item.Drive, item.Path)).ToList();
                else if (item.Type == "file")
                    model.files=model.files.Concat(GetFiles(item.Path, item.Drive)).ToList();
            }
            return model;
        }
        private List<Win32_Directory> GetDirectory(string Drive, string Path)
        {

            List<Win32_Directory> dirs = new List<Win32_Directory>();

            ManagementScope scope = new ManagementScope("\\\\.\\root\\CIMV2");
            scope.Connect();

            ObjectQuery Query = new ObjectQuery(string.Format("SELECT * FROM win32_directory Where Drive='{0}' AND Path='{1}' ", Drive, Path));

            ManagementObjectSearcher Searcher = new ManagementObjectSearcher(scope, Query);

            foreach (ManagementObject WmiObject in Searcher.Get())
            {
                var dir = new Win32_Directory();
                dir.Name = WmiObject["Name"] == null ? null : WmiObject["Name"].ToString();
                dir.Path = WmiObject["Path"] == null ? null : WmiObject["Path"].ToString();
                dir.Readable = WmiObject["Readable"] == null ? null : WmiObject["Readable"].ToString();
                dir.Status = WmiObject["Status"] == null ? null : WmiObject["Status"].ToString();
                dir.System = WmiObject["System"] == null ? null : WmiObject["System"].ToString();
                dir.Writeable = WmiObject["Writeable"] == null ? null : WmiObject["Writeable"].ToString();
                dir.AccessMask = WmiObject["AccessMask"] == null ? null : WmiObject["AccessMask"].ToString();
                dir.Archive = WmiObject["Archive"] == null ? null : WmiObject["Archive"].ToString();
                dir.Caption = WmiObject["Caption"] == null ? null : WmiObject["Caption"].ToString();
                dir.Compressed = WmiObject["Compressed"] == null ? null : WmiObject["Compressed"].ToString();
                dir.CompressionMethod = WmiObject["CompressionMethod"] == null ? null : WmiObject["CompressionMethod"].ToString();
                dir.CreationClassName = WmiObject["CreationClassName"] == null ? null : WmiObject["CreationClassName"].ToString();
                dir.CreationDate = WmiObject["CreationDate"] == null ? null : WmiObject["CreationDate"].ToString();
                dir.CSCreationClassName = WmiObject["CSCreationClassName"] == null ? null : WmiObject["CSCreationClassName"].ToString();
                dir.CSName = WmiObject["CSName"] == null ? null : WmiObject["CSName"].ToString();
                dir.Description = WmiObject["Description"] == null ? null : WmiObject["Description"].ToString();
                dir.Drive = WmiObject["Drive"] == null ? null : WmiObject["Drive"].ToString();
                dir.EightDotThreeFileName = WmiObject["EightDotThreeFileName"] == null ? null : WmiObject["EightDotThreeFileName"].ToString();
                dir.Encrypted = WmiObject["Encrypted"] == null ? null : WmiObject["Encrypted"].ToString();
                dir.EncryptionMethod = WmiObject["EncryptionMethod"] == null ? null : WmiObject["EncryptionMethod"].ToString();
                dir.Extension = WmiObject["Extension"] == null ? null : WmiObject["Extension"].ToString();
                dir.FileName = WmiObject["FileName"] == null ? null : WmiObject["FileName"].ToString();
                dir.FileSize = WmiObject["FileSize"] == null ? null : WmiObject["FileSize"].ToString();
                dir.FileType = WmiObject["FileType"] == null ? null : WmiObject["FileType"].ToString();
                dir.FSCreationClassName = WmiObject["FSCreationClassName"] == null ? null : WmiObject["FSCreationClassName"].ToString();
                dir.FSName = WmiObject["FSName"] == null ? null : WmiObject["FSName"].ToString();
                dir.Hidden = WmiObject["Hidden"] == null ? null : WmiObject["Hidden"].ToString();
                dir.InstallDate = WmiObject["InstallDate"] == null ? null : WmiObject["InstallDate"].ToString();
                dir.InUseCount = WmiObject["InUseCount"] == null ? null : WmiObject["InUseCount"].ToString();
                dir.LastAccessed = WmiObject["LastAccessed"] == null ? null : WmiObject["LastAccessed"].ToString();
                dir.LastModified = WmiObject["LastModified"] == null ? null : WmiObject["LastModified"].ToString();
                dir.files = GetFiles(dir.Path.Replace("\\", "\\\\"), dir.Drive);
                dirs.Add(dir);
            }

            return dirs;
        }
        public List<CIM_DataFile> GetFiles(string Path, string Drive)
        {
            List<CIM_DataFile> files = new List<CIM_DataFile>();

            ManagementScope scope = new ManagementScope("\\\\.\\root\\CIMV2");
            scope.Connect();
            //look how the \ char is escaped. 
            ObjectQuery Query = new ObjectQuery(string.Format("SELECT * FROM CIM_DataFile  Where Drive='{0}' AND Path='{1}' ", Drive, Path));

            ManagementObjectSearcher Searcher = new ManagementObjectSearcher(scope, Query);

            foreach (ManagementObject WmiObject in Searcher.Get())
            {
                var item = new CIM_DataFile();

                item.AccessMask = WmiObject["AccessMask"] == null ? null : WmiObject["AccessMask"].ToString();
                item.Archive = WmiObject["Archive"] == null ? null : WmiObject["Archive"].ToString();
                item.Caption = WmiObject["Caption"] == null ? null : WmiObject["Caption"].ToString();
                item.Compressed = WmiObject["Compressed"] == null ? null : WmiObject["Compressed"].ToString();
                item.CompressionMethod = WmiObject["CompressionMethod"] == null ? null : WmiObject["CompressionMethod"].ToString();
                item.CreationClassName = WmiObject["CreationClassName"] == null ? null : WmiObject["CreationClassName"].ToString();
                item.CreationDate = WmiObject["CreationDate"] == null ? null : WmiObject["CreationDate"].ToString();
                item.CSCreationClassName = WmiObject["CSCreationClassName"] == null ? null : WmiObject["CSCreationClassName"].ToString();
                item.CSName = WmiObject["CSName"] == null ? null : WmiObject["CSName"].ToString();
                item.Description = WmiObject["Description"] == null ? null : WmiObject["Description"].ToString();
                item.Drive = WmiObject["Drive"] == null ? null : WmiObject["Drive"].ToString();
                item.EightDotThreeFileName = WmiObject["EightDotThreeFileName"] == null ? null : WmiObject["EightDotThreeFileName"].ToString();
                item.Encrypted = WmiObject["Encrypted"] == null ? null : WmiObject["Encrypted"].ToString();
                item.EncryptionMethod = WmiObject["EncryptionMethod"] == null ? null : WmiObject["EncryptionMethod"].ToString();
                item.Extension = WmiObject["Extension"] == null ? null : WmiObject["Extension"].ToString();
                item.FileName = WmiObject["FileName"] == null ? null : WmiObject["FileName"].ToString();
                item.FileSize = WmiObject["FileSize"] == null ? null : WmiObject["FileSize"].ToString();
                item.FileType = WmiObject["FileType"] == null ? null : WmiObject["FileType"].ToString();
                item.FSCreationClassName = WmiObject["FSCreationClassName"] == null ? null : WmiObject["FSCreationClassName"].ToString();
                item.FSName = WmiObject["FSName"] == null ? null : WmiObject["FSName"].ToString();
                item.Hidden = WmiObject["Hidden"] == null ? null : WmiObject["Hidden"].ToString();
                item.InstallDate = WmiObject["InstallDate"] == null ? null : WmiObject["InstallDate"].ToString();
                item.InUseCount = WmiObject["InUseCount"] == null ? null : WmiObject["InUseCount"].ToString();
                item.LastAccessed = WmiObject["LastAccessed"] == null ? null : WmiObject["LastAccessed"].ToString();
                item.LastModified = WmiObject["LastModified"] == null ? null : WmiObject["LastModified"].ToString();
                item.Manufacturer = WmiObject["Manufacturer"] == null ? null : WmiObject["Manufacturer"].ToString();
                item.Name = WmiObject["Name"] == null ? null : WmiObject["Name"].ToString();
                item.Path = WmiObject["Path"] == null ? null : WmiObject["Path"].ToString();
                item.Readable = WmiObject["Readable"] == null ? null : WmiObject["Readable"].ToString();
                item.Status = WmiObject["Status"] == null ? null : WmiObject["Status"].ToString();
                item.System = WmiObject["System"] == null ? null : WmiObject["System"].ToString();
                item.Version = WmiObject["Version"] == null ? null : WmiObject["Version"].ToString();
                item.Writeable = WmiObject["Writeable"] == null ? null : WmiObject["Writeable"].ToString();
                files.Add(item);
            }

            return files;
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
        public string AssignmentType;
        public string Caption;
        public string Description;
        public string IdentifyingNumber;
        public string InstallDate;
        public string InstallDate2;
        public string InstallLocation;
        public string InstallState;
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
        public string WordCount;
        public string Version;
    };
    public class ApplicationEvent
    {
        public string Message { get; set; }
        public string ComputerName { get; set; }
        public string Type { get; set; }
        public string User { get; set; }
        public string EventCode { get; set; }
        public string Category { get; set; }
        public string SourceName { get; set; }
        public string RecordNumber { get; set; }
        public string TimeWritten { get; set; }
    }

    public class Win32_Directory
    {
        public List<CIM_DataFile> files = new List<CIM_DataFile>();
        public string Caption { get; set; }
        public string Description { get; set; }
        public string InstallDate { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string AccessMask { get; set; }
        public string Archive { get; set; }
        public string Compressed { get; set; }
        public string CompressionMethod { get; set; }
        public string CreationClassName { get; set; }
        public string CreationDate { get; set; }
        public string CSCreationClassName { get; set; }
        public string CSName { get; set; }
        public string Drive { get; set; }
        public string EightDotThreeFileName { get; set; }
        public string Encrypted { get; set; }
        public string EncryptionMethod { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public string FSCreationClassName { get; set; }
        public string FSName { get; set; }
        public string Hidden { get; set; }
        public string InUseCount { get; set; }
        public string LastAccessed { get; set; }
        public string LastModified { get; set; }
        public string Path { get; set; }
        public string Readable { get; set; }
        public string System { get; set; }
        public string Writeable { get; set; }
    };

    public class CIM_DataFile
    {
        public string Caption { get; set; }
        public string Description { get; set; }
        public string InstallDate { get; set; }
        public string Status { get; set; }
        public string AccessMask { get; set; }
        public string Archive { get; set; }
        public string Compressed { get; set; }
        public string CompressionMethod { get; set; }
        public string CreationClassName { get; set; }
        public string CreationDate { get; set; }
        public string CSCreationClassName { get; set; }
        public string CSName { get; set; }
        public string Drive { get; set; }
        public string EightDotThreeFileName { get; set; }
        public string Encrypted { get; set; }
        public string EncryptionMethod { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public string FSCreationClassName { get; set; }
        public string FSName { get; set; }
        public string Hidden { get; set; }
        public string InUseCount { get; set; }
        public string LastAccessed { get; set; }
        public string LastModified { get; set; }
        public string Path { get; set; }
        public string Readable { get; set; }
        public string System { get; set; }
        public string Writeable { get; set; }
        public string Manufacturer { get; set; }
        public string Version { get; set; }
    };
    public class MyDirectory
    {
        public string Drive { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
    }
    public class FileExplorerModel
    {
        public List<Win32_Directory> directories = new List<Win32_Directory>();
        public List<CIM_DataFile> files = new List<CIM_DataFile>();
    }
}
