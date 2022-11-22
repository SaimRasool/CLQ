using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Web.UI.DAL
{
    public class Common
    {
        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            try
            {
                List<T> data = new List<T>();
                foreach (DataRow row in dt.Rows)
                {
                    T item = GetItem<T>(row);
                    data.Add(item);
                }
                return data;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }  
        }

        public static T GetItem<T>(DataRow dr)
        {
            try
            {
                Type temp = typeof(T);
                T obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in dr.Table.Columns)
                {
                    foreach (PropertyInfo pro in temp.GetProperties())
                    {
                        if (pro.Name == column.ColumnName)
                            if (dr[column.ColumnName] != DBNull.Value)
                            {
                                if (pro.PropertyType == typeof(String))
                                    pro.SetValue(obj, Convert.ToString(dr[column.ColumnName]), null);
                                else if (pro.PropertyType.ToString().Contains("Int16"))//(pro.PropertyType == typeof(Int16))
                                    pro.SetValue(obj, Convert.ToInt16(dr[column.ColumnName]), null);
                                else if (pro.PropertyType.ToString().Contains("Int32"))//(pro.PropertyType == typeof(Int32))
                                    pro.SetValue(obj, Convert.ToInt32(dr[column.ColumnName]), null);
                                else if (pro.PropertyType.ToString().Contains("Int64"))//(pro.PropertyType == typeof(Int64))
                                    pro.SetValue(obj, Convert.ToInt64(dr[column.ColumnName]), null);
                                else if (pro.PropertyType.ToString().Contains("Decimal"))//(pro.PropertyType == typeof(Decimal))
                                    pro.SetValue(obj, Convert.ToDecimal(dr[column.ColumnName]), null);
                                else if (pro.PropertyType.ToString().Contains("Boolean"))//(pro.PropertyType == typeof(Boolean))
                                    pro.SetValue(obj, Convert.ToBoolean(dr[column.ColumnName]), null);
                                else if (pro.PropertyType.ToString().Contains("DateTime"))//(pro.PropertyType == typeof(DateTime))
                                    pro.SetValue(obj, Convert.ToDateTime(dr[column.ColumnName]), null);
                                //else if (pro.PropertyType.ToString().Contains("TimeSpan"))//(pro.PropertyType == typeof(TimeSpan))
                                //pro.SetValue(obj, TimeSpan.Parse(dr[column.ColumnName].ToString()), null);
                                else if (pro.PropertyType.ToString().Contains("Char"))//(pro.PropertyType == typeof(DateTime))
                                    pro.SetValue(obj, Convert.ToChar(dr[column.ColumnName]), null);
                                else if (pro.PropertyType.ToString().Contains("TimeSpan"))//(pro.PropertyType == typeof(DateTime))
                                    pro.SetValue(obj, (TimeSpan)dr[column.ColumnName], null);
                            }
                            else
                                continue;
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {


                throw new Exception(ex.Message);
            }
            
        }

        public static string GetFileType(string p_FileName)
        {
            string v_FileType = "data:image/png;base64,";
            if (p_FileName.Contains("gif"))
            {
                v_FileType = "data:image/gif;base64,";
            }
            if (p_FileName.Contains("png"))
            {
                v_FileType = "data:image/png;base64,";
            }
            if (p_FileName.Contains("jpg"))
            {
                v_FileType = "data:image/jpeg;base64,";
            }
            if (p_FileName.Contains("jpeg"))
            {
                v_FileType = "data:image/jpeg;base64,";
            }

            return v_FileType;
        }

      
    }

}