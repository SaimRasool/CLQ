using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace FOS.Web.UI.DAL
{
    class SqlDatabaseUtility
    {
        public SqlConnection GetConnection()
        {
            string cnstr = ConfigurationManager.ConnectionStrings["FOS_WallCoatConnectionString"].ConnectionString;
            SqlConnection cn = new SqlConnection(cnstr);
            cn.Open();
            return cn;
        }

        /// <summary>
        /// ExecuteCommand method is used to perform Insert/Update/Delete operations on database
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        /// 
        public bool ExecuteCommand(string storedProcName, Dictionary<string, SqlParameter> procParameters)
        {
            try
            {
                int rc;
                using (SqlConnection cn = GetConnection())
                {

                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = storedProcName;

                        foreach (var procParameter in procParameters)
                        {
                            cmd.Parameters.Add(procParameter.Value);
                        }

                        cmd.ExecuteScalar();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public bool ExecuteCommand(string storedProcName, Dictionary<string, SqlParameter> procParameters)
        //{
        //    try
        //    {
        //        int rc;
        //        using (SqlConnection cn = GetConnection())
        //        {
        //            //create a SQL command to execute the stored procedure
        //            using (SqlCommand cmd = cn.CreateCommand())
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.CommandText = storedProcName;
        //                //assign parameters passed in to the command
        //                foreach (var procParameter in procParameters)
        //                {
        //                    cmd.Parameters.Add(procParameter.Value);
        //                }
        //                string someText = storedProcName;
        //                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\csc4.txt", true))
        //                 file.WriteLine(someText);
        //                if (someText == "usp_updatePlotNature")
        //                {
        //                    someText = someText;
        //                }
        //                cmd.ExecuteScalar();
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// ExecuteQuery method is used to perform Get operations on database
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="procParameters"></param>
        /// <returns></returns>
        public DataSet ExecuteQuery(string storedProcName, Dictionary<string, SqlParameter> procParameters)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection cn = GetConnection())
                {
                    //create a SQL command to execute the stored procedure
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = storedProcName;
                        //assign parameters passed in to the command
                        foreach (var procParameter in procParameters)
                        {
                            cmd.Parameters.Add(procParameter.Value);
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}