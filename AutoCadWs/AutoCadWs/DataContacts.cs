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
    }
    [DataContract]
    public class Operation
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int FirmId { get; set; }
        [DataMember]
        public Firm Firm { get; set; }
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract]
    public class Computer
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
    public class CheckLicense
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Nullable<int> ComputerId { get; set; }
        [DataMember]
        public string Output { get; set; }

        [DataMember]
        public Nullable<int> IsUnlicensed { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CheckDate { get; set; }

        [DataMember]
        public Nullable<System.DateTime> UpdateDate { get; set; }
        [DataMember]
        public Nullable<int> OperationId { get; set; }

    }
}
