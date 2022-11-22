using FOS.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FOS.Shared
{
    public class AdminData
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "* Required")]
        public string Name { get; set; }
    }
    public class RegionData
    {
        public int ID { get; set; }

         [Required(ErrorMessage = "* Required")]
        public string Name { get; set; }
        public string ShortCode { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
    public class SubDepartmentData
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "*Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fax { get; set; }
        public string Telephone { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string Address { get; set; }
        public string SubDepartment { get; set; }
        public string Department { get; set; }
        public List<Department> Dept { get; set; }
        public int DeptID { get; set; }
        public List<Regions> RegionsList { get; set; }
        public int RegionID { get; set; }
        public string Region { get; set; }
    }
    public class CenterDepartmentData
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class DepartmentData
    {
        public int ID { get; set; }
        [Required(ErrorMessage="*Required")]
        public string Name { get; set; }
        public List<Department> Category { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public List<SubDepartmentData> Center { get; set; }
        public int CenterID { get; set; }
        public string CenterName { get; set; }
        public DateTime  OnCreated { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class QAData
    {
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public string Center { get; set; }
        public string Department { get; set; }
        public decimal? TotalMarks { get; set; }
        public decimal? TotalNumber { get; set; }
        public string Question { get; set; }
        public int RatedMarks { get; set; }
        public decimal? TM { get; set; }
        public decimal? OM { get; set; }
        public decimal? NA { get; set; }
        public decimal TOP { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public decimal? ObtainedPercentage { get; set; }
        public int? NotApplicable { get; set; }
        public int QAID { get; set; }
        public string QuestionName { get; set; }
        public string QuestionHint { get; set; }
        public List<Department> Dept { get; set; }
        public int DeptID { get; set; }
        public List<SubDepartmentData> SubDept { get; set; }
        public int SubDeptID { get; set; }
        public List<CenterDepartmentData> CenterDept { get; set; }
        public int CenterDeptID { get; set; }
        //new added
        public int RatedValue { get; set; }
        public string remarks { get; set; }
        public List<QAData> QADataList { get; set; }
        public List<Regions> RegionsList { get; set; }
        public int RegionID { get; set; }
        public List<Audit> Audit { get; set; }
        public int audit_id { get; set; }
        public decimal? AuditYear { get; set; }
        public List<ComplianceReport> ComplianceReports { get; set; }

    }
    public class RatingData
    {
        public int TRQID { get; set; }
        public int RID { get; set; }
        public int ParentRID { get; set; }
        public string RName { get; set; }
        public string RHint { get; set; }
        public int RatedNumber { get; set; }
        public string Path { get; set; }
        public string MenuInitials { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool ShowMenu { get; set; }
        public string Access { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Delete { get; set; }
        public bool Update { get; set; }
        public bool One { get; set; }
        public bool Two { get; set; }
        public bool Three { get; set; }
        public bool Four { get; set; }
        public bool Five { get; set; }
        public bool Six { get; set; }
        public bool Seven { get; set; }
        public bool Eight { get; set; }
        public bool Nine { get; set; }
        public bool Ten { get; set; }
        public bool NA { get; set; }
        public bool Zero { get; set; }
        public int TotalChildPages { get; set; }
        public string Type { get; set; }
        public bool Status { get; set; }
        public string Icon { get; set; }
        public string remarks { get; set; }

    }
    public class CalculateMark
    {
        public int OM { get; set; }
        public double OMP { get; set; }
        public int TM { get; set; }
        public int NA { get; set; }
    }
    public class DoctorData
    {
        public int EmpID { get; set; }
        public string Prefix { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string EFName { get; set; }
        public string EMName { get; set; }
        public string ELName { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime JoiningData { get; set; }
        public string JoiningDate { get; set; }
        [RegularExpression("^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "CNIC No must follow the XXXXX-XXXXXXX-X format!")]
        public string CNIC { get; set; }
        //public List<DesignationData> Designation { get; set; }
        public string DesignationName { get; set; }
        public int DesignationID { get; set; }
        public int? PayScale { get; set; }
        public string HPhoneNo { get; set; }
        public string OPhoneNo { get; set; }
        public string OfficeAdd { get; set; }
        public string HomeAdd { get; set; }
        public string Gender { get; set; }
        public List<Department> Dept { get; set; }
        public int DeptID { get; set; }
        public List<SubDepartmentData> SubDept { get; set; }
        public int SubDeptID { get; set; }
        public string MobileNo { get; set; }
        public string PMDC_No { get; set; }
        public DateTime DateOfTermination { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        //public List<Day> Days { get; set; }
        public string WorkingDaysID { get; set; }
     
        public string UserName { get; set; }

        public string Password { get; set; }
        [NotMapped] // Does not effect with your database
        [Compare("Password")]
        public string ConfirmPass { get; set; }
        [DisplayName("Doctor Image *")]
        public string DrImgPath { get; set; }
        public HttpPostedFileBase DrImg { get; set; }
        //public HttpPostedFileBase ImageFile { get; set; }

    }
    public class StaffDetailData
    {
        public int id { get; set; }
        public int full_time { get; set; }
        public int part_time { get; set; }
        public int visiting { get; set; }
        public int male { get; set; }
        public int female { get; set; }
        

        public List<Department> Category { get; set; }
        public int CategoryID { get; set; }
        public List<SubDepartmentData> Center { get; set; }
        public int CenterID { get; set; }
        public string SubDeptName { get; set; }
        public List<StaffType> StaffTypes { get; set; }
        public int StaffTypeId { get; set; }
        public List<StaffSubTypeData> StaffSubTypes { get; set; }
        public List<Regions> RegionsList { get; set; }
        public int RegionID { get; set; }
        public int sub_staff_id { get; set; }
        public string Name { get; set; }

    }
    public class StaffSubTypeData
    {
        public decimal ID { get; set; }
        public int StaffTypeId { get; set; }
        public string Name { get; set; }
    }

    public partial class ObservationData
    {
        public int ID { get; set; }
        public int CenterID { get; set; }
        public string CenterDeptName { get; set; }
        public string Observation { get; set; }
        public string Recomendation { get; set; }
        public List<Regions> RegionsList { get; set; }
        public int RegionID { get; set; }
        public List<Department> Dept { get; set; }
        public int DeptID { get; set; }
        public List<SubDepartmentData> SubDept { get; set; }
        public int SubDeptID { get; set; }

        public decimal? AuditYear { get; set; }
    }
}