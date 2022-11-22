using FOS.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FOS.Web.UI.DAL
{
    public class DALResult
    {
        SqlDatabaseUtility obj_SqlDatabaseUtility = null;

        public QAData Get_RatingResult(string deptID, string CenterDeptID, string SubDeptID)
        {
            QAData model = new QAData();
            obj_SqlDatabaseUtility = new SqlDatabaseUtility();
            Dictionary<string, SqlParameter> queryParameters = new Dictionary<string, SqlParameter>();
            queryParameters["@deptID"] = new SqlParameter("@deptID", deptID);
            queryParameters["@CenterDeptID"] = new SqlParameter("@CenterDeptID", CenterDeptID);
            queryParameters["@SubDeptID"] = new SqlParameter("@SubDeptID", SubDeptID);
            DataSet ds = new DataSet();
            ds = obj_SqlDatabaseUtility.ExecuteQuery("get_result", queryParameters);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                model.QADataList = Common.ConvertDataTableToList<QAData>(dt);
            }
            return model;
        }

        public DataSet get_emp_staff(int CenterID)
        {
            QAData model = new QAData();
            obj_SqlDatabaseUtility = new SqlDatabaseUtility();
            Dictionary<string, SqlParameter> queryParameters = new Dictionary<string, SqlParameter>();
            queryParameters["@CenterID"] = new SqlParameter("@CenterID", CenterID);
            DataSet ds = new DataSet();
            ds = obj_SqlDatabaseUtility.ExecuteQuery("usp_staff_report", queryParameters);
           
            return ds;
        }

        public  List<EmpDepartmentData> GetEmpDepartment(int UserID, int CategoryID)
        {
            List<EmpDepartmentData> data = new List<EmpDepartmentData>();
            try
            {
                obj_SqlDatabaseUtility = new SqlDatabaseUtility();
                Dictionary<string, SqlParameter> queryParameters = new Dictionary<string, SqlParameter>();
                queryParameters["@EmpID"] = new SqlParameter("@EmpID", UserID);
                queryParameters["@CategoryID"] = new SqlParameter("@CategoryID", CategoryID);
                DataSet ds = new DataSet();
                ds = obj_SqlDatabaseUtility.ExecuteQuery("usp_emp_department", queryParameters);
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    data = Common.ConvertDataTableToList<EmpDepartmentData>(dt);
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}