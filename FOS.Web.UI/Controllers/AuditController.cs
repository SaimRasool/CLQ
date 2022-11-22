using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Shared;
using FOS.Web.UI.Models;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace CLQApp.Controllers
{
    public class AuditController : Controller
    {
        private FOSDataModel db = new FOSDataModel();

        #region AuditComponents
        // GET: AuditComponents
        public ActionResult Index()
        {
            var auditComponents = db.AuditComponents.Include(a => a.Department).Include(a => a.SubDepartment);
            return View(auditComponents.ToList());
        }

        // GET: AuditComponents/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditComponent auditComponent = db.AuditComponents.Find(id);
            if (auditComponent == null)
            {
                return HttpNotFound();
            }
            return View(auditComponent);
        }

        // GET: AuditComponents/SubDept/5
        public ActionResult SubDept(string id)
        {
            ViewBag.SubDeptID = new SelectList(db.SubDepartments, "ID", "Name").Where(x => x.Value == id);
            return View();
        }

        // GET: AuditComponents/Create
        public ActionResult Create()
        {
            ViewBag.DeptID = new SelectList(db.Departments, "ID", "Name");
            ViewBag.SubDeptID = new SelectList(db.SubDepartments, "ID", "Name");
            return View();
        }

        // POST: AuditComponents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DeptID,SubDeptID,AuditDate,AuditTime,AuditerName,InstitutionAddress,PhoneNo,FaxNo,Email,InstitutionHead,HeadPhoneNo")] AuditComponent auditComponent)
        {
            if (ModelState.IsValid)
            {
                db.AuditComponents.Add(auditComponent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeptID = new SelectList(db.Departments, "ID", "Name", auditComponent.DeptID);
            ViewBag.SubDeptID = new SelectList(db.SubDepartments, "ID", "Name", auditComponent.SubDeptID);
            return View(auditComponent);
        }

        // GET: AuditComponents/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditComponent auditComponent = db.AuditComponents.Find(id);
            if (auditComponent == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeptID = new SelectList(db.Departments, "ID", "Name", auditComponent.DeptID);
            ViewBag.SubDeptID = new SelectList(db.SubDepartments, "ID", "Name", auditComponent.SubDeptID);
            return View(auditComponent);
        }

        // POST: AuditComponents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DeptID,SubDeptID,AuditDate,AuditTime,AuditerName,InstitutionAddress,PhoneNo,FaxNo,Email,InstitutionHead,HeadPhoneNo")] AuditComponent auditComponent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auditComponent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeptID = new SelectList(db.Departments, "ID", "Name", auditComponent.DeptID);
            ViewBag.SubDeptID = new SelectList(db.SubDepartments, "ID", "Name", auditComponent.SubDeptID);
            return View(auditComponent);
        }

        // GET: AuditComponents/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditComponent auditComponent = db.AuditComponents.Find(id);
            if (auditComponent == null)
            {
                return HttpNotFound();
            }
            return View(auditComponent);
        }

        // POST: AuditComponents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            AuditComponent auditComponent = db.AuditComponents.Find(id);
            db.AuditComponents.Remove(auditComponent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Observation
        public ActionResult AddObervation()
        {
            ObservationData obj = new ObservationData();
            List<Department> AllDept = ManageAdmin.GetAllDepartment();
            obj.Dept = AllDept;
            obj.RegionsList = ManageAdmin.GetAllRegions();
            List<SubDepartmentData> DeptList = ManageAdmin.GetAllSubDepartment();
            obj.SubDept = DeptList;
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                obj.AuditYear = dbContext.Audit.Where(u => u.audit_year == DateTime.Today.Year && u.is_active == true).Select(u => u.audit_year).FirstOrDefault();

            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateObervation([Bind(Exclude = "TID")] ObservationData newObervation)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newObervation != null)
                {
                    if (boolFlag)
                    {
                        int Response = ManageAdmin.AddUpdateObservation(newObervation);

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

        public JsonResult ObservationDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ObservationData>();

                dtsource = ManageAdmin.GetObservationForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<ObservationData> data = ManageAdmin.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageAdmin.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<ObservationData> result = new DTResult<ObservationData>
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
        public int DeleteDepartment(int ObservationID)
        {
            return FOS.Setup.ManageAdmin.DeleteObservation(ObservationID);
        }
        #endregion

        #region Audit
        public ActionResult CreateAudit()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateAudit([Bind(Exclude = "TID")] Audit newAudit)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newAudit != null)
                {
                    if (boolFlag)
                    {
                        int Response = ManageAdmin.AddUpdateAudit(newAudit);

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

        public JsonResult AuditDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<Audit>();

                dtsource = ManageAdmin.GetAuditForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<Audit> data = ManageAdmin.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageAdmin.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<Audit> result = new DTResult<Audit>
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
        public int DeleteAudit(int audit_id)
        {
            return FOS.Setup.ManageAdmin.DeleteAudit(audit_id);
        }
        public int UpdateAudit(int audit_id)
        {
            return FOS.Setup.ManageAdmin.UpdateAuditStatus(audit_id);
        }
        #endregion

        #region Approve Audit Report by Lead Auditor
        public ActionResult LeadApprovel()
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
        public ActionResult UpdateLeadApprovel(int CategoryID, int CenterID, int RegionID)
        {
                try
                {
                int audit_id = db.Audit.Where(u => u.audit_year == DateTime.Today.Year && u.is_active == true).Select(u => u.audit_id).FirstOrDefault();

                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Approvel approvelObj = dbContext.Approvel.Where(u => u.AuditID == audit_id && u.CenterID == CenterID).FirstOrDefault();
                        if (approvelObj==null)
                        {
                            approvelObj = new Approvel();
                            approvelObj.AuditID = audit_id;
                            approvelObj.CenterID = CenterID;
                            approvelObj.LeadApprovel = true;
                            approvelObj.AdminApprovel = false;
                            approvelObj.LeadApprovelDate = DateTime.Now;
                            dbContext.Approvel.Add(approvelObj);
                            dbContext.SaveChanges();
                        }
                        scope.Complete();
                    }
                }
            }
                catch (Exception exp)
                {
                    Log.Instance.Error(exp, "Report Not Working");

                }
            return RedirectToAction("LeadApprovel");
        }
        public ActionResult AdminApprovel()
        {
            return View();
        }
        public ActionResult LoadData()
        {
            int audit_id = db.Audit.Where(u => u.audit_year == DateTime.Today.Year && u.is_active == true).Select(u => u.audit_id).FirstOrDefault();
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault().ToString();
                var length = Request.Form.GetValues("length").FirstOrDefault().ToString();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault().ToString();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault().ToString();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault().ToString();
                searchValue = searchValue.ToLower();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                List<ApprovelRequest> obj=new List<ApprovelRequest>();
                List<CenterDepartmentData> CenterDept = ManageAdmin.GetAllCenterDepartment();
                var result = from approvel in db.Approvel
                    join centers in db.SubDepartments on approvel.CenterID equals centers.ID 
                    where approvel.AuditID == audit_id
                             select new ApprovelRequest
                             {
                                 Center = centers.Name,
                                 CenterID = (int)approvel.CenterID,
                                 LeadApprovel = approvel.LeadApprovel,
                                 AdminApprovel = approvel.AdminApprovel,
                                 LeadApprovelDate = (DateTime)approvel.LeadApprovelDate    
                             };
                obj = result.ToList();
                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {

                    obj = obj.OrderBy(sortColumn + " " + sortColumnDir).Skip(skip).Take(pageSize).ToList();
                }
                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    obj = obj.Where(m => m.Center.ToLower().Contains(searchValue) || (DateTime.TryParse(searchValue, out DateTime dateTime) && m.LeadApprovelDate > Convert.ToDateTime(searchValue) && m.LeadApprovelDate < Convert.ToDateTime(searchValue))).ToList();
                }

                //total number of rows count     
                recordsTotal = obj.Count();
                //Paging     
                var data = obj.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        [HttpPost]
        public ActionResult Approved(int id)
        {
            Approvel approvelObj = db.Approvel.Where(u => u.ID == id).FirstOrDefault();
            approvelObj.AdminApprovel = true;
            approvelObj.AdminApprovelDate = DateTime.Today;
            db.SaveChanges();
            return Json(true);
        }
        public ActionResult LoadAdminData()
        {
            int audit_id = db.Audit.Where(u => u.audit_year == DateTime.Today.Year && u.is_active == true).Select(u => u.audit_id).FirstOrDefault();
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault().ToString();
                var length = Request.Form.GetValues("length").FirstOrDefault().ToString();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault().ToString();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault().ToString();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault().ToString();
                searchValue = searchValue.ToLower();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                List<ApprovelRequest> obj = new List<ApprovelRequest>();
                List<CenterDepartmentData> CenterDept = ManageAdmin.GetAllCenterDepartment();
                var result = from approvel in db.Approvel
                             join centers in db.SubDepartments on approvel.CenterID equals centers.ID
                             where approvel.AuditID == audit_id && approvel.AdminApprovel==false
                             select new ApprovelRequest
                             {
                                 ID=approvel.ID,
                                 Center = centers.Name,
                                 CenterID = (int)approvel.CenterID,
                                 LeadApprovel = approvel.LeadApprovel,
                                 AdminApprovel = approvel.AdminApprovel,
                                 LeadApprovelDate = (DateTime)approvel.LeadApprovelDate
                             };
                obj = result.ToList();
                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {

                    obj = obj.OrderBy(sortColumn + " " + sortColumnDir).Skip(skip).Take(pageSize).ToList();
                }
                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    obj = obj.Where(m => m.Center.ToLower().Contains(searchValue) || (DateTime.TryParse(searchValue, out DateTime dateTime) && m.LeadApprovelDate > Convert.ToDateTime(searchValue) && m.LeadApprovelDate < Convert.ToDateTime(searchValue))).ToList();
                }

                //total number of rows count     
                recordsTotal = obj.Count();
                //Paging     
                var data = obj.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion
    }
    public class ApprovelRequest
    {
        public string asc { get; set; }
        public int ID { get; set; }
        public string Center { get; set; }
        public int CenterID { get; set; }
        public bool LeadApprovel { get; set; }
        public DateTime LeadApprovelDate { get; set; }
        public bool AdminApprovel { get; set; }
        public DateTime AdminApprovelDate { get; set; }
    }
}
