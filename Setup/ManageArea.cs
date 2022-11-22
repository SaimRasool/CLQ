using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI.WebControls;

namespace FOS.Setup
{
   public class ManageArea
    {
    
        public static int DeleteAccess(int ID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_Access obj = dbContext.Tbl_Access.Where(u => u.ID == ID).FirstOrDefault();
                    dbContext.Tbl_Access.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Area Failed");
                Resp = 1;
            }
            return Resp;
        }
        public static int AddUpdateAccess(AreaData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Tbl_Access AreaObj = new Tbl_Access();

                        if (obj.ID == 0)
                        {
                            AreaObj.SaleOfficerID = obj.SaleOfficerID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.ReportedUp = obj.intSaleOfficerIDfrom;
                            AreaObj.ReportedDown = obj.intSaleOfficerIDto;
                            AreaObj.IsActive = true;
                            AreaObj.IsDeleted = false;
                            dbContext.Tbl_Access.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Tbl_Access.Where(u => u.ID == obj.ID).FirstOrDefault();
                            AreaObj.SaleOfficerID = obj.SaleOfficerID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.ReportedUp = obj.intSaleOfficerIDfrom;
                            AreaObj.ReportedDown = obj.intSaleOfficerIDto;
                            AreaObj.IsActive = true;
                            AreaObj.IsDeleted = false;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
        public static List<AreaData> GetResult7(string search, string sortOrder, int start, int length, List<AreaData> dtResult, List<string> columnFilters)
        {
            return FilterResult7(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }
        public static int Count7(string search, List<AreaData> dtResult, List<string> columnFilters)
        {
            return FilterResult7(search, dtResult, columnFilters).Count();
        }
        private static IQueryable<AreaData> FilterResult7(string search, List<AreaData> dtResult, List<string> columnFilters)
        {
            IQueryable<AreaData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(search.ToLower()))
                //&& (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                //&& (columnFilters[4] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[4].ToLower())))
                //&& (columnFilters[5] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[5].ToLower())))
                //&& (columnFilters[6] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[6].ToLower())))
                ));

            return results;
        }
      


        public static List<AreaData> GetResult(string search, string sortOrder, int start, int length, List<AreaData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<AreaData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<AreaData> FilterResult(string search, List<AreaData> dtResult, List<string> columnFilters)
        {
            IQueryable<AreaData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionName != null && p.RegionName.ToLower().Contains(search.ToLower()) || p.CityName != null && p.CityName.ToLower().Contains(search.ToLower()) || p.Message != null && p.Message.ToLower().Contains(search.ToLower()) || p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(search.ToLower()) || p.RetailerName != null && p.RetailerName.ToLower().Contains(search.ToLower()) || p.DealerName != null && p.DealerName.ToLower().Contains(search.ToLower())))
                && (columnFilters[1] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[1].ToLower())))
                && (columnFilters[2] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[2].ToLower())))
                && (columnFilters[3] == null || (p.SchoolName != null && p.SchoolName.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.Message != null && p.Message.ToLower().Contains(columnFilters[4].ToLower())))
                );

            return results;
        }

     
    }
}
