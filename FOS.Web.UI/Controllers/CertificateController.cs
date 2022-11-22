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
    public class CertificateController : Controller
    {
        [System.Web.Http.Authorize]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public void Index(FormCollection form, HttpPostedFileBase file)
        {

            try
            {
                var allowedextention = new[] { ".jpg", ".Jpg", ".jpeg", ".png" };

                if (file != null && file.ContentLength > 0)
                {
                    if (!Directory.Exists(Server.MapPath("~/Uploads")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads"));
                    }
                    var ext = Path.GetExtension(file.FileName);
                    if (allowedextention.Contains(ext))
                    {
                        var path = "~/Uploads/Image_" + file.FileName;
                        path = Server.MapPath(path);

                        file.SaveAs(path);

                        //path = path.Replace("~/", "/../");
                        var uri = new System.Uri(path).AbsoluteUri;
                        Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();
                        string title = form["Title"];
                        string serial = form["Serial"];
                        DateTime fromDate = Convert.ToDateTime(form["FromDate"]);
                        DateTime toDate = Convert.ToDateTime(form["ToDate"]);
                     
                        ReportViewer1.ReportPath = Server.MapPath("~/Views/Certificate/ECertificate.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        //ReportViewer1.SetParameters(prm);

                        // <reportParameter>
                        ReportParameterCollection ReportParamMeters = new ReportParameterCollection();
                        ReportViewer1.Refresh();
                        ReportParamMeters.Add(new ReportParameter("Title", title));
                        ReportParamMeters.Add(new ReportParameter("SerialNo", serial));
                        ReportParamMeters.Add(new ReportParameter("From", fromDate.ToLongDateString().ToString()));
                        ReportParamMeters.Add(new ReportParameter("To", toDate.ToLongDateString().ToString()));
                        ReportParamMeters.Add(new ReportParameter("Image", uri));
                        ReportViewer1.SetParameters(ReportParamMeters);


                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=Certificate" + serial + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();
                        //return Json(0);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
