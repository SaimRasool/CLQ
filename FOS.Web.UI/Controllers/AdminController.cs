using FOS.Setup;
using FOS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FOS.DataLayer;
using FluentValidation.Results;
using FOS.Web.UI.Models;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.DataSets;
using CrystalDecisions.CrystalReports.Engine;
using Shared.Diagnostics.Logging;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Reporting.WebForms;
using System.Text;
using System.Transactions;
using System.Web.UI;
using ASPNET_MVC_Samples.Models;
using Newtonsoft.Json;
using FOS.Web.UI.DAL;
using System.Data.OleDb;

namespace FOS.Web.UI.Controllers
{
    public class AdminController : Controller
    {
        OleDbConnection Econ;
        private void ExcelConn(string filepath)
        {
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", filepath);
            Econ = new OleDbConnection(constr);
        }

        #region Region
        [CustomAuthorize]
        public ActionResult Region()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateRegion([Bind(Exclude = "TID")] RegionData newRegion)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRegion != null)
                {

                    if (boolFlag)
                    {
                        int Response = ManageAdmin.AddUpdateRegion(newRegion);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }
        //Get All Region Method...
        public JsonResult RegionDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<RegionData>();

                dtsource = ManageAdmin.GetRegionForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<RegionData> data = ManageAdmin.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageAdmin.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<RegionData> result = new DTResult<RegionData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        //Delete Department Method...
        public int DeleteRegion(int RegionID)
        {
            return FOS.Setup.ManageAdmin.DeleteRegion(RegionID);
        }

        #endregion

        #region Category
        [CustomAuthorize]
        public ActionResult Category()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateCategory([Bind(Exclude = "TID")] AdminData newDepartment)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newDepartment != null)
                {

                    if (boolFlag)
                    {
                        int Response = ManageAdmin.AddUpdateCategory(newDepartment);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }
        //Get All Region Method...
        public JsonResult CategoryDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<AdminData>();

                dtsource = ManageAdmin.GetCategoryForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<AdminData> data = ManageAdmin.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageAdmin.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<AdminData> result = new DTResult<AdminData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        //Delete Department Method...
        public int DeleteCategory(int DepartmentID)
        {
            return FOS.Setup.ManageAdmin.DeleteCategory(DepartmentID);
        }

        #endregion

        #region Center
        public ActionResult Center()
        {
            SubDepartmentData data = new SubDepartmentData();
            data.Dept = ManageAdmin.GetAllDepartment();
            data.RegionsList = ManageAdmin.GetAllRegions();
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateSubDepartment([Bind(Exclude = "TID")] SubDepartmentData newSubDepartment)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newSubDepartment != null)
                {
                    if (newSubDepartment.ID == 0)
                    {
                        FOSDataModel db = new FOSDataModel();
                        var checkData = db.SubDepartments.Where(u => u.Name == newSubDepartment.SubDepartment && u.DeptID == newSubDepartment.DeptID).FirstOrDefault();

                        if (checkData == null)
                        {
                            boolFlag = true;
                        }
                        else
                        {
                            boolFlag = false;
                        }
                    }

                    if (boolFlag == true)
                    {
                        int Response = ManageAdmin.AddUpdateSubDepartment(newSubDepartment);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        return Content("2");
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }
        //Get All Region Method...
        public JsonResult SubDepartmentDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<SubDepartmentData>();

                dtsource = ManageAdmin.GetSubDepartmentForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SubDepartmentData> data = ManageAdmin.GetResultSubDepartment(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageAdmin.CountSubDepartment(param.Search.Value, dtsource, columnSearch);
                DTResult<SubDepartmentData> result = new DTResult<SubDepartmentData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        //Delete Department Method...
        public int DeleteSubDepartment(int ID)
        {
            return FOS.Setup.ManageAdmin.DeleteSubDepartment(ID);
        }
        #endregion

        #region Department
        [CustomAuthorize]
        public ActionResult Department()
        {
            DepartmentData obj = new DepartmentData();
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Category = AllDept;
            List<SubDepartmentData> CenterList = ManageAdmin.GetAllSubDepartment();
            obj.Center = CenterList;

            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateDepartment([Bind(Exclude = "TID")] DepartmentData newDepartment)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newDepartment != null)
                {
                    if (boolFlag)
                    {
                        int Response = ManageAdmin.AddUpdateDepartment(newDepartment);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }
        //Get All Region Method...
        public JsonResult DepartmentDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<DepartmentData>();

                dtsource = ManageAdmin.GetDepartmentForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<DepartmentData> data = ManageAdmin.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageAdmin.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<DepartmentData> result = new DTResult<DepartmentData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        //Delete Department Method...
        public int DeleteDepartment(int DepartmentID)
        {
            return FOS.Setup.ManageAdmin.DeleteDepartment(DepartmentID);
        }

        #endregion
      
        #region QA
        public ActionResult QA()
        {
            //FOSDataModel db = new FOSDataModel();
            //string[] dummy = {"a","b","c","d"};
            //IEnumerable<string> Letters = dummy;
            //IQueryable<String> Ltr = db.TestingTables.Select(x=> x.Letters);
            //Ltr = Ltr.Take<String>(3);
            //TestingTable tt;
            //foreach(var result in Ltr)
            //{
            //    tt = new TestingTable();
            //    //tt.ID = db.TestingTables.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault() + 1;
            //    tt.Letters = result.Trim();
            //    db.TestingTables.Add(tt);             
            //}
            //db.SaveChanges();

            QAData obj = new QAData();
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            obj.RegionsList = ManageAdmin.GetAllRegions();
            List<SubDepartmentData> DeptList = ManageAdmin.GetAllSubDepartment();
            obj.SubDept = DeptList;
            List<CenterDepartmentData> CenterDept = ManageAdmin.GetAllCenterDepartment();
            obj.CenterDept = CenterDept;
            return View(obj);
        }
        public JsonResult GetSubDepartmentListByDeptWise(int DeptID)
        {
            var result = ManageAdmin.GetSubDepartmentDepartmentWise(DeptID);
            return Json(result);
        }
        public JsonResult GetCenterDepartmentListByDeptWise(int CategoryID)
        {
            int EmpID = Convert.ToInt32(Session["UserID"]);
            var result = ManageAdmin.GetCenterDepartmentDepartmentWise(CategoryID, EmpID);
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateQA([Bind(Exclude = "TID")] QAData newQA)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newQA != null)
                {
                    if (newQA.QAID == 0)
                    {
                        FOSDataModel db = new FOSDataModel();
                        var checkData = db.QAs.Where(u => u.DeptID == newQA.SubDeptID && u.CategoryID == newQA.DeptID && u.Question == newQA.QuestionName).FirstOrDefault();

                        if (checkData == null)
                        {
                            boolFlag = true;
                        }
                        else
                        {
                            boolFlag = false;
                        }
                    }

                    if (boolFlag == true)
                    {

                        int Response = ManageAdmin.AddUpdateQA(newQA);

                        if (Response == 1)
                        {
                            Content("<script language='javascript' type='text/javascript'>alert('Save Successfully!');</script>");
                            return RedirectToAction("QA", "Admin");

                        }
                        else if (Response == 2)
                        {
                            return RedirectToAction("QA", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("QA", "Admin");
                        }
                    }
                    else
                    {
                        return RedirectToAction("QA", "Admin");
                    }
                }

                return RedirectToAction("QA", "Admin");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }
        public JsonResult QADataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<QAData>();

                dtsource = ManageAdmin.GetQAForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<QAData> data = ManageAdmin.GetResultQA(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageAdmin.CountQA(param.Search.Value, dtsource, columnSearch);
                DTResult<QAData> result = new DTResult<QAData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public int DeleteQA(int ID)
        {
            return FOS.Setup.ManageAdmin.DeleteQA(ID);
        }
        public ActionResult ImportsQA()
        {
            QAData obj = new QAData();
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            obj.RegionsList = ManageAdmin.GetAllRegions();
            List<SubDepartmentData> DeptList = ManageAdmin.GetAllSubDepartment();
            obj.SubDept = DeptList;
            List<CenterDepartmentData> CenterDept = ManageAdmin.GetAllCenterDepartment();
            obj.CenterDept = CenterDept;
            return View(obj);
        }
        [HttpPost]
        public ActionResult ImportsQA(HttpPostedFileBase file, QAData newQA)
        {
            //upload code
            var allowedextention = new[] { ".xlsx", ".xls" };
            if (file != null)
            {
                var ext = Path.GetExtension(file.FileName);
                if (allowedextention.Contains(ext))
                {
                    string folderPath = "~/excelfolder";
                    string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    if (!Directory.Exists(Server.MapPath(folderPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(folderPath));
                    }
                    //string filepath = "/excelfolder/" + filename;
                    file.SaveAs(Path.Combine(Server.MapPath(folderPath), filename));
                    if (InsertExceldata(filename, newQA))
                        return RedirectToAction("ImportsQA");
                    else
                    {
                        return Json(new { success = false, responseText = "Please Upload a Valid Excel file. Any Cell Value or Cloumn Name is Invalid" }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new { success = false, responseText = "Please Upload Excel file This is not a excel file" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(new { success = false, responseText = "Please Upload a file" }, JsonRequestBehavior.AllowGet);
        }
        private bool InsertExceldata(string filename, QAData newQA)
        {
            try
            {
                FOSDataModel db = new FOSDataModel();
                string fullpath = Server.MapPath("~/excelfolder/") + filename;
                ExcelConn(fullpath);
                Econ.Open();
                DataTable dtSchema = Econ.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });
                if (dtSchema == null || dtSchema.Rows.Count < 1)
                {
                    throw new Exception("Error: Could not determine the name of the first worksheet.");
                }
                string query = string.Format("Select * from [{0}]", dtSchema.Rows[0].Field<string>("TABLE_NAME"));
                // string query = string.Format("Select * from [{0}]", dtSchema.Rows[0]["TABLE_NAME"].ToString());
                DataSet ds = new DataSet();
                OleDbDataAdapter oda = new OleDbDataAdapter(query, Econ);
                Econ.Close();
                oda.Fill(ds);
                DataTable dt = ds.Tables[0];
                // dt.Rows.Remove(dt.Rows[0]);//remove 2nd title row
                //remove empty rows
                dt = dt.Rows.Cast<DataRow>().Where(row => !(row.ItemArray[0] is System.DBNull)).CopyToDataTable();
                //remove empty rows
                foreach (DataRow oRow in dt.Rows)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        using (FOSDataModel dbContext = new FOSDataModel())
                        {
                            var checkData = db.QAs.Where(u => u.DeptID == newQA.SubDeptID && u.CategoryID == newQA.DeptID && u.Question == newQA.QuestionName).FirstOrDefault();

                            if (checkData != null)
                            {
                                QA QAObj = new QA();
                                QAObj.ID = dbContext.QAs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                QAObj.CategoryID = newQA.DeptID;
                                QAObj.CenterID = newQA.SubDeptID;
                                QAObj.DeptID = newQA.CenterDeptID;
                                QAObj.Question = oRow.ItemArray[0].ToString();
                                QAObj.QuestionHint = oRow.ItemArray[1].ToString();
                                QAObj.IsActive = true;
                                QAObj.IsDeleted = false;
                                QAObj.OnCreated = DateTime.Now;
                                dbContext.QAs.Add(QAObj);
                                dbContext.SaveChanges();
                                scope.Complete();
                            }
                        }
                    }
                }
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Rating
        //[CustomAuthorize]
        public ActionResult Rating()
        {
            Session["ObtainedMarks"] = null;
            Session["TotalNumber"] = null;
            QAData obj = new QAData();
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            obj.RegionsList = ManageAdmin.GetAllRegions();
            List<SubDepartmentData> DeptList = ManageAdmin.GetAllSubDepartment();
            obj.SubDept = DeptList;
            List<CenterDepartmentData> CenterDept = ManageAdmin.GetAllCenterDepartment();
            obj.CenterDept = CenterDept;
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                obj.AuditYear = dbContext.Audit.Where(u => u.audit_year == DateTime.Today.Year && u.is_active == true).Select(u => u.audit_year).FirstOrDefault();
                obj.audit_id = dbContext.Audit.Where(u => u.audit_year == DateTime.Today.Year && u.is_active == true).Select(u => u.audit_id).FirstOrDefault();

            }
            return View(obj);
        }
        public string GetQAList(int CategoryID, int CenterID, int DeptID, int audit_id)
        {
            string Res = "";
            List<RatingData> questions = ManageAdmin.GetQList(CategoryID, CenterID, DeptID, audit_id);
            //int TotalNumber = questions.Count * 10;
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
                Res = "";
                int sr = 0;
                foreach (RatingData d in questions)
                {
                    sr = sr + 1;
                    Res += "<div class='container'>" +
                          "<div class='row' style='margin-left:30px'>" +
                                "<div class='col-lg-12 col-md-12 col-sm-12'><input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                                "<input type='checkbox' checked name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin-left:38px' />" +
                                "&nbsp;&nbsp; " + sr + " &nbsp;&nbsp;" +
                                "<button class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' ><i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i></button>" +
                                "&nbsp;&nbsp;&nbsp;<label style='display:unset!important;color:black'  rows='2' cols='70' readonly>" + d.RName + "</label></div>" +
                          //"<div class='col-lg-2 col-md-2 col-sm-1'></div>" +
                          //"<div class='col-lg-8 col-md-8 col-sm-1'><label  rows='2' cols='50'></label></div>"+
                          "</div>" +
                          "<div class='row radiobutton' style='margin-left:76px'>" +
                           //Question.1
                           "<div class='col-lg-1 col-md-1 col-sm-2'><p>Fully Met</p></div>" +
                           "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                           "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='0' />  </div>" +
                           //Question.2
                           "<div class='col-lg-1 col-md-1 col-sm-2'><p>Partially Met</p></div>" +
                           "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                           "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='1' />  </div>" +
                           //Question.3
                           "<div class='col-lg-1 col-md-1 col-sm-2'><p>Not Met</p></div>" +
                           "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                           "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='2' />  </div>" +
                           //Question.4
                           "<div class='col-lg-1 col-md-1 col-sm-2'><p>NA</p></div>" +
                           "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                           "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "' value='-1' />  </div>" +
                           //TextArea
                           "<div class='col-lg-1 col-md-1 col-sm-2'><p>Remarks</p></div>" +
                           "<div class='col-lg-1 col-md-1 col-sm-2'>" +

                           "<textarea  name='remarks" + d.RID + "' class='form-control'></textarea>  </div>" +
                           //Image Upload
                           "<div class='col-lg-2 col-md-2 col-sm-3'>" + " <input type='button' id='btnCapture' data-id='" + d.RID + "' value='Capture'/>" +
                           "<input name='UploadedPicture" + d.RID + "' data-id='" + d.RID + "' type='file' accept='image/*;capture=camera' class='form-control' onchange='loadImage(this)' >" +
                           "<img id='outputImage" + d.RID + "'  width='80' height='80'  />  </div>" +
                            "<input type='hidden' name='ImagePath" + d.RID + "'/> " +
                            "<input type='hidden' name='ImageName" + d.RID + "'/> " +
                           "</div>";
                    //Res += "<input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                    //   "<input type='checkbox' name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin: 12px  10px 0 0px;' />" +
                    //   "<li class='page_border_role'><button style='height:30px' type='button'" + d.RID + " class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' >" +
                    //   "<i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i>" +
                    //   "</button><label style='margin-left:5px;' rows='2' cols='60' readonly>" + d.RName + "</label></br>" +
                    //   "<span style=float:right;font-size: 12px;'><input name='action" + d.RID + "' type='radio' class='cbAction' id='One" + d.RID + "' value='10' data-id='" + d.RID + "' data-one='" + d.One + "' style='margin: -2px 2px 0 0px;' /> 10</span>" +
                    //   "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' value='9' data-two='" + d.Two + "' style='margin: -2px 2px 0 0px;' /> 9</span>" +
                    //   "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "'class='cbAction' data-id='" + d.RID + "' data-three='" + d.Three + "' value='8' style='margin: -2px 2px 0 0px;' /> 8</span>" +
                    //   "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-four='" + d.Four + "' value='7' style='margin: -2px 2px 0 0px;' /> 7</span>" +
                    //   "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-five='" + d.Five + "' value='6' style='margin: -2px 2px 0 0px;' /> 6</span>" +
                    //   "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-six='" + d.Six + "' value='5' style='margin: -2px 2px 0 0px;' /> 5</span>" +
                    //   "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-seven='" + d.Seven + "' value='4' style='margin: -2px 2px 0 0px;' /> 4</span>" +
                    //   "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-eight='" + d.Eight + "' value='3' style='margin: -2px 2px 0 0px;' /> 3</span>" +
                    //   "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-nine='" + d.Nine + "' value='2' style='margin: -2px 2px 0 0px;' /> 2</span>" +
                    //   "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Ten + "' value='1' style='margin: -2px 2px 0 0px;' /> 1</span>" +
                    //   "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='0' style='margin: -2px 2px 0 0px;' /> 0</span>" +
                    //   "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "' value='-1' style='margin: -2px 2px 0 0px;' /> Not Applicable</span></li>";
                }
                Res += "<label style='font-weight: 600;' id='TotalM' color:#ce0019;'>Fully Met:  " + TotalNumber + "</label><label id='ObtainedMarks'>Partially Met: " + OM + "</label><label id='ObtainedMarksP'>Not Met: " + OMP + "</label><label id='NAQ'>Not Applicable: " + NA + "</label>";
            }
            return Res;
        }
        public JsonResult GetQHint(int ID)
        {
            FOSDataModel db = new FOSDataModel();
            QAData obj = new QAData();
            var hint = db.QAs.Where(x => x.ID == ID).Select(x => new QAData
            {
                QuestionName = x.Question,
                QuestionHint = x.QuestionHint
            }).FirstOrDefault();
            return Json(hint);
        }
        public JsonResult GetPagesActionData()
        {
            List<ActionPage> obj = new List<ActionPage>();
            //obj.Add(new ActionPage { Page = 5, Update = true , Delete = true , Read = true , Write = true});
            return Json(obj);
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
            foreach (var d in PageActions)
            {
                if (d.RatedValue == 0 && d.CategoryID == 0 && d.CenterID == 0 && d.DeptID == 0 && d.QRID == 0)
                {
                    //return  ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Inserted Successfully')", true);
                    continue;
                }
                else
                {
                    RatedNumber obj = new RatedNumber();
                    obj.ID = db.RatedNumbers.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault() + 1;
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
                    db.RatedNumbers.Add(obj);
                    db.SaveChanges();
                    d.Status = true;
                }
            }
            //int TM = Convert.ToInt32(Session["TotalMarks"]);
            //int TTM = 0;
            //if(NA < 0)
            //{
            //    int MTM = NA * 10;
            //    TTM = TM + MTM;
            //}
            //else
            //{
            //    TTM = TM;
            //}
            //double OMP = (double)OM/TTM;
            //double OMPer = OMP * 100;
            cm.OM = OM;
            cm.OMP = OMP;
            cm.TM = TM;
            cm.NA = NA;
            Session["TotalMarks"] = null;
            PageActions.Clear();
            return Json(cm);
        }

        #endregion 

        #region Request To Edit Checklist
        public ActionResult SendRequest()
        {
            Session["ObtainedMarks"] = null;
            Session["TotalNumber"] = null;
            QAData obj = new QAData();
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            obj.RegionsList = ManageAdmin.GetAllRegions();
            List<SubDepartmentData> DeptList = ManageAdmin.GetAllSubDepartment();
            obj.SubDept = DeptList;
            List<CenterDepartmentData> CenterDept = ManageAdmin.GetAllCenterDepartment();
            obj.CenterDept = CenterDept;
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                obj.AuditYear = dbContext.Audit.Where(u => u.audit_year == DateTime.Today.Year && u.is_active == true).Select(u => u.audit_year).FirstOrDefault();
                obj.audit_id = dbContext.Audit.Where(u => u.audit_year == DateTime.Today.Year && u.is_active == true).Select(u => u.audit_id).FirstOrDefault();

            }
            return View(obj);
        }
        public string GetQAListForUpdateRequest(int CategoryID, int CenterID, int DeptID, int audit_id)
        {

            string Res = "";
            List<RatingData> questions = ManageAdmin.GetQListForUpdateRequest(CategoryID, CenterID, DeptID, audit_id);
            // int TotalNumber = questions.Count * 10;
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
                Res = "";
                int sr = 0;
                foreach (RatingData d in questions)
                {
                    sr = sr + 1;
                    if (d.RatedNumber == 2)
                    {
                        Res += "<div class='container'>" +
                         "<div class='row' style='margin-left:30px'>" +
                               "<div class='col-lg-12 col-md-12 col-sm-12'><input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                               "<input type='checkbox' checked name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin-left:38px' />" +
                               "&nbsp;&nbsp; " + sr + " &nbsp;&nbsp;" +
                               "<button class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' ><i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i></button>" +
                               "&nbsp;&nbsp;&nbsp;<label style='display:unset!important;color:black'  rows='2' cols='70' readonly>" + d.RName + "</label></div>" +
                         //"<div class='col-lg-2 col-md-2 col-sm-1'></div>" +
                         //"<div class='col-lg-8 col-md-8 col-sm-1'><label  rows='2' cols='50'></label></div>"+
                         "</div>" +
                         "<div class='row radiobutton' style='margin-left:76px'>" +
                          //Question.1
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Fully Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='0' />  </div>" +
                          //Question.2
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Partially Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='1' />  </div>" +
                          //Question.3
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Not Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "'checked  value='2' />  </div>" +
                          //Question.4
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>NA</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "' value='-1' />  </div>" +
                          //TextArea
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Remarks</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +

                          "<textarea  name='remarks" + d.RID + "' class='form-control' >" + d.remarks + "</textarea>  </div>" +

                          "</div>";
                    }
                    else if (d.RatedNumber == 1)
                    {
                        Res += "<div class='container'>" +
                         "<div class='row' style='margin-left:30px'>" +
                               "<div class='col-lg-12 col-md-12 col-sm-12'><input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                               "<input type='checkbox' checked name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin-left:38px' />" +
                               "&nbsp;&nbsp; " + sr + " &nbsp;&nbsp;" +
                               "<button class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' ><i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i></button>" +
                               "&nbsp;&nbsp;&nbsp;<label style='display:unset!important;color:black'  rows='2' cols='70' readonly>" + d.RName + "</label></div>" +
                         //"<div class='col-lg-2 col-md-2 col-sm-1'></div>" +
                         //"<div class='col-lg-8 col-md-8 col-sm-1'><label  rows='2' cols='50'></label></div>"+
                         "</div>" +
                         "<div class='row radiobutton' style='margin-left:76px'>" +
                          //Question.1
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Fully Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='0' />  </div>" +
                          //Question.2
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Partially Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' checked value='1' />  </div>" +
                          //Question.3
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Not Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='2' />  </div>" +
                          //Question.4
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>NA</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "' value='-1' />  </div>" +
                          //TextArea
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Remarks</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +

                          "<textarea  name='remarks" + d.RID + "' class='form-control' >" + d.remarks + "</textarea>  </div>" +

                          "</div>";
                    }
                    else if (d.RatedNumber == 0)
                    {
                        Res += "<div class='container'>" +
                         "<div class='row' style='margin-left:30px'>" +
                               "<div class='col-lg-12 col-md-12 col-sm-12'><input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                               "<input type='checkbox' checked name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin-left:38px' />" +
                               "&nbsp;&nbsp; " + sr + " &nbsp;&nbsp;" +
                               "<button class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' ><i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i></button>" +
                               "&nbsp;&nbsp;&nbsp;<label style='display:unset!important;color:black'  rows='2' cols='70' readonly>" + d.RName + "</label></div>" +
                         //"<div class='col-lg-2 col-md-2 col-sm-1'></div>" +
                         //"<div class='col-lg-8 col-md-8 col-sm-1'><label  rows='2' cols='50'></label></div>"+
                         "</div>" +
                         "<div class='row radiobutton' style='margin-left:76px'>" +
                          //Question.1
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Fully Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' checked value='0' />  </div>" +
                          //Question.2
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Partially Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='1' />  </div>" +
                          //Question.3
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Not Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='2' />  </div>" +
                          //Question.4
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>NA</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "' value='-1' />  </div>" +
                          //TextArea
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Remarks</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +

                          "<textarea  name='remarks" + d.RID + "' class='form-control' >" + d.remarks + "</textarea>  </div>" +

                          "</div>";
                    }
                    else if (d.RatedNumber == -1)
                    {
                        Res += "<div class='container'>" +
                         "<div class='row' style='margin-left:30px'>" +
                               "<div class='col-lg-12 col-md-12 col-sm-12'><input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                               "<input type='checkbox' checked name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin-left:38px' />" +
                               "&nbsp;&nbsp; " + sr + " &nbsp;&nbsp;" +
                               "<button class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' ><i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i></button>" +
                               "&nbsp;&nbsp;&nbsp;<label style='display:unset!important;color:black'  rows='2' cols='70' readonly>" + d.RName + "</label></div>" +
                         //"<div class='col-lg-2 col-md-2 col-sm-1'></div>" +
                         //"<div class='col-lg-8 col-md-8 col-sm-1'><label  rows='2' cols='50'></label></div>"+
                         "</div>" +
                         "<div class='row radiobutton' style='margin-left:76px'>" +
                          //Question.1
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Fully Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='0' />  </div>" +
                          //Question.2
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Partially Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='1' />  </div>" +
                          //Question.3
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Not Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='2' />  </div>" +
                          //Question.4
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>NA</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "'checked value='-1' />  </div>" +
                          //TextArea
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Remarks</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +

                          "<textarea  name='remarks" + d.RID + "' class='form-control' >" + d.remarks + "</textarea>  </div>" +

                          "</div>";
                        //    //Res += "<input type='checkbox' name='page' data-id='" + d.PageID + "' id='page' class='page' style='float: left; margin: 12px  10px 0 0px;' /><li class='page_border_role'><input type='text' data-id='" + d.PageID + "' id='menuPage' name='page' value='" + d.ParentPageName + "'/>";    
                        //    Res += "<input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                        //    "<input type='checkbox' name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin: 12px  10px 0 0px;' />" +
                        //    "<li class='page_border_role'><button style='height:30px' type='button'" + d.RID + " class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' >" +
                        //    "<i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i>" +
                        //    "</button><label style='margin-left:5px' rows='2' cols='60' readonly>" + d.RName + "</label>" +
                        //    "<span style=float:right;font-size: 12px;'><input name='action" + d.RID + "' type='radio'  class='cbAction' id='One" + d.RID + "' value='10' data-id='" + d.RID + "' data-one='" + d.One + "' style='margin: -2px 2px 0 0px;' /> 10</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' value='9' data-two='" + d.Two + "' style='margin: -2px 2px 0 0px;' /> 9</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "'class='cbAction' data-id='" + d.RID + "' data-three='" + d.Three + "' value='8' style='margin: -2px 2px 0 0px;' /> 8</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-four='" + d.Four + "' value='7' style='margin: -2px 2px 0 0px;' /> 7</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-five='" + d.Five + "' value='6' style='margin: -2px 2px 0 0px;' /> 6</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-six='" + d.Six + "' value='5' style='margin: -2px 2px 0 0px;' /> 5</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-seven='" + d.Seven + "' value='4' style='margin: -2px 2px 0 0px;' /> 4</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-eight='" + d.Eight + "' value='3' style='margin: -2px 2px 0 0px;' /> 3</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-nine='" + d.Nine + "' value='2' style='margin: -2px 2px 0 0px;' /> 2</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Ten + "' value='1' style='margin: -2px 2px 0 0px;' /> 1</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='0' style='margin: -2px 2px 0 0px;' /> 0</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='checkbox' checked name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "' value='-1' style='margin: -2px 2px 0 0px;' /> Not Applicable</span></li>";
                    }
                }
                Res += "<label style='font-weight: 600;' id='TotalM' color:#ce0019;'>Fully Met:  " + TotalNumber + "</label><label id='ObtainedMarks'>Partially Met: " + OM + "</label><label id='ObtainedMarksP'>Not Met: " + OMP + "</label><label id='NAQ'>Not Applicable: " + NA + "</label>";
            }
            return Res;
        }
        public JsonResult RequestEditRatedNumber(List<ActionPage> PageActions)
        {
            CalculateMark cm = new CalculateMark();
            FOSDataModel db = new FOSDataModel();
            //ImagesTable imageObj = new ImagesTable();
            //Session["ObtainedMarks"] = null;
            //var ImgPath = Convert.ToString(Session["ImagePath"]);
            //var PAction = PageActions.FirstOrDefault();
            //imageObj.CenterID = PAction.CenterID;
            //imageObj.CategoryID = PAction.CategoryID;
            //imageObj.DeptID = PAction.DeptID;
            //imageObj.UserID = Convert.ToInt32(Session["UserID"]);
            //imageObj.ImagePath = ImgPath;
            //imageObj.OnCreated = DateTime.Now;
            //imageObj.IsActive = true;
            //db.ImagesTables.Add(imageObj);
            //db.SaveChanges();


            int OM = 0;
            int TM = 0;
            int OMP = 0;
            int NA = 0;
            foreach (var d in PageActions)
            {
                TempRatedNumber obj = new TempRatedNumber();
                obj.ID = db.TempRatedNumbers.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault() + 1;
                obj.CategoryID = d.CategoryID;
                obj.AuditID = d.audit_id;
                obj.CenterID = d.CenterID;
                obj.DeptID = d.DeptID;
                obj.QuestionID = d.QRID;
                obj.RatedValue = d.RatedValue;
                obj.remarks = d.remarks;
                obj.EmpID = Convert.ToInt32(Session["UserID"]);
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
                //obj.ImagePath = Convert.ToString(Session["ImagePath"]);
                db.TempRatedNumbers.Add(obj);
                db.SaveChanges();
                d.Status = true;
            }

            cm.OM = OM;
            cm.OMP = OMP;
            cm.TM = TM;
            cm.NA = NA;
            Session["TotalMarks"] = null;
            return Json(cm);
        }
        #endregion

        #region Update Rating
        //[CustomAuthorize]
        public ActionResult UpdateRating()
        {
            Session["ObtainedMarks"] = null;
            Session["TotalNumber"] = null;
            QAData obj = new QAData();
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            obj.RegionsList = ManageAdmin.GetAllRegions();
            List<SubDepartmentData> DeptList = ManageAdmin.GetAllSubDepartment();
            obj.SubDept = DeptList;
            List<CenterDepartmentData> CenterDept = ManageAdmin.GetAllCenterDepartment();
            obj.CenterDept = CenterDept;
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                obj.AuditYear = dbContext.Audit.Where(u => u.audit_year == DateTime.Today.Year && u.is_active == true).Select(u => u.audit_year).FirstOrDefault();
                obj.audit_id = dbContext.Audit.Where(u => u.audit_year == DateTime.Today.Year && u.is_active == true).Select(u => u.audit_id).FirstOrDefault();

            }
            return View(obj);
        }
        public string GetQAListForUpdate(int CategoryID, int CenterID, int DeptID, int audit_id)
        {

            string Res = "";
            List<RatingData> questions = ManageAdmin.GetQListForUpdate(CategoryID, CenterID, DeptID, audit_id);
            int TotalNumber = questions.Count * 10;
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
                Res = "";
                int sr = 0;
                foreach (RatingData d in questions)
                {
                    sr = sr + 1;
                    if (d.RatedNumber == 2)
                    {
                        Res += "<div class='container'>" +
                         "<div class='row' style='margin-left:30px'>" +
                               "<div class='col-lg-12 col-md-12 col-sm-12'><input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                               "<input type='checkbox' checked name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin-left:38px' />" +
                               "&nbsp;&nbsp; " + sr + " &nbsp;&nbsp;" +
                               "<button class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' ><i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i></button>" +
                               "&nbsp;&nbsp;&nbsp;<label style='display:unset!important;color:black'  rows='2' cols='70' readonly>" + d.RName + "</label></div>" +
                         //"<div class='col-lg-2 col-md-2 col-sm-1'></div>" +
                         //"<div class='col-lg-8 col-md-8 col-sm-1'><label  rows='2' cols='50'></label></div>"+
                         "</div>" +
                         "<div class='row radiobutton' style='margin-left:76px'>" +
                          //Question.1
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Fully Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='0' />  </div>" +
                          //Question.2
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Partially Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='1' />  </div>" +
                          //Question.3
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Not Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "'checked  value='2' />  </div>" +
                          //Question.4
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>NA</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "' value='-1' />  </div>" +
                          //TextArea
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Remarks</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +

                          "<textarea  name='remarks" + d.RID + "' class='form-control' >" + d.remarks + "</textarea>  </div>" +

                          "</div>";
                    }
                    else if (d.RatedNumber == 1)
                    {
                        Res += "<div class='container'>" +
                         "<div class='row' style='margin-left:30px'>" +
                               "<div class='col-lg-12 col-md-12 col-sm-12'><input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                               "<input type='checkbox' checked name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin-left:38px' />" +
                               "&nbsp;&nbsp; " + sr + " &nbsp;&nbsp;" +
                               "<button class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' ><i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i></button>" +
                               "&nbsp;&nbsp;&nbsp;<label style='display:unset!important;color:black'  rows='2' cols='70' readonly>" + d.RName + "</label></div>" +
                         //"<div class='col-lg-2 col-md-2 col-sm-1'></div>" +
                         //"<div class='col-lg-8 col-md-8 col-sm-1'><label  rows='2' cols='50'></label></div>"+
                         "</div>" +
                         "<div class='row radiobutton' style='margin-left:76px'>" +
                          //Question.1
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Fully Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='0' />  </div>" +
                          //Question.2
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Partially Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' checked value='1' />  </div>" +
                          //Question.3
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Not Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='2' />  </div>" +
                          //Question.4
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>NA</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "' value='-1' />  </div>" +
                          //TextArea
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Remarks</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +

                          "<textarea  name='remarks" + d.RID + "' class='form-control' >" + d.remarks + "</textarea>  </div>" +

                          "</div>";
                    }
                    else if (d.RatedNumber == 0)
                    {
                        Res += "<div class='container'>" +
                         "<div class='row' style='margin-left:30px'>" +
                               "<div class='col-lg-12 col-md-12 col-sm-12'><input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                               "<input type='checkbox' checked name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin-left:38px' />" +
                               "&nbsp;&nbsp; " + sr + " &nbsp;&nbsp;" +
                               "<button class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' ><i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i></button>" +
                               "&nbsp;&nbsp;&nbsp;<label style='display:unset!important;color:black'  rows='2' cols='70' readonly>" + d.RName + "</label></div>" +
                         //"<div class='col-lg-2 col-md-2 col-sm-1'></div>" +
                         //"<div class='col-lg-8 col-md-8 col-sm-1'><label  rows='2' cols='50'></label></div>"+
                         "</div>" +
                         "<div class='row radiobutton' style='margin-left:76px'>" +
                          //Question.1
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Fully Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' checked value='0' />  </div>" +
                          //Question.2
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Partially Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='1' />  </div>" +
                          //Question.3
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Not Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='2' />  </div>" +
                          //Question.4
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>NA</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "' value='-1' />  </div>" +
                          //TextArea
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Remarks</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +

                          "<textarea  name='remarks" + d.RID + "' class='form-control' >" + d.remarks + "</textarea>  </div>" +

                          "</div>";
                    }
                    else if (d.RatedNumber == -1)
                    {
                        Res += "<div class='container'>" +
                         "<div class='row' style='margin-left:30px'>" +
                               "<div class='col-lg-12 col-md-12 col-sm-12'><input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                               "<input type='checkbox' checked name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin-left:38px' />" +
                               "&nbsp;&nbsp; " + sr + " &nbsp;&nbsp;" +
                               "<button class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' ><i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i></button>" +
                               "&nbsp;&nbsp;&nbsp;<label style='display:unset!important;color:black'  rows='2' cols='70' readonly>" + d.RName + "</label></div>" +
                         //"<div class='col-lg-2 col-md-2 col-sm-1'></div>" +
                         //"<div class='col-lg-8 col-md-8 col-sm-1'><label  rows='2' cols='50'></label></div>"+
                         "</div>" +
                         "<div class='row radiobutton' style='margin-left:76px'>" +
                          //Question.1
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Fully Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='0' />  </div>" +
                          //Question.2
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Partially Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='1' />  </div>" +
                          //Question.3
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Not Met</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='2' />  </div>" +
                          //Question.4
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>NA</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +
                          "<input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "'checked value='-1' />  </div>" +
                          //TextArea
                          "<div class='col-lg-1 col-md-1 col-sm-2'><p>Remarks</p></div>" +
                          "<div class='col-lg-1 col-md-1 col-sm-2'>" +

                          "<textarea  name='remarks" + d.RID + "' class='form-control' >" + d.remarks + "</textarea>  </div>" +

                          "</div>";
                        //    //Res += "<input type='checkbox' name='page' data-id='" + d.PageID + "' id='page' class='page' style='float: left; margin: 12px  10px 0 0px;' /><li class='page_border_role'><input type='text' data-id='" + d.PageID + "' id='menuPage' name='page' value='" + d.ParentPageName + "'/>";    
                        //    Res += "<input type='hidden' id='TotalMarks' name='TotalMarks' value='" + TotalNumber + "'>" +
                        //    "<input type='checkbox' name='page' data-id='" + d.RID + "' id='page1'" + d.RID + "' class='mypage' style='float: left; margin: 12px  10px 0 0px;' />" +
                        //    "<li class='page_border_role'><button style='height:30px' type='button'" + d.RID + " class='btn btn-small btn-success' onclick='GetHint(" + d.RID + ")' value='" + d.RHint + "' id='btnHint'" + d.RID + "' >" +
                        //    "<i class='icon-plus icon-white' style='padding-right:8px; margin-left:3px'></i>" +
                        //    "</button><label style='margin-left:5px' rows='2' cols='60' readonly>" + d.RName + "</label>" +
                        //    "<span style=float:right;font-size: 12px;'><input name='action" + d.RID + "' type='radio'  class='cbAction' id='One" + d.RID + "' value='10' data-id='" + d.RID + "' data-one='" + d.One + "' style='margin: -2px 2px 0 0px;' /> 10</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' value='9' data-two='" + d.Two + "' style='margin: -2px 2px 0 0px;' /> 9</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "'class='cbAction' data-id='" + d.RID + "' data-three='" + d.Three + "' value='8' style='margin: -2px 2px 0 0px;' /> 8</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-four='" + d.Four + "' value='7' style='margin: -2px 2px 0 0px;' /> 7</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-five='" + d.Five + "' value='6' style='margin: -2px 2px 0 0px;' /> 6</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-six='" + d.Six + "' value='5' style='margin: -2px 2px 0 0px;' /> 5</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-seven='" + d.Seven + "' value='4' style='margin: -2px 2px 0 0px;' /> 4</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-eight='" + d.Eight + "' value='3' style='margin: -2px 2px 0 0px;' /> 3</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-nine='" + d.Nine + "' value='2' style='margin: -2px 2px 0 0px;' /> 2</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Ten + "' value='1' style='margin: -2px 2px 0 0px;' /> 1</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='radio' name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.Zero + "' value='0' style='margin: -2px 2px 0 0px;' /> 0</span>" +
                        //    "<span style='float:right; margin-right:20px;font-size: 12px;'><input type='checkbox' checked name='action" + d.RID + "' class='cbAction' data-id='" + d.RID + "' data-ten='" + d.NA + "' value='-1' style='margin: -2px 2px 0 0px;' /> Not Applicable</span></li>";
                    }
                }
                Res += "<label style='font-weight: 600;' id='TotalM' color:#ce0019;'>Fully Met:  " + TotalNumber + "</label><label id='ObtainedMarks'>Partially Met: " + OM + "</label><label id='ObtainedMarksP'>Not Met: " + OMP + "</label><label id='NAQ'>Not Applicable: " + NA + "</label>";
            }
            return Res;
        }
        public JsonResult UpdateRatedNumber(List<ActionPage> PageActions)
        {
            CalculateMark cm = new CalculateMark();
            Session["ObtainedMarks"] = null;
            int OM = 0;
            int TM = 0;
            int OMP = 0;
            int NA = 0;
            foreach (var d in PageActions)
            {
                FOSDataModel db = new FOSDataModel();
                RatedNumber obj = new RatedNumber();
                TempRatedNumber tobj = new TempRatedNumber();
                tobj = db.TempRatedNumbers.Where(x => x.QuestionID == d.QRID && x.IsActive == true && x.AuditID == d.audit_id).FirstOrDefault();
                tobj.IsActive = false;
                obj = db.RatedNumbers.Where(x => x.QuestionID == d.QRID && x.AuditID == d.audit_id).FirstOrDefault();
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
                obj.ModifiedDate = DateTime.Now;
                obj.IsActive = true;
                obj.IsDeleted = false;
                db.SaveChanges();
            }
            cm.OM = OM;
            cm.OMP = OMP;
            cm.TM = TM;
            cm.NA = NA;
            Session["TotalMarks"] = null;
            return Json(cm);
        }

        #endregion

        #region Rating Result
        public ActionResult CheckListResult()
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
        public JsonResult GetSubDepartmentListByDeptAndRegionWise(int DeptID, int RegionID)
        {
            var result = ManageAdmin.GetSubDepartmentDepartmentAndRegionWise(DeptID, RegionID);
            return Json(result);
        }
        public JsonResult GetCenterResult(int CategoryID, int CenterID)
        {
            FOSDataModel db = new FOSDataModel();
            List<QAData> obj = new List<QAData>();
            var CD = db.CenterDepartments.Where(x => x.CategoryID == CategoryID).Select(x => x.ID).ToList();
            foreach (var DeptID in CD)
            {
                //obj = new List<QAData>();
                var result = db.GetCLRList(CategoryID, CenterID, DeptID).Select(x => new QAData
                {
                    Category = x.Category,
                    Center = x.Center,
                    Department = x.Department,
                    TotalMarks = x.TotalNumber,
                    ObtainedMarks = x.ObtainedMarks,
                    ObtainedPercentage = x.ObtainedPercentage
                }).FirstOrDefault();
                obj.Add(result);
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTotalResult(int CategoryID, int CenterID, int audit_id)
        {
            QAData obj = new QAData();
            List<QAData> dtsource = new List<QAData>();
            dtsource = ManageAdmin.GetCLRForGrid(CategoryID, CenterID, audit_id);
            int TM = 0;
            int OM = 0;
            int NA = 0;
            int TOP = 0;
            if (dtsource.Count != 0)
            {
                foreach (var resultObj in dtsource)
                {
                    TM = TM + Convert.ToInt32(resultObj.TotalMarks);
                    OM = OM + Convert.ToInt32(resultObj.ObtainedMarks);
                    NA = NA + Convert.ToInt32(resultObj.NotApplicable);
                    TOP = TOP + Convert.ToInt32(resultObj.ObtainedPercentage);
                }
                obj.TM = TM;
                obj.OM = OM;
                obj.TOP = TOP;
                obj.NA = NA;
            }
            else
            {
                obj.TM = 0;
                obj.OM = 0;
                obj.TOP = 0;
            }
            return Json(obj);
        }
        public void GetTotalResultReport(int CategoryID, int CenterID, int audit_id)
        {
            FOSDataModel db = new FOSDataModel();
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();

            try
            {

                GetCLRList1_1_Result obj = new GetCLRList1_1_Result();
                List<GetCLRList1_1_Result> dtsource = new List<GetCLRList1_1_Result>();
                dtsource = ManageAdmin.GetCLRForGrid3(CategoryID, CenterID, audit_id);

                //ReportParameter[] prm = new ReportParameter[3];
                //prm[0] = new ReportParameter("DateFrom", StartingDate);
                //prm[1] = new ReportParameter("DateTo", EndingDate);
                //prm[2] = new ReportParameter("SaleOfficer", SaleOfficer);
                ReportViewer1.ReportPath = Server.MapPath("~/Views/Admin/AuditResult.rdlc");
                ReportViewer1.EnableExternalImages = true;
                ReportDataSource dt2 = new ReportDataSource("DataSet1", dtsource);
                //ReportViewer1.SetParameters(prm);
                ReportViewer1.DataSources.Clear();
                ReportViewer1.DataSources.Add(dt2);
                ReportViewer1.Refresh();



                Warning[] warnings;
                string[] streamIds;
                string contentType;
                string encoding;
                string extension;

                //Export the RDLC Report to Byte Array.
                byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);
                //using (FileStream fs = File.Create(Server.MapPath("~/download/") + dt2))
                //{
                //    fs.Write(bytes, 0, bytes.Length);
                //}
                //Download the RDLC Report in Word, Excel, PDF and Image formats.
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
            //return Json(0);
        }
        //public void GetTotalSummaryResultReport(int CategoryID, int CenterID, int audit_id)
        //{
        //    FOSDataModel db = new FOSDataModel();
        //    Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();

        //    try
        //    {
        //    //    GetCLRList1_1_Result obj = new GetCLRList1_1_Result();
        //    //    GetSummaryCLRList1_1_Result obj2 = new GetSummaryCLRList1_1_Result();
        //        List<GetCLRList1_1_Result> dtsource = new List<GetCLRList1_1_Result>();
        //        dtsource = ManageAdmin.GetCLRForGrid3(CategoryID, CenterID, audit_id);
        //        for (int i = 0; i < dtsource.Count; i++)
        //        {
        //            int total = Convert.ToInt32(dtsource[i].TotalNumber + dtsource[i].ObtainedMarks + dtsource[i].ObtainedPercentage);
        //            if (total==0)
        //            {
        //                total = 1;
        //            }
        //            dtsource[i].FullPer = decimal.Round((Convert.ToDecimal(dtsource[i].TotalNumber / total) * 100), 2, MidpointRounding.AwayFromZero);
        //            dtsource[i].PartialPer = decimal.Round((Convert.ToDecimal(dtsource[i].ObtainedMarks / total) * 100), 2, MidpointRounding.AwayFromZero);
        //            dtsource[i].NotPer = decimal.Round((Convert.ToDecimal(dtsource[i].ObtainedPercentage / total) * 100), 2, MidpointRounding.AwayFromZero);
        //        }
        //        List<GetSummaryCLRList1_1_Result> dtsource2 = new List<GetSummaryCLRList1_1_Result>();
        //        dtsource2 = ManageAdmin.GetSummaryCLRForGrid1(CategoryID, CenterID, audit_id);
        //        int TotalRecord = dtsource.Where(x => x.TotalNumber != 0 || x.ObtainedMarks != 0 || x.ObtainedPercentage != 0).Count();
        //        ReportParameterCollection ReportParamMeters = new ReportParameterCollection();
        //        ReportParamMeters.Add(new ReportParameter("TotalRecord", TotalRecord.ToString()));
        //        //ReportParameter[] prm = new ReportParameter[3];
        //        //prm[0] = new ReportParameter("DateFrom", StartingDate);
        //        //prm[1] = new ReportParameter("DateTo", EndingDate);
        //        //prm[2] = new ReportParameter("SaleOfficer", SaleOfficer);
        //        ReportViewer1.ReportPath = Server.MapPath("~/Views/Admin/AuditSummaryResult.rdlc");
        //        ReportViewer1.EnableExternalImages = true;
        //        ReportDataSource dt = new ReportDataSource("DataSet2", dtsource);
        //        ReportDataSource dt2 = new ReportDataSource("DataSet1", dtsource2);
        //        //ReportViewer1.SetParameters(prm);
        //        ReportViewer1.DataSources.Clear();
        //        ReportViewer1.DataSources.Add(dt);
        //        ReportViewer1.DataSources.Add(dt2);
        //        ReportViewer1.Refresh();



        //        Warning[] warnings;
        //        string[] streamIds;
        //        string contentType;
        //        string encoding;
        //        string extension;

        //        //Export the RDLC Report to Byte Array.
        //        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);
        //        //using (FileStream fs = File.Create(Server.MapPath("~/download/") + dt2))
        //        //{
        //        //    fs.Write(bytes, 0, bytes.Length);
        //        //}
        //        //Download the RDLC Report in Word, Excel, PDF and Image formats.
        //        Response.Clear();
        //        Response.Buffer = true;
        //        Response.Charset = "";
        //        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //        Response.ContentType = contentType;
        //        Response.AddHeader("content-disposition", "attachment;filename=AuditSummaryReport" + DateTime.Now + ".Pdf");
        //        Response.BinaryWrite(bytes);
        //        Response.Flush();

        //        Response.End();
        //        //return Json(0);
        //    }

        //    catch (Exception exp)
        //    {
        //        Log.Instance.Error(exp, "Report Not Working");

        //    }
        //    //return Json(0);
        //}

        public void GetTotalSummaryResultReport(int CategoryID, int CenterID, int audit_id,int RegionID)
        {
            FOSDataModel db = new FOSDataModel();
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();

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
                //ReportParameter[] prm = new ReportParameter[3];
                //prm[0] = new ReportParameter("DateFrom", StartingDate);
                //prm[1] = new ReportParameter("DateTo", EndingDate);
                //prm[2] = new ReportParameter("SaleOfficer", SaleOfficer);
                ReportViewer1.ReportPath = Server.MapPath("~/Views/Admin/AuditSummaryResult.rdlc");
                ReportViewer1.EnableExternalImages = true;
                ReportDataSource dt = new ReportDataSource("DataSet2", dtsource);
                ReportDataSource dt2 = new ReportDataSource("DataSet1", dtsource2);
                //ReportViewer1.SetParameters(prm);
                ReportViewer1.DataSources.Clear();
                int TotalRecord = dtsource.Where(x => x.TotalNumber != 0 || x.ObtainedMarks != 0 || x.ObtainedPercentage != 0).Count();
                
                // <reportParameter>
                ReportParameterCollection ReportParamMeters = new ReportParameterCollection();
                ReportParamMeters.Add(new ReportParameter("TotalRecord", TotalRecord.ToString()));
                Audit audit = new Audit();
                Regions region = new Regions();
                audit = db.Audit.Where(u => u.audit_id == audit_id).FirstOrDefault();
                region = db.Regions.Where(u => u.ID == RegionID).FirstOrDefault();
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
                //using (FileStream fs = File.Create(Server.MapPath("~/download/") + dt2))
                //{
                //    fs.Write(bytes, 0, bytes.Length);
                //}
                //Download the RDLC Report in Word, Excel, PDF and Image formats.
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
            //return Json(0);
        }
        public void GetQuestionareReport(int CategoryID, int CenterID, int audit_id)
        {
            FOSDataModel db = new FOSDataModel();
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();

            try
            {
                GetCLRList1_1_Result obj = new GetCLRList1_1_Result();
                GetSummaryCLRList1_1_Result obj2 = new GetSummaryCLRList1_1_Result();

                List<GetCLRList1_1_Result> dtsource = new List<GetCLRList1_1_Result>();
                dtsource = ManageAdmin.GetCLRForGrid3(CategoryID, CenterID, audit_id);


                List<GetSummaryCLRList1_2_Result> dtsource2 = new List<GetSummaryCLRList1_2_Result>();
                dtsource2 = ManageAdmin.GetAllQuestionsForGrid1(CategoryID, CenterID, audit_id);

                //ReportParameter[] prm = new ReportParameter[3];
                //prm[0] = new ReportParameter("DateFrom", StartingDate);
                //prm[1] = new ReportParameter("DateTo", EndingDate);
                //prm[2] = new ReportParameter("SaleOfficer", SaleOfficer);
                ReportViewer1.ReportPath = Server.MapPath("~/Views/Admin/QAReport.rdlc");
                ReportViewer1.EnableExternalImages = true;
                ReportDataSource dt = new ReportDataSource("DataSet2", dtsource);
                ReportDataSource dt2 = new ReportDataSource("DataSet1", dtsource2);
                //ReportViewer1.SetParameters(prm);
                ReportViewer1.DataSources.Clear();
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
                //using (FileStream fs = File.Create(Server.MapPath("~/download/") + dt2))
                //{
                //    fs.Write(bytes, 0, bytes.Length);
                //}
                //Download the RDLC Report in Word, Excel, PDF and Image formats.
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;filename=QuestionReport" + DateTime.Now + ".Pdf");
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
        public JsonResult CLRDataHandler(DTParameters param)
        {

            try
            {
                var dtsource = new List<QAData>();
                dtsource = ManageAdmin.GetCLRForGrid(param.CategoryID, param.CenterID, param.audit_id);


                List<String> columnSearch = new List<string>();
                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }
                List<QAData> data = ManageAdmin.GetResultR(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageAdmin.CountR(param.Search.Value, dtsource, columnSearch);
                for (int i = 0; i < data.Count; i++)
                {
                    int total = Convert.ToInt32(data[i].TotalMarks + data[i].ObtainedMarks + data[i].ObtainedPercentage);
                    if (total == 0)
                        total = 1;
                    data[i].TM = decimal.Round((Convert.ToDecimal(data[i].TotalMarks / total) * 100), 2, MidpointRounding.AwayFromZero);
                    data[i].OM = decimal.Round((Convert.ToDecimal(data[i].ObtainedMarks / total) * 100), 2, MidpointRounding.AwayFromZero);
                    data[i].NA = decimal.Round((Convert.ToDecimal(data[i].ObtainedPercentage / total) * 100), 2, MidpointRounding.AwayFromZero);
                }
                DTResult<QAData> result = new DTResult<QAData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                //List<DataPoint> _dataPoints = new List<DataPoint>();
                //double y = 0; 
                //string label = ""; 

                //for (int i = 0; i < data.Count; i++)
                //{
                //    y = Convert.ToDouble(data[i].TotalMarks);
                //    label = data[i].Department;

                //    _dataPoints.Add(new DataPoint(y, label)); 
                //}
                //ViewBag.DataPoints = JsonConvert.SerializeObject(_dataPoints, _jsonSetting);
                //ViewBag.DataPoints = JsonConvert.SerializeObject(DataService.GetRandomDataForCategoryAxis(4), _jsonSetting);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public ActionResult NewReports()
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

        public void GetComparisonReport(int CategoryID, int CenterID, int audit_id, int to_audit_id)
        {
            FOSDataModel db = new FOSDataModel();
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();

            try
            {
                GetCLRList1_1_Result obj = new GetCLRList1_1_Result();

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

                List<GetCLRList1_1_Result> dtsource2 = new List<GetCLRList1_1_Result>();
                dtsource2 = ManageAdmin.GetCLRForGrid3(CategoryID, CenterID, to_audit_id);
                for (int i = 0; i < dtsource.Count; i++)
                {
                    int total = Convert.ToInt32(dtsource2[i].TotalNumber + dtsource2[i].ObtainedMarks + dtsource2[i].ObtainedPercentage);
                    if (total == 0)
                    {
                        total = 1;
                    }
                    dtsource2[i].FullPer = decimal.Round((Convert.ToDecimal(dtsource2[i].TotalNumber / total) * 100), 2, MidpointRounding.AwayFromZero);
                    dtsource2[i].PartialPer = decimal.Round((Convert.ToDecimal(dtsource2[i].ObtainedMarks / total) * 100), 2, MidpointRounding.AwayFromZero);
                    dtsource2[i].NotPer = decimal.Round((Convert.ToDecimal(dtsource2[i].ObtainedPercentage / total) * 100), 2, MidpointRounding.AwayFromZero);
                }
                Audit obj2 = new Audit();
                obj2 = db.Audit.Where(u => u.audit_id == audit_id).FirstOrDefault();
                ReportParameterCollection ReportParamMeters = new ReportParameterCollection();
                ReportParamMeters.Add(new ReportParameter("AuditFrom", obj2.audit_year.ToString()));
                obj2 = db.Audit.Where(u => u.audit_id == to_audit_id).FirstOrDefault();
                ReportParamMeters.Add(new ReportParameter("AuditTo", obj2.audit_year.ToString()));
                int TotalRecord = dtsource.Where(x => x.TotalNumber != 0 || x.ObtainedMarks != 0 || x.ObtainedPercentage != 0).Count();
                ReportParamMeters.Add(new ReportParameter("TotalRecord", TotalRecord.ToString()));

                ReportViewer1.ReportPath = Server.MapPath("~/Views/Admin/AuditComparisonReport.rdlc");

                ReportViewer1.SetParameters(ReportParamMeters);

                ReportViewer1.EnableExternalImages = true;
                ReportDataSource dt = new ReportDataSource("DataSet1", dtsource);
                ReportDataSource dt2 = new ReportDataSource("DataSet2", dtsource2);
                //ReportViewer1.SetParameters(prm);
                ReportViewer1.DataSources.Clear();
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
                //using (FileStream fs = File.Create(Server.MapPath("~/download/") + dt2))
                //{
                //    fs.Write(bytes, 0, bytes.Length);
                //}
                //Download the RDLC Report in Word, Excel, PDF and Image formats.
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;filename=ComparisonReport" + DateTime.Now + ".Pdf");
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
        #endregion

        #region Rating Result

        public ActionResult RatingResult()
        {
            QAData obj = new QAData();
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            List<SubDepartmentData> DeptList = ManageAdmin.GetAllSubDepartment();
            obj.SubDept = DeptList;
            List<CenterDepartmentData> CenterDept = ManageAdmin.GetAllCenterDepartment();
            obj.CenterDept = CenterDept;
            ViewBag.DataPoints = JsonConvert.SerializeObject(DataService.GetRandomDataForCategoryAxis(5), _jsonSetting);
            return View(obj);
        }


        public ActionResult GetResultDetail(string deptID, string CenterDeptID, string SubDeptID)
        {
            DALResult objService = new DALResult();
            QAData model = objService.Get_RatingResult(deptID, CenterDeptID, SubDeptID);
            return PartialView("~/Views/Shared/_ResultDetail.cshtml", model);
        }
        #endregion

        #region Dashboard
        public ActionResult Dashboard()
        {
            QAData obj = new QAData();
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            List<SubDepartmentData> DeptList = ManageAdmin.GetAllSubDepartment();
            obj.SubDept = DeptList;
            obj.Audit = ManageAdmin.GetAuditForGrid();
            return View(obj);
        }

        public JsonResult GetDashBoard(int CategoryID, int CenterID, int audit_id)
        {
            List<GetCLRList1_1_Result> dtsource = new List<GetCLRList1_1_Result>();
            try
            {
                dtsource = ManageAdmin.GetCLRForGrid3(CategoryID, CenterID, audit_id);
                for (int i = 0; i < dtsource.Count; i++)
                {
                    int total = Convert.ToInt32(dtsource[i].TotalNumber + dtsource[i].ObtainedMarks + dtsource[i].ObtainedPercentage);
                    if (total == 0)
                        total = 1;
                    dtsource[i].FullPer = decimal.Round((Convert.ToDecimal(dtsource[i].TotalNumber / total) * 100), 2, MidpointRounding.AwayFromZero);
                    dtsource[i].PartialPer = decimal.Round((Convert.ToDecimal(dtsource[i].ObtainedMarks / total) * 100), 2, MidpointRounding.AwayFromZero);
                    dtsource[i].NotPer = decimal.Round((Convert.ToDecimal(dtsource[i].ObtainedPercentage / total) * 100), 2, MidpointRounding.AwayFromZero);
                }

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
            }
            return Json(dtsource);
        }

        #endregion

        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
    }
}
