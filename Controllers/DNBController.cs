using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static HRM.Models.DnbAssignFeeStructureViewModel;

namespace HRM.Controllers
{
    public class DNBController : Controller
    {
        HRMEntities hrentity = new HRMEntities();
        //GET: DNB
        public ActionResult Index(int? studentId, int? year, string status)
        {
            var selectedYear = year ?? DateTime.Now.Year;
            var today = DateTime.Now;

            // Fetch all records from SP
            var data = hrentity.sp_GetDnbFeeReport(studentId, year, status).ToList();
            if (data == null || data.Count == 0)
            {
                ViewBag.Message = "⚠️ No records found for the given criteria.";
            }

            //Filter out Govt Body records from all calculations
            var filteredData = data
                .Where(x => string.IsNullOrEmpty(x.PayableTo) || !x.PayableTo.Contains("Govt Body"))
                .ToList();

            //Totals (excluding Govt Body)
            var totalExpected = filteredData.Sum(x => (decimal?)x.Amount) ?? 0;

            //Get Collections
            var totalCollected = filteredData
                 .Where(x => x.VerificationStatus == "Verified")
                 .Sum(x => (decimal?)x.Amount) ?? 0;

            //Get Pending amount
            var pendingAmount = filteredData
                .Where(x => string.IsNullOrEmpty(x.VerificationStatus) ||
                            x.VerificationStatus == "Pending Verification")
                .Sum(x => (decimal?)x.Amount) ?? 0;

            //Get Overdue amount
            var overdueAmount = filteredData
                .Where(x => x.DueDate < today &&
                            (string.IsNullOrEmpty(x.VerificationStatus) ||
                             x.VerificationStatus != "Pending Verification"))
                .Sum(x => (decimal?)x.Amount) ?? 0;

            //Counts verified for fraph
            var verifiedCount = filteredData.Count(x => x.VerificationStatus == "Verified");
            //Get pending count for graph
            var pendingCount = filteredData.Count(x =>
                string.IsNullOrEmpty(x.VerificationStatus) ||
                x.VerificationStatus == "Pending Verification");
            //Get Cancelled count
            var cancelCOunt= filteredData.Count(x => x.VerificationStatus == "Cancel");
            //Get rejected count for graph
            var rejectedCount = filteredData.Count(x => x.VerificationStatus == "Rejected");

            //ViewBags for Dashboard
            ViewBag.SelectedYear = selectedYear;
            ViewBag.TotalExpected = totalExpected;
            ViewBag.TotalCollected = totalCollected;
            ViewBag.PendingAmount = pendingAmount;
            ViewBag.OverdueAmount = overdueAmount;
            ViewBag.VerifiedCount = verifiedCount;
            ViewBag.PendingCount = pendingCount;
            ViewBag.RejectedCount = rejectedCount;

            //Dropdown year list
            ViewBag.Years = hrentity.TblDnbFeeStructure
                .Select(y => y.DueDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            // ✅ Final report list (filtered)
            return View(data);
        }

        //View all DNB Employee       
        public ActionResult DNbEmpList()
        {

            List<DnbStudentListViewModel> objmas = (
        from emp in hrentity.tblEmpAssesmentMas
        join dnb in hrentity.TblDnbStudent
            on emp.intid equals dnb.fk_EmployeeId into dnbJoin
        from dnbData in dnbJoin.DefaultIfEmpty()
        where emp.fk_intdeptid == 2048
            && emp.fk_intdesiid == 3183
            && emp.bitIsLeft != true
            && emp.bitstatusdeactive != true
        orderby emp.vchName
        select new
        {
            emp.intid,
            emp.vchName,
            emp.vchEmpFcode,
            emp.tblDeptMas.vchdeptname,
            emp.tblDesignationMas.vchdesignation,
            emp.dtDOJ,            
            DnbStudentId = (int?)dnbData.DnbStudentId
            
        }
    )
    .AsEnumerable()
    .Select(x => new DnbStudentListViewModel
    {
        EmployeeId = x.intid,
        DnbStudentId= x.DnbStudentId,
        Name = x.vchName,
        EmpCode = x.vchEmpFcode,
        Department = x.vchdeptname,
        Designation = x.vchdesignation,
        DOJ = x.dtDOJ?.ToString("dd/MM/yyyy"),
        FeeStatus = x.DnbStudentId.HasValue ? "Assigned" : "Not Assigned"
    })
    .ToList();
            return View(objmas);
        }

        //Load fee structure setup form
        [HttpGet]
        public ActionResult Assign(int id)
        {
            var student = hrentity.tblEmpAssesmentMas.Find(id);
            if (student == null) return HttpNotFound();

            var vm = new AssignFeeStructureViewModel
            {
                StudentId = student.intid,
                StudentName = student.vchName,
                //DurationInYears = student.DurationInYears,
                StartDate =Convert.ToDateTime(student.dtDOJ)
            };          

            return View(vm);
        }

        [HttpPost]
        public JsonResult Assign(AssignFeeStructureViewModel model)
        {
            try
            {
                if (model == null || model.StudentId == 0)
                    return Json(new { success = false, message = "Invalid student data received." });

                if (model.YearlyFees == null || !model.YearlyFees.Any())
                    return Json(new { success = false, message = "No yearly fee data received." });

                //Check for existing DNB Student record
                var dnbStudent = hrentity.TblDnbStudent
                    .FirstOrDefault(x => x.fk_EmployeeId == model.StudentId);

                string createdBy = (Session["descript"] != null && !string.IsNullOrEmpty(Session["descript"].ToString()))
                    ? Session["descript"].ToString()
                    : (User.Identity?.Name ?? "System");
                int code = Convert.ToInt32(Session["id"].ToString());
                if (dnbStudent == null)
                {
                    //Create new DNB student entry
                    dnbStudent = new TblDnbStudent
                    {
                        fk_EmployeeId = model.StudentId,
                        CourseName = "DNB - " + model.StudentName,
                        DurationInYears = model.YearlyFees.Count,
                        StartDate = model.StartDate,
                        EndDate = model.StartDate.AddYears(model.YearlyFees.Count),
                        Status = "Active",
                        vchCreatedBy = createdBy,
                        dtCreated = DateTime.Now,
                        intcode = code
                    };
                    hrentity.TblDnbStudent.Add(dnbStudent);
                    hrentity.SaveChanges();
                    // 🧠 Detach so EF doesn't keep tracking it
                    hrentity.Entry(dnbStudent).State = System.Data.Entity.EntityState.Detached;
                }

                //Prevent duplicate fee structure creation
                bool alreadyHasStructure = hrentity.TblDnbFeeStructure
                    .Any(x => x.DnbStudentId == dnbStudent.DnbStudentId);

                if (alreadyHasStructure)
                {
                    return Json(new { success = false, message = "Fee structure already assigned for this DNB student." });
                }

                //Insert fee structures
                foreach (var fee in model.YearlyFees)
                {
                    var structure = new TblDnbFeeStructure
                    {
                        DnbStudentId = dnbStudent.DnbStudentId,
                        YearNumber = fee.YearNumber,
                        PayableTo = fee.PayableTo ?? "Institution",
                        Amount = fee.Amount,
                        DueDate = fee.DueDate,
                        ApprovedBy = createdBy,
                        ApprovedDate = DateTime.Now,
                        PaymentStatus = "Pending"
                    };
                    hrentity.TblDnbFeeStructure.Add(structure);
                }
                //Commit all records
                hrentity.SaveChanges();

                //Success response
                return Json(new
                {
                    success = true,
                    message = $"Fee structure successfully assigned for {model.YearlyFees.Count} year(s) under DNB Student ID {dnbStudent.DnbStudentId}."
                });
            }
            catch (Exception ex)
            {

                var baseEx = ex.GetBaseException().Message;
                return Json(new { success = false, message = "Server error: " + baseEx });
            }
        }

        public ActionResult ViewFeeStructure(int id)
        {
            // id = DnbStudentId
            var student = hrentity.TblDnbStudent
                .FirstOrDefault(s => s.DnbStudentId == id);

            if (student == null)
                return HttpNotFound();

            var feeStructure = (
                from f in hrentity.TblDnbFeeStructure
                where f.DnbStudentId == id
                orderby f.YearNumber
                select new DnbFeeStructureVM
                {
                    YearNumber = f.YearNumber,
                    PayableTo = f.PayableTo,
                    Amount = f.Amount,
                    DueDate = f.DueDate,
                    PaymentStatus = f.PaymentStatus,
                    ApprovedBy = f.ApprovedBy,
                    ApprovedDate = f.ApprovedDate
                }   
            ).ToList();

            var vm = new DnbFeeStructurePageVM
            {
                StudentId = student.DnbStudentId,
                EmployeeId = student.fk_EmployeeId,
                StudentName = student.CourseName,
                Duration = student.DurationInYears,
                StartDate = student.StartDate,
                EndDate = Convert.ToDateTime(student.EndDate),
                FeeDetails = feeStructure
            };

            return View(vm);
        }

        //for account officer actions

        //for view all verification pending slips
        //Show all pending verifications
        public async Task<ActionResult> FeeVerification()
        {
            var data = await GetPendingAsync();
            return View(data);
        }

        public async Task<IEnumerable<DnbFeeVerificationViewModel>> GetPendingAsync()
        {
            var data = await (
                from sub in hrentity.TblDnbFeeSubmission
                join fee in hrentity.TblDnbFeeStructure
                    on sub.FeeStructureId equals fee.FeeStructureId
                join stu in hrentity.TblDnbStudent
                    on fee.DnbStudentId equals stu.DnbStudentId
                where sub.Status == "Pending Verification"
                orderby sub.SubmittedDate descending
                select new DnbFeeVerificationViewModel
                {
                    StudentName=sub.TblDnbFeeStructure.TblDnbStudent.tblEmpAssesmentMas.vchName,
                    SubmissionId = sub.SubmissionId,
                    PaymentReferenceNo = sub.PaymentReferenceNo,
                    PaymentMode = sub.PaymentMode,
                    PaymentDate = sub.PaymentDate,
                    PaymentScreenshotPath = sub.PaymentScreenshotPath,
                    FileName = sub.FileName,
                    SubmittedBy = sub.SubmittedBy,
                    SubmittedDate = (DateTime)sub.SubmittedDate,
                    Status = sub.Status,
                    VerifiedBy = sub.VerifiedBy,
                    VerifiedRemarks = sub.VerifiedRemarks,

                    FeeStructureId = fee.FeeStructureId,
                    YearNumber = fee.YearNumber,
                    Amount = fee.Amount,
                    PayableTo = fee.PayableTo,
                    CourseName = stu.CourseName,
                    EmployeeId = stu.fk_EmployeeId
                }
            ).ToListAsync();
            return data;
        }

        public async Task<DnbFeeVerificationViewModel> GetByIdAsync(int id)
        {
            var data = await (
                from sub in hrentity.TblDnbFeeSubmission
                join fee in hrentity.TblDnbFeeStructure
                    on sub.FeeStructureId equals fee.FeeStructureId
                join stu in hrentity.TblDnbStudent
                    on fee.DnbStudentId equals stu.DnbStudentId
                where sub.SubmissionId == id
                select new DnbFeeVerificationViewModel
                {
                    SubmissionId = sub.SubmissionId,
                    PaymentReferenceNo = sub.PaymentReferenceNo,
                    PaymentMode = sub.PaymentMode,
                    PaymentDate = sub.PaymentDate,
                    PaymentScreenshotPath = sub.PaymentScreenshotPath,
                    FileName = sub.FileName,
                    SubmittedBy = sub.SubmittedBy,
                    SubmittedDate = (DateTime)sub.SubmittedDate,
                    Status = sub.Status,
                    VerifiedBy = sub.VerifiedBy,
                    VerifiedRemarks = sub.VerifiedRemarks,

                    FeeStructureId = fee.FeeStructureId,
                    YearNumber = fee.YearNumber,
                    Amount = fee.Amount,
                    PayableTo = fee.PayableTo,
                    CourseName = stu.CourseName,
                    EmployeeId = stu.fk_EmployeeId
                }
            ).FirstOrDefaultAsync();

            return data;
        }


        //Load verification modal for selected record
        [HttpGet]
        public async Task<ActionResult> Verify(int id)
        {
            var model = await (
                from sub in hrentity.TblDnbFeeSubmission
                join fee in hrentity.TblDnbFeeStructure
                    on sub.FeeStructureId equals fee.FeeStructureId
                join stu in hrentity.TblDnbStudent
                    on fee.DnbStudentId equals stu.DnbStudentId
                where sub.SubmissionId == id
                select new DnbFeeVerificationViewModel
                {
                    SubmissionId = sub.SubmissionId,
                    PaymentReferenceNo = sub.PaymentReferenceNo,
                    PaymentMode = sub.PaymentMode,
                    PaymentDate = sub.PaymentDate,
                    PaymentScreenshotPath = sub.PaymentScreenshotPath,
                    FileName=sub.FileName,
                    SubmittedBy = sub.SubmittedBy,
                    SubmittedDate = (DateTime)sub.SubmittedDate,
                    Status = sub.Status,
                    VerifiedBy = sub.VerifiedBy,
                    VerifiedRemarks = sub.VerifiedRemarks,

                    FeeStructureId = fee.FeeStructureId,
                    YearNumber = fee.YearNumber,
                    Amount = fee.Amount,
                    PayableTo = fee.PayableTo,
                    CourseName = stu.CourseName,
                    EmployeeId = stu.fk_EmployeeId,
                   
                }
            ).FirstOrDefaultAsync();

            if (model == null)
                return null;

            return PartialView("_VerificationModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult VerifySlip(DnbFeeVerificationViewModel model)
        {
            try
            {
                if (model == null || model.SubmissionId <= 0)
                    return Json(new { success = false, message = "Invalid submission." });

                var submission = hrentity.TblDnbFeeSubmission.Find(model.SubmissionId);
                //var feeStructure= hrentity.TblDnbFeeStructure.Find(submission.FeeStructureId);
                if (submission == null)
                    return Json(new { success = false, message = "Record not found in DB." });

                submission.Status = model.Status ?? "Verified";
                //feeStructure.PaymentStatus =model.Status?? "Verified";
                submission.VerifiedBy = Session["descript"].ToString()??"System";
                submission.VerifiedRemarks = model.VerifiedRemarks ?? "Verified by Accounts";
                submission.VerificationDate = DateTime.Now;
                hrentity.SaveChanges();
                return Json(new { success = true, message = "Verification updated." });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { success = false, message = ex.Message + " | " + (ex.InnerException?.Message ?? "") });
            }
        }


    }
}