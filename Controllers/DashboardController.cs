using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using ClosedXML.Excel;
using HRM.Models;
using Microsoft.Reporting.WebForms;

namespace HRM.Controllers
{
    public class DashboardController : Controller
    {
        HRMEntities Objentity = new HRMEntities();

        IndusGroupEntities objCompany = new IndusGroupEntities();      
        
        //GET: Dashboard
        //for nabh Assessment Detail
        public ActionResult AssesmentView()
        {
            if (Session["SessionDate"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                DateTime fromdt = Convert.ToDateTime(Session["SessionDate"].ToString());
                var getallAssessment = (from e in Objentity.tblEmpAssesmentDetails join m in Objentity.tblEmpAssesmentMas on e.fk_AssEmpId equals m.intid
                                        where  m.dtcreated >= fromdt && e.intcode == code select e).ToList();
                if (getallAssessment.Count() != 0)
                {
                    return View(getallAssessment);
                }
                else
                {
                    ViewBag["Empty"] = "0 Records found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult Index(string smsg)
        {
            if (Session["descript"] != null)
            {
                if (smsg != null)
                {
                    ViewBag.Message = smsg.ToString();
                }
                int code = Convert.ToInt32(Session["id"].ToString());
                //New Emp Count       
                //Pending Authorization Count
                if (Session["Sfrom"] != null)
                {
                    int salfrom = Convert.ToInt32(Session["Sfrom"].ToString());
                    int salto = Convert.ToInt32(Session["Sto"].ToString());
                    var newautorization = (from e in Objentity.tblEmpAssesmentMas
                                           where e.bitgoauthor == true && e.bitAuthorBack!=true && e.bitauthorised == false && e.bitauthorcancel == false
                                           && e.intsalary >= salfrom && e.intsalary <= salto
                                           && e.intcode == code
                                           select e).Count();
                    ViewBag.AuthoriseGoCount = newautorization.ToString();
                }
                else
                {
                    if (Session["SessionDate"] != null)
                    {
                        DateTime fromdt = Convert.ToDateTime(Session["SessionDate"].ToString());
                        var newautorization = (from e in Objentity.tblEmpAssesmentMas
                                               where e.bitgoauthor == true && e.bitauthorised == false && e.bitauthorcancel == false && e.bitIsLeft != true && e.bitstatusdeactive != true && e.bitIsPartialAuthorised != true
                                               && e.dtDOJ >=fromdt && e.intcode == code
                                               select e).Count();
                        ViewBag.AuthoriseGoCount = newautorization.ToString();
                    }
                    else
                    {
                        var newautorization = (from e in Objentity.tblEmpAssesmentMas
                                               where e.bitgoauthor == true && e.bitauthorised == false && e.bitauthorcancel == false && e.bitIsLeft != true && e.bitstatusdeactive != true && e.bitIsPartialAuthorised != true
                                               && e.intcode == code
                                               select e).Count();
                        ViewBag.AuthoriseGoCount = newautorization.ToString();
                    }
                }

                //Active Employee Count
                if (Session["SessionDate"] != null)
                {
                    DateTime SessionDate = Convert.ToDateTime(Session["SessionDate"].ToString());
                    //string NewSdate1 = fromdt.ToString("dd/MM/yyyy");
                    // DateTime FromDate = Convert.ToDateTime(NewSdate1);
                    var Active = (from e in Objentity.tblEmpAssesmentMas where (e.dtDOJ >= SessionDate) && (e.bittempstatusactive == true && e.bitIsPartialAuthorised != true && e.intcode == code) select e).Count();
                    ViewBag.ActiveCount = Active.ToString();
                    //Left Employee Count
                    var Left = (from e in Objentity.tblEmpAssesmentMas where e.dtcreated >= SessionDate && e.bitstatusdeactive == true && e.bitIsLeft == true && e.intcode == code select e).Count();
                    ViewBag.LeftCount = Left.ToString();
                }
                else
                {
                    var Active = (from e in Objentity.tblEmpAssesmentMas where e.bittempstatusactive == true && e.bitstatusdeactive != true && e.bitIsPartialAuthorised != true && e.intcode == code select e).Count();
                    ViewBag.ActiveCount = Active.ToString();
                    //Left Employee Count
                    var Left = (from e in Objentity.tblEmpAssesmentMas where e.bitstatusdeactive == true && e.intcode == code select e).Count();
                    ViewBag.LeftCount = Left.ToString();
                }
                

                //Cancelled authorization Employee Count
                var CancelAuthor = (from e in Objentity.tblEmpAssesmentMas where e.bitauthorcancel == true && e.bitauthorised == false && e.intcode == code select e).Count();
                ViewBag.CancelAuthor = CancelAuthor.ToString();

                //Authorised employee count
                var AuthorisedCount = (from e in Objentity.tblEmpAssesmentMas where e.bitauthorised == true && e.bittempstatusactive == false && e.bitstatusdeactive != true && e.bitIsUnjoined != true && e.bitIsTransferred!=true && e.intcode == code select e).Count();
                ViewBag.AuthorisedCount = AuthorisedCount.ToString();

                //Get Partial Authorization Count
                //For Session Date
                if (Session["SessionDate"] != null)
                {
                    DateTime NewDate1=Convert.ToDateTime(Session["SessionDate"].ToString());
                    DateTime NewDate2 = Convert.ToDateTime(NewDate1.ToString("yyyy-MM-dd"));
                    var PartialAuthorCount = (from e in Objentity.tblEmpAssesmentMas where e.dtDOJ>=NewDate2 && e.bitIsPartialAuthorised == true && e.bitIsUnjoined == false && e.bitIsLeft != true && e.intcode == code select e).Count();
                    ViewBag.PartialAuthorisedCount = PartialAuthorCount.ToString();
                }
                else
                {
                    var PartialAuthorCount = (from e in Objentity.tblEmpAssesmentMas where e.bitIsPartialAuthorised == true && e.bitIsUnjoined == false && e.bitIsLeft != true && e.intcode == code select e).Count();
                    ViewBag.PartialAuthorisedCount = PartialAuthorCount.ToString();
                }

                //Get Transferred Emplyee count
                var TransferCount = (from e in Objentity.tblTransferDetail where e.intTransferToCode == code && e.BitTransferComplete!=true select e).Count();
                ViewBag.TransferCount = TransferCount.ToString();
                
                //Get Assigned Exit Form
                var ExitFormAssigned = (from e in Objentity.tblEmpAssesmentMas where e.bitAssignExitForm == true && e.bittempstatusactive==true select e).Count();       
                ViewBag.ExitAssigned= ExitFormAssigned.ToString();

                //Get Red flag employee count
                var RedCount = (from e in Objentity.tblEmpAssesmentMas where e.BitIsFlaggingEmp == true && e.BitIsRedFlagging == true select e).ToList();
                if (RedCount.Count() != 0)
                {
                    ViewBag.RedFlagCount = RedCount.Count().ToString();
                }
                else
                {
                    ViewBag.RedFlagCount = "0";
                }
                //get sessional users
                string ISHOD = Session["DeptHOD"].ToString();
                string ISHR = Session["HrAdmin"].ToString();
                string ISVPHR = Session["ISVPHR"].ToString();
                string MainAdmin = Session["MainAdmin"].ToString();
                if (Session["usrid"] != null)
                {
                    int aa = Convert.ToInt32(Session["usrid"].ToString());
                    //Get Assigned Assesment Count User Wise
                    if (MainAdmin == "True")
                    {
                        var selectedlist = (from d in this.Objentity.tblLeaveApplicationDetail
                                            join e in this.Objentity.tblLeaveApplication on d.fk_AppID equals e.unID
                                            where d.bitHODComplete == false && d.bitHRComplete == false && e.bitIsApproved == false && e.bitIsRejected == false && e.intCode == code
                                            select e).ToList();
                        if (selectedlist.Count() == 0)
                        {
                            ViewBag.LeaveCount = "0";
                        }
                        else
                        {
                            ViewBag.LeaveCount = selectedlist.Count().ToString();
                        }
                    }
                    else 
                    {
                        var selectedlist = (from d in this.Objentity.tblLeaveApplicationDetail
                                            join e in this.Objentity.tblLeaveApplication on d.fk_AppID equals e.unID
                                            where d.fk_AssignUserid==aa && d.bitISAssigned == (bool?)true && d.bitISApproved != (bool?)true && e.intCode == code
                                            select e).ToList();
                        if (selectedlist.Count() == 0)
                        {
                            ViewBag.LeaveCount = "0";
                        }
                        else
                        {
                            ViewBag.LeaveCount = selectedlist.Count().ToString();
                        }
                    }                  
                    //Assigned asssessment counter
                    var AssmntCount = (from e in Objentity.tblEmpAssesmentMas
                                        join a in Objentity.tblEmpAssesmentDetails on e.intid equals a.fk_AssEmpId
                                        where a.fk_userid == aa && a.BitIsAssignCompleted == false && e.bitIsLeft != true && e.bitIsConsultant != true && e.intcode == code
                                        select e).Count();
                    if (AssmntCount == 0)
                    {
                        ViewBag.AssigendCount = "0";
                    }
                    else
                    {
                        ViewBag.AssigendCount = AssmntCount.ToString();
                    }
                }
                
                if (ISHOD == "True")
                {
                    //requisition
                    var getCount = (from e in Objentity.tblRequisition where e.bitISAssignToHR == true && e.bitISForwardFromHR != true && e.bitStatusComplete!=true && e.intCode == code select e).ToList();
                    ViewBag.Requisition = getCount.Count.ToString();                    
                }               
                else if (ISHR == "True" && ISVPHR!="True")
                {
                    //requisitions
                    var getCount = (from e in Objentity.tblRequisition where e.bitISAssignToHR == true && e.bitISForwardFromHR != true && e.bitStatusComplete != true && e.intCode == code select e).ToList();
                    ViewBag.Requisition = getCount.Count.ToString();
                }
                else if (ISHR=="True" && ISVPHR == "True")
                {
                    var getCount = (from e in Objentity.tblRequisition where e.bitISAssignToVPHR == true && e.bitISForwardFromVPHR != true && e.bitStatusComplete != true && e.intCode == code select e).ToList();
                    ViewBag.Requisition = getCount.Count.ToString();
                }
                //Consultant pending requests
                var getRequestCount = (from e in Objentity.tblConsultantSlips where e.bitIsRequest == true && e.bitIsPending == true && e.bitIsComplete == false && e.intcode==code select e).Count();
                ViewBag.ConsultantRequest = getRequestCount.ToString();
                //For notification
                var Notification = "";
                ViewBag.Notification = Notification;             
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //View All Active Employee
        public ActionResult ActiveView()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            //int pageSize = 10;
            //int pageNumber = page ?? 1;
            //For NABH
            if (Session["SessionDate"] != null)
            {
                DateTime SessionDate = Convert.ToDateTime(Session["SessionDate"].ToString());
                var activeemplist = (from e in Objentity.tblEmpAssesmentMas where (e.dtDOJ >= SessionDate) && (e.bittempstatusactive == true && e.bitIsPartialAuthorised != true && e.bitIsConsultant!=true && e.intcode == code) select e).ToList();
                if (activeemplist.Count != 0)
                {
                    TempData["Success"] = "Total" + " " + activeemplist.Count() + " " + "Employees";
                    return View(activeemplist);
                }
                else
                {
                    TempData["Empty"] = "No Active employee avilable currently!";
                    return View();
                }
            }
            else
            {
                var activeemplist = (from e in Objentity.tblEmpAssesmentMas where e.bittempstatusactive == true && e.bitstatusdeactive != true && e.bitIsPartialAuthorised != true && e.intcode == code orderby e.dtDOJ descending select e).ToList();
                if (activeemplist.Count != 0)
                {
                    TempData["Success"] = "Total" + " " + activeemplist.Count() + " " + "Employees";
                    return View(activeemplist);
                }
                else
                {
                    TempData["Empty"] = "No active employee avilable currently!";
                    return View();
                }
            }
        }

        public ActionResult CurrentExp(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedEmp = (from e in Objentity.tblEmpAssesmentMas
                                       join d in Objentity.tblEmpDetails on e.intid equals d.fk_intempid where e.intid == id select e).FirstOrDefault();
                    if (selectedEmp != null)
                    {
                        LocalReport lr = new LocalReport();
                        string filepath = String.Empty;
                        HttpClient _client = new HttpClient();

                        //Get dataset value and fields
                        var selectedobj = (from e in Objentity.spCurrentExpCertificate(id) select e).ToList();

                        //get path
                        filepath = Path.Combine(Server.MapPath("~/Content/Report"), ("CurrentExpLetter.rdl"));
                        //open streams
                        using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                        {
                            lr.LoadReportDefinition(filestream);
                            lr.DataSources.Add(new ReportDataSource(name: "DataSet1", selectedobj));
                            ReportParameter ptr = new ReportParameter("id", id.ToString());
                            lr.SetParameters(ptr);
                            byte[] pdfData = lr.Render("PDF");
                            return File(pdfData, contentType: "Application/pdf");
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Selected employee other detail not found in database, try again or contact to admin!";
                        return RedirectToAction("ActiveView");
                    }
                }
                else
                {
                    TempData["Error"] = "";
                    return RedirectToAction("ActiveView");
                }
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        //Export to Excel all active employee
        public ActionResult LeftView()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            //NABH Session Date
            if (Session["SessionDate"] != null)
            {
                DateTime SessionDate = Convert.ToDateTime(Session["SessionDate"].ToString());
                var leftlist = (from e in Objentity.tblEmpAssesmentMas where e.dtcreated>=SessionDate && e.bitstatusdeactive == true && e.bitIsLeft==true && e.intcode == code select e).ToList();
                if (leftlist.Count != 0)
                {
                    TempData["Success"] = "Total " + "" + leftlist.Count() + " Employees, Showing color according to assigned flag";
                    return View(leftlist);
                }
                else
                {
                    TempData["Empty"] = "No Left employee avilable currently!";
                    return View();
                }
            }
            else
            {
                var leftlist = (from e in Objentity.tblEmpAssesmentMas where e.bittempstatusactive == false && e.bitstatusdeactive == true && e.intcode == code select e).ToList();
                if (leftlist.Count != 0)
                {
                    TempData["Success"] = "Total " + "" + leftlist.Count() + " Employees, Showing color according to assigned flag";
                    return View(leftlist);
                }
                else
                {
                    TempData["Empty"] = "No Left employee avilable currently!";
                    return View();
                }
            }
        }

        //View All Cancelled Autorization Employee
        public ActionResult CancelAuthorView()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var activeemplist = (from e in Objentity.tblEmpAssesmentMas where e.bitauthorcancel == true && e.intcode == code select e).ToList();
            if (activeemplist.Count != 0)
            {
                TempData["Success"] = "Total" + " " + activeemplist.Count() + " Employees";
                return View(activeemplist);
            }
            else
            {
                TempData["Empty"] = "No canceled authorization employee avilable currently!";
                return View();
            }
        }      

        public ActionResult ViewActiveDetails(int id)
        {
            return RedirectToAction("ViewHrFullDetails", "Authorization", new { id = id });
        }

        //for cancelled details view
        public ActionResult ViewActiveDetails1(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var selecetdemp = (from e in Objentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).ToList();
            if (selecetdemp.Count != 0)
            {
                var reason = (from e in Objentity.tblConversation where e.fk_intEmpid == id && e.intcode == code && e.bitIsAuthorCancelReason == true select e).FirstOrDefault();
                if (reason != null && reason.vchMsg != null)
                {
                    TempData["CancelReson"] = reason.vchMsg.ToString();
                }
                else
                {
                    var oldreason = (from e in Objentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                    if (oldreason != null)
                    {
                        TempData["CancelReson"] = oldreason.vchcancelreason.ToString();
                    }
                    else
                    {
                        TempData["CancelReson"] = "Empty!";
                    }
                }
                //TempData["Success"] = "Total" + selecetdemp.Count() + " Employees";
                return View(selecetdemp);
            }
            else
            {
                TempData["Empty"] = "No Left employee avilable currently!";
                return View();
            }
        }

        //view all authorise employee for update final details
        public ActionResult ViewAuthorisedEmp()
        {
            return RedirectToAction("ViewAuthorisedEmp", "Authorization");
        }

        //parial session Error
        public ActionResult _DashSessionError()
        {
            return View();
        }

        public ActionResult EmployeeDeactive()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var activeemplist = (from e in Objentity.tblEmpAssesmentMas where e.bittempstatusactive == true && e.intcode == code && e.bitstatusdeactive == false select e).ToList();
            return View(activeemplist);
        }

        public ActionResult DeactiveEmp(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                DeactiveEmpModelView objmodel = new DeactiveEmpModelView();
                var selecetdemp = (from e in Objentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                if (selecetdemp != null)
                {
                    objmodel.empid = selecetdemp.intid;
                    objmodel.fcode = selecetdemp.vchEmpFcode;
                    objmodel.empname = selecetdemp.vchName;
                    if (selecetdemp.vchAadharNo != null)
                    {
                        objmodel.vchAadharNo = selecetdemp.vchAadharNo.ToString();
                    }
                    //if (selecetdemp.BitIsFlaggingEmp != false)
                    //{
                    //    if (selecetdemp.BitIsRedFlagging == true)
                    //    {
                    //        objmodel.BitIsRedFlagging = true;
                    //    }
                    //    else
                    //    {
                    //        objmodel.BitIsRedFlagging = false;
                    //    }
                    //    if (selecetdemp.BitIsOrangeFlagging == true)
                    //    {
                    //       objmodel.BitIsOrangeFlagging = true;
                    //    }
                    //    else
                    //    {
                    //        objmodel.BitIsOrangeFlagging = false;
                    //    }
                    //    if (selecetdemp.BitIsGreenFlagging == true)
                    //    {
                    //        objmodel.BitIsGreenFlagging = true;
                    //    }
                    //    else
                    //    {
                    //        objmodel.BitIsGreenFlagging= false;
                    //    }
                    //}
                    return View(objmodel);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult DeactiveEmp(DeactiveEmpModelView objmodel)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int id = Convert.ToInt32(objmodel.empid);
                    var selecetdemp = (from e in Objentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                    if (selecetdemp != null)
                    {
                        selecetdemp.bitstatusdeactive = true;
                        selecetdemp.bittempstatusactive = false;
                        if (objmodel.Empstatus == "Left")
                        {
                            selecetdemp.bitIsLeft = true;
                        }
                        if(objmodel.Empstatus== "Terminate")
                        {
                            selecetdemp.bitIsTerminated = true;
                        }
                        selecetdemp.dtDOL = objmodel.dol;
                        if (objmodel.BitIsFlaggingEmp != null)
                        {
                            if (objmodel.BitIsFlaggingEmp == "Red")
                            {
                                selecetdemp.BitIsRedFlagging = true;
                            }
                            if (objmodel.BitIsFlaggingEmp == "Orange")
                            {
                                selecetdemp.BitIsOrangeFlagging = true;
                            }
                            if (objmodel.BitIsFlaggingEmp == "Green")
                            {
                                selecetdemp.BitIsGreenFlagging = true;
                            }
                            selecetdemp.BitIsFlaggingEmp = true;
                        }
                        else
                        {
                            TempData["Error"] = "Select flag color for employee!";
                            return View("DeactiveEmp", new { id = objmodel.empid });
                        }
                        if (objmodel.vchRemarks != null)
                        {
                            selecetdemp.vchAssDeactiveRemarks = objmodel.vchRemarks.ToString();
                        }
                        else
                        {
                            selecetdemp.vchAssDeactiveRemarks = "N/A";
                        }
                        selecetdemp.vchAadharNo = objmodel.vchAadharNo;
                        selecetdemp.vchdeactiveby = Session["descript"].ToString();
                        selecetdemp.dtdecavited = DateTime.Now;
                        selecetdemp.vchdeactivedhost = Session["hostname"].ToString();
                        selecetdemp.vchdeactivedipused = Session["ipused"].ToString();
                        try
                        {
                            Objentity.SaveChanges();
                            TempData["Success"] = "Employee status changed successfully!";
                            return RedirectToAction("EmployeeDeactive");
                        }
                         catch (DbEntityValidationException ex)
                        {
                            // Retrieve the validation errors
                            foreach (var validationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    TempData["Error"] = ("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                }
                            }
                            return RedirectToAction("EmployeeDeactive");
                        }
                       
                    }
                }
                else
                {
                    TempData["Error"] = "Model Error Generated Please Contact To Administrator!";
                    return RedirectToAction("DeactiveEmp");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
            return View();
        }

        #region Employee Transfer
        public ActionResult NewTransfer(int id)
        {
            if (Session["descript"] != null)
            {
                var selectedemp = (from e in Objentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                if (selectedemp != null)
                {
                    return View(selectedemp);
                }
                else
                {
                    return View();
                }
               
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
        
        //For transfer employee
        public ActionResult TransferEmp(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selecetdEmp = (from e in Objentity.tblEmpAssesmentMas where e.intid == id select e).First();
                    if (selecetdEmp != null)
                    {
                        //get current company code && remove from select transfer company
                        int comp_code = Convert.ToInt32(Session["id"].ToString());
                        //for select transfer to branch
                        var allbranches = (from e in objCompany.IndusCompanies where (( e.intPK >= 2 && e.intPK <= 4 ) || ( e.intPK >= 14 && e.intPK <= 16 ) || (e.intPK >= 21 && e.intPK <= 26)) && e.intPK != comp_code select e).ToList();
                        List<SelectListItem> allbranch = new List<SelectListItem>();
                        allbranch.Add(new SelectListItem { Text = "Select Transfer To Branch", Value = "0" });
                        foreach (var branch in allbranches)
                        {
                            SelectListItem selectListItem = new SelectListItem()
                            {
                                Text = branch.descript.ToString(),
                                Value = branch.intPK.ToString()
                            };
                            allbranch.Add(selectListItem);
                        }
                        ViewBag.AllBranch = new SelectList(allbranch, "Text", "Value");
                        EmpTransferModelView objnew = new EmpTransferModelView();
                        objnew.empid = selecetdEmp.intid;
                        objnew.empname = selecetdEmp.vchName;
                        objnew.empcode = selecetdEmp.vchEmpFcode;
                        if (selecetdEmp.vchAadharNo != null)
                        {
                            objnew.AadharNo = selecetdEmp.vchAadharNo;
                        }
                        objnew.transferSalary = Convert.ToInt32(selecetdEmp.intsalary);                       
                        return View(objnew);
                    }
                    else
                    {
                        TempData["Error"] = "Employee id should not be null or empty!";
                        return RedirectToAction("EmployeeDeactive");
                    }
                }
                else
                {
                    TempData["Error"] = "Employee id should not be null or empty!";
                    return RedirectToAction("EmployeeDeactive");
                }
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        [HttpPost]
        public ActionResult TransferEmp(EmpTransferModelView objtransfer )
        {
            if (Session["descript"] != null)
            {
                //get current com code
                int comp_code = Convert.ToInt32(Session["id"].ToString());
                //create ne transfer object
                tblTransferDetail newobj = new tblTransferDetail();
                if (ModelState.IsValid)
                {
                    //get selected branch
                    var TransferToCompany = (from e in objCompany.IndusCompanies where e.intPK == objtransfer.ToTransferBranch select e).FirstOrDefault();
                    if (TransferToCompany != null) 
                    { 
                    var selectedEmp = (from e in Objentity.tblEmpAssesmentMas where e.intid == objtransfer.empid select e).FirstOrDefault();
                        if (selectedEmp != null)
                        {
                           
                            var selectedDept = (from e in Objentity.tblDeptMas where e.intid == selectedEmp.fk_intdeptid select e).FirstOrDefault();
                            if (selectedDept != null)
                            {
                                var selecetdDesi = (from e in Objentity.tblDesignationMas where e.intid == selectedEmp.fk_intdesiid select e).FirstOrDefault();
                                if (selecetdDesi != null)
                                {
                                    try
                                    {
                                        newobj.fk_empid = selectedEmp.intid;
                                        newobj.intTransferSalary = selectedEmp.intsalary;
                                        newobj.fk_transferDept = selectedDept.intid;
                                        newobj.fk_TransferDesi = selecetdDesi.intid;
                                        newobj.intTransferredCode = selectedEmp.intcode;
                                        newobj.vchTransferFromBranch = Session["Compname"].ToString();
                                        newobj.vchTransferToBranch = TransferToCompany.descript;
                                        newobj.intTransferToCode = TransferToCompany.intPK;
                                        newobj.vchTransferRemarks = objtransfer.tremarks;
                                        newobj.vchOldEmpCode = selectedEmp.vchEmpFcode;
                                        newobj.dtRelieved = objtransfer.DOL;
                                        newobj.dtTransferredDOJ = selectedEmp.dtDOJ;
                                        newobj.dtTransfer = DateTime.Now;
                                        newobj.vchTransferBy = Session["descript"].ToString();
                                        newobj.vchTransferHost = Session["hostname"].ToString();
                                        newobj.vchTransferIP = Session["ipused"].ToString();
                                        //update master status as transferred employee                                        
                                        selectedEmp.vchAadharNo = objtransfer.AadharNo;
                                        selectedEmp.bitIsTransferred = true;
                                        selectedEmp.bitstatusdeactive = false;
                                        selectedEmp.bittempstatusactive = false;
                                        selectedEmp.dtDOL = objtransfer.DOL;
                                        Objentity.tblTransferDetail.Add(newobj);
                                        Objentity.SaveChanges();
                                        TempData["Success"] = "Employee transferred successfully";
                                        return RedirectToAction("EmployeeDeactive");
                                    }                                   
                                     catch (DbEntityValidationException ex)
                                    {
                                        // Retrieve the validation errors
                                        foreach (var validationErrors in ex.EntityValidationErrors)
                                        {
                                            foreach (var validationError in validationErrors.ValidationErrors)
                                            {
                                                TempData["Error"]=("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                                
                                            }
                                        }
                                        return RedirectToAction("EmployeeDeactive");
                                    }                                  
                                
                                }
                                else
                                {
                                    //for select transfer to branch
                                    var allbranches = (from e in objCompany.IndusCompanies where ((e.intPK >= 2 && e.intPK <= 4) || (e.intPK >= 14 && e.intPK <= 16) || (e.intPK >= 21 && e.intPK <= 26)) && e.intPK != comp_code select e).ToList();
                                    List<SelectListItem> allbranch = new List<SelectListItem>();
                                    allbranch.Add(new SelectListItem { Text = "Select Transfer To Branch", Value = "0" });
                                    foreach (var branch in allbranches)
                                    {
                                        SelectListItem selectListItem = new SelectListItem()
                                        {
                                            Text = branch.descript.ToString(),
                                            Value = branch.intPK.ToString()
                                        };
                                        allbranch.Add(selectListItem);
                                    }
                                    ViewBag.AllBranch = new SelectList(allbranch, "Text", "Value");
                                    ViewBag.Error = "Employee designation not found please check properly and try again!";
                                    return View(objtransfer);
                                }
                            }                            
                            else
                            {
                                //for select transfer to branch
                                var allbranches = (from e in objCompany.IndusCompanies where ((e.intPK >= 2 && e.intPK <= 4) || (e.intPK >= 14 && e.intPK <= 16) || (e.intPK >= 21 && e.intPK <= 26)) && e.intPK != comp_code select e).ToList();
                                List<SelectListItem> allbranch = new List<SelectListItem>();
                                allbranch.Add(new SelectListItem { Text = "Select Transfer To Branch", Value = "0" });
                                foreach (var branch in allbranches)
                                {
                                    SelectListItem selectListItem = new SelectListItem()
                                    {
                                        Text = branch.descript.ToString(),
                                        Value = branch.intPK.ToString()
                                    };
                                    allbranch.Add(selectListItem);
                                }
                                ViewBag.AllBranch = new SelectList(allbranch, "Text", "Value");
                                ViewBag.Error = "Employee department not found please check properly and try again!";
                                return View(objtransfer);
                            }
                        }
                        else
                        {
                            //for select transfer to branch
                            var allbranches = (from e in objCompany.IndusCompanies where ((e.intPK >= 2 && e.intPK <= 4) || (e.intPK >= 14 && e.intPK <= 16) || (e.intPK >= 21 && e.intPK <= 26)) && e.intPK != comp_code select e).ToList();
                            List<SelectListItem> allbranch = new List<SelectListItem>();
                            allbranch.Add(new SelectListItem { Text = "Select Transfer To Branch", Value = "0" });
                            foreach (var branch in allbranches)
                            {
                                SelectListItem selectListItem = new SelectListItem()
                                {
                                    Text = branch.descript.ToString(),
                                    Value = branch.intPK.ToString()
                                };
                                allbranch.Add(selectListItem);
                            }
                            ViewBag.AllBranch = new SelectList(allbranch, "Text", "Value");
                            ViewBag.Error = "Employee department not found please check properly and try again!";
                            return View(objtransfer);
                        }
                    }
                    else
                    {
                        //for select transfer to branch
                        var allbranches = (from e in objCompany.IndusCompanies where ((e.intPK >= 2 && e.intPK <= 4) || (e.intPK >= 14 && e.intPK <= 16) || (e.intPK >= 21 && e.intPK <= 26)) && e.intPK != comp_code select e).ToList();
                        List<SelectListItem> allbranch = new List<SelectListItem>();
                        allbranch.Add(new SelectListItem { Text = "Select Transfer To Branch", Value = "0" });
                        foreach (var branch in allbranches)
                        {
                            SelectListItem selectListItem = new SelectListItem()
                            {
                                Text = branch.descript.ToString(),
                                Value = branch.intPK.ToString()
                            };
                            allbranch.Add(selectListItem);
                        }
                        ViewBag.AllBranch = new SelectList(allbranch, "Text", "Value");
                        ViewBag.Error = "Selecetd indus company not found please tray again!";
                        return View(objtransfer);
                    }

                }
                else
                {
                    //for select transfer to branch
                    var allbranches = (from e in objCompany.IndusCompanies where ((e.intPK >= 2 && e.intPK <= 4) || (e.intPK >= 14 && e.intPK <= 16) || (e.intPK >= 21 && e.intPK <= 26)) && e.intPK != comp_code select e).ToList();
                    List<SelectListItem> allbranch = new List<SelectListItem>();
                    allbranch.Add(new SelectListItem { Text = "Select Transfer To Branch", Value = "0" });
                    foreach (var branch in allbranches)
                    {
                        SelectListItem selectListItem = new SelectListItem()
                        {
                            Text = branch.descript.ToString(),
                            Value = branch.intPK.ToString()
                        };
                        allbranch.Add(selectListItem);
                    }
                    ViewBag.AllBranch = new SelectList(allbranch, "Text", "Value");
                    ViewBag.Error = "System error generated please contact to administrator!";
                    return View(objtransfer);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ViewTransferEmp()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedList = (from e in Objentity.tblTransferDetail
                                    join m in Objentity.tblEmpAssesmentMas on e.fk_empid equals m.intid
                                    where e.intTransferToCode == code && m.bitIsTransferred == true && e.BitTransferComplete!=true
                                    select e).ToList();
                if (selectedList.Count() != 0)
                {
                    ViewBag.Success = "Fount " + selectedList.Count().ToString() + " transferred employee";
                    return View(selectedList);
                }
                else
                {
                    ViewBag.Empty = "0 Employee Transfer found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult TransferRecieve(int id)
        {
            if (Session["descript"] != null)
            {
                //tblTransferDetail objtransfer = new tblTransferDetail();
                var objtransfer = (from e in Objentity.tblTransferDetail where e.fk_empid == id select e).FirstOrDefault();
                if (objtransfer != null)
                {
                    RecieveTransferViewModel modelclass = new RecieveTransferViewModel();
                    modelclass.vchTransferFromBranch = objtransfer.vchTransferFromBranch;
                    modelclass.vchTransferToBranch = objtransfer.vchTransferToBranch;
                    modelclass.dtTransferredDOJ = objtransfer.dtTransferredDOJ;
                    modelclass.dtRelieved = objtransfer.dtRelieved;
                    modelclass.intTransferSalary = objtransfer.intTransferSalary;
                    modelclass.empid = objtransfer.fk_empid;
                    modelclass.intid = objtransfer.intid;
                    modelclass.vchTransferRemarks = objtransfer.vchTransferRemarks;
                    //select all position
                    var allposii = (from e in Objentity.tblPositionCategoryMas
                                    where e.BitDesiMapping == true
                                    select e).ToList();
                    List<SelectListItem> allpossi = new List<SelectListItem>();
                    allpossi.Add(new SelectListItem { Text = "Select position", Value = "0" });
                    foreach (var position in allposii)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = position.vchPosCatName,
                            Value = position.intid.ToString()
                        };
                        allpossi.Add(selectListItem);
                    }
                    ViewBag.AllPosition = new SelectList(allpossi, "Text", "Value");
                    //Select all department master
                    var deptlist = (from e in Objentity.tblDeptMas select e).ToList();
                    List<SelectListItem> newlist = new List<SelectListItem>();
                    newlist.Add(new SelectListItem { Text = "Select department", Value = "0" });
                    foreach (var dpt in deptlist)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = dpt.vchdeptname,
                            Value = dpt.intid.ToString()
                        };
                        newlist.Add(selectListItem);
                    }
                    ViewBag.DeptList = new SelectList(newlist, "Text", "Value");
                    return View(modelclass);
                }
                else
                {
                    TempData["Error"] = "Employee detail not found please try again or contact to administrator!";
                    return View();
                }
            }
            else
            {
                TempData["Error"] = "Employee ID should not be null or 0!";
                return View();
            }
        }

        [HttpPost]
        public ActionResult TransferRecieve(RecieveTransferViewModel objmodel, FormCollection fc)
        {
            if (Session["descript"] != null)
            {
                //recieved user details
                string user = Session["descript"].ToString();
                int code = Convert.ToInt32(Session["id"].ToString());
                //select transfer detail emp
                var transfertbl = (from e in Objentity.tblTransferDetail where e.intid == objmodel.intid select e).FirstOrDefault();
                //new object
                tblEmpAssesmentMas objnew = new tblEmpAssesmentMas();
                var selectedMasEmp = (from e in Objentity.tblEmpAssesmentMas where e.intid == objmodel.empid select e).FirstOrDefault();
                //change status selected old employee
                selectedMasEmp.bitIsTransferred = true;
                selectedMasEmp.bitstatusdeactive = true;
                selectedMasEmp.bitIsLeft = true;
                //for new record in assessment master
                objnew.vchName = selectedMasEmp.vchName;
                objnew.vchAadharNo = selectedMasEmp.vchAadharNo;
                objnew.fk_PositionId = selectedMasEmp.fk_PositionId;
                objnew.fk_intdeptid = objmodel.fk_ReacievedDept;           
                objnew.fk_intdesiid= Convert.ToInt32(fc.Get("selecteddesi"));
                objnew.intsalary = objmodel.intRecievedSalary;
                objnew.fk_inttitid = selectedMasEmp.fk_inttitid;
                objnew.vchgender = selectedMasEmp.vchgender;
                objnew.dtDOJ = objmodel.dtRecieveddoj;
                objnew.vchMobile = selectedMasEmp.vchMobile;
                objnew.decExperience = selectedMasEmp.decExperience;
                objnew.vchWorkedArea = selectedMasEmp.vchWorkedArea;
                objnew.dtcreated = DateTime.Now;
                objnew.vchcreatedby = user;
                objnew.vchcreatedhost = Session["hostname"].ToString();
                objnew.vchcreatedipused = Session["ipused"].ToString();
                objnew.vchAssignedBy = selectedMasEmp.vchAssignedBy;
                objnew.dtAssign = selectedMasEmp.dtAssign;
                objnew.BitCompleteAssesment = selectedMasEmp.BitCompleteAssesment;
                objnew.BitAssesmentResultPass = selectedMasEmp.BitAssesmentResultPass;
                objnew.vch_Status = selectedMasEmp.vch_Status;
                objnew.BitStatus = selectedMasEmp.BitStatus;
                objnew.BitAllowUpload = selectedMasEmp.BitAllowUpload;
                objnew.BitIsUploadCompleted = selectedMasEmp.BitIsUploadCompleted;
                objnew.decSkillMarks = selectedMasEmp.decSkillMarks;
                objnew.vchSkillStatus = selectedMasEmp.vchSkillStatus;
                objnew.BitAssesmentResultFail = selectedMasEmp.BitAssesmentResultFail;
                objnew.vchfname = selectedMasEmp.vchfname;
                objnew.vchmname = selectedMasEmp.vchmname;
                objnew.vchlname = selectedMasEmp.vchlname;
                objnew.bitofficialdetails = selectedMasEmp.bitofficialdetails;
                objnew.bittempqualidetails = selectedMasEmp.bittempqualidetails;
                objnew.bittempperdetails = selectedMasEmp.bittempperdetails;
                objnew.bittempcontdetails = selectedMasEmp.bittempcontdetails;
                objnew.bitqualidetails = selectedMasEmp.bitqualidetails;
                objnew.bitperdetails = selectedMasEmp.bitperdetails;
                objnew.bitcontdetails = selectedMasEmp.bitcontdetails;
                objnew.bitgoauthor = selectedMasEmp.bitgoauthor;
                objnew.bitauthorised = selectedMasEmp.bitauthorised;
                objnew.dtupdatedby = selectedMasEmp.dtupdatedby;
                objnew.intcode = Convert.ToInt32(Session["id"].ToString());
                objnew.vchEmpOldCode = selectedMasEmp.vchEmpFcode;
                objnew.vchReceiveRemarks = objmodel.vchRecievedRemarks;
                objnew.bittempstatusactive = true;               
                objnew.bitIsPartialAuthorised = selectedMasEmp.bitIsPartialAuthorised;
                objnew.bitIsTransferred = true;
                objnew.vchTransferRemarks = selectedMasEmp.vchTransferRemarks;
                //transfer detail table entry
                transfertbl.fk_ReacievedDept = objmodel.fk_ReacievedDept;
                transfertbl.fk_RecievedDesi= Convert.ToInt32(fc.Get("selecteddesi"));
                transfertbl.intRecievedSalary = objmodel.intRecievedSalary;
                transfertbl.dtRecieved = objmodel.dtRecieveddoj;
                transfertbl.vchRecievedRemarks = objmodel.vchRecievedRemarks;               
                transfertbl.BitTransferComplete = true;
                Objentity.tblEmpAssesmentMas.Add(objnew);
                Objentity.SaveChanges();
                //get new added emplyee id for other details.
                var newEmp = (from e in Objentity.tblEmpAssesmentMas where e.vchEmpOldCode == selectedMasEmp.vchEmpFcode select e).FirstOrDefault();
                if (newEmp != null)
                {
                    //add all document entry and empdetail table data in new objects.
                    tblEmpDetails objempdetail = new tblEmpDetails();
                    tblDocDetails objdocument = new tblDocDetails();
                    //select emp detail
                    var oldempdetail = (from e in Objentity.tblEmpDetails where e.fk_intempid == selectedMasEmp.intid select e).FirstOrDefault();
                    if (oldempdetail != null)
                    {
                        try
                        {
                            objempdetail.fk_intempid = newEmp.intid;
                            objempdetail.vchsex = oldempdetail.vchsex;
                            objempdetail.vchmaritalststus = oldempdetail.vchmaritalststus;
                            objempdetail.dtDob = oldempdetail.dtDob;
                            objempdetail.intage = oldempdetail.intage;
                            objempdetail.vchFatherName = oldempdetail.vchFatherName;
                            objempdetail.vchmothername = oldempdetail.vchmothername;
                            objempdetail.vchspouse = oldempdetail.vchspouse;
                            objempdetail.vchtaddress = oldempdetail.vchtaddress;
                            objempdetail.vchtstate = oldempdetail.vchtstate;
                            objempdetail.vchtcity = oldempdetail.vchtcity;
                            objempdetail.inttpin = oldempdetail.inttpin;
                            objempdetail.vchpaddress = oldempdetail.vchpaddress;
                            objempdetail.vchpstate = oldempdetail.vchpstate;
                            objempdetail.vchpcity = oldempdetail.vchpcity;
                            objempdetail.intppin = oldempdetail.intppin;
                            objempdetail.vchpstreet = oldempdetail.vchpstreet;
                            objempdetail.vchtmobile = oldempdetail.vchtmobile;
                            objempdetail.vchpmobile = oldempdetail.vchpmobile;
                            objempdetail.vchNominee = oldempdetail.vchNominee;
                            objempdetail.vchcreatedby = user;
                            objempdetail.intcode = Convert.ToInt32(Session["id"].ToString());
                            objempdetail.vchipused = Session["ipused"].ToString();
                            objempdetail.intyr = Convert.ToInt32(Session["yr"]);
                            objempdetail.fk_titid = oldempdetail.fk_titid;
                            objempdetail.vchfname = oldempdetail.vchfname;
                            objempdetail.vchmname = oldempdetail.vchmname;
                            objempdetail.vchlname = oldempdetail.vchlname;
                            objempdetail.BitCompleted = true;
                            objempdetail.vchRelation = oldempdetail.vchRelation;
                            Objentity.tblEmpDetails.Add(objempdetail);
                            Objentity.SaveChanges();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            // Retrieve the validation errors
                            foreach (var validationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    TempData["Error"] = ("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                                }
                            }
                            return RedirectToAction("ViewTransferEmp");
                        }
                    }
                    //document entry as a link copy only,use only uploaded document
                    var olddocument = (from e in Objentity.tblDocDetails where e.fk_empAssid == selectedMasEmp.intid select e).ToList();
                    if (olddocument.Count() != 0)
                    {
                        foreach(var doc in olddocument)
                        {
                            objdocument.fk_empAssid = newEmp.intid;
                            if (doc.fk_intdocid != null)
                            {
                                objdocument.fk_intdocid = doc.fk_intdocid;
                            }
                            if (doc.fk_qualiid != null)
                            {
                                objdocument.fk_qualiid = doc.fk_qualiid;
                            }
                            objdocument.vchdocname = doc.vchdocname;
                            objdocument.vchdocadd = doc.vchdocadd;
                            objdocument.vchfilename = doc.vchfilename;
                            objdocument.dtcreated = DateTime.Now;
                            objdocument.vchcreatedby = user;
                            objdocument.intcode = Convert.ToInt32(Session["id"].ToString());
                            objdocument.vchipused= Session["ipused"].ToString();
                            objdocument.vchhostname= Session["hostname"].ToString();
                            objdocument.BitIsCompDoc = doc.BitIsCompDoc;
                            objdocument.BitIsCompQuali = doc.BitIsCompQuali;
                            objdocument.BitIsProfilePic = doc.BitIsProfilePic;
                            Objentity.tblDocDetails.Add(objdocument);
                            Objentity.SaveChanges();
                        }
                    }
                    //for code generation
                    var getDptCode = (from e in Objentity.tblDeptMas where e.intid == newEmp.fk_intdeptid select e).FirstOrDefault();
                    var getCodeMas = (from e in Objentity.tblEmpCodeMas where e.intid == newEmp.fk_intdeptid select e).FirstOrDefault();
                    //generate new code
                    if (objmodel.bitIsCorporateemp == true)
                    {
                        //check 1st is already co employee code or not
                        if (selectedMasEmp.bitIsCorporateemp == true)
                        {
                            newEmp.vchEmpFcode = selectedMasEmp.vchEmpFcode;
                            newEmp.bitIsCorporateemp = true;
                            newEmp.bitIsUnitEmp = false;
                            Objentity.SaveChanges();
                        }
                        else
                        {
                            int codekaYear = 2023;
                            var getcode = (from e in Objentity.tblEmpCodeMas where e.intBranchCode == 0 && e.intJoinYear == codekaYear select e).FirstOrDefault();
                            //Generate corporate code                            
                            int currentcode = getcode.intCurrentCode;
                            int newcode = currentcode + 1;
                            int number = newcode;
                            int counter = 0;
                            string finalnumber = "";
                            //int fnumber = 0;
                            while (number > 0)
                            {
                                number = number / 10;
                                counter++;
                            }
                            if (counter > 0 && counter < 2)
                            {
                                finalnumber = string.Concat("000" + newcode.ToString());

                            }
                            if (counter > 1 && counter < 3)
                            {
                                finalnumber = string.Concat("00" + newcode.ToString());
                            }
                            if (counter > 2 && counter < 4)
                            {
                                finalnumber = string.Concat("0" + newcode.ToString());

                            }
                            if (counter > 3 && counter < 5)
                            {
                                finalnumber = newcode.ToString();
                            }
                            //Get Dept Code.
                            string fdeptcode = getDptCode.vchdepCode.ToString();
                            //Get Branch Code.
                            string branchcode = getcode.vchUnitCode.ToString();
                            //Get Year Code.
                            DateTime today = DateTime.Now;
                            DateTime joindate = Convert.ToDateTime(objnew.dtDOJ);
                            string yearCode = joindate.ToString("yy");
                            string finalCompleteCode = branchcode + "-" + yearCode + "-" + fdeptcode + "-" + finalnumber;
                            getcode.intCurrentCode = newcode;
                            newEmp.vchEmpFcode = finalCompleteCode;
                            newEmp.bitIsCorporateemp = true;
                            newEmp.bitIsUnitEmp = false;
                            Objentity.SaveChanges();
                        }
                    }
                    if (objmodel.bitIsCorporateemp == false)
                    {
                        int codekaYear = 2023;
                        var getcode = (from e in Objentity.tblEmpCodeMas where e.intBranchCode == code && e.intJoinYear == codekaYear select e).FirstOrDefault();
                        //Generate corporate code                            
                        int currentcode = getcode.intCurrentCode;
                        int newcode = currentcode + 1;
                        int number = newcode;
                        int counter = 0;
                        string finalnumber = "";
                        //int fnumber = 0;
                        while (number > 0)
                        {
                            number = number / 10;
                            counter++;
                        }
                        if (counter > 0 && counter < 2)
                        {
                            finalnumber = string.Concat("000" + newcode.ToString());

                        }
                        if (counter > 1 && counter < 3)
                        {
                            finalnumber = string.Concat("00" + newcode.ToString());
                        }
                        if (counter > 2 && counter < 4)
                        {
                            finalnumber = string.Concat("0" + newcode.ToString());

                        }
                        if (counter > 3 && counter < 5)
                        {
                            finalnumber = newcode.ToString();
                        }
                        //Get Dept Code.
                        string fdeptcode = getDptCode.vchdepCode.ToString();
                        //Get Branch Code.
                        string branchcode = getcode.vchUnitCode.ToString();
                        //Get Year Code.
                        DateTime today = DateTime.Now;
                        DateTime joindate = Convert.ToDateTime(objnew.dtDOJ);
                        string yearCode = joindate.ToString("yy");
                        string finalCompleteCode = branchcode + "-" + yearCode + "-" + fdeptcode + "-" + finalnumber;
                        getcode.intCurrentCode = newcode;
                        newEmp.vchEmpFcode = finalCompleteCode;
                        newEmp.bitIsCorporateemp = false;
                        newEmp.bitIsUnitEmp = true;
                        Objentity.SaveChanges();
                    }
                    TempData["Success"] = "Employee recieved successfully, recieved employee added in active list from now!";
                    return RedirectToAction("ViewTransferEmp");
                }
                TempData["Error"] = "Employee detail not found check it again or contact to administrator!";
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region View Partial Authorization Candidate

        //Get All Partial Employee 
        public ActionResult ViewPartialAuthorization()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            if (Session["SessionDate"] != null)
            {
                DateTime NewDate1 = Convert.ToDateTime(Session["SessionDate"].ToString());
                DateTime NewDate2 = Convert.ToDateTime(NewDate1.ToString("yyyy-MM-dd"));
                var uploadscomp = (from e in Objentity.tblEmpAssesmentMas
                                   where e.dtDOJ>=NewDate2 && e.bitIsPartialAuthorised == true && e.bitIsUnjoined == false && e.bitIsLeft != true && e.intcode == code
                                   select e).ToList();
                if (uploadscomp.Count != 0)
                {
                    return View(uploadscomp);
                }
                else
                {
                    TempData["Empty"] = "No Partial Authorised Candidate In Database!";
                    return View();
                }
            }
            else
            {
                var uploadscomp = (from e in Objentity.tblEmpAssesmentMas
                                   where e.bitIsPartialAuthorised == true && e.bitIsUnjoined == false && e.bitIsLeft != true && e.intcode == code
                                   select e).ToList();
                if (uploadscomp.Count != 0)
                {
                    return View(uploadscomp);
                }
                else
                {
                    TempData["Empty"] = "No Partial Authorised Candidate In Database!";
                    return View();
                }
            }
        }

        //Upload final document
        public ActionResult UploadFinalDoc(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                UpdatePartialEmployee objmodel = new UpdatePartialEmployee();
                var getEmpData = (from e in Objentity.spGetPartialAuthorizedEmp(id, code) select e).FirstOrDefault();
                if (getEmpData != null)
                {
                    objmodel.empid = getEmpData.intid;
                    if (getEmpData.vchEmpFcode != null)
                    {
                        objmodel.fcode = getEmpData.vchEmpFcode;
                    }
                    else
                    {
                        objmodel.fcode = "Not Assign";
                    }
                    if (getEmpData.vchEmpTcode != null)
                    {
                        objmodel.tcode = getEmpData.vchEmpTcode;
                    }
                    else
                    {
                        objmodel.tcode = "Not Assign";
                    }
                    objmodel.empname = getEmpData.vchname;
                    objmodel.FatherName = getEmpData.vchFatherName;
                    if (getEmpData.dtFDOJ != null)
                    {
                        objmodel.DOJ = (DateTime)getEmpData.dtFDOJ;
                    }
                    else
                    {
                        objmodel.DOJ = (DateTime)getEmpData.dtDOJ;
                        // objmodel.FatherName=selecetdemp.tblEmpDetails.
                    }
                    return View(objmodel);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
            return View();
        }

        public ActionResult PartialCompDocument(int id)
        {
            if (Session["descript"] != null)
            {
                //check emp id
                if (id != 0)
                {
                    var selecetdemp = (from e in Objentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    //check emp details avilable or not
                    if (selecetdemp != null)
                    {
                        int positionid = selecetdemp.fk_PositionId;

                        int code = Convert.ToInt32(Session["id"].ToString());
                        List<tblPosDocMap> New_list = new List<tblPosDocMap>();
                        var getCompDoc = (from e in Objentity.tblPosDocMap where e.fk_PosCatid == selecetdemp.fk_PositionId && e.IsSelected==true select e).ToList();
                        var compdoc = (from e in Objentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompDoc == true && e.intcode == code select e).ToList();
                        if(getCompDoc.Count()!=0 && compdoc.Count() != 0)
                        {
                            foreach(var Doc in getCompDoc)
                            {
                                foreach(var uploaded in compdoc)
                                {
                                    if (Doc.fk_docid == uploaded.fk_intdocid)
                                    {
                                        Doc.BitIsUploaded = true;
                                        Doc.ComparedFileName = uploaded.vchfilename;
                                        Doc.fk_TempEmpId = uploaded.fk_empAssid;
                                        if (uploaded.dtcreated != null)
                                        {
                                            Doc.dt_TempUploaded = uploaded.dtcreated;
                                        }
                                    }
                                    Doc.fk_TempEmpId = uploaded.fk_empAssid;
                                }
                                New_list.Add(Doc);
                            }
                        }
                        if (compdoc != null)
                        {
                            int positionID = selecetdemp.fk_PositionId;                           
                            return View(New_list);
                        }
                        else
                        {
                            TempData["Empty"] = "Compulsory document not found please chect it!";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["Empty"] = "Employee document not found in database!";
                        return View();
                    }
                }
                else
                {
                    TempData["Empty"] = "Compulsory document not found please chect it!";
                    return View();
                }             

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //Upload new document
        public ActionResult UploadNewDoc(int id, int docid)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedemp = (from e in Objentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                var selectedDoc = (from e in Objentity.tblDocMas where e.intid == docid select e).FirstOrDefault();
                if (selectedDoc != null)
                {
                    ViewBag.Docname = selectedDoc.vchdocname.ToString();
                }
                ViewBag.Mobile = selectedemp.vchMobile.ToString();
                ViewBag.Empname = selectedemp.vchName.ToString();            
                ViewBag.EmpID = id.ToString();
                //For compulsory document
                UpPartialAuthDocViewModel objmodel = new UpPartialAuthDocViewModel();
                int fk_possiid = selectedemp.fk_PositionId;                     
                objmodel.empid = selectedemp.intid;
                objmodel.fk_docid = docid;
                return View(objmodel);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult UploadNewDoc(UpPartialAuthDocViewModel objmdel) //,FormCollection fmcolect)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int year = Convert.ToInt32(Session["yr"].ToString());
                    //docdetails table object
                    tblDocDetails objdocdetail = new tblDocDetails();
                    int empid = objmdel.empid;
                    //get all emp id qualifications
                    int docid = objmdel.fk_docid;
                    //get mas doc
                    var MasDoc = (from e in Objentity.tblDocMas where e.intid == docid select e).FirstOrDefault();
                    //get mas data
                    var empmas = (from e in Objentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();            
                    //check is doc uploaded or not
                    var checkdoc = (from e in Objentity.tblDocDetails where e.fk_empAssid == empid && e.fk_intdocid == docid && e.intcode == code select e).FirstOrDefault();
                    if (objmdel.newpdfFile != null)
                        {
                            //string empid = empid.ToString();
                            string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                            string Final_datetime = datetime.Replace(" ", "");
                            string extension = Path.GetExtension(objmdel.newpdfFile.FileName);
                            if (extension != ".pdf")
                            {
                                TempData["Error"] = "Please select .pdf file for upload!";
                                return RedirectToAction("UploadNewDoc", new { id = empid });
                            }
                            else
                            {
                                string filename = Path.GetFileNameWithoutExtension(objmdel.newpdfFile.FileName);
                                string final_FileName = filename.Replace(" ", "");
                                string newfilename = "New" + "_" + final_FileName + Final_datetime + empid.ToString() + extension;
                                string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + newfilename));
                                //save file in upload folder
                                objmdel.newpdfFile.SaveAs(path);
                                objdocdetail.fk_empAssid = empid;
                                objdocdetail.fk_intdocid = docid;
                                objdocdetail.vchdocname =  MasDoc.vchdocname.ToString();
                                objdocdetail.vchfilename = newfilename.ToString();
                                objdocdetail.vchdocadd = path.ToString();
                                objdocdetail.vchcreatedby = Session["descript"].ToString();
                                objdocdetail.dtcreated = DateTime.Now;
                                objdocdetail.vchipused = Session["ipused"].ToString();
                                objdocdetail.vchhostname = Session["hostname"].ToString();
                                objdocdetail.intcode = code;
                                objdocdetail.intyr = year;
                                objdocdetail.BitIsCompDoc = true;
                                Objentity.tblDocDetails.Add(objdocdetail);
                                Objentity.SaveChanges();
                                TempData["Success"] = "Document upload successfully!";
                                return RedirectToAction("UploadFinalDoc", new { id = empid });
                            }
                        }
                    else
                        {
                            TempData["Error"] = "Select pdf file for upload!";
                            return View("UploadNewDoc", new { id = objmdel.empid });
                        }
                    
                }
                else
                {
                    //ModelState.AddModelError("newpdfFile", "ModelError Generated!");
                    TempData["Error"] = "Model error generated please try again with document and .pdf file selections or contact to administrator!";
                    return RedirectToAction("UploadNewDoc", new { id = objmdel.empid });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult UpPartialDoc(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedemp = (from e in Objentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                ViewBag.Mobile = selectedemp.vchMobile.ToString();
                ViewBag.Empname = selectedemp.vchName.ToString();
                ViewBag.EmpID = id.ToString();
                //For compulsory document
                DocCompulsoryModel objmodel = new DocCompulsoryModel();
                int fk_possiid = selectedemp.fk_PositionId;
                var compdoc = (from e in Objentity.tblPosDocMap
                               where e.fk_PosCatid == fk_possiid && e.IsSelected == true
                               select e).ToList();
                List<SelectListItem> compdocument = new List<SelectListItem>();
                foreach (var codoc in compdoc)
                {
                    SelectListItem selectListItem = new SelectListItem()
                    {
                        Text = codoc.tblDocMas.vchdocname.ToString(),
                        Value = codoc.tblDocMas.intid.ToString()
                    };
                    compdocument.Add(selectListItem);
                }
                objmodel.empid = selectedemp.intid;
                objmodel.compdoc = compdocument;
                objmodel.BitIsCompleted = selectedemp.bitCompDocP;
                return View(objmodel);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //Display detail only for candidate remarks and document for admin user
        public ActionResult ViewPartialAuthorisedDetail(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                UpdatePartialEmployee objmodel = new UpdatePartialEmployee();
                var getEmpData = (from e in Objentity.spGetPartialAuthorizedEmp(id, code) select e).FirstOrDefault();
                if (getEmpData != null)
                {
                    objmodel.empid = getEmpData.intid;
                    if (getEmpData.vchEmpFcode != null)
                    {
                        objmodel.fcode = getEmpData.vchEmpFcode;
                    }
                    else
                    {
                        objmodel.fcode = "Not Assign";
                    }
                    if (getEmpData.vchEmpTcode != null)
                    {
                        objmodel.tcode = getEmpData.vchEmpTcode;
                    }
                    else
                    {
                        objmodel.tcode = "Not Assign";
                    }
                    objmodel.empname = getEmpData.vchname;
                    objmodel.FatherName = getEmpData.vchFatherName;
                    if (getEmpData.dtFDOJ != null)
                    {
                        objmodel.DOJ = (DateTime)getEmpData.dtFDOJ;
                    }
                    else
                    {
                        objmodel.DOJ = (DateTime)getEmpData.dtDOJ;
                        // objmodel.FatherName=selecetdemp.tblEmpDetails.
                    }
                    return View(objmodel);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
            return View();
        }

        public ActionResult Completed(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                if (id != 0)
                {
                    //required document model object
                    List<tblDocMas> docList = new List<tblDocMas>();
                    var selectedEmp = (from e in Objentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                    if (selectedEmp.vchEmpFcode != null)
                    {
                        int posID = selectedEmp.fk_PositionId;
                        //get Required Doc int list
                        List<int> reqDoc = (from e in Objentity.tblPosDocMap where e.fk_PosCatid == posID && e.bitRequireForPartialToComplete select e.fk_docid).ToList();
                        //for uploaded doc
                        List<int> uploadedDoc = new List<int>();
                        //get all employee uploads
                        var UpDocuments = (from e in Objentity.tblDocDetails where e.fk_empAssid == id select e).ToList();
                        if (UpDocuments.Count() != 0)
                        {
                            foreach(var doc in UpDocuments)
                            {
                                int fkdocid = Convert.ToInt32(doc.fk_intdocid);
                                uploadedDoc.Add(fkdocid);
                            }
                        }
                        if (reqDoc.Count() != 0)
                        {
                            Boolean compareList = reqDoc.All(x => uploadedDoc.Contains(x));
                            if (compareList == false)
                            {
                                List<int> notUploaded = reqDoc.Except(uploadedDoc).ToList();
                                if (notUploaded.Count() != 0)
                                {
                                    foreach(var requiredDoc in notUploaded)
                                    {
                                        var getPending = (from e in Objentity.tblDocMas where e.intid == requiredDoc select e).FirstOrDefault();
                                        if (getPending != null)
                                        {
                                            docList.Add(getPending);
                                        }                                                
                                    }
                                    TempData["PendingDoc"] = docList;
                                    return RedirectToAction("UploadFinalDoc",new { id = id });
                                }
                            }
                        }
                        selectedEmp.bitIsPartialAuthorised = false;
                        selectedEmp.vchUpdatePartialToActive = Session["descript"].ToString();
                        selectedEmp.dtPartialUpdate = DateTime.Now;
                        Objentity.SaveChanges();
                        TempData["Success"] = "Employee removed successfully from partial employee authorized panel view!";
                        return RedirectToAction("ViewPartialAuthorization");
                    }
                    else
                    {
                        TempData["Error"] = "Employee final code not generated please generate it first from authorised view!";
                        return View();
                    }
                }
                else
                {
                    TempData["Empty"] = "Employee id should not be null!";
                    return RedirectToAction("ViewPartialAuthorization");
                }
            }
            else
            {
                //session error return
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //Button Back to List as cancel
        public ActionResult Cancel()
        {
            return RedirectToAction("ViewPartialAuthorization");
        }

        #endregion

        #region Aadhaar Number Cycle Searching, Redflag List
        public ActionResult AadhaarSearch()
        {
            //_PartialAadharResult(id);
            return View();
        }

        [HttpPost]
        public ActionResult AadhaarSearch(SearchAadhaarModelView objnew)
        {
            if (Session["descript"] != null)
            {
                if (objnew.aadhaarno != null)
                {
                   
                    var selectedCard = (from e in Objentity.tblEmpAssesmentMas where e.vchAadharNo == objnew.aadhaarno select e).ToList();
                    if (selectedCard.Count() != 0)
                    {
                        string id = objnew.aadhaarno.ToString();                                         
                        return View();
                    }
                    else
                    {
                        ViewBag.Error = "No record found in any indus branch!";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Aadhaar number should not be null or empty!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult _PartialInputAadhaar()
        {
            SearchAadhaarModelView objup = new SearchAadhaarModelView();
            return View();
        }

        public ActionResult _PartialAadharResult(SearchAadhaarModelView objmodel)
        {
            if (Session["descript"] != null)
            {
                if (objmodel.aadhaarno != null)
                {
                   var selectedCard = (from e in Objentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.aadhaarno select e).ToList();
                    if (selectedCard.Count() != 0)
                    {
                        return View("_PartialAadharResult", selectedCard);
                    }
                    else
                    {
                        ViewBag.Error = "The Aadhaar card number you entered not match any records in the system!";
                        return View("_PartialAadharResult", selectedCard);
                    }
                }
                else
                {
                    ViewBag.Error = "Card number should not be null or empty!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //view all red flagging employee
        public ActionResult RedFlagEmpList()
        {
            if (Session["descript"] != null)
            {
                

                var list = (from e in Objentity.tblEmpAssesmentMas where e.BitIsFlaggingEmp == true && e.BitIsRedFlagging == true orderby e.intcode select e).ToList();
                if (list.Count() != 0)
                {
                    ViewBag.Success = "FOUND " + list.Count() + " RED FLAG EMPLOYEE IN ALL INDUS UNITS";
                    return View(list);
                }
                else
                {
                    ViewBag.Empty = "0 records found in database";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }
        #endregion

        #region Charts
        //public ActionResult chartDash()
        //{
        //    if (Session["descript"] != null)
        //    {
        //        DateTime currentDate = DateTime.Now;
        //        // Get the first day of the current month
        //        DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        //        // Loop through each day of the month until today's date
        //        for (DateTime date = firstDayOfMonth; date <= currentDate; date = date.AddDays(1))
        //        {
        //            Console.WriteLine(date.ToShortDateString());
        //           // retun view(
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionErroe1", "Home");
        //    }
        //}
        #endregion

        #region Leave management/approval HR end and Manager
        public ActionResult ApprovedLeave()
        {
            if (base.Session["descript"] == null)
            {
                return base.RedirectToAction("_SessionError1", "Home");
            }
            int userid = Convert.ToInt32(base.Session["usrid"].ToString());
            int code = (int)Convert.ToInt16(base.Session["id"].ToString());
            string IsHOD = base.Session["DeptHOD"].ToString();
            string MainAdmin = base.Session["MainAdmin"].ToString();
            string HrAdmin = base.Session["HrAdmin"].ToString();
            string IsVPHR = Session["ISVPHR"].ToString();
            if (MainAdmin == "True" || HrAdmin == "True")
            {
                List<LeaveViewModel> getApprpvedList = (from e in this.Objentity.tblLeaveApplication
                                                             join d in this.Objentity.tblLeaveApplicationDetail on e.unID equals d.fk_AppID
                                                             where e.bitIsApproved == true && d.fk_AssignUserid == userid && e.intCode == code
                                                             select new LeaveViewModel { Master = e, Detail = d }).ToList();
                if (getApprpvedList.Count() != 0)
                {
                    return View(getApprpvedList);
                }
                else
                {
                    TempData["Empty"] = "0 Records found in database!";
                    return View();
                }
            }
            else if (IsVPHR == "True")
            {
                List<LeaveViewModel> getApprpvedList2 = (from e in this.Objentity.tblLeaveApplication
                                                         join d in this.Objentity.tblLeaveApplicationDetail on e.unID equals d.fk_AppID
                                                         where d.bitHODComplete == true && d.fk_AssignUserid == userid && e.bitISVpAssigned==true && e.intCode == code
                                                         select new LeaveViewModel { Master = e, Detail = d }).ToList();
                if (getApprpvedList2.Count() != 0)
                {
                    return View(getApprpvedList2);
                }
                base.TempData["Empty"] = "0 Records found in database!";
                return base.View();
            }
            else
            {
                List<LeaveViewModel> getApprpvedList2 = (from e in this.Objentity.tblLeaveApplication
                                                              join d in this.Objentity.tblLeaveApplicationDetail on e.unID equals d.fk_AppID
                                                              where d.bitHODComplete == true && d.fk_AssignUserid == userid && e.intCode == code
                                                         select new LeaveViewModel { Master = e, Detail = d }).ToList();
                
                if (getApprpvedList2.Count() != 0)
                {
                    return View(getApprpvedList2);
                }
                base.TempData["Empty"] = "0 Records found in database!";
                return base.View();
            }
        }

        public ActionResult UnitApprovedLeave()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            if (base.Session["descript"] == null)
            {
                return base.RedirectToAction("_SessionError1", "Home");
            }
            List<tblLeaveApplication> getApprpvedList = (from e in this.Objentity.tblLeaveApplication
                                                         join d in this.Objentity.tblLeaveApplicationDetail on e.unID equals d.fk_AppID
                                                         where e.bitIsApproved == true && e.bitIsRejected!=true && e.intCode == code
                                                         select e).ToList<tblLeaveApplication>();
            if (getApprpvedList.Count<tblLeaveApplication>() != 0)
            {
                return View(getApprpvedList);
            }
            else
            {
                TempData["Empty"] = "0 Records found in database!";
                return View();
            }

        }

        public ActionResult UnitApprovedExportToExcel()
        {
            int code = Convert.ToInt32(Session["id"].ToString());

            if (Session["descript"] == null)
            {
                return RedirectToAction("_SessionError1", "Home");
            }

            var getApprovedList = (from e in this.Objentity.tblLeaveApplication
                                   join d in this.Objentity.tblLeaveApplicationDetail on e.unID equals d.fk_AppID
                                   where e.bitIsApproved == true && e.bitIsRejected != true && e.intCode == code
                                   select e).ToList();

            if (!getApprovedList.Any())
            {
                TempData["Empty"] = "0 Records found in database!";
                return RedirectToAction("Index"); // Change this as per your view
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                DataTable dt = new DataTable("Approved Leaves");
                dt.Columns.Add("Name");
                dt.Columns.Add("Employee Code");
                dt.Columns.Add("Leave Type");
                dt.Columns.Add("From Date");
                dt.Columns.Add("To Date");
                dt.Columns.Add("Approved On");
                foreach (var item in getApprovedList)
                {
                    dt.Rows.Add(item.tblEmpAssesmentMas.vchName, item.tblEmpAssesmentMas.vchEmpFcode, item.tblLeaveType.leaveType, item.dtStartFrom, item.dtEndDate, item.dtApproved);
                }

                wb.Worksheets.Add(dt, "ApprovedLeaves");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    byte[] content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllUnitApprovedLeaves.xlsx");
                }
            }
        }

        public ActionResult AssignedLeave()
        {            
            if (base.Session["descript"] == null)
            {
                return base.RedirectToAction("_SessionError1", "Home");
            }
            int userid = Convert.ToInt32(base.Session["usrid"].ToString());
            int code = (int)Convert.ToInt16(base.Session["id"].ToString());
            string IsHOD = base.Session["DeptHOD"].ToString();
            string MainAdmin = base.Session["MainAdmin"].ToString();
            string HrAdmin = base.Session["HrAdmin"].ToString();
            string IsVPHR = Session["ISVPHR"].ToString();
            if (MainAdmin == "True")
            {
                List<tblLeaveApplication> getList = (from d in this.Objentity.tblLeaveApplicationDetail
                                                     join e in this.Objentity.tblLeaveApplication on d.fk_AppID equals e.unID
                                                     where d.bitHODComplete == false && d.bitHRComplete == false && e.bitIsApproved == false && e.bitIsRejected == false && e.intCode == code
                                                     select e).ToList<tblLeaveApplication>();
                if (getList.Count<tblLeaveApplication>() != 0)
                {
                    return base.View(getList);
                }
                base.TempData["Empty"] = "0 Records found in database!";
                return base.View();
            }         
            else if (HrAdmin == "True")
            {
                List<tblLeaveApplication> getList2 = (from d in this.Objentity.tblLeaveApplicationDetail
                                                      join e in this.Objentity.tblLeaveApplication on d.fk_AppID equals e.unID
                                                      where d.fk_AssignUserid == userid && d.bitISAssigned == (bool?)true && e.bitIsApproved != (bool?)true && e.intCode == code
                                                      select e).ToList<tblLeaveApplication>();
                if (getList2.Count<tblLeaveApplication>() != 0)
                {
                    return base.View(getList2);
                }
                base.TempData["Empty"] = "0 Records found in database!";
                return base.View();
            }
            else
            {
                List<tblLeaveApplication> getList2 = (from d in this.Objentity.tblLeaveApplicationDetail
                                                      join e in this.Objentity.tblLeaveApplication on d.fk_AppID equals e.unID
                                                      where d.fk_AssignUserid == userid && d.bitISAssigned == (bool?)true && d.bitISApproved != (bool?)true && e.intCode == code
                                                      select e).ToList<tblLeaveApplication>();
                if (getList2.Count<tblLeaveApplication>() != 0)
                {
                    return base.View(getList2);
                }
                base.TempData["Empty"] = "0 Records found in database!";
                return base.View();
            }
        }

        public ActionResult leaveShortStatus(System.Guid id)
        {
            if (id != null)
            {
                var getDetail = (from e in Objentity.tblLeaveApplicationDetail where e.fk_AppID == id select e).ToList();
                if (getDetail.Count() != 0)
                {
                    return PartialView("_partialleaveShortStatus", getDetail);
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


        public ActionResult Details(System.Guid id)
        {
            if (id != null)
            {
                int code = Convert.ToInt16(Session["id"].ToString());
                tblLeaveApplicationDetail getLeaveDetail = new tblLeaveApplicationDetail();
                var IsHOD = Session["DeptHOD"].ToString();
                var MainAdmin = Session["MainAdmin"].ToString();
                var IsHR = Session["HrAdmin"].ToString();
                if (IsHR =="True" || MainAdmin=="True")
                {
                    int HrID = Convert.ToInt32(Session["IS_BHR"].ToString());
                    getLeaveDetail = (from e in Objentity.tblLeaveApplicationDetail where e.fk_AppID == id  && e.fk_AssignUserid==HrID select e).FirstOrDefault();
                }
                if (IsHOD =="True")
                {
                    int HODID = Convert.ToInt32(Session["HOD_ID"].ToString());
                    getLeaveDetail = (from e in Objentity.tblLeaveApplicationDetail where e.fk_AppID == id && e.fk_AssignUserid==HODID select e).FirstOrDefault();
                }
                if (getLeaveDetail != null)
                {
                    var getMAsLEave = (from e in Objentity.tblLeaveApplication where e.unID == getLeaveDetail.fk_AppID select e).FirstOrDefault();
                    int empid = getMAsLEave.fk_Empid;
                    ViewBag.dtFrom = getLeaveDetail.tblLeaveApplication.dtStartFrom.ToString("dd/MM/yyyy");
                    ViewBag.dtTo = getLeaveDetail.tblLeaveApplication.dtEndDate.ToString("dd/MM/yyyy");
                    //for leave get
                    //get is emp or consultant
                    var getEmp = (from e in Objentity.tblEmpAssesmentMas where e.intid == getMAsLEave.fk_Empid select e).FirstOrDefault();
                    //get leave counter if taken before
                    if (getEmp.bitIsConsultant == true)
                    {
                        var checkOldleave = (from e in Objentity.spGetConsumedConsultantLeave(empid) select e).ToList();

                        List<SelectListItem> consumed = new List<SelectListItem>();
                        if (checkOldleave.Count() != 0)
                        {
                            foreach (var leave in checkOldleave)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Text = leave.leaveType,
                                    Value = leave.Leavecounter.ToString()
                                };
                                consumed.Add(selectListItem);
                            }
                            ViewBag.LeaveCounter = new SelectList(consumed, "Text", "Value");
                        }
                    }
                    else
                    {
                        var checkOldleave = (from e in Objentity.spGetConsumedLeave(empid) select e).ToList();
                        List<SelectListItem> consumed = new List<SelectListItem>();
                        List<ConsumedLeaveMV> lvConsumedModel = new List<ConsumedLeaveMV>();
                        if (checkOldleave.Count() != 0)
                        {
                            foreach (var leave in checkOldleave)
                            {
                                decimal totalAllowed = leave.decMaxDayPerYear ?? 0;
                                decimal halfLimit = 0;
                                if (leave.bitAllowYearHalfCheck == true)
                                {
                                    halfLimit = totalAllowed / 2;
                                    Boolean HalfDayLimit = false;
                                    decimal used = leave.Leavecounter;
                                    if (used >= halfLimit)
                                    {
                                        HalfDayLimit = true;
                                    }
                                    else
                                    {
                                        HalfDayLimit = false;
                                    }
                                    lvConsumedModel.Add(new ConsumedLeaveMV
                                    {
                                        LeaveType = leave.leaveType,
                                        ConsumedCount = used,
                                        HalfYearLimit = halfLimit,
                                        IsHalfYearReached = HalfDayLimit
                                    });
                                }
                                else
                                {
                                    decimal used = leave.Leavecounter;
                                    Boolean FullYearLimit = false;
                                    //compare with full year allowed
                                    if (used >= totalAllowed)
                                    {
                                        FullYearLimit = true;
                                    }
                                    else
                                    {
                                        FullYearLimit = false;
                                    }
                                    lvConsumedModel.Add(new ConsumedLeaveMV
                                    {
                                        LeaveType = leave.leaveType,
                                        ConsumedCount = used,
                                        HalfYearLimit = totalAllowed,
                                        IsHalfYearReached = FullYearLimit
                                    });
                                }
                            }
                            ViewBag.LeaveCounter = lvConsumedModel;
                        }
                     
                    
                
                        //if (checkOldleave.Count() != 0)
                        //{
                        //    foreach (var leave in checkOldleave)
                        //    {
                        //        SelectListItem selectListItem = new SelectListItem
                        //        {
                        //            Text = leave.leaveType,
                        //            Value = leave.Leavecounter.ToString()
                        //        };
                        //        consumed.Add(selectListItem);
                        //    }
                        //    ViewBag.LeaveCounter = new SelectList(consumed, "Text", "Value");
                        //}

                        //get all leave type for if adjustment applied type to another
                        List<tblLeaveType> ltype = new List<tblLeaveType>();
                        ltype = (from e in Objentity.tblLeaveType where e.bitForConsultant == false select e).ToList();
                        List<SelectListItem> types = new List<SelectListItem>();
                        types.Add(new SelectListItem { Text = "Select leave type", Value = "0" });
                        DateTime today = DateTime.Now;
                        int year = today.Year;
                        bool isFirstHalf = today.Month <= 6;
                        foreach (var type in ltype)
                        {
                            decimal carryOver = 0;
                            //decimal used = 0;
                            decimal totalAllowed = type.decMaxDayPerYear ?? 0;
                            var usedLeaves = Objentity.tblLeaveApplication.Where(l => l.fk_Empid == empid && l.fk_LeaveType == type.unID
                                && l.dtApproved.Value.Year == year && l.bitIsApproved == true).ToList();
                            //get half year leave
                            decimal usedFirstHalf = usedLeaves.Where(l => l.dtApproved.Value.Month <= 6)
                                                             .Sum(l => l.decdaysApproved ?? 0);
                            //get 1st half consulmed leave
                            decimal usedSecondHalf = usedLeaves.Where(l => l.dtApproved.Value.Month > 6)
                                                            .Sum(l => l.decdaysApproved ?? 0);
                            //get full year consumed leave
                            decimal fullYearConsumed = usedLeaves.Where(l => l.dtApproved.Value.Year == year)
                                                                .Sum(l => l.decdaysApproved ?? 0);

                            if (type.bitAllowYearHalfCheck == true)
                            {
                                decimal firstHalfLimit = totalAllowed / 2;
                                if (isFirstHalf)
                                {
                                    if (usedFirstHalf < firstHalfLimit)
                                    {
                                        types.Add(new SelectListItem
                                        {
                                            Text = type.leaveType,
                                            Value = type.unID.ToString()
                                        });
                                    }
                                }
                                else
                                {
                                    carryOver = Math.Max(firstHalfLimit - usedFirstHalf, 0);
                                    decimal totalAvailableSecondHalf = firstHalfLimit + carryOver;
                                    if (usedSecondHalf < totalAvailableSecondHalf)
                                    {
                                        types.Add(new SelectListItem
                                        {
                                            Text = type.leaveType,
                                            Value = type.unID.ToString()
                                        });
                                    }
                                }
                            }
                            else
                            {
                                if (fullYearConsumed < totalAllowed)
                                {
                                    types.Add(new SelectListItem
                                    {
                                        Text = type.leaveType,
                                        Value = type.unID.ToString()
                                    });
                                }
                                else
                                {

                                }
                            }
                        }
                        ViewBag.Types = new SelectList(types, "Text", "Value");
                    }                    
                    //put joined days since for HR only
                    return View(getLeaveDetail);

                }
                else
                {
                    TempData["Error"] = "Leave detail not found, contact to administrator!";
                    return RedirectToAction("AssignedLeave");
                }
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Details(tblLeaveApplicationDetail objdetail, tblLeaveApplication objmas)
        {
            if (base.Session["descript"] == null)
            {
                return base.RedirectToAction("_SessionError1", "Home");
            }
            int code = (int)Convert.ToInt16(base.Session["id"].ToString());
            string IsHOD = base.Session["DeptHOD"].ToString();
            string MainAdmin = base.Session["MainAdmin"].ToString();
            string HrAdmin = base.Session["HrAdmin"].ToString();
            Guid fk_AppID = objdetail.fk_AppID;
            Guid un_ID = objdetail.UN_ID;
            if (objdetail.fk_AssignUserid != 0)
            {
                tblLeaveApplication getMasLeave = (from e in this.Objentity.tblLeaveApplication
                                                   where e.unID == objdetail.fk_AppID
                                                   select e).FirstOrDefault<tblLeaveApplication>();
                tblLeaveApplicationDetail getLeaveDetail = (from e in this.Objentity.tblLeaveApplicationDetail
                                                            where e.UN_ID == objdetail.UN_ID && e.fk_AssignUserid == objdetail.fk_AssignUserid
                                                            select e).FirstOrDefault<tblLeaveApplicationDetail>();
                string msg = string.Empty;
                if (objdetail.vchGetApprovalType == "Same")
                {
                    msg = "Leave approved same as applied successfully!";
                    getMasLeave.vchStatus = "Approved";
                    if (HrAdmin != "True")
                    {
                        getMasLeave.TempBitHODComplete = true;
                        //getMasLeave.bitIsApproved = true;
                        getMasLeave.TempBitAssignedHR = true;
                        getMasLeave.vchStatus = "Pending";
                        getLeaveDetail.vchCurrentStatus = "Complete";
                        getLeaveDetail.bitHODComplete = true;
                        getLeaveDetail.bitIsSameApproved = true;
                        getLeaveDetail.bitISApproved = new bool?(true);
                        getLeaveDetail.dtApprovedRange = getMasLeave.leaveDateRange;
                        getLeaveDetail.decApprvedDays = new decimal?(getMasLeave.decdaysRequest);
                    }
                    if (HrAdmin == "True")
                    {
                        getMasLeave.vchStatus = "Approved";
                        getMasLeave.TempBitHRComplete = true;
                        getMasLeave.bitIsApproved = true;
                        getMasLeave.decdaysApproved = new decimal?(getMasLeave.decdaysRequest);
                        getMasLeave.appovedDateRange = getMasLeave.leaveDateRange;
                        getMasLeave.bitSameApprove = true;
                        getMasLeave.vchApprovedBy = base.Session["descript"].ToString();
                        getMasLeave.dtApproved = new DateTime?(DateTime.Now);
                        getLeaveDetail.vchCurrentStatus = "Complete";
                        getLeaveDetail.bitISApproved = new bool?(true);
                        getLeaveDetail.bitIsSameApproved = true;
                        getLeaveDetail.bitHRComplete = true;
                        getLeaveDetail.dtApprovedRange = getMasLeave.leaveDateRange;
                        getLeaveDetail.decApprvedDays = new decimal?(getMasLeave.decdaysRequest);
                    }
                    getLeaveDetail.bitIsSameApproved = true;
                }
                if (objdetail.vchGetApprovalType == "Partial")
                {
                    msg = "Leave approved as partial successfully!";
                    getMasLeave.vchStatus = "Partial Approved";
                    if (HrAdmin != "True")
                    {
                        getMasLeave.bitIsPartialApproved = true;
                        getMasLeave.decdaysApproved = objdetail.decApprvedDays;
                        getMasLeave.TempBitHODComplete = true;
                        getMasLeave.TempBitAssignedHR = true;
                        getMasLeave.fk_LeaveType = objdetail.tblLeaveApplication.fk_LeaveType;
                        getLeaveDetail.vchCurrentStatus = "Complete";
                        getLeaveDetail.bitHODComplete = true;
                        getLeaveDetail.bitISApproved = new bool?(true);
                        getLeaveDetail.BitIsPartialApproved = true;
                        getLeaveDetail.dtApprovedRange = objdetail.dtApprovedRange;
                        getLeaveDetail.decApprvedDays = objdetail.decApprvedDays;
                        getLeaveDetail.vchApproveRemarks = objdetail.vchApproveRemarks;
                    }
                    else if (HrAdmin == "True")
                    {
                        getMasLeave.vchStatus = "Approved";
                        getMasLeave.bitIsPartialApproved = true;
                        getMasLeave.bitIsApproved = true;
                        getMasLeave.TempBitAssignedHR = true;
                        getMasLeave.decdaysApproved = objdetail.decApprvedDays;
                        getMasLeave.appovedDateRange = objdetail.dtApprovedRange;
                        getMasLeave.fk_LeaveType = objdetail.tblLeaveApplication.fk_LeaveType;
                        getMasLeave.vchApprovedBy = base.Session["descript"].ToString();
                        getMasLeave.dtApproved = new DateTime?(DateTime.Now);
                        getLeaveDetail.vchCurrentStatus = "Complete";
                        getLeaveDetail.bitHRComplete = true;
                        getLeaveDetail.bitISApproved = new bool?(true);
                        getLeaveDetail.BitIsPartialApproved = true;
                        getLeaveDetail.dtApprovedRange = objdetail.dtApprovedRange;
                        getLeaveDetail.decApprvedDays = objdetail.decApprvedDays;
                    }
                }
                if (objdetail.vchGetApprovalType == "Rejected")
                {
                    msg = "Leave rejected successfully!";
                    if (HrAdmin != "True")
                    {
                        getMasLeave.bitIsRejected = true;
                        getMasLeave.vchStatus = "Rejected";
                        getMasLeave.decdaysApproved = new decimal?(0m);
                        getMasLeave.appovedDateRange = "N/A";
                        //getMasLeave.bitIsApproved = true;
                        getMasLeave.TempBitHODComplete = true;
                        getMasLeave.TempBitAssignedHR = true;
                        getMasLeave.vchApprovedBy = base.Session["descript"].ToString();
                        getLeaveDetail.vchCurrentStatus = "Rejected";
                        getLeaveDetail.bitHODComplete = true;
                        getLeaveDetail.decApprvedDays = new decimal?(0m);
                        getLeaveDetail.dtApprovedRange = "N/A";
                        getLeaveDetail.bitISApproved = new bool?(true);
                        getLeaveDetail.BitIsRejected = true;
                    }
                    else if (HrAdmin == "True")
                    {
                        getMasLeave.vchStatus = "Rejected";
                        getMasLeave.bitIsRejected = true;
                        getMasLeave.bitIsApproved = true;
                        getMasLeave.TempBitAssignedHR = true;
                        getMasLeave.vchApprovedBy = base.Session["descript"].ToString();
                        getMasLeave.dtApproved = new DateTime?(DateTime.Now);
                        getMasLeave.decdaysApproved = new decimal?(0m);
                        getMasLeave.appovedDateRange = "N/A";
                        getLeaveDetail.vchCurrentStatus = "Rejected";
                        getLeaveDetail.bitHRComplete = true;
                        getLeaveDetail.bitISApproved = new bool?(true);
                        getLeaveDetail.BitIsRejected = true;
                        getLeaveDetail.decApprvedDays = new decimal?(0m);
                        getLeaveDetail.dtApprovedRange = "N/A";
                    }
                }
                getLeaveDetail.vchApproveRemarks = objdetail.vchApproveRemarks;
                getLeaveDetail.vchGetApprovalType = objdetail.vchGetApprovalType;
                getLeaveDetail.vchApprovedBy = base.Session["descript"].ToString();
                getLeaveDetail.dtApproved = new DateTime?(DateTime.Now);
                getLeaveDetail.vchApprovedHost = base.Session["hostname"].ToString();
                getLeaveDetail.vchApprovedIP = base.Session["ipused"].ToString();
                this.Objentity.SaveChanges();
                base.TempData["Success"] = msg;
                return base.RedirectToAction("AssignedLeave");
            }
            base.TempData["Error"] = "Selecetd leave detail not found please contact to administrator!";
            return base.View();
        }

        public ActionResult ApprovedLeaveDetail(System.Guid id)
        {
            if (id != null)
            {
                int code = Convert.ToInt16(Session["id"].ToString());
                var IsHOD = Session["DeptHOD"].ToString();
                var MainAdmin = Session["MainAdmin"].ToString();
                var HrAdmin = Session["HrAdmin"].ToString();
                if (MainAdmin == "True")
                {
                    var getleaveDetail = (from e in Objentity.tblLeaveApplicationDetail
                                          join d in Objentity.tblLeaveApplication
                                          on e.fk_AppID equals d.unID
                                          where d.unID == id
                                          select e).FirstOrDefault();
                    if (getleaveDetail != null)
                    {
                        return View(getleaveDetail);
                    }
                    else
                    {
                        TempData["Empty"] = "Leave detail not found, try again or contact to administrator!";
                        return View();
                    }
                }
                    if (IsHOD == "True")
                {
                    //get hod id
                    int HODID = Convert.ToInt32(Session["HOD_ID"].ToString());
                    var getleaveDetail = (from e in Objentity.tblLeaveApplicationDetail
                                          join d in Objentity.tblLeaveApplication
                                          on e.fk_AppID equals d.unID
                                          where d.unID == id && e.fk_AssignUserid==HODID
                                          select e).FirstOrDefault();
                    if (getleaveDetail != null)
                    {
                        return View(getleaveDetail);
                    }
                    else
                    {
                        TempData["Empty"] = "Leave detail not found, try again or contact to administrator!";
                        return View();
                    }
                }
                if (HrAdmin =="True")
                {
                    //get hod id
                    int HRID = Convert.ToInt32(Session["IS_BHR"].ToString());
                    var getleaveDetail = (from e in Objentity.tblLeaveApplicationDetail
                                          join d in Objentity.tblLeaveApplication
                                          on e.fk_AppID equals d.unID
                                          where d.unID == id && e.fk_AssignUserid == HRID
                                          select e).FirstOrDefault();
                    if (getleaveDetail != null)
                    {
                        return View(getleaveDetail);
                    }
                    else
                    {
                        TempData["Empty"] = "Leave detail not found, try again or contact to administrator!";
                        return View();
                    }
                }
                return View();
            }
            else
            {
                return View();
            }
        }

        //when hr approved an leave to view HOD approved leave detail
        public ActionResult ForHRView(System.Guid id)
        {
            if (id != null)
            {
                var getleaveDetail = (from e in Objentity.tblLeaveApplicationDetail
                                      join d in Objentity.tblLeaveApplication
                                      on e.fk_AppID equals d.unID
                                      where d.unID == id && e.bitHODComplete==true
                                      select e).FirstOrDefault();
                if (getleaveDetail != null)
                {
                    return View(getleaveDetail);
                }
                else
                {
                    TempData["Empty"] = "Leave detail not found, try again or contact to administrator!";
                    return View();
                }

            }
            else
            {
                return View();
            }
        }

        public ActionResult LeaveStatus()
        {
            if (base.Session["descript"] != null)
            {
                int code = Convert.ToInt32(base.Session["id"].ToString());
                List<tblLeaveApplicationDetail> getList = Objentity.tblLeaveApplicationDetail.Where((tblLeaveApplicationDetail e) => e.intCode == (int?)code && e.bitHRComplete==false).ToList();
                return View(getList);
            }
            return RedirectToAction("_SessionError1", "Home");
        }

        public ActionResult LeaveReport()
        {
            if (base.Session["descript"] != null)
            {
                return View();
            }
            return RedirectToAction("_SessionError1", "Dashboard");
        }

        [HttpPost]
        public ActionResult LeaveReport(FormCollection fc)
        {
            if (base.Session["descript"] != null)
            {
                List<spLeaveReport_Result> objreport = new List<spLeaveReport_Result>();
                List<spLeaveReport_Result> objreport2 = new List<spLeaveReport_Result>();
                int? code = Convert.ToInt32(base.Session["id"].ToString());
                string dtrange = fc["dtraneguser"].ToString();
                string NSdate = dtrange.Split(' ')[0];
                string[] Edate = dtrange.Split('-');
                string NEdate = string.Empty;
                string[] array = Edate;
                foreach (string word in array)
                {
                    NEdate = word;
                }
                DateTime NewSdate1 = DateTime.ParseExact(NSdate.Trim().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime NewEdate1 = DateTime.ParseExact(NEdate.Trim().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fmdate = NewSdate1.ToString("dd/MM/yyyy");
                string todate = NewEdate1.ToString("dd/MM/yyyy");
                string FSdate = NewSdate1.ToString("yyyy-MM-dd HH:mm");
                string FEdate = NewEdate1.ToString("yyyy-MM-dd HH:mm");
                base.Session["Sdate"] = FSdate;
                base.Session["Edate"] = FEdate;
                if (FSdate != null && FEdate != null)
                {
                    objreport2 = (from e in Objentity.spLeaveReport(code, FSdate, FEdate)
                                  select (e)).ToList();
                    if (objreport2.Count() != 0)
                    {
                        base.TempData["Success"] = "Found  " + objreport2.Count() + " from date : " + fmdate + " to " + todate;
                        return View(objreport2);
                    }
                    base.TempData["Empty"] = "No record found from date: " + fmdate + " To " + todate;
                    return View();
                }
                base.TempData["Error"] = "Please select date range";
                return View();
            }
            return RedirectToAction("_SessionError1", "Dashboard");
        }

        public ActionResult LeaveExport()
        {
            string FSdate = base.Session["Sdate"].ToString();
            string FEdate = base.Session["Edate"].ToString();
            int code = Convert.ToInt32(base.Session["id"].ToString());
            if (FSdate != null && FEdate != null)
            {
                List<spLeaveReport_Result> selectedlist = (from e in Objentity.spLeaveReport(code, FSdate, FEdate)
                                                           select (e)).ToList();
                if (selectedlist.Count() != 0)
                {
                    DataTable dt = new DataTable("Grid");
                    dt.Columns.AddRange(new DataColumn[9]
                    {
                    new DataColumn("EmpCode"),
                    new DataColumn("Name"),
                    new DataColumn("Department"),
                    new DataColumn("DateFrom"),
                    new DataColumn("ToDate"),
                    new DataColumn("LeaveType"),
                    new DataColumn("Status"),
                    new DataColumn("Total"),
                    new DataColumn("Reason")
                    });
                    foreach (spLeaveReport_Result emp in selectedlist)
                    {
                        dt.Rows.Add(emp.vchEmpFcode, emp.vchName, emp.vchdeptname, emp.dtStartFrom.ToString("dd/MM/yyyy"), emp.dtEndDate.ToString("dd/MM/yyyy"), emp.leaveType, emp.vchStatus, emp.takenLeave, emp.vchReason);
                    }
                     XLWorkbook eb = new XLWorkbook();
                    eb.Worksheets.Add(dt);
                     MemoryStream ms = new MemoryStream();
                    eb.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LeaveReport.xlsx");
                }
                return View();
            }
            return View();
        }

        #endregion

        #region Consultant leave
        public ActionResult ConsultantNew()
        {
            if (base.Session["descript"] == null)
            {
                return base.RedirectToAction("_SessionError1", "Home");
            }
            int userid = Convert.ToInt32(base.Session["usrid"].ToString());
            int code = (int)Convert.ToInt16(base.Session["id"].ToString());
            string IsHOD = base.Session["DeptHOD"].ToString();
            string MainAdmin = base.Session["MainAdmin"].ToString();
            string HrAdmin = base.Session["HrAdmin"].ToString();
            string IsVPHR = Session["ISVPHR"].ToString();            
            if(MainAdmin == "True" || IsVPHR == "True")
            {
                List<tblLeaveApplication> getList = (from e in Objentity.tblLeaveApplication where e.bitISVpAssigned==true && e.bitISVpApproved==false && e.intCode==code select e 
                                                     ).ToList<tblLeaveApplication>();
                if (getList.Count<tblLeaveApplication>() != 0)
                {
                    return View(getList);
                }
                base.TempData["Empty"] = "0 Records found in database!";
                return base.View();
            }
            else
            {
                ViewBag.Error = "Model error contact to administrator!";
                return View();
            }
        }
        public ActionResult ConsultantApproved()
        {
            if (base.Session["descript"] == null)
            {
                return base.RedirectToAction("_SessionError1", "Home");
            }
            int userid = Convert.ToInt32(base.Session["usrid"].ToString());
            int code = (int)Convert.ToInt16(base.Session["id"].ToString());
            string IsHOD = base.Session["DeptHOD"].ToString();
            string MainAdmin = base.Session["MainAdmin"].ToString();
            string HrAdmin = base.Session["HrAdmin"].ToString();
            string IsVPHR = Session["ISVPHR"].ToString();
            //get all approved
            var getApproved = (from e in Objentity.tblLeaveApplication where e.bitISVpApproved == true && e.intCode == code select e).ToList();
            if (getApproved.Count() != 0)
            {
                return View(getApproved);
            }
            else
            {
                ViewBag.Error = "Empty database, 0 records found in database!";
                return View();
            }
        }
        public ActionResult ConsultantLeaveDeatil(System.Guid id)
        {
            if (base.Session["descript"] == null)
            {
                return base.RedirectToAction("_SessionError1", "Home");
            }
            int code = Convert.ToInt16(Session["id"].ToString());
            var getSelected = (from e in Objentity.tblLeaveApplication where e.unID == id select e).FirstOrDefault();
            if (getSelected != null)
            {
                //get is emp or consultant
                var getEmp = (from e in Objentity.tblEmpAssesmentMas where e.intid == getSelected.fk_Empid select e).FirstOrDefault();
                int empid = Convert.ToInt32(getSelected.fk_Empid);
                ViewBag.dtFrom = getSelected.dtStartFrom.ToString("dd/MM/yyyy");
                ViewBag.dtTo = getSelected.dtEndDate.ToString("dd/MM/yyyy");
                ViewBag.Type = getSelected.tblLeaveType.leaveType.ToString();
                //get leave counter if taken get
                if (getEmp.bitIsConsultant == true)
                {
                    //get consultant leave counter if taken before
                    var checkOldleave = (from e in Objentity.spGetConsumedConsultantLeave(empid) select e).ToList();
                    List<SelectListItem> consumed = new List<SelectListItem>();
                    List<ConsumedLeaveMV> lvConsumedModel = new List<ConsumedLeaveMV>();
                    if (checkOldleave.Count() != 0)
                    {
                        foreach (var leave in checkOldleave)
                        {
                            decimal totalAllowed = leave.decMaxDayPerYear ?? 0;
                            decimal halfLimit = 0;
                            if (leave.bitAllowYearHalfCheck == true)
                            {
                                halfLimit = totalAllowed / 2;
                                Boolean HalfDayLimit = false;
                                decimal used = leave.Leavecounter;
                                if (used >= halfLimit)
                                {
                                    HalfDayLimit = true;
                                }
                                else
                                {
                                    HalfDayLimit = false;
                                }
                                lvConsumedModel.Add(new ConsumedLeaveMV
                                {
                                    LeaveType = leave.leaveType,
                                    ConsumedCount = used,
                                    HalfYearLimit = halfLimit,
                                    IsHalfYearReached = HalfDayLimit
                                });
                            }
                            else
                            {
                                decimal used = leave.Leavecounter;
                                Boolean FullYearLimit = false;
                                //compare with full year allowed
                                if (used >= totalAllowed)
                                {
                                    FullYearLimit = true;
                                }
                                else
                                {
                                    FullYearLimit = false;
                                }
                                lvConsumedModel.Add(new ConsumedLeaveMV
                                {
                                    LeaveType = leave.leaveType,
                                    ConsumedCount = used,
                                    HalfYearLimit = totalAllowed,
                                    IsHalfYearReached = FullYearLimit
                                });
                            }
                        }
                        ViewBag.LeaveCounter = lvConsumedModel;
                    }
                }
                //get all leave type for if adjustment applied type to another
                List<tblLeaveType> ltype = new List<tblLeaveType>();
                ltype = (from e in Objentity.tblLeaveType where e.bitForConsultant == true select e).ToList();
                List<SelectListItem> types = new List<SelectListItem>();
                types.Add(new SelectListItem { Text = "Select leave type", Value = "0" });
                DateTime today = DateTime.Now;
                int year = today.Year;
                bool isFirstHalf = today.Month <= 6;
                foreach (var type in ltype)
                {
                    decimal carryOver = 0;
                    //decimal used = 0;
                    decimal totalAllowed = type.decMaxDayPerYear ?? 0;
                    var usedLeaves = Objentity.tblLeaveApplication.Where(l => l.fk_Empid == empid && l.fk_LeaveType == type.unID
                        && l.dtApproved.Value.Year == year && l.bitIsApproved == true).ToList();
                    //get half year leave
                    decimal usedFirstHalf = usedLeaves.Where(l => l.dtApproved.Value.Month <= 6)
                                                     .Sum(l => l.decdaysApproved ?? 0);
                    //get 1st half consulmed leave
                    decimal usedSecondHalf = usedLeaves.Where(l => l.dtApproved.Value.Month > 6)
                                                    .Sum(l => l.decdaysApproved ?? 0);
                    //get full year consumed leave
                    decimal fullYearConsumed = usedLeaves.Where(l => l.dtApproved.Value.Year == year)
                                                        .Sum(l => l.decdaysApproved ?? 0);

                    if (type.bitAllowYearHalfCheck == true)
                    {
                        decimal firstHalfLimit = totalAllowed / 2;
                        if (isFirstHalf)
                        {
                            if (usedFirstHalf < firstHalfLimit)
                            {
                                types.Add(new SelectListItem
                                {
                                    Text = type.leaveType,
                                    Value = type.unID.ToString()
                                });
                            }
                        }
                        else
                        {
                            carryOver = Math.Max(firstHalfLimit - usedFirstHalf, 0);
                            decimal totalAvailableSecondHalf = firstHalfLimit + carryOver;
                            if (usedSecondHalf < totalAvailableSecondHalf)
                            {
                                types.Add(new SelectListItem
                                {
                                    Text = type.leaveType,
                                    Value = type.unID.ToString()
                                });
                            }
                        }
                    }
                    else
                    {
                        if (fullYearConsumed < totalAllowed)
                        {
                            types.Add(new SelectListItem
                            {
                                Text = type.leaveType,
                                Value = type.unID.ToString()
                            });
                        }
                        else
                        {

                        }
                    }
                }
                ViewBag.Types = new SelectList(types, "Text", "Value");
                //return View(getSelected);
                return PartialView("_ConsultantLvApprovedAction", getSelected);
            }
            else
            {
                TempData["Error"] = "Leave detail not found contact to admninistrator!";
                return RedirectToAction("ConsultantNew");
            }
        }

        [HttpPost]
        public ActionResult SubmitLeaveQuickAction(tblLeaveApplication objmas)
        {
            if (base.Session["descript"] == null)
            {
                return base.RedirectToAction("_SessionError1", "Home");
            }
            int code = (int)Convert.ToInt16(base.Session["id"].ToString());
            string IsHOD = base.Session["DeptHOD"].ToString();
            string MainAdmin = base.Session["MainAdmin"].ToString();
            string HrAdmin = base.Session["HrAdmin"].ToString();
            Guid UID = objmas.unID;
            tblLeaveApplication getMasLeave = (from e in this.Objentity.tblLeaveApplication
                                               where e.unID == objmas.unID
                                               select e).FirstOrDefault<tblLeaveApplication>();
            string msg = string.Empty;
            if (objmas.vchApprovalType == "Same")
            {
                getMasLeave.vchStatus = "Approved";
                getMasLeave.vchApprovedBy = Session["descript"].ToString();
                getMasLeave.dtApproved = DateTime.Now;
                //getMasLeave.appovedDateRange=
                getMasLeave.bitSameApprove = true;
                getMasLeave.appovedDateRange = getMasLeave.leaveDateRange;
                getMasLeave.decdaysApproved = getMasLeave.decdaysRequest;
                getMasLeave.bitIsApproved = true;
                getMasLeave.bitISVpApproved = true;
                Objentity.SaveChanges();
                TempData["Success"] = "Leave approved same as appied successfully!";
                return RedirectToAction("ConsultantNew");
            }
            if (objmas.vchApprovalType == "Partial")
            {
                getMasLeave.vchStatus = "Partial Approved";
                getMasLeave.vchApprovedBy = Session["descript"].ToString();
                getMasLeave.dtApproved = DateTime.Now;
                //getMasLeave.appovedDateRange=
                getMasLeave.bitIsPartialApproved = true;
                getMasLeave.appovedDateRange = objmas.appovedDateRange;
                getMasLeave.decdaysApproved = objmas.decdaysApproved;
                getMasLeave.bitISVpApproved = true;
                getMasLeave.bitIsApproved = true;
                Objentity.SaveChanges();
                TempData["Success"] = "Leave approved same as appied successfully!";
                return RedirectToAction("ConsultantNew");
            }
            if (objmas.vchApprovalType == "Rejected")
            {
                getMasLeave.vchStatus = "Rejected";
                getMasLeave.vchApprovedBy = Session["descript"].ToString();
                getMasLeave.dtApproved = DateTime.Now;
                //getMasLeave.appovedDateRange=
                getMasLeave.decdaysApproved = 0;
                getMasLeave.bitIsRejected = true;
                getMasLeave.bitISVpApproved = true;              
                Objentity.SaveChanges();
                TempData["Success"] = "Leave approved same as appied successfully!";
                return RedirectToAction("ConsultantNew");
            }
            else
            {
                TempData["Error"] = "Select leave approval type, Leave does not approved try again!";
                return RedirectToAction("ConsultantNew");
            }           
        }

        public ActionResult ApprovedConsultantLeaveDetail(System.Guid id)
        {
            if (id != null)
            {
                int code = Convert.ToInt16(Session["id"].ToString());
                var IsHOD = Session["DeptHOD"].ToString();
                var MainAdmin = Session["MainAdmin"].ToString();
                var HrAdmin = Session["HrAdmin"].ToString();               
                var getleaveDetail = (from e in Objentity.tblLeaveApplication
                                          where e.unID == id
                                          select e).FirstOrDefault();
                if (getleaveDetail != null)
                {
                    return View(getleaveDetail);
                }
                else
                {
                    TempData["Empty"] = "Leave detail not found, try again or contact to administrator!";
                    return RedirectToAction("ConsultantApproved");
                }
               
            }
            else
            {
                TempData["Error"] = "Leave id should not be null, contact to administrator!";
                return View("ConsultantApproved");
            }
        }
        #endregion

        #region Active employee upload more documents as other documents
        public ActionResult MoreUpload(int id)
        {
            if (Session["descript"] != null)
            {
                //mas object
                List<tblOtherDocumentMas> objdoc = new List<tblOtherDocumentMas>();
                var getOtherDocumentMas = (from e in Objentity.tblOtherDocumentMas select e).ToList();
                var getDocDetail = (from e in Objentity.tblOtherDocDetail where e.fk_empAssid == id select e).ToList();
                if (getOtherDocumentMas.Count() != 0)
                {
                    foreach (var doc in getOtherDocumentMas)
                    {
                        foreach (var uploaded in getDocDetail)
                        {
                            if (doc.intid == uploaded.fk_intOtherdocid)
                            {
                                doc.bitIsSelected = true;
                                doc.vchCompareFileAdd = uploaded.vchfilename;
                                doc.dtTempUploaded = uploaded.dtcreated;
                                doc.vchTempDocName = uploaded.vchNameDisplay;
                            }
                        }
                        doc.TempEmpID = id;
                        objdoc.Add(doc);
                    }
                    return View(objdoc);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult UploadOtherDoc(int id, int docid)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedemp = (from e in Objentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                var selectedDoc = (from e in Objentity.tblOtherDocumentMas where e.intid == docid select e).FirstOrDefault();
                if (selectedDoc != null)
                {
                    ViewBag.Docname = selectedDoc.vchDocName.ToString();
                }
                ViewBag.Mobile = selectedemp.vchMobile.ToString();
                ViewBag.Empname = selectedemp.vchName.ToString();
                //For compulsory document
                UploadOtherDocument objmodel = new UploadOtherDocument();
                int fk_possiid = selectedemp.fk_PositionId;
                objmodel.empid = selectedemp.intid;
                objmodel.fk_docid = docid;
                return View(objmodel);
            }
            else
            {
                return RedirectToAction("SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult UploadOtherDoc(UploadOtherDocument objup)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int year = Convert.ToInt32(Session["yr"].ToString());
                    tblOtherDocDetail objdocdetail = new tblOtherDocDetail();
                    int empid = objup.empid;
                    //get all emp id qualifications
                    int docid = objup.fk_docid;
                    //get mas doc
                    var MasDoc = (from e in Objentity.tblOtherDocumentMas where e.intid == docid select e).FirstOrDefault();
                    //get mas data
                    var empmas = (from e in Objentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    //check is doc uploaded or not
                    var checkdoc = (from e in Objentity.tblOtherDocDetail where e.fk_intOtherdocid == empid && e.fk_intOtherdocid == docid && e.vchNameDisplay==objup.Name select e).FirstOrDefault();
                    if (objup.newpdfFile != null)
                    {
                        //string empid = empid.ToString();
                        string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                        string Final_datetime = datetime.Replace(" ", "");
                        string extension = Path.GetExtension(objup.newpdfFile.FileName);
                        if (extension != ".pdf")
                        {
                            TempData["Error"] = "Please select .pdf file for upload!";
                            return RedirectToAction("UpByPassDoc", new { id = empid });
                        }
                        else
                        {
                            string filename = Path.GetFileNameWithoutExtension(objup.newpdfFile.FileName);
                            string final_FileName = filename.Replace(" ", "");
                            string newfilename = final_FileName + Final_datetime + empid.ToString() + extension;
                            string finalfilename = newfilename.Replace(" ", "");
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/OtherDocuments/" + finalfilename));
                            //save file in upload folder
                            objup.newpdfFile.SaveAs(path);
                            objdocdetail.fk_empAssid = empid;
                            objdocdetail.fk_intOtherdocid = docid;
                            objdocdetail.vchDocName = MasDoc.vchDocName.ToString();
                            objdocdetail.vchNameDisplay = objup.Name;
                            objdocdetail.vchfilename = finalfilename.ToString();
                            objdocdetail.vchdocadd = path.ToString();
                            objdocdetail.vchcreatedby = Session["descript"].ToString();
                            objdocdetail.dtcreated = DateTime.Now;
                            objdocdetail.vchipused = Session["ipused"].ToString();
                            objdocdetail.vchhostname = Session["hostname"].ToString();                            
                            Objentity.tblOtherDocDetail.Add(objdocdetail);
                            Objentity.SaveChanges();
                            TempData["Success"] = "Document upload successfully!";
                            return RedirectToAction("MoreUpload", new { id = empid });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Select pdf file for upload!";
                        return View("MoreUpload", new { id = objup.empid });
                    }
                }
            
                else
                {
                    ModelState.AddModelError("Name", "Model Error generated try again or contact to administrator!");
                    return View();
                }
            }
            else
            {
                return RedirectToAction("SessionError1", "Home");
            }
        }

        public ActionResult ViewMultiple(int id, int docid)
        {
            if (Session["descript"] != null)
            {
                var getDetail = (from e in Objentity.tblOtherDocDetail where e.fk_empAssid == id && e.fk_intOtherdocid == docid select e).ToList();
                return View(getDetail);
            }
            else
            {
                return RedirectToAction("SessionError1", "Home");
            }
        }

        //to remove single record
        public ActionResult DeleteOtherDoc(int id,int empid)
        {
            if (Session["descript"] != null)
            {
                if (id != 0 && empid != 0)
                {
                    var selectedDoc = (from e in Objentity.tblOtherDocDetail where e.fk_empAssid == empid && e.fk_intOtherdocid == id select e).FirstOrDefault();
                    if (selectedDoc != null)
                    {
                        System.IO.File.Delete(selectedDoc.vchdocadd);
                        Objentity.tblOtherDocDetail.Remove(selectedDoc);
                        Objentity.SaveChanges();
                        TempData["Success"] = "Document deleted successfully!";
                        return RedirectToAction("MoreUpload", new { id = selectedDoc.fk_empAssid });
                    }
                    else
                    {
                        TempData["Error"] = "Document details not found please check it again and try!";
                        return RedirectToAction("MoreUpload", new { id = empid });
                    }
                }
                else
                {
                    TempData["Error"] = "Document id should not be null or 0";
                    return RedirectToAction("ActiveView");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
        //remove from multiple records
        public ActionResult DeleteOtherDocDetail(int id, int empid,int fkdocid)
        {
            if (Session["descript"] != null)
            {
                if (id != 0 && empid != 0 && fkdocid != 0)
                {
                    var selectedDoc = (from e in Objentity.tblOtherDocDetail where e.fk_empAssid == empid && e.intid == id && e.fk_intOtherdocid==fkdocid select e).FirstOrDefault();
                    if (selectedDoc != null)
                    {
                        System.IO.File.Delete(selectedDoc.vchdocadd);
                        Objentity.tblOtherDocDetail.Remove(selectedDoc);
                        Objentity.SaveChanges();
                        TempData["Success"] = "Document deleted successfully!";
                        return RedirectToAction("MoreUpload", new { id = selectedDoc.fk_empAssid });
                    }
                    else
                    {
                        TempData["Error"] = "Document details not found please check it again and try!";
                        return RedirectToAction("MoreUpload", new { id = empid });
                    }
                }
                else
                {
                    TempData["Error"] = "Document id should not be null or 0";
                    return RedirectToAction("ActiveView");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
        #endregion

        #region Consultant salary slips requests and complete request
        public ActionResult ConsultantRequest()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt16(Session["id"].ToString());
                    var getRequests = (from e in Objentity.tblConsultantSlips where e.bitIsRequest == true && e.intcode==code select e).ToList();
                    if (getRequests.Count() != 0)
                    {
                        return View(getRequests);
                    }
                    else
                    {
                        TempData["Empty"] = "0 Records found in database!";
                        return View();
                    }
               
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ProceedRequest(Guid id)
        {
            if (Session["descript"] != null)
            {
                var getDetail = (from e in Objentity.tblConsultantSlips where e.UNid == id select e).FirstOrDefault();
                tblConsultantSlipDetail objdetails = new tblConsultantSlipDetail();
                if (getDetail != null)
                {
                    if (getDetail.bitIsQuraterRequest == true)
                    {
                        //list for months
                        List<SelectListItem> monthlist = new List<SelectListItem>();
                        if (getDetail.fk_Quarter == 1)
                        {
                            monthlist = new List<SelectListItem>
                        {
                            new SelectListItem { Text = "October", Value = "10" },
                            new SelectListItem { Text = "November", Value = "11" },
                            new SelectListItem { Text = "December", Value = "12" }                          
                        };
                        }
                        if (getDetail.fk_Quarter == 2)
                        {
                            monthlist = new List<SelectListItem>
                        {
                            new SelectListItem { Text = "April", Value = "4" },
                            new SelectListItem { Text = "May", Value = "5" },
                            new SelectListItem { Text = "June", Value = "6" }
                        };
                        }
                        if (getDetail.fk_Quarter == 3)
                        {
                            monthlist = new List<SelectListItem>
                        {
                            new SelectListItem { Text = "July", Value = "7" },
                            new SelectListItem { Text = "August", Value = "8" },
                            new SelectListItem { Text = "September", Value = "9" }
                        };
                        }
                        if (getDetail.fk_Quarter == 4)
                        {
                            monthlist = new List<SelectListItem>
                        {
                            new SelectListItem { Text = "January", Value = "1" },
                            new SelectListItem { Text = "February", Value = "2" },
                            new SelectListItem { Text = "March", Value = "3" }
                        };
                        }
                        ViewBag.MonthList = (monthlist);
                    }
                    else if (getDetail.bitIsMonthRequest == true)
                    {
                        int fromMonth = Convert.ToInt32(getDetail.intFromMonth);
                        int toMonth = Convert.ToInt32(getDetail.intToMonth);
                        List<SelectListItem> monthlist = new List<SelectListItem>();
                        //get all selected month
                        var getAllMonth = (from e in Objentity.tblMonthMaster where e.intid >= fromMonth && e.intid <= toMonth select e).ToList();
                        foreach(var month in getAllMonth)
                        {
                            SelectListItem selectListItem = new SelectListItem                                
                                {
                                    Text=month.Month,
                                    Value=month.intid.ToString()
                                };
                            monthlist.Add(selectListItem);
                        }
                        ViewBag.MonthList = (monthlist);
                    }                    
                    objdetails.fk_SlipId = getDetail.UNid;
                    return PartialView("_PartialProceedRQST", objdetails);
                }
                else
                {
                    return RedirectToAction("ConsultantRequest");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult _PartialProceedRQST()
        {
            if (Session["descript"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult ProceedRequest(Guid fk_SlipId, string[] Months, decimal[] Fees, decimal[] TDS, decimal[] NetPayables)
        {
            if (Session["descript"] != null)
            {
                //get master entry
                var master = (from e in Objentity.tblConsultantSlips where e.UNid == fk_SlipId select e).FirstOrDefault();
                tblConsultantSlipDetail objnewdetail = new tblConsultantSlipDetail();
                for(int i = 0; i < Months.Length; i++)
                {
                    
                    objnewdetail.fk_SlipId = fk_SlipId;
                    objnewdetail.vchMonth = Months[i];
                    objnewdetail.decAmount = Fees[i];
                    objnewdetail.decTDS = TDS[i];
                    objnewdetail.decNetAmount = NetPayables[i];
                    objnewdetail.intYear =Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                    objnewdetail.vchCreatedBy = Session["descript"].ToString();
                    objnewdetail.dtCreated = DateTime.Now;
                    Objentity.tblConsultantSlipDetail.Add(objnewdetail);
                    Objentity.SaveChanges();               
                }
                //update temprary completion in master
                master.bitTempComplete = true;
                Objentity.SaveChanges();
                TempData["Success"] = "Request slip saved successfully!";
                return RedirectToAction("ConsultantRequest");
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PrintSlip(Guid id)
        {
            if (Session["descript"] != null)
            {
                var getSlip = (from e in Objentity.spGetConsultantSlip(id) select e).ToList();
                var getslipDetails = (from e in Objentity.spGetConsultantSlipDetail(id) select e).ToList();
                if(getSlip!=null && getslipDetails.Count() != 0)
                {
                    LocalReport lr = new LocalReport();
                    string filepath = String.Empty;
                    HttpClient _client = new HttpClient();                  

                    //get path
                    filepath = Path.Combine(Server.MapPath("~/Content/Report"), ("ConsultantSlip.rdl"));
                    //open streams
                    using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        lr.LoadReportDefinition(filestream);
                        lr.DataSources.Add(new ReportDataSource(name: "DataSet1", getSlip));
                        lr.DataSources.Add(new ReportDataSource(name: "DataSet2", getslipDetails));
                        ReportParameter ptr = new ReportParameter("UID", id.ToString());
                        lr.SetParameters(ptr);
                        byte[] pdfData = lr.Render("PDF");
                        return File(pdfData, contentType: "Application/pdf");
                    }
                }
                else
                {
                    TempData["Error"] = "Slip detail not found please check it again!";
                    return View();
                }              
            }
            else
            {
                return RedirectToAction("_SessionError1","Home");
            }
        }

        public ActionResult DoFinalPrint(Guid id)
        {
            if (Session["descript"] != null)
            {
                var getSlip = (from e in Objentity.tblConsultantSlips where e.UNid==id select e).FirstOrDefault();
                if (getSlip != null)
                {
                    getSlip.bitIsComplete = true;
                    Objentity.SaveChanges();
                    ViewBag.Success = "Print verified successfully!";
                    return RedirectToAction("ConsultantRequest");
                }
                else
                {
                    TempData["Error"] = "Slip detail not found please check it and try again!";
                    return RedirectToAction("ConsultantRequest");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

       //657210310000186
       //bkid0006572
    }
}

