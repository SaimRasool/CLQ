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
    
    public partial class RatedNumber
    {
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public int CenterID { get; set; }
        public int DeptID { get; set; }
        public int RatedValue { get; set; }
        public Nullable<System.DateTime> OnCreated { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public int QuestionID { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ImagePath { get; set; }
        public string remarks { get; set; }
        public Nullable<int> AuditID { get; set; }
    }
}
