using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LicenseControllerWs.DAL
{
    [DataContract]
    public partial class UsersDTO
    {

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
    [DataContract]
    public class FirmDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string Contact { get; set; }
        [DataMember]
        public string PhoneNo { get; set; }
        [DataMember]
        public System.DateTime InsertDate { get; set; }
        [DataMember]
        public int UserId { get; set; }
    }
    [DataContract]
    public class OperationDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int FirmId { get; set; }
        [DataMember]
        public FirmDTO Firm { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ComputerCount { get; set; }
    }

    [DataContract]
    public class ComputerDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Ip { get; set; }
        [DataMember]
        public string PyshicalAddress { get; set; }
        [DataMember]
        public bool IsRootMachine { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public bool IsComputer { get; set; }
        [DataMember]
        public int FirmId { get; set; }
        [DataMember]
        public bool IsVisible { get; set; }
        [DataMember]
        public System.DateTime InsertDate { get; set; }

    }
    [DataContract]
    public class CheckLicenseDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public Nullable<int> ComputerId { get; set; }
        [DataMember]
        public string Output { get; set; }

        [DataMember]
        public Nullable<System.DateTime> CheckDate { get; set; }

        [DataMember]
        public Nullable<System.DateTime> UpdateDate { get; set; }
        [DataMember]
        public Nullable<int> OperationId { get; set; }
        [DataMember]
        public Nullable<int> FirmId { get; set; }
        [DataMember]
        public string MachineName { get; set; }
        [DataMember]
        public Nullable<bool> State { get; set; }
        [DataMember]
        public Nullable<bool> Installed { get; set; }
        [DataMember]
        public Nullable<bool> Uninstalled { get; set; }

        [DataMember]
        public DateTime? InstallDate { get; set; }

        [DataMember]
        public DateTime? UnInstallDate { get; set; }

        [DataMember]
        public Nullable<bool> IsFound { get; set; }

        [DataMember]
        public Nullable<int> AppId { get; set; }
        [DataMember]
        public Nullable<System.Guid> LogId { get; set; }
    }
    [DataContract]
    public class SoftwareDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string AppName { get; set; }
        [DataMember]
        public Nullable<bool> IsEnable { get; set; }

    }
    [DataContract]
    public class FE_ControlListDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Drive { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public Nullable<int> AppId { get; set; }
    }
    [DataContract]
    public partial class ControlPointDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Nullable<bool> WillChecked { get; set; }
        [DataMember]
        public string AvgExecTime { get; set; }
    }
}
