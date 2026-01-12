using HRM.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
using ClosedXML.Excel;
using System.Data;
using System.IO;

namespace HRM.Controllers
{
    public class AccountsController : Controller
    {
        // GET: Accounts        
        HRMEntities hrentity = new HRMEntities();
        //list declaration for year and month
        List<SelectListItem> ddlMonths = new List<SelectListItem>();
        List<SelectListItem> ddlYears = new List<SelectListItem>();
        public ActionResult AccountView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AccountView(FormCollection formdata)
        {
            List<spGetdoj_Result> objreport = new List<spGetdoj_Result>();
            List<spGetdoj123_Result> objreport1 = new List<spGetdoj123_Result>();
            int code = Convert.ToInt32(Session["id"].ToString());         
            var dtrange = formdata["dtraneguser"].ToString();
            string NSdate = dtrange.Split(' ')[0];
            String[] Edate = dtrange.Split('-');
            string NEdate = string.Empty;
            foreach (String word in Edate)
            {
                NEdate = (word);
            }
            DateTime NewSdate1 = DateTime.ParseExact(NSdate.Trim().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);    
            DateTime NewEdate1 = DateTime.ParseExact(NEdate.Trim().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string fmdate = NewSdate1.ToString("dd/MM/yyyy");
            string todate = NewEdate1.ToString("dd/MM/yyyy");
            string FSdate = NewSdate1.ToString("yyyy-MM-dd HH:mm");
            string FEdate = NewEdate1.ToString("yyyy-MM-dd HH:mm");
            Session["Sdate"] = FSdate;
            Session["Edate"] = FEdate;
            if (FSdate != null && FEdate != null)
            {

                objreport1 = (from e in hrentity.spGetdoj123(FSdate, FEdate, code) select e).ToList();
                if (objreport1.Count() != 0)
                {
                    //objreport = selectedlist.ToList();
                    TempData["Success"] = "Found " + " " + objreport1.Count() + " joining from date : " + fmdate + " to " + todate;
                    //TempData["Selecteddate"]="Selected date range From "+ NewSdate1 + "To " + NewEdate1;
                    return View(objreport1);

                }
                else
                {
                    TempData["Empty"] = "No record found from date: " + fmdate + " To " + todate;
                    return View();
                }
                //return View();
            }
            else
            {
                TempData["Error"] = "Please select date range";
                return View();
            }
        }

        //Export corner
        public ActionResult ExcelExport()
        {
            string FSdate = Session["Sdate"].ToString();
            string FEdate = Session["Edate"].ToString();
            //Session.Remove("Sdate");
            //Session.Remove("Edate");
            int code = Convert.ToInt32(Session["id"].ToString());
            if (FSdate != null && FEdate != null)
            {

                var selectedlist = (from e in hrentity.spGetdoj123(FSdate, FEdate, code) select e).ToList();
                if (selectedlist.Count() != 0)
                {
                    DataTable dt = new DataTable("Grid");
                    dt.Columns.AddRange(new DataColumn[24]
                    {
                       new DataColumn("AadhaarNo"),
                        new DataColumn("EmpCode"),
                        new DataColumn("Employee Name"),
                        new DataColumn("Department"),
                        new DataColumn("Designation"),
                        new DataColumn("Is Replacement"),
                        new DataColumn("DOJ"),
                        new DataColumn("Salary"),
                        new DataColumn("Experience"),
                        new DataColumn("DOB"),
                        new DataColumn("Marital Status"),
                        new DataColumn("Gender"),
                        new DataColumn("Mobile"),
                        new DataColumn("Father"),
                        new DataColumn("Mother"),
                        new DataColumn("Spouse"),
                        new DataColumn("TempCity"),
                        new DataColumn("TempState"),
                        new DataColumn("TempPin"),
                        new DataColumn("TempAddress"),
                        new DataColumn("City"),
                        new DataColumn("State"),
                        new DataColumn("Pin"),
                        new DataColumn("Address")
                    });
                    foreach (var emp in selectedlist)
                    {
                        dt.Rows.Add(emp.vchAadharNo, emp.vchEmpFcode, emp.vchName, emp.vchdeptname, emp.vchdesignation, emp.vchRplcmntName, emp.dtDOJ.ToString("d"), emp.intsalary, emp.decExperience, emp.dtDob, emp.Mstatus, 
                         emp.vchsex,emp.vchMobile,emp.vchFatherName,emp.vchmothername,emp.vchspouse, emp.TempCity,emp.TempState,emp.TempPin,emp.TempAddress,emp.City,emp.State,emp.Pin,emp.Address);
                    }
                    using (XLWorkbook eb = new XLWorkbook())
                    {
                        eb.Worksheets.Add(dt);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            eb.SaveAs(ms);
                            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Joining.xlsx");
                        }
                    }
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult DeactiveView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeactiveView(FormCollection formdata)
        {
            IEnumerable<spGetdol_Result> objreport = new List<spGetdol_Result>();
            Int32? code = Convert.ToInt32(Session["id"].ToString());
            var dtrange = formdata["dtraneguser"].ToString();
            string NSdate = dtrange.Split(' ')[0];
            String[] Edate = dtrange.Split('-');
            string NEdate = string.Empty;
            foreach (String word in Edate)
            {
                NEdate = (word);

            }
            DateTime NewSdate1 = DateTime.ParseExact(NSdate.Trim().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime NewEdate1 = DateTime.ParseExact(NEdate.Trim().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string fmdate = NewSdate1.ToString("dd/MM/yyyy");
            string todate = NewEdate1.ToString("dd/MM/yyyy");
            string FSdate = NewSdate1.ToString("yyyy-MM-dd HH:mm");
            string FEdate = NewEdate1.ToString("yyyy-MM-dd HH:mm");
            Session["Sdate"] = FSdate;
            Session["Edate"] = FEdate;
            if (FSdate != null && FEdate != null)
            {

                objreport = (from e in hrentity.spGetdol(FSdate, FEdate, code) select e).ToList();
                if (objreport.Count() != 0)
                {
                    //objreport = selectedlist.ToList();
                    TempData["Success"] = "Found " + " " + objreport.Count() + " left employee from date : " + fmdate + " to " + todate;
                    //TempData["Selecteddate"]="Selected date range From "+ NewSdate1 + "To " + NewEdate1;
                    return View(objreport);

                }
                else
                {
                    TempData["Empty"] = "No record found from date: " + fmdate + " To " + todate;
                    return View();
                }
                //return View();
            }
            else
            {
                TempData["Error"] = "Please select date range";
                return View();
            }
        }

        public ActionResult ExcelExportDOL()
        {
            string FSdate = Session["Sdate"].ToString();
            string FEdate = Session["Edate"].ToString();
            //Session.Remove("Sdate");
            //Session.Remove("Edate");
            int code = Convert.ToInt32(Session["id"].ToString());
            if (FSdate != null && FEdate != null)
            {

                var selectedlist = (from e in hrentity.spGetdol(FSdate, FEdate, code) select e).ToList();
                if (selectedlist.Count() != 0)
                {
                    DataTable dt = new DataTable("Grid");
                    dt.Columns.AddRange(new DataColumn[8]
                    {
                        new DataColumn("Employee Name"),
                        new DataColumn("Department"),
                        new DataColumn("Designation"),
                        new DataColumn("Is Replacement"),
                        new DataColumn("DOJ"),
                        new DataColumn("DOL"),
                        new DataColumn("Salary"),
                        new DataColumn("Address")
                    });
                    foreach (var emp in selectedlist)
                    {
                        dt.Rows.Add(emp.vchName, emp.vchdeptname, emp.vchdesignation, emp.vchRplcmntName, emp.dtDOJ.ToString("d"),emp.dtDOL.ToString("d"), emp.intsalary, emp.vchpaddress);
                    }
                    using (XLWorkbook eb = new XLWorkbook())
                    {
                        eb.Worksheets.Add(dt);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            eb.SaveAs(ms);
                            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LeftEmployee.xlsx");
                        }
                    }
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        //mothod for year
        private SelectList GetYears(int? iSelectedYear)
        {
            int CurrentYear = DateTime.Now.Year;

            for (int i = 2021; i <= CurrentYear; i++)
            {
                ddlYears.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }

            //Default It will Select Current Year  
            return new SelectList(ddlYears, "Value", "Text", iSelectedYear);

        }

        //method for month
        private SelectList GetMonths(int? iSelectedYear)
        {
            var months = Enumerable.Range(1, 12).Select(i => new
            {
                A = i,
                B = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
            });

            int CurrentMonth = 1; //January  

            if (iSelectedYear == DateTime.Now.Year)
            {
                CurrentMonth = DateTime.Now.Month;
                months = Enumerable.Range(1, CurrentMonth).Select(i => new
                {
                    A = i,
                    B = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
                });
            }

            foreach (var item in months)
            {
                ddlMonths.Add(new SelectListItem { Text = item.B.ToString(), Value = item.A.ToString() });
            }

            //Default for Current Month  
            return new SelectList(ddlMonths, "Value", "Text", CurrentMonth);

        }   


        //Gets the to-do Lists.    
        public JsonResult GetCustomers(string word, int page, int rows, string searchString) //(string datefrom, string dateto,
        {
            //#1 Create Instance of DatabaseContext class for Accessing Database.  
            //#2 Setting Paging  
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());

            //#3 Linq Query to Get Customer   
            var Results = hrentity.tblEmpMas.Where(m=>m.intcode==code).Select(
              a => new
              {
                  a.vchEmpFcode,
                  a.vchname,
                  a.vchfname,
                  a.vchlname,
                  a.vchdeptname,
                  a.vchdesignation,
                  a.dtDOJ,
                  a.intsalary
              });

            //#4 Get Total Row Count  
            int totalRecords = Results.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);           
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = Results
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        
        //For searching and shorting employee in active list
        public ActionResult ActiveData()
        {
            return View();
        }
       
        public ActionResult SearchActive()
        {
            if (Session["descript"] != null)
            {                
                hrentity = new HRMEntities();
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedList = hrentity.spGetSearchEmployee(code).ToList();                   
                if (selectedList.Count != 0)
                {
                    return Json(new { data = selectedList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var errormsg = "No record found in database!";
                    return Json(new { data = errormsg }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }     
    }
}
