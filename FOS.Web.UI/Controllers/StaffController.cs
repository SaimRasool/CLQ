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

namespace FOS.Web.UI.Controllers
{
    public class StaffController : Controller
    {
        public ActionResult StaffDetail()
        {
            StaffDetailData data = new StaffDetailData();
            data.Category = ManageAdmin.GetAllDepartment();
            List<SubDepartmentData> CenterList = ManageAdmin.GetAllSubDepartment();
            data.RegionsList = ManageAdmin.GetAllRegions();
            data.Center = CenterList;
            data.StaffTypes = ManageAdmin.GetAllStaffType();
            if (data.StaffSubTypes == null || data.StaffSubTypes.Count == 0)
            {
                data.StaffSubTypes = new List<StaffSubTypeData>();
                data.StaffSubTypes.Add(new StaffSubTypeData() { ID = 0, Name = "--Select Sub Staff--" });

            }
            return View(data);
        }

        public JsonResult GetSubStaffDetail(int StaffTypeId)
        {
            var result = ManageAdmin.GetSubStaffListStaffWise(StaffTypeId);
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateSatffDetail([Bind(Exclude = "TID")] StaffDetailData model)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (model != null)
                {

                    if (boolFlag)
                    {
                        int Response = ManageAdmin.AddUpdateStaffDetail(model);

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

        public ActionResult CenterProfile()
        {
            StaffDetailData data = new StaffDetailData();
            data.Category = ManageAdmin.GetAllDepartment();
            data.RegionsList = ManageAdmin.GetAllRegions();
            List<SubDepartmentData> CenterList = ManageAdmin.GetAllSubDepartment();
            data.Center = CenterList;
            return View(data);
        }
        public JsonResult StaffDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<StaffDetailData>();

                dtsource = ManageAdmin.GetStaffDetailForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<StaffDetailData> data = ManageAdmin.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageAdmin.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<StaffDetailData> result = new DTResult<StaffDetailData>
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
        public void GetStaffReport(int CenterID)
        {
            FOSDataModel db = new FOSDataModel();
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();

            try
            {
                DALResult obj = new DALResult();
                DataSet ds = obj.get_emp_staff(CenterID);              
                ReportViewer1.ReportPath = Server.MapPath("~/Views/Staff/Staff_Report.rdlc");
                ReportViewer1.EnableExternalImages = true;
                ReportDataSource dt = new ReportDataSource("DataSet1", ds.Tables[0]);
                ReportDataSource dt2 = new ReportDataSource("DataSet2", ds.Tables[1]);
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
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;filename=CenterProfileReport" + DateTime.Now + ".Pdf");
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

        //Delete Department Method...
        public int DeleteStaffDetail(int id)
        {
            return FOS.Setup.ManageAdmin.DeleteStaffDetail(id);
        }


    }
}
