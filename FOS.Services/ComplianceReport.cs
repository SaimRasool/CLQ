//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FOS.DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class ComplianceReport
    {
        public int Id { get; set; }
        public Nullable<int> CenterID { get; set; }
        public Nullable<int> AuditID { get; set; }
        public string Path { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    }
}
