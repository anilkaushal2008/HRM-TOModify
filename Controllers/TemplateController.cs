using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc; 
using HRM.Models;
using Microsoft.Reporting.WebForms;

namespace HRM.Controllers
{
    public class TemplateController : Controller
    {
        //private HRMEntities _hrmentity;     
        HRMEntities objentity = new HRMEntities();

        //GET: Template
        #region Master and Employee Offer Letters

        #region Offer Letter Mas

        public ActionResult IndexOfferMas()
        {
            if (Session["descript"] != null)
            {
                var list = (from e in objentity.tblLetterOfferMas select e).ToList();
                if (list.Count() != 0)
                {
                    ViewBag.Success = "Found " + list.Count() + " Offer letter master";
                    return View(list);
                }
                else
                {
                    ViewBag.Empty = "0 records found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult NewOfferMas()
        {
            if (Session["descript"] != null)
            {
                //for master type selection
                List<SelectListItem> OfferMastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Master Offer letter",
                                                  Value = "1"
                                                 }
                                             
                };
                ViewBag.OfferType = new SelectList(OfferMastype, "Value", "Text");
                //for gender slecection
                List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="All",
                                                  Value = "1"
                                                 }
                };
                ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult NewOfferMas(tblLetterOfferMas objMas)
        {
            if (Session["descript"] != null)
            {
                ////check letter code
                //var checklettercode = (from e in objentity.tblExperienceMas where e.vchLetterCode == objtemplate.vchLetterCode select e).FirstOrDefault();
                //if (checklettercode == null)
                //{
                var checkMasname = (from e in objentity.tblLetterOfferMas where e.vchName == objMas.vchName select e).FirstOrDefault();
                if (checkMasname == null)
                {
                    //For selecetd gender
                    if (objMas.vchForGender == "1" || objMas.vchForGender=="All")
                    {
                        objMas.vchForGender = "All";
                    }
                    
                    //for mas type
                    if (objMas.vchMasType == "1" || objMas.vchMasType == "Offer letter master")
                    {
                        objMas.vchMasType = "Offer letter master";

                    }                    
                    objMas.vchCreatedBy = Session["descript"].ToString();
                    objMas.dtCreated = DateTime.Now;
                    objMas.vchCreatedIP = Session["ipused"].ToString();
                    objMas.vchCreatedHost = Session["hostname"].ToString();
                    objentity.tblLetterOfferMas.Add(objMas);
                    objentity.SaveChanges();
                    ViewBag.Success = "Master letter saved successfully!";
                    //For gender selection
                    List<SelectListItem> genderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="All",
                                                  Value = "1"
                                                 }
                                             };
                    ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                    //for master type selection
                    List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Offer letter master",
                                                  Value = "1"
                                                 }
                                             };
                    ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                    return View();
                }
                else
                {

                    //For gender selection
                    List<SelectListItem> genderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="All",
                                                  Value = "1"
                                                 }
                                             };
                    ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                    //for master type selection
                    List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Offer letter master",
                                                  Value = "1"
                                                 }
                                             };
                    ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                    ViewBag.Error = "Master letter name already in used, please check it in view all template!";
                    return View();
                }                        
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        public ActionResult EditLetterOfferMas(int id)
        {
            if (Session["descript"] != null)
            {
                var selectedLetter = (from e in objentity.tblLetterOfferMas where e.intid == id select e).FirstOrDefault();
                if (selectedLetter != null)
                {
                    //for master type selection
                    List<SelectListItem> OfferMastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Master Offer letter",
                                                  Value = "1"
                                                 }

                };
                    ViewBag.OfferType = new SelectList(OfferMastype, "Value", "Text");
                    //for gender slecection
                    List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="All",
                                                  Value = "1"
                                                 }
                };
                    ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                    return View(selectedLetter);
                }
                else
                {
                    //for master type selection
                    List<SelectListItem> OfferMastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Master Offer letter",
                                                  Value = "1"
                                                 }

                };
                    ViewBag.OfferType = new SelectList(OfferMastype, "Value", "Text");
                    //for gender slecection
                    List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="All",
                                                  Value = "1"
                                                 }
                };
                    ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                    return View();
                }

            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditLetterOfferMas(tblLetterOfferMas objmas)
        {
            if (Session["descript"] != null)
            {
                var selectedLetter = (from e in objentity.tblLetterOfferMas where e.intid == objmas.intid select e).FirstOrDefault();
                if (selectedLetter != null)
                {
                    //For selecetd gender
                    if (objmas.vchForGender == "1" || objmas.vchForGender=="All")
                    {
                        selectedLetter.vchForGender = "All";
                    }                   
                    //for mas type
                    if (objmas.vchMasType == "1" || objmas.vchMasType == "Offer Master Letter")
                    {
                        selectedLetter.vchMasType = "Offer Master Letter";

                    }
                    selectedLetter.vchName = objmas.vchName;
                    selectedLetter.vchMasHeading = objmas.vchMasHeading;
                    selectedLetter.vchLetterCode = objmas.vchLetterCode;
                    selectedLetter.txtMasContent = objmas.txtMasContent;
                    selectedLetter.vchUpdatedBy = Session["descript"].ToString();
                    selectedLetter.dtUpdated = DateTime.Now;
                    selectedLetter.vchUpdatedHost = Session["hostname"].ToString();
                    selectedLetter.vchUpdatedIp = Session["ipused"].ToString();
                    objentity.SaveChanges();
                    TempData["Success"] = "Offer master updated successfully!";
                    return RedirectToAction("IndexOfferMas");
                }
                else
                {
                    //for gender selection
                    List<SelectListItem> genderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="All",
                                                  Value = "1"
                                                 }
                                             };
                    ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                    //for master type selection
                    List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Offer Master Letter",
                                                  Value = "1"
                                                 }
                                             };
                    ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                    ViewBag.Error = "Selected master detail not found please try again or contact to administrator!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult OfferView(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedLetter = (from e in objentity.tblLetterOfferMas where e.intid == id select e).FirstOrDefault();
                    if (selectedLetter != null)
                    {                      
                        return View(selectedLetter);
                    }
                    else
                    {
                       ViewBag.Error = "Letter detail not found please check it and try again!";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Letter Id should not be null or 0!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Employee offer letter

        public ActionResult IndexEmpOfferLetter()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var list = (from e in objentity.tblLetterOfferDetail where e.intCode == code orderby e.dtCreated descending select e).ToList();
                if (list.Count() != 0)
                {
                    ViewBag.Success= "Found " + list.Count() + " employee offer letter";
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
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult EmpNewOffer()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt16(Session["id"].ToString());
                //select employee code which not red flag or not active employee
                var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                         join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid 
                                         where e.bittempstatusactive == true && e.bitstatusdeactive == false && e.BitIsRedFlagging == false && (e.bitIsPartialAuthorised==true || e.bitIsPartialAuthorised==false)
                                         && e.intcode == code && e.bitOfferLetter!=true
                                         select e).ToList();
                List<SelectListItem> allemp = new List<SelectListItem>();
                if (allEmployeeForExp != null)
                {
                    foreach (var emp in allEmployeeForExp)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = emp.intid.ToString(),
                            Text = emp.vchEmpFcode
                        };
                        allemp.Add(selectListItem);
                    }
                    ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                }
                //for department selection
                var selDept = (from e in objentity.tblDeptMas select e).ToList();
                if (selDept.Count() != 0)
                {
                    List<SelectListItem> desi = new List<SelectListItem>();
                    foreach (var dpt in selDept)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = dpt.intid.ToString(),
                            Text = dpt.vchdeptname
                        };
                        desi.Add(selectListItem);
                    }
                    ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = "Departments not found",
                        Text = "Departments not found"
                    };
                    ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                }
                //for designation selection
                var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                if (selectDesig.Count() != 0)
                {
                    List<SelectListItem> desi = new List<SelectListItem>();
                    foreach (var dess in selectDesig)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = dess.intid.ToString(),
                            Text = dess.vchdesignation
                        };
                        desi.Add(selectListItem);
                    }
                    ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = "Desigantions not found",
                        Text = "Designations not found"
                    };
                    ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                }

                //check is employee code is already used in experience or nor 
                // for sub type selection
                var selectMaster = (from e in objentity.tblLetterOfferMas select e).ToList();
                List<SelectListItem> typeselction = new List<SelectListItem>();
                foreach (var item2 in selectMaster)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = item2.intid.ToString(),
                        Text = item2.vchMasType.ToString()
                    };
                    typeselction.Add(selectListItem);
                }
                ViewBag.OfferType = new SelectList(typeselction, "Value", "Text"); 
                //for gender slecection
                List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Value = "1",
                                                  Text="Male"
                                             },
                                             new SelectListItem
                                             {
                                                 Value="2",
                                                 Text="Female"
                                                
                                             }
                  
                };
                ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //Get content offer letter from master
        public ActionResult GetOfferContent(string selectedOption)
        {
            if (Session["descript"] != null)
            {
                int newid = Convert.ToInt32(selectedOption);
                var selectedMas = (from e in objentity.tblLetterOfferMas where e.intid == newid select e).FirstOrDefault();
                if (selectedMas != null)
                {
                    var data = new
                    {
                        Result = selectedMas.txtMasContent.ToString()
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = new
                    {
                        Result = "Master content not found!"
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var data = "not found!";
                return Json(data);
            }
        }

        //gethrmsid function called from verious controller to get hrms existing employee details
        public ActionResult Gethrmsid(string empid)
        {
            if (Session["descript"] != null)
            {
                int id = Convert.ToInt32(empid.ToString());
                //check letter is avilable for employee or not
                var checkletter = (from e in objentity.tblLetterOfferDetail where e.fk_HRMS_id == id select e).FirstOrDefault();
                if (checkletter == null)
                {
                    var selectedMas = (from e in objentity.tblEmpAssesmentMas
                                       join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                       where e.intid == id
                                       select new { e.vchName, e.vchEmpFcode, e.fk_intdeptid, e.fk_intdesiid, d.vchFatherName, d.vchpcity, d.vchpstate, d.vchpaddress,d.vchsex }).FirstOrDefault();
                    if (selectedMas != null)
                    {
                        string getdgender = string.Empty;
                        if (selectedMas.vchsex == "Male")
                        {
                            getdgender = "1";
                        }
                        if (selectedMas.vchsex == "Female")
                        {
                            getdgender = "2";
                        }
                        var data = new
                        {
                            vchname = selectedMas.vchName,
                            vchempcode = selectedMas.vchEmpFcode,
                            fathername = selectedMas.vchFatherName,
                            city = selectedMas.vchpcity,
                            state = selectedMas.vchpstate,
                            add = selectedMas.vchpaddress,
                            deptid = selectedMas.fk_intdeptid,
                            desiid = selectedMas.fk_intdesiid,                           
                            vchGender = getdgender                           

                        };                       
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var error = new
                        {
                            alertmessage = "Employee detail not found!"
                        };
                        return Json(error, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var error = new
                    {
                        alertmessage = "Selected employee letter already generated, please check in view all employee letter!"
                    };
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult EmpNewOffer(tblLetterOfferDetail objnew)
        {
            if (Session["descript"] != null)
            {
               
                    //get selecetd HRMS code
                    tblEmpAssesmentMas objEmpMas = new tblEmpAssesmentMas();
                    if (objnew.fk_HRMS_id != 0 || objnew.fk_HRMS_id != null)
                    {
                        objEmpMas = (from e in objentity.tblEmpAssesmentMas where e.intid == objnew.fk_HRMS_id select e).FirstOrDefault();
                        if (objEmpMas != null)
                        {
                            objnew.vchEmpCode = objEmpMas.vchEmpFcode;
                        }
                    }
                
                    //check 
                    //create letter code 
                    int code = Convert.ToInt32(Session["id"].ToString());
                    string finalBranchCode = Session["branchCode"].ToString();
                    string hrCharachter = "HR";
                    int masid = Convert.ToInt32(objnew.fk_OfferMas_id.ToString());
                    var selectedMas = (from e in objentity.tblLetterOfferMas where e.intid == masid select e).FirstOrDefault();
                    string certificateCode = selectedMas.vchLetterCode;
                    var selectedDept = (from e in objentity.tblDeptMas where e.intid == objnew.fk_department select e).FirstOrDefault();
                    string deptCode = selectedDept.vchdepCode;
                    string CodeYear = DateTime.Now.ToString("yy");
                    int certificateCurrentcount = 0;
                    int year = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                    var getCode = (from e in objentity.tblLetterNumberMas where e.intcode == code && e.intYear == year select e).FirstOrDefault();
                    if (getCode != null)
                    {
                    try
                    {
                        //get current count number of certificate
                        certificateCurrentcount = Convert.ToInt16(getCode.intCurrent);
                        int newcode = certificateCurrentcount + 1;
                        int number = newcode;
                        int counter = 0;
                        string finalnumber = "";
                        //int fnumber = 0;
                        while (number > 0)
                        {
                            number = number / 10;
                            counter++;
                        }
                        if (counter == 1)
                        {
                            finalnumber = string.Concat("00" + newcode.ToString());
                        }
                        if (counter == 2)
                        {
                            finalnumber = string.Concat("0" + newcode.ToString());
                        }
                        if (counter == 3)
                        {
                            finalnumber = newcode.ToString();
                        }

                        //    //Format = Branch/HR/EC/EmpDpt/yr/series
                        string Ref_Code = finalBranchCode + "/" + hrCharachter + "/" + certificateCode + "/" + deptCode + "/" + CodeYear + "/" + finalnumber;
                        objnew.vchRefCode = Ref_Code;
                        //update current digit with new digit
                        getCode.intCurrent = newcode;
                        if (objnew.vchGender == "1" || objnew.vchGender == "Male")
                        {
                            objnew.vchGender = "Male";
                        }
                        if (objnew.vchGender == "2" || objnew.vchGender == "Female")
                        {
                            objnew.vchGender = "Female";
                        }
                        int yr = Convert.ToInt32(Session["yr"].ToString());
                        objnew.vchCreatedBy = Session["descript"].ToString();
                        objnew.dtCreated = DateTime.Now;
                        objnew.vchCreatedHost = Session["hostname"].ToString();
                        objnew.vchCreatedIP = Session["ipused"].ToString();
                        objnew.intCode = code;
                        objnew.intyr = yr;
                        
                        objnew.vchCompany = Session["Compname"].ToString();
                        //update hrms emp master for offer letter prepared
                        if (objEmpMas != null)
                        {
                            objEmpMas.bitOfferLetter = true;
                        }
                        objentity.tblLetterOfferDetail.Add(objnew);
                        objentity.SaveChanges();
                        TempData["Success"] = "Offer letter created and saved successfully";
                        return RedirectToAction("EmpNewOffer");
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
                        return RedirectToAction("EmpNewOffer");
                    }
                }                         

            else
            {
                //select employee code which not red flag or not active employee
                var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                         where e.bittempstatusactive == true && e.bitstatusdeactive == false && e.BitIsRedFlagging == false && e.bitIsPartialAuthorised == true
                                         && e.isHrmsEmployee == true && e.intcode == code
                                         select e).ToList();
                List<SelectListItem> allemp = new List<SelectListItem>();
                if (allEmployeeForExp != null)
                {
                    foreach (var emp in allEmployeeForExp)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = emp.intid.ToString(),
                            Text = emp.vchEmpFcode
                        };
                        allemp.Add(selectListItem);
                    }
                    ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                }
                //for department selection
                var selDept = (from e in objentity.tblDeptMas select e).ToList();
                if (selDept.Count() != 0)
                {
                    List<SelectListItem> desi = new List<SelectListItem>();
                    foreach (var dpt in selDept)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = dpt.intid.ToString(),
                            Text = dpt.vchdeptname
                        };
                        desi.Add(selectListItem);
                    }
                    ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = "Departments not found",
                        Text = "Departments not found"
                    };
                    ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                }
                //for designation selection
                var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                if (selectDesig.Count() != 0)
                {
                    List<SelectListItem> desi = new List<SelectListItem>();
                    foreach (var dess in selectDesig)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = dess.intid.ToString(),
                            Text = dess.vchdesignation
                        };
                        desi.Add(selectListItem);
                    }
                    ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = "Desigantions not found",
                        Text = "Designations not found"
                    };
                    ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                }

                //check is employee code is already used in experience or nor 
                // for sub type selection
                var selectMaster = (from e in objentity.tblLetterOfferMas select e).ToList();
                List<SelectListItem> typeselction = new List<SelectListItem>();
                foreach (var item2 in selectMaster)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = item2.intid.ToString(),
                        Text = item2.vchMasType.ToString()
                    };
                    typeselction.Add(selectListItem);
                }
                ViewBag.OfferType = new SelectList(typeselction, "Value", "Text");
                //for gender slecection
                List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem
                                             {
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                             new SelectListItem
                                             {
                                                 Text="Female",
                                                 Value="2"
                                             }
                    };
                ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                ViewBag.Error = "Letter code series not found please contact to administrator!";
                return View();
            }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult EmpOfferEdit(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                if (id != 0)
                {
                    var selecetd = (from e in objentity.tblLetterOfferDetail where e.intid == id select e).FirstOrDefault();
                    if (selecetd != null)
                    {
                        int fk_EmpAssId = Convert.ToInt32(selecetd.fk_HRMS_id);
                        //select employee code which not red flag or not active employee
                        var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                                 join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                                 where e.bittempstatusactive == true && e.bitstatusdeactive == false &&
                                                 e.bitIsPartialAuthorised == false && e.BitIsRedFlagging == false
                                                 && e.intcode == code
                                                 select e).ToList();
                        List<SelectListItem> allemp = new List<SelectListItem>();
                        if (allEmployeeForExp != null)
                        {
                            foreach (var emp in allEmployeeForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allemp.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                        }
                        //for department selection
                        var selDept = (from e in objentity.tblDeptMas select e).ToList();
                        if (selDept.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dpt in selDept)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dpt.intid.ToString(),
                                    Text = dpt.vchdeptname
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Departments not found",
                                Text = "Departments not found"
                            };
                            ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                        }
                        //for designation selection
                        var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                        if (selectDesig.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dess in selectDesig)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dess.intid.ToString(),
                                    Text = dess.vchdesignation
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Desigantions not found",
                                Text = "Designations not found"
                            };
                            ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                        }

                        //check is employee code is already used in experience or nor 
                        // for sub type selection
                        var selectMaster = (from e in objentity.tblLetterOfferMas select e).ToList();
                        List<SelectListItem> typeselction = new List<SelectListItem>();
                        foreach (var item2 in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item2.intid.ToString(),
                                Text = item2.vchMasType.ToString()
                            };
                            typeselction.Add(selectListItem);
                        }
                        ViewBag.OfferType = new SelectList(typeselction, "Value", "Text");
                        //for gender slecection
                        List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                             new SelectListItem
                                             {
                                                 Text="Female",
                                                 Value="2"
                                             }

                            };
                        ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                        if (selecetd.dtAppdate != null)
                        {
                            string formatdate = selecetd.dtAppdate.ToString("dd/MM/yyyy");
                            //IFormatProvider format=new IFormatProvider
                            selecetd.dtAppdate = DateTime.ParseExact(formatdate,"dd/mm/yyyy",CultureInfo.InvariantCulture);
                        }
                        return View(selecetd);
                    }
                    else
                    {
                        var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                                 join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                                 where e.bittempstatusactive == true && e.bitstatusdeactive == false &&
                                                 e.bitIsPartialAuthorised == false && e.BitIsRedFlagging == false
                                                 && e.intcode == code
                                                 select e).ToList();
                        List<SelectListItem> allemp = new List<SelectListItem>();
                        if (allEmployeeForExp != null)
                        {
                            foreach (var emp in allEmployeeForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allemp.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                        }
                        //for department selection
                        var selDept = (from e in objentity.tblDeptMas select e).ToList();
                        if (selDept.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dpt in selDept)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dpt.intid.ToString(),
                                    Text = dpt.vchdeptname
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Departments not found",
                                Text = "Departments not found"
                            };
                            ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                        }
                        //for designation selection
                        var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                        if (selectDesig.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dess in selectDesig)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dess.intid.ToString(),
                                    Text = dess.vchdesignation
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Desigantions not found",
                                Text = "Designations not found"
                            };
                            ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                        }

                        //check is employee code is already used in experience or nor 
                        // for sub type selection
                        var selectMaster = (from e in objentity.tblLetterOfferMas select e).ToList();
                        List<SelectListItem> typeselction = new List<SelectListItem>();
                        foreach (var item2 in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item2.intid.ToString(),
                                Text = item2.vchMasType.ToString()
                            };
                            typeselction.Add(selectListItem);
                        }
                        ViewBag.OfferType = new SelectList(typeselction, "Value", "Text");
                        //for gender slecection
                        List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                             new SelectListItem
                                             {
                                                 Text="Female",
                                                 Value="2"
                                             }

                            };
                        ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                        ViewBag.Error = "Selected letter detail not found please check it or contact to administrator!";
                        return View();
                    }
                    
                }
                else
                {
                    var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                             join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                             where e.bittempstatusactive == true && e.bitstatusdeactive == false &&
                                             e.bitIsPartialAuthorised == false && e.BitIsRedFlagging == false
                                             && e.intcode == code
                                             select e).ToList();
                    List<SelectListItem> allemp = new List<SelectListItem>();
                    if (allEmployeeForExp != null)
                    {
                        foreach (var emp in allEmployeeForExp)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = emp.intid.ToString(),
                                Text = emp.vchEmpFcode
                            };
                            allemp.Add(selectListItem);
                        }
                        ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                    }
                    //for department selection
                    var selDept = (from e in objentity.tblDeptMas select e).ToList();
                    if (selDept.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dpt in selDept)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dpt.intid.ToString(),
                                Text = dpt.vchdeptname
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Departments not found",
                            Text = "Departments not found"
                        };
                        ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                    }
                    //for designation selection
                    var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                    if (selectDesig.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dess in selectDesig)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dess.intid.ToString(),
                                Text = dess.vchdesignation
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Desigantions not found",
                            Text = "Designations not found"
                        };
                        ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                    }

                    //check is employee code is already used in experience or nor 
                    // for sub type selection
                    var selectMaster = (from e in objentity.tblLetterOfferMas select e).ToList();
                    List<SelectListItem> typeselction = new List<SelectListItem>();
                    foreach (var item2 in selectMaster)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = item2.intid.ToString(),
                            Text = item2.vchMasType.ToString()
                        };
                        typeselction.Add(selectListItem);
                    }
                    ViewBag.OfferType = new SelectList(typeselction, "Value", "Text");
                    //for gender slecection
                    List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                             new SelectListItem
                                             {
                                                 Text="Female",
                                                 Value="2"
                                             }

                            };
                    ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                    ViewBag.Error = "Letter id should not be null or 0";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult EmpOfferEdit(tblLetterOfferDetail objupdate)
        {
            if (Session["descript"] != null)
            {
                var selctedEmp = (from e in objentity.tblLetterOfferDetail where e.intid == objupdate.intid select e).FirstOrDefault();
                var selcetdDept = (from e in objentity.tblDeptMas where e.intid == objupdate.fk_department select e).FirstOrDefault();
                if (selctedEmp != null)
                {
                    if (objupdate.bitIshMRMSemp == true)
                    {
                        selctedEmp.bitIshMRMSemp = true;
                        //check if HRMS employee is changed
                        var selectedHrmSEMP = (from e in objentity.tblEmpAssesmentMas where e.intid == objupdate.fk_HRMS_id select e).FirstOrDefault();
                        if (selectedHrmSEMP != null)
                        {
                            selctedEmp.vchEmpCode = selectedHrmSEMP.vchEmpFcode;
                        }
                        if (objupdate.fk_HRMS_id != null)
                        {
                            selctedEmp.fk_HRMS_id = objupdate.fk_HRMS_id;
                        }
                        if (objupdate.vchGender == "1")
                        {
                            selctedEmp.vchGender = "Male";
                        }
                        if (objupdate.vchGender == "2")
                        {
                            selctedEmp.vchGender = "Female";
                        }
                        selctedEmp.fk_OfferMas_id = objupdate.fk_OfferMas_id;
                        selctedEmp.vchName = objupdate.vchName;
                        selctedEmp.vchFatherName = objupdate.vchFatherName;
                        selctedEmp.vchCity = objupdate.vchCity;
                        selctedEmp.vchState = objupdate.vchState;
                        selctedEmp.vchAddress = objupdate.vchAddress;
                        //dept id not changed
                        if (objupdate.fk_department != selctedEmp.fk_department)
                        {
                            string refNo = selctedEmp.vchRefCode;
                            //II/HR/OL/FO/24/007 index is 9 and 10 for department code replace it if dept is changed on update
                            int startIndex = 9;
                            int count = 2;
                            string replacementdept = selcetdDept.vchdepCode.ToString();
                            string new_RefNo = ReplaceSubstringAtIndex(refNo, startIndex, count, replacementdept);
                            selctedEmp.vchRefCode = new_RefNo;
                            selctedEmp.fk_department = objupdate.fk_department;
                        }
                        selctedEmp.fk_designation = objupdate.fk_designation;
                        if (objupdate.dtAppdate != null)
                        {
                            selctedEmp.dtAppdate = objupdate.dtAppdate;
                        }
                        if (objupdate.dtAcceptdate != null)
                        {
                            selctedEmp.dtAcceptdate = objupdate.dtAcceptdate;
                        }
                        if (objupdate.dtAppdate != null)
                        {
                            selctedEmp.dtDOJ = objupdate.dtDOJ;
                        }
                        selctedEmp.txtContent = objupdate.txtContent;
                        selctedEmp.vchUpdatedBy = Session["descript"].ToString();
                        selctedEmp.dtUpdated = DateTime.Now;
                        selctedEmp.vchUpdatedHost = Session["hostname"].ToString();
                        selctedEmp.vchUpdatedIp = Session["ipused"].ToString();
                        objentity.SaveChanges();                       
                        TempData["Success"] = "Letter updated successfully!";
                        return RedirectToAction("IndexEmpOfferLetter");
                    }
                    else
                    {
                        //set null or false hrms employee detail
                        selctedEmp.bitIshMRMSemp = false;
                        selctedEmp.fk_HRMS_id = null;
                        selctedEmp.vchEmpCode = null;
                        //update other detail
                        selctedEmp.fk_designation = objupdate.fk_designation;
                        //update department and update department code also in ref_no
                        if (objupdate.fk_department != selctedEmp.fk_department)
                        {
                            string refNo = selctedEmp.vchRefCode;
                            //II/HR/OL/FO/24/007 index is 9 and 10 for department code replace it if dept is changed on update
                            int startIndex = 9;
                            int count = 2;
                            string replacementdept = selcetdDept.vchdepCode.ToString();
                            string new_RefNo = ReplaceSubstringAtIndex(refNo, startIndex, count, replacementdept);
                            selctedEmp.vchRefCode = new_RefNo;
                            selctedEmp.fk_department = objupdate.fk_department;
                        }
                        selctedEmp.fk_OfferMas_id = objupdate.fk_OfferMas_id;
                        selctedEmp.vchName = objupdate.vchName;
                        selctedEmp.vchFatherName = objupdate.vchFatherName;
                        selctedEmp.vchCity = objupdate.vchCity;
                        selctedEmp.vchState = objupdate.vchState;
                        selctedEmp.vchAddress = objupdate.vchAddress;
                        selctedEmp.dtAppdate = objupdate.dtAppdate;
                        selctedEmp.dtAcceptdate = objupdate.dtAcceptdate;
                        selctedEmp.dtDOJ = objupdate.dtDOJ;
                        selctedEmp.txtContent = objupdate.txtContent;
                        selctedEmp.vchUpdatedBy = Session["descript"].ToString();
                        selctedEmp.dtUpdated = DateTime.Now;
                        selctedEmp.vchUpdatedHost = Session["hostname"].ToString();
                        selctedEmp.vchUpdatedIp = Session["ipused"].ToString();
                        objentity.SaveChanges();
                        ViewBag.Success = "Letter updated successfully!";
                        return RedirectToAction("IndexEmpOfferLetter");
                    }
                }
                else
                {
                    ViewBag.Error = "Selecetd letter detail not found please check again and try!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PrintEmpOffer(int id)
        {
            //System.Diagnostics.Debugger.Break();
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedCertificate = (from e in objentity.tblLetterOfferDetail
                                               join m in objentity.tblLetterOfferMas on e.fk_OfferMas_id equals m.intid
                                               where e.intid == id
                                               select e).FirstOrDefault();
                    if (selectedCertificate == null)
                    {
                        TempData["Error"] = "Certificate detail not found please check it again!";
                        return View();
                    }
                    else
                    {
                        LocalReport lr = new LocalReport();
                        string filepath = String.Empty;
                        HttpClient _client = new HttpClient();                      

                        //Get dataset value and fields
                        var selectedobj = (from e in objentity.spEmpOfferLetter(id) select e).ToList();

                        //get path
                        filepath = Path.Combine(Server.MapPath("~/Content/Report"), ("OfferLetter.rdl"));
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
                }
                else
                {
                    ViewBag.Error = "Offer letter id/detail not found please check it and try again!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #endregion

        #region Master and Employee Experience Letters     

        #region Experience Letter master
        public ActionResult IndexExperienceMas()
        {
            if (Session["descript"] != null)
            {
                //tblExperienceMas
                var selected = (from e in objentity.tblExperienceMas select e).ToList();
                if (selected.Count() != 0)
                {
                    ViewBag.Success = "Found " + selected.Count() + " Experience Master";
                    return View(selected);
                }
                else
                {
                    ViewBag.Empty = "Database is empty!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ViewExperienceMas(int id)
        {
            if (Session["descript"] != null)
            {
                var selected = (from e in objentity.tblExperienceMas where e.intid == id select e).FirstOrDefault();
                if (selected != null)
                {
                    return View(selected);
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

        public ActionResult EditExperienceMas(int id)
        {
            if (Session["descript"] != null)
            {
                var selectedLetter = (from e in objentity.tblExperienceMas where e.intid == id select e).FirstOrDefault();
                if (selectedLetter != null)
                {
                    //select Gender from drop down list for selection letter type

                    //for gender selection
                    List<SelectListItem> genderList = new List<SelectListItem> {

                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                },
                                             new SelectListItem{
                                                 Text="All",
                                                 Value ="3"
                                                }
                                             };
                    ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                    //for master type selection
                    List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                }
                                             };
                    ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                    return View(selectedLetter);
                }
                else
                {
                    //select Gender from drop down list for selection letter type
                    //for gender slecection
                    List<SelectListItem> genderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                },
                                              new SelectListItem{
                                                 Text="All",
                                                 Value ="3"
                                                }
                                             };
                    ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                    //for master type selection
                    List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                }
                                             };
                    ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditExperienceMas(tblExperienceMas objedit)
        {
            if (Session["descript"] != null)
            {
                if (objedit.intid != 0)
                {
                    var editexp = (from e in objentity.tblExperienceMas where e.intid == objedit.intid select e).FirstOrDefault();
                    if (editexp != null)
                    {
                        editexp.vchName = objedit.vchName;
                        editexp.vchHeading = objedit.vchHeading;
                        //For gender
                        if (objedit.vchForGender == "1" || editexp.vchForGender == "Male")
                        {
                            editexp.vchForGender = "Male";
                        }
                        if (objedit.vchForGender == "2" || editexp.vchForGender == "Female")
                        {
                            editexp.vchForGender = "Female";
                        }
                        if (objedit.vchForGender == "3" || editexp.vchForGender == "All")
                        {
                            editexp.vchForGender = "All";
                        }
                        //for mas type
                        if (objedit.vchType == "1" || objedit.vchType == "Admin")
                        {
                            editexp.vchType = "Admin";

                        }
                        if (objedit.vchType == "2" || objedit.vchType == "Nursing")
                        {
                            editexp.vchType = "Nursing";
                        }
                        editexp.txtContent = objedit.txtContent;
                        editexp.vchUpdatedBy = Session["descript"].ToString();
                        editexp.dtUpdated = DateTime.Now;
                        editexp.vchUpdatedIp = Session["ipused"].ToString();
                        editexp.vchUpdatedHost = Session["hostname"].ToString();
                        objentity.SaveChanges();
                        TempData["Success1"] = "Template updated successfully!";
                        return RedirectToAction("IndexExperienceMas");
                    }
                    else
                    {
                        //for gender slecection
                        List<SelectListItem> genderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                },
                                              new SelectListItem{
                                                 Text="All",
                                                 Value ="3"
                                                }
                                             };
                        ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                        //for master type selection
                        List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                }
                                             };
                        ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                        return View();
                    }
                }
                else
                {
                    //for gender slecection
                    List<SelectListItem> genderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                },
                                              new SelectListItem{
                                                 Text="All",
                                                 Value ="3"
                                                }
                                             };
                    ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                    //for master type selection
                    List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                }
                                             };
                    ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult NewExperienceMas()
        {
            if (Session["descript"] != null)
            {
                //select Gender from drop down list for selection letter type
                //for gender slecection
                List<SelectListItem> genderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                },
                                              new SelectListItem{
                                                 Text="All",
                                                 Value ="3"
                                                }
                                             };
                ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                //for master type selection
                List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                }
                                             };
                ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }      

        [HttpPost]
        public ActionResult NewExperienceMas(tblExperienceMas objtemplate)
        {
            if (Session["descript"] != null)
            {
                ////check letter code
                //var checklettercode = (from e in objentity.tblExperienceMas where e.vchLetterCode == objtemplate.vchLetterCode select e).FirstOrDefault();
                //if (checklettercode == null)
                //{
                var checkMasname = (from e in objentity.tblExperienceMas where e.vchName == objtemplate.vchName select e).FirstOrDefault();
                if (checkMasname == null)
                {
                    //For selecetd gender
                    if (objtemplate.vchForGender == "1")
                    {
                        objtemplate.vchForGender = "Male";
                    }
                    if (objtemplate.vchForGender == "2")
                    {
                        objtemplate.vchForGender = "Female";
                    }
                    if (objtemplate.vchForGender == "3")
                    {
                        objtemplate.vchForGender = "All";
                    }
                    //for mas type
                    if (objtemplate.vchType == "1" || objtemplate.vchType == "Admin")
                    {
                        objtemplate.vchType = "Admin";

                    }
                    if (objtemplate.vchType == "2" || objtemplate.vchType == "Nursing")
                    {
                        objtemplate.vchType = "Nursing";
                    }
                    objtemplate.vchCreatedBy = Session["descript"].ToString();
                    objtemplate.dtCreated = DateTime.Now;
                    objtemplate.vchCreatedIP = Session["ipused"].ToString();
                    objtemplate.vchCreatedHost = Session["hostname"].ToString();
                    objentity.tblExperienceMas.Add(objtemplate);
                    objentity.SaveChanges();
                    TempData["Success"] = "Master letter saved successfully!";
                    //For gender selection
                    List<SelectListItem> genderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                },
                                              new SelectListItem{
                                                 Text="All",
                                                 Value ="3"
                                                },
                                             };
                    ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                    //for master type selection
                    List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                }
                                             };
                    ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                    return RedirectToAction("NewExperienceMas");
                }
                else
                {
                    TempData["Error"] = "Master letter name already in used, please check it in view all template!";
                    //for gender selection
                    List<SelectListItem> genderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                },
                                              new SelectListItem{
                                                 Text="All",
                                                 Value ="3"
                                                },
                                             };
                    ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                    //for master type selection
                    List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                }
                                             };
                    ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                    return View();
                }
                //}
                //else
                //{

                //    TempData["Error"] = "Master code already in use, please enter another code!";
                //    //for gender selection
                //    List<SelectListItem> genderList = new List<SelectListItem> {
                //                             new SelectListItem{
                //                                  Text="Male",
                //                                  Value = "1"
                //                                 },
                //                             new SelectListItem{
                //                                 Text="Female",
                //                                 Value ="2"
                //                                },
                //                              new SelectListItem{
                //                                 Text="All",
                //                                 Value ="3"
                //                                },
                //                             };
                //    ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                //    //for master type selection
                //    List<SelectListItem> Mastype = new List<SelectListItem> {
                //                             new SelectListItem{
                //                                  Text="Admin",
                //                                  Value = "1"
                //                                 },
                //                             new SelectListItem{
                //                                 Text="Nursing",
                //                                 Value ="2"
                //                                }
                //                             };
                //    ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                //    return View();
                //}            
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Candidate Experience Letter

        public ActionResult ViewEmpExpAll()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var LetterList = (from e in objentity.tblExperienceDetail where e.intCode == code orderby e.dtCreated descending select e).ToList();
                if (LetterList.Count() != 0)
                {
                    ViewBag.Success = "Found " + LetterList.Count() + " Employee Experience Letter";
                    return View(LetterList);
                }
                else
                {
                    ViewBag.Empty = "0 Letter found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult NewEmpExp()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                //select employee code which not red flag or not active employee
                var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                         where e.bitstatusdeactive == true && e.BitIsRedFlagging == false
                                         && (e.isHrmsEmployee == true || e.bitIsByPassEntry == true || e.isPayrollEmployee == true || e.bitIsUnitEmp==true || e.bitIsLeft==true) && e.bitExpLetter!=true && e.intcode == code
                                         select e).ToList();
                List<SelectListItem> allemp = new List<SelectListItem>();
                if (allEmployeeForExp != null)
                {
                    foreach (var emp in allEmployeeForExp)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = emp.intid.ToString(),
                            Text = emp.vchEmpFcode
                        };
                        allemp.Add(selectListItem);
                    }
                    ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                }
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
                //for select master type letter 
                var selectMaster = (from e in objentity.tblExperienceMas select e).ToList();
                if (selectMaster != null)
                {
                    //for master name selection
                    List<SelectListItem> nameselection = new List<SelectListItem>();
                    foreach (var item in selectMaster)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = item.intid.ToString(),
                            Text = item.vchName
                        };
                        nameselection.Add(selectListItem);
                    }
                    ViewBag.MasType = new SelectList(nameselection, "Value", "Text");
                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = "Master not found!",
                        Text = "Master not found"
                    };
                    empty.Add(selectListItem);
                    ViewBag.MasType = new SelectList(empty, "Value", "Text");

                }
                //for department selection
                var selDept = (from e in objentity.tblDeptMas select e).ToList();
                if (selDept.Count() != 0)
                {
                    List<SelectListItem> desi = new List<SelectListItem>();
                    foreach (var dpt in selDept)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = dpt.intid.ToString(),
                            Text = dpt.vchdeptname
                        };
                        desi.Add(selectListItem);
                    }
                    ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = "Departments not found",
                        Text = "Departments not found"
                    };
                    ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                }
                //for designation selection
                var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                if (selectDesig.Count() != 0)
                {
                    List<SelectListItem> desi = new List<SelectListItem>();
                    foreach (var dess in selectDesig)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = dess.intid.ToString(),
                            Text = dess.vchdesignation
                        };
                        desi.Add(selectListItem);
                    }
                    ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = "Desigantion not found",
                        Text = "Designation not found"
                    };
                    ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                }
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //Get EMployee if does not contant hrms id in tblExperience detail
        public ActionResult GetExphrmsid(string empid)
        {
            if (Session["descript"] != null)
            {
                int id = Convert.ToInt32(empid.ToString());
                //check letter is avilable for employee or not
                var checkletter = (from e in objentity.tblExperienceDetail where e.intHRMSid == id select e).FirstOrDefault();
                if (checkletter == null)
                {
                    var selectedMas = (from e in objentity.tblEmpAssesmentMas
                                       join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                       where e.intid == id
                                       select new { e.vchName, e.vchEmpFcode, e.fk_intdeptid, e.fk_intdesiid, d.vchFatherName, d.vchpcity, d.vchpstate, d.vchpaddress, d.vchsex,e.dtDOJ,e.dtDOL }).FirstOrDefault();
                    if (selectedMas != null)
                    {
                        string getdgender = string.Empty;
                        if (selectedMas.vchsex == "Male")
                        {
                            getdgender = "1";
                        }
                        if (selectedMas.vchsex == "Female")
                        {
                            getdgender = "2";
                        }
                        var data = new
                        {
                            vchname = selectedMas.vchName,
                            vchempcode = selectedMas.vchEmpFcode,
                            fathername = selectedMas.vchFatherName,
                            city = selectedMas.vchpcity,
                            state = selectedMas.vchpstate,
                            add = selectedMas.vchpaddress,
                            deptid = selectedMas.fk_intdeptid,
                            desiid = selectedMas.fk_intdesiid,
                            vchgender = getdgender,
                            dtDOJ=selectedMas.dtDOJ,
                            dtDOL=selectedMas.dtDOL
                        };
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var error = new
                        {
                            alertmessage = "Employee detail not found!"
                        };
                        return Json(error, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var error = new
                    {
                        alertmessage = "Selected employee letter already generated, please check in view all employee letter!"
                    };
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //Get Master Letter Content
        public ActionResult GetExpContent(string selectedOption)
        {
            if (Session["descript"] != null)
            {
                int newid = Convert.ToInt32(selectedOption);
                var selectedMas = (from e in objentity.tblExperienceMas where e.intid == newid select e).FirstOrDefault();
                if (selectedMas != null)
                {
                    var data = new
                    {
                        Result = selectedMas.txtContent
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = new
                    {
                        Result = "Master deatil not found!"
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var data = "not found!";
                return Json(data);
            }
        }

        [HttpPost]
        public ActionResult NewEmpExp(tblExperienceDetail newobj)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                //Select master from HRMS for update status as experience letter is prepared to employee
                tblEmpAssesmentMas objempmas = new tblEmpAssesmentMas();
                if (newobj.intHRMSid != 0 && newobj.intHRMSid!=null)
                {
                   var Selected_HrmsEmp = (from e in objentity.tblEmpAssesmentMas where e.intid == newobj.intHRMSid select e).FirstOrDefault();
                    if (Selected_HrmsEmp != null)
                    {
                        objempmas = Selected_HrmsEmp; 
                    }
                }
                //for select master type letter used in dropdownlist return view
                var selectMaster = (from e in objentity.tblExperienceMas select e).ToList();
                //Declare for master selected object
                tblExperienceMas objExpMas = new tblExperienceMas();
                //Get Master Experience Selected
                if (newobj != null && newobj.fk_Masid != 0)
                {
                    int expMasId = Convert.ToInt32(newobj.fk_Masid.ToString());
                    objExpMas = (from e in objentity.tblExperienceMas where e.intid == expMasId select e).FirstOrDefault();
                }
                else
                {       
                    //for master selection 
                    if (objExpMas != null)
                    {
                        
                        List<SelectListItem> nameselection = new List<SelectListItem>();
                        foreach (var item in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item.intid.ToString(),
                                Text = item.vchName
                            };
                            nameselection.Add(selectListItem);
                        }
                        ViewBag.MasType = new SelectList(nameselection, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Master not found!",
                            Text = "Master not found!"
                        };
                        empty.Add(selectListItem);
                        ViewBag.MasType = new SelectList(empty, "Value", "Text");
                    }
                    //for department selection
                    var selDept = (from e in objentity.tblDeptMas select e).ToList();
                    if (selDept.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dpt in selDept)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dpt.intid.ToString(),
                                Text = dpt.vchdeptname
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                    }                  
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Departments not found",
                            Text = "Departments not found"
                        };
                        ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                    }
                    //for designation selection
                    var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                    if (selectDesig.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dess in selectDesig)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dess.intid.ToString(),
                                Text = dess.vchdesignation
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Desigantion not found",
                            Text = "Designation not found"
                        };
                        ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                    }
                    ViewBag.Error = "Master letters not found please create masters first or contact to administrator";
                    return View();
                }
                //Proposed Codes:
                //IIH/HR/EC/BB2301(EC - Experience Certificate, BB - Blood Bank)
                //IIH/HR/EC/PL2301
                //The Certificates will be issued on the letter Heads of the Company which are in the custody of the HODs / HRs only.
                string hrCharachter = "HR";
                string dptCode = string.Empty;
                string certificateCode = string.Empty;
                int certificateCurrentcount = 0;
                tblDesignationMas objdesi = new tblDesignationMas();
                if (newobj != null && newobj.fk_designationId != 0)
                {
                    int desiID = Convert.ToInt32(newobj.fk_designationId);
                    objdesi = (from e in objentity.tblDesignationMas where e.intid == desiID select e).FirstOrDefault();
                }
                int deptID = Convert.ToInt32(objdesi.intdeptid);
                var getDeptCode = (from e in objentity.tblDeptMas where e.intid == deptID select e).FirstOrDefault();
                if (getDeptCode != null)
                {
                    dptCode = getDeptCode.vchdepCode.ToString();

                }
                else
                {
                    //for master name selection
                    List<SelectListItem> nameselection = new List<SelectListItem>();
                    foreach (var item in selectMaster)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = item.intid.ToString(),
                            Text = item.vchName
                        };
                        nameselection.Add(selectListItem);
                    }
                    ViewBag.SelectName = new SelectList(nameselection, "Value", "Text");

                    //for designation selection
                    var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                    if (selectDesig.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dess in selectDesig)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dess.intid.ToString(),
                                Text = dess.vchdesignation
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                    }
                    ViewBag.Error = "Department code not found corresponding designation please check it again or contact to administrator!";
                    return View();
                }
                //gender selection
                if (newobj.vchGender == "1" || newobj.vchGender == "Male")
                {
                    newobj.vchGender = "Male";
                }
                if (newobj.vchGender == "2" || newobj.vchGender == "Female")
                {
                    newobj.vchGender = "Female";
                }
                //Get Certificate code from master
                certificateCode = objExpMas.vchLetterCode.ToString();
                int year1 = 2024;
                //get branch wise cetrificate count
                var getCode = (from e in objentity.tblLetterNumberMas where  e.intcode == code && e.intYear==year1 select e).FirstOrDefault();
                if (getCode != null)
                {
                    //get current count number of certificate
                    certificateCurrentcount = Convert.ToInt16(getCode.intCurrent);
                    int newcode = certificateCurrentcount + 1;
                    int number = newcode;
                    int counter = 0;
                    string finalnumber = "";
                    //int fnumber = 0;
                    while (number > 0)
                    {
                        number = number / 10;
                        counter++;
                    }
                    if (counter == 1)
                    {
                        finalnumber = string.Concat("00" + newcode.ToString());
                    }
                    if (counter == 2)
                    {
                        finalnumber = string.Concat("0" + newcode.ToString());
                    }
                    if (counter == 3)
                    {
                        finalnumber = newcode.ToString();
                    }
                    //Set Branch Code
                    string finalBranchCode = Session["branchCode"].ToString();
                    DateTime aajkidate = DateTime.Now;
                    string year = aajkidate.ToString("yy");
                    //Format = Branch/HR/EC/EmpDpt/yr/series
                    string Ref_Code = finalBranchCode + "/" + hrCharachter + "/" + certificateCode + "/" + dptCode + "/" + year + "/" + finalnumber;
                    newobj.vchCompany = Session["Compname"].ToString();
                    newobj.intCode = Convert.ToInt16(Session["id"].ToString());
                    newobj.vchRefCode = Ref_Code;
                    newobj.vchCreatedBy = Session["descript"].ToString();
                    newobj.dtCreated = DateTime.Now;
                    newobj.vchCreatedIP = Session["ipused"].ToString();
                    newobj.vchCreatedHost = Session["hostname"].ToString();
                    //update status of is hrms employee or not
                    if (newobj.bitIshMRMSemp == true)
                    {
                        newobj.bitIshMRMSemp = true;
                    }
                    else
                    {
                        newobj.bitIshMRMSemp = false;
                    }
                    //update hrms status and add experience letter code
                    if(objempmas!=null)
                    {
                        objempmas.bitExpLetter = true;
                        newobj.vchEmpCode=objempmas.vchEmpFcode;
                        newobj.intHRMSid = objempmas.intid;
                        newobj.bitIshMRMSemp = true;                
                    }
                    else
                    {
                        newobj.vchEmpCode = null;
                    }
                   
                    //update ref_code master count
                    getCode.intCurrent = newcode;
                    try
                    {
                        objentity.tblExperienceDetail.Add(newobj);
                        objentity.SaveChanges();
                        TempData["Success"] = "Certificate saved successfully!";
                        return RedirectToAction("NewEmpExp");
                    }
                    catch(DbEntityValidationException ex)
                    {
                        // Retrieve the validation errors
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                //TempData["Error"] = ("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                TempData["Error"] = (validationError.ErrorMessage);

                            }
                        }
                        return RedirectToAction("NewEmpExp");
                    }
                }
                else
                {
                    //Select All Department For selection department
                    var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
                    if (selectedDept.Count() != 0)
                    {
                        List<SelectListItem> newlist = new List<SelectListItem>();
                        foreach (var dpt in selectedDept)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dpt.intid.ToString(),
                                Text = dpt.vchdeptname
                            };
                            newlist.Add(selectListItem);
                        }
                        ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
                    }
                    TempData["Error"] = "Certificate code not found please check it again or contact to administrator!";
                    return RedirectToAction("NewEmpExp");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult EmpExpEdit(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                if (id != 0)
                {
                    var selecetd = (from e in objentity.tblExperienceDetail where e.intid == id select e).FirstOrDefault();
                    if (selecetd != null)
                    {
                        int fk_EmpAssId = Convert.ToInt32(selecetd.intHRMSid);
                        //select employee code which not red flag or not active employee
                        var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                         where e.bitstatusdeactive == true && e.BitIsRedFlagging == false
                                         && (e.isHrmsEmployee == true || e.bitIsByPassEntry == true || e.isPayrollEmployee == true || e.bitIsUnitEmp==true || e.bitIsLeft==true) && e.intcode == code
                                         select e).ToList();
                        List<SelectListItem> allemp = new List<SelectListItem>();
                        if (allEmployeeForExp != null)
                        {
                            foreach (var emp in allEmployeeForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allemp.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                        }
                        //for department selection
                        var selDept = (from e in objentity.tblDeptMas select e).ToList();
                        if (selDept.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dpt in selDept)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dpt.intid.ToString(),
                                    Text = dpt.vchdeptname
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Departments not found",
                                Text = "Departments not found"
                            };
                            ViewBag.SelectDpt = new SelectList(empty, "Value", "Text");
                        }
                        //for designation selection
                        var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                        if (selectDesig.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dess in selectDesig)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dess.intid.ToString(),
                                    Text = dess.vchdesignation
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Desigantions not found",
                                Text = "Designations not found"
                            };
                            ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                        }

                        //check is employee code is already used in experience or nor 
                        // for sub type selection
                        var selectMaster = (from e in objentity.tblExperienceMas select e).ToList();
                        List<SelectListItem> typeselction = new List<SelectListItem>();
                        foreach (var item2 in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item2.intid.ToString(),
                                Text = item2.vchName.ToString()
                            };
                            typeselction.Add(selectListItem);
                        }
                        ViewBag.MasType = new SelectList(typeselction, "Value", "Text");
                        //for gender slecection
                        List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{                                                  
                                                  Value = "1",
                                                  Text="Male"
                                             },
                                             new SelectListItem
                                             {
                                                 Value="2",
                                                 Text="Female"                                                 
                                             }

                            };
                        ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                        return View(selecetd);
                    }
                    else
                    {
                        var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                                 where e.bitstatusdeactive == true && e.BitIsRedFlagging == false
                                                 && (e.isHrmsEmployee == true || e.bitIsByPassEntry == true || e.isPayrollEmployee == true) && e.intcode == code
                                                 select e).ToList();
                        List<SelectListItem> allemp = new List<SelectListItem>();
                        if (allEmployeeForExp != null)
                        {
                            foreach (var emp in allEmployeeForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allemp.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "HRMS Employee not avilable",
                                Text = "HRMS Employee not avilable"
                            };
                            ViewBag.Employees = new SelectList(empty, "Value", "Text");
                        }
                        //for department selection
                        var selDept = (from e in objentity.tblDeptMas select e).ToList();
                        if (selDept.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dpt in selDept)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dpt.intid.ToString(),
                                    Text = dpt.vchdeptname
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Departments not found",
                                Text = "Departments not found"
                            };
                            ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                        }
                        //for designation selection
                        var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                        if (selectDesig.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dess in selectDesig)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dess.intid.ToString(),
                                    Text = dess.vchdesignation
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Desigantions not found",
                                Text = "Designations not found"
                            };
                            ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                        }

                        //check is employee code is already used in experience or nor 
                        // for sub type selection
                        var selectMaster = (from e in objentity.tblExperienceMas select e).ToList();
                        List<SelectListItem> typeselction = new List<SelectListItem>();
                        foreach (var item2 in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item2.intid.ToString(),
                                Text = item2.vchName.ToString()
                            };
                            typeselction.Add(selectListItem);
                        }
                        ViewBag.MasType = new SelectList(typeselction, "Value", "Text");
                        //for gender slecection
                        List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                             new SelectListItem
                                             {
                                                 Text="Female",
                                                 Value="2"
                                             }

                            };
                        ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                        ViewBag.Error = "Selected letter detail not found please check it or contact to administrator!";
                        return View();
                    }
                }
                else
                {
                    var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                             where e.bitstatusdeactive == true && e.BitIsRedFlagging == false
                                             && (e.isHrmsEmployee == true || e.bitIsByPassEntry == true || e.isPayrollEmployee == true) && e.intcode == code
                                             select e).ToList();
                    List<SelectListItem> allemp = new List<SelectListItem>();
                    if (allEmployeeForExp != null)
                    {
                        foreach (var emp in allEmployeeForExp)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = emp.intid.ToString(),
                                Text = emp.vchEmpFcode
                            };
                            allemp.Add(selectListItem);
                        }
                        ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "HRMS Employee not avilable",
                            Text = "HRMS Employee not avilable"
                        };
                        ViewBag.Employees = new SelectList(empty, "Value", "Text");
                    }
                    //for department selection
                    var selDept = (from e in objentity.tblDeptMas select e).ToList();
                    if (selDept.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dpt in selDept)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dpt.intid.ToString(),
                                Text = dpt.vchdeptname
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Departments not found",
                            Text = "Departments not found"
                        };
                        ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                    }
                    //for designation selection
                    var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                    if (selectDesig.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dess in selectDesig)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dess.intid.ToString(),
                                Text = dess.vchdesignation
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Desigantions not found",
                            Text = "Designations not found"
                        };
                        ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                    }

                    //check is employee code is already used in experience or nor 
                    // for sub type selection
                    var selectMaster = (from e in objentity.tblExperienceMas select e).ToList();
                    if (selectMaster != null)
                    {
                        List<SelectListItem> typeselction = new List<SelectListItem>();
                        foreach (var item2 in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item2.intid.ToString(),
                                Text = item2.vchName.ToString()
                            };
                            typeselction.Add(selectListItem);
                        }
                        ViewBag.OfferType = new SelectList(typeselction, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Experience Master not found.",
                            Text = "Experience Master not found."
                        };
                        ViewBag.Employees = new SelectList(empty, "Value", "Text");
                    }
                    //for gender slecection
                    List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                             new SelectListItem
                                             {
                                                 Text="Female",
                                                 Value="2"
                                             }

                            };
                    ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                    ViewBag.Error = "Letter id should not be null or 0";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult EmpExpEdit(tblExperienceDetail objedit)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                //HrmsEmpMas Object
                tblEmpAssesmentMas objEmpmas = new tblEmpAssesmentMas();
                tblEmpAssesmentMas OldHRMSEmp = new tblEmpAssesmentMas();
                if (objedit != null)
                {
                    var deptMas = (from e in objentity.tblDeptMas where e.intid == objedit.fk_department select e).FirstOrDefault();
                    var selectedObj = (from e in objentity.tblExperienceDetail where e.intid == objedit.intid select e).FirstOrDefault();
                    if (selectedObj != null)
                    {

                        //get empHRMS Mas and update letter details if  
                        if (objedit.intHRMSid != 0 && objedit.intHRMSid != null)
                        {
                            objEmpmas = (from e in objentity.tblEmpAssesmentMas where e.intid == objedit.intHRMSid select e).FirstOrDefault();
                            if (objEmpmas != null)
                            { 
                                //check employee updated?
                                if (objEmpmas.intid != selectedObj.intHRMSid)
                                {
                                    if (selectedObj.intHRMSid != null && selectedObj.intHRMSid != 0)
                                    {
                                        //now change old hrms emp status
                                        OldHRMSEmp = (from e in objentity.tblEmpAssesmentMas where e.intid == selectedObj.intHRMSid select e).FirstOrDefault();
                                        OldHRMSEmp.bitExpLetter = false;
                                        //
                                        selectedObj.vchEmpCode = objEmpmas.vchEmpFcode;
                                        selectedObj.intHRMSid = objEmpmas.intid;
                                        objEmpmas.bitExpLetter = true;
                                    } 
                                }
                                else
                                {

                                }
                            }
                        }
                        //update ref_code if department is changed othwise it will be same as old
                        if (objedit.fk_department != selectedObj.fk_department)
                        {
                            string refNo = selectedObj.vchRefCode;
                            //II/HR/OL/FO/24/007 index is 9 and 10 for department code replace it if dept is changed on update
                            int startIndex = 9;
                            int count = 2;
                            string replacementdept = deptMas.vchdepCode.ToString();
                            //call ref_code replacement function for ref_no update
                            string new_RefNo = ReplaceSubstringAtIndex(refNo, startIndex, count, replacementdept);
                            selectedObj.vchRefCode = new_RefNo;
                            selectedObj.fk_department = objedit.fk_department;
                        }
                        // if designation is changed update it other leave as old
                        if (objedit.fk_designationId != selectedObj.fk_designationId)
                        {
                            selectedObj.fk_designationId = objedit.fk_designationId;
                        }
                        selectedObj.vchName = objedit.vchName;
                        selectedObj.vchFatherName = objedit.vchFatherName;                        
                        selectedObj.dtSdate = objedit.dtSdate;
                        selectedObj.dtEdate = objedit.dtEdate;
                        selectedObj.dtRelieving = objedit.dtRelieving;
                        selectedObj.txtContent = objedit.txtContent;
                        //gender selection
                        if (objedit.vchGender == "1" || objedit.vchGender == "Male")
                        {
                            selectedObj.vchGender = "Male";
                        }
                        if (objedit.vchGender == "2" || objedit.vchGender == "Female")
                        {
                            selectedObj.vchGender = "Female";
                        }
                        selectedObj.vchState = objedit.vchState;
                        selectedObj.vchCity = objedit.vchCity;
                        selectedObj.vchAddress = objedit.vchAddress;
                        //Update user details for updation 
                        selectedObj.vchUpdatedBy = Session["descript"].ToString();
                        selectedObj.vchUpdatedHost = Session["hostname"].ToString();
                        selectedObj.dtUpdated = DateTime.Now;
                        selectedObj.vchUpdatedIp = Session["ipused"].ToString();
                        objentity.SaveChanges();
                        TempData["Success"] = "Certificate detail updated successfully!";
                        return RedirectToAction("ViewEmpExpAll");
                    }
                    else
                    {
                        var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                                 where e.bittempstatusactive == true && e.bitstatusdeactive == false && e.BitIsRedFlagging == false
                                                 && e.isHrmsEmployee == true && e.intcode == code
                                                 select e).ToList();
                        List<SelectListItem> allemp = new List<SelectListItem>();
                        if (allEmployeeForExp != null)
                        {
                            foreach (var emp in allEmployeeForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allemp.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "HRMS Employee not avilable",
                                Text = "HRMS Employee not avilable"
                            };
                            ViewBag.Employees = new SelectList(empty, "Value", "Text");
                        }
                        //for department selection
                        var selDept = (from e in objentity.tblDeptMas select e).ToList();
                        if (selDept.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dpt in selDept)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dpt.intid.ToString(),
                                    Text = dpt.vchdeptname
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Departments not found",
                                Text = "Departments not found"
                            };
                            ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                        }
                        //for designation selection
                        var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                        if (selectDesig.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dess in selectDesig)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dess.intid.ToString(),
                                    Text = dess.vchdesignation
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Desigantions not found",
                                Text = "Designations not found"
                            };
                            ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                        }

                        //check is employee code is already used in experience or nor 
                        // for sub type selection
                        var selectMaster = (from e in objentity.tblExperienceMas select e).ToList();
                        if (selectMaster != null)
                        {
                            List<SelectListItem> typeselction = new List<SelectListItem>();
                            foreach (var item2 in selectMaster)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = item2.intid.ToString(),
                                    Text = item2.vchName.ToString()
                                };
                                typeselction.Add(selectListItem);
                            }
                            ViewBag.MasType = new SelectList(typeselction, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Experience Master not found.",
                                Text = "Experience Master not found."
                            };
                            ViewBag.Employees = new SelectList(empty, "Value", "Text");
                        }
                        //for gender slecection
                        List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                             new SelectListItem
                                             {
                                                 Text="Female",
                                                 Value="2"
                                             }

                            };
                        ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                        ViewBag.Error = "Selected certificate detail not found please try again or contact to administrator!";
                        return View();
                    }
                }
                else
                {
                    var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                             where e.bittempstatusactive == true && e.bitstatusdeactive == false && e.BitIsRedFlagging == false
                                             && e.isHrmsEmployee == true && e.intcode == code
                                             select e).ToList();
                    List<SelectListItem> allemp = new List<SelectListItem>();
                    if (allEmployeeForExp != null)
                    {
                        foreach (var emp in allEmployeeForExp)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = emp.intid.ToString(),
                                Text = emp.vchEmpFcode
                            };
                            allemp.Add(selectListItem);
                        }
                        ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "HRMS Employee not avilable",
                            Text = "HRMS Employee not avilable"
                        };
                        ViewBag.Employees = new SelectList(empty, "Value", "Text");
                    }
                    //for department selection
                    var selDept = (from e in objentity.tblDeptMas select e).ToList();
                    if (selDept.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dpt in selDept)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dpt.intid.ToString(),
                                Text = dpt.vchdeptname
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Departments not found",
                            Text = "Departments not found"
                        };
                        ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                    }
                    //for designation selection
                    var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                    if (selectDesig.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dess in selectDesig)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dess.intid.ToString(),
                                Text = dess.vchdesignation
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Desigantions not found",
                            Text = "Designations not found"
                        };
                        ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                    }

                    //check is employee code is already used in experience or nor 
                    // for sub type selection
                    var selectMaster = (from e in objentity.tblExperienceMas select e).ToList();
                    if (selectMaster != null)
                    {
                        List<SelectListItem> typeselction = new List<SelectListItem>();
                        foreach (var item2 in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item2.intid.ToString(),
                                Text = item2.vchName.ToString()
                            };
                            typeselction.Add(selectListItem);
                        }
                        ViewBag.OfferType = new SelectList(typeselction, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Experience Master not found.",
                            Text = "Experience Master not found."
                        };
                        ViewBag.Employees = new SelectList(empty, "Value", "Text");
                    }
                    //for gender slecection
                    List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                             new SelectListItem
                                             {
                                                 Text="Female",
                                                 Value="2"
                                             }

                            };
                    ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                    ViewBag.Error = "Model error generated contact to administrator!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }       

        public ActionResult EmpExperiencePrint(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedCertificate = (from e in objentity.tblExperienceDetail
                                               join m in objentity.tblExperienceMas on e.fk_Masid equals m.intid
                                               where e.intid == id
                                               select e).FirstOrDefault();
                    if (selectedCertificate == null)
                    {
                        ViewBag.Error = "Certificate detail not found please check it again!";
                        return View();
                    }
                    else
                    {
                        //get file and master id
                        string Report_Name = string.Empty;
                        // for admin female
                        if (selectedCertificate.vchGender == "Female" && selectedCertificate.tblExperienceMas.vchType == "Admin")
                        {
                            Report_Name = "ExpAdminFemale.rdl";
                        }
                        // admin male
                        if(selectedCertificate.vchGender=="Male" && selectedCertificate.tblExperienceMas.vchType == "Admin")
                        {
                            Report_Name = "ExpAdminMale.rdl";
                        }
                        //for nursing male
                        if (selectedCertificate.vchGender == "Male" && selectedCertificate.tblExperienceMas.vchType == "Nursing")
                        {
                            Report_Name = "ExpNursingMale.rdl";
                        }
                        //for nursing female
                        if (selectedCertificate.vchGender == "Female" && selectedCertificate.tblExperienceMas.vchType == "Nursing")
                        {
                            Report_Name = "ExpNursingFemale.rdl";
                        }
                        LocalReport lr = new LocalReport();
                        string filepath = String.Empty;
                        HttpClient _client = new HttpClient();
                        string CertificateType = selectedCertificate.tblExperienceMas.vchType.ToString();
                        //Get dataset value and fields
                        var selectedobj = (from e in objentity.spEmpExpLetter(id) select e).ToList();
                        //get path                
                        filepath = Path.Combine(Server.MapPath("~/Content/Report"), (Report_Name));
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
                }
                else
                {
                    TempData["Error"] = "Offer letter id/detail not found please check it and try again!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }

        }


        #endregion

        #endregion

        #region Internship Master and Employee Experience

        #region Intership Master

        public ActionResult IndexInterShip()
        {
            if (Session["descript"] != null)
            {
                var selectedList = (from e in objentity.tblIntershipMas select e).ToList();
                if (selectedList.Count() != 0)
                {
                    ViewBag.Success = "Found " + selectedList.Count() + " Internship certificate master";
                    return View(selectedList);
                }
                else
                {
                    ViewBag.Empty = "0 Records found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult NewIntershipMas()
        {
            if (Session["descript"] != null)
            {               
                //for gender slecection
                List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                }
                };
                ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                //for master type selection
                List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                }
                                             };
                ViewBag.Mastype = new SelectList(Mastype, "Value", "Text");
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }     

        [HttpPost]
        public ActionResult NewIntershipMas(tblIntershipMas objnew)
        {
            if (Session["descript"] != null)
            {
                //For selecetd gender
                if (objnew.vchForGender == "1")
                {
                    objnew.vchForGender = "Male";
                }
                if (objnew.vchForGender == "2")
                {
                    objnew.vchForGender = "Female";
                }                
                objnew.vchCreatedBy = Session["descript"].ToString();
                objnew.dtCreated = DateTime.Now;
                objnew.vchCreatedHost = Session["hostname"].ToString();
                objnew.vchCreatedIP = Session["ipused"].ToString();
                objnew.intcode = Convert.ToInt32(Session["id"].ToString());
                objnew.intyear = Convert.ToInt32(Session["yr"].ToString());
                objentity.tblIntershipMas.Add(objnew);
                objentity.SaveChanges();
                TempData["Success"] = "Master letter saved successfully!";
                return RedirectToAction("NewIntershipMas");
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        public ActionResult EditIntershipMas(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedMas = (from e in objentity.tblIntershipMas where e.intid == id select e).FirstOrDefault();
                    if (selectedMas != null)
                    {
                        if (selectedMas.vchForGender == "Male")
                        {
                            selectedMas.vchForGender = "1";
                        }
                        else
                        {
                            selectedMas.vchForGender = "2";
                        }
                        //for gender slecection
                        List<SelectListItem> interngenderList = new List<SelectListItem>
                        {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"                                                  
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"                                                 
                                             }
                        };
                        ViewBag.GenderList = new SelectList(interngenderList, "Value", "Text");
                        return View(selectedMas);
                    }
                    else
                    {
                        //for master type selection
                        List<SelectListItem> OfferMastype = new List<SelectListItem>
                        {
                                             new SelectListItem{
                                                  Text="Master internship letter for male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Master internship letter for female",
                                                 Value ="2"
                                                }
                        };
                        ViewBag.OfferType = new SelectList(OfferMastype, "Value", "Text");
                        //for gender slecection
                        List<SelectListItem> offergenderList = new List<SelectListItem>
                        {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                }
                        };
                        ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                        ViewBag.Error = "Selected letter detail not found please check it again and try!";
                        return View();
                    }
                }
                else
                {
                    //for master type selection
                    List<SelectListItem> OfferMastype = new List<SelectListItem>
                        {
                                             new SelectListItem{
                                                  Text="Master internship letter for male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Master internship letter for female",
                                                 Value ="2"
                                                }
                        };
                    ViewBag.OfferType = new SelectList(OfferMastype, "Value", "Text");
                    //for gender slecection
                    List<SelectListItem> offergenderList = new List<SelectListItem>
                        {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                }
                        };
                    ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                    TempData["Error"] = "Letter id should not be null or 0 please check again and try!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditIntershipMas(tblIntershipMas objupdate)
        {
            if (Session["descript"] != null)
            {
                int id = objupdate.intid;
                var updateLetter = (from e in objentity.tblIntershipMas where e.intid == id select e).FirstOrDefault();
                if (updateLetter != null)
                {
                    //For selecetd gender
                    if (objupdate.vchForGender == "1")
                    {
                        updateLetter.vchForGender = "Male";
                    }
                    if (objupdate.vchForGender == "2")
                    {
                        updateLetter.vchForGender = "Female";
                    }
                    //for mas type
                    if (objupdate.vchMasName == "1" || objupdate.vchMasName == "Master internship letter for male")
                    {
                        updateLetter.vchMasName = "Master internship letter for male";

                    }
                    if (objupdate.vchMasName == "2" || objupdate.vchMasName == "Master internship letter for female")
                    {
                        updateLetter.vchMasName = "Master internship letter for female";
                    }                    
                    updateLetter.vchLetterCode = objupdate.vchLetterCode;
                    updateLetter.vchLetterHeading = objupdate.vchLetterHeading;
                    updateLetter.txtMasContent = objupdate.txtMasContent;
                    updateLetter.dtUpdated = objupdate.dtUpdated;
                    updateLetter.vchUpdatedBy = objupdate.vchUpdatedBy;
                    updateLetter.vchUpdatedHost = objupdate.vchUpdatedHost;
                    updateLetter.vchUpdatedIp = objupdate.vchUpdatedIp;
                    objentity.SaveChanges();
                    ViewBag.Success = "Intership mater updated successfully!";
                    return RedirectToAction("IndexInterShip");
                }
                else
                {
                    TempData["Error"] = "Selected letter detail not found in database please check it again n try!";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult ViewIntershipMas(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedLetter = (from e in objentity.tblIntershipMas where e.intid == id select e).FirstOrDefault();
                    if (selectedLetter != null)
                    {
                        return View(selectedLetter);
                    }
                    else
                    {
                        TempData["Error"] = "Letter detail not found please check it and try again!";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "Letter Id should not be null or 0!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Employee Intership Letter
        public ActionResult EmpIndexIntership()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var letterList = (from e in objentity.tblIntershipDetail where e.intcode == code orderby e.dtCreated descending select e).ToList();
                if (letterList.Count() != 0)
                {
                    ViewBag.Success="Found "+letterList.Count()+" Intership certificate";
                    return View(letterList);
                }
                else
                {
                    ViewBag.Empty = "0 Records found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1");
            }
        }

        public ActionResult EmpNewIntership()
        {
            int code = Convert.ToInt16(Session["id"].ToString());
            //select employee code which not red flag or not active employee
            var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                     join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                     where e.bittempstatusactive == true && e.bitstatusdeactive == false &&
                                     e.bitIsPartialAuthorised == false && e.BitIsRedFlagging == false
                                     && e.bitInterShipLetter!=true && e.intcode == code
                                     select e).ToList();
            List<SelectListItem> allemp = new List<SelectListItem>();
            if (allEmployeeForExp != null)
            {
                foreach (var emp in allEmployeeForExp)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = emp.intid.ToString(),
                        Text = emp.vchEmpFcode
                    };
                    allemp.Add(selectListItem);
                }
                ViewBag.Employees = new SelectList(allemp, "Value", "Text");
            }
            //for department selection
            var selDept = (from e in objentity.tblDeptMas select e).ToList();
            if (selDept.Count() != 0)
            {
                List<SelectListItem> desi = new List<SelectListItem>();
                foreach (var dpt in selDept)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = dpt.intid.ToString(),
                        Text = dpt.vchdeptname
                    };
                    desi.Add(selectListItem);
                }
                ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
            }
            else
            {
                List<SelectListItem> empty = new List<SelectListItem>();
                SelectListItem selectListItem = new SelectListItem
                {
                    Value = "Departments not found",
                    Text = "Departments not found"
                };
                ViewBag.SelectDpt = new SelectList(empty, "Value", "Text");
            }
            //for designation selection
            var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
            if (selectDesig.Count() != 0)
            {
                List<SelectListItem> desi = new List<SelectListItem>();
                foreach (var dess in selectDesig)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = dess.intid.ToString(),
                        Text = dess.vchdesignation
                    };
                    desi.Add(selectListItem);
                }
                ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
            }
            else
            {
                List<SelectListItem> empty = new List<SelectListItem>();
                SelectListItem selectListItem = new SelectListItem
                {
                    Value = "Desigantions not found",
                    Text = "Designations not found"
                };
                ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
            }
            //check is employee code is already used in experience or nor 
            // for Master type selection
            var selectMaster = (from e in objentity.tblIntershipMas select e).ToList();
            List<SelectListItem> typeselction = new List<SelectListItem>();
            if (selectMaster != null)
            {
                foreach (var item2 in selectMaster)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = item2.intid.ToString(),
                        Text = item2.vchMasName.ToString()
                    };
                    typeselction.Add(selectListItem);
                }
                ViewBag.MasterType = new SelectList(typeselction, "Value", "Text");
            }
            else
            {
                List<SelectListItem> empty = new List<SelectListItem>();
                SelectListItem selectListItem = new SelectListItem
                {
                    Value = "Master not found",
                    Text = "Master not found"
                };
                ViewBag.MasterType = new SelectList(empty, "Value", "Text");
            }
            //for gender slecection
            List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                }
                };
            ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
            return View();
        }

        public ActionResult GetInternhrmsid(string empid)
        {
            if (Session["descript"] != null)
            {
                int id = Convert.ToInt32(empid.ToString());
                //check letter is avilable for employee or not
                var checkletter = (from e in objentity.tblIntershipDetail where e.fk_hrmsEMPid == id select e).FirstOrDefault();
                if (checkletter == null)
                {
                    var selectedMas = (from e in objentity.tblEmpAssesmentMas
                                       join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                       where e.intid == id
                                       select new { e.vchName, e.vchEmpFcode, e.fk_intdeptid, e.fk_intdesiid, d.vchFatherName, d.vchpcity, d.vchpstate, d.vchpaddress,d.vchsex }).FirstOrDefault();
                    if (selectedMas != null)
                    {
                        string getdgender = string.Empty;
                        if (selectedMas.vchsex == "Male")
                        {
                            getdgender = "1";
                        }
                        if (selectedMas.vchsex == "Female")
                        {
                            getdgender = "2";
                        }
                        var data = new
                        {
                            vchname = selectedMas.vchName,
                            vchempcode = selectedMas.vchEmpFcode,
                            fathername = selectedMas.vchFatherName,
                            city = selectedMas.vchpcity,
                            state = selectedMas.vchpstate,
                            add = selectedMas.vchpaddress,
                            deptid = selectedMas.fk_intdeptid,
                            desiid = selectedMas.fk_intdesiid,
                            Gender= getdgender
                        };
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var error = new
                        {
                            alertmessage = "Employee detail not found!"
                        };
                        return Json(error, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var error = new
                    {
                        alertmessage = "Selected employee letter already generated, please check in view all employee letter!"
                    };
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult GetInternshipMasContent(string selectedOption)
        {
            if (Session["descript"] != null)
            {
                if (selectedOption != null)
                {
                    int id = Convert.ToInt32(selectedOption);
                    var selectedMas = (from e in objentity.tblIntershipMas where e.intid == id select e).FirstOrDefault();
                    if (selectedMas != null)
                    {
                        var data = new
                        {
                            Result = selectedMas.txtMasContent.ToString()
                        };
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var data = new
                        {
                            Result = "Master content not found!"
                        };
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var data = new
                    {
                        Result = "Master id shuld not be 0!"
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult EmpNewIntership(tblIntershipDetail objnew)
        {
            if (Session["descript"] != null)
            {
                //get selecetd HRMS code
                tblEmpAssesmentMas objEmpMas = new tblEmpAssesmentMas();
                if (objnew.fk_hrmsEMPid != 0 || objnew.fk_hrmsEMPid != null)
                {
                    objEmpMas = (from e in objentity.tblEmpAssesmentMas where e.intid == objnew.fk_hrmsEMPid select e).FirstOrDefault();
                    if (objEmpMas != null)
                    {
                        objnew.bitIsHRMSemp = true;
                        objnew.vchEmpCode = objEmpMas.vchEmpFcode;
                        objEmpMas.bitInterShipLetter = true;
                    }
                }
                //create letter code 
                int code = Convert.ToInt32(Session["id"].ToString());
                string finalBranchCode = Session["branchCode"].ToString();
                string hrCharachter = "HR";
                int masid = Convert.ToInt32(objnew.fk_Masid.ToString());
                var selectedMas = (from e in objentity.tblIntershipMas where e.intid == masid select e).FirstOrDefault();
                string certificateCode = selectedMas.vchLetterCode;
                var selectedDept = (from e in objentity.tblDeptMas where e.intid == objnew.fk_department select e).FirstOrDefault();
                string deptCode = selectedDept.vchdepCode;
                string CodeYear = DateTime.Now.ToString("yy");
                int year = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int certificateCurrentcount = 0;
                var getCode = (from e in objentity.tblLetterNumberMas where  e.intcode == code && e.intYear==year select e).FirstOrDefault();
                if (getCode != null)
                {
                    //get current count number of certificate
                    certificateCurrentcount = Convert.ToInt16(getCode.intCurrent);
                    int newcode = certificateCurrentcount + 1;
                    int number = newcode;
                    int counter = 0;
                    string finalnumber = "";
                    //int fnumber = 0;
                    while (number > 0)
                    {
                        number = number / 10;
                        counter++;
                    }
                    if (counter == 1)
                    {
                        finalnumber = string.Concat("00" + newcode.ToString());
                    }
                    if (counter == 2)
                    {
                        finalnumber = string.Concat("0" + newcode.ToString());
                    }
                    if (counter == 3)
                    {
                        finalnumber = newcode.ToString();
                    }
                    //Format = Branch/HR/EC/EmpDpt/yr/series
                    string Ref_Code = finalBranchCode + "/" + hrCharachter + "/" + certificateCode + "/" + deptCode + "/" + CodeYear + "/" + finalnumber;
                    objnew.vchRef_No = Ref_Code;                   
                    //update current digit with new digit
                    getCode.intCurrent = newcode;
                    int yr = Convert.ToInt32(Session["yr"].ToString());                    
                    objnew.vchCreatedBy = Session["descript"].ToString();
                    objnew.dtCreated = DateTime.Now;
                    objnew.vchCreatedHost = Session["hostname"].ToString();
                    objnew.vchCreatedIP = Session["ipused"].ToString();
                    objnew.intcode = code;
                    objnew.intyear = yr;
                    objnew.vchCompany = Session["Compname"].ToString();
                    objentity.tblIntershipDetail.Add(objnew); 
                    objentity.SaveChanges();
                    TempData["Success"] = "Letter saved successfully!";
                    return RedirectToAction("EmpIndexIntership");
                }
                else
                {
                    //select employee code which not red flag or not active employee
                    var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                             join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                             where e.bittempstatusactive == true && e.bitstatusdeactive == false &&
                                             e.bitIsPartialAuthorised == false && e.BitIsRedFlagging == false
                                             && e.intcode == code
                                             select e).ToList();                    
                    if (allEmployeeForExp.Count() != 0)
                    {
                        List<SelectListItem> allemp = new List<SelectListItem>();
                        foreach (var emp in allEmployeeForExp)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = emp.intid.ToString(),
                                Text = emp.vchEmpFcode
                            };
                            allemp.Add(selectListItem);
                        }
                        ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Empty employee list",
                            Text = "Empty employee list"
                        };
                        empty.Add(selectListItem);
                        ViewBag.Employees = new SelectList(empty, "Value", "Text");
                    }
                    //for department selection
                    var selDept = (from e in objentity.tblDeptMas select e).ToList();
                    if (selDept.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dpt in selDept)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dpt.intid.ToString(),
                                Text = dpt.vchdeptname
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Departments not found",
                            Text = "Departments not found"
                        };
                        ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                    }
                    //for designation selection
                    var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                    if (selectDesig.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dess in selectDesig)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dess.intid.ToString(),
                                Text = dess.vchdesignation
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Desigantions not found",
                            Text = "Designations not found"
                        };
                        ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                    }
                    //check is employee code is already used in experience or nor 
                    // for Master type selection
                    var selectMaster = (from e in objentity.tblIntershipMas select e).ToList();                                     
                    if (selectMaster.Count() != 0)
                    {
                        List<SelectListItem> typeselction = new List<SelectListItem>();
                        foreach (var item2 in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item2.intid.ToString(),
                                Text = item2.vchMasName.ToString()
                            };
                            typeselction.Add(selectListItem);
                        }
                        ViewBag.MasterType = new SelectList(typeselction, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Master not found",
                            Text = "Master not found"
                        };
                        ViewBag.MasterType = new SelectList(empty, "Value", "Text");
                    }
                    ViewBag.Error = "Letter code series not found please contact to administrator!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult EmpEditIntership(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                if (id != 0)
                {
                    var selecetd = (from e in objentity.tblIntershipDetail where e.intid == id select e).FirstOrDefault();

                    if (selecetd != null)
                    {
                        //for seleceted gender
                        //if (selecetd.vchGender == "Male")
                        //{
                        //    selecetd.vchGender = "1";
                        //}
                        //if (selecetd.vchGender == "Female")
                        //{
                        //    selecetd.vchGender = "2";
                        //}
                        int fk_EmpAssId = Convert.ToInt32(selecetd.fk_hrmsEMPid);
                        //select employee code which not red flag or not active employee
                        var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                                 where e.bittempstatusactive == true && e.bitstatusdeactive == false && e.BitIsRedFlagging == false
                                                 && e.isHrmsEmployee == true && e.intcode == code
                                                 select e).ToList();
                        List<SelectListItem> allemp = new List<SelectListItem>();
                        if (allEmployeeForExp != null)
                        {
                            foreach (var emp in allEmployeeForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allemp.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                        }
                        //for department selection
                        var selDept = (from e in objentity.tblDeptMas select e).ToList();
                        if (selDept.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dpt in selDept)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dpt.intid.ToString(),
                                    Text = dpt.vchdeptname
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Departments not found",
                                Text = "Departments not found"
                            };
                            ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                        }
                        //for designation selection
                        var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                        if (selectDesig.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dess in selectDesig)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dess.intid.ToString(),
                                    Text = dess.vchdesignation
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Desigantions not found",
                                Text = "Designations not found"
                            };
                            ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                        }

                        //check is employee code is already used in experience or nor 
                        // for sub type selection
                        var selectMaster = (from e in objentity.tblIntershipMas select e).ToList();
                        List<SelectListItem> typeselction = new List<SelectListItem>();
                        foreach (var item2 in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item2.intid.ToString(),
                                Text = item2.vchMasName.ToString()
                            };
                            typeselction.Add(selectListItem);
                        }
                        ViewBag.MasType = new SelectList(typeselction, "Value", "Text");
                        //for gender slecection
                        List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                }
                        };
                        ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                        return View(selecetd);
                    }
                    else
                    {
                        //select employee code which not red flag or not active employee
                        var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                                 where e.bittempstatusactive == true && e.bitstatusdeactive == false && e.BitIsRedFlagging == false
                                                 && e.isHrmsEmployee == true && e.intcode == code
                                                 select e).ToList();
                        List<SelectListItem> allemp = new List<SelectListItem>();
                        if (allEmployeeForExp != null)
                        {
                            foreach (var emp in allEmployeeForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allemp.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                        }
                        //for department selection
                        var selDept = (from e in objentity.tblDeptMas select e).ToList();
                        if (selDept.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dpt in selDept)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dpt.intid.ToString(),
                                    Text = dpt.vchdeptname
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Departments not found",
                                Text = "Departments not found"
                            };
                            ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                        }
                        //for designation selection
                        var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                        if (selectDesig.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dess in selectDesig)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dess.intid.ToString(),
                                    Text = dess.vchdesignation
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Desigantions not found",
                                Text = "Designations not found"
                            };
                            ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                        }
                        //check is employee code is already used in experience or nor 
                        // for sub type selection
                        var selectMaster = (from e in objentity.tblIntershipMas select e).ToList();
                        List<SelectListItem> typeselction = new List<SelectListItem>();
                        foreach (var item2 in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item2.intid.ToString(),
                                Text = item2.vchMasName.ToString()
                            };
                            typeselction.Add(selectListItem);
                        }
                        ViewBag.MasType = new SelectList(typeselction, "Value", "Text");
                        //for gender slecection
                        List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                }
                        };
                        ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                        ViewBag.Error = "Selected letter detail not found please check it or contact to administrator!";
                        return View();
                    }
                }
                else
                {
                    //select employee code which not red flag or not active employee
                    var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                             join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                             where e.bittempstatusactive == true && e.bitstatusdeactive == false &&
                                             e.bitIsPartialAuthorised == false && e.BitIsRedFlagging == false
                                             && e.intcode == code
                                             select e).ToList();
                    List<SelectListItem> allemp = new List<SelectListItem>();
                    if (allEmployeeForExp != null)
                    {
                        foreach (var emp in allEmployeeForExp)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = emp.intid.ToString(),
                                Text = emp.vchEmpFcode
                            };
                            allemp.Add(selectListItem);
                        }
                        ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                    }
                    //for department selection
                    var selDept = (from e in objentity.tblDeptMas select e).ToList();
                    if (selDept.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dpt in selDept)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dpt.intid.ToString(),
                                Text = dpt.vchdeptname
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Departments not found",
                            Text = "Departments not found"
                        };
                        ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                    }
                    //for designation selection
                    var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                    if (selectDesig.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dess in selectDesig)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dess.intid.ToString(),
                                Text = dess.vchdesignation
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Desigantions not found",
                            Text = "Designations not found"
                        };
                        ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                    }
                    //check is employee code is already used in experience or nor 
                    // for sub type selection
                    var selectMaster = (from e in objentity.tblIntershipMas select e).ToList();
                    List<SelectListItem> typeselction = new List<SelectListItem>();
                    foreach (var item2 in selectMaster)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = item2.intid.ToString(),
                            Text = item2.vchMasName.ToString()
                        };
                        typeselction.Add(selectListItem);
                    }
                    ViewBag.MasType = new SelectList(typeselction, "Value", "Text");
                    ViewBag.Error = "Employee id should not be null or 0";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }               
            
        }

        [HttpPost]
        public ActionResult EmpEditIntership(tblIntershipDetail objupdate)
        {
            if (Session["descript"] != null)
            {
                var selctedEmp = (from e in objentity.tblIntershipDetail where e.intid == objupdate.intid select e).FirstOrDefault();
                var selcetdDept = (from e in objentity.tblDeptMas where e.intid == objupdate.fk_department select e).FirstOrDefault();
                tblEmpAssesmentMas OldHrmsEmp = new tblEmpAssesmentMas();
                if (selctedEmp != null)
                {
                    if (objupdate.bitIsHRMSemp == true)
                    {
                        selctedEmp.bitIsHRMSemp = true;
                        //check if HRMS employee is changed
                        var selectedHrmSEMP = (from e in objentity.tblEmpAssesmentMas where e.intid == objupdate.fk_hrmsEMPid select e).FirstOrDefault();
                        if (selectedHrmSEMP != null)
                        {
                            //check employee updated?
                            if (selectedHrmSEMP.intid != selctedEmp.fk_hrmsEMPid)
                            {
                                //now change old hrms emp status
                                OldHrmsEmp = (from e in objentity.tblEmpAssesmentMas where e.intid == selctedEmp.fk_hrmsEMPid select e).FirstOrDefault();
                                OldHrmsEmp.bitExpLetter = false;                                
                                selctedEmp.vchEmpCode = selectedHrmSEMP.vchEmpFcode;
                                selctedEmp.fk_hrmsEMPid = selectedHrmSEMP.intid;
                                selectedHrmSEMP.bitExpLetter = true;
                            }                            
                        }
                        //if (objupdate.vchGender == "1")
                        //{
                        //    objupdate.vchGender = "Male";
                        //}
                        //if (objupdate.vchGender == "2")
                        //{
                        //    objupdate.vchGender = "Female";
                        //}
                        if (objupdate.fk_hrmsEMPid != null)
                        {
                            selctedEmp.fk_hrmsEMPid = objupdate.fk_hrmsEMPid;
                        }
                        selctedEmp.fk_Masid = objupdate.fk_Masid;
                        selctedEmp.vchName = objupdate.vchName;
                        selctedEmp.vchFatherName = objupdate.vchFatherName;
                        selctedEmp.vchCity = objupdate.vchCity;
                        selctedEmp.vchState = objupdate.vchState;
                        selctedEmp.vchAddress = objupdate.vchAddress;
                        //dept id not changed
                        if (objupdate.fk_department != selctedEmp.fk_department)
                        {
                            string refNo = selctedEmp.vchRef_No;
                            //II/HR/OL/FO/24/007 index is 9 and 10 for department code replace it if dept is changed on update
                            //Call replacement function for department code replacement
                            int startIndex = 9;
                            int count = 2;
                            string replacementdept = selcetdDept.vchdepCode.ToString();
                            string new_RefNo = ReplaceSubstringAtIndex(refNo, startIndex, count, replacementdept);
                            selctedEmp.vchRef_No = new_RefNo;
                            selctedEmp.fk_department = objupdate.fk_department;
                        }
                        selctedEmp.fk_designation = objupdate.fk_designation;
                        selctedEmp.dtApplication = objupdate.dtApplication;
                        selctedEmp.dtDOS = objupdate.dtDOS;
                        selctedEmp.dtDOE = objupdate.dtDOE;
                        selctedEmp.txtContent = objupdate.txtContent;
                        selctedEmp.vchUpdatedBy = Session["descript"].ToString();
                        selctedEmp.dtUpdated = DateTime.Now;
                        selctedEmp.vchUpdatedHost = Session["hostname"].ToString();
                        selctedEmp.vchUpdatedIp = Session["ipused"].ToString();
                        objentity.SaveChanges();
                        TempData["Success"] = "Letter updated successfully!";
                        return RedirectToAction("EmpIndexIntership");
                    }
                    else
                    {
                        //set null or false hrms employee detail
                        selctedEmp.bitIsHRMSemp = false;
                        selctedEmp.fk_hrmsEMPid = null;
                        selctedEmp.vchEmpCode = null;
                        //update other detail
                        selctedEmp.fk_designation = objupdate.fk_designation;
                        //update department and update department code also in ref_no
                        if (objupdate.fk_department != selctedEmp.fk_department)
                        {
                            string refNo = selctedEmp.vchRef_No;
                            //II/HR/OL/FO/24/007 index is 9 and 10 for department code replace it if dept is changed on update
                            int startIndex = 9;
                            int count = 2;
                            string replacementdept = selcetdDept.vchdepCode.ToString();
                            string new_RefNo = ReplaceSubstringAtIndex(refNo, startIndex, count, replacementdept);
                            selctedEmp.vchRef_No = new_RefNo;
                            selctedEmp.fk_department = objupdate.fk_department;
                        }                        
                        selctedEmp.fk_Masid = objupdate.fk_Masid;
                        selctedEmp.vchName = objupdate.vchName;
                        selctedEmp.vchFatherName = objupdate.vchFatherName;
                        selctedEmp.vchCity = objupdate.vchCity;
                        selctedEmp.vchState = objupdate.vchState;
                        selctedEmp.vchAddress = objupdate.vchAddress;
                        selctedEmp.dtApplication = objupdate.dtApplication;
                        selctedEmp.dtDOS = objupdate.dtDOS;
                        selctedEmp.dtDOE = objupdate.dtDOE;
                        selctedEmp.txtContent = objupdate.txtContent;
                        selctedEmp.vchUpdatedBy = Session["descript"].ToString();
                        selctedEmp.dtUpdated = DateTime.Now;
                        selctedEmp.vchUpdatedHost = Session["hostname"].ToString();
                        selctedEmp.vchUpdatedIp = Session["ipused"].ToString();
                        objentity.SaveChanges();
                        TempData["Success"] = "Letter updated successfully!";
                        return RedirectToAction("EmpIndexIntership");
                    }
                }
                else
                {
                    TempData["Error"] = "Selecetd letter detail not found please check again and try!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult EmpInternshipPrint(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedCertificate = (from e in objentity.tblIntershipDetail
                                               join m in objentity.tblIntershipMas on e.fk_Masid equals m.intid
                                               where e.intid == id
                                               select e).FirstOrDefault();
                    if (selectedCertificate == null)
                    {
                        ViewBag.Error = "Certificate detail not found please check it again!";
                        return View();
                    }
                    else
                    {
                        //get file and master id
                        string Report_Name = string.Empty;
                        // for admin female
                        if (selectedCertificate.vchGender == "1" && selectedCertificate.tblIntershipMas.vchMasName == "Admin internship for male candidate")
                        {
                            Report_Name = "IntershipAdmin_Male.rdl";
                        }
                        // admin male
                        if (selectedCertificate.vchGender == "2" && selectedCertificate.tblIntershipMas.vchMasName == "Admin internship for female candidate")
                        {
                            Report_Name = "IntershipAdmin_Female.rdl";
                        }
                        //for nursing male
                        if (selectedCertificate.vchGender == "1" && selectedCertificate.tblIntershipMas.vchMasName == "Nursing internship for male candidate")
                        {
                            Report_Name = "IntershipNursing_Male.rdl";
                        }
                        //for nursing female
                        if (selectedCertificate.vchGender == "2" && selectedCertificate.tblIntershipMas.vchMasName == "Nursing internship for female candidate")
                        {
                            Report_Name = "IntershipNursing_Female.rdl";
                        }
                        LocalReport lr = new LocalReport();
                        string filepath = String.Empty;
                        HttpClient _client = new HttpClient();                       
                        //Get dataset value and fields
                        var selectedobj = (from e in objentity.spEmpinternshipLetter(id) select e).ToList();
                        //get path                
                        filepath = Path.Combine(Server.MapPath("~/Content/Report"), (Report_Name));
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
                }
                else
                {
                    TempData["Error"] = "Offer letter id/detail not found please check it and try again!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }

        }

        //public ActionResult EmpPrintIntership(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        if (id != 0)
        //        {
        //            var selectedCertificate = (from e in objentity.tblIntershipDetail
        //                                       join m in objentity.tblIntershipMas on e.fk_Masid equals m.intid
        //                                       where e.intid == id
        //                                       select e).FirstOrDefault();
        //            if (selectedCertificate == null)
        //            {
        //                ViewBag.Error = "Certificate detail not found please check it again!";
        //                return View();
        //            }
        //            else
        //            {
        //                //get file and master id
        //                string Report_Name = string.Empty;
        //                // for admin female
        //                //string newid = id.ToString();
        //                LocalReport lr1 = new LocalReport();
        //                string filepath = String.Empty;
        //                HttpClient _client = new HttpClient();
        //                //string CertificateType = selectedCertificate.tblIntershipMas.vchType.ToString();
        //                //Get dataset value and fields
        //                var selectedobj = (from e in objentity.spGetEmpInterLetter(id) select e).ToList();
        //                //get path                
        //                filepath = Path.Combine(Server.MapPath("~/Content/Report"), "IntershipLetter.rdl");
        //                //open streams
        //                using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
        //                {
        //                    lr1.LoadReportDefinition(filestream);
        //                    lr1.DataSources.Add(new ReportDataSource(name: "DataSet1", selectedobj));
        //                    ReportParameter ptr = new ReportParameter("id", id.ToString());
        //                    lr1.SetParameters(ptr);
        //                    byte[] pdfData = lr1.Render("PDF");
        //                    return File(pdfData, contentType: "Application/pdf");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Offer letter id/detail not found please check it and try again!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        #endregion

        #endregion

        #region Appointment Master and Employee Appointment

        #region Appointment Master Letter

        public ActionResult IndexAppointMas()
        {
            if (Session["descript"] != null)
            {
                var selectedList = (from e in objentity.tblLetterAppointMas select e).ToList();
                if (selectedList.Count() != 0)
                {
                    ViewBag.Success = "Found " + selectedList.Count() + " Appointment Master";
                    return View(selectedList);
                }
                else
                {
                    ViewBag.Empty = "0 Record found in database";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult NewAppointMas()
        {
            if (Session["descript"] != null)
            {
                //for master type selection
                List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                },
                                             new SelectListItem
                                             {
                                                 Text="All",
                                                 Value="3"
                                             }
                };                          
                ViewBag.AppointType = new SelectList(Mastype, "Value", "Text");
                //for gender slecection
                List<SelectListItem> appointgenderList = new List<SelectListItem> {
                                             new SelectListItem
                                             {
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                              new SelectListItem
                                             {
                                                  Text="Female",
                                                  Value = "2"
                                             },
                                               new SelectListItem
                                             {
                                                  Text="All",
                                                  Value = "3"
                                             }
                };
                ViewBag.GenderList = new SelectList(appointgenderList, "Value", "Text");
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult NewAppointMas(tblLetterAppointMas objnew)
        {
            if (Session["descript"] != null)
            {
                if (objnew.vchForGender == "1")
                {
                    objnew.vchForGender = "Male";
                }
                else if(objnew.vchForGender=="2")
                {
                    objnew.vchForGender = "Female";
                }
                else if(objnew.vchForGender=="3")
                {
                    objnew.vchForGender = "All";
                }
                if (objnew.vchMasType == "1")
                {
                    objnew.vchMasType = "Admin";
                }
                else if (objnew.vchMasType == "2")
                {
                    objnew.vchMasType = "Nursing";
                }
                else if (objnew.vchMasType == "3")
                {
                    objnew.vchMasType = "All";
                }
                objnew.dtCreated = DateTime.Now;
                objnew.vchCreatedBy = Session["descript"].ToString();
                objnew.vchCreatedHost = Session["hostname"].ToString();
                objnew.vchCreatedIP = Session["ipused"].ToString();
                objnew.intcode = Convert.ToInt32(Session["id"].ToString());
                objnew.intyear = Convert.ToInt32(Session["yr"].ToString());
                objentity.tblLetterAppointMas.Add(objnew);
                objentity.SaveChanges();
                TempData["Success"] = "Master saved successfully";
                return RedirectToAction("NewAppointMas");
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult EditAppointMas(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedLetter = (from e in objentity.tblLetterAppointMas select e).FirstOrDefault();
                    if (selectedLetter != null)
                    {
                        //for selecetd vchGender 
                        if (selectedLetter.vchForGender == "Male")
                        {
                            selectedLetter.vchForGender = "1";
                        }
                        if (selectedLetter.vchForGender == "Female")
                        {
                            selectedLetter.vchForGender = "1";
                        }
                        if (selectedLetter.vchForGender == "All")
                        {
                            selectedLetter.vchForGender = "3";
                        }
                        //for selecetd Mas type
                        if (selectedLetter.vchMasType == "Admin")
                        {
                            selectedLetter.vchMasType = "1";
                        }
                        if (selectedLetter.vchMasType == "Nursing")
                        {
                            selectedLetter.vchMasType = "1";
                        }
                        if (selectedLetter.vchMasType == "All")
                        {
                            selectedLetter.vchMasType = "3";
                        }
                        //for master type selection
                        List<SelectListItem> AppointMastype = new List<SelectListItem> 
                        {
                              new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                },
                                             new SelectListItem
                                             {
                                                 Text="All",
                                                 Value="3"
                                             }
                        };
                        ViewBag.AppointMastype = new SelectList(AppointMastype, "Value", "Text");
                        //for gender slecection
                        List<SelectListItem> genderList = new List<SelectListItem> 
                        {
                             new SelectListItem
                                             {
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                              new SelectListItem
                                             {
                                                  Text="Female",
                                                  Value = "2"
                                             },
                                               new SelectListItem
                                             {
                                                  Text="All",
                                                  Value = "3"
                                             }
                        };
                        ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
                        return View(selectedLetter);
                    }
                    else
                    {
                        //for master type selection
                        List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                },
                                             new SelectListItem
                                             {
                                                 Text="All",
                                                 Value="3"
                                             }
                };
                        ViewBag.AppointType = new SelectList(Mastype, "Value", "Text");
                        //for gender slecection
                        List<SelectListItem> appointgenderList = new List<SelectListItem> {
                                             new SelectListItem
                                             {
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                              new SelectListItem
                                             {
                                                  Text="Female",
                                                  Value = "2"
                                             },
                                               new SelectListItem
                                             {
                                                  Text="All",
                                                  Value = "3"
                                             }
                };
                        ViewBag.GenderList = new SelectList(appointgenderList, "Value", "Text");
                        ViewBag.Error = "Selected appointment letter detail not found please check it again and try";
                        return View();
                    }
                }
                else
                {
                    //for master type selection
                    List<SelectListItem> Mastype = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Admin",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Nursing",
                                                 Value ="2"
                                                },
                                             new SelectListItem
                                             {
                                                 Text="All",
                                                 Value="3"
                                             }
                };
                    ViewBag.AppointType = new SelectList(Mastype, "Value", "Text");
                    //for gender slecection
                    List<SelectListItem> appointgenderList = new List<SelectListItem> {
                                             new SelectListItem
                                             {
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                              new SelectListItem
                                             {
                                                  Text="Female",
                                                  Value = "2"
                                             },
                                               new SelectListItem
                                             {
                                                  Text="All",
                                                  Value = "3"
                                             }
                };
                    ViewBag.GenderList = new SelectList(appointgenderList, "Value", "Text");
                    ViewBag.Error = "Appointment letter id should not be null or 0";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditAppointMas(tblLetterAppointMas objupdate)
        {
            if (Session["descript"] != null)
            {
                var selecetdLetter = (from e in objentity.tblLetterAppointMas where e.intid == objupdate.intid select e).FirstOrDefault();
                if (selecetdLetter != null)
                {
                    if (objupdate.vchForGender == "1")
                    {
                        selecetdLetter.vchForGender = "Male";
                    }
                    else if (objupdate.vchForGender == "2")
                    {
                        selecetdLetter.vchForGender = "Female";
                    }
                    else if (objupdate.vchForGender == "3")
                    {
                        selecetdLetter.vchForGender = "All";
                    }
                    if (objupdate.vchMasType == "1")
                    {
                        selecetdLetter.vchMasType = "Admin";
                    }
                    else if (objupdate.vchMasType == "2")
                    {
                        selecetdLetter.vchMasType = "Nursing";
                    }
                    else if (objupdate.vchMasType == "3")
                    {
                        selecetdLetter.vchMasType = "All";
                    }                  
                    selecetdLetter.vchMasName = objupdate.vchMasName;                    
                    selecetdLetter.vchMasHeading = objupdate.vchMasHeading;
                    selecetdLetter.vchMasLetterCode = objupdate.vchMasLetterCode;
                    selecetdLetter.TextMasContent = objupdate.TextMasContent;
                    selecetdLetter.vchUpdatedBy = Session["descript"].ToString();
                    selecetdLetter.dtUpdated = DateTime.Now;
                    selecetdLetter.vchUpdatedIp = Session["ipused"].ToString();
                    selecetdLetter.vchUpdatedHost = Session["hostname"].ToString();
                    objentity.SaveChanges();
                    TempData["Success"] = "Appointment letter updated successfully";
                    return RedirectToAction("IndexAppointMas");
                }
                else
                {
                    TempData["Error"] = "Selected letter detail not found, check it and try again";
                    return View();
                }
               
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ViewAppointMas(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedLetter = (from e in objentity.tblLetterAppointMas where e.intid == id select e).FirstOrDefault();
                    if (selectedLetter != null)
                    {
                        return View(selectedLetter);
                    }
                    else
                    {
                        TempData["Error"] = "Selected appointment letter detail not found, please check it and try again";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "Appointment letter id should not be null or 0";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Employee Appointment Letter

        public ActionResult IndexEmpAppoint()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedList = (from e in objentity.tblLetterAppointDetail where e.intcode==code select e).ToList();
                if (selectedList.Count() != 0)
                {
                    ViewBag.Success = "Found " + selectedList.Count() + " Appointment Letter";
                    return View(selectedList);
                }
                else
                {
                    ViewBag.Empty = "0 Letter found in database";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult NewEmpAppoint()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt16(Session["id"].ToString());
                //select employee code which not red flag or not active employee
                var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                         //join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                         where (e.bittempstatusactive == true || e.bitIsPartialAuthorised==true ) && e.bitstatusdeactive == false
                                         && e.BitIsRedFlagging == false
                                         && e.bitJoinLetter!=true && e.intcode == code
                                         select e).ToList();
                List<SelectListItem> allemp = new List<SelectListItem>();
                if (allEmployeeForExp != null)
                {
                    foreach (var emp in allEmployeeForExp)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = emp.intid.ToString(),
                            Text = emp.vchEmpFcode
                        };
                        allemp.Add(selectListItem);
                    }
                    ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = "Empty employee list",
                        Text = "Empty employee list"
                    };
                    allemp.Add(selectListItem);
                    ViewBag.Employees = new SelectList(empty, "Value", "Text");
                }
                //for department selection
                var selDept = (from e in objentity.tblDeptMas select e).ToList();
                if (selDept.Count() != 0)
                {
                    List<SelectListItem> desi = new List<SelectListItem>();
                    foreach (var dpt in selDept)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = dpt.intid.ToString(),
                            Text = dpt.vchdeptname
                        };
                        desi.Add(selectListItem);
                    }
                    ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = "Departments not found",
                        Text = "Departments not found"
                    };
                    ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                }
                //for designation selection
                var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                if (selectDesig.Count() != 0)
                {
                    List<SelectListItem> desi = new List<SelectListItem>();
                    foreach (var dess in selectDesig)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = dess.intid.ToString(),
                            Text = dess.vchdesignation
                        };
                        desi.Add(selectListItem);
                    }
                    ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = "Desigantions not found",
                        Text = "Designations not found"
                    };
                    ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                }
                //check is employee code is already used in experience or nor 
                //for sub type selection
                var selectMaster = (from e in objentity.tblLetterAppointMas select e).ToList();
                if (selectMaster.Count() != 0)
                {
                    List<SelectListItem> typeselction = new List<SelectListItem>();
                    foreach (var item2 in selectMaster)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = item2.intid.ToString(),
                            Text = item2.vchMasName.ToString()
                        };
                        typeselction.Add(selectListItem);
                    }
                    ViewBag.MasterType = new SelectList(typeselction, "Value", "Text");
                }
                else
                {
                    List<SelectListItem> empty = new List<SelectListItem>();
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Value = "Master is empty",
                        Text = "Master is empty"
                    };
                    empty.Add(selectListItem);
                    ViewBag.MasterType = new SelectList(empty, "Value", "Text");
                }
                //for gender slecection
                List<SelectListItem> appointgenderList = new List<SelectListItem> {
                                             new SelectListItem
                                             {
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                              new SelectListItem
                                             {
                                                  Text="Female",
                                                  Value = "2"
                                             }
                };
                ViewBag.GenderList = new SelectList(appointgenderList, "Value", "Text");
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult GetAppointhrmsid(string empid)
        {
            if (Session["descript"] != null)
            {
                int id = Convert.ToInt32(empid.ToString());
                //check letter is avilable for employee or not
                var checkletter = (from e in objentity.tblLetterAppointDetail where e.fk_hrmsEMPid == id select e).FirstOrDefault();
                if (checkletter == null)
                {
                    var selectedMas = (from e in objentity.tblEmpAssesmentMas
                                       join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                       where e.intid == id
                                       select new { e.vchName, e.vchEmpFcode, e.fk_intdeptid, e.fk_intdesiid, d.vchFatherName, d.vchpcity, d.vchpstate, d.vchpaddress,d.vchsex,e.intsalary }).FirstOrDefault();
                    if (selectedMas != null)
                    {
                        string selectedGender = string.Empty;
                        if (selectedMas.vchsex == "Male")
                        {
                            selectedGender = "1";
                        }
                        else
                        {
                            selectedGender = "2";
                        }
                        var data = new
                        {
                            vchname = selectedMas.vchName,
                            vchempcode = selectedMas.vchEmpFcode,
                            fathername = selectedMas.vchFatherName,
                            city = selectedMas.vchpcity,
                            state = selectedMas.vchpstate,
                            add = selectedMas.vchpaddress,
                            deptid = selectedMas.fk_intdeptid,
                            desiid = selectedMas.fk_intdesiid,
                            Gender = selectedGender,
                            Salary=selectedMas.intsalary
                        };
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var error = new
                        {
                            alertmessage = "Employee detail not found!"
                        };
                        return Json(error, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var error = new
                    {
                        alertmessage = "Selected employee letter already generated, please check in view all employee letter!"
                    };
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult GetMasAppointContent(string selectedOption)
        {
            if (Session["descript"] != null)
            {
                if (selectedOption != null)
                {
                    int id = Convert.ToInt32(selectedOption);
                    var selectedMas = (from e in objentity.tblLetterAppointMas where e.intid == id select e).FirstOrDefault();
                    if (selectedMas != null)
                    {
                        var data = new
                        {
                            Result = selectedMas.TextMasContent.ToString()
                        };
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var data = new
                        {
                            Result = "Master content not found!"
                        };
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var data = new
                    {
                        Result = "Master id shuld not be 0!"
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }            

        [HttpPost]
        public ActionResult NewEmpAppoint(tblLetterAppointDetail objnew)
        {
            if (Session["descript"] != null)
            {
                //get selecetd HRMS code
                tblEmpAssesmentMas objEmpMas = new tblEmpAssesmentMas();
                if (objnew.fk_hrmsEMPid != 0 || objnew.fk_hrmsEMPid != null)
                {
                    objEmpMas = (from e in objentity.tblEmpAssesmentMas where e.intid == objnew.fk_hrmsEMPid select e).FirstOrDefault();
                    if (objEmpMas != null)
                    {
                        objnew.vchEmpCode = objEmpMas.vchEmpFcode;
                    }
                }
                //create letter code 
                int code = Convert.ToInt32(Session["id"].ToString());
                string finalBranchCode = Session["branchCode"].ToString();
                string hrCharachter = "HR";
                int masid = Convert.ToInt32(objnew.fk_AppointMasid.ToString());
                var selectedMas = (from e in objentity.tblLetterAppointMas where e.intid == masid select e).FirstOrDefault();
                string certificateCode = selectedMas.vchMasLetterCode;
                var selectedDept = (from e in objentity.tblDeptMas where e.intid == objnew.fk_department select e).FirstOrDefault();
                string deptCode = selectedDept.vchdepCode;
                string CodeYear = DateTime.Now.ToString("yy");
                int year1 = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int certificateCurrentcount = 0;
                var getCode = (from e in objentity.tblLetterNumberMas where e.intcode == code && e.intYear==year1 select e).FirstOrDefault();
                if (getCode != null)
                {
                    //get current count number of certificate
                    certificateCurrentcount = Convert.ToInt16(getCode.intCurrent);
                    int newcode = certificateCurrentcount + 1;
                    int number = newcode;
                    int counter = 0;
                    string finalnumber = "";
                    //int fnumber = 0;
                    while (number > 0)
                    {
                        number = number / 10;
                        counter++;
                    }
                    if (counter == 1)
                    {
                        finalnumber = string.Concat("00" + newcode.ToString());
                    }
                    if (counter == 2)
                    {
                        finalnumber = string.Concat("0" + newcode.ToString());
                    }
                    if (counter == 3)
                    {
                        finalnumber = newcode.ToString();
                    }

                    //    //Format = Branch/HR/EC/EmpDpt/yr/series
                    string Ref_Code = finalBranchCode + "/" + hrCharachter + "/" + certificateCode + "/" + deptCode + "/" + CodeYear + "/" + finalnumber;
                    objnew.vchRef_No = Ref_Code;
                    //update current digit with new digit
                    getCode.intCurrent = newcode;
                    if (objnew.vchGender == "Male")
                    {
                        objnew.vchGender = "1";
                    }
                    if (objnew.vchGender == "Female")
                    {
                        objnew.vchGender = "2";
                    }
                    int yr = Convert.ToInt32(Session["yr"].ToString());
                    objnew.vchCreatedBy = Session["descript"].ToString();
                    objnew.dtCreated = DateTime.Now;
                    objnew.vchCreatedHost = Session["hostname"].ToString();
                    objnew.vchCreatedIP = Session["ipused"].ToString();
                    objnew.intcode = code;
                    objnew.intyear = yr;
                    objnew.vchCompany = Session["Compname"].ToString();
                    objentity.tblLetterAppointDetail.Add(objnew);
                    objentity.SaveChanges();
                    TempData["Success"] = "Letter saved successfully!";
                    return RedirectToAction("NewEmpAppoint");
                }
                else
                {
                    //select employee code which not red flag or not active employee
                    var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                             join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                             where e.bittempstatusactive == true && e.bitstatusdeactive == false &&
                                             e.bitIsPartialAuthorised == false && e.BitIsRedFlagging == false
                                             && e.intcode == code
                                             select e).ToList();
                    List<SelectListItem> allemp = new List<SelectListItem>();
                    if (allEmployeeForExp != null)
                    {
                        foreach (var emp in allEmployeeForExp)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = emp.intid.ToString(),
                                Text = emp.vchEmpFcode
                            };
                            allemp.Add(selectListItem);
                        }
                        ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                    }
                    //for department selection
                    var selDept = (from e in objentity.tblDeptMas select e).ToList();
                    if (selDept.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dpt in selDept)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dpt.intid.ToString(),
                                Text = dpt.vchdeptname
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Departments not found",
                            Text = "Departments not found"
                        };
                        ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                    }
                    //for designation selection
                    var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                    if (selectDesig.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dess in selectDesig)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dess.intid.ToString(),
                                Text = dess.vchdesignation
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Desigantions not found",
                            Text = "Designations not found"
                        };
                        ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                    }

                    //check is employee code is already used in experience or nor 
                    // for sub type selection
                    var selectMaster = (from e in objentity.tblLetterAppointMas select e).ToList();
                    List<SelectListItem> typeselction = new List<SelectListItem>();
                    foreach (var item2 in selectMaster)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = item2.intid.ToString(),
                            Text = item2.vchMasName.ToString()
                        };
                        typeselction.Add(selectListItem);
                    }
                    ViewBag.OfferType = new SelectList(typeselction, "Value", "Text");
                    //for gender slecection
                    List<SelectListItem> appointgenderList = new List<SelectListItem> {
                                             new SelectListItem
                                             {
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                              new SelectListItem
                                             {
                                                  Text="Female",
                                                  Value = "2"
                                             }
                };
                    ViewBag.GenderList = new SelectList(appointgenderList, "Value", "Text");
                    ViewBag.Error = "Letter code series not found please contact to administrator!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }           
        
        public ActionResult EditEmpAppoint(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt16(Session["id"].ToString());
                if (id != 0)
                {
                    var selectedLetter = (from e in objentity.tblLetterAppointDetail where e.intid == id select e).FirstOrDefault();
                    if (selectedLetter != null)
                    {
                        //For selected gender
                        if (selectedLetter.vchGender == "Male")
                        {
                            selectedLetter.vchGender = "1";
                        }
                        if (selectedLetter.vchGender == "Female")
                        {
                            selectedLetter.vchGender = "2";
                        }
                        //select employee code which not red flag or not active employee
                        var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                                 join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                                 where e.bittempstatusactive == true && e.bitstatusdeactive == false &&
                                                 e.bitIsPartialAuthorised == false && e.BitIsRedFlagging == false
                                                 && e.intcode == code
                                                 select e).ToList();
                        List<SelectListItem> allemp = new List<SelectListItem>();
                        if (allEmployeeForExp != null)
                        {
                            foreach (var emp in allEmployeeForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allemp.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                        }
                        //for department selection
                        var selDept = (from e in objentity.tblDeptMas select e).ToList();
                        if (selDept.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dpt in selDept)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dpt.intid.ToString(),
                                    Text = dpt.vchdeptname
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Departments not found",
                                Text = "Departments not found"
                            };
                            ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                        }
                        //for designation selection
                        var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                        if (selectDesig.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dess in selectDesig)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dess.intid.ToString(),
                                    Text = dess.vchdesignation
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Desigantions not found",
                                Text = "Designations not found"
                            };
                            ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                        }

                        //check is employee code is already used in experience or nor 
                        // for sub type selection
                        var selectMaster = (from e in objentity.tblLetterAppointMas select e).ToList();
                        List<SelectListItem> typeselction = new List<SelectListItem>();
                        foreach (var item2 in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item2.intid.ToString(),
                                Text = item2.vchMasName.ToString()
                            };
                            typeselction.Add(selectListItem);
                        }
                        ViewBag.MasterType = new SelectList(typeselction, "Value", "Text");
                        //for gender slecection
                        List<SelectListItem> appointgenderList = new List<SelectListItem> {
                                             new SelectListItem
                                             {
                                                  Text="Male",
                                                  Value = "1"
                                             },
                                              new SelectListItem
                                             {
                                                  Text="Female",
                                                  Value = "2"
                                             }
                };
                        ViewBag.GenderList = new SelectList(appointgenderList, "Value", "Text");
                        return View(selectedLetter);
                    }
                    else
                    {
                        //select employee code which not red flag or not active employee
                        var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                                 join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                                 where e.bittempstatusactive == true && e.bitstatusdeactive == false &&
                                                 e.bitIsPartialAuthorised == false && e.BitIsRedFlagging == false
                                                 && e.intcode == code
                                                 select e).ToList();
                        List<SelectListItem> allemp = new List<SelectListItem>();
                        if (allEmployeeForExp != null)
                        {
                            foreach (var emp in allEmployeeForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allemp.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                        }
                        //for department selection
                        var selDept = (from e in objentity.tblDeptMas select e).ToList();
                        if (selDept.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dpt in selDept)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dpt.intid.ToString(),
                                    Text = dpt.vchdeptname
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Departments not found",
                                Text = "Departments not found"
                            };
                            ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                        }
                        //for designation selection
                        var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                        if (selectDesig.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dess in selectDesig)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dess.intid.ToString(),
                                    Text = dess.vchdesignation
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Desigantions not found",
                                Text = "Designations not found"
                            };
                            ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                        }

                        //check is employee code is already used in experience or nor 
                        // for sub type selection
                        var selectMaster = (from e in objentity.tblLetterAppointMas select e).ToList();
                        List<SelectListItem> typeselction = new List<SelectListItem>();
                        foreach (var item2 in selectMaster)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = item2.intid.ToString(),
                                Text = item2.vchMasName.ToString()
                            };
                            typeselction.Add(selectListItem);
                        }
                        ViewBag.MasterType = new SelectList(typeselction, "Value", "Text");
                        ViewBag.Error = "Selected letter detail not found please check and try again!";
                        return View();
                    }
                }
                else
                {
                    //select employee code which not red flag or not active employee
                    var allEmployeeForExp = (from e in objentity.tblEmpAssesmentMas
                                             join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid
                                             where e.bittempstatusactive == true && e.bitstatusdeactive == false &&
                                             e.bitIsPartialAuthorised == false && e.BitIsRedFlagging == false
                                             && e.intcode == code
                                             select e).ToList();
                    List<SelectListItem> allemp = new List<SelectListItem>();
                    if (allEmployeeForExp != null)
                    {
                        foreach (var emp in allEmployeeForExp)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = emp.intid.ToString(),
                                Text = emp.vchEmpFcode
                            };
                            allemp.Add(selectListItem);
                        }
                        ViewBag.Employees = new SelectList(allemp, "Value", "Text");
                    }
                    //for department selection
                    var selDept = (from e in objentity.tblDeptMas select e).ToList();
                    if (selDept.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dpt in selDept)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dpt.intid.ToString(),
                                Text = dpt.vchdeptname
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Departments not found",
                            Text = "Departments not found"
                        };
                        ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                    }
                    //for designation selection
                    var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                    if (selectDesig.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dess in selectDesig)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dess.intid.ToString(),
                                Text = dess.vchdesignation
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Desigantions not found",
                            Text = "Designations not found"
                        };
                        ViewBag.SelectDesi = new SelectList(empty, "Value", "Text");
                    }

                    //check is employee code is already used in experience or nor 
                    // for sub type selection
                    var selectMaster = (from e in objentity.tblLetterAppointMas select e).ToList();
                    List<SelectListItem> typeselction = new List<SelectListItem>();
                    foreach (var item2 in selectMaster)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = item2.intid.ToString(),
                            Text = item2.vchMasName.ToString()
                        };
                        typeselction.Add(selectListItem);
                    }
                    ViewBag.MasterType = new SelectList(typeselction, "Value", "Text");
                    ViewBag.Error = "Letter id should not be null or 0";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditEmpAppoint(tblLetterAppointDetail objupdate)
        {
            if (Session["descript"] != null)
            {
                tblEmpAssesmentMas oldHrmsEmp = new tblEmpAssesmentMas();
                var selctedEmp = (from e in objentity.tblLetterAppointDetail where e.intid == objupdate.intid select e).FirstOrDefault();
                var selcetdDept = (from e in objentity.tblDeptMas where e.intid == objupdate.fk_department select e).FirstOrDefault();
                if (selctedEmp != null)
                {
                    if (objupdate.bitIsHRMSemp == true)
                    {
                        selctedEmp.bitIsHRMSemp = true;
                        //check if HRMS employee is changed
                        var selectedHrmSEMP = (from e in objentity.tblEmpAssesmentMas where e.intid == objupdate.fk_hrmsEMPid select e).FirstOrDefault();
                        if (selectedHrmSEMP != null)
                        {
                            //check employee updated?
                            if (selectedHrmSEMP.intid != selctedEmp.fk_hrmsEMPid)
                            {
                                //now change old hrms emp status
                                oldHrmsEmp = (from e in objentity.tblEmpAssesmentMas where e.intid == selctedEmp.fk_hrmsEMPid select e).FirstOrDefault();
                                oldHrmsEmp.bitExpLetter = false;
                                selctedEmp.vchEmpCode = selectedHrmSEMP.vchEmpFcode;
                                selctedEmp.fk_hrmsEMPid = selectedHrmSEMP.intid;
                                selectedHrmSEMP.bitExpLetter = true;
                            }
                        }                       
                        selctedEmp.fk_AppointMasid = objupdate.fk_AppointMasid;
                        selctedEmp.vchName = objupdate.vchName;
                        selctedEmp.vchFatherName = objupdate.vchFatherName;
                        selctedEmp.vchCity = objupdate.vchCity;
                        selctedEmp.vchState = objupdate.vchState;
                        selctedEmp.vchAddress = objupdate.vchAddress;

                        //dept id not changed
                        if (objupdate.fk_department != selctedEmp.fk_department)
                        {
                            string refNo = selctedEmp.vchRef_No;
                            //II/HR/OL/FO/24/007 index is 9 and 10 for department code replace it if dept is changed on update
                            int startIndex = 9;
                            int count = 2;
                            string replacementdept = selcetdDept.vchdepCode.ToString();
                            string new_RefNo = ReplaceSubstringAtIndex(refNo, startIndex, count, replacementdept);
                            selctedEmp.vchRef_No = new_RefNo;
                            selctedEmp.fk_department = objupdate.fk_department;
                        }
                        selctedEmp.fk_designation = objupdate.fk_designation;
                        selctedEmp.dtApplication = objupdate.dtApplication;
                        selctedEmp.dtDOJ = objupdate.dtDOJ;                       
                        selctedEmp.txtContent = objupdate.txtContent;
                        selctedEmp.vchUpdatedBy = Session["descript"].ToString();
                        selctedEmp.dtUpdated = DateTime.Now;
                        selctedEmp.vchUpdatedHost = Session["hostname"].ToString();
                        selctedEmp.vchUpdatedIp = Session["ipused"].ToString();
                        objentity.SaveChanges();
                        TempData["Success"] = "Letter updated successfully!";
                        return RedirectToAction("IndexEmpAppoint");
                    }
                    else
                    {
                        //set null or false hrms employee detail
                        selctedEmp.bitIsHRMSemp = false;
                        selctedEmp.fk_hrmsEMPid = null;
                        selctedEmp.vchEmpCode = null;
                        //update other detail
                        selctedEmp.fk_designation = objupdate.fk_designation;
                        //update department and update department code also in ref_no
                        if (objupdate.fk_department != selctedEmp.fk_department)
                        {
                            string refNo = selctedEmp.vchRef_No;
                            //II/HR/OL/FO/24/007 index is 9 and 10 for department code replace it if dept is changed on update
                            int startIndex = 9;
                            int count = 2;
                            string replacementdept = selcetdDept.vchdepCode.ToString();
                            string new_RefNo = ReplaceSubstringAtIndex(refNo, startIndex, count, replacementdept);
                            selctedEmp.vchRef_No = new_RefNo;
                            selctedEmp.fk_department = objupdate.fk_department;
                        }
                        selctedEmp.fk_AppointMasid = objupdate.fk_AppointMasid;
                        selctedEmp.vchName = objupdate.vchName;
                        selctedEmp.vchFatherName = objupdate.vchFatherName;
                        selctedEmp.vchCity = objupdate.vchCity;
                        selctedEmp.vchState = objupdate.vchState;
                        selctedEmp.vchAddress = objupdate.vchAddress;
                        selctedEmp.dtApplication = objupdate.dtApplication;
                        selctedEmp.dtDOJ = objupdate.dtDOJ;
                        selctedEmp.txtContent = objupdate.txtContent;
                        selctedEmp.vchUpdatedBy = Session["descript"].ToString();
                        selctedEmp.dtUpdated = DateTime.Now;
                        selctedEmp.vchUpdatedHost = Session["hostname"].ToString();
                        selctedEmp.vchUpdatedIp = Session["ipused"].ToString();
                        objentity.SaveChanges();
                        TempData["Success"] = "Letter updated successfully!";
                        return RedirectToAction("IndexEmpAppoint");
                    }
                }
                else
                {
                    TempData["Error"] = "Selecetd letter detail not found please check again and try!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PrintEmpAppoint(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedCertificate = (from e in objentity.tblLetterAppointDetail
                                               join m in objentity.tblLetterAppointMas on e.fk_AppointMasid equals m.intid
                                               where e.intid == id
                                               select e).FirstOrDefault();
                    if (selectedCertificate == null)
                    {
                        ViewBag.Error = "Certificate detail not found please check it again!";
                        return View();
                    }
                    else
                    {
                        string Report_Name = string.Empty;
                        LocalReport lr = new LocalReport();
                        string filepath = String.Empty;
                        HttpClient _client = new HttpClient();
                        //Get dataset value and fields
                        var selectedobj = (from e in objentity.spEmpAppointLetter(id) select e).ToList();
                        //get path                
                        filepath = Path.Combine(Server.MapPath("~/Content/Report"), ("Appointment.rdl"));
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
                }
                else
                {
                    TempData["Error"] = "Offer letter id/detail not found please check it and try again!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #endregion

        #region Ref_No  department code replacement function for all certificate using string replacement
        private string ReplaceSubstringAtIndex(string input, int startIndex, int lengthToReplace, string replacementString)
        {
            if (startIndex < 0 || startIndex + lengthToReplace > input.Length)
            {
                throw new ArgumentException("Invalid indices");
            }

            string part1 = input.Substring(0, startIndex);
            string part2 = replacementString;
            string part3 = input.Substring(startIndex + lengthToReplace);

            return part1 + part2 + part3;
        }
        #endregion       

        #region Current Experience Letter Create, Update, Print

        public ActionResult ViewCurrentEmp()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var GetList = (from e in objentity.tblEmpAssesmentMas where (e.bittempstatusactive == true || e.bitIsPartialAuthorised == true) && e.intcode == code select e).ToList();
                if (GetList.Count() != 0)
                {
                    return View(GetList);
                }
                else
                {
                    ViewBag.Error = "0 record found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        public ActionResult CreateCurrExp(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedEmp = (from e in objentity.tblEmpAssesmentMas
                                       join d in objentity.tblEmpDetails on e.intid equals d.fk_intempid where e.intid == id select e).FirstOrDefault();
                    if (selectedEmp != null)
                    {                    
                        //get employee all detail from detail table
                        var detailemp = (from e in objentity.tblEmpDetails where e.fk_intempid == id select e).FirstOrDefault();
                        //for department selection
                        var selDept = (from e in objentity.tblDeptMas select e).ToList();
                        if (selDept.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dpt in selDept)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dpt.intid.ToString(),
                                    Text = dpt.vchdeptname
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Departments not found",
                                Text = "Departments not found"
                            };
                            ViewBag.SelectDpt = new SelectList(empty, "Value", "Text");
                        }
                        //get state
                        var allState = (from e in objentity.tblState select e).ToList();
                        List<SelectListItem> newlist = new List<SelectListItem>();
                        newlist.Add(new SelectListItem { Text = "Select state", Value = "0" });
                        foreach (var st in allState)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = st.intid.ToString(),
                                Text = st.vchState
                            };
                            newlist.Add(selectListItem);
                        }
                        ViewBag.AllState = new SelectList(newlist, "Value", "Text");
                        //for gender slecection
                        List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                }
                         };
                        ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                        //for designation selection
                        var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                        if (selectDesig.Count() != 0)
                        {
                            List<SelectListItem> desi = new List<SelectListItem>();
                            foreach (var dess in selectDesig)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = dess.intid.ToString(),
                                    Text = dess.vchdesignation
                                };
                                desi.Add(selectListItem);
                            }
                            ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                        }
                        else
                        {
                            List<SelectListItem> empty = new List<SelectListItem>();
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = "Desigantion not found",
                                Text = "Designation not found"
                            };
                            ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                        }
                        tblCurrentExpDetail objtable = new tblCurrentExpDetail();
                        objtable.bitIsDetailAvilable = true;
                        objtable.vchName = selectedEmp.vchName;
                        objtable.vchFatherName = detailemp.vchFatherName;
                        objtable.vchCity = detailemp.vchpcity;
                        objtable.vchstate = detailemp.vchpstate;
                        objtable.vchAddress = detailemp.vchpaddress;                        
                        objtable.fk_MasId = selectedEmp.intid;
                        objtable.fk_deptId = Convert.ToInt32(selectedEmp.fk_intdeptid);                        
                        objtable.fk_desiId = Convert.ToInt32(selectedEmp.fk_intdesiid);                    
                        if (detailemp.vchsex == "Male")
                        {
                            objtable.vchGender = "1";
                        }
                        else
                        {
                            objtable.vchGender = "2";
                        }                       
                        objtable.dtdoj = Convert.ToDateTime(selectedEmp.dtDOJ);
                        return View(objtable);
                    }
                    else
                    {
                        //get emp if payroll emp
                        var selectedEmpMas = (from e in objentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                        if (selectedEmpMas != null)
                        {
                            //for department selection
                            var selDept = (from e in objentity.tblDeptMas select e).ToList();
                            if (selDept.Count() != 0)
                            {
                                List<SelectListItem> desi = new List<SelectListItem>();
                                foreach (var dpt in selDept)
                                {
                                    SelectListItem selectListItem = new SelectListItem
                                    {
                                        Value = dpt.intid.ToString(),
                                        Text = dpt.vchdeptname
                                    };
                                    desi.Add(selectListItem);
                                }
                                ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                            }
                            else
                            {
                                List<SelectListItem> empty = new List<SelectListItem>();
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = "Departments not found",
                                    Text = "Departments not found"
                                };
                                ViewBag.SelectDpt = new SelectList(empty, "Value", "Text");
                            }
                            //get state
                            var allState = (from e in objentity.tblState select e).ToList();
                            List<SelectListItem> newlist = new List<SelectListItem>();
                            newlist.Add(new SelectListItem { Text = "Select state", Value = "0" });
                            foreach (var st in allState)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = st.intid.ToString(),
                                    Text = st.vchState
                                };
                                newlist.Add(selectListItem);
                            }
                            ViewBag.AllState = new SelectList(newlist, "Text", "Value");
                            //for gender slecection
                            List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                }
                         };
                            ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                            //for designation selection
                            var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                            if (selectDesig.Count() != 0)
                            {
                                List<SelectListItem> desi = new List<SelectListItem>();
                                foreach (var dess in selectDesig)
                                {
                                    SelectListItem selectListItem = new SelectListItem
                                    {
                                        Value = dess.intid.ToString(),
                                        Text = dess.vchdesignation
                                    };
                                    desi.Add(selectListItem);
                                }
                                ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                            }
                            else
                            {
                                List<SelectListItem> empty = new List<SelectListItem>();
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = "Desigantion not found",
                                    Text = "Designation not found"
                                };
                                ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                            }
                            tblCurrentExpDetail objtable = new tblCurrentExpDetail();
                            objtable.bitIsDetailAvilable = false;
                            if (selectedEmpMas.vchgender == "Male")
                            {
                                objtable.vchGender = "1";
                            }
                            else
                            {
                                objtable.vchGender = "2";
                            }
                            objtable.fk_MasId = selectedEmpMas.intid;
                            objtable.vchName = selectedEmpMas.vchName;
                            objtable.dtdoj = Convert.ToDateTime(selectedEmpMas.dtDOJ);
                            objtable.fk_deptId = Convert.ToInt32(selectedEmpMas.fk_intdeptid);
                            objtable.fk_desiId = Convert.ToInt32(selectedEmpMas.fk_intdesiid);
                            return View(objtable);
                        }
                        else
                        {
                            return View();
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
                return RedirectToAction("_SessionError", "Home");
            }
        }

        [HttpPost]
        public ActionResult CreateCurrExp(tblCurrentExpDetail newobj, FormCollection fc)
        {
            if (Session["descript"] != null)
            {
                //select mas data for status update current experience
                var masEmp = (from e in objentity.tblEmpAssesmentMas where e.intid == newobj.fk_MasId select e).FirstOrDefault();
                int cityid = Convert.ToInt32(fc.Get("selectedCity"));
                //get state
                tblCity objcity = new tblCity();
                tblState objState = new tblState();
                if (masEmp != null)
                {
                    if (newobj.bitIsDetailAvilable != true)
                    {
                        objcity = (from e in objentity.tblCity where e.intid == cityid select e).FirstOrDefault();
                        int stateid = objcity.fk_stateid;
                        objState = (from e in objentity.tblState where e.intid == stateid select e).FirstOrDefault();
                        newobj.vchstate = objState.vchState;
                        newobj.vchCity = objcity.vchCityName;
                        newobj.fk_CityId = objcity.intid;
                        newobj.fkStateId = objcity.fk_stateid;
                    }
                    else
                    {
                        string EnteredCity = newobj.vchCity;
                        var selecetdCity = (from e in objentity.tblCity where e.vchCityName == EnteredCity select e).FirstOrDefault();
                        if (selecetdCity != null)
                        {
                            objState = (from e in objentity.tblState where e.intid == selecetdCity.fk_stateid select e).FirstOrDefault();
                            int stateid = selecetdCity.fk_stateid;
                            objState = (from e in objentity.tblState where e.intid == stateid select e).FirstOrDefault();
                            //name
                            newobj.vchstate = objState.vchState;
                            newobj.vchCity = selecetdCity.vchCityName;
                            //id's
                            newobj.fk_CityId = selecetdCity.intid;
                            newobj.fkStateId = selecetdCity.fk_stateid;
                            newobj.bitIsDetailUpdated = true;
                        }
                        else
                        {                            
                            //only name when city table data does not match leave city name and state as entered                           
                        }
                    }
                    newobj.vchCreatedBy = Session["descript"].ToString();
                    newobj.vchCreatedHost = Session["hostname"].ToString();
                    newobj.vchCratedIp = Session["ipused"].ToString();
                    newobj.vchBranch = Session["Compname"].ToString();
                    newobj.intCode = Convert.ToInt32(Session["id"].ToString());
                    newobj.DtCeated = DateTime.Now;
                    //from master entry
                    newobj.fk_deptId = Convert.ToInt32(masEmp.fk_intdeptid);
                    newobj.fk_desiId = Convert.ToInt32(masEmp.fk_intdesiid);
                    if (masEmp.vchEmpFcode != null)
                    {
                        newobj.vchEmpFcode = masEmp.vchEmpFcode;
                    }                   
                    //master update 
                    masEmp.bitIsCurrentExp = true;
                    //Generate reference number
                    //Proposed Codes:
                    //IIH/HR/EC/BB2301(EC - Experience Certificate, BB - Blood Bank)
                    //IIH/HR/EC/PL2301
                    //The Certificates will be issued on the letter Heads of the Company which are in the custody of the HODs / HRs only.
                    string hrCharachter = "HR";
                    string dptCode = string.Empty;
                    string certificateCode = string.Empty;
                    int certificateCurrentcount = 0;
                    tblDesignationMas objdesi = new tblDesignationMas();
                    int desiID = Convert.ToInt32(newobj.fk_desiId);
                    objdesi = (from e in objentity.tblDesignationMas where e.intid == desiID select e).FirstOrDefault();
                    int deptID = Convert.ToInt32(newobj.fk_deptId);
                    var getDeptCode = (from e in objentity.tblDeptMas where e.intid == deptID select e).FirstOrDefault();
                    if (getDeptCode != null)
                    {
                        dptCode = getDeptCode.vchdepCode.ToString();
                    }
                    else
                    {
                        TempData["Error"] = "Department code should not null contact to administrator!";
                        return RedirectToAction("ViewCurrentEmp", "Template");
                    }
                    //Get Certificate code from master
                    certificateCode = "CE";
                    int year1 = 2024;
                    int code = Convert.ToInt32(Session["id"].ToString());
                    //get branch wise cetrificate count
                    var getCode = (from e in objentity.tblLetterNumberMas where e.intcode == code && e.intYear == year1 select e).FirstOrDefault();
                    if (getCode != null)
                    {
                        //get current count number of certificate
                        certificateCurrentcount = Convert.ToInt16(getCode.intCurrent);
                        int newcode = certificateCurrentcount + 1;
                        int number = newcode;
                        int counter = 0;
                        string finalnumber = "";
                        //int fnumber = 0;
                        while (number > 0)
                        {
                            number = number / 10;
                            counter++;
                        }
                        if (counter == 1)
                        {
                            finalnumber = string.Concat("00" + newcode.ToString());
                        }
                        if (counter == 2)
                        {
                            finalnumber = string.Concat("0" + newcode.ToString());
                        }
                        if (counter == 3)
                        {
                            finalnumber = newcode.ToString();
                        }
                        //Set Branch Code
                        string finalBranchCode = Session["branchCode"].ToString();
                        DateTime aajkidate = DateTime.Now;
                        string year = aajkidate.ToString("yy");
                        //Format = Branch/HR/EC/EmpDpt/yr/series
                        string Ref_Code = finalBranchCode + "/" + hrCharachter + "/" + certificateCode + "/" + dptCode + "/" + year + "/" + finalnumber;
                        newobj.vchRefNo = Ref_Code.ToString();
                        try
                        {
                            objentity.tblCurrentExpDetail.Add(newobj);
                            objentity.SaveChanges();

                            TempData["Success"] = "Certificate generated successfully!";
                            return RedirectToAction("ViewCurrentEmp", "Template");
                        }
                        catch(DbEntityValidationException ex)
                        {
                            // Retrieve the validation errors
                            foreach (var validationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    TempData["Error"] = ("Error : ",  validationError.ErrorMessage + "Update aadhaar number from active/partial authorised employee view");

                                }
                            }
                            return RedirectToAction("ViewCurrentEmp");
                        }
                    }

                }
                TempData["Error"] = "Selected employee detail not found contact to administrator!";
                return RedirectToAction("CreateCurrExp", new { id = newobj.fk_MasId });
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        [HttpGet]
        public ActionResult GetCity(string state_id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int stateid = Convert.ToInt32(state_id);
            List<tblCity> citylist = new List<tblCity>();
            citylist = (from e in objentity.tblCity where e.fk_stateid == stateid select e).ToList();
            var result = (from d in citylist
                          select new
                          {
                              id = d.intid,
                              vchCity = d.vchCityName
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult PrintCurrExp(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedCertificate = (from e in objentity.tblCurrentExpDetail where e.fk_MasId == id
                                               select e).FirstOrDefault();
                    if (selectedCertificate == null)
                    {
                        ViewBag.Error = "Certificate detail not found please check it again!";
                        return RedirectToAction("ViewCurrentEmp","Template");
                    }
                    else
                    {
                        string Report_Name = string.Empty;
                        LocalReport lr = new LocalReport();
                        string filepath = String.Empty;
                        HttpClient _client = new HttpClient();
                        //Get dataset value and fields
                        var selectedobj = (from e in objentity.spCurrentExpCertificate(id) select e).ToList();
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
                }
                else
                {
                    TempData["Error"] = "Offer letter id/detail not found please check it and try again!";
                    return RedirectToAction("ViewCurrentEmp","Template");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult EditCurrentExp(int id)
        {
            if (Session["descript"] != null)
            {
                tblCurrentExpDetail Selecetd = (from e in objentity.tblCurrentExpDetail where e.fk_MasId == id select e).FirstOrDefault();
                if (Selecetd != null)
                {
                    //for department selection
                    var selDept = (from e in objentity.tblDeptMas select e).ToList();
                    if (selDept.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dpt in selDept)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dpt.intid.ToString(),
                                Text = dpt.vchdeptname
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDpt = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Departments not found",
                            Text = "Departments not found"
                        };
                        ViewBag.SelectDpt = new SelectList(empty, "Value", "Text");
                    }
                    //get state
                    var allState = (from e in objentity.tblState select e).ToList();
                    List<SelectListItem> newlist = new List<SelectListItem>();                   
                    foreach (var st in allState)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = st.intid.ToString(),
                            Text = st.vchState,
                            Selected=Convert.ToBoolean(Selecetd.fkStateId)
                        };
                        newlist.Add(selectListItem);
                    }
                    ViewBag.AllState = new SelectList(newlist, "Text", "Value");
                    //for gender slecection
                    List<SelectListItem> offergenderList = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Male",
                                                  Value = "1"
                                                  //Selected=Convert.ToBoolean(Selecetd.vchGender)
                                                 },
                                             new SelectListItem{
                                                 Text="Female",
                                                 Value ="2"
                                                 //Selected=Convert.ToBoolean(Selecetd.vchGender)
                                                }
                         };
                    ViewBag.GenderList = new SelectList(offergenderList, "Value", "Text");
                    //for designation selection
                    var selectDesig = (from e in objentity.tblDesignationMas select e).ToList();
                    if (selectDesig.Count() != 0)
                    {
                        List<SelectListItem> desi = new List<SelectListItem>();
                        foreach (var dess in selectDesig)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = dess.intid.ToString(),
                                Text = dess.vchdesignation
                            };
                            desi.Add(selectListItem);
                        }
                        ViewBag.SelectDesi = new SelectList(desi, "Value", "Text");
                    }
                    else
                    {
                        List<SelectListItem> empty = new List<SelectListItem>();
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = "Desigantion not found",
                            Text = "Designation not found"
                        };
                        ViewBag.Desigations = new SelectList(empty, "Value", "Text");
                    }
                    return View(Selecetd);
                }
                else
                {
                    TempData["Error"] = "Certificate detail not found please chect it again or contact to admin";
                    return RedirectToAction("", "");
                }
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditCurrentExp(tblCurrentExpDetail objupdate, FormCollection fc)
        {
            if (Session["descript"] != null)
            {
                var selectedECertificate = (from e in objentity.tblCurrentExpDetail where e.fk_MasId == objupdate.fk_MasId select e).FirstOrDefault();
                if (selectedECertificate != null)
                {
                    //update state id
                    if (selectedECertificate.fkStateId != objupdate.fkStateId)
                    {
                        var getStateName = (from e in objentity.tblState where e.intid == objupdate.fkStateId select e).FirstOrDefault();
                        selectedECertificate.fkStateId = objupdate.fkStateId;
                        selectedECertificate.vchstate = getStateName.vchState;
                    }
                    //update city
                    int cityid = Convert.ToInt32(fc.Get("selectedCity"));
                    if (cityid != 0)
                    {
                        var getcity = (from e in objentity.tblCity where e.intid == cityid select e).FirstOrDefault();
                        selectedECertificate.fk_CityId = cityid;
                        selectedECertificate.vchCity = getcity.vchCityName;
                    }
                    //update other detail father name, address
                    selectedECertificate.vchFatherName = objupdate.vchFatherName;
                    selectedECertificate.vchAddress = objupdate.vchAddress;
                    selectedECertificate.vchUpdatedBy = Session["descript"].ToString();
                    selectedECertificate.vchUpdatedHost = Session["hostname"].ToString();
                    selectedECertificate.vchUpdatedIP = Session["ipused"].ToString();
                    objentity.SaveChanges();
                    TempData["Success"] = "Certifictae updated successfully!";
                    return RedirectToAction("ViewCurrentEmp", "Template");
                }
                else
                {
                    TempData["Success"] = "Selected certificate detail not found please check it again or contact to administration!";
                    return RedirectToAction("EditCurrentExp", "Template", new { id = objupdate.fk_MasId });
                }
            }
            else
            {
                return View();
            }
        }
        #endregion

        //Old code for references 

        //#region Employee Experience Letter

        //public ActionResult ExpLetterMale()
        //{
        //    if (Session["descript"] != null)
        //    {
        //        TemplateMasDetailViewModel objmodel = new TemplateMasDetailViewModel();
        //        var selectedMas = (from e in objentity.tblTemplateMas where e.vchGender == "Male" && e.vchType == "Experience" select e).FirstOrDefault();
        //        if (selectedMas != null)
        //        {
        //            objmodel.fk_letterid = selectedMas.intid;
        //            objmodel.LetterHead = selectedMas.vchTemplateHeading;
        //            objmodel.LetterContent = selectedMas.txtLetterContent;
        //            objmodel.fk_letterid = selectedMas.intid;
        //            //Select All Department For selection department
        //            var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //            if (selectedDept.Count() != 0)
        //            {
        //                List<SelectListItem> newlist = new List<SelectListItem>();
        //                foreach (var dpt in selectedDept)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem
        //                    {
        //                        Value = dpt.intid.ToString(),
        //                        Text = dpt.vchdeptname
        //                    };
        //                    newlist.Add(selectListItem);
        //                }
        //                ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //            }
        //            return View(objmodel);
        //        }
        //        else
        //        {
        //            //Proposed Codes:
        //            //IIH/HR/EC/BB2301(EC-Experience Certificate, BB-Blood Bank)
        //            //IIH/HR/EC/PL2301
        //            //The Certificates will be issued on the letter Heads of the Company which are in the custody of the HODs/HRs only.

        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}


        //public ActionResult _PartialContent(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        var selectedMas = (from e in objentity.tblExperienceMas where e.intid == id select e).FirstOrDefault();
        //        if (selectedMas != null)
        //        {                    
        //            return View(selectedMas);
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Master deatil not found!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        TempData["Error"] = "Error master letter content not found!";
        //        return View();
        //    }
        //}

        //[HttpPost]
        //public ActionResult ExpLetterMale(TemplateMasDetailViewModel objnewmodel)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        //Proposed Codes:
        //        //IIH/HR/EC/BB2301(EC - Experience Certificate, BB - Blood Bank)
        //        //IIH/HR/EC/PL2301
        //        //The Certificates will be issued on the letter Heads of the Company which are in the custody of the HODs / HRs only.
        //        string hrCharachter = "HR";
        //        string dptCode = string.Empty;
        //        string certificateCode = string.Empty;
        //        int certificateCurrentcount = 0;
        //        int deptID = Convert.ToInt32(objnewmodel.Department);
        //        var getDeptCode = (from e in objentity.tblDeptMas where e.intid == deptID select e).FirstOrDefault();
        //        if (getDeptCode != null)
        //        {
        //            dptCode = getDeptCode.vchdepCode.ToString();
        //        }
        //        else
        //        {
        //            //Select All Department For selection department
        //            var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //            if (selectedDept.Count() != 0)
        //            {
        //                List<SelectListItem> newlist = new List<SelectListItem>();
        //                foreach (var dpt in selectedDept)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem
        //                    {
        //                        Value = dpt.intid.ToString(),
        //                        Text = dpt.vchdeptname
        //                    };
        //                    newlist.Add(selectListItem);
        //                }
        //                ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //            }
        //            TempData["Error"] = "Department code not found please check it again or contact to administrator!";
        //            return View();
        //        }
        //        string currentYear = (DateTime.Now.ToString("YYYY"));
        //        var getCode = (from e in objentity.tblCertificateCodeMas where e.CertificateCode == "EC" select e).FirstOrDefault();
        //        if (getCode != null)
        //        {
        //            //get certificate code
        //            certificateCode = getCode.CertificateCode.ToString();
        //            //get current count number of certificate
        //            certificateCurrentcount = Convert.ToInt16(getCode.intCurrent);
        //            int newcode = certificateCurrentcount + 1;
        //            int number = newcode;
        //            int counter = 0;
        //            string finalnumber = "";
        //            //int fnumber = 0;
        //            while (number > 0)
        //            {
        //                number = number / 10;
        //                counter++;
        //            }
        //            if (counter == 1)
        //            {
        //                finalnumber = string.Concat("00" + newcode.ToString());
        //            }
        //            if (counter == 2)
        //            {
        //                finalnumber = string.Concat("0" + newcode.ToString());

        //            }
        //            if (counter == 3)
        //            {
        //                finalnumber = newcode.ToString();
        //            }
        //            //Set Branch Code
        //            string finalBranchCode = string.Empty;
        //            string bcode = Session["id"].ToString();
        //            if (bcode == "3")
        //            {
        //                finalBranchCode = "IH";
        //            }
        //            if (bcode == "4")
        //            {
        //                finalBranchCode = "IS";
        //            }
        //            if (bcode == "14")
        //            {
        //                finalBranchCode = "II";
        //            }
        //            if (bcode == "15")
        //            {
        //                finalBranchCode = "IF";
        //            }
        //            if (bcode == "16")
        //            {
        //                finalBranchCode = "KS";
        //            }
        //            if (bcode == "21")
        //            {
        //                finalBranchCode = "MH";
        //            }
        //            if (bcode == "22")
        //            {
        //                finalBranchCode = "HS";
        //            }
        //            if (bcode == "23")
        //            {
        //                finalBranchCode = "MY";
        //            }
        //            DateTime aajkidate = DateTime.Now;
        //            string year = aajkidate.ToString("yy");
        //            //Format = Branch/HR/EC/EmpDpt/yr/series
        //            string Ref_Code = finalBranchCode + "/" + hrCharachter + "/" + certificateCode + "/" + dptCode + "/" + year + "/" + finalnumber;
        //            tblTemplateDetail objdetail = new tblTemplateDetail();
        //            objdetail.vchEmpName = objnewmodel.Name;
        //            objdetail.vchDepartment = objnewmodel.Department;
        //            objdetail.vchFaterName = objnewmodel.spouse;
        //            objdetail.dtSdate = objnewmodel.Sdate;
        //            objdetail.dtEdate = objnewmodel.Edate;
        //            objdetail.dtRelieved = objnewmodel.Rdate;
        //            objdetail.txtContent = objnewmodel.LetterContent;
        //            objdetail.vchRef_Code = Ref_Code.ToString();
        //            objdetail.fk_letterid = objnewmodel.fk_letterid;
        //            objdetail.vchCompany = Session["Compname"].ToString();
        //            objdetail.intcode = Convert.ToInt16(Session["id"].ToString());
        //            objdetail.vchCreatedBy = Session["descript"].ToString();
        //            objdetail.dtCreated = DateTime.Now;
        //            objentity.tblTemplateDetail.Add(objdetail);
        //            getCode.intCurrent = newcode;
        //            objentity.SaveChanges();
        //            TempData["Success"] = "Certificate saved successfully!";
        //            return RedirectToAction("ExpLetterMale");
        //        }
        //        else
        //        {
        //            //Select All Department For selection department
        //            var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //            if (selectedDept.Count() != 0)
        //            {
        //                List<SelectListItem> newlist = new List<SelectListItem>();
        //                foreach (var dpt in selectedDept)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem
        //                    {
        //                        Value = dpt.intid.ToString(),
        //                        Text = dpt.vchdeptname
        //                    };
        //                    newlist.Add(selectListItem);
        //                }
        //                ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //            }
        //            TempData["Error"] = "Certificate code not found please check it again or contact to administrator!";
        //            return RedirectToAction("ExpLetterMale");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //public ActionResult ExpLetterFemale()
        //{
        //    if (Session["descript"] != null)
        //    {
        //        TemplateMasDetailViewModelFemale objmodel = new TemplateMasDetailViewModelFemale();
        //        var selectedMas = (from e in objentity.tblTemplateMas where e.vchGender == "Female" && e.vchType == "Experience" select e).FirstOrDefault();
        //        if (selectedMas != null)
        //        {
        //            objmodel.fk_letterid = selectedMas.intid;
        //            objmodel.LetterHead = selectedMas.vchTemplateHeading;
        //            objmodel.LetterContent = selectedMas.txtLetterContent;
        //            //Select All Department For selection department
        //            var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //            if (selectedDept.Count() != 0)
        //            {
        //                List<SelectListItem> newlist = new List<SelectListItem>();
        //                foreach (var dpt in selectedDept)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem
        //                    {
        //                        Value = dpt.intid.ToString(),
        //                        Text = dpt.vchdeptname
        //                    };
        //                    newlist.Add(selectListItem);
        //                }
        //                ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //            }
        //            return View(objmodel);
        //        }
        //        else
        //        {
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //[HttpPost]
        //public ActionResult ExpLetterFemale(TemplateMasDetailViewModelFemale objnewmodel)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        //Proposed Codes:
        //        //IIH/HR/EC/BB2301(EC - Experience Certificate, BB - Blood Bank)
        //        //IIH/HR/EC/PL2301
        //        //The Certificates will be issued on the letter Heads of the Company which are in the custody of the HODs / HRs only.
        //        string hrCharachter = "HR";
        //        string dptCode = string.Empty;
        //        string certificateCode = string.Empty;
        //        int certificateCurrentcount = 0;
        //        int deptID = Convert.ToInt32(objnewmodel.Department);
        //        var getDeptCode = (from e in objentity.tblDeptMas where e.intid == deptID select e).FirstOrDefault();
        //        if (getDeptCode != null)
        //        {
        //            dptCode = getDeptCode.vchdepCode.ToString();
        //        }
        //        else
        //        {
        //            //Select All Department For selection department
        //            var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //            if (selectedDept.Count() != 0)
        //            {
        //                List<SelectListItem> newlist = new List<SelectListItem>();
        //                foreach (var dpt in selectedDept)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem
        //                    {
        //                        Value = dpt.intid.ToString(),
        //                        Text = dpt.vchdeptname
        //                    };
        //                    newlist.Add(selectListItem);
        //                }
        //                ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //            }
        //            TempData["Error"] = "Department code not found please check it again or contact to administrator!";
        //            return View();
        //        }
        //        //for code only comon certificate type='Experience'
        //        var getCode = (from e in objentity.tblCertificateCodeMas where e.CertificateCode == "EC" select e).FirstOrDefault();
        //        if (getCode != null)
        //        {
        //            //get certificate code
        //            certificateCode = getCode.CertificateCode.ToString();
        //            //get current count number of certificate
        //            certificateCurrentcount = Convert.ToInt16(getCode.intCurrent);
        //            int newcode = certificateCurrentcount + 1;
        //            int number = newcode;
        //            int counter = 0;
        //            string finalnumber = "";
        //            //int fnumber = 0;
        //            while (number > 0)
        //            {
        //                number = number / 10;
        //                counter++;
        //            }
        //            if (counter == 1)
        //            {
        //                finalnumber = string.Concat("00" + newcode.ToString());
        //            }
        //            if (counter == 2)
        //            {
        //                finalnumber = string.Concat("0" + newcode.ToString());
        //            }
        //            if (counter == 3)
        //            {
        //                finalnumber = newcode.ToString();
        //            }
        //            //Set Branch Code
        //            string finalBranchCode = string.Empty;
        //            string bcode = Session["id"].ToString();
        //            if (bcode == "3")
        //            {
        //                finalBranchCode = "IH";
        //            }
        //            if (bcode == "4")
        //            {
        //                finalBranchCode = "ISS";
        //            }
        //            if (bcode == "14")
        //            {
        //                finalBranchCode = "IIH";
        //            }
        //            if (bcode == "15")
        //            {
        //                finalBranchCode = "IF";
        //            }
        //            if (bcode == "16")
        //            {
        //                finalBranchCode = "KS";
        //            }
        //            if (bcode == "21")
        //            {
        //                finalBranchCode = "MH";
        //            }
        //            if (bcode == "22")
        //            {
        //                finalBranchCode = "HS";
        //            }
        //            if (bcode == "23")
        //            {
        //                finalBranchCode = "MY";
        //            }
        //            DateTime aajkidate = DateTime.Now;
        //            string year = aajkidate.ToString("yy");
        //            //Format = Branch/HR/EC/EmpDpt/yr/series
        //            string Ref_Code = finalBranchCode + "/" + hrCharachter + "/" + certificateCode + "/" + dptCode + "/" + year + "/" + finalnumber;
        //            tblTemplateDetail objdetail = new tblTemplateDetail();
        //            objdetail.vchEmpName = objnewmodel.Name;
        //            objdetail.vchDepartment = objnewmodel.Department;
        //            objdetail.vchFaterName = objnewmodel.spouse;
        //            objdetail.dtSdate = objnewmodel.Sdate;
        //            objdetail.dtEdate = objnewmodel.Edate;
        //            objdetail.dtRelieved = objnewmodel.Rdate;
        //            objdetail.txtContent = objnewmodel.LetterContent;
        //            objdetail.vchRef_Code = Ref_Code.ToString();
        //            objdetail.fk_letterid = objnewmodel.fk_letterid;
        //            objdetail.vchCompany = Session["Compname"].ToString();
        //            objdetail.intcode = Convert.ToInt16(Session["id"].ToString());
        //            objdetail.vchCreatedBy = Session["descript"].ToString();
        //            objdetail.dtCreated = DateTime.Now;
        //            objentity.tblTemplateDetail.Add(objdetail);
        //            getCode.intCurrent = newcode;
        //            objentity.SaveChanges();
        //            TempData["Success"] = "Certificate saved successfully!";
        //            return RedirectToAction("ExpLetterFemale");
        //        }
        //        else
        //        {
        //            //Select All Department For selection department
        //            var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //            if (selectedDept.Count() != 0)
        //            {
        //                List<SelectListItem> newlist = new List<SelectListItem>();
        //                foreach (var dpt in selectedDept)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem
        //                    {
        //                        Value = dpt.intid.ToString(),
        //                        Text = dpt.vchdeptname
        //                    };
        //                    newlist.Add(selectListItem);
        //                }
        //                ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //            }
        //            TempData["Error"] = "Certificate code not found please check it again or contact to administrator!";
        //            return RedirectToAction("ExpLetterFemale");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //public ActionResult EmpLetterDetail(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        if (id != 0)
        //        {
        //            var selectedLetter = (from e in objentity.tblTemplateDetail where e.intid == id select e).FirstOrDefault();
        //            return View(selectedLetter);
        //        }
        //        else
        //        {
        //            TempData["EmptyModel"] = "Details not passed in model please check again an try again!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}

        //public ActionResult ViewAllCertificate()
        //{
        //    if (Session["descript"] != null)
        //    {
        //        int code = Convert.ToInt16(Session["id"].ToString());
        //        var selecCertificate = (from e in objentity.tblTemplateDetail where e.intcode==code select e).ToList();
        //        if (selecCertificate.Count > 0)
        //        {
        //            return View(selecCertificate);
        //        }
        //        else
        //        {
        //            TempData["Empty"] = "0 Record found in database!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }

        //}

        //public ActionResult EmpCertificateDetail(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        if (id != 0)
        //        {
        //            var selectedCertificate = (from e in objentity.tblTemplateDetail
        //                                       join m in objentity.tblTemplateMas on e.fk_letterid equals m.intid
        //                                       where e.intid == id
        //                                       select e).FirstOrDefault();
        //            if (selectedCertificate == null)
        //            {
        //                TempData["Error"] = "Certificate detail not found please check it again!";
        //                return View();
        //            }
        //            else
        //            {
        //                string param1 = id.ToString();
        //                LocalReport lr = new LocalReport();                       
        //                string filepath = String.Empty;
        //                HttpClient _client = new HttpClient();
        //                string gender = selectedCertificate.tblTemplateMas.vchGender.ToString();
        //                string CertificateType = selectedCertificate.tblTemplateMas.vchType.ToString();
        //                if (gender == "Male" && CertificateType == "Experience")
        //                {
        //                    //Get dataset value and fields                           
        //                    var selectedobj = (from e in objentity.spGetExpLetter(id) select e).ToList();
        //                    //get path
        //                    filepath = Path.Combine(Server.MapPath("~/Content/Report"), ("ExpLetter.rdl"));
        //                    //open streams
        //                    using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
        //                    {
        //                        lr.LoadReportDefinition(filestream);
        //                        lr.DataSources.Add(new ReportDataSource(name: "DataSet1", selectedobj));
        //                        ReportParameter ptr = new ReportParameter("id", param1);                               
        //                        lr.SetParameters(ptr);
        //                        byte[] pdfData = lr.Render("PDF");
        //                        return File(pdfData, contentType: "Application/pdf");
        //                    }
        //                }
        //                if (gender == "Female" && CertificateType == "Experience")
        //                {
        //                    //Get dataset value and fields                           
        //                    var selectedobj = (from e in objentity.spGetExpLetter(id) select e).ToList();
        //                    //get report path
        //                    filepath = Path.Combine(Server.MapPath("~/Content/Report"), ("ExpLetterFemale.rdl"));
        //                    //open streams
        //                    using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
        //                    {
        //                        lr.LoadReportDefinition(filestream);
        //                        lr.DataSources.Add(new ReportDataSource(name: "DataSet1", selectedobj));
        //                        ReportParameter ptr = new ReportParameter("id", param1);
        //                        lr.SetParameters(ptr);
        //                        byte[] pdfData = lr.Render("PDF");
        //                        return File(pdfData, contentType: "Application/pdf");
        //                    }
        //                }
        //                if (gender == "All")
        //                {
        //                    filepath = "N/A";
        //                    return View();
        //                }
        //                else
        //                {
        //                    return View();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Certificate id not found please try again or contact to administrator!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //public ActionResult EditEmpLetter(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        if (id != 0)
        //        {
        //            var selected = (from e in objentity.tblTemplateDetail where e.intid == id select e).FirstOrDefault();
        //            if (selected != null)
        //            {
        //                //Select All Department For selection department
        //                var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //                if (selectedDept.Count() != 0)
        //                {
        //                    List<SelectListItem> newlist = new List<SelectListItem>();
        //                    foreach (var dpt in selectedDept)
        //                    {
        //                        SelectListItem selectListItem = new SelectListItem
        //                        {
        //                            Value = dpt.intid.ToString(),
        //                            Text = dpt.vchdeptname
        //                        };
        //                        newlist.Add(selectListItem);
        //                    }
        //                    ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //                }                        
        //                ViewBag.Sdate = Convert.ToDateTime(selected.dtSdate).ToString("dd/MM/yyyy");
        //                ViewBag.Edate = Convert.ToDateTime(selected.dtEdate).ToString("dd/MM/yyyy");
        //                ViewBag.Rdate = Convert.ToDateTime(selected.dtRelieved).ToString("dd/MM/yyyy");
        //                return View(selected);
        //            }
        //            else
        //            {
        //                //Select All Department For selection department
        //                var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //                if (selectedDept.Count() != 0)
        //                {
        //                    List<SelectListItem> newlist = new List<SelectListItem>();
        //                    foreach (var dpt in selectedDept)
        //                    {
        //                        SelectListItem selectListItem = new SelectListItem
        //                        {
        //                            Value = dpt.intid.ToString(),
        //                            Text = dpt.vchdeptname
        //                        };
        //                        newlist.Add(selectListItem);
        //                    }
        //                    ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //                }
        //                TempData["Error"] = "Letter detail not found please contact to administrator!";
        //                return View();
        //            }
        //        }
        //        else
        //        {
        //            //Select All Department For selection department
        //            var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //            if (selectedDept.Count() != 0)
        //            {
        //                List<SelectListItem> newlist = new List<SelectListItem>();
        //                foreach (var dpt in selectedDept)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem
        //                    {
        //                        Value = dpt.intid.ToString(),
        //                        Text = dpt.vchdeptname
        //                    };
        //                    newlist.Add(selectListItem);
        //                }
        //                ViewBag.DeptList = new SelectList(newlist, "Value", "Text");

        //            }
        //            TempData["Error"] = "Letter ID not found please check it and try again!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //[HttpPost]
        //public ActionResult EditEmpLetter(tblTemplateDetail objmodel)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        if (objmodel.intid != 0)
        //        {
        //            int id = objmodel.intid;
        //            var selected = (from e in objentity.tblTemplateDetail where e.intid == id select e).FirstOrDefault();
        //            if (objmodel.vchEmpName != null)
        //            {
        //                selected.vchEmpName=objmodel.vchEmpName;
        //            }
        //            if (objmodel.vchFaterName != null)
        //            {
        //                selected.vchFaterName = objmodel.vchFaterName;
        //            }
        //            if (objmodel.dtSdate != null)
        //            {
        //                selected.dtSdate = objmodel.dtSdate;
        //            }
        //            if (objmodel.dtEdate != null)
        //            {
        //                selected.dtEdate = objmodel.dtEdate;
        //            }
        //            if (objmodel.dtRelieved != null)
        //            {
        //                selected.dtRelieved = objmodel.dtRelieved;
        //            }
        //            if (objmodel.vchDepartment != null)
        //            {
        //              selected.vchDepartment=objmodel.vchDepartment;
        //            }
        //            if (objmodel.txtContent != null)
        //            {
        //                selected.txtContent = objmodel.txtContent;
        //            }
        //            objentity.SaveChanges();
        //            TempData["success"] = "Letter updated successfully!";
        //            return RedirectToAction("ViewAllCertificate");
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Letter should not be null or empty, please try again!";
        //            return RedirectToAction("ViewAllCertificate");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("", "");              
        //    }
        //}

        //#endregion

        //#region Offer letter Master
        //public ActionResult IndexOfferLetter()
        //{
        //    if (Session["descript"] != null)
        //    {
        //        var selected = (from e in objentity.tblTemplateMas where e.vchTemplateName == "Offer Letter" select e).ToList();
        //        if (selected.Count > 0)
        //        {
        //            TempData["Success"] = "Found " + selected.Count() + "Offer Letter";
        //            return View();
        //        }
        //        else
        //        {
        //            TempData["Empty"] = "No record found in database!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //public ActionResult ViewOfferLetter(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        var selectedLetter = (from e in objentity.tblTemplateMas where e.intid == id select e).FirstOrDefault();
        //        if (selectedLetter != null)
        //        {
        //            return View(selectedLetter);
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Selected letter not found please check it again carefully!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //public ActionResult EditOfferLetter(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        var editletter = (from e in objentity.tblTemplateMas where e.intid == id select e).FirstOrDefault();
        //        if (editletter != null)
        //        {
        //            return View(editletter);
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Selected letter not found please check it again carefully!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //[HttpPost]
        //public ActionResult EditOfferLetter(tblTemplateDetail objtbl)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        var editletter = (from e in objentity.tblTemplateMas where e.intid == objtbl.intid select e).FirstOrDefault();
        //        if (editletter != null)
        //        {
        //            return View(editletter);
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Selected letter not found please check it again carefully!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //#endregion

        //#region Employee Offer Letter

        //public ActionResult EmpOfferLetter()
        //{
        //    if (Session["descript"] != null)
        //    {
        //        tblOfferLetter objmodel = new tblOfferLetter();
        //        var selectedLetter = (from e in objentity.tblTemplateMas where e.vchType == "OL" select e).FirstOrDefault();
        //        objmodel.txtContent = selectedLetter.txtLetterContent.ToString();
        //        objmodel.fk_templateid = selectedLetter.intid;
        //        //Select All Department For selection department
        //        var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //        if (selectedDept.Count() != 0)
        //        {
        //            List<SelectListItem> newlist = new List<SelectListItem>();
        //            foreach (var dpt in selectedDept)
        //            {
        //                SelectListItem selectListItem = new SelectListItem
        //                {
        //                    Value = dpt.intid.ToString(),
        //                    Text = dpt.vchdeptname
        //                };
        //                newlist.Add(selectListItem);
        //            }
        //            ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //        }
        //        // Select all Position
        //        var allPosition = (from e in objentity.tblPositionCategoryMas orderby e.vchPosCatName select e).ToList();
        //        if (allPosition.Count() != 0)
        //        {
        //            List<SelectListItem> positionList = new List<SelectListItem>();
        //            foreach (var position in allPosition)
        //            {
        //                SelectListItem selectListItem = new SelectListItem
        //                {
        //                    Value = position.intid.ToString(),
        //                    Text = position.vchPosCatName
        //                };
        //                positionList.Add(selectListItem);
        //            }
        //            ViewBag.PositionList = new SelectList(positionList, "Value", "Text");
        //        }
        //        return View(objmodel);
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //[HttpPost]
        //public ActionResult EmpOfferLetter(tblOfferLetter objmodel)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        string hrCharachter = "HR";
        //        string dptCode = string.Empty;
        //        string certificateCode = string.Empty;
        //        int certificateCurrentcount = 0;
        //        int deptID = Convert.ToInt32(objmodel.fk_deptId);
        //        int positionID = Convert.ToInt32(objmodel.fk_positionId);
        //        var getDeptCode = (from e in objentity.tblDeptMas where e.intid == deptID select e).FirstOrDefault();
        //        if (getDeptCode != null)
        //        {
        //            dptCode = getDeptCode.vchdepCode.ToString();
        //        }
        //        else
        //        {
        //            //Select All Department For selection department
        //            var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //            if (selectedDept.Count() != 0)
        //            {
        //                List<SelectListItem> newlist = new List<SelectListItem>();
        //                foreach (var dpt in selectedDept)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem
        //                    {
        //                        Value = dpt.intid.ToString(),
        //                        Text = dpt.vchdeptname
        //                    };
        //                    newlist.Add(selectListItem);
        //                }
        //                ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //            }
        //            // Select all Position
        //            var allPosition = (from e in objentity.tblPositionCategoryMas orderby e.vchPosCatName select e).ToList();
        //            if (allPosition.Count() != 0)
        //            {
        //                List<SelectListItem> positionList = new List<SelectListItem>();
        //                foreach (var position in allPosition)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem
        //                    {
        //                        Value = position.intid.ToString(),
        //                        Text = position.vchPosCatName
        //                    };
        //                    positionList.Add(selectListItem);
        //                }
        //                ViewBag.PositionList = new SelectList(positionList, "Value", "Text");
        //            }
        //            TempData["Error"] = "Department code not found please check it again or contact to administrator!";
        //            return View();
        //        }

        //        var getCode = (from e in objentity.tblCertificateCodeMas where e.CertificateCode == "OL" select e).FirstOrDefault();
        //        if (getCode != null)
        //        {
        //            //get certificate code
        //            certificateCode = getCode.CertificateCode.ToString();
        //            //get current count number of certificate
        //            certificateCurrentcount = Convert.ToInt16(getCode.intCurrent);
        //            int newcode = certificateCurrentcount + 1;
        //            int number = newcode;
        //            int counter = 0;
        //            string finalnumber = "";
        //            //int fnumber = 0;
        //            while (number > 0)
        //            {
        //                number = number / 10;
        //                counter++;
        //            }
        //            if (counter == 1)
        //            {
        //                finalnumber = string.Concat("00" + newcode.ToString());
        //            }
        //            if (counter == 2)
        //            {
        //                finalnumber = string.Concat("0" + newcode.ToString());

        //            }
        //            if (counter == 3)
        //            {
        //                finalnumber = newcode.ToString();
        //            }
        //            //Set Branch Code
        //            string finalBranchCode = string.Empty;
        //            string bcode = Session["id"].ToString();
        //            if (bcode == "3")
        //            {
        //                finalBranchCode = "IH";
        //            }
        //            if (bcode == "4")
        //            {
        //                finalBranchCode = "ISS";
        //            }
        //            if (bcode == "14")
        //            {
        //                finalBranchCode = "IIH";
        //            }
        //            if (bcode == "15")
        //            {
        //                finalBranchCode = "IF";
        //            }
        //            if (bcode == "16")
        //            {
        //                finalBranchCode = "KS";
        //            }
        //            if (bcode == "21")
        //            {
        //                finalBranchCode = "MH";
        //            }
        //            if (bcode == "22")
        //            {
        //                finalBranchCode = "HS";
        //            }
        //            if (bcode == "23")
        //            {
        //                finalBranchCode = "MY";
        //            }
        //            DateTime aajkidate = DateTime.Now;
        //            string year = aajkidate.ToString("yy");
        //            //Format = Branch/HR/EC/EmpDpt/yr/series
        //            string Ref_Code = finalBranchCode + "/" + hrCharachter + "/" + certificateCode + "/" + dptCode + "/" + year + "/" + finalnumber;
        //            //tblOfferLetter objoffer = new tblOfferLetter();
        //            objmodel.vchRef_Code = Ref_Code.ToString();
        //            objmodel.vchBranch = Session["Compname"].ToString();
        //            objmodel.intCode = Convert.ToInt16(Session["id"].ToString());
        //            objmodel.issueBy = Session["descript"].ToString();
        //            objmodel.dtissuesCertificate = DateTime.Now;
        //            objentity.tblOfferLetter.Add(objmodel);
        //            getCode.intCurrent = newcode;
        //            objentity.SaveChanges();
        //            TempData["Success"] = "Certificate saved successfully!";
        //            return RedirectToAction("EmpOfferLetter");
        //        }
        //        else
        //        {
        //            //Select All Department For selection department
        //            var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //            if (selectedDept.Count() != 0)
        //            {
        //                List<SelectListItem> newlist = new List<SelectListItem>();
        //                foreach (var dpt in selectedDept)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem
        //                    {
        //                        Value = dpt.intid.ToString(),
        //                        Text = dpt.vchdeptname
        //                    };
        //                    newlist.Add(selectListItem);
        //                }
        //                ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //            }
        //            // Select all Position
        //            var allPosition = (from e in objentity.tblPositionCategoryMas orderby e.vchPosCatName select e).ToList();
        //            if (allPosition.Count() != 0)
        //            {
        //                List<SelectListItem> positionList = new List<SelectListItem>();
        //                foreach (var position in allPosition)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem
        //                    {
        //                        Value = position.intid.ToString(),
        //                        Text = position.vchPosCatName
        //                    };
        //                    positionList.Add(selectListItem);
        //                }
        //                ViewBag.PositionList = new SelectList(positionList, "Value", "Text");
        //            }
        //            TempData["Error"] = "Certificate code not found please check it again or contact to administrator!";
        //            return RedirectToAction("EmpOfferLetter");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //public ActionResult ViewAllOfferLetter()
        //{
        //    if (Session["descript"] != null)
        //    {
        //        int code = Convert.ToInt32(Session["id"].ToString());
        //        var offerlist = (from e in objentity.tblOfferLetter where e.intCode == code select e).ToList();
        //        if (offerlist.Count > 0)
        //        {
        //            int count = offerlist.Count();
        //            TempData["Success"] = +count + " Offer letter found";
        //            return View(offerlist);
        //        }
        //        else
        //        {
        //            TempData["Empty"] = "0 Offer letter found in database!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //public ActionResult OfferLetterPrint(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        if (id != 0)
        //        {
        //            var selectedCertificate = (from e in objentity.tblOfferLetter
        //                                       join m in objentity.tblTemplateMas on e.fk_templateid equals m.intid
        //                                       where e.intid == id
        //                                       select e).FirstOrDefault();
        //            if (selectedCertificate == null)
        //            {
        //                TempData["Error"] = "Certificate detail not found please check it again!";
        //                return View();
        //            }
        //            else
        //            {
        //                LocalReport lr = new LocalReport();
        //                string filepath = String.Empty;
        //                HttpClient _client = new HttpClient();

        //                string CertificateType = selectedCertificate.tblTemplateMas.vchType.ToString();

        //                //Get dataset value and fields
        //                var selectedobj = (from e in objentity.spGetOfferLetter(id) select e).ToList();

        //                //get path
        //                filepath = Path.Combine(Server.MapPath("~/Content/Report"), ("OfferLetter.rdl"));
        //                //open streams
        //                using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
        //                {
        //                    lr.LoadReportDefinition(filestream);
        //                    lr.DataSources.Add(new ReportDataSource(name: "DataSet1", selectedobj));
        //                    ReportParameter ptr = new ReportParameter("id", id.ToString());
        //                    lr.SetParameters(ptr);
        //                    byte[] pdfData = lr.Render("PDF");
        //                    return File(pdfData, contentType: "Application/pdf");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Offer letter id/detail not found please check it and try again!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //public ActionResult EditEmpOfferLetter(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        if (id != 0)
        //        {
        //            var selectedLetter = (from e in objentity.tblOfferLetter where e.intid == id select e).FirstOrDefault();
        //            if (selectedLetter != null)
        //            {
        //                //Select All Department For selection department
        //                var selectedDept = (from e in objentity.tblDeptMas select e).ToList();
        //                if (selectedDept.Count() != 0)
        //                {
        //                    List<SelectListItem> newlist = new List<SelectListItem>();
        //                    foreach (var dpt in selectedDept)
        //                    {
        //                        SelectListItem selectListItem = new SelectListItem
        //                        {
        //                            Value = dpt.intid.ToString(),
        //                            Text = dpt.vchdeptname
        //                        };
        //                        newlist.Add(selectListItem);
        //                    }
        //                    ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
        //                }
        //                // Select all Position
        //                var allPosition = (from e in objentity.tblPositionCategoryMas orderby e.vchPosCatName select e).ToList();
        //                if (allPosition.Count() != 0)
        //                {
        //                    List<SelectListItem> positionList = new List<SelectListItem>();
        //                    foreach (var position in allPosition)
        //                    {
        //                        SelectListItem selectListItem = new SelectListItem
        //                        {
        //                            Value = position.intid.ToString(),
        //                            Text = position.vchPosCatName
        //                        };
        //                        positionList.Add(selectListItem);
        //                    }
        //                    ViewBag.PositionList = new SelectList(positionList, "Value", "Text");
        //                }
        //                ViewBag.AppDate = Convert.ToDateTime(selectedLetter.dtApllicationdate).ToString("dd/MM/yyyy");
        //                ViewBag.BeforeDate = Convert.ToDateTime(selectedLetter.dtJoinBefore).ToString("dd/MM/yyyy");
        //                ViewBag.AcceptDate = Convert.ToDateTime(selectedLetter.dtAcceptance).ToString("dd/MM/yyyy");
        //                return View(selectedLetter);
        //            }
        //            else
        //            {
        //                TempData["Error"] = "Selected letter detail not found please check it again and try again!";
        //                return View();
        //            }
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Selected employee id not found please check it and try again!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //[HttpPost]
        //public ActionResult EditEmpOfferLetter(tblOfferLetter objmodel)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        int id = objmodel.intid;
        //        var selectedLetter = (from e in objentity.tblOfferLetter where e.intid == id select e).FirstOrDefault();
        //        if (selectedLetter != null)
        //        {
        //            selectedLetter.fk_deptId = objmodel.fk_deptId;
        //            selectedLetter.fk_positionId = objmodel.fk_positionId;
        //            selectedLetter.dtApllicationdate = objmodel.dtApllicationdate;
        //            selectedLetter.vchName = objmodel.vchName;
        //            selectedLetter.dtJoinBefore = objmodel.dtJoinBefore;
        //            selectedLetter.dtAcceptance = objmodel.dtAcceptance;
        //            selectedLetter.txtContent = objmodel.txtContent;
        //            selectedLetter.dtUpdated = DateTime.Now;
        //            selectedLetter.vchUpdatedBy = Session["descript"].ToString();
        //            objentity.SaveChanges();
        //            TempData["Success"] = "Letter updated successfully";
        //            return RedirectToAction("ViewAllOfferLetter");
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Selected letter detail not found";
        //            return View("");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}
        //#endregion

        //#region Candidate Intership Letter

        //public ActionResult InternshipIndex()
        //{
        //    if (Session["descript"] != null)
        //    {
        //        int code = Convert.ToInt32(Session["id"].ToString());
        //        var selectedList = (from e in objentity.tblinternship where e.intCode==code select e).ToList();
        //        if (selectedList.Count != 0)
        //        {
        //            int countR = selectedList.Count();
        //            TempData["Success1"] = + countR + " Records found";
        //            return View(selectedList);
        //        }
        //        else
        //        {
        //            TempData["Empty"] = "No letter found in database";
        //            return View();
        //        }

        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //public ActionResult InternshipNew()
        //{
        //    if (Session["descript"] != null)
        //    {
        //        var selectedMas = (from e in objentity.tblTemplateMas where e.vchType == "IC" select e).FirstOrDefault();
        //        if (selectedMas != null)
        //        {
        //            tblinternship objtable = new tblinternship();
        //            objtable.fk_TemplateId = selectedMas.intid;
        //            objtable.txtContent = selectedMas.txtLetterContent;
        //            //for gender selection
        //            List<SelectListItem> genderList = new List<SelectListItem> {
        //                                     new SelectListItem{
        //                                          Text="Male",
        //                                          Value = "1"
        //                                         },
        //                                     new SelectListItem{
        //                                         Text="Female",
        //                                         Value ="2"
        //                                        }
        //                                     };
        //            ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
        //            return View(objtable);
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Master data not found contact to administrator!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //[HttpPost]
        //public ActionResult InternshipNew(tblinternship objmodel)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        if (objmodel != null)
        //        {
        //            string hrCharachter = "HR";
        //            string dptCode = "INT";
        //            string certificateCode = string.Empty;
        //            int certificateCurrentcount = 0;
        //            var getCode = (from e in objentity.tblCertificateCodeMas where e.CertificateCode == "IC" select e).FirstOrDefault();
        //            if (getCode != null)
        //            {
        //                //get certificate code
        //                certificateCode = getCode.CertificateCode.ToString();
        //                //get current count number of certificate
        //                certificateCurrentcount = Convert.ToInt16(getCode.intCurrent);
        //                int newcode = certificateCurrentcount + 1;
        //                int number = newcode;
        //                int counter = 0;
        //                string finalnumber = "";
        //                //int fnumber = 0;
        //                while (number > 0)
        //                {
        //                    number = number / 10;
        //                    counter++;
        //                }
        //                if (counter == 1)
        //                {
        //                    finalnumber = string.Concat("00" + newcode.ToString());
        //                }
        //                if (counter == 2)
        //                {
        //                    finalnumber = string.Concat("0" + newcode.ToString());

        //                }
        //                if (counter == 3)
        //                {
        //                    finalnumber = newcode.ToString();
        //                }
        //                //Set Branch Code
        //                string finalBranchCode = string.Empty;
        //                string bcode = Session["id"].ToString();
        //                if (bcode == "3")
        //                {
        //                    finalBranchCode = "IH";
        //                }
        //                if (bcode == "4")
        //                {
        //                    finalBranchCode = "ISS";
        //                }
        //                if (bcode == "14")
        //                {
        //                    finalBranchCode = "IIH";
        //                }
        //                if (bcode == "15")
        //                {
        //                    finalBranchCode = "IF";
        //                }
        //                if (bcode == "16")
        //                {
        //                    finalBranchCode = "KS";
        //                }
        //                if (bcode == "21")
        //                {
        //                    finalBranchCode = "MH";
        //                }
        //                if (bcode == "22")
        //                {
        //                    finalBranchCode = "HS";
        //                }
        //                if (bcode == "23")
        //                {
        //                    finalBranchCode = "MY";
        //                }
        //                DateTime aajkidate = DateTime.Now;
        //                string year = aajkidate.ToString("yy");
        //                string Ref_Code = finalBranchCode + "/" + hrCharachter + "/" + certificateCode + "/" + dptCode + "/" + year + "/" + finalnumber;
        //                objmodel.vchRefCode = Ref_Code;
        //                if (objmodel.vchGender == "1")
        //                {
        //                    objmodel.vchGender = "Male";
        //                }
        //                else
        //                {
        //                    objmodel.vchGender = "Female";
        //                }
        //                objmodel.vchcreatedBy = Session["descript"].ToString();
        //                objmodel.dtcreated = DateTime.Now;
        //                objmodel.vchCreatedIp = Session["ipused"].ToString();
        //                objmodel.vchCreatedHost = Session["hostname"].ToString();
        //                objmodel.vchBranch = Session["Compname"].ToString();
        //                objmodel.intCode = Convert.ToInt32(Session["id"].ToString());
        //                objentity.tblinternship.Add(objmodel);
        //                objentity.SaveChanges();
        //                TempData["Success"] = "Record saved successfully";
        //                return RedirectToAction("InternshipNew");
        //            }
        //            else
        //            {
        //                //for gender selection
        //                List<SelectListItem> genderList = new List<SelectListItem> {
        //                                     new SelectListItem{
        //                                          Text="Male",
        //                                          Value = "1"
        //                                         },
        //                                     new SelectListItem{
        //                                         Text="Female",
        //                                         Value ="2"
        //                                        }
        //                                     };
        //                ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
        //                TempData["Error"] = "Department code not found so please contact to application administrator!";
        //                return View();
        //            }
        //        }
        //        else
        //        {
        //            //for gender selection
        //            List<SelectListItem> genderList = new List<SelectListItem> {
        //                                     new SelectListItem{
        //                                          Text="Male",
        //                                          Value = "1"
        //                                         },
        //                                     new SelectListItem{
        //                                         Text="Female",
        //                                         Value ="2"
        //                                        }
        //                                     };
        //            ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
        //            TempData["Error"] = "Model error generated please generate certificate carefully, try again";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //public ActionResult InernshipEdit(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        if (id != 0)
        //        {
        //            var selected = (from e in objentity.tblinternship where e.intid == id select e).FirstOrDefault();
        //            if (selected != null)
        //            {
        //                //for gender selection
        //                List<SelectListItem> genderList = new List<SelectListItem> {
        //                                     new SelectListItem{
        //                                          Text="Male",
        //                                          Value = "1"
        //                                         },
        //                                     new SelectListItem{
        //                                         Text="Female",
        //                                         Value ="2"
        //                                        }
        //                                     };
        //                ViewBag.GenderList = new SelectList(genderList, "Value", "Text");
        //                ViewBag.dtDOS = Convert.ToDateTime(selected.dtDOS).ToString("dd/MM/yyyy");
        //                ViewBag.dtDOE = Convert.ToDateTime(selected.dtDOE).ToString("dd/MM/yyyy");
        //                return View(selected);
        //            }
        //            else
        //            {
        //                TempData["Error"] = "Letter id not found please check it again, and try!";
        //                return RedirectToAction("InternshipIndex");
        //            }
        //        }
        //        else
        //        {
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}   

        //[HttpPost]
        //public ActionResult InernshipEdit(tblinternship objnewmodel)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        int id = objnewmodel.intid;
        //        var selecetd = (from e in objentity.tblinternship where e.intid == id select e).FirstOrDefault();
        //        if (selecetd != null)
        //        {
        //            selecetd.vchName = objnewmodel.vchName;
        //            if (objnewmodel.vchGender == "1")
        //            {
        //                objnewmodel.vchGender = "Male";
        //            }
        //            if(objnewmodel.vchGender=="2")
        //            {
        //                objnewmodel.vchGender = "Female";
        //            }
        //            selecetd.vchFatherName = objnewmodel.vchFatherName;
        //            selecetd.txtContent = objnewmodel.txtContent;
        //            selecetd.dtDOS = objnewmodel.dtDOS;
        //            selecetd.dtDOE = objnewmodel.dtDOE;
        //            selecetd.vchUpdatedBy = Session["descript"].ToString();
        //            selecetd.vchGender = objnewmodel.vchGender;
        //            selecetd.dtupdated = DateTime.Now;
        //            selecetd.vchUpdatedIp = Session["ipused"].ToString();
        //            selecetd.vchUpdatedHost = Session["hostname"].ToString();
        //            objentity.SaveChanges();
        //            TempData["Success"] = "Letter detail updated successfully!";
        //            return RedirectToAction("InternshipIndex");
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Letter detail not found please check it again and try!";
        //            return RedirectToAction("InternshipIndex");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1","Home");
        //    }
        //}

        //public ActionResult printInternship(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        if (id != 0)
        //        {
        //            var selected = (from e in objentity.tblinternship where e.intid == id select e).FirstOrDefault();
        //            if (selected != null)
        //            {
        //                int masid = selected.fk_TemplateId;
        //                var TemplateMas = (from e in objentity.tblTemplateMas where e.intid == masid select e).FirstOrDefault();
        //                LocalReport lr = new LocalReport();
        //                string filepath = String.Empty;
        //                HttpClient _client = new HttpClient();
        //                string CertificateType = TemplateMas.vchType.ToString();
        //                //Get dataset value and fields
        //                var selectedobj = (from e in objentity.spGetInternshipLetter(id) select e).ToList();
        //                //get path
        //                filepath = Path.Combine(Server.MapPath("~/Content/Report"), ("IntershipLetter.rdl"));
        //                //open streams
        //                using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
        //                {
        //                    lr.LoadReportDefinition(filestream);
        //                    lr.DataSources.Add(new ReportDataSource(name: "DataSet1", selectedobj));
        //                    ReportParameter ptr = new ReportParameter("id", id.ToString());
        //                    lr.SetParameters(ptr);
        //                    byte[] pdfData = lr.Render("PDF");
        //                    return File(pdfData, contentType: "Application/pdf"); //,string.Format("Internship{0}.pdf",id));
        //                }
        //            }
        //            else
        //            {
        //                TempData["error"] = "Letter not found please check it and try again.";
        //                return View();
        //            }
        //        }
        //        else
        //        {
        //            TempData["error"] = "Letter id not found please check it and try again.";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        //#endregion       
    }
}