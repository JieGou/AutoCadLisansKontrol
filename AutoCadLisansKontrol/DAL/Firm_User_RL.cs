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
    
    public partial class Firm_User_RL
    {
        public Nullable<int> FirmId { get; set; }
        public Nullable<int> UserId { get; set; }
        public int Id { get; set; }
    
        public virtual FirmEntity Firm { get; set; }
        public virtual Users Users { get; set; }
    }
}
