using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Shared;
using FOS.Web.UI.DAL;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Shared.Diagnostics.Logging;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
using ValidationResult = FluentValidation.Results.ValidationResult;
using FOS.Web.UI.Models;
using Newtonsoft.Json;
using ASPNET_MVC_Samples.Models;
using System.Net.Mail;

namespace FOS.Web.UI.Controllers
{
    public class EmailController : Controller
    {

        public ActionResult EmailReport()
        {
            QAData obj = new QAData();
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            obj.RegionsList = ManageAdmin.GetAllRegions();
            List<SubDepartmentData> DeptList = ManageAdmin.GetAllSubDepartment();
            obj.SubDept = DeptList;
            List<CenterDepartmentData> CenterDept = ManageAdmin.GetAllCenterDepartment();
            obj.CenterDept = CenterDept;
            obj.Audit = ManageAdmin.GetAuditForGrid();
            ViewBag.DataPoints = JsonConvert.SerializeObject(DataService.GetRandomDataForCategoryAxis(5), _jsonSetting);
            return View(obj);
        }
        public JsonResult GetTotalSummaryResultReport(int CategoryID, int CenterID, int audit_id, int RegionID)
        {
            FOSDataModel db = new FOSDataModel();
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();
            Audit audit = new Audit();
            Regions region = new Regions();
            audit = db.Audit.Where(u => u.audit_id == audit_id).FirstOrDefault();
            region = db.Regions.Where(u => u.ID == RegionID).FirstOrDefault();
            Approvel approvel=db.Approvel.Where(u=>u.AuditID==audit_id && u.CenterID==CenterID).FirstOrDefault();
            if (approvel.LeadApprovel == true && approvel.AdminApprovel == true)
            {
                try
                {
                    GetCLRList1_1_Result obj = new GetCLRList1_1_Result();
                    GetSummaryCLRList1_1_Result obj2 = new GetSummaryCLRList1_1_Result();
                    List<GetCLRList1_1_Result> dtsource = new List<GetCLRList1_1_Result>();
                    dtsource = ManageAdmin.GetCLRForGrid3(CategoryID, CenterID, audit_id);
                    for (int i = 0; i < dtsource.Count; i++)
                    {
                        int total = Convert.ToInt32(dtsource[i].TotalNumber + dtsource[i].ObtainedMarks + dtsource[i].ObtainedPercentage);
                        if (total == 0)
                        {
                            total = 1;
                        }
                        dtsource[i].FullPer = decimal.Round((Convert.ToDecimal(dtsource[i].TotalNumber / total) * 100), 2, MidpointRounding.AwayFromZero);
                        dtsource[i].PartialPer = decimal.Round((Convert.ToDecimal(dtsource[i].ObtainedMarks / total) * 100), 2, MidpointRounding.AwayFromZero);
                        dtsource[i].NotPer = decimal.Round((Convert.ToDecimal(dtsource[i].ObtainedPercentage / total) * 100), 2, MidpointRounding.AwayFromZero);
                    }
                    List<GetSummaryCLRList1_1_Result> dtsource2 = new List<GetSummaryCLRList1_1_Result>();
                    dtsource2 = ManageAdmin.GetSummaryCLRForGrid1(CategoryID, CenterID, audit_id);
                    ReportViewer1.ReportPath = Server.MapPath("~/Views/Admin/AuditSummaryResult.rdlc");
                    ReportViewer1.EnableExternalImages = true;
                    ReportDataSource dt = new ReportDataSource("DataSet2", dtsource);
                    ReportDataSource dt2 = new ReportDataSource("DataSet1", dtsource2);
                    ReportViewer1.DataSources.Clear();
                    int TotalRecord = dtsource.Where(x => x.TotalNumber != 0 || x.ObtainedMarks != 0 || x.ObtainedPercentage != 0).Count();
                    ReportParameterCollection ReportParamMeters = new ReportParameterCollection();
                    ReportParamMeters.Add(new ReportParameter("TotalRecord", TotalRecord.ToString()));
                    ReportParamMeters.Add(new ReportParameter("AuditYear", audit.audit_year.ToString()));
                    ReportParamMeters.Add(new ReportParameter("Region", region.Name.ToString()));
                    ReportViewer1.SetParameters(ReportParamMeters);
                    //</reportParameter>
                    ReportViewer1.DataSources.Add(dt);
                    ReportViewer1.DataSources.Add(dt2);
                    ReportViewer1.Refresh();
                    Warning[] warnings;
                    string[] streamIds;
                    string contentType;
                    string encoding;
                    string extension;
                    //Export the RDLC Report to Byte Array.
                    byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);
                    string filename = Server.MapPath("~/Reports/") + "Report.pdf";
                    using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Create))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();
                    }
                    Email(filename, CenterID, audit.audit_year.ToString());
                    return Json("Email Sent Successfully");
                }
                catch (Exception exp)
                {
                    Log.Instance.Error(exp, "Report Not Working");

                }
            }
            return Json("Email Not Sent Please First Approved the Reports");            
        }
        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        protected void Email(string file, int CenterID, string year)
        {
            FOSDataModel db = new FOSDataModel();
            AuditComponent auditComponent = new AuditComponent();
            auditComponent = db.AuditComponents.Where(u => u.SubDeptID == CenterID).FirstOrDefault();

            using (MailMessage mm = new MailMessage("AHF.Audit@gmail.com", "saimrasoolkambo@gmail.com"))
            {
                mm.Subject = "Performance Assessment Report " + year;
                mm.Body = "Dear Sir/Madam,<br>Assalam O Alaikum.<br>Hope you will be fine by the grace of ALLAH.<br>Please find the attached Performance Assessment Report. Kindly submit the compliance report after a month with supporting documents through Performance Assessment Application Login.<br>For any queries feel free to contact Mr. Abu Ul Hassan (Manager QIPS).<br>Mobile #. +92 3234875804"
                    + "<br>Last date to Submit Compliance Report" + DateTime.Now.AddMonths(1) + "<br> Regards,<br>Performance Assessor <br> <strong>Alkhidmat Health Foundation</strong>";
                mm.Attachments.Add(new Attachment(file));
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                NetworkCredential credential = new NetworkCredential();
                credential.UserName = "AHF.Audit@gmail.com";
                credential.Password = "pjhudgljbbiejrnw";
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = credential;
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(mm);
            }
        }


    }
}
