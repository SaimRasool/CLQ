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
    
    public partial class Designation
    {
        public int Designation_ID { get; set; }
        public string Designation_Name { get; set; }
        public Nullable<int> Designation_Main_Id { get; set; }
        public Nullable<int> Min_PayScale { get; set; }
        public string Designation_Type { get; set; }
        public string Main_Cat { get; set; }
        public Nullable<int> Dept_Id { get; set; }
        public Nullable<bool> Request_Forwarding { get; set; }
        public string Designation_Abb { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}