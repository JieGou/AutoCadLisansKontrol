//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutoCadLisansKontrol.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ComputerEntity
    {
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
    
        public virtual FirmEntity Firm { get; set; }
    }
}
