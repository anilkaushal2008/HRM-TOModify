using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
using HRM.Models;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Data.Entity.Validation;
using Microsoft.Reporting.WebForms;
using System.Net.Http;

namespace HRM.Controllers
{
    public class AssesmentController : Controller
    {
        HRMEntities hrentity = new HRMEntities();

        #region Add, view, edit and remove question master action 
        // View all questions
        public ActionResult ViewAllQuest()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            hrentity = new HRMEntities();
            var Qlist = (from e in hrentity.tblAssQuestMas select e).ToList();
            if (Qlist.Count != 0)
            {
                return View(Qlist);
            }
            else
            {
                TempData["Empty"] = "No record found in database";
            }
            return View();
        }

        //Add new question
        public ActionResult AddnewQuest()
        {
            List<SelectListItem> Type = new List<SelectListItem>();
            Type.Add(new SelectListItem { Text = "Number", Value = "1" });
            Type.Add(new SelectListItem { Text = "Yes/No", Value = "2" });
            Type.Add(new SelectListItem { Text = "Selection", Value = "3" });
            ViewBag.Type = new SelectList(Type, "Text", "Value");
            return View();
        }

        [HttpPost]
        public ActionResult AddnewQuest(tblAssQuestMas objnew)
        {
            if (ModelState.IsValid)
            {
                hrentity = new HRMEntities();
                if (objnew.vchAnsType == "1")
                {
                    objnew.vchAnsType = "Number";
                }
                if (objnew.vchAnsType == "2")
                {

                    objnew.vchAnsType = "Yes/No";
                }
                if (objnew.vchAnsType == "3")
                {

                    objnew.vchAnsType = "Selection";
                }

                objnew.dtcreated = DateTime.Now;
                objnew.vchcreatedby = Session["descript"].ToString();
                objnew.vchhostname = Session["hostname"].ToString();
                objnew.vchipused = Session["ipused"].ToString();
                objnew.intcode = Convert.ToInt32(Session["id"].ToString());
                objnew.intyr = Convert.ToInt32(Session["yr"].ToString());
                hrentity.tblAssQuestMas.Add(objnew);
                hrentity.SaveChanges();
                TempData["Success"] = "New question added successfully!";
                return RedirectToAction("AddnewQuest");
            }
            TempData["Error"] = "Model error generated!";
            return View();
        }

        //Edit question

        public ActionResult QMasEdit(int id)
        {
            if (id != 0)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selected = (from e in hrentity.tblAssQuestMas where e.intqid == id select e).FirstOrDefault();
                if (selected != null)
                {
                    List<SelectListItem> Type = new List<SelectListItem>();
                    Type.Add(new SelectListItem { Text = "Number", Value = "1" });
                    Type.Add(new SelectListItem { Text = "Yes/No", Value = "2" });
                    Type.Add(new SelectListItem { Text = "Selection", Value = "3" });
                    ViewBag.Type = new SelectList(Type, "Text", "Value");
                    return View(selected);
                }
            }
            else
            {
                TempData["Error"] = "An error generated try again!";
                return View();
            }
            return View();
        }

        [HttpPost]
        public ActionResult QMasEdit(tblAssQuestMas obj)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            //check ans type
            if (obj.vchAnsType == "1")
            {
                obj.vchAnsType = "Number";
            }
            if (obj.vchAnsType == "2")
            {

                obj.vchAnsType = "Yes/No";
            }
            if (obj.vchAnsType == "3")
            {

                obj.vchAnsType = "Selection";
            }
            var updated = (from e in hrentity.tblAssQuestMas where e.intqid == obj.intqid select e).FirstOrDefault();
            updated.vchQuestion = obj.vchQuestion;
            updated.vchAnsType = obj.vchAnsType;
            updated.dtupdated = DateTime.Now;
            updated.vchupdatedby = Session["descript"].ToString();
            updated.vchupdatedipused = Session["ipused"].ToString();
            updated.vchupdatedhostname = Session["hostname"].ToString();
            hrentity.SaveChanges();
            TempData["Success"] = "question updated successfully!";
            return RedirectToAction("ViewAllQuest");
        }

        //Delete question
        public ActionResult DeleteQuest(int id)
        {
            if (id != 0)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selected = (from e in hrentity.tblAssQuestMas where e.intqid == id select e).FirstOrDefault();
                if (selected != null)
                {
                    hrentity.tblAssQuestMas.Remove(selected);
                    TempData["Success"] = "Question removed successfully!";
                }
            }
            return View();
        }

        #endregion

        #region Requisition

        public ActionResult IndexRequi()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["descript"].ToString());
                var MainAdmin = Session["MainAdmin"].ToString();
                var HrAdmin = Session["HrAdmin"].ToString();
                if (MainAdmin == "True" || HrAdmin == "True")
                {

                    return View();
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        public ActionResult NewRequisition()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                List<SelectListItem> types = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="New",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Replacement",
                                                 Value ="2"
                                                }
                                             };
                ViewBag.Types = new SelectList(types, "Value", "Text");
                //get all employee code               
                var getList = (from e in hrentity.tblEmpAssesmentMas where e.BitIsRedFlagging != true && (e.bittempstatusactive == true || e.bitIsPartialAuthorised == true) && e.bitIsConsultant != true && e.intcode == code select e).ToList();
                var EmpList = new List<SelectListItem>();
                EmpList.Add(new SelectListItem { Value = "Select employee code", Text = "0" });
                foreach (var emp in getList)
                {
                    if (emp.vchEmpFcode != null)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = emp.intid.ToString(),
                            Value = emp.vchEmpFcode.ToString()
                        };
                        EmpList.Add(selectListItem);
                    }                 
                }
                ViewBag.EmpList = new SelectList(EmpList, "Value", "Text");
                //get all position
                var allposii = (from e in hrentity.tblPositionCategoryMas
                                where e.BitDesiMapping == true
                                orderby e.vchPosCatName
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
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //to Select existing employee
        public JsonResult GetEMpList()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var getList = (from e in hrentity.tblEmpAssesmentMas where e.BitIsRedFlagging != true && (e.bittempstatusactive == true || e.bitIsPartialAuthorised == true) && e.bitIsConsultant != true && e.intcode == code select e).ToList();
                List<SelectListItem> EmpList = new List<SelectListItem>();
                //EmpList.Add(new SelectListItem { Text = "Select employee code", Value = "0" });
                foreach (var emp in getList)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Text = emp.intid.ToString(),
                        Value=emp.vchEmpFcode.ToString()
                    };
                    EmpList.Add(selectListItem);
                }

                return Json(EmpList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult NewRequisition(tblRequisition objdata)
        {
            if (Session["descript"] != null)
            {
                           
                int code = Convert.ToInt32(Session["id"].ToString());
                if (ModelState.IsValid)
                {                   
                 
                    if (objdata.vchReqType == 1)
                    {
                        objdata.vchTypeName = "New";
                        objdata.bitISNew = true;
                    }
                    else if(objdata.vchReqType==2)
                    {
                        objdata.BitIsReplacement = true;
                        objdata.vchTypeName = "Replacement";
                    }
                    if (objdata.fkReplacement_empid != null)
                    {
                        //get selected replacement
                        int empid = Convert.ToInt32(objdata.fkReplacement_empid);
                        var objSelected = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid select e).FirstOrDefault();
                        objdata.fkReplacement_empid = objdata.fkReplacement_empid;
                        objdata.vchRplcMentName = objSelected.vchName;
                        objdata.vchRplcMentEmpCode = objSelected.vchEmpFcode;
                    }                    
                    objdata.dtRequest = objdata.dtRequest;
                    objdata.vchRequestBy = Session["descript"].ToString();
                    objdata.vchRequestHost = Session["hostname"].ToString();
                    objdata.vchRequestIP = Session["ipused"].ToString();
                    objdata.intCode = code;
                    objdata.bitStatusPending = true;
                    //check is HOD or HR Manager OR VPHR
                    string ISHOD = Session["DeptHOD"].ToString();
                    string ISHR = Session["HrAdmin"].ToString();
                    string ISVPHR = Session["ISVPHR"].ToString();
                    if (ISHOD == "True")
                    {
                        objdata.bitRequestByMgr = true;
                        objdata.bitISAssignToHR = true;
                    }
                    else if (ISHR == "True")
                    {
                        objdata.bitISAssignFromHR = true;
                        objdata.bitISAssignToVPHR = true;
                    }
                    else if (ISVPHR == "True")
                    {
                        objdata.bitISAssignFromVPHR = true;
                        objdata.bitISAssignToCFO = true;
                    }
                    //generate requisition number
                    string req = "R";
                    var getNumberMas = (from e in hrentity.tblRequisitionNumberMas select e).FirstOrDefault();
                    int current = Convert.ToInt32(getNumberMas.intCurrent);
                    int newNumber = current + 1;
                    objdata.vchRNumber = req + newNumber.ToString();
                    getNumberMas.intCurrent = newNumber;
                    hrentity.tblRequisition.Add(objdata);
                    hrentity.SaveChanges();
                    TempData["Success"] = "Requisition saved and assigned successfully!";
                    return RedirectToAction("NewRequisition","Assesment");
                }
                else
                {
                    List<SelectListItem> types = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="New",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Replacement",
                                                 Value ="2"
                                                }
                                             };
                    ViewBag.Types = new SelectList(types, "Value", "Text");
                    var allposii = (from e in hrentity.tblPositionCategoryMas
                                    where e.BitDesiMapping == true
                                    orderby e.vchPosCatName
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
                    TempData["Error"] = "Model error generated please contact to administrator!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        } 

        public ActionResult ViewRequisition()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                //get assigned requisition
                string ISHOD = Session["DeptHOD"].ToString();
                string ISHR = Session["HrAdmin"].ToString();
                string ISVPHR = Session["ISVPHR"].ToString();
                if (ISHOD == "True")
                {
                    var getList = (from e in hrentity.tblRequisition where e.bitISAssignToHR == true && e.bitISForwardFromHR != true && e.bitStatusComplete!=true && e.intCode == code select e).ToList();
                    if (getList.Count() != 0)
                    {
                        return View(getList);
                    }
                    else
                    {
                        ViewBag.Error = "0 Requisition assigned to you now!";
                        return View();
                    }
                }
                else if (ISHR == "True" && ISVPHR != "True")
                {
                    var getList = (from e in hrentity.tblRequisition where e.bitISAssignToHR == true && e.bitISForwardFromHR != true && e.bitStatusComplete != true && e.intCode == code select e).ToList();
                    if (getList.Count() != 0)
                    {
                        return View(getList);
                    }
                    else
                    {
                        ViewBag.Error = "0 Requisition assigned to you now!";
                        return View();
                    }
                }
                else if (ISHR == "True" && ISVPHR == "True")
                {
                    var getList = (from e in hrentity.tblRequisition where e.bitISAssignToVPHR == true && e.bitISForwardFromVPHR != true && e.bitStatusComplete != true && e.intCode == code select e).ToList();
                    if (getList.Count() != 0)
                    {
                        return View(getList);
                    }
                    else
                    {
                        ViewBag.Error = "0 Requisition assigned to you now!";
                        return View();
                    }
                }
                else
                {
                    TempData["Empty"] = "0 Requisition found!";
                    return View("ViewRequisition");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult GetRequisition(int id)
        {
            if (Session["descript"] != null)
            {
                var getDetail = (from e in hrentity.tblRequisition where e.intid == id select e).FirstOrDefault();
                if (getDetail != null)
                {
                    PartialRequisitionActionView(id);
                    return View(getDetail);
                }
                else
                {
                    TempData["Error"] = "Details not found pleace check it or contact to administrator!";
                    return RedirectToAction("ViewRequisition");
                }
            }
            else
            {
                return RedirectToAction("_SessionError","Home");
            }
        }

        [HttpPost]
        public ActionResult GetRequisition(tblRequisition objtable, RequisitionAuthoriseModel objmodel)
        {
            if (Session["descript"] != null)
            {
                var getSelecetd = (from e in hrentity.tblRequisition where e.intid == objmodel.id select e).FirstOrDefault();
                if (getSelecetd != null) {
                    if (ModelState.IsValid)
                    {
                        //check is HOD or HR Manager OR VPHR
                        string ISHOD = Session["DeptHOD"].ToString();
                        string ISHR = Session["HrAdmin"].ToString();
                        string ISVPHR = Session["ISVPHR"].ToString();
                        if (objmodel.bitAction == "Approved")
                        {
                            if (ISHR == "True")
                            {
                                getSelecetd.bitISApprovedByHR = true;
                                getSelecetd.dtApprovedByHR = DateTime.Now;
                                getSelecetd.vchApprovalRemarks = objmodel.vchRemarks;
                                getSelecetd.bitStatusComplete = true;
                                getSelecetd.dtApproved = objmodel.dtApproved;
                                //getSelecetd.dtApproved = DateTime.Now;
                                getSelecetd.vchApprovedBy = Session["descript"].ToString();
                                getSelecetd.bitStatusPending = false;
                            }
                            if (ISVPHR == "True")
                            {
                                getSelecetd.bitISApprvoedByVPHR = true;
                                getSelecetd.dtApprovedByVPHR = DateTime.Now;
                                getSelecetd.vchApprovalRemarks = objmodel.vchRemarks;
                                getSelecetd.bitStatusComplete = true;
                                //getSelecetd.dtApproved = DateTime.Now;
                                getSelecetd.dtApproved = objmodel.dtApproved;
                                getSelecetd.vchApprovedBy = Session["descript"].ToString();
                                getSelecetd.bitStatusPending = false;
                            }
                            hrentity.SaveChanges();
                            TempData["Success"] = "Requisition approved successfully!";
                        }
                        if (objmodel.bitAction == "Forward")
                        {
                            if (ISHR == "True")
                            {
                                getSelecetd.bitISForwardFromHR = true;
                                getSelecetd.bitISAssignFromHR = true;
                                getSelecetd.bitISAssignToVPHR = true;
                                getSelecetd.dtForwardFromHR = DateTime.Now;
                                getSelecetd.vchApprovalRemarks = objmodel.vchRemarks;
                                getSelecetd.bitStatusForward = true;                               
                            }
                            if (ISVPHR == "True")
                            {
                                getSelecetd.bitISForwardFromVPHR = true;
                                getSelecetd.bitISAssignFromVPHR = true;
                                getSelecetd.bitISAssignToCFO = true;
                                getSelecetd.dtForwardFromVPHR = DateTime.Now;
                                getSelecetd.vchApprovalRemarks = objmodel.vchRemarks;
                                getSelecetd.bitStatusForward = true;
                            }
                            hrentity.SaveChanges();
                            TempData["Success"] = "Requisition Forwarded Successfully!";
                        }
                        if (objmodel.bitAction == "Cancel")
                        {
                            if (ISHR == "True")
                            {                                
                                getSelecetd.bitStatusCancel = true;
                                getSelecetd.dtCancel = DateTime.Now;
                                getSelecetd.vchApprovalRemarks = objmodel.vchRemarks;
                                getSelecetd.vchCancelBy = Session["descript"].ToString();
                                getSelecetd.bitCancelByHR = true;
                                getSelecetd.dtCancel = DateTime.Now;
                                getSelecetd.vchCancelBy = Session["descript"].ToString();
                                getSelecetd.bitStatusPending = false;                              
                            }
                            if (ISVPHR == "True")
                            {
                                getSelecetd.bitStatusCancel = true;
                                getSelecetd.dtCancel = DateTime.Now;
                                getSelecetd.vchApprovalRemarks = objmodel.vchRemarks;
                                getSelecetd.vchCancelBy = Session["descript"].ToString();
                                getSelecetd.bitCancelByVPHR = true;
                                getSelecetd.dtCancel = DateTime.Now;
                                getSelecetd.vchCancelBy = Session["descript"].ToString();
                                getSelecetd.bitStatusPending = false;
                            }
                            hrentity.SaveChanges();
                            TempData["Success"] = "Requisition Cancelled Successfully!";
                        }                        
                        return RedirectToAction("ViewRequisition");
                    }
                    else
                    {
                        TempData["Error"] = "Selected requisition detail not found so try again or contact to administrator!";
                        return View("ViewRequisition");
                    }
                }
                else
                {
                    TempData["error"] = "Model error generated retry or contact to administrator!";
                    return RedirectToAction("ViewRequisition");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ViewAllReq()
        {
            if (Session["descript"] != null)
            {
                //check is HOD or HR Manager OR VPHR
                string ISHOD = Session["DeptHOD"].ToString();
                string ISHR = Session["HrAdmin"].ToString();
                string ISVPHR = Session["ISVPHR"].ToString();
                int code = Convert.ToInt32(Session["id"].ToString());
                List<tblRequisition> getRequisitionList = new List<tblRequisition>();
                if (ISHOD == "True")
                {
                     getRequisitionList = (from e in hrentity.tblRequisition where e.bitRequestByMgr==true && e.intCode == code select e).ToList();
                   
                }
                if (ISHR == "True" && ISVPHR=="False")
                {
                     getRequisitionList = (from e in hrentity.tblRequisition where (e.bitISAssignToHR==true || e.bitISAssignToVPHR==true || e.bitRequestByHR == true) && e.intCode == code select e).ToList();
                }
                if (ISVPHR == "True")
                {
                     getRequisitionList = (from e in hrentity.tblRequisition where e.intCode == code select e).ToList();                    
                }
                if (getRequisitionList.Count() != 0)
                {
                    return View(getRequisitionList);
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

        public ActionResult PrintRequi(int id)
        {
            if (Session["descript"] != null)
            {
                var getRequi = (from e in hrentity.spGetRequisition(id) select e).ToList();
                if (getRequi != null)
                {
                    LocalReport lr = new LocalReport();
                    string filepath = String.Empty;
                    HttpClient _client = new HttpClient();
                    //get path                
                    filepath = Path.Combine(Server.MapPath("~/Content/Report"), ("Requisition.rdl"));
                    //open streams
                    using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        lr.LoadReportDefinition(filestream);
                        lr.DataSources.Add(new ReportDataSource(name: "DataSet1", getRequi));
                        ReportParameter ptr = new ReportParameter("id", id.ToString());
                        lr.SetParameters(ptr);
                        byte[] pdfData = lr.Render("PDF");
                        return File(pdfData, contentType: "Application/pdf");
                    }
                }
                else
                {
                    TempData["Error"] = "Selected requisition not found!";
                    return RedirectToAction("ViewAllRequi");
                }
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        public ActionResult PartialRequisitionActionView(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var requi = (from e in hrentity.tblRequisition where e.intid == id && e.intCode == code select e).FirstOrDefault();
                RequisitionAuthoriseModel objmodel = new RequisitionAuthoriseModel();
                objmodel.id = requi.intid;               
                return View(objmodel);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Emp assesment View, add, edit, delete actions, single assessment details, assign new, Fill Assessment 

        //View New Employee assesment /view to assign new assesment
        public ActionResult ViewAllAssesment()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectdata = (from e in hrentity.tblEmpAssesmentMas where e.BitStatus == false && e.bittempstatusactive != true && e.bitIsByPassEntry!=true && e.intcode == code select e).ToList();
                if (selectdata.Count() == 0)
                {
                    TempData["Empty"] = "No record found in database!";
                    return View();
                }
                return View(selectdata);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //Add new employee for assesment
        public ActionResult AssignNewAssement()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var allposii = (from e in hrentity.tblPositionCategoryMas
                                where e.BitDesiMapping == true orderby e.vchPosCatName
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
                var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                List<SelectListItem> Departments = new List<SelectListItem>();
                Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                foreach (var dept in allDept)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Text = dept.vchdeptname,
                        Value = dept.intid.ToString(),
                    };
                    Departments.Add(selectListItem);
                }
                ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                Session["Skill"] = "";
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpGet]
        public JsonResult DeptStatus(string dptID)
        {
            if (dptID != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int deptID = Convert.ToInt32(dptID);
                var getMapping = (from e in hrentity.tblManPowerDetail where e.fk_deptid == deptID && e.intCode == code select e).FirstOrDefault();
                if (getMapping != null)
                {
                    //check count
                    int AllowedCount = getMapping.intManPowerCount;
                    if (AllowedCount != 0)
                    {
                        int getCurrentCount = (from e in hrentity.tblEmpAssesmentMas
                                               where (e.bittempstatusactive == true || e.bitIsPartialAuthorised == true)
                                                && (e.bitstatusdeactive != true && e.bitIsLeft != true && e.bitIsUnjoined!=true && e.fk_intdeptid == deptID && e.intcode == code)
                                               select e).Count();
                        if (getCurrentCount >= AllowedCount)
                        {
                            var result = new
                            {
                                message = "You can't hire new candidate in this department",
                                Param1 = "Max manpower allowed: "+AllowedCount,
                                Param2 = "Your branch current manpower in this department: "+ getCurrentCount
                            };
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("", JsonRequestBehavior.AllowGet);
                        }                        
                    }
                    else
                    {
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                   
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
      
        public JsonResult getyouSkill(string result)
        {
            if (result != null)
            {
                Session["SkillTMarks"] = result.ToString();
                string output1 = "";
                return Json(new { Success = 1, output1, JsonRequestBehavior.AllowGet });
            }
            else
            {
                string output2 = "";
                return Json(new { Success = 2, output2, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public ActionResult AssignNewAssement(tblEmpAssesmentMas objnew, FormCollection form)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    int skilldata = Convert.ToInt32(Session["SkillTMarks"].ToString());
                    //int skilldata = 100; // for by pass skill testing
                    Session.Remove("SkillTMarks");
                    int code = Convert.ToInt32(Session["id"].ToString());
                    //decimal skillm = Convert.ToDecimal(objnew.decSkillMarks);
                    //string skillstatus = objnew.vchSkillStatus.ToString();
                    int poistionid = objnew.fk_PositionId;
                    int DeptId = Convert.ToInt32(objnew.fk_intdeptid);
                    //get dept mapping detail
                    var GetCounter = (from e in hrentity.tblManPowerDetail where e.intCode == code && e.fk_deptid == DeptId select e).FirstOrDefault();                   
                    int ActiveDeptCount=0;
                    int DeptCounter = 0;
                    if (GetCounter != null)
                    {
                        DeptCounter = Convert.ToInt32(GetCounter.intManPowerCount);
                        if (DeptCounter != 0)
                        {
                            var GetActiveEmpCount = (from e in hrentity.tblEmpAssesmentMas where (e.bittempstatusactive == true || e.bitIsPartialAuthorised == true) && e.bitstatusdeactive != true && e.bitIsLeft != true && e.bitIsUnjoined!=true && e.fk_intdeptid == DeptId && e.intcode == code select e).ToList();
                            if (GetActiveEmpCount.Count() != 0)
                            {
                                ActiveDeptCount = GetActiveEmpCount.Count();
                            }
                        }
                        else
                        {
                            //TempData["Error"] = "Department manpower mapping not found please contact to administrator!";
                            //return RedirectToAction("AssignNewAssement", "Assesment");
                            var checkskillm = (from e in hrentity.tblSkillMarksAndPositionMappMas where e.fk_PositionID == poistionid select e).FirstOrDefault();
                            //check positional mapped skill marks.  
                            if (checkskillm != null)
                            {
                                int fromskillnumb = checkskillm.intSkillMarksFm;
                                if (skilldata >= fromskillnumb)
                                {
                                    var CheckOnlyAadhar = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objnew.vchAadharNo && e.BitStatus == false select e).FirstOrDefault();
                                    var CheckAadharAssess = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objnew.vchAadharNo && e.BitStatus == true select e).FirstOrDefault();
                                    var checkAadhaar = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objnew.vchAadharNo && e.bittempstatusactive == true select e).FirstOrDefault();
                                    var checkAdharRedFlag = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objnew.vchAadharNo && e.BitIsFlaggingEmp == true select e).FirstOrDefault();
                                    if (checkAdharRedFlag != null && checkAdharRedFlag.BitIsFlaggingEmp == true)
                                    {
                                        //for red flag
                                        if (checkAdharRedFlag.BitIsRedFlagging == true)
                                        {
                                            ModelState.AddModelError("vchRplcmntName", "Entered aadhaar number is red flaggining so candidate not eligible");
                                            var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                            List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                            var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                            List<SelectListItem> Departments = new List<SelectListItem>();
                                            Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                            foreach (var dept in allDept)
                                            {
                                                SelectListItem selectListItem = new SelectListItem
                                                {
                                                    Text = dept.vchdeptname,
                                                    Value = dept.intid.ToString(),
                                                };
                                                Departments.Add(selectListItem);
                                            }
                                            ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                            return View();
                                        }
                                        //for orange flag
                                        else if (checkAdharRedFlag.BitIsOrangeFlagging == true)
                                        {
                                            DateTime aajkidate = DateTime.Now;
                                            var leftdatetime = checkAdharRedFlag.dtDOL;
                                            DateTime newjoindate = leftdatetime.Value.AddMonths(6);
                                            string joiningdate = newjoindate.ToString("dd/MM/yyyy");
                                            if (aajkidate < newjoindate)
                                            {
                                                ModelState.AddModelError("vchRplcmntName", "Entered candidate is orange flag candidate so candidate can re-join after " + joiningdate + " date");
                                                var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                                List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                                var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                                List<SelectListItem> Departments = new List<SelectListItem>();
                                                Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                                foreach (var dept in allDept)
                                                {
                                                    SelectListItem selectListItem = new SelectListItem
                                                    {
                                                        Text = dept.vchdeptname,
                                                        Value = dept.intid.ToString(),
                                                    };
                                                    Departments.Add(selectListItem);
                                                }
                                                ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                                return View();
                                            }
                                            else
                                            {
                                                string name = objnew.vchName.ToString();
                                                string mobile = objnew.vchMobile.ToString();
                                                var checkemp = (from e in hrentity.tblEmpAssesmentMas where e.vchName == name && e.vchMobile == mobile && e.intcode == code select e).FirstOrDefault();
                                                var checkmobile = (from e in hrentity.tblEmpAssesmentMas where e.vchMobile == mobile && e.intcode == code && e.bittempstatusactive == true select e).FirstOrDefault();
                                                if (checkemp != null)
                                                {
                                                    //TempData["Error"] = "Entered employee name and mobile are in used!";
                                                    var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                                    List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                                    var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                                    List<SelectListItem> Departments = new List<SelectListItem>();
                                                    Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                                    foreach (var dept in allDept)
                                                    {
                                                        SelectListItem selectListItem = new SelectListItem
                                                        {
                                                            Text = dept.vchdeptname,
                                                            Value = dept.intid.ToString(),
                                                        };
                                                        Departments.Add(selectListItem);
                                                    }
                                                    ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                                    ModelState.AddModelError("vchName", "Entered name in used!");
                                                    ModelState.AddModelError("vchMobile", "Entered mobile number is in used!");
                                                    return View();
                                                }
                                                else if (checkmobile != null)
                                                {
                                                    var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                                    List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                                    var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                                    List<SelectListItem> Departments = new List<SelectListItem>();
                                                    Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                                    foreach (var dept in allDept)
                                                    {
                                                        SelectListItem selectListItem = new SelectListItem
                                                        {
                                                            Text = dept.vchdeptname,
                                                            Value = dept.intid.ToString(),
                                                        };
                                                        Departments.Add(selectListItem);
                                                    }
                                                    ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                                    ModelState.AddModelError("vchMobile", "Entered mobile number are in used!");
                                                    return View();
                                                }
                                                objnew.dtcreated = DateTime.Now;
                                                objnew.vchcreatedby = Session["descript"].ToString();
                                                objnew.vchcreatedhost = Session["hostname"].ToString();
                                                objnew.vchcreatedipused = Session["ipused"].ToString();
                                                objnew.intcode = code;
                                                objnew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                                objnew.decSkillMarks = skilldata;
                                                hrentity.tblEmpAssesmentMas.Add(objnew);
                                                hrentity.SaveChanges();
                                                TempData["Success"] = "Employee details saved succesfully!";
                                                return RedirectToAction("AssignNewAssement");
                                            }
                                        }
                                        //for green flag
                                        else if (checkAdharRedFlag.BitIsGreenFlagging == true)
                                        {
                                            DateTime aajkidate = DateTime.Now;
                                            var leftdatetime = checkAdharRedFlag.dtDOL;
                                            DateTime newjoindate = leftdatetime.Value.AddMonths(3);
                                            string joiningdate = newjoindate.ToString("dd/MM/yyyy");
                                            if (aajkidate < newjoindate)
                                            {
                                                ModelState.AddModelError("vchRplcmntName", "Enter candidate is green flag candidate so candidate can re-join after " + joiningdate + " date");
                                                var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                                List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                                var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                                List<SelectListItem> Departments = new List<SelectListItem>();
                                                Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                                foreach (var dept in allDept)
                                                {
                                                    SelectListItem selectListItem = new SelectListItem
                                                    {
                                                        Text = dept.vchdeptname,
                                                        Value = dept.intid.ToString(),
                                                    };
                                                    Departments.Add(selectListItem);
                                                }
                                                ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                                return View();
                                            }
                                        }
                                    }
                                    //for new assessment
                                    else if (CheckOnlyAadhar != null)
                                    {
                                        ModelState.AddModelError("vchRplcmntName", "Entered aadhaar number is already in use new assessment list!");
                                        var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                        List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                        var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                        List<SelectListItem> Departments = new List<SelectListItem>();
                                        Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                        foreach (var dept in allDept)
                                        {
                                            SelectListItem selectListItem = new SelectListItem
                                            {
                                                Text = dept.vchdeptname,
                                                Value = dept.intid.ToString(),
                                            };
                                            Departments.Add(selectListItem);
                                        }
                                        ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                        return View();
                                    }
                                    //for new complete assessment
                                    else if (CheckAadharAssess != null)
                                    {
                                        ModelState.AddModelError("vchRplcmntName", "Entered aadhaar number is already in use assigned assessment list!");
                                        var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                        List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                        var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                        List<SelectListItem> Departments = new List<SelectListItem>();
                                        Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                        foreach (var dept in allDept)
                                        {
                                            SelectListItem selectListItem = new SelectListItem
                                            {
                                                Text = dept.vchdeptname,
                                                Value = dept.intid.ToString(),
                                            };
                                            Departments.Add(selectListItem);
                                        }
                                        ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                        return View();
                                    }
                                    //for active list use
                                    if (checkAadhaar != null)
                                    {
                                        ModelState.AddModelError("vchRplcmntName", "Entered aadhaar number is already in use active employee list!");
                                        var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                        List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                        var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                        List<SelectListItem> Departments = new List<SelectListItem>();
                                        Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                        foreach (var dept in allDept)
                                        {
                                            SelectListItem selectListItem = new SelectListItem
                                            {
                                                Text = dept.vchdeptname,
                                                Value = dept.intid.ToString(),
                                            };
                                            Departments.Add(selectListItem);
                                        }
                                        ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                        return View();
                                    }
                                    else
                                    {
                                        string name = objnew.vchName.ToString();
                                        string mobile = objnew.vchMobile.ToString();
                                        var checkemp = (from e in hrentity.tblEmpAssesmentMas where e.vchName == name && e.vchMobile == mobile && e.intcode == code select e).FirstOrDefault();
                                        var checkmobile = (from e in hrentity.tblEmpAssesmentMas where e.vchMobile == mobile && e.intcode == code && e.bittempstatusactive == true select e).FirstOrDefault();
                                        if (checkemp != null)
                                        {
                                            //TempData["Error"] = "Entered employee name and mobile are in used!";
                                            var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                            List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                            var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                            List<SelectListItem> Departments = new List<SelectListItem>();
                                            Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                            foreach (var dept in allDept)
                                            {
                                                SelectListItem selectListItem = new SelectListItem
                                                {
                                                    Text = dept.vchdeptname,
                                                    Value = dept.intid.ToString(),
                                                };
                                                Departments.Add(selectListItem);
                                            }
                                            ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                            ModelState.AddModelError("vchName", "Entered name in used!");
                                            ModelState.AddModelError("vchMobile", "Entered mobile number is in used!");
                                            return View();
                                        }
                                        else if (checkmobile != null)
                                        {
                                            var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                            List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                            var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                            List<SelectListItem> Departments = new List<SelectListItem>();
                                            Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                            foreach (var dept in allDept)
                                            {
                                                SelectListItem selectListItem = new SelectListItem
                                                {
                                                    Text = dept.vchdeptname,
                                                    Value = dept.intid.ToString(),
                                                };
                                                Departments.Add(selectListItem);
                                            }
                                            ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                            ModelState.AddModelError("vchMobile", "Entered mobile number are in used!");
                                            return View();
                                        }
                                        objnew.dtcreated = DateTime.Now;
                                        objnew.vchcreatedby = Session["descript"].ToString();
                                        objnew.vchcreatedhost = Session["hostname"].ToString();
                                        objnew.vchcreatedipused = Session["ipused"].ToString();
                                        objnew.intcode = code;
                                        objnew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                        objnew.decSkillMarks = skilldata;
                                        hrentity.tblEmpAssesmentMas.Add(objnew);
                                        hrentity.SaveChanges();
                                        TempData["Success"] = "Employee details saved succesfully!";
                                        return RedirectToAction("AssignNewAssement");
                                    }
                                }
                                else
                                {
                                    string name = objnew.vchName.ToString();
                                    string mobile = objnew.vchMobile.ToString();
                                    var checkemp = (from e in hrentity.tblEmpAssesmentMas where e.vchName == name && e.vchMobile == mobile && e.intcode == code select e).FirstOrDefault();
                                    var checkmobile = (from e in hrentity.tblEmpAssesmentMas where e.vchMobile == mobile && e.intcode == code && e.bittempstatusactive == true select e).FirstOrDefault();
                                    if (checkemp != null)
                                    {
                                        //TempData["Error"] = "Entered employee name and mobile are in used!";
                                        var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                        List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                        var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                        List<SelectListItem> Departments = new List<SelectListItem>();
                                        Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                        foreach (var dept in allDept)
                                        {
                                            SelectListItem selectListItem = new SelectListItem
                                            {
                                                Text = dept.vchdeptname,
                                                Value = dept.intid.ToString(),
                                            };
                                            Departments.Add(selectListItem);
                                        }
                                        ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                        ModelState.AddModelError("vchName", "Entered name in used!");
                                        ModelState.AddModelError("vchMobile", "Entered mobile number is in used!");
                                        return View();
                                    }
                                    else if (checkmobile != null)
                                    {
                                        var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                        List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                        var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                        List<SelectListItem> Departments = new List<SelectListItem>();
                                        Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                        foreach (var dept in allDept)
                                        {
                                            SelectListItem selectListItem = new SelectListItem
                                            {
                                                Text = dept.vchdeptname,
                                                Value = dept.intid.ToString(),
                                            };
                                            Departments.Add(selectListItem);
                                        }
                                        ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                        ModelState.AddModelError("vchMobile", "Entered mobile number are in used!");
                                        return View();
                                    }
                                    objnew.dtcreated = DateTime.Now;
                                    objnew.vchcreatedby = Session["descript"].ToString();
                                    objnew.vchcreatedhost = Session["hostname"].ToString();
                                    objnew.vchcreatedipused = Session["ipused"].ToString();
                                    objnew.intcode = code;
                                    objnew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                    objnew.decSkillMarks = skilldata;
                                    hrentity.tblEmpAssesmentMas.Add(objnew);
                                    hrentity.SaveChanges();
                                    TempData["Success"] = "Employee details saved succesfully!";
                                    return RedirectToAction("AssignNewAssement");
                                }
                            }
                            else
                            {
                                TempData["Error"] = "Skill mapping not found for current position, please map it first!";
                                return RedirectToAction("AssignNewAssement");
                            }
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Department not found in manpower mapping please contact to administrator!";
                        return RedirectToAction("AssignNewAssement", "Assesment");
                    }
                    if (ActiveDeptCount >= DeptCounter)
                    {
                        TempData["Error"] = "Department maximum manpower has reached, Maximum number is : " + DeptCounter.ToString();
                        return RedirectToAction("AssignNewAssement", "Assesment");
                    }
                    else
                    {
                        var checkskillm = (from e in hrentity.tblSkillMarksAndPositionMappMas where e.fk_PositionID == poistionid select e).FirstOrDefault();
                        //check positional mapped skill marks.  
                        if (checkskillm != null)
                        {
                            int fromskillnumb = checkskillm.intSkillMarksFm;
                            if (skilldata >= fromskillnumb)
                            {
                                var CheckOnlyAadhar = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objnew.vchAadharNo && e.BitStatus == false select e).FirstOrDefault();
                                var CheckAadharAssess = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objnew.vchAadharNo && e.BitStatus == true select e).FirstOrDefault();
                                var checkAadhaar = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objnew.vchAadharNo && e.bittempstatusactive == true select e).FirstOrDefault();
                                var checkAdharRedFlag = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objnew.vchAadharNo && e.BitIsFlaggingEmp == true select e).FirstOrDefault();
                                if (checkAdharRedFlag != null && checkAdharRedFlag.BitIsFlaggingEmp == true)
                                {
                                    //for red flag
                                    if (checkAdharRedFlag.BitIsRedFlagging == true)
                                    {
                                        ModelState.AddModelError("vchRplcmntName", "Entered aadhaar number is red flaggining so candidate not eligible");
                                        var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                        List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                        var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                        List<SelectListItem> Departments = new List<SelectListItem>();
                                        Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                        foreach (var dept in allDept)
                                        {
                                            SelectListItem selectListItem = new SelectListItem
                                            {
                                                Text = dept.vchdeptname,
                                                Value = dept.intid.ToString(),
                                            };
                                            Departments.Add(selectListItem);
                                        }
                                        ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                        return View();
                                    }
                                    //for orange flag
                                    else if (checkAdharRedFlag.BitIsOrangeFlagging == true)
                                    {
                                        DateTime aajkidate = DateTime.Now;
                                        var leftdatetime = checkAdharRedFlag.dtDOL;
                                        DateTime newjoindate = leftdatetime.Value.AddMonths(6);
                                        string joiningdate = newjoindate.ToString("dd/MM/yyyy");
                                        if (aajkidate < newjoindate)
                                        {
                                            ModelState.AddModelError("vchRplcmntName", "Enter candidate is orange flag candidate so candidate can re-join after " + joiningdate + " date");
                                            var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                            List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                            var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                            List<SelectListItem> Departments = new List<SelectListItem>();
                                            Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                            foreach (var dept in allDept)
                                            {
                                                SelectListItem selectListItem = new SelectListItem
                                                {
                                                    Text = dept.vchdeptname,
                                                    Value = dept.intid.ToString(),
                                                };
                                                Departments.Add(selectListItem);
                                            }
                                            ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                            return View();
                                        }
                                        else
                                        {
                                            string name = objnew.vchName.ToString();
                                            string mobile = objnew.vchMobile.ToString();
                                            var checkemp = (from e in hrentity.tblEmpAssesmentMas where e.vchName == name && e.vchMobile == mobile && e.intcode == code select e).FirstOrDefault();
                                            var checkmobile = (from e in hrentity.tblEmpAssesmentMas where e.vchMobile == mobile && e.intcode == code && e.bittempstatusactive == true select e).FirstOrDefault();
                                            if (checkemp != null)
                                            {
                                                //TempData["Error"] = "Entered employee name and mobile are in used!";
                                                var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                                List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                                var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                                List<SelectListItem> Departments = new List<SelectListItem>();
                                                Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                                foreach (var dept in allDept)
                                                {
                                                    SelectListItem selectListItem = new SelectListItem
                                                    {
                                                        Text = dept.vchdeptname,
                                                        Value = dept.intid.ToString(),
                                                    };
                                                    Departments.Add(selectListItem);
                                                }
                                                ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                                ModelState.AddModelError("vchName", "Entered name in used!");
                                                ModelState.AddModelError("vchMobile", "Entered mobile number is in used!");
                                                return View();
                                            }
                                            else if (checkmobile != null)
                                            {
                                                var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                                List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                                ModelState.AddModelError("vchMobile", "Entered mobile number are in used!");
                                                var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                                List<SelectListItem> Departments = new List<SelectListItem>();
                                                Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                                foreach (var dept in allDept)
                                                {
                                                    SelectListItem selectListItem = new SelectListItem
                                                    {
                                                        Text = dept.vchdeptname,
                                                        Value = dept.intid.ToString(),
                                                    };
                                                    Departments.Add(selectListItem);
                                                }
                                                ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                                return View();
                                            }
                                            objnew.dtcreated = DateTime.Now;
                                            objnew.vchcreatedby = Session["descript"].ToString();
                                            objnew.vchcreatedhost = Session["hostname"].ToString();
                                            objnew.vchcreatedipused = Session["ipused"].ToString();
                                            objnew.intcode = code;
                                            objnew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                            objnew.decSkillMarks = skilldata;
                                            hrentity.tblEmpAssesmentMas.Add(objnew);
                                            hrentity.SaveChanges();
                                            TempData["Success"] = "Employee details saved succesfully!";
                                            return RedirectToAction("AssignNewAssement");
                                        }
                                    }
                                    //for green flag
                                    else if (checkAdharRedFlag.BitIsGreenFlagging == true)
                                    {
                                        DateTime aajkidate = DateTime.Now;
                                        var leftdatetime = checkAdharRedFlag.dtDOL;
                                        DateTime newjoindate = leftdatetime.Value.AddMonths(3);
                                        string joiningdate = newjoindate.ToString("dd/MM/yyyy");
                                        if (aajkidate < newjoindate)
                                        {
                                            ModelState.AddModelError("vchRplcmntName", "Enter candidate is green flag candidate so candidate can re-join after " + joiningdate + " date");
                                            var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                            List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                            var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                            List<SelectListItem> Departments = new List<SelectListItem>();
                                            Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                            foreach (var dept in allDept)
                                            {
                                                SelectListItem selectListItem = new SelectListItem
                                                {
                                                    Text = dept.vchdeptname,
                                                    Value = dept.intid.ToString(),
                                                };
                                                Departments.Add(selectListItem);
                                            }
                                            ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                            return View();
                                        }
                                    }
                                }
                                //for new assessment
                                else if (CheckOnlyAadhar != null)
                                {
                                    ModelState.AddModelError("vchRplcmntName", "Entered aadhaar number is already in use new assessment list!");
                                    var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                    List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                    var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                    List<SelectListItem> Departments = new List<SelectListItem>();
                                    Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                    foreach (var dept in allDept)
                                    {
                                        SelectListItem selectListItem = new SelectListItem
                                        {
                                            Text = dept.vchdeptname,
                                            Value = dept.intid.ToString(),
                                        };
                                        Departments.Add(selectListItem);
                                    }
                                    ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                    return View();
                                }
                                //for new complete assessment
                                else if (CheckAadharAssess != null)
                                {
                                    ModelState.AddModelError("vchRplcmntName", "Entered aadhaar number is already in use assigned assessment list!");
                                    var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                    List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                    var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                    List<SelectListItem> Departments = new List<SelectListItem>();
                                    Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                    foreach (var dept in allDept)
                                    {
                                        SelectListItem selectListItem = new SelectListItem
                                        {
                                            Text = dept.vchdeptname,
                                            Value = dept.intid.ToString(),
                                        };
                                        Departments.Add(selectListItem);
                                    }
                                    ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                    return View();
                                }
                                //for active list use
                                if (checkAadhaar != null)
                                {
                                    ModelState.AddModelError("vchRplcmntName", "Entered aadhaar number is already in use active employee list!");
                                    var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                    List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                    var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                    List<SelectListItem> Departments = new List<SelectListItem>();
                                    Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                    foreach (var dept in allDept)
                                    {
                                        SelectListItem selectListItem = new SelectListItem
                                        {
                                            Text = dept.vchdeptname,
                                            Value = dept.intid.ToString(),
                                        };
                                        Departments.Add(selectListItem);
                                    }
                                    ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                    return View();
                                }
                                else
                                {
                                    string name = objnew.vchName.ToString();
                                    string mobile = objnew.vchMobile.ToString();
                                    var checkemp = (from e in hrentity.tblEmpAssesmentMas where e.vchName == name && e.vchMobile == mobile && e.intcode == code select e).FirstOrDefault();
                                    var checkmobile = (from e in hrentity.tblEmpAssesmentMas where e.vchMobile == mobile && e.intcode == code && e.bittempstatusactive == true select e).FirstOrDefault();
                                    if (checkemp != null)
                                    {
                                        //TempData["Error"] = "Entered employee name and mobile are in used!";
                                        var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                        List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                        var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                        List<SelectListItem> Departments = new List<SelectListItem>();
                                        Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                        foreach (var dept in allDept)
                                        {
                                            SelectListItem selectListItem = new SelectListItem
                                            {
                                                Text = dept.vchdeptname,
                                                Value = dept.intid.ToString(),
                                            };
                                            Departments.Add(selectListItem);
                                        }
                                        ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                        ModelState.AddModelError("vchName", "Entered name in used!");
                                        ModelState.AddModelError("vchMobile", "Entered mobile number is in used!");
                                        return View();
                                    }
                                    else if (checkmobile != null)
                                    {
                                        var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                        List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                        var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                        List<SelectListItem> Departments = new List<SelectListItem>();
                                        Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                        foreach (var dept in allDept)
                                        {
                                            SelectListItem selectListItem = new SelectListItem
                                            {
                                                Text = dept.vchdeptname,
                                                Value = dept.intid.ToString(),
                                            };
                                            Departments.Add(selectListItem);
                                        }
                                        ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                        ModelState.AddModelError("vchMobile", "Entered mobile number are in used!");
                                        return View();
                                    }
                                    objnew.dtcreated = DateTime.Now;
                                    objnew.vchcreatedby = Session["descript"].ToString();
                                    objnew.vchcreatedhost = Session["hostname"].ToString();
                                    objnew.vchcreatedipused = Session["ipused"].ToString();
                                    objnew.intcode = code;
                                    objnew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                    objnew.decSkillMarks = skilldata;
                                    hrentity.tblEmpAssesmentMas.Add(objnew);
                                    hrentity.SaveChanges();
                                    TempData["Success"] = "Employee details saved succesfully!";
                                    return RedirectToAction("AssignNewAssement");
                                }
                            }
                            else
                            {
                                string name = objnew.vchName.ToString();
                                string mobile = objnew.vchMobile.ToString();
                                var checkemp = (from e in hrentity.tblEmpAssesmentMas where e.vchName == name && e.vchMobile == mobile && e.intcode == code select e).FirstOrDefault();
                                var checkmobile = (from e in hrentity.tblEmpAssesmentMas where e.vchMobile == mobile && e.intcode == code && e.bittempstatusactive == true select e).FirstOrDefault();
                                if (checkemp != null)
                                {
                                    //TempData["Error"] = "Entered employee name and mobile are in used!";
                                    var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                    List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                    var allDept = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
                                    List<SelectListItem> Departments = new List<SelectListItem>();
                                    Departments.Add(new SelectListItem { Text = "Select department", Value = "0" });
                                    foreach (var dept in allDept)
                                    {
                                        SelectListItem selectListItem = new SelectListItem
                                        {
                                            Text = dept.vchdeptname,
                                            Value = dept.intid.ToString(),
                                        };
                                        Departments.Add(selectListItem);
                                    }
                                    ViewBag.AllDept = new SelectList(Departments, "Text", "Value");
                                    ModelState.AddModelError("vchName", "Entered name in used!");
                                    ModelState.AddModelError("vchMobile", "Entered mobile number is in used!");
                                    return View();
                                }
                                else if (checkmobile != null)
                                {
                                    var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true select e).ToList();
                                    List<SelectListItem> allpossi = new List<SelectListItem>();
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
                                    ModelState.AddModelError("vchMobile", "Entered mobile number are in used!");
                                    return View();
                                }
                                objnew.dtcreated = DateTime.Now;
                                objnew.vchcreatedby = Session["descript"].ToString();
                                objnew.vchcreatedhost = Session["hostname"].ToString();
                                objnew.vchcreatedipused = Session["ipused"].ToString();
                                objnew.intcode = code;
                                objnew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                objnew.decSkillMarks = skilldata;
                                hrentity.tblEmpAssesmentMas.Add(objnew);
                                hrentity.SaveChanges();
                                TempData["Success"] = "Employee details saved succesfully!";
                                return RedirectToAction("AssignNewAssement");
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Skill mapping not found for current position, please map it first!";
                            return RedirectToAction("ViewAllAssesment");
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Model error generated!";
                    return RedirectToAction("ViewAllAssesment");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }    

        //Send new assesment from view all
        public ActionResult SendAssesment(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                int empid = selectedemp.intid;
                if (selectedemp != null)
                {
                    string Candidate = selectedemp.vchName.ToString();
                    //tblEmpAssesmentMas objmas = new tblEmpAssesmentMas();
                    tblEmpAssesmentDetails objdetail = new tblEmpAssesmentDetails();
                    int selectedPosi = selectedemp.fk_PositionId;
                    var getuserdesi = (from e in hrentity.tblPosDesiMap where e.fk_PosCatid == selectedPosi && e.IsSelected == true select e).ToList();
                    if (getuserdesi.Count != 0)
                    {
                        foreach (var sel in getuserdesi)
                        {
                            int desiid = sel.fk_desiid;
                            objdetail.fk_AssEmpId = id;
                            objdetail.fk_positionid = selectedPosi;
                            //for username and user desiid
                            var selecteduser = (from e in hrentity.tblUserMaster where e.fk_intDesignationid == desiid && e.intcode == code && e.bitActive == true select e).FirstOrDefault();
                            #region for notifaction email
                            //check email id for email notification
                            //string TOid = selecteduser.vchEmail.ToString();
                            //if (TOid != null)
                            //{
                            //    //string Subject = "New candidate assessment assigned to you";
                            //string FromUsr = "it.support@indushospital.in";
                            //string Body = "New candidate " + selectedemp.vchName + " assigned to you please check your HRMS dashboard";
                            //var r = MailSender.SendEmail(Subject, Body, FromUsr, TOid);
                            //}
                            //else
                            //{
                            //    TempData["Error"] = "User email id not found please update email id";
                            //    return View("ViewAllAssesment");
                            //}
                            #endregion
                            if (selecteduser != null)
                            {
                                //check if user already assigned
                                int userid = selecteduser.intid;
                                var chkassigned = (from e in hrentity.tblEmpAssesmentDetails where e.fk_userid == userid && e.fk_AssEmpId == id select e).FirstOrDefault();
                                if (chkassigned == null)
                                {
                                    objdetail.fk_userid = selecteduser.intid;
                                    objdetail.vchAssignedToUser = selecteduser.vchUsername;
                                    objdetail.vchCurrentStatus = "Assigned";
                                    objdetail.intcode = code;
                                    objdetail.intyr = Convert.ToInt32(Session["yr"].ToString());
                                    hrentity.tblEmpAssesmentDetails.Add(objdetail);
                                    hrentity.SaveChanges();
                                    string mob = selecteduser.vchMobile.ToString();
                                    var uri = "http://164.52.195.161/API/SendMsg.aspx?uname=20170839&pass=Indus2017&send=HOSPIN&dest=" + mob + "&msg=" + Candidate + " Candidate assign for assessment to you, please login Indus HRMS %26 fill the assessment.Indus Healthcare Services Pvt. Ltd.&priority=1";
                                    if (uri != null)
                                    {
                                        try
                                        {
                                            HttpWebRequest createrequest = (HttpWebRequest)WebRequest.Create(uri);
                                            // getting response of sms
                                            HttpWebResponse myResp = (HttpWebResponse)createrequest.GetResponse();
                                            System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                                            string responseString = _responseStreamReader.ReadToEnd();
                                            _responseStreamReader.Close();
                                            myResp.Close();
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                            }
                            else
                            {
                                TempData["Error"] = "User not found please check position mapping and user!";
                                return RedirectToAction("ViewAllAssesment");
                            }
                        }
                        //master table entry for assesment status
                        selectedemp.vchAssignedBy = Session["descript"].ToString();
                        selectedemp.vchAssignhostname = Session["hostname"].ToString();
                        selectedemp.vchAssignipused = Session["ipused"].ToString();
                        selectedemp.dtAssign = DateTime.Now;
                        selectedemp.BitStatus = true;
                        string compcode = string.Empty;
                        if (empid != 0)
                        {
                            string tcode = empid.ToString();
                            string newcode = "T";
                            compcode = newcode + tcode;
                        }
                        if (compcode != null)
                        {
                            selectedemp.vchEmpTcode = compcode.ToString();                            
                        }
                        else
                        {
                            TempData["Success"] = "Employee Temp code not generated please contact to administrator!";
                            return RedirectToAction("ViewAllAssesment");
                        }
                        hrentity.SaveChanges();
                        TempData["Success"] = "Employee assessment assign successfully!";
                        return RedirectToAction("ViewAllAssesment");
                    }
                    else
                    {
                        TempData["Error"] = "Designation mapping not found against current position please map it First or Contact to administrator!";
                        return RedirectToAction("ViewAllAssesment");
                    }
                }
                else
                {
                    TempData["Error"] = "Model error generated, contact to administrator!";
                    return RedirectToAction("ViewAllAssesment");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }            
        }

        //Assign assesment view for all users to fill assessment
        public ActionResult ViewAssignedAss()
        {
            if (Session["descript"] != null)
            {
                //for admin user and HRadmin user
                //int user = Convert.ToInt16(Session["descript"].ToString();
                int code= Convert.ToInt16(Session["id"].ToString());
                var MainAdmin = Session["MainAdmin"].ToString();
                //var HrAdmin = Session["HrAdmin"].ToString();
                if (MainAdmin != "True") // && HrAdmin!="True")  //Only admin have assigned counts
                {

                    int usrid = Convert.ToInt32(Session["usrid"].ToString());
                    var selectedlist = (from e in hrentity.tblEmpAssesmentMas
                                        join a in hrentity.tblEmpAssesmentDetails on e.intid equals a.fk_AssEmpId
                                        where a.fk_userid == usrid && a.BitIsAssignCompleted == false && e.bitIsLeft!=true && e.bitIsConsultant!=true
                                        && e.intcode==code && a.intcode==code
                                        select e).ToList();
                    if (selectedlist.Count != 0)
                    {
                        return View(selectedlist);
                    }
                    else
                    {
                        TempData["Empty"] = "No new assessment avilable for you now!";
                        return View();
                    }
                }
                else
                {
                    //admin user view
                    int usrid = Convert.ToInt32(Session["usrid"].ToString());
                    var selectedlist = (from e in hrentity.tblEmpAssesmentMas
                                        join a in hrentity.tblEmpAssesmentDetails on e.intid equals a.fk_AssEmpId
                                        where a.BitIsAssignCompleted == false && e.bitIsLeft != true && e.intcode == code && e.bitIsConsultant!=true && a.intcode == code
                                        select e).ToList();
                    if (selectedlist.Count != 0)
                    {
                        return View(selectedlist);
                    }
                    else
                    {
                        TempData["Empty"] = "No new assesment avilable for admin!";
                        return View();
                    }
                }

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }   

        public ActionResult FillAssesment(int id)
        {
            //get all details of selected employee
            int code = Convert.ToInt16(Session["id"].ToString());
            var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode==code select e).FirstOrDefault();
            if (selectedemp != null)
            //get all question from quest master for assesment
            {
                List<SelectListItem> getallquest = new List<SelectListItem>();
                List<SelectListItem> getallqtype = new List<SelectListItem>();
                var allquest = (from e in hrentity.tblAssQuestMas select e).ToList();
                if (allquest.Count != 0)
                {
                    foreach (var q in allquest)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = q.vchQuestion.ToString(),
                            Value = q.intqid.ToString(),
                            Selected = Convert.ToBoolean(q.IsSelected),
                        };
                        getallquest.Add(selectListItem);
                    }

                }
                ViewBag.AllQuest = getallquest;
                return View(selectedemp);
            }
            else
            {
                return View();
            }

        }

        public JsonResult SaveData(string EmpID, string numberinput1, string numberinput2, string numberinput3, string Yescheck, string Nocheck, string remarks)
        {
            if (Session["descript"] != null)
            {
                //for last assessment from HR side
                var isAdminHr = Session["HrAdmin"].ToString();
                int code = Convert.ToInt16(Session["id"].ToString());
                if (isAdminHr != "True")
                {
                    if (numberinput1 == null || numberinput1 == "," || numberinput2 == null || numberinput2 == "," || numberinput3 == null || numberinput3 == ",")
                    {
                        var output1 = "Assessment number should not be null or empty!";
                        return Json(new { Success = 1, output1, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        int usrid = Convert.ToInt32(Session["usrid"].ToString());
                        int empid = Convert.ToInt32(EmpID);
                        tblAssesmentQuestDetails objdew = new tblAssesmentQuestDetails();
                        string[] qinput1 = numberinput1.Split(',');
                        int sum1 = 0; int sum2 = 0; int sum3 = 0;
                        //check ans value
                        foreach (var ans in qinput1)
                        {
                            int intval = Convert.ToInt32(ans);
                            if (intval <= 25)
                            {
                                sum1 = sum1 + intval;
                            }
                            else
                            {
                                var output2 = "Assessment number should not be greater than 25!";
                                return Json(new { Success = 2, output2, JsonRequestBehavior.AllowGet });
                            }
                        }
                        string[] qinput2 = numberinput2.Split(',');
                        //check ans value2
                        foreach (var ans in qinput2)
                        {
                            int intval = Convert.ToInt32(ans);
                            if (intval <= 20)
                            {
                                sum2 = sum2 + intval;
                            }
                            else
                            {
                                var output3 = "Assessment number should not be greater than 20!";
                                return Json(new { Success = 3, output3, JsonRequestBehavior.AllowGet });
                            }
                        }
                        //check ans value3
                        string[] qinput3 = numberinput3.Split(',');
                        foreach (var ans in qinput3)
                        {
                            int intval = Convert.ToInt32(ans);
                            if (intval <= 10)
                            {
                                sum3 = sum3 + intval;
                            }
                            else
                            {
                                var output4 = "Assessment number should not be greater than 10!";
                                return Json(new { Success = 4, output4, JsonRequestBehavior.AllowGet });
                            }
                        }
                        int total = sum1 + sum2 + sum3;
                        //All condition satisfied go ahead
                        string[] inputs = qinput1.Concat(qinput2).ToArray();
                        string[] allinput = inputs.Concat(qinput3).ToArray();
                        //count check for all answer
                        int getcount = allinput.Count();
                        if (getcount < 5)
                        {
                            var output5 = "Please fill all question answer in numbers!";
                            return Json(new { Success = 5, output5, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            var checkass = (from e in hrentity.tblAssesmentQuestDetails where e.fk_intEmpAssId == empid && e.fk_userid == usrid && e.intcode==code select e).ToList();
                            if (checkass.Count == 0)
                            {
                                //save all input number answer
                                int i = 1;
                                foreach (var ans in allinput)
                                {
                                    objdew.fk_intEmpAssId = empid;
                                    objdew.fk_userid = usrid;
                                    objdew.fk_qid = i;
                                    objdew.vchAnswer = ans.ToString();
                                    objdew.intTotal = total;
                                    objdew.dtAssesment = DateTime.Now;
                                    objdew.vchAssesmentBy = Session["descript"].ToString();
                                    objdew.vchAssesmentIpused = Session["ipused"].ToString();
                                    objdew.vchAssesmentHost = Session["hostname"].ToString();
                                    objdew.intcode = code;
                                    objdew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                    if (remarks != null && remarks != "")
                                    {
                                        objdew.vchSpecialRemarks = remarks.ToString();
                                    }
                                    else
                                    {
                                        objdew.vchSpecialRemarks = "Not Provided";
                                    }
                                    hrentity.tblAssesmentQuestDetails.Add(objdew);
                                    hrentity.SaveChanges();
                                    i++;
                                }
                                //save checkbox Yes and special remarks answer in database.
                                if (Yescheck != "" && Yescheck != null)
                                {
                                    string[] YesValeCheckbox = Yescheck.Split(',');
                                    foreach (var yes in YesValeCheckbox)
                                    {
                                        objdew.fk_intEmpAssId = empid;
                                        objdew.fk_qid = Convert.ToInt32(yes);
                                        objdew.vchAnswer = "Yes";
                                        objdew.dtAssesment = DateTime.Now;
                                        objdew.vchAssesmentBy = Session["descript"].ToString();
                                        objdew.vchAssesmentIpused = Session["ipused"].ToString();
                                        objdew.vchAssesmentHost = Session["hostname"].ToString();
                                        objdew.intcode = code;
                                        objdew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                        if (remarks != null && remarks != "")
                                        {
                                            objdew.vchSpecialRemarks = remarks.ToString();
                                        }
                                        else
                                        {
                                            objdew.vchSpecialRemarks = "Not Provided";
                                        }
                                        hrentity.tblAssesmentQuestDetails.Add(objdew);
                                        hrentity.SaveChanges();
                                    }
                                }
                                //save checkbox No and special remarks answer in database.
                                if (Nocheck != "" && Nocheck != null)
                                {
                                    string[] NoValeCheckbox = Nocheck.Split(',');
                                    foreach (var No in NoValeCheckbox)
                                    {
                                        objdew.fk_intEmpAssId = empid;
                                        objdew.fk_userid = usrid;
                                        objdew.fk_qid = Convert.ToInt32(No);
                                        objdew.vchAnswer = "No";
                                        objdew.dtAssesment = DateTime.Now;
                                        objdew.vchAssesmentBy = Session["descript"].ToString();
                                        objdew.vchAssesmentIpused = Session["ipused"].ToString();
                                        objdew.vchAssesmentHost = Session["hostname"].ToString();
                                        objdew.intcode = code;
                                        objdew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                        if (remarks != null && remarks != "")
                                        {
                                            objdew.vchSpecialRemarks = remarks.ToString();
                                        }
                                        else
                                        {
                                            objdew.vchSpecialRemarks = "Not Provided";
                                        }
                                        hrentity.tblAssesmentQuestDetails.Add(objdew);
                                        hrentity.SaveChanges();
                                        int userid = Convert.ToInt32(Session["usrid"].ToString());
                                        //update assigned user details in Assesment Details table(tblEmpAssesmentDetails)
                                        var getusrAss = (from e in hrentity.tblEmpAssesmentDetails where e.fk_userid == userid && e.fk_AssEmpId == empid && e.intcode==code select e).FirstOrDefault();
                                        getusrAss.dtCompleted = DateTime.Now;
                                        getusrAss.vchCompletedBy = Session["descript"].ToString();
                                        getusrAss.vchipused = Session["ipused"].ToString();
                                        getusrAss.vchhostname = Session["hostname"].ToString();
                                        getusrAss.vchCurrentStatus = "Completed";
                                        getusrAss.BitIsAssignCompleted = true;
                                        hrentity.SaveChanges();
                                    }
                                }
                                //Save answer according to total of number type question 9 to 12 no quest ans.
                                if (total != 0)
                                {
                                    for (int j = 9; j <= 12; j++)
                                    {
                                        objdew.fk_intEmpAssId = empid;
                                        objdew.fk_userid = usrid;
                                        objdew.fk_qid = j;
                                        if (j == 9)
                                        {
                                            if (total >= 86 && total <= 100)
                                            {
                                                objdew.vchAnswer = "Yes";
                                            }
                                            else
                                            {
                                                objdew.vchAnswer = "No";
                                            }
                                        }
                                        else if (j == 10)
                                        {
                                            if (total >= 61 && total <= 85)
                                            {
                                                objdew.vchAnswer = "Yes";
                                            }
                                            else
                                            {
                                                objdew.vchAnswer = "No";
                                            }
                                        }
                                        else if (j == 11)
                                        {
                                            if (total >= 36 && total <= 60)
                                            {
                                                objdew.vchAnswer = "Yes";
                                            }
                                            else
                                            {
                                                objdew.vchAnswer = "No";
                                            }
                                        }
                                        else if (j == 12)
                                        {
                                            if (total <= 35)
                                            {
                                                objdew.vchAnswer = "Yes";
                                            }
                                            else
                                            {
                                                objdew.vchAnswer = "No";
                                            }
                                        }
                                        objdew.intTotal = total;
                                        objdew.dtAssesment = DateTime.Now;
                                        objdew.vchAssesmentBy = Session["descript"].ToString();
                                        objdew.vchAssesmentIpused = Session["ipused"].ToString();
                                        objdew.vchAssesmentHost = Session["hostname"].ToString();
                                        objdew.intcode = code;
                                        objdew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                        hrentity.tblAssesmentQuestDetails.Add(objdew);
                                        hrentity.SaveChanges();
                                    }
                                }

                            }
                            else
                            {
                                var output8 = "Current user filled assesment is already present in database!";
                                return Json(new { Success = 8, output8, JsonRequestBehavior.AllowGet });
                            }
                            var output6 = "Assesment submitted successfully!";
                            return Json(new { Success = 6, output6, JsonRequestBehavior.AllowGet });
                        }
                    }
                }
                else
                {
                    //check all assessment filled or not
                    int id1 = Convert.ToInt32(EmpID);                    
                   var checkallstatus = (from e in hrentity.tblEmpAssesmentDetails where e.fk_AssEmpId == id1 && e.intcode==code select e).ToList();
                    var selected_candidate = (from e in hrentity.tblEmpAssesmentMas where e.intid == id1 && e.intcode == code select e).FirstOrDefault();
                    //get final assessment designation from position, desigantion mapping
                    int fk_positionID = Convert.ToInt32(selected_candidate.fk_PositionId);
                    var final_ass_desigantion = (from e in hrentity.tblPosDesiMap where e.fk_PosCatid == fk_positionID && e.bitIsLastAssessment == true select e).FirstOrDefault();
                    //get final assessment user using desigantion
                    int final_desi = Convert.ToInt32(final_ass_desigantion.fk_desiid);
                    var lastassessment_user = (from e in hrentity.tblUserMaster where e.fk_intDesignationid == final_desi && e.intcode==code && e.bitActive==true select e).FirstOrDefault();
                    if (checkallstatus.Count != 0)
                    {
                        if (code == 14)
                        {
                            foreach (var ststus in checkallstatus)
                            {

                                string hradmin = Session["HrAdmin"].ToString();
                                if (ststus.vchCurrentStatus == "Assigned"
                                    && ststus.BitIsAssignCompleted == false
                                    && ststus.vchAssignedToUser != lastassessment_user.vchUsername)
                                {
                                    var output7 = "Assessment pending from some user, HR Manager/VP HR assessment should be filled at last!";
                                    return Json(new { Success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                            }
                        }
                        else if(code==3)
                        {
                            foreach (var ststus in checkallstatus)
                            {
                                string hradmin = Session["HrAdmin"].ToString();
                                if (ststus.vchCurrentStatus == "Assigned" && 
                                    ststus.BitIsAssignCompleted == false && 
                                    ststus.vchAssignedToUser != lastassessment_user.vchUsername)
                                {
                                    var output7 = "Assessment pending from some user, HR Manager/VP HR assessment should be filled at last!";
                                    return Json(new { Success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                            }
                        }
                        else if(code==4)
                        {
                            foreach (var ststus in checkallstatus)
                            {
                                string hradmin = Session["HrAdmin"].ToString();
                                if ((ststus.vchCurrentStatus == "Assigned" && 
                                    ststus.BitIsAssignCompleted == false && 
                                    ststus.vchAssignedToUser != lastassessment_user.vchUsername))
                                {
                                    var output7 = "Assessment pending from some user, HR Manager/VP HR assessment should be filled at last!";
                                    return Json(new { Success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                            }
                        }
                        else if (code == 2)
                        {
                            foreach (var ststus in checkallstatus)
                            {
                                string hradmin = Session["HrAdmin"].ToString();
                                if (ststus.vchCurrentStatus == "Assigned" && 
                                    ststus.BitIsAssignCompleted == false && 
                                    ststus.vchAssignedToUser != lastassessment_user.vchUsername)
                                {
                                    var output7 = "Assessment pending from some user, HR Manager/VP HR assessment should be filled at last!";
                                    return Json(new { Success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                            }
                        }
                        else if(code==15)
                        {
                            foreach (var ststus in checkallstatus)
                            {
                                string hradmin = Session["HrAdmin"].ToString();
                                if (ststus.vchCurrentStatus == "Assigned" && 
                                    ststus.BitIsAssignCompleted == false && 
                                    ststus.vchAssignedToUser != lastassessment_user.vchUsername)
                                {
                                    var output7 = "Assessment pending from some user, HR Manager/VP HR assessment should be filled at last!";
                                    return Json(new { Success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                            }
                        }                      
                        else if (code == 16)
                        {
                            foreach (var ststus in checkallstatus)
                            {
                                string hradmin = Session["HrAdmin"].ToString();
                                if (ststus.vchCurrentStatus == "Assigned" && 
                                    ststus.BitIsAssignCompleted == false && 
                                    ststus.vchAssignedToUser != lastassessment_user.vchUsername)
                                {
                                    var output7 = "Assessment pending from some user, HR Manager/VP HR assessment should be filled at last!";
                                    return Json(new { Success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                            }
                        }
                        else if (code == 21)
                        {
                            foreach (var ststus in checkallstatus)
                            {
                                string hradmin = Session["HrAdmin"].ToString();
                                if (ststus.vchCurrentStatus == "Assigned" && 
                                    ststus.BitIsAssignCompleted == false && 
                                    ststus.vchAssignedToUser != lastassessment_user.vchUsername)
                                {
                                    var output7 = "Assessment pending from some user, HR Manager/VP HR assessment should be filled at last!";
                                    return Json(new { Success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                            }
                        }
                        else if (code == 22)
                        {
                            foreach (var ststus in checkallstatus)
                            {
                                string hradmin = Session["HrAdmin"].ToString();
                                if (ststus.vchCurrentStatus == "Assigned" && 
                                    ststus.BitIsAssignCompleted == false && 
                                    ststus.vchAssignedToUser != lastassessment_user.vchUsername)
                                {
                                    var output7 = "Assessment pending from some user, HR Manager/VP HR assessment should be filled at last!";
                                    return Json(new { Success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                            }
                        }
                        else if (code == 23)
                        {
                            foreach (var ststus in checkallstatus)
                            {
                                string hradmin = Session["HrAdmin"].ToString();
                                if (ststus.vchCurrentStatus == "Assigned" && 
                                    ststus.BitIsAssignCompleted == false && 
                                    ststus.vchAssignedToUser != lastassessment_user.vchUsername)
                                {
                                    var output7 = "Assessment pending from some user, HR Manager/VP HR assessment should be filled at last!";
                                    return Json(new { Success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                            }
                        }
                        else if(code==24)
                        {
                            foreach(var status in checkallstatus)
                            {
                                string hradmin = Session["HrAdmin"].ToString();
                                if(status.vchCurrentStatus=="Assigned" && status.BitIsAssignCompleted==false
                                    && status.vchAssignedToUser != lastassessment_user.vchUsername)
                                {
                                    var output7 = "Assessment pending from some user, HR Manager/VP HR assessment should be filled at last!";
                                    return Json(new { Success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                            }
                        }
                        else if (code == 25)
                        {
                            foreach (var status in checkallstatus)
                            {
                                string hradmin = Session["HrAdmin"].ToString();
                                if (status.vchCurrentStatus == "Assigned" && status.BitIsAssignCompleted == false
                                    && status.vchAssignedToUser != lastassessment_user.vchUsername)
                                {
                                    var output7 = "Assessment pending from some user, HR Manager/VP HR assessment should be filled at last!";
                                    return Json(new { Success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                            }
                        }
                        //If All Assesment Filled fill HR assessment
                        if (numberinput1 == null || numberinput1 == "," || numberinput2 == null || numberinput2 == "," || numberinput3 == null || numberinput3 == ",")
                        {
                            var output1 = "Assessment number should not be null or empty!";
                            return Json(new { Success = 1, output1, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            int usrid = Convert.ToInt32(Session["usrid"].ToString());
                            int empid = Convert.ToInt32(EmpID);
                            tblAssesmentQuestDetails objdew = new tblAssesmentQuestDetails();
                            string[] qinput1 = numberinput1.Split(',');
                            int sum1 = 0; int sum2 = 0; int sum3 = 0;
                            //check ans value
                            foreach (var ans in qinput1)
                            {
                                int intval = Convert.ToInt32(ans);
                                if (intval <= 25)
                                {
                                    sum1 = sum1 + intval;
                                }
                                else
                                {
                                    var output2 = "Assessment number should not be greater than 25!";
                                    return Json(new { Success = 2, output2, JsonRequestBehavior.AllowGet });
                                }
                            }
                            string[] qinput2 = numberinput2.Split(',');
                            //check ans value2
                            foreach (var ans in qinput2)
                            {
                                int intval = Convert.ToInt32(ans);
                                if (intval <= 20)
                                {
                                    sum2 = sum2 + intval;
                                }
                                else
                                {
                                    var output3 = "Assessment number should not be greater than 20!";
                                    return Json(new { Success = 3, output3, JsonRequestBehavior.AllowGet });
                                }
                            }
                            //check ans value3
                            string[] qinput3 = numberinput3.Split(',');
                            foreach (var ans in qinput3)
                            {
                                int intval = Convert.ToInt32(ans);
                                if (intval <= 10)
                                {
                                    sum3 = sum3 + intval;
                                }
                                else
                                {
                                    var output4 = "Assessment number should not be greater than 10!";
                                    return Json(new { Success = 4, output4, JsonRequestBehavior.AllowGet });
                                }
                            }
                            int total = sum1 + sum2 + sum3;
                            //All condition satisfied go ahead
                            string[] inputs = qinput1.Concat(qinput2).ToArray();
                            string[] allinput = inputs.Concat(qinput3).ToArray();
                            //count check for all answer
                            int getcount = allinput.Count();
                            if (getcount < 5)
                            {
                                var output5 = "Please fill all question answer in numbers!";
                                return Json(new { Success = 5, output5, JsonRequestBehavior.AllowGet });
                            }
                            else
                            {
                                var checkass = (from e in hrentity.tblAssesmentQuestDetails where e.fk_intEmpAssId == empid && e.fk_userid == usrid && e.intcode==code select e).ToList();
                                if (checkass.Count == 0)
                                {
                                    //save all input number answer
                                    int i = 1;
                                    foreach (var ans in allinput)
                                    {
                                        objdew.fk_intEmpAssId = empid;
                                        objdew.fk_userid = usrid;
                                        objdew.fk_qid = i;
                                        objdew.vchAnswer = ans.ToString();
                                        objdew.intTotal = total;
                                        objdew.dtAssesment = DateTime.Now;
                                        objdew.vchAssesmentBy = Session["descript"].ToString();
                                        objdew.vchAssesmentIpused = Session["ipused"].ToString();
                                        objdew.vchAssesmentHost = Session["hostname"].ToString();
                                        objdew.intcode = code;
                                        objdew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                        if (remarks != null && remarks != "")
                                        {
                                            objdew.vchSpecialRemarks = remarks.ToString();
                                        }
                                        else
                                        {
                                            objdew.vchSpecialRemarks = "Not Provided";
                                        }
                                        hrentity.tblAssesmentQuestDetails.Add(objdew);
                                        hrentity.SaveChanges();
                                        i++;
                                    }

                                    //save checkbox Yes and special remarks answer in database.
                                    if (Yescheck != "" && Yescheck != null)
                                    {
                                        string[] YesValeCheckbox = Yescheck.Split(',');
                                        foreach (var yes in YesValeCheckbox)
                                        {
                                            objdew.fk_intEmpAssId = empid;
                                            objdew.fk_qid = Convert.ToInt32(yes);
                                            objdew.vchAnswer = "Yes";
                                            objdew.dtAssesment = DateTime.Now;
                                            objdew.vchAssesmentBy = Session["descript"].ToString();
                                            objdew.vchAssesmentIpused = Session["ipused"].ToString();
                                            objdew.vchAssesmentHost = Session["hostname"].ToString();
                                            objdew.intcode = code;
                                            objdew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                            if (remarks != null && remarks != "")
                                            {
                                                objdew.vchSpecialRemarks = remarks.ToString();
                                            }
                                            else
                                            {
                                                objdew.vchSpecialRemarks = "Not Provided";
                                            }
                                            hrentity.tblAssesmentQuestDetails.Add(objdew);
                                            hrentity.SaveChanges();
                                        }
                                    }
                                    //save checkbox No and special remarks answer in database.
                                    if (Nocheck != "" && Nocheck != null)
                                    {
                                        string[] NoValeCheckbox = Nocheck.Split(',');
                                        foreach (var No in NoValeCheckbox)
                                        {
                                            objdew.fk_intEmpAssId = empid;
                                            objdew.fk_userid = usrid;
                                            objdew.fk_qid = Convert.ToInt32(No);
                                            objdew.vchAnswer = "No";
                                            objdew.dtAssesment = DateTime.Now;
                                            objdew.vchAssesmentBy = Session["descript"].ToString();
                                            objdew.vchAssesmentIpused = Session["ipused"].ToString();
                                            objdew.vchAssesmentHost = Session["hostname"].ToString();
                                            objdew.intcode = code;
                                            objdew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                            if (remarks != null && remarks != "")
                                            {
                                                objdew.vchSpecialRemarks = remarks.ToString();
                                            }
                                            else
                                            {
                                                objdew.vchSpecialRemarks = "Not Provided";
                                            }
                                            hrentity.tblAssesmentQuestDetails.Add(objdew);
                                            hrentity.SaveChanges();
                                            int userid = Convert.ToInt32(Session["usrid"].ToString());
                                            //update assigned user details in Assesment Details table(tblEmpAssesmentDetails)
                                            var getusrAss = (from e in hrentity.tblEmpAssesmentDetails where e.fk_userid == userid && e.fk_AssEmpId == empid && e.intcode==code select e).FirstOrDefault();
                                            getusrAss.dtCompleted = DateTime.Now;
                                            getusrAss.vchCompletedBy = Session["descript"].ToString();
                                            getusrAss.vchipused = Session["ipused"].ToString();
                                            getusrAss.vchhostname = Session["hostname"].ToString();
                                            getusrAss.vchCurrentStatus = "Completed";
                                            getusrAss.BitIsAssignCompleted = true;
                                            hrentity.SaveChanges();
                                        }
                                    }
                                    //Save answer according to total of number type question 9 to 12 no quest ans.
                                    if (total != 0)
                                    {
                                        for (int j = 9; j <= 12; j++)
                                        {
                                            objdew.fk_intEmpAssId = empid;
                                            objdew.fk_userid = usrid;
                                            objdew.fk_qid = j;
                                            if (j == 9)
                                            {
                                                if (total >= 86 && total <= 100)
                                                {
                                                    objdew.vchAnswer = "Yes";
                                                }
                                                else
                                                {
                                                    objdew.vchAnswer = "No";
                                                }
                                            }
                                            else if (j == 10)
                                            {
                                                if (total >= 61 && total <= 85)
                                                {
                                                    objdew.vchAnswer = "Yes";
                                                }
                                                else
                                                {
                                                    objdew.vchAnswer = "No";
                                                }
                                            }
                                            else if (j == 11)
                                            {
                                                if (total >= 36 && total <= 60)
                                                {
                                                    objdew.vchAnswer = "Yes";
                                                }
                                                else
                                                {
                                                    objdew.vchAnswer = "No";
                                                }
                                            }
                                            else if (j == 12)
                                            {
                                                if (total <= 35)
                                                {
                                                    objdew.vchAnswer = "Yes";
                                                }
                                                else
                                                {
                                                    objdew.vchAnswer = "No";
                                                }
                                            }
                                            objdew.intTotal = total;
                                            objdew.dtAssesment = DateTime.Now;
                                            objdew.vchAssesmentBy = Session["descript"].ToString();
                                            objdew.vchAssesmentIpused = Session["ipused"].ToString();
                                            objdew.vchAssesmentHost = Session["hostname"].ToString();
                                            objdew.intcode = code;
                                            objdew.intyr = Convert.ToInt32(Session["yr"].ToString());
                                            hrentity.tblAssesmentQuestDetails.Add(objdew);
                                            hrentity.SaveChanges();
                                        }
                                    }
                                    //Update Master table for final status of assesment
                                    var getmasemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id1 && e.intcode==code select e).FirstOrDefault();
                                    if (getmasemp != null)
                                    {
                                        getmasemp.BitCompleteAssesment = true;
                                        hrentity.SaveChanges();

                                    }
                                }
                                else
                                {
                                    var output8 = "Current user filled assesment is already present in database!";
                                    return Json(new { Success = 8, output8, JsonRequestBehavior.AllowGet });
                                }

                            }
                            var output6 = "Assesment submitted successfully!";
                            return Json(new { Success = 6, output6, JsonRequestBehavior.AllowGet });
                        }
                    }
                    else
                    {
                        var output9 = "System error!";
                        return Json(new { Success = 9, output9, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            else
            {
                var output10 = "Your session has expired!";
                return Json(new { Success = 10, output10, JsonRequestBehavior.AllowGet });
            }
        }

        //assesment user wise
        public ActionResult ViewCompletedAss()
        {
            if (Session["descript"] != null)
            {
                //for admin user
                
                int code = Convert.ToInt32(Session["id"].ToString());
                var MainAdmin = Session["MainAdmin"].ToString();
                var HrAdmin = Session["HrAdmin"].ToString();
                if (MainAdmin != "True")
                {
                    int usrid = Convert.ToInt32(Session["usrid"].ToString());
                    var selectedlist = (from e in hrentity.tblEmpAssesmentMas
                                        join a in hrentity.tblEmpAssesmentDetails on e.intid equals a.fk_AssEmpId
                                        where a.fk_userid == usrid && a.BitIsAssignCompleted == true && e.bitIsConsultant!=true && e.intcode==code 
                                        select e).ToList();
                    if (selectedlist.Count != 0)
                    {
                        return View(selectedlist);
                    }
                    else
                    {
                        TempData["Empty"] = "No completed assesment avilable for you now!";
                        return View();
                    }
                }
                else
                {
                    //admin user view / hradmin view
                    //int usrid = Convert.ToInt32(Session["usrid"].ToString());
                    var selectedlist = (from e in hrentity.tblEmpAssesmentMas
                                        join a in hrentity.tblEmpAssesmentDetails on e.intid equals a.fk_AssEmpId
                                        where a.BitIsAssignCompleted == true && e.intcode==code
                                        select e).ToList();
                    if (selectedlist.Count != 0)
                    {
                        return View(selectedlist);
                    }
                    else
                    {
                        TempData["Empty"] = "No comleted assesment avilable for admin!";
                        return View();
                    }
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult SingleAssesmentDetails(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                if (id != 0)
                {
                    var MainAdmin = Session["MainAdmin"].ToString();
                    //var HrAdmin = Session["HrAdmin"].ToString();
                    if (MainAdmin != "True")
                    {
                        //select specific user filled assessment using specific emp id
                        int userid = Convert.ToInt32(Session["usrid"].ToString());
                        //int code = Convert.ToInt32(Session["id"].ToString());
                        var selecteddata = (from e in hrentity.tblAssesmentQuestDetails where e.fk_intEmpAssId == id && e.fk_userid == userid && e.intcode==code select e).ToList();
                        var selecteddata1 = (from e in hrentity.tblAssesmentQuestDetails where e.fk_intEmpAssId == id && e.fk_userid == userid && e.intcode==code select e).FirstOrDefault();
                        var selectedMas = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode==code select e).FirstOrDefault();
                        int posiid = selectedMas.fk_PositionId;
                        var selectedPossiMas = (from e in hrentity.tblPositionCategoryMas where e.intid == posiid select e).FirstOrDefault();
                        ViewBag.Total = selecteddata1.intTotal.ToString();
                        ViewBag.Remarks = selecteddata1.vchSpecialRemarks.ToString();
                        ViewBag.Exp = selectedMas.decExperience.ToString();
                        ViewBag.AssBy = selecteddata1.vchAssesmentBy.ToString();
                        DateTime Date1 = (DateTime)selecteddata1.dtAssesment;
                        ViewBag.Date = Date1.ToString("dd/MM/yyyy");
                        ViewBag.EmpName = selectedMas.vchName.ToString();
                        ViewBag.Possi = selectedPossiMas.vchPosCatName.ToString();
                        return View(selecteddata);
                    }
                    else
                    {
                        //admin view or Hr AdminView
                        //error check it
                        var selecteddata = (from e in hrentity.tblAssesmentQuestDetails
                                            where e.fk_intEmpAssId == id && e.intcode==code
                                            select e).ToList();
                        var selecteddata1 = (from e in hrentity.tblAssesmentQuestDetails where e.fk_intEmpAssId == id && e.intcode == code select e).FirstOrDefault();
                        var selectedMas = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                        int posiid = selectedMas.fk_PositionId;
                        var selectedPossiMas = (from e in hrentity.tblPositionCategoryMas where e.intid == posiid select e).FirstOrDefault();
                        ViewBag.Total = selecteddata1.intTotal.ToString();
                        ViewBag.Remarks = selecteddata1.vchSpecialRemarks.ToString();
                        ViewBag.Exp = selectedMas.decExperience.ToString();
                        ViewBag.AssBy = selecteddata1.vchAssesmentBy.ToString();
                        DateTime Date1 = (DateTime)selecteddata1.dtAssesment;
                        ViewBag.Date = Date1.ToString("dd/MM/yyyy");
                        ViewBag.EmpName = selectedMas.vchName.ToString();
                        ViewBag.Possi = selectedPossiMas.vchPosCatName.ToString();
                        return View(selecteddata);
                    }
                }
                else
                {
                    TempData["ErrorMsg"] = "Id should not be null!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //View All Assesment Status in HR view as admin
        public ActionResult ViewAllAssStatus()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                //for admin user
                var user = Session["descript"].ToString();
                var isadmin = Session["MainAdmin"].ToString();
                var ishradmin = Session["HrAdmin"].ToString();
                if (isadmin != "True" && ishradmin != "True")
                {
                    int usrid = Convert.ToInt32(Session["usrid"].ToString());
                        var selectedlist = (from e in hrentity.tblEmpAssesmentDetails
                                        join a in hrentity.tblEmpAssesmentMas on e.fk_AssEmpId equals a.intid
                                        where e.fk_userid == usrid && a.bitIsByPassEntry!=true && e.intcode== code && a.bittempstatusactive != true && a.bitauthorcancel != true && a.BitAssesmentResultFail != true && a.bitstatusdeactive != true && a.bitIsConsultant!=true && a.bitIsLeft!=true
                                        select e).ToList();
                    if (selectedlist.Count != 0)
                    {
                        return View(selectedlist);
                    }
                    else
                    {
                        TempData["Empty"] = "No record avilable in database!";
                        return View();
                    }
                }
                else
                {
                    //admin user view
                    int usrid = Convert.ToInt32(Session["usrid"].ToString());
                    var selectedlist = (from e in hrentity.tblEmpAssesmentDetails
                                        join a in hrentity.tblEmpAssesmentMas on e.fk_AssEmpId equals a.intid
                                        where e.intcode == code && a.bittempstatusactive!=true && a.bitauthorcancel!=true && a.BitAssesmentResultFail!=true && a.bitstatusdeactive!=true
                                        select e).ToList();
                    if (selectedlist.Count != 0)
                    {
                        return View(selectedlist);
                    }
                    else
                    {
                        TempData["Empty"] = "No record avilable in database!";
                        return View();
                    }
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //View Single Assessment detail to HR View
        public ActionResult SingleStatusView(int id, int uid)
        {
            if (id != 0 && uid != 0)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selecteddata = (from e in hrentity.tblAssesmentQuestDetails where e.fk_intEmpAssId == id && e.fk_userid == uid && e.intcode==code select e).ToList();
                var selecteddata1 = (from e in hrentity.tblAssesmentQuestDetails where e.fk_intEmpAssId == id && e.fk_userid == uid && e.intcode==code select e).FirstOrDefault();
                var selectedMas = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode==code select e).FirstOrDefault();
                int posiid = selectedMas.fk_PositionId;
                var selectedPossiMas = (from e in hrentity.tblPositionCategoryMas where e.intid == posiid select e).FirstOrDefault();
                ViewBag.Total = selecteddata1.intTotal.ToString();
                ViewBag.Remarks = selecteddata1.vchSpecialRemarks.ToString();
                ViewBag.Exp = selectedMas.decExperience.ToString();
                ViewBag.AssBy = selecteddata1.vchAssesmentBy.ToString();
                DateTime Date1 = (DateTime)selecteddata1.dtAssesment;
                ViewBag.Date = Date1.ToString("dd/MM/yyyy");
                ViewBag.EmpName = selectedMas.vchName.ToString();
                ViewBag.Possi = selectedPossiMas.vchPosCatName.ToString();
                return View(selecteddata);
            }
            else
            {
                TempData["Empty"] = "No result found in database!";
                return View();
            }
        }

        //Final View for allowing Upload document
        public ActionResult AllComAss()
        {
            //for admin user
            int code = Convert.ToInt32(Session["id"].ToString());
            var user = Session["descript"].ToString();
            var isadmin = Session["MainAdmin"].ToString();
            var ishradmin = Session["HrAdmin"].ToString();
            if (isadmin == "True" || ishradmin == "True")
            {
                var CompList = (from e in hrentity.tblEmpAssesmentMas
                                where e.BitCompleteAssesment == true
                                && e.BitAllowUpload != true
                                && e.BitIsUploadCompleted != true
                                && e.BitAssesmentResultFail != true
                                && e.bitIsByPassEntry!=true
                                && e.intcode==code
                                select e).ToList();
                if (CompList.Count != 0)
                {
                    return View(CompList);
                }
                else
                {
                    TempData["Empty"] = "No new completed assessment found in batabase!";
                    return View();
                }
            }
            else
            {
                TempData["Empty"] = "No records found in database!";
                return View();
            }
        }

        //Save Final upload user int emp user
        public ActionResult AllowForUploadDoc(int id)
        {
            if (id != 0)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.BitAllowUpload != true && e.intcode==code select e).FirstOrDefault();
                if (selectedemp != null)
                {
                    //selectedemp.BitAssesmentResultPass = true;
                    // hrentity.SaveChanges();
                    var checkemp = (from e in hrentity.tblEmpLoginUser where e.fk_intEmpID == id && e.intcode==code select e).FirstOrDefault();
                    if (checkemp == null)
                    {
                        tblEmpLoginUser objnew = new tblEmpLoginUser();
                        objnew.fk_intEmpID = id;
                        objnew.vchmobile = selectedemp.vchMobile;
                        objnew.vchOTP = "Temp";
                        objnew.intcode = code;
                        objnew.intyr = Convert.ToInt32(Session["yr"].ToString());
                        hrentity.tblEmpLoginUser.Add(objnew);
                        hrentity.SaveChanges();
                        string mob = selectedemp.vchMobile.ToString();
                        var uri = "http://164.52.195.161/API/SendMsg.aspx?uname=20170839&pass=Indus2017&send=HOSPIN&dest=" + mob + "&msg=Upload your document on link : https://bit.ly/3dYNahD Thanks, Indus Healthcare Services Pvt. Ltd.&priority=1";
                        if (uri != null)
                        {
                            try
                            {
                                HttpWebRequest createrequest = (HttpWebRequest)WebRequest.Create(uri);
                                // getting response of sms
                                HttpWebResponse myResp = (HttpWebResponse)createrequest.GetResponse();
                                System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                                string responseString = _responseStreamReader.ReadToEnd();
                                _responseStreamReader.Close();
                                myResp.Close();
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            TempData["Error"] = "User already allowed for uploading";
                            return RedirectToAction("AllComAss");
                        }
                    }
                    //update mas table
                    selectedemp.BitAssesmentResultPass = true;
                    selectedemp.BitAllowUpload = true;
                    hrentity.SaveChanges();
                    TempData["Success"] = "Employee allowed uploads document and details successfully and uploading link sent to candidate mobile!";
                    return RedirectToAction("AllComAss");

                }
                else
                {
                    TempData["Error"] = "Error generated";
                    return RedirectToAction("AllComAss");
                }
            }
            TempData["Error"] = "Model Error generated";
            return RedirectToAction("AllComAss");
        }

        public ActionResult ViewAllDectiveAss()
        {
            int code=Convert.ToInt32(Session["id"].ToString());
            var list = (from e in hrentity.tblEmpAssesmentMas where e.BitAssesmentResultFail == true && e.intcode==code select e).ToList();
            if(list.Count()!=0)
            {
                return View(list);
            }
            else
            {
                TempData["Empty"] = "No deactivated assessment avilable till now!";
                return View();
            }
        }

        public ActionResult DeactiveAssesment(int id)
        {
            var selectedass = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
            if (selectedass != null)
            {
                AssDeactiveViewModel objdata = new AssDeactiveViewModel();
                objdata.ID = selectedass.intid;
                ViewBag.EmpName = selectedass.vchName.ToString();
                return View(objdata);
            }
            else
            {
                return View();
            } 
        }

        [HttpPost]
        public ActionResult DeactiveAssesment(AssDeactiveViewModel objnew)
        {
            if (ModelState.IsValid)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int id = objnew.ID;
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                if (selectedemp != null)
                {
                    selectedemp.BitAssesmentResultFail = true;
                    selectedemp.vchAssDeactiveRemarks = objnew.Remarks.ToString();
                    selectedemp.vchAssDeactiveBy = Session["descript"].ToString();
                    selectedemp.vchAssDeactiveDt = DateTime.Now;
                    hrentity.SaveChanges();
                    TempData["Success"] = "Assessment deactive successfully!";
                    return RedirectToAction("AllComAss");
                }
            }
            TempData["Error"] = "Model error generated!";
            return RedirectToAction("AllComAss");
        }

        #endregion

        #region Personal Details HR View

        //View Pending Uploads employee
        public ActionResult ViewAlluplodaed()
        {
           if(Session["descript"]!=null)
                { 
                int code = Convert.ToInt32(Session["id"].ToString());
                var uploadscomp = (from e in hrentity.tblEmpAssesmentMas
                                   where ((e.bitperdetails==false || e.bitCompDocP==false || e.bitcontdetails==false || e.bitProfileComplete==false) && (e.BitAllowUpload==true)
                                   )&& e.bitIsByPassEntry!=true && e.bitIsTransferred != true && e.intcode == code select e).ToList();                
                if (uploadscomp.Count != 0)
                {
                    return View(uploadscomp);
                }
                else
                {
                    TempData["Error"] = "No completed uploaded found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ViewCompDocEmp(int id)
        {
            if (id != 0)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selected = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode==code select e).ToList();
                if (selected != null)
                {
                    return View(selected);
                }
                else
                {
                    TempData["Error"] = "No data found against current employee id!";
                    return View();
                }
            }
            else
            {
                TempData["Error"] = "Id should not be zero or null!";
                return View();
            }
        }

        public ActionResult AddEmpDetails(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectempcode = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode==code select e).FirstOrDefault();
            ViewBag.SelectedEmpID = selectempcode.intid.ToString();
            int positionid = selectempcode.fk_PositionId;
            var positionname = (from e in hrentity.tblPositionCategoryMas where e.intid == positionid select e).FirstOrDefault();
            ViewBag.Position = positionname.vchPosCatName.ToString();
            //for gender slecection
            List<SelectListItem> genderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                }
                                             };
            ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
            //for marital status selection
            List<SelectListItem> Maritalstatus = new List<SelectListItem>
                             {
                                new SelectListItem{
                                      Text = "Single",
                                      Value = "1" },
                                new SelectListItem{
                                      Text = "Married",
                                      Value = "2" }            
                              };
            ViewBag.MaritalStatus = new SelectList(Maritalstatus, "Value", "Text");
            //FOr Title selection
            IEnumerable<tblTitleMas> selecttit = (from e in hrentity.tblTitleMas select e).ToList();
            List<SelectListItem> Title = new List<SelectListItem>();
            foreach (var tit in selecttit)
            {
                SelectListItem selectListItem = new SelectListItem()
                {
                    Value = tit.intid.ToString(),
                    Text = tit.vchname.ToString()
                };
                Title.Add(selectListItem);
            }
            ViewBag.Title1 = new SelectList(Title, "Value", "Text");
            //For selection State
            IEnumerable<tblState> selectState = (from e in hrentity.tblState select e).ToList();
            List<SelectListItem> allState = new List<SelectListItem>();
            foreach(var state in selectState)
            {
                SelectListItem selectListItem = new SelectListItem()
                {
                    Value = state.intid.ToString(),
                    Text = state.vchState.ToString()
                };
                allState.Add(selectListItem);
            }
            ViewBag.AllState = new SelectList(allState, "Value", "Text");
            return View();
        }

        //Global for select city 
        [HttpGet]
        public ActionResult GetCity(string state_id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int stateid = Convert.ToInt32(state_id);
            List<tblCity> citylist = new List<tblCity>();
            citylist = (from e in hrentity.tblCity where e.fk_stateid == stateid select e).ToList();
            var result = (from d in citylist
                          select new
                          {
                              id = d.intid,
                              vchCity = d.vchCityName
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddEmpDetails(tblEmpDetails newdata, FormCollection yvalaform)
        {
            if (Session["descript"] != null)
            {
                int empid = Convert.ToInt32(yvalaform.Get("empIDD"));
                int code1 = Convert.ToInt32(Session["id"].ToString());
                int tempcity = Convert.ToInt32(yvalaform.Get("selectedTCity"));
                int Pcity = Convert.ToInt32(yvalaform.Get("selectedPCity"));
                var selectedTCity = (from e in hrentity.tblCity where e.intid == tempcity select e).FirstOrDefault();
                var selectedPCity = (from e in hrentity.tblCity where e.intid == Pcity select e).FirstOrDefault();
                var selectedTState = (from e in hrentity.tblState where e.intid == selectedTCity.fk_stateid select e).FirstOrDefault();
                var selectedPState = (from e in hrentity.tblState where e.intid == selectedPCity.fk_stateid select e).FirstOrDefault();
                newdata.vchtcity = selectedTCity.vchCityName;
                newdata.vchpcity = selectedPCity.vchCityName;
                newdata.vchtstate = selectedTState.vchState;
                newdata.vchpstate = selectedPState.vchState;
                newdata.fk_tCity = tempcity;
                newdata.fk_city = Pcity;
                newdata.fktState = selectedTState.intid;
                //select master emp
                var selectedemployee = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code1 select e).FirstOrDefault();
                if (empid != 0)
                {
                    DateTime DOB = Convert.ToDateTime(newdata.dtDob);
                    newdata.dtDob = DOB;
                    newdata.fk_intempid = empid;
                    if (newdata.vchmaritalststus == "1")
                    {
                        newdata.vchmaritalststus = "Single";
                    }
                    else if (newdata.vchmaritalststus == "2")
                    {
                        newdata.vchmaritalststus = "Married";
                    }
                    else if (newdata.vchmaritalststus == "3")
                    {
                        newdata.vchmaritalststus = "Widowed";
                    }
                    if (newdata.vchsex == "1")
                    {
                        newdata.vchsex = "Male";
                    }
                    else if (newdata.vchsex == "2")
                    {
                        newdata.vchsex = "Female";
                    }
                    if (newdata.vchspouse != null)
                    {
                        newdata.vchspouse = newdata.vchspouse;
                    }
                    else
                    {
                        newdata.vchspouse = "N/A";
                    }
                    newdata.vchcreatedby = Session["descript"].ToString();
                    newdata.dtcreated = DateTime.Now;
                    newdata.vchipused = Session["ipused"].ToString();
                    newdata.vchhostname = Session["hostname"].ToString();
                    //Code for Temp Emp Code
                    string tcode = selectedemployee.intid.ToString();
                    string code = "T";
                    string compcode = code + tcode;
                    newdata.vchEmpTcode = compcode.ToString();
                    newdata.intcode = code1;
                    newdata.intyr = Convert.ToInt32(Session["yr"].ToString());
                    try
                    {
                        hrentity.tblEmpDetails.Add(newdata);
                        hrentity.SaveChanges();
                        if (newdata.BitCompleted == false)
                        {
                            selectedemployee.bittempperdetails = true;
                            hrentity.SaveChanges();
                        }
                        else if (newdata.BitCompleted == true)
                        {
                            selectedemployee.bittempperdetails = true;
                            selectedemployee.bitperdetails = true;
                            hrentity.SaveChanges();
                        }                        
                    }
                    catch (DbEntityValidationException ex)
                    {
                        //Retrieve the validation errors
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                TempData["Error"] = ("Property:", validationError.PropertyName, validationError.ErrorMessage);
                                return RedirectToAction("ViewCompDocEmp", new { id = empid });
                            }
                        }
                    }
                    TempData["Success"] = "Your personal details saved successfully!";
                    return RedirectToAction("ViewCompDocEmp", new { id = empid });
                }
                else
                {
                    TempData["Error"] = "ID Error generated!";
                    return RedirectToAction("ViewCompDocEmp", new { id = empid });
                }               
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        public ActionResult EditDetail(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectedemp = (from e in hrentity.tblEmpDetails where e.fk_intempid == id && e.intcode==code select e).FirstOrDefault();
            if (selectedemp != null)
            {
                var selectempcode = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode==code select e).FirstOrDefault();
                int positionid = selectempcode.fk_PositionId;
                ViewBag.SelectedEmpID = selectempcode.intid.ToString();
                var positionname = (from e in hrentity.tblPositionCategoryMas where e.intid == positionid select e).FirstOrDefault();
                ViewBag.Position = positionname.vchPosCatName.ToString();
                //for gender slecection
                if (selectedemp.vchsex == "Male")
                {
                    selectedemp.vchsex = "1";
                }
                else
                {
                    selectedemp.vchsex = "2";
                }
                
                List<SelectListItem> genderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                }
                                             };
                ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                //for marital status selection
                if(selectedemp.vchmaritalststus== "Single")
                {
                    selectedemp.vchmaritalststus = "1";
                }
                if (selectedemp.vchmaritalststus == "Married")
                {
                    selectedemp.vchmaritalststus = "2";
                }
                List<SelectListItem> Maritalstatus = new List<SelectListItem>
                             {
                                new SelectListItem{
                                      Text = "Single",
                                      Value = "1"
                                     
                                },
                                new SelectListItem{
                                      Text = "Married",
                                      Value = "2"
                                     
                                }
                              };
                ViewBag.MaritalStatus = new SelectList(Maritalstatus, "Value", "Text");
                IEnumerable<tblTitleMas> selecttit = (from e in hrentity.tblTitleMas select e).ToList();
                List<SelectListItem> Title = new List<SelectListItem>();
                foreach (var tit in selecttit)
                {
                    SelectListItem selectListItem = new SelectListItem()
                    {
                        Value = tit.intid.ToString(),
                        Text = tit.vchname.ToString(),
                        Selected = Convert.ToBoolean(tit.IsSelected)
                    };
                    Title.Add(selectListItem);
                }
                ViewBag.Title1 = new SelectList(Title, "Value", "Text");
                //For selection State
                IEnumerable<tblState> selectState = (from e in hrentity.tblState select e).ToList();
                List<SelectListItem> allState = new List<SelectListItem>();
                foreach (var state in selectState)
                {
                    SelectListItem selectListItem = new SelectListItem()
                    {
                        Value = state.intid.ToString(),
                        Text = state.vchState.ToString()
                    };
                    allState.Add(selectListItem);
                }
                ViewBag.AllState = new SelectList(allState, "Value", "Text");
                return View(selectedemp);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditDetail(tblEmpDetails objupdate, FormCollection formcol)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int empid = Convert.ToInt32(formcol.Get("empIDD"));            
            var check = formcol.GetValue("chkpaddress");        
            int code1 = Convert.ToInt32(Session["id"].ToString());
            int tempcity = Convert.ToInt32(formcol.Get("selectedTCity"));
            int Pcity = Convert.ToInt32(formcol.Get("selectedPCity"));
            var selectedTCity = (from e in hrentity.tblCity where e.intid == tempcity select e).FirstOrDefault();
            var selectedPCity = (from e in hrentity.tblCity where e.intid == Pcity select e).FirstOrDefault();
            var selectedTState = (from e in hrentity.tblState where e.intid == selectedTCity.fk_stateid select e).FirstOrDefault();
            var selectedPState = (from e in hrentity.tblState where e.intid == selectedPCity.fk_stateid select e).FirstOrDefault();
            var seleceted = (from e in hrentity.tblEmpDetails where e.fk_intempid == empid && e.intcode==code select e).FirstOrDefault();
            if (seleceted != null)
            {
                seleceted.fk_titid = objupdate.fk_titid;
                seleceted.vchfname = objupdate.vchfname;
                if (objupdate.vchmname != null)
                {
                    seleceted.vchmname = objupdate.vchmname;
                }
                seleceted.vchlname = objupdate.vchlname;
                if (objupdate.vchmaritalststus == "1")
                {
                    seleceted.vchmaritalststus = "Single";
                }
                else if (objupdate.vchmaritalststus == "2")
                {
                    seleceted.vchmaritalststus = "Married";
                }
                else if (objupdate.vchmaritalststus == "3")
                {
                    objupdate.vchmaritalststus = "Widowed";
                }
                if (objupdate.vchsex == "1")
                {
                    seleceted.vchsex = "Male";
                }
                else if (objupdate.vchsex == "2")
                {
                    seleceted.vchsex = "Female";
                }
                seleceted.vchFatherName = objupdate.vchFatherName;
                seleceted.vchmothername = objupdate.vchmothername;
                if (objupdate.vchspouse != null)
                {
                    seleceted.vchspouse = objupdate.vchspouse;
                }
                else
                {
                    seleceted.vchspouse = "N/A";
                }
                seleceted.vchNominee = objupdate.vchNominee;
                seleceted.vchRelation = objupdate.vchRelation;
                seleceted.fk_tCity = tempcity;
                seleceted.fk_city = Pcity;
                seleceted.fktState = selectedTState.intid;
                seleceted.vchtaddress = objupdate.vchtaddress;
                seleceted.vchtcity = selectedTCity.vchCityName;
                seleceted.vchtstate = selectedTState.vchState;
                seleceted.vchtmobile = objupdate.vchtmobile;
                seleceted.vchpaddress = objupdate.vchpaddress;
                seleceted.vchpcity = selectedPCity.vchCityName;
                seleceted.vchpstate = selectedPState.vchState;
                seleceted.vchpmobile = objupdate.vchpmobile;
                seleceted.vchupdatedby = Session["descript"].ToString();
                seleceted.dtupdated = DateTime.Now;
                seleceted.vchupdatedip = Session["ipused"].ToString();
                seleceted.vchupdatedhost = Session["hostname"].ToString();
                hrentity.SaveChanges();
                //select mas table for update data
                var empmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid select e).FirstOrDefault();
                if (objupdate.BitCompleted == false)
                {
                    empmas.bittempperdetails = true;
                    hrentity.SaveChanges();
                }
                else if (objupdate.BitCompleted == true)
                {
                    empmas.bittempperdetails = true;
                    empmas.bitperdetails = true;
                    hrentity.SaveChanges();
                    TempData["Success"] = "Candidate details updated successfully!";
                    return RedirectToAction("ViewCompDocEmp", new { id = empid });
                }
                TempData["Success"] = "Candidate details updated successfully!";
                return RedirectToAction("ViewCompDocEmp", new { id = empid });
            }
            else
            {
                TempData["Error"] = "Selected cnadidate detail not found in database please check it again!";
                return RedirectToAction("ViewCompDocEmp", new { id = empid });
            }            
        }

        public ActionResult ViewDetails(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var detail = (from e in hrentity.tblEmpDetails where e.fk_intempid == id && e.intcode==code select e).FirstOrDefault();
            if (detail != null)
            {
                return View(detail);
            }
            else
            {
                TempData["Error"] = "Error generated please check it again!";
                return RedirectToAction("EmpDetailActions", new { id = id });
            }

        }

        public ActionResult UnlockEmpDetail(int id)
        {
            //change only in master table
            var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
            if (selectedemp !=null)
            {
                selectedemp.bitperdetails = false;
                hrentity.SaveChanges();
                TempData["Success"] = "Candidate details update unlocked successfully!";
                return RedirectToAction("ViewCompDocEmp", new { id = id });
            }
            else
            {
                TempData["Error"] = "Error generated please contact to administrator!";
                return RedirectToAction("ViewCompDocEmp", new { id = id });
            }
        }

        #endregion

        #region Compulsory document HR View
        public ActionResult UpCompDoc(int id)
        {
            if (Session["descript"] != null)
            {
                //return RedirectToAction("UpByPassDoc", "Authorization", new { id = id });
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                ViewBag.Mobile = selectedemp.vchMobile.ToString();
                ViewBag.Empname = selectedemp.vchName.ToString();
                ViewBag.EmpID = id.ToString();
                //For compulsory document
                DocCompulsoryModel objmodel = new DocCompulsoryModel();
                int fk_possiid = selectedemp.fk_PositionId;
                var compdoc = (from e in hrentity.tblPosDocMap
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

        [HttpPost]
        public ActionResult UpCompDoc(DocCompulsoryModel objmdel, FormCollection fmcolect)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int year= Convert.ToInt32(Session["yr"].ToString());
                    //docdetails table object
                    tblDocDetails objdocdetail = new tblDocDetails();
                    //get emp id
                    int id1 = Convert.ToInt32(fmcolect.Get("hdempid"));
                    //int id = Convert.ToInt32(newdoc.empid);
                    //get all emp id qualifications
                    int docid = Convert.ToInt32(objmdel.compdocname);
                    //get mas data
                    var empmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == id1 && e.intcode==code select e).FirstOrDefault();
                    if (objmdel.BitIsCompleted == true)
                    {
                        empmas.bitCompDocP = true;
                        hrentity.SaveChanges();
                    }
                    else
                    {
                        empmas.bitCompDocT = true;
                        hrentity.SaveChanges();
                    }
                    //get comp doc name
                    var selecteddoc = (from e in hrentity.tblPosDocMap where e.fk_docid == docid  select e).FirstOrDefault();
                    //get selected doc details
                    var docdetails = (from e in hrentity.tblDocMas where e.intid == docid  select e).FirstOrDefault();
                    //check is doc uploaded or not
                    var checkdoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id1 && e.fk_intdocid == docid && e.intcode==code select e).FirstOrDefault();
                    if (checkdoc != null)
                    {                     
                        string docname = checkdoc.vchdocname.ToString();
                        TempData["Error"] = "Against" + " " + docname + " " + "document already uploaded";
                        return RedirectToAction("UpCompDoc", new { id = id1 });
                    }
                    else
                    {
                        if (objmdel.compdocument != null)
                        {
                            string empid = id1.ToString();
                            string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                            string extension = Path.GetExtension(objmdel.compdocument.FileName);
                            if (extension != ".pdf")
                            {
                                TempData["Error"] = "Please select .pdf file for upload!";
                                return RedirectToAction("UpCompDoc", new { id = id1 });
                            }
                            else
                            {
                                string filename = Path.GetFileNameWithoutExtension(objmdel.compdocument.FileName);
                                string newfilename = filename + datetime + empid + extension;
                                string finalfilename = newfilename.Replace(" ", "");
                                string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + finalfilename));
                                //save file in upload folder
                                objmdel.compdocument.SaveAs(path);
                                if (objmdel.BitIsCompleted == true)
                                {
                                    empmas.bitCompDocP = true;
                                }
                                else
                                {
                                    empmas.bitCompDocT = true;
                                }
                                objdocdetail.fk_empAssid = id1;
                                objdocdetail.fk_intdocid = docid;
                                objdocdetail.vchdocname = selecteddoc.tblDocMas.vchdocname.ToString();
                                objdocdetail.vchfilename = finalfilename.ToString();
                                objdocdetail.vchdocadd = path.ToString();
                                objdocdetail.vchcreatedby = Session["descript"].ToString();
                                objdocdetail.dtcreated = DateTime.Now;
                                objdocdetail.vchipused = Session["ipused"].ToString();
                                objdocdetail.vchhostname = Session["hostname"].ToString();
                                objdocdetail.intcode = code;
                                objdocdetail.intyr = year;
                                objdocdetail.BitIsCompDoc = true;
                                empmas.bitCompDocT = true;
                                hrentity.tblDocDetails.Add(objdocdetail);
                                hrentity.SaveChanges();
                                TempData["Success"] = "Document upload successfully!";
                                return RedirectToAction("UpCompDoc", new { id = id1 });
                            }
                        }
                    }
                    return View();
                }
                else
                {
                    TempData["Error"] = "Model error generated please select document and file too!";
                    return RedirectToAction("UpCompDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ViewCompDoc(int id)
        {
            if (Session["descript"] != null)
            {
                int code=Convert.ToInt32(Session["id"].ToString());
                var selecteddoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompDoc == true && e.intcode==code select e).ToList();
                if (selecteddoc.Count != 0)
                {
                    return View(selecteddoc);
                }
                else
                {
                    TempData["Empty"] = "No document avilable in database!";
                    return View();
                }
            }
            else
            {
                TempData["Empty"] = "Invalid employee id!";
                return View();
            }
        }

        public ActionResult UnlockCompDoc(int id)
        {
            if (Session["descript"] != null)
            {
                //change only in master table
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                if (selectedemp != null)
                {
                    selectedemp.bitCompDocP = false;
                    //Will generates error while update comp doc on unlock time
                    //selectedemp.BitIsUploadCompleted = false;
                    hrentity.SaveChanges();
                    TempData["Success"] = "Candidate upload compulsory document unlocked successfully!";
                    return RedirectToAction("ViewCompDocEmp", new { id = id });
                }
                else
                {
                    TempData["Error"] = "Error generated please contact to administrator!";
                    return RedirectToAction("ViewCompDocEmp", new { id = id });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult DeleteCompDoc(int id)
        {
            if (Session["descript"] != null)
            {
                //delete from details table only
                var selecteddata = (from e in hrentity.tblDocDetails where e.intid == id select e).FirstOrDefault();
                if (selecteddata != null)
                {
                    //master table changes 1stly
                    int empid = selecteddata.fk_empAssid;
                    var selectedmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid select e).FirstOrDefault();
                    if (selectedmas != null)
                    {
                        selectedmas.bitCompDocP = false;
                        hrentity.SaveChanges();
                    }
                    System.IO.File.Delete(selecteddata.vchdocadd);
                    hrentity.tblDocDetails.Remove(selecteddata);
                    hrentity.SaveChanges();
                    TempData["Success"] = "Compulsory document delete successfully!";
                    return RedirectToAction("ViewCompDocEmp", new { id = empid });
                }
                else
                {
                    int empid = selecteddata.fk_empAssid;
                    TempData["Error"] = "Error generated please contact to administrator!";
                    return RedirectToAction("ViewCompDocEmp", new { id = empid });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Qualification document HR View

        public ActionResult employeeupload(int id)
        {
            if (id != 0)
            {
                int code=Convert.ToInt32(Session["id"].ToString());
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode==code select e).FirstOrDefault();
                ViewBag.Mobile = selectedemp.vchMobile.ToString();
                ViewBag.Empname = selectedemp.vchName.ToString();
                var empallquali = (from e in hrentity.tblQualiMas select e).ToList();
                List<SelectListItem> quali = new List<SelectListItem>();
                foreach (var qi in empallquali)
                {
                    SelectListItem selectListItem = new SelectListItem()
                    {
                        Text = qi.vchqname.ToString(),
                        Value = qi.intqualiid.ToString()

                    };
                    quali.Add(selectListItem);
                }
                List<string> getselected = new List<string>();
                DocumentViewModel objmodel = new DocumentViewModel();
                ViewBag.SelectedQuali = new SelectList(quali, "Value", "Text");
                //for post method
                objmodel.empid = selectedemp.intid;
                objmodel.docnamelist = quali;
                int fk_possiid = selectedemp.fk_PositionId;
                objmodel.BitCompleted = selectedemp.BitIsUploadCompleted;
                //TempData["docname"] = quali;
                ViewBag.EmpID = id.ToString();
                return View(objmodel);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult employeeupload(DocumentViewModel objnew, FormCollection formdata)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    int code=Convert.ToInt32(Session["id"].ToString());
                    int year=Convert.ToInt32(Session["yr"].ToString());
                    //docdetails table object
                    tblDocDetails objdocdetail = new tblDocDetails();
                    //get emp id
                    int id1 = Convert.ToInt32(formdata.Get("hdempid"));
                    //int id = Convert.ToInt32(newdoc.empid);
                    //get all emp id qualifications
                    int docid = Convert.ToInt32(objnew.filename);
                    var selecteddoc = (from e in hrentity.tblQualiMas where e.intqualiid == docid select e).FirstOrDefault();
                    //get all master document from docmas and check if doc is present in doc table or not
                    var checktable = (from e in hrentity.tblDocDetails where e.fk_empAssid == id1 && e.fk_qualiid == docid && e.intcode==code select e).FirstOrDefault();
                    if (checktable != null)
                    {
                        var selecetdmasemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id1 && e.intcode==code select e).FirstOrDefault();
                        if (selecetdmasemp.bittempcontdetails == false || selecetdmasemp.bitcontdetails == false)
                        {
                            selecetdmasemp.bittempcontdetails = true;
                            selecetdmasemp.bitcontdetails = true;
                            selecetdmasemp.BitIsUploadCompleted = true;
                            hrentity.SaveChanges();
                        }
                        selecetdmasemp.BitIsUploadCompleted = true;
                        string selecetddocname = checktable.vchdocname.ToString();
                        TempData["Error"] = "Against" + " " + selecetddocname + " " + "uploaded against current name!";
                        return RedirectToAction("employeeupload", new { id = id1 });
                    }
                    else
                    {

                        //check for pdf null
                        if (objnew.pdfFile != null)
                        {
                            //filename new format filename+datetime+empid+extension                         
                            string empid = id1.ToString();
                            string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                            string extension = Path.GetExtension(objnew.pdfFile.FileName);
                            if (extension != ".pdf")
                            {
                                //ModelState.AddModelError("filename", "Please select .pdf file for upload!");
                                TempData["Error"] = "Please select .pdf file for upload!";
                                return RedirectToAction("employeeupload", new { id = empid });
                            }
                            else
                            {
                                string filename = Path.GetFileNameWithoutExtension(objnew.pdfFile.FileName);
                                string newfilename = filename + datetime + empid + extension;
                                string finalfilename = newfilename.Replace(" ", "");
                                string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + finalfilename));
                                //save file in upload folder
                                objnew.pdfFile.SaveAs(path);
                                //Check for is final submission or not
                                var selecetdmasemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id1 && e.intcode==code select e).FirstOrDefault();
                                if (objnew.BitCompleted == true)
                                {
                                    selecetdmasemp.bittempcontdetails = true;
                                    selecetdmasemp.bitcontdetails = true;
                                    selecetdmasemp.BitIsUploadCompleted = true;
                                    objdocdetail.fk_empAssid = id1;
                                    objdocdetail.fk_qualiid = docid;
                                    objdocdetail.vchdocname = selecteddoc.vchqname;
                                    objdocdetail.vchdocadd = path.ToString();
                                    objdocdetail.vchfilename = finalfilename.ToString();
                                    objdocdetail.vchcreatedby = Session["descript"].ToString();
                                    objdocdetail.dtcreated = DateTime.Now;
                                    objdocdetail.vchipused = Session["ipused"].ToString();
                                    objdocdetail.vchhostname = Session["hostname"].ToString();
                                    objdocdetail.intcode = code;
                                    objdocdetail.intyr = year;
                                    objdocdetail.BitIsCompQuali = true;
                                    hrentity.tblDocDetails.Add(objdocdetail);
                                    hrentity.SaveChanges();
                                    TempData["Success"] = "Final upload successfully!";
                                    return RedirectToAction("ViewAlluplodaed", new { id = empid });
                                }
                                else
                                {
                                    //save path and emp id in database
                                    selecetdmasemp.bittempcontdetails = true;
                                    objdocdetail.fk_empAssid = id1;
                                    objdocdetail.fk_qualiid = docid;
                                    objdocdetail.vchdocname = selecteddoc.vchqname;
                                    objdocdetail.vchdocadd = path.ToString();
                                    objdocdetail.vchfilename = finalfilename.ToString();
                                    objdocdetail.vchcreatedby = Session["descript"].ToString();
                                    objdocdetail.dtcreated = DateTime.Now;
                                    objdocdetail.vchipused = Session["ipused"].ToString();
                                    objdocdetail.vchhostname = Session["hostname"].ToString();
                                    objdocdetail.intcode = code;
                                    objdocdetail.intyr = year;
                                    objdocdetail.BitIsCompQuali = true;
                                    hrentity.tblDocDetails.Add(objdocdetail);
                                    hrentity.SaveChanges();
                                    TempData["Success"] = "Document upload successfully!";
                                    return RedirectToAction("employeeupload", new { id = empid });
                                }
                            }
                        }
                        else
                        {
                            //null pdf file return error
                            return View();
                        }
                    }
                }
                else
                {
                    //model error return
                    return View();
                }
            }
            else
            {
                //session error return
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ViewEmpDoc(int id)
        {
            if (id != 0)
            {
                //int id = Convert.ToInt32(Session["empid"]);
                int code = Convert.ToInt32(Session["id"].ToString());
                var selecteddoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompQuali == true && e.intcode==code select e).ToList();
                if (selecteddoc.Count != 0)
                {
                    return View(selecteddoc);
                }
                else
                {
                    TempData["Empty"] = "No document avilable in database!";
                    return View();
                }
            }
            else
            {
                TempData["Error"] = "Invalid employee id!";
                return View();
            }
        }

        public ActionResult ViewEmpDocDetails(int id)
        {
            if (id != 0)
            {
                int code = Convert.ToInt32(Session["id"]);
                var selecteddoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompQuali == true && e.intcode==code select e).ToList();
                if (selecteddoc.Count != 0)
                {
                    return View(selecteddoc);
                }
                else
                {
                    TempData["Empty"] = "No document avilable in database!";
                    return View();
                }
            }
            else
            {
                TempData["Error"] = "Invalid employee id!";
                return View();
            }
        }

        public ActionResult UnlockQualiDoc(int id)
        {
            if (Session["descript"] != null)
            {
                //change only in master table
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                if (selectedemp != null)
                {
                    selectedemp.bitcontdetails = false;
                    selectedemp.BitIsUploadCompleted = false;
                    hrentity.SaveChanges();
                    TempData["Success"] = "Candidate upload qualification document unlocked successfully!";
                    return RedirectToAction("ViewCompDocEmp", new { id = id });
                }
                else
                {
                    TempData["Error"] = "Error generated please contact to administrator!";
                    return RedirectToAction("ViewCompDocEmp", new { id = id });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult DeleteQuliDoc(int id)
        {
            if (Session["descript"] != null)
            {
                var selectedqualidoc = (from e in hrentity.tblDocDetails where e.intid == id select e).FirstOrDefault();
                if (selectedqualidoc != null)
                {
                    int selectedempid = selectedqualidoc.fk_empAssid;
                    //change in master table
                    var selectedmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == selectedempid select e).FirstOrDefault();
                    if (selectedmas != null)
                    {
                        selectedmas.bitcontdetails = false;
                        selectedmas.BitIsUploadCompleted = false;
                        hrentity.SaveChanges();
                    }
                    hrentity.tblDocDetails.Remove(selectedqualidoc);
                    hrentity.SaveChanges();
                    TempData["Success"] = "Qualification document deleted successfully!";
                    return RedirectToAction("ViewCompDocEmp", new { id = selectedempid });
                }
                else
                {
                    int selectedempid = selectedqualidoc.fk_empAssid;
                    TempData["Error"] = "Error generated please contact to administrator!";
                    return RedirectToAction("ViewCompDocEmp", new { id = selectedempid });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Profile Picture HR View

        public ActionResult ProfilePic(int id)
        {
            if (id != 0)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                ProfileViewModel objmodel = new ProfileViewModel();
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode==code select e).FirstOrDefault();
                ViewBag.Mobile = selectedemp.vchMobile.ToString();
                ViewBag.Empname = selectedemp.vchName.ToString();
                ViewBag.EmpProfileName = "Profile Picture";
                ViewBag.EmpID = id.ToString();
                objmodel.BitIsCompleted = Convert.ToBoolean(selectedemp.bitProfileComplete);
                return View(objmodel);

            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ProfilePic(ProfileViewModel objpic, FormCollection fromform)
        {
            if (ModelState.IsValid)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int year = Convert.ToInt32(Session["yr"].ToString());
                //docdetails table object
                tblDocDetails objdocdetail = new tblDocDetails();
                //get emp id
                int id1 = Convert.ToInt32(fromform.Get("hdempid"));
                string selectedDoc = objpic.picname.ToString();
                //int id = Convert.ToInt32(newdoc.empid);
                //get all emp id qualifications
                //int docid = Convert.ToInt32(objpic.profilepic);
                //var selecteddoc = (from e in hrentity.tblQualiMas where e.intqualiid == docid select e).FirstOrDefault();
                //get all master document from docmas and check if doc is present in doc table or not
                var checktable = (from e in hrentity.tblDocDetails where e.fk_empAssid == id1 && e.BitIsProfilePic == true && e.intcode==code select e).FirstOrDefault();
                if (checktable != null)
                {
                    string selecetddocname = checktable.vchdocname.ToString();
                    TempData["Error"] = "Against" + " " + selecetddocname + " " + "uploaded against current name!";
                    return RedirectToAction("ProfilePic", new { id = id1 });
                }
                else
                {
                    //check for pdf null
                    if (objpic.profilepic != null)
                    {
                        //filename new format filename+datetime+empid+extension                         
                        string empid = id1.ToString();
                        string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                        string extension = Path.GetExtension(objpic.profilepic.FileName);
                        if (extension != ".jpg")
                        {
                            //ModelState.AddModelError("filename", "Please select .jpg file for upload!");
                            TempData["Error"] = "Please select .jpg file for upload!";
                            return RedirectToAction("ProfilePic", new { id = empid });
                        }
                        else
                        {
                            string filename = Path.GetFileNameWithoutExtension(objpic.profilepic.FileName);                            
                            string newfilename = filename + datetime + empid + extension;
                            string finalfilename = newfilename.Replace(" ", "");
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + finalfilename));
                            //save file in upload folder
                            objpic.profilepic.SaveAs(path);
                            var selecetdmasemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id1 && e.intcode==code select e).FirstOrDefault();
                            selecetdmasemp.bitProfileComplete = true;
                            selecetdmasemp.BitIsUploadCompleted = true;
                            objdocdetail.fk_empAssid = id1;
                            objdocdetail.vchdocname = selectedDoc;
                            objdocdetail.vchdocadd = path.ToString();
                            objdocdetail.vchfilename = finalfilename.ToString();
                            objdocdetail.vchcreatedby = Session["descript"].ToString();
                            objdocdetail.dtcreated = DateTime.Now;
                            objdocdetail.vchipused = Session["ipused"].ToString();
                            objdocdetail.vchhostname = Session["hostname"].ToString();
                            objdocdetail.BitIsProfilePic = true;
                            objdocdetail.intcode = code;
                            objdocdetail.intyr = year;
                            hrentity.tblDocDetails.Add(objdocdetail);
                            hrentity.SaveChanges();
                            TempData["Success"] = "Photograph upload successfully!";
                            return RedirectToAction("ViewCompDocEmp", new { id = empid });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Please select profile file for upload!";
                        return RedirectToAction("ProfilePic", new { id = id1 });
                    }
                }
            }
            else
            {
                int id1 = Convert.ToInt32(fromform.Get("hdempid"));
                TempData["Error"] = "Model Error Generated!";
                return RedirectToAction("ProfilePic", new { id = id1 });
            }
        }

        public ActionResult viewProfile(int id)
        {
            if (Session["descript"] != null)
            {
                //int empid = Convert.ToInt32(Session["empid"].ToString());
                int code = Convert.ToInt32(Session["id"].ToString());
                var selecteddoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsProfilePic == true && e.intcode==code select e).ToList();
                if (selecteddoc.Count != 0)
                {
                    return View(selecteddoc);
                }
                else
                {
                    TempData["Empty"] = "No document avilable in database!";
                    return View();
                }
            }
            else
            {
                TempData["Empty"] = "Invalid employee id!";
                return View();
            }
        }

        public ActionResult DeleteProfile(int id)
        {
            if (Session["descript"] != null)
            {
                var selecteddata = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsProfilePic == true select e).FirstOrDefault();
                if (selecteddata != null)
                {
                    //change in master table
                    var selecetdmasemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    if (selecetdmasemp != null)
                    {
                        selecetdmasemp.bitProfileComplete = false;
                        selecetdmasemp.BitIsUploadCompleted = false;
                        hrentity.SaveChanges();
                    }
                    //remove details table entry
                    hrentity.tblDocDetails.Remove(selecteddata);
                    hrentity.SaveChanges();
                    TempData["Success"] = "Profile picture removed successfully!";
                    return RedirectToAction("ViewCompDocEmp", new { id = id });
                }
                else
                {
                    TempData["Error"] = "Error generated please contact to admin!";
                    return RedirectToAction("ViewCompDocEmp", new { id = id });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region After uploades completion send employee for autorization Send view and other actions

        public ActionResult SendAuthorization()
        {

            return View();
        }

        #endregion

        #region View Completed Upload for HR View
        public ActionResult ViewCompletedUploads()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var uploadscomp = (from e in hrentity.tblEmpAssesmentMas
                               where e.BitIsUploadCompleted == true && e.BitAssesmentResultFail != true
                               && e.bittofficialdetails != true && e.bitofficialdetails != true && e.bittempcontdetails == true
                               && e.bitcontdetails == true && e.bitCompDocP == true && e.bitProfileComplete == true
                               && e.bitperdetails == true && e.intcode == code
                               select e).ToList();
            if (uploadscomp.Count != 0)
            {
                return View(uploadscomp);
            }
            else
            {
                TempData["Error"] = "No completed uploads avilable in database now!";
                return View();
            }
        }        

        #endregion

        #region Email class

        //public static class MailSender
        //{

        //    public static string SendEmail(string strSubject, string strBody, string strFromEmailAddress, string StrToEmailAddress)
        //    {
        //        MailMessage mailMsg = new MailMessage();
        //        mailMsg.From = new MailAddress(strFromEmailAddress);
        //        string[] multi = StrToEmailAddress.Split(',');
        //        foreach (string multiemailid in multi)
        //        {
        //            mailMsg.To.Add(new MailAddress(multiemailid));
        //        }
        //        //mailMsg.To.Add(StrToEmailAddress);
        //        mailMsg.Subject = strSubject;
        //        mailMsg.IsBodyHtml = true;
        //        mailMsg.Body = strBody;
        //        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(strBody, null, "text/html");
        //        mailMsg.AlternateViews.Add(htmlView);
        //        var client = new SmtpClient();
        //        try
        //        {
        //            client.UseDefaultCredentials = false;
        //            client.Credentials = new System.Net.NetworkCredential("it.support@indushospital.in", "support12#");
        //            client.EnableSsl = true;
        //            client.Send(mailMsg);
        //            return "1";
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.Message.ToString();
        //        }
        //    }
        //    static void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //    {

        //    }
        //}

        #endregion        

    }
}