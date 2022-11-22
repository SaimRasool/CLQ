using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FOS.Shared;
using FOS.DataLayer;
using System.Transactions;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using System.Globalization;
using Shared.Diagnostics.Logging;

namespace FOS.Setup
{
   public class ManageHome
    {
      
        public List<TotalAppointmentDeparmentWiseToday_Result> TotalAppointmentToday()
        {
            List<TotalAppointmentDeparmentWiseToday_Result> Obj = new List<TotalAppointmentDeparmentWiseToday_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                Obj = dbContext.TotalAppointmentDeparmentWiseToday().ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return Obj;
        }


        public List<Sp_Top10VisitsCityWiseGraph1_1_Result> TopSalesCityWise(DateTime StartingDate)
        {
            List<Sp_Top10VisitsCityWiseGraph1_1_Result> RetailerObj = new List<Sp_Top10VisitsCityWiseGraph1_1_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();
                StartingDate = Convert.ToDateTime("2019-01-08");
                RetailerObj = dbContext.Sp_Top10VisitsCityWiseGraph1_1(StartingDate).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }



        //public List<Sp_PresentSOBarGraph1_1_Result> TotalPresentSO()
        //{
        //    List<Sp_PresentSOBarGraph1_1_Result> RetailerObj = new List<Sp_PresentSOBarGraph1_1_Result>();
        //    try
        //    {


        //        FOSDataModel dbContext = new FOSDataModel();

        //        RetailerObj = dbContext.Sp_PresentSOBarGraph1_1().ToList();


        //    }

        //    catch (Exception exp)
        //    {
        //        Log.Instance.Error(exp, "Data not Found");
        //        throw;
        //    }
        //    return RetailerObj;
        //}




        public List<Sp_SOVisitsTodayFinal_Result> SOVisitsToday()
        {
            List<Sp_SOVisitsTodayFinal_Result> RetailerObj = new List<Sp_SOVisitsTodayFinal_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_SOVisitsTodayFinal().ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }
    }
}
