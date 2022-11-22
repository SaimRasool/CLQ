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
using System.Linq.Dynamic;
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
using System.Web.Security;
using FOS.Web.UI.Common;
using static System.Windows.Forms.DataFormats;
using System.IO;

namespace FOS.Web.UI.Controllers
{
    public class DepartmentController : Controller
    {
        [System.Web.Http.Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [System.Web.Http.Authorize]
        public ActionResult Login()
        {
            return View();
        }
        public JsonResult UserAuth(string userName, string password, string returnUrl)
        {
            Log.Instance.Info("A new user is trying to sign in");
            int userId = 0;
            userId = FOS.AdminPanel.ManageLogin.DepartmentUserAuth(userName, password);
            string response = FOS.AdminPanel.ManageLogin.DepartmentUserAuthTest(userName, password);
            if (userId > 0)
            {
                ViewBag.res = userId.ToString();

                Log.Instance.Info("Correct credentials");
                Session["UserID"] = userId;
                SessionManager.Store("UserName", userName.ToString());
                SessionManager.Store("UserID", userId);
                FormsAuthentication.SetAuthCookie(userName, false);
            }
            else
            {
                Log.Instance.Info("Login failed");
                SessionManager.Destroy("UserName");
            }

            string pageUrl = string.IsNullOrEmpty(returnUrl) ? string.Format("{0}/Department/Index", Settings.AppPath) : returnUrl;
            return Json(new { status = response, url = pageUrl }, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.Authorize]

        public void Logout()
        {
            Session["AppAuth"] = false;
            Session["UserName"] = null;

            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("~/Department/Login");
        }
        [System.Web.Http.Authorize]
        public ActionResult Reports()
        {
            FOSDataModel db = new FOSDataModel();
            var approvel = db.Approvel.Where(u=>u.LeadApprovel == false && u.AdminApprovel == false).ToList();
            QAData obj = new QAData();
            obj.Audit = ManageAdmin.GetAuditForGrid();
            foreach (var author in approvel)
            {
                obj.Audit.RemoveAll(x => x.audit_id == author.AuditID);
            }
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            if (System.Web.HttpContext.Current.Session["UserID"]==null)
            {
                Response.Redirect("~/Department/Login");
            }
            obj.CenterDeptID = (int)Session["UserID"];
            SubDepartment  sub= db.SubDepartments.Where(u => u.ID == obj.CenterDeptID).FirstOrDefault();
            obj.RegionID =(int) sub.RegionsID;
            obj.DeptID =(int) sub.DeptID;
            return View(obj);
        }
        [System.Web.Http.Authorize]
        public ActionResult ComplianceReports()
        {
            FOSDataModel db = new FOSDataModel();
            var approvel = db.Approvel.Where(u => u.LeadApprovel==false && u.AdminApprovel == false).ToList();
            QAData obj = new QAData();
            obj.Audit = ManageAdmin.GetAuditForGrid();
            foreach (var author in approvel)
            {
                obj.Audit.RemoveAll(x => x.audit_id == author.AuditID);
            }
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            if (System.Web.HttpContext.Current.Session["UserID"] == null)
            {
                Response.Redirect("~/Department/Login");
            }
            obj.CenterDeptID = (int)Session["UserID"];
            SubDepartment sub = db.SubDepartments.Where(u => u.ID == obj.CenterDeptID).FirstOrDefault();
            obj.RegionID = (int)sub.RegionsID;
            obj.DeptID = (int)sub.DeptID;
            return View(obj);
        }
        [System.Web.Http.Authorize]
        public void GetComplianceReport(int CategoryID, int CenterID, int audit_id, int RegionID)
        {
            FOSDataModel db = new FOSDataModel();
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();
            try
            {
                GetCLRList1_1_Result obj = new GetCLRList1_1_Result();
                GetSummaryCLRList1_1_Result obj2 = new GetSummaryCLRList1_1_Result();
                List<GetSummaryCLRList1_1_Result> dtsource = new List<GetSummaryCLRList1_1_Result>();
                dtsource = ManageAdmin.GetSummaryCLRForGrid1(CategoryID, CenterID, audit_id);
                dtsource = dtsource.Where(a => a.RatedMarks == 1 || a.RatedMarks == 2).ToList();
                ReportViewer1.ReportPath = Server.MapPath("~/Views/Department/Compliance.rdlc");
                ReportViewer1.EnableExternalImages = true;
                ReportDataSource dt2 = new ReportDataSource("DataSet1", dtsource);
                ReportViewer1.DataSources.Clear();

                // <reportParameter>
                ReportParameterCollection ReportParamMeters = new ReportParameterCollection();
                Audit audit = new Audit();
                Regions region = new Regions();
                audit = db.Audit.Where(u => u.audit_id == audit_id).FirstOrDefault();
                region = db.Regions.Where(u => u.ID == RegionID).FirstOrDefault();
                ReportParamMeters.Add(new ReportParameter("AuditYear", audit.audit_year.ToString()));
                ReportParamMeters.Add(new ReportParameter("Region", region.Name.ToString()));
                ReportViewer1.SetParameters(ReportParamMeters);
                //</reportParameter>

                ReportViewer1.DataSources.Add(dt2);
                ReportViewer1.Refresh();



                Warning[] warnings;
                string[] streamIds;
                string contentType;
                string encoding;
                string extension;
                string filenameToSave = "AssessmentReport" + DateTime.Now + ".xlsx";
                string renderFormat = (filenameToSave.EndsWith(".xlsx") ? "EXCELOPENXML" : "Excel");

                byte[] bytes = ReportViewer1.Render(renderFormat, null, out contentType, out encoding, out extension, out streamIds, out warnings);

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;filename=AssessmentReport" + DateTime.Now + ".xlsx");
                Response.BinaryWrite(bytes);
                Response.Flush();

                Response.End();
                //return Json(0);
            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }
            //return Json(0);
        }
        [System.Web.Http.Authorize]
        public ActionResult UploadCompliance()
        {
            FOSDataModel db = new FOSDataModel();
            var approvel = db.Approvel.Where(u => u.LeadApprovel == false && u.AdminApprovel == false).ToList();
            QAData obj = new QAData();
            obj.Audit = ManageAdmin.GetAuditForGrid();
            foreach (var author in approvel)
            {
                obj.Audit.RemoveAll(x => x.audit_id == author.AuditID);
            }
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            if (System.Web.HttpContext.Current.Session["UserID"] == null)
            {
                Response.Redirect("~/Department/Login");
            }
            obj.CenterDeptID = (int)Session["UserID"];
            SubDepartment sub = db.SubDepartments.Where(u => u.ID == obj.CenterDeptID).FirstOrDefault();
            obj.DeptID = (int)sub.DeptID;
            return View(obj);
        }

        [HttpPost]
        public ActionResult UploadCompliance(QAData mdl, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                FOSDataModel db = new FOSDataModel();
                ComplianceReport obj = new ComplianceReport();
                var allowedextention = new[] { ".jpg", ".Jpg", ".jpeg", ".png", ".doc", ".pdf", ".docx", ".xlsx", ".xls",".txt" };
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        if (!Directory.Exists(Server.MapPath("~/Uploads")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Uploads"));
                        }
                        var ext = Path.GetExtension(file.FileName);
                        if (allowedextention.Contains(ext))
                        {
                            var path = "~/Uploads/File" + $@"{Guid.NewGuid()}" + ext;
                            file.SaveAs(Server.MapPath(path));
                            obj.Path = path;
                            //Replace("~/", "/../")
                            obj.Date = DateTime.Today;
                            obj.AuditID = mdl.audit_id;
                            obj.CenterID = mdl.CenterDeptID;
                            db.ComplianceReport.Add(obj);
                            db.SaveChanges();
                        }
                    }
                }          
                    return RedirectToAction("UploadCompliance");

            }
            catch (Exception)
            {

                throw;
            }

        }
        [System.Web.Http.Authorize]
        public ActionResult DownloadCompliance()
        {
            QAData obj = new QAData();
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            obj.Audit = ManageAdmin.GetAuditForGrid();
            obj.RegionsList = ManageAdmin.GetAllRegions();
            List<SubDepartmentData> DeptList = ManageAdmin.GetAllSubDepartment();
            obj.SubDept = DeptList;
            List<CenterDepartmentData> CenterDept = ManageAdmin.GetAllCenterDepartment();
            obj.CenterDept = CenterDept;
            return View(obj);
        }
        public ActionResult Download(string file)
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + file;
            string contentType = MimeMapping.GetMimeMapping(filepath);
            byte[] filedata = System.IO.File.ReadAllBytes(Server.MapPath(file));
            return File(filedata, contentType);
        }
        public ActionResult GetCompliance(int CenterID, int audit_id)
        {
            QAData obj = new QAData();
            FOSDataModel db = new FOSDataModel();
            obj.ComplianceReports = new List<ComplianceReport>();
            obj.ComplianceReports  = db.ComplianceReport.Where(a => a.CenterID == CenterID && a.AuditID == audit_id).ToList();
            return PartialView("~/Views/Shared/_ComplianceReport.cshtml", obj);
        }
    }
}
