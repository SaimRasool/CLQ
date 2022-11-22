using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using StaffType = FOS.DataLayer.StaffType;

namespace FOS.Setup
{
    public class ManageAdmin
    {
        #region Region
        // Insert OR Update Department ...
        public static int AddUpdateRegion(RegionData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Regions DepartmentObj = new Regions();

                        if (obj.ID == 0)
                        {
                            DepartmentObj.ID = dbContext.Regions.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            DepartmentObj.Name = obj.Name;
                            DepartmentObj.ShortCode = obj.ShortCode;
                            DepartmentObj.CreatedDate = DateTime.Today;
                            DepartmentObj.IsActive = true;
                            dbContext.Regions.Add(DepartmentObj);
                        }
                        else
                        {
                            DepartmentObj = dbContext.Regions.Where(u => u.ID == obj.ID).FirstOrDefault();
                            DepartmentObj.Name = obj.Name;
                            DepartmentObj.ShortCode = obj.ShortCode;
                            DepartmentObj.LastUpdate = DateTime.Today;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Region Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Region"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
        // Delete Region ...
        public static int DeleteRegion(int RegionID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Regions obj = dbContext.Regions.Where(u => u.ID == RegionID).FirstOrDefault();
                    dbContext.Regions.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Department Failed");
                Resp = 1;
            }
            return Resp;
        }


        // Get All Regions For Grid ...
        public static List<RegionData> GetRegionForGrid()
        {
            List<RegionData> RegionData = new List<RegionData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RegionData = dbContext.Regions.Where(u => u.IsActive == true)
                            .ToList().Select(
                                u => new RegionData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    ShortCode = u.ShortCode
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Department List Failed");
                throw;
            }

            return RegionData;
        }


        public static List<RegionData> GetResult(string search, string sortOrder, int start, int length, List<RegionData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<RegionData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<RegionData> FilterResult(string search, List<RegionData> dtResult, List<string> columnFilters)
        {
            IQueryable<RegionData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[2].ToLower())))

                );

            return results;
        }

        #endregion

        #region Department
        // Insert OR Update Department ...
        public static int AddUpdateCategory(AdminData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Department DepartmentObj = new Department();

                        if (obj.ID == 0)
                        {
                            DepartmentObj.ID = dbContext.Departments.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            DepartmentObj.Name = obj.Name;
                            DepartmentObj.IsActive = true;
                            dbContext.Departments.Add(DepartmentObj);
                        }
                        else
                        {
                            DepartmentObj = dbContext.Departments.Where(u => u.ID == obj.ID).FirstOrDefault();
                            DepartmentObj.Name = obj.Name;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Region Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Region"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
        // Delete Region ...
        public static int DeleteCategory(int DepartmentID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Department obj = dbContext.Departments.Where(u => u.ID == DepartmentID).FirstOrDefault();
                    dbContext.Departments.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Department Failed");
                Resp = 1;
            }
            return Resp;
        }


        // Get All Regions For Grid ...
        public static List<AdminData> GetCategoryForGrid()
        {
            List<AdminData> DepartmentData = new List<AdminData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    DepartmentData = dbContext.Departments.Where(u => u.IsActive == true)
                            .ToList().Select(
                                u => new AdminData
                                {
                                    ID = u.ID,
                                    Name = u.Name
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Department List Failed");
                throw;
            }

            return DepartmentData;
        }


        public static List<AdminData> GetResult(string search, string sortOrder, int start, int length, List<AdminData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<AdminData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<AdminData> FilterResult(string search, List<AdminData> dtResult, List<string> columnFilters)
        {
            IQueryable<AdminData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[2].ToLower())))

                );

            return results;
        }
        #endregion

        #region Sub Department
        public static List<Department> GetAllDepartment()
        {
            List<Department> data = new List<Department>();
            try
            {

                using (FOSDataModel db = new FOSDataModel())
                {
                    data = db.Departments.ToList();
                    //data = db.Departments.Where(u => u.IsActive == true).Select(u => new AdminData
                    //{
                    //    ID = u.ID,
                    //    Name = u.Name
                    //}).ToList();

                }
                data.Insert(0, new Department
                {
                    ID = 0,
                    Name = "--Select Department--"
                });
                return data;
            }
            catch (Exception ex) { }
            return data;
        }
        public static List<AdminData> GetSubDepartmentDepartmentAndRegionWise(int DeptID, int RegionID)
        {
            List<AdminData> obj = new List<AdminData>();
            using (FOSDataModel db = new FOSDataModel())
            {
                obj = db.SubDepartments.Where(u => u.DeptID == DeptID && u.RegionsID == RegionID).Select(x => new AdminData
                {
                    ID = x.ID,
                    Name = x.Name
                }).ToList();
                obj.Insert(0, new AdminData
                {
                    ID = 0,
                    Name = "--Select Center--"
                });
            }
            return obj;
        }
        public static List<Regions> GetAllRegions()
        {
            List<Regions> data = new List<Regions>();
            try
            {

                using (FOSDataModel db = new FOSDataModel())
                {
                    data = db.Regions.ToList();
                    //data = db.Departments.Where(u => u.IsActive == true).Select(u => new AdminData
                    //{
                    //    ID = u.ID,
                    //    Name = u.Name
                    //}).ToList();

                }
                data.Insert(0, new Regions
                {
                    ID = 0,
                    Name = "--Select Regions--"
                });
                return data;
            }
            catch (Exception ex) { }
            return data;
        }
        // Insert OR Update Department ...
        public static int AddUpdateSubDepartment(SubDepartmentData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        SubDepartment DepartmentObj = new SubDepartment();

                        if (obj.ID == 0)
                        {
                            DepartmentObj.ID = dbContext.SubDepartments.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            DepartmentObj.Name = obj.SubDepartment;
                            DepartmentObj.Name = obj.Name;
                            DepartmentObj.Email = obj.Email;
                            DepartmentObj.Address = obj.Address;
                            DepartmentObj.Telephone = obj.Telephone;
                            DepartmentObj.Fax = obj.Fax;
                            DepartmentObj.DeptID = obj.DeptID;
                            DepartmentObj.RegionsID = obj.RegionID;
                            DepartmentObj.IsActive = true;
                            DepartmentObj.IsDeleted = false;
                            DepartmentObj.CreatedOn = DateTime.Now;
                            DepartmentObj.Password = obj.Password;
                            dbContext.SubDepartments.Add(DepartmentObj);
                        }
                        else
                        {
                            DepartmentObj = dbContext.SubDepartments.Where(u => u.ID == obj.ID).FirstOrDefault();
                            DepartmentObj.ID = obj.ID;
                            DepartmentObj.Name = obj.SubDepartment;
                            DepartmentObj.Email = obj.Email;
                            DepartmentObj.Password = obj.Password;
                            DepartmentObj.Address = obj.Address;
                            DepartmentObj.Telephone = obj.Telephone;
                            DepartmentObj.Fax = obj.Fax;
                            DepartmentObj.DeptID = obj.DeptID;
                            DepartmentObj.RegionsID = obj.RegionID;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Region Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Region"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
        // Delete Region ...
        public static int DeleteSubDepartment(int DepartmentID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SubDepartment obj = dbContext.SubDepartments.Where(u => u.ID == DepartmentID).FirstOrDefault();
                    dbContext.SubDepartments.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete SubDepartment Failed");
                Resp = 1;
            }
            return Resp;
        }


        // Get All Regions For Grid ...
        public static List<SubDepartmentData> GetSubDepartmentForGrid()
        {
            List<SubDepartmentData> DepartmentData = new List<SubDepartmentData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    DepartmentData = dbContext.GetSubDepartment()
                            .ToList().Select(
                                u => new SubDepartmentData
                                {
                                    ID = u.ID,
                                    SubDepartment = u.SubDepartment,
                                    Department = u.Department,
                                    Region = u.Region
                                    //Name = u.Name
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Department List Failed");
                throw;
            }

            return DepartmentData;
        }


        public static List<SubDepartmentData> GetResultSubDepartment(string search, string sortOrder, int start, int length, List<SubDepartmentData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int CountSubDepartment(string search, List<SubDepartmentData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<SubDepartmentData> FilterResult(string search, List<SubDepartmentData> dtResult, List<string> columnFilters)
        {
            IQueryable<SubDepartmentData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.SubDepartment != null && p.SubDepartment.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.SubDepartment != null && p.SubDepartment.ToLower().Contains(columnFilters[2].ToLower())))

                );

            return results;
        }
        #endregion

        #region CenterDepartment
        // Insert OR Update Department ...
        public static int AddUpdateDepartment(DepartmentData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        CenterDepartment DepartmentObj = new CenterDepartment();

                        if (obj.ID == 0)
                        {
                            DepartmentObj.ID = dbContext.CenterDepartments.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            DepartmentObj.CategoryID = obj.CategoryID;
                            DepartmentObj.CenterID = obj.CenterID;
                            DepartmentObj.OnCreated = DateTime.Now;
                            DepartmentObj.IsActive = true;
                            DepartmentObj.IsDeleted = false;
                            dbContext.CenterDepartments.Add(DepartmentObj);
                        }
                        else
                        {
                            DepartmentObj = dbContext.CenterDepartments.Where(u => u.ID == obj.ID).FirstOrDefault();
                            DepartmentObj.Name = obj.Name;
                            DepartmentObj.CategoryID = obj.CategoryID;
                            DepartmentObj.CenterID = obj.CenterID;
                            DepartmentObj.OnCreated = DateTime.Now;
                            DepartmentObj.IsActive = true;
                            DepartmentObj.IsDeleted = false;
                            dbContext.CenterDepartments.Add(DepartmentObj);
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Department Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Department"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
        // Delete Region ...
        public static int DeleteDepartment(int DepartmentID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    CenterDepartment obj = dbContext.CenterDepartments.Where(u => u.ID == DepartmentID).FirstOrDefault();
                    dbContext.CenterDepartments.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Department Failed");
                Resp = 1;
            }
            return Resp;
        }


        // Get All Department For Grid ...
        public static List<DepartmentData> GetDepartmentForGrid()
        {
            List<DepartmentData> DepartmentData = new List<DepartmentData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    DepartmentData = dbContext.CenterDepartmentList1_1()
                            .ToList().Select(
                                u => new DepartmentData
                                {
                                    ID = u.ID,
                                    CategoryName = u.Category,
                                    CenterName = u.Center,
                                    Name = u.Department
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Department List Failed");
                throw;
            }

            return DepartmentData;
        }


        public static List<DepartmentData> GetResult(string search, string sortOrder, int start, int length, List<DepartmentData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<DepartmentData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }



        private static IQueryable<DepartmentData> FilterResult(string search, List<DepartmentData> dtResult, List<string> columnFilters)
        {
            IQueryable<DepartmentData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[2].ToLower())))

                );

            return results;
        }
        #endregion

        #region QA
        public static List<QAData> GetQAForGrid()
        {
            List<QAData> EmpData = new List<QAData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //DepartmentData = dbContext.GetDoctorForGrid1_2()
                    EmpData = dbContext.QAs.Select(
                                u => new QAData
                                {
                                    QAID = u.ID,
                                    QuestionName = u.Question,
                                    QuestionHint = u.QuestionHint
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get QA List Failed");
                throw;
            }

            return EmpData;
        }
        public static List<SubDepartmentData> GetSubDepartment()
        {
            List<SubDepartmentData> objQAData = new List<SubDepartmentData>();
            using (FOSDataModel db = new FOSDataModel())
            {
                objQAData = db.SubDepartments.Select(u => new SubDepartmentData
                {
                    ID = u.ID,
                    SubDepartment = u.Name
                }).ToList();
                objQAData.Insert(0, new SubDepartmentData
                {
                    ID = 0,
                    SubDepartment = "--Select SubDepartment--"
                });
            }
            return objQAData;
        }
        public static List<SubDepartmentData> GetAllSubDepartment()
        {
            List<SubDepartmentData> objQAData = new List<SubDepartmentData>();
            using (FOSDataModel db = new FOSDataModel())
            {

                objQAData.Insert(0, new SubDepartmentData
                {
                    ID = 0,
                    SubDepartment = "--Select Center--"
                });
            }
            return objQAData;
        }
        public static List<CenterDepartmentData> GetAllCenterDepartment()
        {
            List<CenterDepartmentData> objQAData = new List<CenterDepartmentData>();
            using (FOSDataModel db = new FOSDataModel())
            {

                objQAData.Insert(0, new CenterDepartmentData
                {
                    ID = 0,
                    Name = "--Select Department--"
                });
            }
            return objQAData;
        }
        public static int DeleteQA(int ID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    QA obj = dbContext.QAs.Where(u => u.ID == ID).FirstOrDefault();
                    dbContext.QAs.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete QA Failed");
                Resp = 1;
            }
            return Resp;
        }

        //public static List<DepartmentData> GetAllDepartment()
        //{
        //    List<DepartmentData> objQAData = new List<DepartmentData>();
        //    using (FOSDataModel db = new FOSDataModel())
        //    {
        //        objQAData = db.Departments.Select(u => new DepartmentData
        //        {
        //            ID = u.Department_ID,
        //            Department = u.Department_Name
        //        }).ToList();
        //        objQAData.Insert(0, new DepartmentData
        //        {
        //            ID = 0,
        //            Department = "--Select Department--"
        //        });
        //    }
        //    return objQAData;
        //}
        public static List<AdminData> GetSubDepartmentDepartmentWise(int DeptID)
        {
            List<AdminData> obj = new List<AdminData>();
            using (FOSDataModel db = new FOSDataModel())
            {
                obj = db.SubDepartments.Where(u => u.DeptID == DeptID).Select(x => new AdminData
                {
                    ID = x.ID,
                    Name = x.Name
                }).ToList();
                obj.Insert(0, new AdminData
                {
                    ID = 0,
                    Name = "--Select Center--"
                });
            }
            return obj;
        }
        public static List<AdminData> GetCenterDepartmentDepartmentWise(int CategoryID, int EmpID)
        {
            List<AdminData> obj = new List<AdminData>();
            using (FOSDataModel db = new FOSDataModel())
            {
                if (EmpID != 1)
                {
                    obj = db.GetDepartmentCategoryWise(CategoryID, EmpID).Select(x => new AdminData
                    {
                        ID = x.ID,
                        Name = x.Name
                    }).ToList();
                }
                else
                {
                    obj = db.CenterDepartments.Where(x => x.CategoryID == CategoryID).Select(x => new AdminData
                    {
                        ID = x.ID,
                        Name = x.Name
                    }).ToList();
                }
                obj.Insert(0, new AdminData
                {
                    ID = 0,
                    Name = "--Select Department--"
                });
            }
            return obj;
        }

        public static int AddUpdateQA(QAData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        QA QAObj = new QA();

                        if (obj.QAID == 0)
                        {
                            QAObj.ID = dbContext.QAs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            QAObj.CategoryID = obj.DeptID;
                            QAObj.CenterID = obj.SubDeptID;
                            QAObj.DeptID = obj.CenterDeptID;
                            QAObj.Question = obj.QuestionName;
                            QAObj.QuestionHint = obj.QuestionHint;
                            QAObj.IsActive = true;
                            QAObj.IsDeleted = false;
                            QAObj.OnCreated = DateTime.Now;
                            dbContext.QAs.Add(QAObj);
                        }
                        else
                        {
                            QAObj = dbContext.QAs.Where(u => u.ID == obj.QAID).FirstOrDefault();
                            QAObj.CategoryID = obj.DeptID;
                            QAObj.CenterID = obj.SubDeptID;
                            QAObj.DeptID = obj.CenterDeptID;
                            QAObj.Question = obj.QuestionName;
                            QAObj.QuestionHint = obj.QuestionHint;
                            QAObj.IsActive = true;
                            QAObj.IsDeleted = false;
                            QAObj.OnCreated = DateTime.Now;
                            dbContext.SaveChanges();

                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add QA Failed");
                Res = 0;

                return Res;
            }
            return Res;
        }
        public static List<QAData> GetResultQA(string search, string sortOrder, int start, int length, List<QAData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int CountQA(string search, List<QAData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<QAData> FilterResult(string search, List<QAData> dtResult, List<string> columnFilters)
        {
            IQueryable<QAData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.QuestionName != null && p.QuestionName.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.QuestionName != null && p.QuestionName.ToLower().Contains(columnFilters[2].ToLower())))

                );

            return results;
        }
        #endregion

        #region Rating
        public static List<RatingData> GetQList(int CategoryID, int CenterID, int DeptID,int audit_id)
        {
            List<RatingData> QData = new List<RatingData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    QData = dbContext.GetQuestionList1_1(CategoryID, CenterID, DeptID,audit_id).Select(
                                u => new RatingData
                                {
                                    RID = u.ID,
                                    RName = u.Question,
                                    RHint = u.QuestionHint
                                }).ToList();

                    //QData = dbContext.QAs.Where(x=> x.CategoryID == CategoryID && x.CenterID == CenterID && x.DeptID == DeptID).Select(
                    //            u => new RatingData
                    //            {
                    //                RID = u.ID,
                    //                RName = u.Question
                    //            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get QA List Failed");
                throw;
            }

            return QData;
        }
        #endregion

        #region UpdateRating
        public static List<RatingData> GetQListForUpdate(int CategoryID, int CenterID, int DeptID,int audit_id)
        {
            List<RatingData> QData = new List<RatingData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    QData = dbContext.GetQuestionListForEdit1_1(CategoryID, CenterID, DeptID, audit_id).Select(
                                u => new RatingData
                                {
                                    TRQID = u.TRQID,
                                    RID = u.ID,
                                    RName = u.Question,
                                    RHint = u.QuestionHint,
                                    RatedNumber = u.RatedValue,
                                    remarks = u.remarks
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get QA List Failed");
                throw;
            }

            return QData;
        }
        public static List<RatingData> GetQListForUpdateRequest(int CategoryID, int CenterID, int DeptID, int audit_id)
        {
            List<RatingData> QData = new List<RatingData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    QData = dbContext.GetQuestionListForUpdate1_1(CategoryID, CenterID, DeptID,audit_id).Select(
                                u => new RatingData
                                {
                                    RID = u.ID,
                                    RName = u.Question,
                                    RHint = u.QuestionHint,
                                    RatedNumber = u.RatedValue,
                                    remarks = u.remarks
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get QA List Failed");
                throw;
            }

            return QData;
        }
        #endregion

        #region Rating Result
        public static List<GetCLRList1_1_Result> GetCLRForGrid3(int CategoryID, int CenterID, int audit_id)
        {
            List<GetCLRList1_1_Result> dtsource = new List<GetCLRList1_1_Result>();
            try
            {
                FOSDataModel db = new FOSDataModel();
                //List<QAData> dtsource = new List<QAData>();
                var CD = db.CenterDepartments.Where(x => x.CategoryID == CategoryID).OrderBy(x => x.Name).Select(x => x.ID).ToList();
                decimal? TMC = 0;
                decimal? OM = 0;
                foreach (var DeptID in CD)
                {
                    //obj = new List<QAData>();
                    var data1 = db.GetCLRList1_1(CategoryID, CenterID, DeptID, audit_id).Select(x => new GetCLRList1_1_Result
                    {
                        Category = x.Category,
                        Center = x.Center,
                        Department = x.Department,
                        TotalNumber = x.TotalNumber,
                        ObtainedMarks = x.ObtainedMarks,
                        ObtainedPercentage = x.ObtainedPercentage,
                        NotApplicable = x.NotApplicable,
                        FullPer = x.FullPer,
                        PartialPer = x.PartialPer,
                        Observation_dt = x.Observation_dt,
                        Recomendation = x.Recomendation,
                        NotPer = x.NotPer
                    }).FirstOrDefault();

                    if (data1 != null)
                    {
                        TMC = TMC + data1.TotalNumber;
                        OM = OM + data1.ObtainedMarks;
                        dtsource.Add(data1);
                    }
                }
                if (OM != 0 && TMC != 0)
                {
                    decimal? TOMP1 = OM / TMC;
                    decimal? TOMP = TOMP1 * 100;
                }

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Department List Failed");
                throw;
            }

            return dtsource;
        }
        public static List<GetSummaryCLRList1_1_Result> GetSummaryCLRForGrid1(int CategoryID, int CenterID, int audit_id)
        {
            List<GetSummaryCLRList1_1_Result> dtsource = new List<GetSummaryCLRList1_1_Result>();
            try
            {
                FOSDataModel db = new FOSDataModel();
                //List<QAData> data1;
                //var CD = db.CenterDepartments.Where(x => x.CategoryID == CategoryID).Select(x => x.ID).ToList();
                //foreach (var DeptID in CD)
                //{
                //data1 = new List<QAData>();
                dtsource = db.GetSummaryCLRList1_1(CategoryID, CenterID, audit_id).Select(x => new GetSummaryCLRList1_1_Result
                {
                    Category = x.Category,
                    Center = x.Center,
                    Department = x.Department,
                    Question = x.Question,
                    RatedMarks = x.RatedMarks,
                }).ToList();
                //dtsource.Add(data1);
                //}          

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Department List Failed");
                throw;
            }

            return dtsource;
        }
        public static List<GetSummaryCLRList1_2_Result> GetAllQuestionsForGrid1(int CategoryID, int CenterID,int audit_id)
        {
            List<GetSummaryCLRList1_2_Result> dtsource = new List<GetSummaryCLRList1_2_Result>();
            try
            {
                FOSDataModel db = new FOSDataModel();
                //List<QAData> data1;
                //var CD = db.CenterDepartments.Where(x => x.CategoryID == CategoryID).Select(x => x.ID).ToList();
                //foreach (var DeptID in CD)
                //{
                //data1 = new List<QAData>();
                dtsource = db.GetSummaryCLRList1_2(CategoryID, CenterID, audit_id).Select(x => new GetSummaryCLRList1_2_Result
                {
                    Department = x.Department,
                    Question = x.Question
                }).ToList();
                //dtsource.Add(data1);
                //}          

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Department List Failed");
                throw;
            }

            return dtsource;
        }
        public static List<QAData> GetCLRForGrid(int CategoryID, int CenterID, int audit_id)
        {
            List<QAData> dtsource = new List<QAData>();
            try
            {
                FOSDataModel db = new FOSDataModel();
                //List<QAData> dtsource = new List<QAData>();
                var CD = db.CenterDepartments.Where(x => x.CategoryID == CategoryID).Select(x => x.ID).ToList();
                decimal? TMC = 0;
                decimal? OM = 0;
                foreach (var DeptID in CD)
                {
                    //obj = new List<QAData>();
                    var data1 = db.GetCLRList1_1(CategoryID, CenterID, DeptID, audit_id).Select(x => new QAData
                    {
                        Category = x.Category,
                        Center = x.Center,
                        Department = x.Department,
                        TotalMarks = x.TotalNumber,
                        ObtainedMarks = x.ObtainedMarks,
                        ObtainedPercentage = x.ObtainedPercentage,
                        NotApplicable = x.NotApplicable
                    }).FirstOrDefault();

                    if (data1 != null)
                    {
                        TMC = TMC + data1.TotalMarks;
                        OM = OM + data1.ObtainedMarks;
                        dtsource.Add(data1);
                    }
                }
                //if (OM != 0 && TMC != 0)
                //{
                //    decimal? TOMP1 = OM / TMC;
                //    decimal? TOMP = TOMP1 * 100;
                //}

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Department List Failed");
                throw;
            }

            return dtsource;
        }
        public static List<QAData> GetResultR(string search, string sortOrder, int start, int length, List<QAData> dtResult, List<string> columnFilters)
        {
            return FilterResultR(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }
        public static int CountR(string search, List<QAData> dtResult, List<string> columnFilters)
        {
            return FilterResultR(search, dtResult, columnFilters).Count();
        }
        private static IQueryable<QAData> FilterResultR(string search, List<QAData> dtResult, List<string> columnFilters)
        {
            IQueryable<QAData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Department != null && p.Department.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Department != null && p.Department.ToLower().Contains(columnFilters[2].ToLower())))

                );

            return results;
        }
        #endregion

        #region Access


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
        #endregion

        #region Staff
        public static List<StaffType> GetAllStaffType()
        {
            List<StaffType> data = new List<StaffType>();
            try
            {

                using (FOSDataModel db = new FOSDataModel())
                {
                    data = db.StaffTypes.ToList();
                }
                data.Insert(0, new StaffType
                {
                    ID = 0,
                    Name = "--Select Staff Type--"
                });
                return data;
            }
            catch (Exception ex) { }
            return data;
        }

        public static List<StaffSubTypeData> GetSubStaffListStaffWise(int StaffTypeId)
        {
            List<StaffSubTypeData> obj = new List<StaffSubTypeData>();
            using (FOSDataModel db = new FOSDataModel())
            {
                obj = db.StaffSubTypes.Where(u => u.StaffTypeId == StaffTypeId).Select(x => new StaffSubTypeData
                {
                    ID = x.ID,
                    Name = x.Name
                }).ToList();
                obj.Insert(0, new StaffSubTypeData
                {
                    ID = 0,
                    Name = "--Select Sub Staff--"
                });
            }
            return obj;
        }

        // Insert OR Update Department ...
        public static int AddUpdateStaffDetail(StaffDetailData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        staff_detail StafObj = new staff_detail();
                        staff_detail tempObj = new staff_detail();
                        tempObj = dbContext.staff_detail.Where(u => u.CenterID == obj.CenterID && u.sub_staff_id == obj.sub_staff_id).FirstOrDefault();
                        if (tempObj != null)
                            obj.id = tempObj.id;
                        if (obj.id == 0)
                        {
                            StafObj.male = obj.male;
                            StafObj.female = obj.female;
                            StafObj.CenterID = obj.CenterID;
                            StafObj.part_time = obj.part_time;
                            StafObj.full_time = obj.full_time;
                            StafObj.visiting = obj.visiting;
                            StafObj.sub_staff_id = obj.sub_staff_id;
                            dbContext.staff_detail.Add(StafObj);
                        }
                        else
                        {
                            StafObj = dbContext.staff_detail.Where(u => u.id == obj.id).FirstOrDefault();
                            StafObj.id = obj.id;
                            StafObj.male = obj.male;
                            StafObj.female = obj.female;
                            StafObj.CenterID = obj.CenterID;
                            StafObj.part_time = obj.part_time;
                            StafObj.full_time = obj.full_time;
                            StafObj.visiting = obj.visiting;
                            StafObj.sub_staff_id = obj.sub_staff_id;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Region Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Region"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static int DeleteStaffDetail(int id)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    staff_detail obj = dbContext.staff_detail.Where(u => u.id == id).FirstOrDefault();
                    dbContext.staff_detail.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Department Failed");
                Resp = 1;
            }
            return Resp;
        }

        // Get All Observation For Grid ...
        public static List<StaffDetailData> GetStaffDetailForGrid()
        {
            List<StaffDetailData> StaffData = new List<StaffDetailData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    StaffData = dbContext.usp_get_StaffDetail()
                            .ToList().Select(
                                u => new StaffDetailData
                                {
                                    id = (int)u.id,
                                    SubDeptName = u.SubDeptName,
                                    Name = u.Name,
                                    male = (int)u.male,
                                    female = (int)u.female,
                                    visiting = (int)u.visiting,
                                    part_time = (int)u.part_time,
                                    full_time = (int)u.full_time
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Department List Failed");
                throw;
            }

            return StaffData;
        }


        public static List<StaffDetailData> GetResult(string search, string sortOrder, int start, int length, List<StaffDetailData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<StaffDetailData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<StaffDetailData> FilterResult(string search, List<StaffDetailData> dtResult, List<string> columnFilters)
        {
            IQueryable<StaffDetailData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower())) || (p.SubDeptName != null && p.SubDeptName.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[2].ToLower())) || (p.SubDeptName != null && p.SubDeptName.ToLower().Contains(columnFilters[2].ToLower())))

                );

            return results;
        }
        #endregion

        #region Observation&Recommendation
        // Insert OR Update Department ...
        public static int AddUpdateObservation(ObservationData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Observation ObservationObj = new Observation();
                        Observation ObservationObj1 = new Observation();
                        ObservationObj1 = dbContext.Observation.Where(u => u.CenterID == obj.SubDeptID).FirstOrDefault();
                        if (ObservationObj1 != null && obj.ID == 0)
                        {
                            Log.Instance.Error("Observation Already Exist");
                            Res = 0;
                        }
                        else
                        {
                            if (obj.ID == 0)
                            {
                                ObservationObj.ID = dbContext.Observation.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                ObservationObj.CenterID = obj.SubDeptID;
                                ObservationObj.Observation_dt = obj.Observation;
                                ObservationObj.Recomendation = obj.Recomendation;
                                ObservationObj.OnCreated = DateTime.Now;
                                ObservationObj.AuditID = dbContext.Audit.Where(u => u.audit_year == obj.AuditYear && u.is_active == true).Select(u => u.audit_id).FirstOrDefault();
                                dbContext.Observation.Add(ObservationObj);
                            }
                            else
                            {
                                ObservationObj = dbContext.Observation.Where(u => u.ID == obj.ID).FirstOrDefault();
                                ObservationObj.CenterID = obj.SubDeptID;
                                ObservationObj.Observation_dt = obj.Observation;
                                ObservationObj.Recomendation = obj.Recomendation;
                                ObservationObj.AuditID = dbContext.Audit.Where(u => u.audit_year == obj.AuditYear && u.is_active == true).Select(u => u.audit_id).FirstOrDefault();
                                ObservationObj.OnCreated = DateTime.Now;
                            }
                            dbContext.SaveChanges();
                            Res = 1;
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Department Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Department"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
        // Delete Region ...
        public static int DeleteObservation(int DepartmentID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Observation obj = dbContext.Observation.Where(u => u.ID == DepartmentID).FirstOrDefault();
                    dbContext.Observation.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Department Failed");
                Resp = 1;
            }
            return Resp;
        }


        // Get All Observation For Grid ...
        public static List<ObservationData> GetObservationForGrid()
        {
            List<ObservationData> ObservationData = new List<ObservationData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    ObservationData = dbContext.usp_get_observation1()
                            .ToList().Select(
                                u => new ObservationData
                                {
                                    ID = (int)u.ID,
                                    CenterDeptName = u.CenterDeptName,
                                    Observation = u.Observation_dt,
                                    Recomendation = u.Recomendation
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Department List Failed");
                throw;
            }

            return ObservationData;
        }


        public static List<ObservationData> GetResult(string search, string sortOrder, int start, int length, List<ObservationData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<ObservationData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<ObservationData> FilterResult(string search, List<ObservationData> dtResult, List<string> columnFilters)
        {
            IQueryable<ObservationData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Observation != null && p.Observation.ToLower().Contains(search.ToLower())) || (p.Recomendation != null && p.Recomendation.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Observation != null && p.Observation.ToLower().Contains(columnFilters[2].ToLower())) || (p.Recomendation != null && p.Recomendation.ToLower().Contains(columnFilters[2].ToLower())))

                );

            return results;
        }
        #endregion

        #region AuditProfile
        // Insert OR Update Department ...
        public static int AddUpdateAudit(Audit obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Audit AuditObj = new Audit();
                        Audit AuditObj1 = new Audit();
                        AuditObj1 = dbContext.Audit.Where(u => u.audit_year == obj.audit_year && u.is_active == true).FirstOrDefault();
                        if (AuditObj1 != null && obj.audit_id == 0)
                        {
                            Log.Instance.Error("Audit Year Already Exist");
                            Res = 0;
                        }
                        else
                        {
                            if (obj.audit_id == 0)
                            {
                                AuditObj.audit_id = dbContext.Audit.OrderByDescending(u => u.audit_id).Select(u => u.audit_id).FirstOrDefault() + 1;
                                AuditObj.audit_year = obj.audit_year;
                                AuditObj.starting_date = obj.starting_date;
                                AuditObj.is_active = true;
                                AuditObj.description = obj.description;
                                dbContext.Audit.Add(AuditObj);
                            }
                            else
                            {
                                AuditObj = dbContext.Audit.Where(u => u.audit_id == obj.audit_id).FirstOrDefault();
                                AuditObj.audit_year = obj.audit_year;
                                AuditObj.starting_date = obj.starting_date;
                                AuditObj.is_active = true;
                                AuditObj.description = obj.description;
                            }
                            dbContext.SaveChanges();
                            Res = 1;
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Department Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Department"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
        // Delete Region ...
        public static int DeleteAudit(int audit_id)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Audit obj = dbContext.Audit.Where(u => u.audit_id == audit_id).FirstOrDefault();
                    dbContext.Audit.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Department Failed");
                Resp = 1;
            }
            return Resp;
        }

        public static int UpdateAuditStatus(int audit_id)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Audit obj = dbContext.Audit.Where(u => u.audit_id == audit_id).FirstOrDefault();
                    obj.is_active = false;
                    obj.closing_date = DateTime.Now;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Department Failed");
                Resp = 1;
            }
            return Resp;
        }

        // Get All Observation For Grid ...
        public static List<Audit> GetAuditForGrid()
        {
            List<Audit> AuditData = new List<Audit>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    AuditData = dbContext.Audit.ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Department List Failed");
                throw;
            }

            return AuditData;
        }

        public static List<Audit> GetResult(string search, string sortOrder, int start, int length, List<Audit> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<Audit> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<Audit> FilterResult(string search, List<Audit> dtResult, List<string> columnFilters)
        {
            IQueryable<Audit> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.audit_year != null && p.audit_year.ToString().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.audit_year != null && p.audit_year.ToString().Contains(columnFilters[2].ToLower())))

                );

            return results;
        }
        #endregion
    }
}
