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
using Org.BouncyCastle.Ocsp;
using static iTextSharp.tool.xml.html.HTML;
using System.Drawing;

namespace FOS.Web.UI.Controllers
{
    public class DepartmentController : Controller
    {
        [System.Web.Http.Authorize]
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.Session["UserID"] == null)
            {
                Response.Redirect("~/Department/Login");
            }
            int centerID = (int)Session["UserID"];
            FOSDataModel db = new FOSDataModel();
            QAData obj = new QAData();
            obj.Audit = new List<Audit>();
            List<Audit> AuditList = ManageAdmin.GetAuditForGrid();
            foreach(var audit in AuditList)
            {
                var approvel = db.Approvel.Where(u => u.LeadApprovel == true && u.AdminApprovel == true && u.CenterID == centerID && u.AuditID==audit.audit_id).FirstOrDefault();
                if (approvel != null)
                    obj.Audit.Add(audit);
            }
            
            //foreach (var author in approvel)
            //{
            //    obj.Audit.RemoveAll(x => x.audit_id == author.AuditID);
            //}
            
            obj.audit_id = obj.Audit.Count>0? obj.Audit.Where(x => x.is_active == true).FirstOrDefault().audit_id:0;
         
            obj.SubDeptID = centerID;
            SubDepartment sub = db.SubDepartments.Where(u => u.ID == obj.SubDeptID).FirstOrDefault();
            List<CenterDepartment> centers = db.CenterDepartments.Where(u => u.CenterID == obj.SubDeptID).ToList();
            obj.CenterDept = new List<CenterDepartmentData>();
            if(obj.audit_id!=0)
            {
                foreach (var cent in centers)
                {

                    CenterDepartmentData data = new CenterDepartmentData();
                    data.Name = cent.Name;
                    data.ID = cent.ID;
                    obj.CenterDept.Add(data);
                }
            }
           
            obj.DeptID = (int)sub.DeptID;
            return View(obj);
        }

        public string GetQAList(int CategoryID, int CenterID, int DeptID, int audit_id)
        {
            FOSDataModel db = new FOSDataModel();
            var obb = db.ComplianceRatedNumber.Where(a => a.AuditID == audit_id && a.CenterID == CenterID && a.DeptID == DeptID).FirstOrDefault();           
            string Res = "";
            if(obb!=null)
            {
                Res="You already submit thier response please contact to Admin";
                return Res;
            }
            List<RatingData> questions = ManageAdmin.GetQList(CategoryID, CenterID, DeptID, audit_id);
            int TotalNumber = 0;
            Session["TotalMarks"] = TotalNumber;
            int ObtainedMarks = 0;
            int ObtainedMarksP = 0;
            int NA = 0;
            if (Session["ObtainedMarks"] != null)
            {
                ObtainedMarks = Convert.ToInt32(Session["ObtainedMarks"]);
            }
            if (Session["OMP"] != null)
            {
                ObtainedMarksP = Convert.ToInt32(Session["OMP"]);
            }
            string OM = Convert.ToString(ObtainedMarks);
            string TM = Convert.ToString(TotalNumber);
            string OMP = Convert.ToString(ObtainedMarksP);
            if (questions.Count > 0)
            {
                Res = "<div class='container'>";
                int sr = 0;
                foreach (RatingData d in questions)
                {
                  
                  sr = sr + 1;
                    Res += "<div class='row'> <div class='col-md-2 col-sm-4'> <input type='checkbox' checked name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' /> &nbsp;&nbsp; " + sr + " &nbsp;&nbsp; " +
                        "<input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'> </div><label style='display:unset!important;color:black'  rows='2' cols='70' readonly>" + d.RName + "</label></div>" +
                           //Question.1
                           "<div class='row'><div class='col-md-2 col-sm-4'>" +
                           "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='0' />" +
                           "<label for='action" + d.RID + "'>Fully Met</label> </div>" +
                           //Question.2

                           "<div class='col-md-2 col-sm-4'>" +
                           "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='1' /> " +
                            "<label for='action" + d.RID + "'>Partially Met</label> </div>" +
                           //Question.3
                          
                           "<div class='col-md-2 col-sm-4'>" +
                           "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='2' />" +
                           "<label for='action" + d.RID + "'>Not Met</label> </div>" +
                           //Question.4
                           "<div class='col-md-2 col-sm-4'>" +
                           "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "' value='-1' />" +
                            "<label for='action" + d.RID + "'>NA</label> </div>" +
                           //TextArea
                           "<div class='col-md-3'>" +
                           "<textarea  name='remarks" + d.RID + "' class='form-control'></textarea> " +
                           "<label for='remarks" + d.RID + "'>Remarks</label> </div>" +
                            //Image Upload
                            "<div class='col-md-3'>" +
                           "<input name='UploadedPicture" + d.RID + "' data-id='" + d.RID + "' type='file' accept='image/*;capture=camera' class='form-control' onchange='loadImage(this)' >" +
                           "</div><div class='col-md-2'><img id='outputImage" + d.RID + "'  width='80' height='80'  />  " +
                            "<input type='hidden' name='ImagePath" + d.RID + "'/> " +
                            "<input type='hidden' name='ImageName" + d.RID + "'/> </div>" +
                           "</div>";
                }
                Res += "</div>";
            }
            return Res;
        }

        [HttpPost]
        public JsonResult AddRatedNumber(List<ActionPage> PageActions)
        {
            CalculateMark cm = new CalculateMark();
            FOSDataModel db = new FOSDataModel();
            int OM = 0;
            int TM = 0;
            int OMP = 0;
            int NA = 0;
            try
            {
                foreach (var d in PageActions)
                {
                    if (d.RatedValue == 0 && d.CategoryID == 0 && d.CenterID == 0 && d.DeptID == 0 && d.QRID == 0)
                    {
                        //return  ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Inserted Successfully')", true);
                        continue;
                    }
                    else
                    {
                        ComplianceRatedNumber obj = new ComplianceRatedNumber();
                        obj.ID = db.ComplianceRatedNumber.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault() + 1;
                        obj.AuditID = d.audit_id;
                        obj.CategoryID = d.CategoryID;
                        obj.CenterID = d.CenterID;
                        obj.DeptID = d.DeptID;
                        obj.QuestionID = d.QRID;
                        obj.RatedValue = d.RatedValue;
                        obj.remarks = d.remarks;
                        if (d.RatedValue == -1)
                        {
                            NA = NA + 1;
                        }
                        if (d.RatedValue == 0)
                        {
                            TM = TM + 1;
                        }
                        if ((d.RatedValue == 1))
                        {
                            OM = OM + 1;
                        }

                        if ((d.RatedValue == 2))
                        {
                            OMP = OMP + 1;
                        }
                        obj.OnCreated = DateTime.Now;
                        obj.IsActive = true;
                        obj.IsDeleted = false;
                        // upload image
                        if (!string.IsNullOrEmpty(d.ImageName))
                        {
                            if (!Directory.Exists(Server.MapPath("~/Images/QuestionImages")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/Images/QuestionImages"));
                            }
                            string ext = Path.GetExtension(d.ImageName);
                            string FileName = Guid.NewGuid() + ext;
                            string path = Path.Combine(Server.MapPath("~/Images/QuestionImages"), FileName);
                            if (!(System.IO.File.Exists(path)))
                            {
                                byte[] imageBytes = Convert.FromBase64String(d.ImagePath.Replace(FOS.Web.UI.DAL.Common.GetFileType(d.ImageName), ""));
                                System.IO.File.WriteAllBytes(path, imageBytes);
                                obj.ImagePath = FileName;
                            }
                        }
                        db.ComplianceRatedNumber.Add(obj);
                        db.SaveChanges();
                        d.Status = true;
                    }
                }

                cm.OM = OM;
                cm.OMP = OMP;
                cm.TM = TM;
                cm.NA = NA;
                Session["TotalMarks"] = null;
                PageActions.Clear();
                return Json(cm);
            }
            catch (Exception e)
            {

                throw e;
            }
           
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
            int centerID = (int)Session["UserID"];
            FOSDataModel db = new FOSDataModel();
            QAData obj = new QAData();
            obj.Audit = new List<Audit>();
            List<Audit> AuditList = ManageAdmin.GetAuditForGrid();
            foreach (var audit in AuditList)
            {
                var approvel = db.Approvel.Where(u => u.LeadApprovel == true && u.AdminApprovel == true && u.CenterID == centerID && u.AuditID == audit.audit_id).FirstOrDefault();
                if (approvel != null)
                    obj.Audit.Add(audit);
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
        public ActionResult ComplianceReports()
        {
            FOSDataModel db = new FOSDataModel();
            var approvel = db.Approvel.Where(u => u.LeadApprovel == false && u.AdminApprovel == false && u.CenterID == (int)Session["UserID"] ).ToList();
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
           
        }
        [System.Web.Http.Authorize]
        public ActionResult UploadCompliance()
        {
            int centerID = (int)Session["UserID"];

            FOSDataModel db = new FOSDataModel();
            QAData obj = new QAData();
            List<Audit> AuditList = ManageAdmin.GetAuditForGrid();
            foreach (var audit in AuditList)
            {
                var approvel = db.Approvel.Where(u => u.LeadApprovel == true && u.AdminApprovel == true && u.CenterID == centerID && u.AuditID == audit.audit_id).FirstOrDefault();
                if (approvel != null)
                    obj.Audit.Add(audit);
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
                var allowedextention = new[] { ".jpg", ".Jpg", ".jpeg", ".png", ".doc", ".pdf", ".docx", ".xlsx", ".xls", ".txt" };
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



        #region download compliance
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
        //download compliance report 
        public void DownlCompliance(int CategoryID, int CenterID, int audit_id, int RegionID)
        {
            FOSDataModel db = new FOSDataModel();
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();
            try
            {
                GetCLRList1_1_Result obj = new GetCLRList1_1_Result();
                GetSummaryCLRList1_1_Result obj2 = new GetSummaryCLRList1_1_Result();
                List<GetSummaryCLRList1_1_Result> dtsource = new List<GetSummaryCLRList1_1_Result>();
                dtsource = ManageAdmin.GetComplianceSummaryCLRForGrid1(CategoryID, CenterID, audit_id);
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

                byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;filename=AssessmentReport" + DateTime.Now + ".Pdf");
                Response.BinaryWrite(bytes);
                Response.Flush();

                Response.End();
                //return Json(0);
            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }
        }
        //Show all uploaded compliance Report
        public ActionResult GetCompliance(int CenterID, int audit_id)
        {
            QAData obj = new QAData();
            FOSDataModel db = new FOSDataModel();
            obj.ComplianceReports = new List<ComplianceReport>();
            obj.ComplianceReports = db.ComplianceReport.Where(a => a.CenterID == CenterID && a.AuditID == audit_id).ToList();
            return PartialView("~/Views/Shared/_ComplianceReport.cshtml", obj);
        }
        //Download uploaded compliance Report
        public ActionResult Download(string file)
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + file;
            string contentType = MimeMapping.GetMimeMapping(filepath);
            byte[] filedata = System.IO.File.ReadAllBytes(Server.MapPath(file));
            return File(filedata, contentType);
        }
      
        
        #endregion
    }
}
