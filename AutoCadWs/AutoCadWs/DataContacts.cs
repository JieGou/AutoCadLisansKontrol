using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoCadWs
{
    [DataContract]
    public class Firm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string PhoneNo { get; set; }
        public System.DateTime InsertDate { get; set; }
    }
    [DataContract]
    public class Operation {
        public int Id { get; set; }
        public int FirmId { get; set; }
        public string Name { get; set; }
    }

    [DataContract]
    public class Computer {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public string PyshicalAddress { get; set; }
        public bool IsRootMachine { get; set; }
        public string Type { get; set; }
        public bool IsComputer { get; set; }
        public int FirmId { get; set; }
        public bool IsVisible { get; set; }
        public System.DateTime InsertDate { get; set; }
        public Visibility Visibility { get { return IsVisible == true ? Visibility.Visible : Visibility.Hidden; } }
    }
    public class CheckLicense {
        public int Id { get; set; }
        public Nullable<int> ComputerId { get; set; }
        public string Output { get; set; }
        public Nullable<int> IsUnlicensed { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> OperationId { get; set; }
    }
}
