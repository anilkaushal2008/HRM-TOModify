using HRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.CodeDom;
using System.Globalization;
using System.Data.Entity.Validation;
using System.Net;
using HRM.Utilities;

namespace HRM.Controllers
{
    public class LeaveController : Controller
    {
        // GET: Consultant
        HRMEntities hrentity = new HRMEntities();

        #region Leave Type Master
        public ActionResult TypeIndex()
        {
            List<tblLeaveType> lisType = hrentity.tblLeaveType.Select((tblLeaveType e) => e).ToList();
            if (lisType.Count() != 0)
            {
                return View(lisType);
            }
            base.ViewBag.Error = "0 record found in database!";
            return View();
        }

        public ActionResult LeaveTypeCreate()
        {
            if (base.Session["descript"] != null)
            {
                return View();
            }
            return RedirectToAction("_SessionError1", "Home");
        }

        [HttpPost]
        public ActionResult LeaveTypeCreate(tblLeaveType objnew)
        {
            if (base.Session["descript"] != null)
            {
                objnew.dtCreated = DateTime.Now;
                objnew.vchCreatedBy = base.Session["descript"].ToString();
                objnew.vchCreatedHost = base.Session["hostname"].ToString();
                objnew.vchCreatedIP = base.Session["ipused"].ToString();
                objnew.unID = Guid.NewGuid();
                hrentity.tblLeaveType.Add(objnew);
                hrentity.SaveChanges();
                base.ViewBag.Success = "Master created successfully!";
                return View();
            }
            return RedirectToAction("_SessionError1", "Home");
        }

        public ActionResult EditType(Guid id)
        {
            if (base.Session["descript"] != null)
            {
                tblLeaveType selected = hrentity.tblLeaveType.Where((tblLeaveType e) => e.unID == id).FirstOrDefault();
                if (selected != null)
                {
                    return View(selected);
                }
                base.TempData["Error"] = "Detail not found please contact to admin!";
                return View("TypeIndex");
            }
            return RedirectToAction("_SessionError1", "Home");
        }

        [HttpPost]
        public ActionResult EditType(tblLeaveType objedit)
        {
            if (base.Session["descript"] != null)
            {
                tblLeaveType getRecord = hrentity.tblLeaveType.Where((tblLeaveType e) => e.unID == objedit.unID).FirstOrDefault();
                if (getRecord != null)
                {
                    getRecord.leaveType = objedit.leaveType;
                    getRecord.decMaxDayPerYear = objedit.decMaxDayPerYear;
                    getRecord.decMaxOnce = objedit.decMaxOnce;
                    getRecord.decMinOnce = objedit.decMinOnce;
                    getRecord.bitAllowYearHalfCheck = objedit.bitAllowYearHalfCheck;
                    getRecord.vchUpdatedBy = base.Session["descript"].ToString();
                    getRecord.vchUpdatedHost = base.Session["hostname"].ToString();
                    getRecord.vchUpdatedIP = base.Session["ipused"].ToString();
                    hrentity.SaveChanges();
                    base.TempData["Success"] = "Record updated successfully!";
                    return RedirectToAction("TypeIndex");
                }
                base.TempData["Error"] = "Record detail not found in database, Check it again or contact to admin!";
                return RedirectToAction("TypeIndex");
            }
            return RedirectToAction("_SessionError1", "Home");
        }

        #endregion

        #region Employee Leave

        public ActionResult LeaveDash()
        {
            if (Session["Ename"] != null)
            {
                //get all consulmed leave
                int empid = Convert.ToInt32(Session["empid"].ToString());
                var GetConsumedLeave = (from e in hrentity.spGetConsumedLeave(empid) select e).ToList();
                if (GetConsumedLeave.Count() != 0)
                {
                    foreach(var lv in GetConsumedLeave)
                    {
                        if (lv.leaveType == "Marriage Leave")
                        {
                            ViewBag.MarriageL = lv.Leavecounter.ToString();
                            ViewBag.Marriagetxt = lv.leaveType.ToString();
                        }
                        if (lv.leaveType == "Leave without pay")
                        {
                            ViewBag.PaidLeave = lv.Leavecounter.ToString();
                            ViewBag.PaidLeavetxt = lv.leaveType.ToString();
                        }
                        if (lv.leaveType == "Casual Leave")
                        {
                            ViewBag.CL = lv.Leavecounter.ToString();
                            ViewBag.CLtxt = lv.leaveType.ToString();
                        }
                        if (lv.leaveType == "Compassionate Leave")
                        {
                            ViewBag.CompL = lv.Leavecounter.ToString();
                            ViewBag.CompLtxt = lv.leaveType.ToString();
                        }
                        if (lv.leaveType == "Sick Leave")
                        {
                            ViewBag.SL = lv.Leavecounter.ToString();
                            ViewBag.SLtxt = lv.leaveType.ToString();
                        }
                        if (lv.leaveType == "General Public Holiday")
                        {
                            ViewBag.GPHoliday = lv.Leavecounter.ToString();
                            ViewBag.GPtxt = lv.leaveType.ToString();
                        }                        
                    }
                }
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult AllLeave(Guid? yearId)
        {
            if (Session["Ename"] != null)
            {
                int empid = Convert.ToInt32(Session["Empid"].ToString());

                // 1. Get all sessions for the dropdown
                var sessions = hrentity.tblYearMaster.OrderByDescending(x => x.vchYearName).ToList();
                ViewBag.Sessions = new SelectList(sessions, "unID", "vchYearName");

                // 2. Determine which session to show
                // If no yearId is passed, default to the currently Active session
                Guid selectedYear;
                if (yearId.HasValue)
                {
                    selectedYear = yearId.Value;
                }
                else
                {
                    var active = sessions.FirstOrDefault(x => x.bitIsActive == true);
                    selectedYear = active != null ? active.unID : sessions.First().unID;
                }

                ViewBag.SelectedYear = selectedYear;

                // 3. Filter the leave history by the selected session
                var history = hrentity.tblLeaveApplication
                                .Where(x => x.fk_Empid == empid && x.fk_YearID == selectedYear)
                                .OrderByDescending(x => x.dtCreated)
                                .ToList();

                return View(history);
            }
            return RedirectToAction("_SessionError1", "Home");
        }

        [HttpGet]
        public ActionResult ApplyLeave()
        {
            if (Session["Ename"] != null)
            {
                int empid = Convert.ToInt32(Session["Empid"].ToString());

                // 1. Fetch Active Session (The core of the Jan 1st Reset)
                var activeSession = hrentity.tblYearMaster.FirstOrDefault(x => x.bitIsActive == true);
                if (activeSession == null)
                {
                    TempData["Error"] = "No active session found. Contact HR.";
                    return RedirectToAction("LeaveDash");
                }

                ViewBag.ActiveYearName = activeSession.vchYearName;
                ViewBag.SessionRange = activeSession.dtStartDate.ToString("dd MMM yyyy") + " to " + activeSession.dtEndDate.ToString("dd MMM yyyy");

                // 2. Date Boundaries (3-Day Grace Period)
                DateTime today = DateTime.Now;
                DateTime minAllowed = (today.Day <= 3) ? new DateTime(today.Year, today.Month, 1).AddMonths(-1) : new DateTime(today.Year, today.Month, 1);
                if (minAllowed < activeSession.dtStartDate) minAllowed = activeSession.dtStartDate;

                ViewBag.MinDate = minAllowed.ToString("dd/MM/yyyy");
                ViewBag.MaxDate = activeSession.dtEndDate.ToString("dd/MM/yyyy");

                // 3. Employee & Leave Eligibility Details
                var userDetail = hrentity.tblEmpLoginUser.FirstOrDefault(e => e.fk_intEmpID == empid);
                var getYourDetail = hrentity.tblEmpAssesmentMas.FirstOrDefault(e => e.intid == empid);

                if (getYourDetail != null)
                {
                    tblLeaveApplication objapp = new tblLeaveApplication { fk_Empid = getYourDetail.intid };
                    ViewBag.Name = getYourDetail.vchName;
                    ViewBag.Code = getYourDetail.vchEmpFcode ?? "Not Available";

                    // Filter Leave Types
                    var ltype = hrentity.tblLeaveType.Where(e => (getYourDetail.bitIsConsultant == true) ? e.bitForConsultant == true : e.bitForConsultant == false).ToList();

                    // 4. Build Smart Rules for Client-Side Validation
                    var typesList = new List<SelectListItem> { new SelectListItem { Text = "Select leave type", Value = "0" } };
                    var rules = new List<object>();

                    foreach (var type in ltype)
                    {
                        var usedInSession = hrentity.tblLeaveApplication.Where(l => l.fk_Empid == empid && l.fk_LeaveType == type.unID && l.fk_YearID == activeSession.unID && l.bitIsApproved == true).ToList();
                        decimal consumed = usedInSession.Sum(l => l.decdaysApproved ?? 0);
                        decimal h1Cons = usedInSession.Where(l => l.dtApproved.HasValue && l.dtApproved.Value.Month <= 6).Sum(l => l.decdaysApproved ?? 0);

                        typesList.Add(new SelectListItem { Text = type.leaveType, Value = type.unID.ToString() });
                        rules.Add(new { id = type.unID.ToString(), min = type.decMinOnce ?? 0.5m, max = type.decMaxOnce ?? 30m, totalAllowed = type.decMaxDayPerYear ?? 0, halfCheck = type.bitAllowYearHalfCheck, consumed = consumed, firstHalfConsumed = h1Cons });
                    }
                    ViewBag.Types = new SelectList(typesList, "Value", "Text");
                    ViewBag.LeaveRules = Newtonsoft.Json.JsonConvert.SerializeObject(rules);

                    // 5. Consumed Table Mapping
                    List<ConsumedLeaveMV> lvConsumedModel = new List<ConsumedLeaveMV>();
                    var raw = (getYourDetail.bitIsConsultant == true) ? hrentity.spGetConsumedConsultantLeave(empid).ToList().Select(x => new { x.leaveType, x.Leavecounter, x.decMaxDayPerYear, x.bitAllowYearHalfCheck }) : hrentity.spGetConsumedLeave(empid).ToList().Select(x => new { x.leaveType, x.Leavecounter, x.decMaxDayPerYear, x.bitAllowYearHalfCheck });
                    //var raw1=
                    foreach (var r in raw) lvConsumedModel.Add(new ConsumedLeaveMV { LeaveType = r.leaveType, ConsumedCount = r.Leavecounter, IsHalfYearReached = r.bitAllowYearHalfCheck == true ? (r.Leavecounter >= (r.decMaxDayPerYear / 2)) : (r.Leavecounter >= r.decMaxDayPerYear) });
                    ViewBag.LeaveCounter = lvConsumedModel;

                    // 6. Overlap Check Dates
                    ViewBag.AppliedDates = hrentity.tblLeaveApplication.Where(x => x.fk_Empid == empid && x.vchStatus != "Rejected" && x.fk_YearID == activeSession.unID).ToList().SelectMany(x => Enumerable.Range(0, (x.dtEndDate - x.dtStartFrom).Days + 1).Select(offset => x.dtStartFrom.AddDays(offset).ToString("dd/MM/yyyy"))).Distinct().ToList();

                    return View(objapp);
                }
            }
            return RedirectToAction("_SessionError1", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyLeave(tblLeaveApplication objnew)
        {
            if (base.Session["Ename"] != null)
            {
                // 1. Fetch the Active Session Master (e.g., 2026)
                var activeSession = hrentity.tblYearMaster.FirstOrDefault(x => x.bitIsActive == true);
                if (activeSession == null)
                {
                    TempData["Error"] = "No active leave session found. Contact HR.";
                    return RedirectToAction("ApplyLeave");
                }

                tblEmpAssesmentMas selecetdEmp = hrentity.tblEmpAssesmentMas.Where((tblEmpAssesmentMas e) => e.intid == objnew.fk_Empid).FirstOrDefault();
                int PositionID = selecetdEmp.fk_PositionId;

                if (base.ModelState.IsValid)
                {
                    // 2. Date Parsing Logic
                    string dtrange = objnew.leaveDateRange.ToString();
                    string NSdate = dtrange.Split('-')[0].Trim();
                    string NEdate = dtrange.Split('-').Last().Trim();

                    DateTime NewSdate1 = DateTime.ParseExact(NSdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime NewEdate1 = DateTime.ParseExact(NEdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    // 3. Server-Side Limit Validation (MinOnce / MaxOnce / Session Check)
                    var leaveTypeRule = hrentity.tblLeaveType.FirstOrDefault(x => x.unID == objnew.fk_LeaveType);
                    if (leaveTypeRule != null)
                    {
                        decimal requestedDays = (decimal)objnew.decdaysRequest;

                        // Validate Min/Max Once
                        if (requestedDays < (leaveTypeRule.decMinOnce ?? 0.5m) || requestedDays > (leaveTypeRule.decMaxOnce ?? 30m))
                        {
                            TempData["Error"] = $"Policy Error: For {leaveTypeRule.leaveType}, you can only apply between {leaveTypeRule.decMinOnce} and {leaveTypeRule.decMaxOnce} days.";
                            return RedirectToAction("ApplyLeave");
                        }

                        // Validate against Session Boundaries
                        if (NewSdate1 < activeSession.dtStartDate || NewEdate1 > activeSession.dtEndDate)
                        {
                            TempData["Error"] = $"Date Error: Application must be within the {activeSession.vchYearName} session range.";
                            return RedirectToAction("ApplyLeave");
                        }
                    }

                    // 4. Populate Master Object
                    objnew.unID = Guid.NewGuid();
                    objnew.fk_YearID = activeSession.unID; // CRITICAL: Tag with 2026 Session ID
                    objnew.dtStartFrom = NewSdate1;
                    objnew.dtEndDate = NewEdate1;
                    objnew.dtCreated = DateTime.Now;
                    objnew.vchCreatedBy = base.Session["Ename"].ToString();
                    objnew.vchCreatedHost = base.Session["hostname"].ToString();
                    objnew.vchCreatedIP = base.Session["ipused"].ToString();
                    objnew.vchStatus = "Pending";
                    objnew.intCode = Convert.ToInt32(base.Session["BrnachCode"].ToString());

                    string ISDoctor = Session["IsConsultant"]?.ToString() ?? "False";
                    int branchid = objnew.intCode;
                    string BranchName = Session["BrnachName"]?.ToString() ?? "";

                    // 5. Assignment Logic (Consultant / Doctor)
                    if (ISDoctor == "True")
                    {
                        objnew.bitISVpAssigned = true;
                        hrentity.tblLeaveApplication.Add(objnew);
                        hrentity.SaveChanges();
                        TempData["Success"] = "Leave request sent successfully to VP HR!";
                        return RedirectToAction("AllLeave");
                    }

                    // 6. Regular Employee Mapping (HOD / HR Manager)
                    var getMapping = hrentity.tblPosDesiMap.Where(e => e.fk_PosCatid == PositionID && e.IsSelected == true).ToList();
                    int UserCount = 0;
                    foreach (var mapuser in getMapping)
                    {
                        if (hrentity.tblUserMaster.Any(e => e.fk_intDesignationid == mapuser.fk_desiid && e.bitActive == true && e.intcode == branchid))
                            UserCount++;
                    }

                    if (UserCount == 1) // Only HR Manager
                    {
                        objnew.TempbitHODNotFound = true;
                        objnew.TempBitAssignedHR = true;
                        hrentity.tblLeaveApplication.Add(objnew);
                        hrentity.SaveChanges();

                        // Detail Entry for HR
                        int HRID = Convert.ToInt32(Session["BHR_UID"]);
                        var UnitHR = hrentity.tblUserMaster.FirstOrDefault(e => e.intid == HRID);
                        AddDetailEntry(objnew.unID, UnitHR, branchid, true, false);
                    }
                    else if (UserCount > 1) // HOD and HR
                    {
                        objnew.TempBitAssignedHOD = true;
                        hrentity.tblLeaveApplication.Add(objnew);
                        hrentity.SaveChanges();

                        int HRID = Convert.ToInt32(Session["BHR_UID"]);
                        foreach (var mapuser in getMapping)
                        {
                            var checkUser = hrentity.tblUserMaster.FirstOrDefault(e => e.fk_intDesignationid == mapuser.fk_desiid && e.bitActive == true && e.intcode == branchid);
                            if (checkUser != null)
                            {
                                bool isHR = (checkUser.intid == HRID);
                                AddDetailEntry(objnew.unID, checkUser, branchid, isHR, !isHR);
                            }
                        }
                    }
                    else
                    {
                        TempData["Error"] = "No Approver (HOD/HR) found for your position. Contact Admin.";
                        return RedirectToAction("ApplyLeave");
                    }

                    TempData["Success"] = "Leave request sent successfully!";
                    return RedirectToAction("AllLeave");
                }
            }
            return RedirectToAction("_SessionError1", "NewEmployeeLogin");
        }

        // Helper Method to clean up repeated detail entry code
        private void AddDetailEntry(Guid appID, tblUserMaster user, int branchid, bool isHR, bool isHOD)
        {
            tblLeaveApplicationDetail objdetail = new tblLeaveApplicationDetail();
            objdetail.UN_ID = Guid.NewGuid();
            objdetail.fk_AppID = appID;
            objdetail.vchAssignedUser = user.vchUsername;
            objdetail.bitISAssigned = true;
            objdetail.fk_AssignUserid = user.intid;
            objdetail.dtAssigned = DateTime.Now;
            objdetail.vchCurrentStatus = "Assigned";
            objdetail.intCode = branchid;
            objdetail.bitAssignedHR = isHR;
            objdetail.bitAssignedHOD = isHOD;
            hrentity.tblLeaveApplicationDetail.Add(objdetail);
            hrentity.SaveChanges();
        }

        [HttpPost]
        public JsonResult CheckLeaveAvailability(DateTime startDate, DateTime endDate, int employeeId)
        {
            if (base.Session["Ename"] != null)
            {
                bool leaveExists = hrentity.tblLeaveApplication.Any((tblLeaveApplication l) => l.fk_Empid == employeeId && ((l.dtEndDate <= endDate && l.dtEndDate >= startDate) || (l.dtStartFrom >= startDate && l.dtStartFrom <= endDate) || (l.dtStartFrom >= startDate && l.dtEndDate <= endDate)));
                return Json(new
                {
                    exists = leaveExists
                });
            }
            bool leaveExists2 = false;
            return Json(new
            {
                exists = leaveExists2
            });
        }

        public ActionResult EmployeeApprovedLeaveDetail(Guid id)
        {
            tblLeaveApplication getleaveDetail = hrentity.tblLeaveApplication.Where((tblLeaveApplication e) => e.unID == id).FirstOrDefault();
            if (getleaveDetail != null)
            {
                List<tblLeaveApplicationDetail> GetDetailList = hrentity.tblLeaveApplicationDetail.Where((tblLeaveApplicationDetail e) => e.fk_AppID == id).ToList();
                base.ViewBag.DetailLeave = GetDetailList;
                return View(getleaveDetail);
            }
            base.TempData["Empty"] = "Leave detail not found, try again or contact to administrator!";
            return View();
        }
        #endregion

        #region leave approval check at Dashboard controller


        #endregion
    }
}