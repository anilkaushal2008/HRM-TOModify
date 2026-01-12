using DocumentFormat.OpenXml.Office2010.Excel;
using HRM.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc; 
using System.Web.UI;
using System.CodeDom;
using System.Net;


namespace HRM.Controllers
{
    public class ConsultantController : Controller
    {
        // GET: Consultant
        HRMEntities hrentity = new HRMEntities();

        #region Consutalant New, Edit,View
        public ActionResult Index()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var ConsultantList = (from e in hrentity.tblEmpAssesmentMas join d in hrentity.tblDoctorUploadDetail on e.intid equals d.fk_ConsultMasId where e.intcode == code && e.bitIsConsultant == true && d.bitIsActive == true select e).ToList();
                if (ConsultantList.Count() != 0)
                {
                    return View(ConsultantList);
                }
                else
                {
                    ViewBag.Empty = "0 Consultatnt found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult NewConsultant()
        {
            if (Session["descript"] != null)
            {
                //objmodel view
                ConsultantViewModel objmodel = new ConsultantViewModel();
                //Select all department master
                var deptlist = (from e in hrentity.tblDeptMas orderby e.vchdeptname select e).ToList();
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
                //for gender selection
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
                //Select consultant type
                List<SelectListItem> ConsltType = new List<SelectListItem> {
                                             new SelectListItem{
                                                  Text="Regular",
                                                  Value = "1"
                                                 },
                                             new SelectListItem{
                                                 Text="Visiting",
                                                 Value ="2"
                                                }
                                             };
                ViewBag.ConsltType = new SelectList(ConsltType, "Value", "Text");
                //for marital status selection
                List<SelectListItem> Maritalstatus = new List<SelectListItem>
                             {
                                new SelectListItem{
                                      Text = "Un-Married",
                                      Value = "1" },
                                new SelectListItem{
                                      Text = "Married",
                                      Value = "2" }
                              };
                ViewBag.MaritalStatus = new SelectList(Maritalstatus, "Value", "Text");
                //get all states
                var getstates = (from e in hrentity.tblState select e).ToList();
                List<SelectListItem> states = new List<SelectListItem>();
                states.Add(new SelectListItem { Text = "Select state", Value = "0" });
                if (getstates.Count() != 0)
                {
                    foreach (var st in getstates)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = st.vchState,
                            Value = st.intid.ToString()
                        };
                        states.Add(selectListItem);
                    }
                    ViewBag.State = new SelectList(states, "Text", "Value");
                }
                else
                {
                    ViewBag.State = new SelectList(states, "NotFound", "NotFound");
                }
                // select all position
                var allposii = (from e in hrentity.tblPositionCategoryMas where e.intid>=5222 && e.intid <= 5226
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

        [HttpGet]
        public ActionResult DesignationList(string dept_id)
        {
            hrentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            int deptid = Convert.ToInt32(dept_id);
            List<tblDesignationMas> desilist = new List<tblDesignationMas>();
            desilist = (from e in hrentity.tblDesignationMas where e.intdeptid == deptid select e).ToList();
            var result = (from d in desilist
                          orderby d.vchdesignation
                          select new
                          {
                              id = d.intid,
                              designation = d.vchdesignation
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult NewConsultant(ConsultantViewModel objmodel, FormCollection fc)
        {
            if (Session["descript"] != null)
            {
                int cityid = Convert.ToInt32(fc.Get("selectedCity"));
                //session id
                int code = Convert.ToInt32(Session["id"].ToString());
                int satateid = Convert.ToInt32(objmodel.fk_state);
                var selecetdState = (from e in hrentity.tblState where e.intid == satateid select e).FirstOrDefault();
                var selecedCity = (from e in hrentity.tblCity where e.intid == cityid select e).FirstOrDefault();
                objmodel.fk_city = cityid;
                if (ModelState.IsValid)
                {
                    try
                    {
                        //master object and entry
                        //check aadhaar number
                        var checkAdhar = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.AadhaarNo select e).FirstOrDefault();
                        if (checkAdhar == null)
                        {
                            //selected position
                            //var selectedPossition = (from e in hrentity.tblPositionCategoryMas where e.intid == 4183 select e).FirstOrDefault();
                            int desiid = Convert.ToInt32(fc.Get("selecteddesi"));
                            tblEmpAssesmentMas objmas = new tblEmpAssesmentMas();
                            tblEmpDetails objdetail = new tblEmpDetails();
                            objmas.vchName = objmodel.Name;
                            objmas.fk_intdeptid = objmodel.fk_Dept;
                            objmas.fk_intdesiid = desiid;
                            objmas.fk_PositionId = objmodel.fk_Position;
                            objmas.vchWorkedArea = "Consultant";
                            objmas.vchMobile = objmodel.mobile;
                            objmas.vchAadharNo = objmodel.AadhaarNo;
                            objmas.decExperience = objmodel.experience;
                            objmas.vchMobile = objmodel.mobile;
                            objmas.intsalary = objmodel.intsalary;
                            objmas.dtDOJ = objmodel.DOJ;
                            objmas.bitIsConsultant = true;
                            objmas.intcode = code;
                            hrentity.tblEmpAssesmentMas.Add(objmas);
                            hrentity.SaveChanges();
                            //get new created consultant mas data
                            var NewDoc = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.AadhaarNo select e).FirstOrDefault();
                            if (NewDoc != null)
                            {
                                //Detail table and detail entry
                                objdetail.fk_intempid = NewDoc.intid;
                                objmas.vchEmpTcode = "T" + NewDoc.intid;
                                objdetail.vchFatherName = objmodel.FatherName;
                                objdetail.vchmothername = objmodel.MotherName;
                                if (objmodel.vchspousename != null)
                                {
                                    objdetail.vchspouse = objmodel.vchspousename;
                                }
                                else
                                {
                                    objdetail.vchspouse = "N/A";
                                }
                                objdetail.dtDob = objmodel.DOB;
                                objdetail.intage = objmodel.age;
                                objdetail.vchQualifications = objmodel.Qualification;
                                objdetail.vchspouse = objmodel.vchspousename;
                                if (objmodel.vchGender == "1")
                                {
                                    objdetail.vchsex = "Male";
                                    objdetail.fk_titid = 1;
                                }
                                if (objmodel.vchGender == "2")
                                {
                                    objdetail.vchsex = "Female";
                                    objdetail.fk_titid = 7;
                                }
                                if (objmodel.vchmaritalststus == "1")
                                {
                                    objdetail.vchmaritalststus = "Un-Married";
                                }
                                if (objmodel.vchmaritalststus == "2")
                                {
                                    objdetail.vchmaritalststus = "Married";
                                }
                                objdetail.vchfname = objmodel.Name;
                                objdetail.vchlname = objmodel.Name;
                                objdetail.vchNominee = objmodel.vchNominee;
                                objdetail.vchRelation = objmodel.vchNomRelation;
                                objdetail.vchpaddress = objmodel.vchaddress;
                                objdetail.vchtaddress = objmodel.vchaddress;
                                objdetail.fk_State = selecetdState.intid;
                                objdetail.fk_city = selecedCity.intid;
                                objdetail.vchpstreet = "N/A";
                                objdetail.vchpstate = selecetdState.vchState;
                                objdetail.vchtstate = selecetdState.vchState;
                                objdetail.vchpcity = selecedCity.vchCityName;
                                objdetail.vchtcity = selecedCity.vchCityName;
                                objdetail.intppin = Convert.ToInt32(objmodel.vchpincode);
                                objdetail.inttpin = Convert.ToInt32(objmodel.vchpincode);
                                objdetail.vchtmobile = objmodel.mobile;
                                objdetail.vchpmobile = objmodel.mobile;
                                objdetail.BitCompleted = true;
                                objdetail.intcode = code;
                                //Upload detail entry 
                                tblDoctorUploadDetail objdoc = new tblDoctorUploadDetail();
                                objdoc.fk_ConsultMasId = NewDoc.intid;
                                if (objmodel.type == "1")
                                {
                                    objdoc.bitIsRegular = true;
                                }
                                if (objmodel.type == "2")
                                {
                                    objdoc.bitisVisiting = true;
                                }
                                //Create Consultant User Mas
                                tblEmpLoginUser objlogin = new tblEmpLoginUser();
                                var checkemp = (from e in hrentity.tblEmpLoginUser where e.fk_intEmpID == NewDoc.intid && e.intcode == code select e).FirstOrDefault();
                                if (checkemp == null)
                                {
                                    objlogin.fk_intEmpID = NewDoc.intid;
                                    objlogin.vchmobile = NewDoc.vchMobile;
                                    objlogin.vchOTP = "Temp";
                                    objlogin.intcode = code;
                                    objlogin.intyr = Convert.ToInt32(Session["yr"].ToString());
                                }
                                hrentity.tblEmpLoginUser.Add(objlogin);
                                hrentity.tblDoctorUploadDetail.Add(objdoc);
                                hrentity.tblEmpDetails.Add(objdetail);
                                hrentity.SaveChanges();
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Aadhaar number already in use please check it first!";
                            return RedirectToAction("NewConsultant");
                        }
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
                    }
                    TempData["Success"] = "Consultant saved successfully, Your can upload consultant document from now!";
                    return RedirectToAction("NewConsultant");
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

        #endregion

        #region Registration Upload       

        public ActionResult UploadConsultantDoc()
        {
            if (Session["descript"] != null)
            {
                List<spGetConsultant_Result> selecetdConsult = new List<spGetConsultant_Result>();
                int Bcode = Convert.ToInt32(Session["id"].ToString());
                selecetdConsult = (from e in hrentity.spGetConsultant(Bcode) select e).ToList();
                if (selecetdConsult.Count() != 0)
                {
                    return View(selecetdConsult);

                }
                else
                {
                    ViewBag.Empty = "0 record found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }


        public ActionResult AddRegistration(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var getConsultant = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                if (getConsultant != null)
                {
                    ConsultRegistration objregistration = new ConsultRegistration();
                    objregistration.empID = getConsultant.intid;
                    objregistration.Name = getConsultant.vchName;
                    objregistration.EmpTcode = getConsultant.vchEmpTcode;
                    objregistration.mobile = getConsultant.vchMobile;
                    objregistration.fileName = "Registration";
                    return View(objregistration);
                }
                else
                {
                    TempData["Error"] = "Consultant detail not found please check it again!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddRegistration(ConsultRegistration objresistration)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    //tale object
                    tblRegistrationDetail objRegistrationDetail = new tblRegistrationDetail();
                    int id = objresistration.empID;
                    var selectedEMp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    if (selectedEMp != null)
                    {
                        //check pdf file
                        string extension = Path.GetExtension(objresistration.pdfFile.FileName);
                        if (extension == ".pdf" || extension == ".PDF")

                        {
                            var selecetdDocEntry = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
                            string filename = Path.GetFileNameWithoutExtension(objresistration.pdfFile.FileName);
                            string empid = id.ToString();
                            string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                            string newfilename = filename + datetime + empid + extension;
                            string finalfilename = newfilename.Replace(" ", "");
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + finalfilename));
                            objresistration.pdfFile.SaveAs(path);
                            objRegistrationDetail.fk_EMpid = id;
                            objRegistrationDetail.vchFileAddress = path.ToString();
                            objRegistrationDetail.vchFileName = finalfilename;
                            objRegistrationDetail.vchDocName = objresistration.fileName;
                            //as current registration
                            objRegistrationDetail.bitIsNew = true;
                            objRegistrationDetail.dtRegistration = DateTime.Now;
                            objRegistrationDetail.vchCreatedBy = Session["descript"].ToString();
                            objRegistrationDetail.dtRegistrationFrom = Convert.ToDateTime(objresistration.dtRegiFrom);
                            objRegistrationDetail.dtRegistrationTo = Convert.ToDateTime(objresistration.dtRegito);
                            //select doctor upload detail for updatation
                            var selecetdDocupload = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
                            selecetdDocupload.bitRegiUploaded = true;
                            selecetdDocupload.bitRegistrationlocked = true;
                            hrentity.tblRegistrationDetail.Add(objRegistrationDetail);
                            hrentity.SaveChanges();
                            TempData["Success"] = "Registration detail saved successfully!";
                            return RedirectToAction("UploadConsultantDoc");
                        }
                        else
                        {
                            TempData["Error"] = "Select only .pdf file for upload!";
                            ModelState.AddModelError("pdfFile", "Please select only .pdf/.PDF file only");
                            return View();
                        }

                    }
                    else
                    {
                        TempData["Error"] = "Selected consultant details not found, so please try again!";
                        return RedirectToAction("UploadConsultantDoc");
                    }
                }
                else
                {
                    TempData["Error"] = "Model error generated please contact to administration!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ViewRegistration(int id)
        {
            if (Session["descript"] != null)
            {
                var selected = (from e in hrentity.tblRegistrationDetail where e.fk_EMpid == id select e).FirstOrDefault();
                if (selected != null)
                {
                    return View(selected);
                }
                else
                {
                    TempData["Error"] = "Selected consultant detail not found, check it again or contact to administrator!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        public ActionResult EditRegistration(int id)
        {
            if (Session["descript"] != null)
            {
                var getConsultant = (from e in hrentity.tblRegistrationDetail join m in hrentity.tblEmpAssesmentMas on e.fk_EMpid equals m.intid where m.intid == id && e.bitIsNew == true select new { e = e, m = m }).FirstOrDefault();
                if (getConsultant != null)
                {
                    ConsultRegistration objregistration = new ConsultRegistration();
                    objregistration.empID = getConsultant.m.intid;
                    objregistration.Name = getConsultant.m.vchName;
                    objregistration.EmpTcode = getConsultant.m.vchEmpTcode;
                    objregistration.mobile = getConsultant.m.vchMobile;
                    objregistration.fileName = "Registration";
                    //objregistration.dtRegiFrom = Convert.ToDateTime(getConsultant.e.dtRegistrationFrom.ToString("dd/MM/yyyy"));
                    objregistration.dtRegiFrom = getConsultant.e.dtRegistrationFrom;
                    objregistration.dtRegito = getConsultant.e.dtRegistrationTo;
                    objregistration.dtRegistration = getConsultant.e.dtRegistration;
                    return View(objregistration);
                }
                else
                {
                    TempData["Error"] = "Selected consultant detail not found, check it again or contact to administrator!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditRegistration(ConsultRegistration objEdit)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    var GetOldRecord = (from e in hrentity.tblRegistrationDetail where e.fk_EMpid == objEdit.empID && e.bitIsNew == true select e).FirstOrDefault();
                    if (GetOldRecord != null)
                    {
                        //check pdf file
                        int id = objEdit.empID;
                        string extension = Path.GetExtension(objEdit.pdfFile.FileName);
                        if (extension == ".pdf" || extension == ".PDF")

                        {
                            string filename = Path.GetFileNameWithoutExtension(objEdit.pdfFile.FileName);
                            string empid = id.ToString();
                            string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                            string newfilename = filename + datetime + empid + extension;
                            string finalfilename = newfilename.Replace(" ", "");
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + finalfilename));
                            objEdit.pdfFile.SaveAs(path);
                            GetOldRecord.fk_EMpid = id;
                            GetOldRecord.vchFileAddress = path.ToString();
                            GetOldRecord.vchFileName = finalfilename;
                            GetOldRecord.vchDocName = objEdit.fileName;
                            //as current registration
                            GetOldRecord.bitIsNew = true;
                            GetOldRecord.dtUpdated = DateTime.Now;
                            GetOldRecord.vchUpdatedBy = Session["descript"].ToString();
                            GetOldRecord.vchUpdatedIP = Session["ipused"].ToString();
                            GetOldRecord.dtRegistrationFrom = Convert.ToDateTime(objEdit.dtRegiFrom);
                            GetOldRecord.dtRegistrationTo = Convert.ToDateTime(objEdit.dtRegito);
                            //select doctor upload detail for updatation
                            var selecetdDocupload = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
                            selecetdDocupload.bitRegiUploaded = true;
                            selecetdDocupload.bitRegistrationlocked = true;
                            hrentity.SaveChanges();
                            TempData["Success"] = "Registration detail updated successfully!";
                            return RedirectToAction("UploadConsultantDoc");
                        }
                        else
                        {
                            ModelState.AddModelError("pdfFile", "Please select only .pdf/.PDF file only");
                            return View();
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Old registration detail not found please check it again or contact to administrator!";
                        return View();
                    }


                }
                else
                {
                    TempData["Error"] = "Model error generated, contact to administrator!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult UnlockRegistration(int id)
        {
            if (Session["descript"] != null)
            {
                var selectedDoctor = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
                if (selectedDoctor != null)
                {
                    selectedDoctor.bitRegistrationlocked = false;
                    hrentity.SaveChanges();
                    TempData["Success"] = "Registration unlocked successfully!";
                    return RedirectToAction("UploadConsultantDoc");
                }
                else
                {
                    TempData["Error"] = "Registration detail not found please check it again or contact to administrator!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region MOU Upload
        public ActionResult UpConsultantMOU(int id)
        {
            if (Session["descript"] != null)
            {
                var GetConultatnt = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).First();
                if (GetConultatnt != null)
                {
                    UpMOUViewModel objViewModel = new UpMOUViewModel();
                    objViewModel.empID = GetConultatnt.intid;
                    objViewModel.Name = GetConultatnt.vchName;
                    objViewModel.EmpTcode = GetConultatnt.vchEmpTcode;
                    objViewModel.mobile = GetConultatnt.vchMobile;
                    objViewModel.fileName = "MOU";
                    return View(objViewModel);
                }
                else
                {
                    TempData["Error"] = "Selected cinsultant detail not found please check it again or contact to administrator1";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult UpConsultantMOU(UpMOUViewModel objmodel)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    tblMouDetail objmou = new tblMouDetail();
                    int id = objmodel.empID;
                    var selectedEMp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    if (selectedEMp != null)
                    {
                        //check pdf file
                        string extension = Path.GetExtension(objmodel.pdfFile.FileName);
                        if (extension == ".pdf" || extension == ".PDF")
                        {
                            var selecetdDocEntry = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
                            string filename = Path.GetFileNameWithoutExtension(objmodel.pdfFile.FileName);
                            string empid = id.ToString();
                            string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                            string newfilename = filename + datetime + empid + extension;
                            string finalfilename = newfilename.Replace(" ", "");
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + finalfilename));
                            objmodel.pdfFile.SaveAs(path);
                            objmou.fk_EMpid = id;
                            objmou.vchFileAddress = path.ToString();
                            objmou.vchFileName = finalfilename;
                            objmou.vchDocName = objmodel.fileName;
                            //as current registration
                            objmou.bitIsCurrent = true;
                            objmou.dtMOUCreated = objmodel.dtCreatedMOU;
                            objmou.vchCreatedBy = Session["descript"].ToString();
                            objmou.dtEffectFrom = Convert.ToDateTime(objmodel.dtEffectFrom);
                            objmou.dtEffectTo = Convert.ToDateTime(objmodel.dtEffectTo);
                            //select doctor upload detail for updatation
                            var selecetdDocupload = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
                            selecetdDocupload.bitMOUUploaded = true;
                            selecetdDocupload.bitMOUlocked = true;
                            hrentity.tblMouDetail.Add(objmou);
                            hrentity.SaveChanges();
                            TempData["Success"] = "MOU detail saved successfully!";
                            return RedirectToAction("UploadConsultantDoc");
                        }
                        else
                        {
                            TempData["Error"] = "Select only .pdf file for upload!";
                            ModelState.AddModelError("pdfFile", "Please select only .pdf/.PDF file only");
                            return View();
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Selected consultant details not found, so please try again!";
                        return RedirectToAction("UploadConsultantDoc");
                    }
                }
                else
                {
                    TempData["Error"] = "Model error generated please contact to administration!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ViewMOU(int id)
        {
            if (Session["descript"] != null)
            {
                var selectedMOU = (from e in hrentity.tblMouDetail where e.fk_EMpid == id && e.bitIsCurrent == true select e).FirstOrDefault();
                if (selectedMOU != null)
                {
                    return View(selectedMOU);
                }
                else
                {
                    TempData["Error"] = "Selected MOU detail not found please try again or contact to administrator!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult UnlockMOU(int id)
        {
            if (Session["descript"] != null)
            {
                var selectedDoctor = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
                if (selectedDoctor != null)
                {
                    selectedDoctor.bitMOUlocked = false;
                    hrentity.SaveChanges();
                    TempData["Success"] = "MOU unlocked successfully!";
                    return RedirectToAction("UploadConsultantDoc");
                }
                else
                {
                    TempData["Error"] = "MOU detail not found please check it again or contact to administrator!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult EditMOU(int id)
        {
            if (Session["descript"] != null)
            {
                var getConsultant = (from e in hrentity.tblMouDetail join m in hrentity.tblEmpAssesmentMas on e.fk_EMpid equals m.intid where m.intid == id && e.bitIsCurrent == true select new { e = e, m = m }).FirstOrDefault();
                if (getConsultant != null)
                {
                    UpMOUViewModel objMOU = new UpMOUViewModel();
                    objMOU.empID = getConsultant.m.intid;
                    objMOU.MOUid = getConsultant.e.intid;
                    objMOU.Name = getConsultant.m.vchName;
                    objMOU.EmpTcode = getConsultant.m.vchEmpTcode;
                    objMOU.mobile = getConsultant.m.vchMobile;
                    objMOU.fileName = "MOU";
                    objMOU.dtEffectFrom = Convert.ToDateTime(getConsultant.e.dtEffectFrom.ToString("dd/MM/yyyy"));
                    objMOU.dtEffectTo = getConsultant.e.dtEffectTo;
                    objMOU.dtCreatedMOU = getConsultant.e.dtMOUCreated;
                    return View(objMOU);
                }
                else
                {
                    TempData["Error"] = "Selected consultant detail not found, check it again or contact to administrator!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditMOU(UpMOUViewModel objEdit)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    var GetOldRecord = (from e in hrentity.tblMouDetail where e.fk_EMpid == objEdit.empID && e.bitIsCurrent == true select e).FirstOrDefault();
                    if (GetOldRecord != null)
                    {
                        //check pdf file
                        int id = objEdit.empID;
                        string extension = Path.GetExtension(objEdit.pdfFile.FileName);
                        if (extension == ".pdf" || extension == ".PDF")
                        {
                            string filename = Path.GetFileNameWithoutExtension(objEdit.pdfFile.FileName);
                            string empid = id.ToString();
                            string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                            string newfilename = filename + datetime + empid + extension;
                            string finalfilename = newfilename.Replace(" ", "");
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + finalfilename));
                            objEdit.pdfFile.SaveAs(path);
                            GetOldRecord.fk_EMpid = id;
                            GetOldRecord.vchFileAddress = path.ToString();
                            GetOldRecord.vchFileName = finalfilename;
                            GetOldRecord.vchDocName = objEdit.fileName;
                            //as current registration
                            GetOldRecord.bitIsCurrent = true;
                            GetOldRecord.dtUpdated = DateTime.Now;
                            GetOldRecord.vchUpdatedBy = Session["descript"].ToString();
                            GetOldRecord.vchUpdatedIP = Session["ipused"].ToString();
                            GetOldRecord.dtEffectFrom = Convert.ToDateTime(objEdit.dtEffectFrom);
                            GetOldRecord.dtEffectTo = Convert.ToDateTime(objEdit.dtEffectTo);
                            //select doctor upload detail for updatation
                            var selecetdDocupload = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
                            selecetdDocupload.bitMOUUploaded = true;
                            selecetdDocupload.bitMOUlocked = true;
                            hrentity.SaveChanges();
                            TempData["Success"] = "MOU detail updated successfully!";
                            return RedirectToAction("UploadConsultantDoc");
                        }
                        else
                        {
                            ModelState.AddModelError("pdfFile", "Please select only .pdf/.PDF file only");
                            return View();
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Old registration detail not found please check it again or contact to administrator!";
                        return View();
                    }


                }
                else
                {
                    TempData["Error"] = "Model error generated, contact to administrator!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Upload other document and generate code

        public ActionResult UploadConultDoc(int id)
        {
            if (Session["descript"] != null)
            {
                //mas object
                List<tblDoctorDocMas> objdoc = new List<tblDoctorDocMas>();
                var getobjdoc = (from e in hrentity.tblDoctorDocMas select e).ToList();
                var getDocDetail = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == id select e).ToList();
                if (getobjdoc.Count() != 0)
                {
                    foreach (var doc in getobjdoc)
                    {
                        foreach (var uploaded in getDocDetail)
                        {
                            if (doc.intid == uploaded.fk_DocMasId)
                            {
                                doc.bitIsSelected = true;
                                doc.vchCompareFileAdd = uploaded.vchFileName;
                                doc.dtTempUploaded = uploaded.dtCreated;
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

        public ActionResult UpDocument(int id, int docid)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                //mast doc id=13(Profile pic)
                if (docid == 4)
                {
                    return RedirectToAction("ConsultantProfile", new { id = id, docid = docid });
                }
                else
                {
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                    var selectedDoc = (from e in hrentity.tblDoctorDocMas where e.intid == docid select e).FirstOrDefault();
                    if (selectedDoc != null)
                    {
                        ViewBag.Docname = selectedDoc.vchDocMasName.ToString();
                    }
                    ViewBag.Mobile = selectedemp.vchMobile.ToString();
                    ViewBag.Empname = selectedemp.vchName.ToString();
                    //For compulsory document
                    UpConsultantDocModel objmodel = new UpConsultantDocModel();
                    int fk_possiid = selectedemp.fk_PositionId;
                    objmodel.empid = selectedemp.intid;
                    objmodel.fk_docid = docid;
                    return View(objmodel);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ConsultantProfile(int id, int docid)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    ProfileViewModel objmodel = new ProfileViewModel();
                    var selectedDOc = (from e in hrentity.tblDoctorDocMas where e.intid == docid select e).FirstOrDefault();
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                    ViewBag.Mobile = selectedemp.vchMobile.ToString();
                    ViewBag.Empname = selectedemp.vchName.ToString();
                    ViewBag.EmpProfileName = selectedDOc.vchDocMasName;
                    ViewBag.EmpID = id.ToString();
                    ViewBag.Filename = selectedDOc.vchDocMasName;
                    objmodel.BitIsCompleted = Convert.ToBoolean(selectedemp.bitProfileComplete);
                    objmodel.docid = docid;
                    objmodel.empid = id;
                    objmodel.picname = selectedDOc.vchDocMasName;
                    return View(objmodel);
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

        [HttpPost]
        public ActionResult ConsultantProfile(ProfileViewModel objpic, FormCollection fromform)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int year = Convert.ToInt32(Session["yr"].ToString());
                    //docdetails table object                  
                    tblConsultantDocDetail objdocdetail = new tblConsultantDocDetail();
                    //get mas doc
                    var MasDoc = (from e in hrentity.tblDoctorDocMas where e.intid == objpic.docid select e).FirstOrDefault();
                    //get emp id
                    int id1 = objpic.empid;
                    string selectedDoc = objpic.picname.ToString();
                    var checktable = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == id1 && e.fk_DocMasId == objpic.docid select e).FirstOrDefault();
                    if (checktable != null)
                    {
                        string selecetddocname = checktable.vchDocName.ToString();
                        TempData["Error"] = "Against" + " " + selecetddocname + " " + "uploaded against current name!";
                        return RedirectToAction("ByPassProfilePic", new { id = id1, objpic.docid });
                    }
                    else
                    {
                        //check for pdf null
                        if (objpic.profilepic != null)
                        {
                            //filename new format filename+datetime+empid+extension                         
                            string empid = id1.ToString();
                            string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                            //remove space
                            string extension = Path.GetExtension(objpic.profilepic.FileName);
                            if (extension != ".jpg")
                            {
                                //ModelState.AddModelError("filename", "Please select .jpg file for upload!");
                                TempData["Error"] = "Please select .jpg file for upload!";
                                return RedirectToAction("UploadConultDoc", new { id = objpic.empid, docid = objpic.docid });
                            }
                            else
                            {
                                string filename = Path.GetFileNameWithoutExtension(objpic.profilepic.FileName);
                                string newfilename = filename + datetime + empid + extension;
                                string finalfilename = newfilename.Replace(" ", "");
                                string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + finalfilename));
                                //save file in upload folder
                                objpic.profilepic.SaveAs(path);
                                var selecetdmasemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id1 && e.intcode == code select e).FirstOrDefault();
                                //save file in upload folder                                
                                objdocdetail.fk_DoctorID = objpic.empid;
                                objdocdetail.fk_DocMasId = objpic.docid;
                                objdocdetail.vchDocName = MasDoc.vchDocMasName.ToString();
                                objdocdetail.vchFileName = finalfilename.ToString();
                                objdocdetail.vchDocAddress = path.ToString();
                                objdocdetail.vchCreatedBy = Session["descript"].ToString();
                                objdocdetail.dtCreated = DateTime.Now;
                                objdocdetail.vchCretaedIP = Session["ipused"].ToString();
                                objdocdetail.vchCreatedHost = Session["hostname"].ToString();
                                objdocdetail.bitIsUpload = true;
                                objdocdetail.bitIsProfile = true;
                                hrentity.tblConsultantDocDetail.Add(objdocdetail);
                                hrentity.SaveChanges();
                                TempData["Success"] = "Photograph uploaded successfully!";
                                return RedirectToAction("UploadConultDoc", new { id = objpic.empid });
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Please select profile file for upload!";
                            return RedirectToAction("UploadConultDoc", new { id = objpic.empid });
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Model Error Generated!";
                    return RedirectToAction("UploadConultDoc", new { id = objpic.empid });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult UpDocument(UpConsultantDocModel objmodel)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int year = Convert.ToInt32(Session["yr"].ToString());
                    //docdetails table object
                    tblConsultantDocDetail objdocdetail = new tblConsultantDocDetail();
                    int empid = objmodel.empid;
                    //get all emp id qualifications
                    int docid = objmodel.fk_docid;
                    //get mas doc
                    var MasDoc = (from e in hrentity.tblDoctorDocMas where e.intid == docid select e).FirstOrDefault();
                    //get mas data
                    var empmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    //check is doc uploaded or not
                    var checkdoc = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == empid && e.fk_DocMasId == docid select e).FirstOrDefault();
                    if (objmodel.newpdfFile != null)
                    {
                        //string empid = empid.ToString();
                        string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                        string Final_datetime = datetime.Replace(" ", "");
                        string extension = Path.GetExtension(objmodel.newpdfFile.FileName);
                        if (extension != ".pdf")
                        {
                            TempData["Error"] = "Please select .pdf file for upload!";
                            return RedirectToAction("UpByPassDoc", new { id = empid });
                        }
                        else
                        {
                            string filename = Path.GetFileNameWithoutExtension(objmodel.newpdfFile.FileName);
                            string final_FileName = filename.Replace(" ", "");
                            string newfilename = final_FileName + Final_datetime + empid.ToString() + extension;
                            string finalfilename = newfilename.Replace(" ", "");
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + finalfilename));
                            //save file in upload folder
                            objmodel.newpdfFile.SaveAs(path);
                            objdocdetail.fk_DoctorID = empid;
                            objdocdetail.fk_DocMasId = docid;
                            objdocdetail.vchDocName = MasDoc.vchDocMasName.ToString();
                            objdocdetail.vchFileName = finalfilename.ToString();
                            objdocdetail.vchDocAddress = path.ToString();
                            objdocdetail.vchCreatedBy = Session["descript"].ToString();
                            objdocdetail.dtCreated = DateTime.Now;
                            objdocdetail.vchCretaedIP = Session["ipused"].ToString();
                            objdocdetail.vchCreatedHost = Session["hostname"].ToString();
                            objdocdetail.bitIsUpload = true;
                            hrentity.tblConsultantDocDetail.Add(objdocdetail);
                            hrentity.SaveChanges();
                            TempData["Success"] = "Document upload successfully!";
                            return RedirectToAction("UploadConultDoc", new { id = empid });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Select pdf file for upload!";
                        return View("UploadConultDoc", new { id = objmodel.empid });
                    }

                }
                else
                {
                    //ModelState.AddModelError("newpdfFile", "ModelError Generated!");
                    TempData["Error"] = "Model error generated please try again with document and .pdf file selections or contact to administrator!";
                    return RedirectToAction("UploadConultDoc", new { id = objmodel.empid });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ViewOther(int id)
        {
            if (Session["descript"] != null)
            {
                var selectedDoctor = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                if (selectedDoctor != null)
                {
                    //get all uploaded document
                    List<tblConsultantDocDetail> all_List = new List<tblConsultantDocDetail>();
                    all_List = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == id select e).ToList();
                    if (all_List.Count() != 0)
                    {
                        return View(all_List);
                    }
                    else
                    {
                        TempData["Error"] = "Selected doctor 0 documents found in database!";
                        return RedirectToAction("UploadConsultantDoc");
                    }
                }
                else
                {
                    TempData["Error"] = "Selected doctor detail not found please check it again or contact to administrator!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult UnlockOtherDOc(int id)
        {
            if (Session["descript"] != null)
            {
                var selectedDoctor = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
                if (selectedDoctor != null)
                {
                    selectedDoctor.bitOtherUploadLock = false;
                    hrentity.SaveChanges();
                    TempData["Success"] = "Other documents unlocked successfully!";
                    return RedirectToAction("UploadConsultantDoc");
                }
                else
                {
                    TempData["Error"] = "Other Document detail not found please check it again or contact to administrator!";
                    return RedirectToAction("UploadConsultantDoc");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult DeleteOtherDoc(int id, int empid)
        {
            if (Session["descript"] != null)
            {
                if (id != 0 && empid != 0)
                {
                    var selectedDoc = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == empid && e.fk_DocMasId == id select e).FirstOrDefault();
                    if (selectedDoc != null)
                    {
                        //remove file first
                        var fullPath = Path.Combine(Server.MapPath("~/Content/Uploads/"), selectedDoc.vchFileName);

                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath); // Delete file first
                        }
                        hrentity.tblConsultantDocDetail.Remove(selectedDoc);
                        hrentity.SaveChanges();
                        TempData["Success"] = "Document deleted successfully!";
                        return RedirectToAction("UploadConultDoc", new { id = empid });
                    }
                    else
                    {
                        TempData["Error"] = "Document details not found please check it again and try!";
                        return RedirectToAction("UploadConultDoc", new { id = empid });
                    }
                }
                else
                {
                    TempData["Error"] = "Document id should not be null or 0";
                    return RedirectToAction("UploadConultDoc", new { id = empid });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult CompleteAllDoc(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selected = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    //Get to Update Documents upload details
                    var getDocDetailMas = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
                    if (selected != null)
                    {
                        //doc object
                        List<tblDoctorDocMas> objDoc = new List<tblDoctorDocMas>();
                        //get all positional required document that should be uploaded
                        int posID = selected.fk_PositionId;
                        List<int> getRequiredDoc = (from e in hrentity.tblDoctorDocMas where e.bitIsComplulsory == true select e.intid).ToList();
                        //get all employee doc uploaded
                        List<int> uploadedDoc = new List<int>();
                        var GetDoctorDoc = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == id select e).ToList();
                        if (GetDoctorDoc != null)
                        {
                            foreach (var docid in GetDoctorDoc)
                            {
                                int Requiredid = Convert.ToInt32(docid.fk_DocMasId);
                                uploadedDoc.Add(Requiredid);
                            }
                        }
                        if (getRequiredDoc.Count() != 0)
                        {
                            //compare both list having common or required doc in emp doc list or not
                            bool CompareItems = getRequiredDoc.All(x => uploadedDoc.Contains(x));
                            if (CompareItems == false)
                            {
                                List<int> notUploaded = getRequiredDoc.Except(uploadedDoc).ToList();
                                foreach (var doc in notUploaded)
                                {
                                    var getPendingDoc = (from e in hrentity.tblDoctorDocMas where e.intid == doc select e).FirstOrDefault();
                                    if (getPendingDoc != null)
                                    {
                                        objDoc.Add(getPendingDoc);
                                    }
                                }
                                TempData["PendingDoc"] = objDoc;
                                return RedirectToAction("UploadConsultantDoc");
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            getDocDetailMas.bitICompletedUpload = true;
                            hrentity.SaveChanges();
                            TempData["Success"] = "Doctor uploads locked successfully! Generate Code From Complete Uploads";
                            return RedirectToAction("UploadConsultantDoc");
                        }
                        getDocDetailMas.bitICompletedUpload = true;
                        hrentity.SaveChanges();
                        TempData["Success"] = "Doctor uploads locked successfully! Generate Code From Complete Uploads";
                        return RedirectToAction("UploadConsultantDoc");
                    }
                    else
                    {
                        TempData["Error"] = "0 employee found in database";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Employee id should not be null or 0!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");

            }
        }

        public ActionResult UploadedConsult()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedList = (from e in hrentity.tblEmpAssesmentMas
                                    join d in hrentity.tblDoctorUploadDetail on e.intid equals d.fk_ConsultMasId
                                    where e.intcode == code && d.bitICompletedUpload == true && d.bitIsActive != true && d.bitIsLeft != true && e.bitIsConsultant == true
                                    select e).ToList();
                if (selectedList.Count() != 0)
                {
                    ViewBag.Success = "Found " + selectedList.Count() + " consultant in database";
                    return View(selectedList);
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

        public ActionResult GenerateConsultCode(int id)
        {
            //Generate Unit code
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
            var getDptCode = (from e in hrentity.tblDeptMas where e.intid == selectedemp.fk_intdeptid select e).FirstOrDefault();
            //get series year
            int codeYear = 2023;
            var getcode = (from e in hrentity.tblEmpCodeMas where e.intBranchCode == code && e.intJoinYear == codeYear select e).FirstOrDefault();
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
            DateTime GetYear = Convert.ToDateTime(selectedemp.dtDOJ);
            string yearCode = GetYear.ToString("yy");
            string finalCompleteCode = branchcode + "-" + yearCode + "-" + fdeptcode + "-" + finalnumber;
            getcode.intCurrentCode = newcode;
            selectedemp.bitIsCorporateemp = false;
            selectedemp.bitIsUnitEmp = true;
            selectedemp.vchEmpFcode = finalCompleteCode;
            selectedemp.vchFinalUpdatedBy = Session["descript"].ToString();
            selectedemp.vchFinalHostname = Session["hostname"].ToString();
            selectedemp.vchipdusedauthor = Session["ipused"].ToString();
            selectedemp.dtFinalUpdated = DateTime.Now;
            //update as active doctor
            var selectedStatus = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
            selectedStatus.bitIsActive = true;
            hrentity.SaveChanges();
            TempData["Success"] = "Code generated successfully, new code is : " + finalCompleteCode;
            return RedirectToAction("UploadedConsult");
        }

        #endregion

        #region Full consultant View
        public ActionResult FullConsultView(int id)
        {
            return View();
        }

        public ActionResult PartialConsultMas(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var personaldetails = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                if (personaldetails != null)
                {
                    return View(personaldetails);
                }
                else
                {
                    TempData["Empty"] = "Official detail not found!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PartialConsultDetail(int id)
        {

            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var contactdet = (from e in hrentity.tblEmpDetails where e.fk_intempid == id select e).FirstOrDefault();
                if (contactdet != null)
                {
                    return View(contactdet);
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

        public ActionResult PartialConsultProfile(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                if (id != 0)
                {
                    string isHrAdmin = Session["HrAdmin"].ToString();
                    string isAuthorizer = Session["AllowAuthorization"].ToString();
                    string isMainAdmin = Session["MainAdmin"].ToString();
                    var docdetails = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == id && e.bitIsProfile == true select e).FirstOrDefault();
                    var selectedimage = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    if (docdetails != null)
                    {
                        return View(docdetails);
                    }
                    else
                    {
                        ViewBag.Empty = "Profile picture not found!";
                        return View();
                    }

                }
                else
                {
                    TempData["Empty"] = "Profile picture not avilable!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PartialConsultMOU(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                List<SelectListItem> mylist = new List<SelectListItem>();
                var mou = (from e in hrentity.tblMouDetail where e.fk_EMpid == id select e).ToList();
                if (mou != null)
                {
                    foreach (var per in mou)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = per.vchDocName.ToString(),
                            Value = per.vchFileName.ToString(),
                        };
                        mylist.Add(selectListItem);
                    }
                    ViewBag.MOU = (mylist);
                    return View();
                }
                else
                {
                    ViewBag.Empty = "MOU not found please chect it!";
                    return View();
                }

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PartialConsultRegistration(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                List<SelectListItem> mylist = new List<SelectListItem>();
                var Registration = (from e in hrentity.tblRegistrationDetail where e.fk_EMpid == id select e).ToList();
                if (Registration != null)
                {
                    foreach (var per in Registration)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = per.vchDocName.ToString(),
                            Value = per.vchFileName.ToString(),
                        };
                        mylist.Add(selectListItem);
                    }
                    ViewBag.Regi = (mylist);
                    return View();
                }
                else
                {
                    ViewBag.Empty = "Compulsory document not found please chect it!";
                    return View();
                }

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PartialConsultDocument(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                List<SelectListItem> mylist = new List<SelectListItem>();
                var compdoc = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == id && e.bitIsProfile != true select e).ToList();
                if (compdoc != null)
                {
                    foreach (var per in compdoc)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = per.vchDocName.ToString(),
                            Value = per.vchFileName.ToString(),
                        };
                        mylist.Add(selectListItem);
                    }
                    ViewBag.OtherDoc = (mylist);
                    return View();
                }
                else
                {
                    ViewBag.Empty = "Other document not found please chect it!";
                    return View();
                }

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Left Consultant
        public ActionResult LeftIndex()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedList = (from e in hrentity.tblEmpAssesmentMas join d in hrentity.tblDoctorUploadDetail on e.intid equals d.fk_ConsultMasId where d.bitIsLeft == true && e.bitIsConsultant == true && e.intcode == code select e).ToList();
                if (selectedList.Count() != 0)
                {
                    ViewBag.Success = "Found " + selectedList.Count() + " Left Consultants";
                    return View(selectedList);
                }
                else
                {
                    ViewBag.Empty = "Found 0 Left Consultants in database";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
        #endregion

        #region Change consultant status

        public ActionResult ConsultDeactivation(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                ConsultantDeActiveModel objmodel = new ConsultantDeActiveModel();
                var selecetdemp = (from e in hrentity.tblEmpAssesmentMas join d in hrentity.tblDoctorUploadDetail on e.intid equals d.fk_ConsultMasId where e.intid == id && e.intcode == code select e).FirstOrDefault();
                if (selecetdemp != null)
                {
                    objmodel.ConsultantId = selecetdemp.intid;
                    objmodel.EmpCode = selecetdemp.vchEmpFcode;
                    objmodel.Name = selecetdemp.vchName;
                    if (selecetdemp.vchAadharNo != null)
                    {
                        objmodel.AadharNo = selecetdemp.vchAadharNo.ToString();
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
        [HttpPost]
        public ActionResult ChangeStatus(ConsultantDeActiveModel objmodel)
        {
            if (Session["descript"] != null)
            {
                int id = objmodel.ConsultantId;
                if (id != 0)
                {
                    var selectedConsult = (from e in hrentity.tblEmpAssesmentMas join d in hrentity.tblDoctorUploadDetail on e.intid equals d.fk_ConsultMasId where e.intid == id select e).FirstOrDefault();
                    if (selectedConsult != null)
                    {
                        //select consultant details
                        var getDetail = (from e in hrentity.tblDoctorUploadDetail where e.fk_ConsultMasId == id select e).FirstOrDefault();
                        selectedConsult.dtDOL = objmodel.dol;
                        selectedConsult.vchFlagRemarks = objmodel.remarks;
                        getDetail.bitIsActive = false;
                        getDetail.bitIsLeft = true;
                        hrentity.SaveChanges();
                        TempData["SUccess"] = "Consultant status changed successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = "Selected Consultant detail not found in database, contact to administrator!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["Error"] = "Consultant id should not be null or empty!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
        #endregion

        #region Consultant Uploads KRA/KPA
        //from accounts corner
        public ActionResult KRADetail(int id)
        {
            if (Session["descript"] == null)
            {
                return RedirectToAction("_SessionError1", "Home");
            }

            // 1. Fetch all documents for this specific doctor
            var uploadedDocs = hrentity.tblKRADocDetail
                                       .Where(e => e.fk_EmpID == id)
                                       .ToList();

            // 2. Identify all unique years from the uploaded documents (e.g., "January-2024" -> "2024")
            var distinctYears = uploadedDocs
                .Select(d => d.vchDocMasName.Split('-').Last())
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            // If no records exist, default to the current year so admin can still upload new ones
            if (!distinctYears.Any())
            {
                distinctYears.Add(DateTime.Now.Year.ToString());
            }

            List<tblMonthMaster> monthMasterList = hrentity.tblMonthMaster.ToList();
            List<tblMonthMaster> finalModel = new List<tblMonthMaster>();

            // 3. Loop through years first, then months, to build the complete history
            foreach (var year in distinctYears)
            {
                foreach (var mMaster in monthMasterList)
                {
                    string lookupKey = $"{mMaster.Month}-{year}";

                    // Check if this doctor has an uploaded document for this Month-Year combo
                    var doc = uploadedDocs.FirstOrDefault(d => d.vchDocMasName == lookupKey);

                    // Create a new instance for each entry to avoid object reference duplication
                    var entry = new tblMonthMaster
                    {
                        Month = lookupKey, // e.g., "May-2025"
                        intTempEmpID = id,
                        bitIsUploaded = doc != null
                    };

                    if (doc != null)
                    {
                        entry.vchTempDocAddress = doc.vchFileAddress;
                        entry.vchTempFileName = doc.vchFileName;
                        entry.vchTempDocMasName = doc.vchDocMasName;
                        entry.dtTempUploaded = doc.dtUpload;
                        entry.decMaxScore = doc.decMaxScore;
                        entry.decTempScore = doc.decFinalScore;

                        // Fetch cohort score
                        var getCohort = hrentity.spGetCohort(
                            doc.vchDocMasName,
                            id,
                            Convert.ToInt32(doc.fk_intDeptid)
                        ).FirstOrDefault();

                        entry.decCohortScore = getCohort?.ContributionPercentage ?? 0;
                    }

                    finalModel.Add(entry);
                }
            }

            return View(finalModel);
        }
        //public ActionResult KRADetail(int id)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        //get all month
        //        List<tblMonthMaster> monthNames = (from e in hrentity.tblMonthMaster select e).ToList();
        //        List<tblMonthMaster> NameWithYear = new List<tblMonthMaster>();
        //        List<tblMonthMaster> objMonth = new List<tblMonthMaster>();
        //        string CurrentYear = DateTime.Now.ToString("yyyy");
        //        foreach (var month in monthNames)
        //        {
        //            if (month.Month != null)
        //            {
        //                month.Month = month.Month.ToString() + "-" + CurrentYear;
        //            }
        //            NameWithYear.Add(month);
        //        }
        //        //get all uploaded doc
        //        var getLisr = (from e in hrentity.tblKRADocDetail where e.fk_EmpID == id select e).ToList();

        //        foreach (var masterName in NameWithYear)
        //        {
        //            foreach (var up in getLisr)
        //            {
        //                if (masterName.Month == up.vchDocMasName)
        //                {
        //                    masterName.bitIsUploaded = true;
        //                    masterName.vchTempDocAddress = up.vchFileAddress;
        //                    masterName.vchTempFileName = up.vchFileName;
        //                    masterName.vchTempDocMasName = up.vchDocMasName;
        //                    masterName.dtTempUploaded = up.dtUpload;
        //                    masterName.decMaxScore = up.decMaxScore;
        //                    masterName.decTempScore = up.decFinalScore;
        //                    spGetCohort_Result getCohort = (from e in this.hrentity.spGetCohort(up.vchDocMasName.ToString(), new int?(Convert.ToInt32(up.fk_EmpID)), new int?(Convert.ToInt32(up.fk_intDeptid)))
        //                                                    select e).FirstOrDefault<spGetCohort_Result>();
        //                    if(getCohort!=null)
        //                    {
        //                        masterName.decCohortScore = getCohort.ContributionPercentage;
        //                    }

        //            else
        //                    {
        //                        masterName.decCohortScore = 0;
        //                    }
        //                }
        //            }
        //            masterName.intTempEmpID = id;
        //            objMonth.Add(masterName);
        //        }
        //        return View(objMonth);
        //    }

        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        public ActionResult UploadKRADoc(int id,string month)
        {
            var model = new UploadKRAModelView();
            {
                model.empid = id;
                model.Filename = month;
                //model.intYear = DateTime.Now.ToString("yyyy");
            }
            return PartialView("_partialKRAUpload", model);
        }

        [HttpPost]
        public ActionResult UploadKRADoc(UploadKRAModelView objnew)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int year = Convert.ToInt32(Session["yr"].ToString());
                    tblKRADocDetail objdetail = new tblKRADocDetail();
                    int empid = objnew.empid;
                    tblEmpAssesmentMas selecetdEmp = (from e in this.hrentity.tblEmpAssesmentMas
                                                      where e.intid == empid
                                                      select e).FirstOrDefault<tblEmpAssesmentMas>();
                    //check is doc uploaded or not
                    var checkdoc = (from e in hrentity.tblKRADocDetail where e.fk_EmpID == empid && e.vchDocMasName == objnew.DocMasName select e).FirstOrDefault();
                    if (checkdoc == null)
                    {
                        if (objnew.newpdfFile != null)
                        {
                            //string empid = empid.ToString();
                            string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                            string Final_datetime = datetime.Replace(" ", "");
                            string extension = Path.GetExtension(objnew.newpdfFile.FileName);
                            if (extension != ".pdf")
                            {
                                TempData["Error"] = "Please select .pdf file for upload!";
                                return RedirectToAction("KRaDetail", new { id = empid });
                            }
                            else
                            {
                                string filename = Path.GetFileNameWithoutExtension(objnew.newpdfFile.FileName);
                                string final_FileName = filename.Replace(" ", "");
                                string newfilename = final_FileName + Final_datetime + empid.ToString() + extension;
                                string finalfilename = newfilename.Replace(" ", "");
                                string path = Path.Combine(Server.MapPath("~/Content/Uploads/KRADoc/" + finalfilename));
                                //save file in upload folder
                                objnew.newpdfFile.SaveAs(path);
                                objdetail.fk_EmpID = empid;
                                objdetail.vchDocMasName = objnew.Filename;
                                objdetail.vchFileName = finalfilename.ToString();
                                objdetail.vchFileAddress = path.ToString();
                                objdetail.decFinalScore = objnew.decFinalScore;
                                objdetail.fk_intDeptid = Convert.ToInt32(selecetdEmp.fk_intdeptid);
                                objdetail.decMaxScore = 100;
                                objdetail.vchUpBy = base.Session["descript"].ToString();
                                objdetail.dtUpload = DateTime.Now;
                                objdetail.itncode = code;                                
                                hrentity.tblKRADocDetail.Add(objdetail);
                                hrentity.SaveChanges();
                                TempData["Success"] = "Document upload successfully!";
                                return RedirectToAction("KRaDetail", new { id = empid });
                            }

                        }
                        else
                        {
                            TempData["Error"] = "Upload an pdf file!";
                            return RedirectToAction("KRaDetail", new { id = empid });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Document already present in database check in view!";
                        return RedirectToAction("KRaDetail", new { id = empid });
                    }
                }
                else
                {
                    ModelState.AddModelError("vchFileName", "Model Error Generated please check it again!");
                    return View();
                }
            }

            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult DeleteKRADoc(int id, string docname)
        {
            if (Session["descript"] != null)
            {
                if (id != 0 && docname != null)
                {
                    var selectedDoc = (from e in hrentity.tblKRADocDetail where e.fk_EmpID == id && e.vchFileName == docname select e).FirstOrDefault();
                    if (selectedDoc != null)
                    {
                        System.IO.File.Delete(selectedDoc.vchFileAddress);
                        hrentity.tblKRADocDetail.Remove(selectedDoc);
                        hrentity.SaveChanges();
                        TempData["Success"] = "Document deleted successfully!";
                        return RedirectToAction("KRADetail", new { id = selectedDoc.fk_EmpID });
                    }
                    else
                    {
                        TempData["Error"] = "Document details not found please check it again and try!";
                        return RedirectToAction("KRADetail", new { id = id });
                    }
                }
                else
                {
                    TempData["Error"] = "Document id should not be null or 0";
                    return RedirectToAction("KRADetail");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
        #endregion

        #region Upload consultant ledger
        public ActionResult IndexLedger(int id)
        {
            if (Session["descript"] != null)
            {
                //get all month
                List<tblLedgerSession> LedgerSession = (from e in hrentity.tblLedgerSession select e).ToList();              
                List<tblLedgerSession> uploadedLedger = new List<tblLedgerSession>();              
                //get all uploaded doc
                var getLisr = (from e in hrentity.tblConsultantLedgerDetail where e.fk_EmpID == id select e).ToList();
                foreach (var masterName in LedgerSession)
                {
                    foreach (var up in getLisr)
                    {
                        if (masterName.intID == up.fk_LedgerMas)
                        {
                            masterName.TempBitIsUploaded = true;
                            masterName.vchTempDocAddress = up.vchFileAddress;
                            masterName.vchTempFileName = up.vchFileName.ToString();
                            masterName.vchTempDocMasName = up.vchDocMasName;
                            masterName.dtTempUploaded = up.dtUpload;
                        }
                    }
                    masterName.intTempEmpid = id;
                    uploadedLedger.Add(masterName);
                }
                return View(uploadedLedger);
            }

            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult uploadLedger(int id,string month)
        {
            var model = new UploadLedgerViewModel();
            {
                model.DocMasName = month;
                model.empid = id;               
            }
            return PartialView("_PartialLedger", model);
        }

        [HttpPost]
        public ActionResult uploadLedger(UploadLedgerViewModel objnew)
        {
            if (ModelState.IsValid)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int year = Convert.ToInt32(Session["yr"].ToString());
                tblConsultantLedgerDetail objdetail = new tblConsultantLedgerDetail();
                int empid = objnew.empid;
                //check is doc uploaded or not
                var checkdoc = (from e in hrentity.tblConsultantLedgerDetail where e.fk_EmpID == empid && e.vchDocMasName == objnew.DocMasName select e).FirstOrDefault();
                if (checkdoc == null)
                {
                    if (objnew.newpdfFile != null)
                    {
                        //string empid = empid.ToString();
                        string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                        string Final_datetime = datetime.Replace(" ", "");
                        string extension = Path.GetExtension(objnew.newpdfFile.FileName);
                        if (extension != ".pdf")
                        {
                            TempData["Error"] = "Please select .pdf file for upload!";
                            return RedirectToAction("IndexLedger", new { id = empid });
                        }
                        else
                        {
                            string filename = Path.GetFileNameWithoutExtension(objnew.newpdfFile.FileName);
                            string final_FileName = filename.Replace(" ", "");
                            string newfilename = final_FileName + Final_datetime + empid.ToString() + extension;
                            string finalfilename = newfilename.Replace(" ", "");
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/ConsultantLedger/" + finalfilename));
                            //save file in upload folder
                            objnew.newpdfFile.SaveAs(path);
                            objdetail.fk_EmpID = empid;
                            //get ledger session mas
                            var getLedgerSession = (from e in hrentity.tblLedgerSession where e.vchSession == objnew.DocMasName select e).FirstOrDefault();
                            objdetail.fk_LedgerMas = getLedgerSession.intID;
                            objdetail.vchDocMasName = objnew.DocMasName;                          
                            objdetail.vchFileName = finalfilename;
                            objdetail.vchFileAddress = path.ToString();
                            objdetail.vchUpBy = Session["descript"].ToString();
                            objdetail.dtUpload = DateTime.Now;
                            objdetail.intYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                            //create GUID
                            objdetail.UUID = Guid.NewGuid();
                            objdetail.intCode = code;
                            try
                            {
                                hrentity.tblConsultantLedgerDetail.Add(objdetail);
                                hrentity.SaveChanges();
                                TempData["Success"] = "Document upload successfully!";
                                return RedirectToAction("IndexLedger", new { id = empid });
                            }
                            catch (DbEntityValidationException ex)
                            {
                                //Retrieve the validation errors
                                foreach (var validationErrors in ex.EntityValidationErrors)
                                {
                                    foreach (var validationError in validationErrors.ValidationErrors)
                                    {
                                        TempData["Error"] = ("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                        return RedirectToAction("IndexLedger", new { id = empid });
                                    }
                                  
                                }
                            }
                            return RedirectToAction("IndexLedger", new { id = empid });
                        }
                        
                    }
                    else
                    {
                        TempData["Error"] = "Upload an pdf file!";
                        return RedirectToAction("IndexLedger", new { id = empid });
                    }
                }
                else
                {
                    TempData["Error"] = "Document already present in database check in view!";
                    return RedirectToAction("IndexLedger", new { id = empid });
                }
            }
            else
            {
                ModelState.AddModelError("vchFileName", "Model Error Generated please check it again!");
                return View();
            }
        }

        public ActionResult DeleteLedger(int id, string docname)
        {
            if (Session["descript"] != null)
            {
                if (id != 0 && docname != null)
                {
                    var selectedDoc = (from e in hrentity.tblConsultantLedgerDetail where e.fk_EmpID == id && e.vchFileName == docname select e).FirstOrDefault();
                    if (selectedDoc != null)
                    {
                        System.IO.File.Delete(selectedDoc.vchFileAddress);
                        hrentity.tblConsultantLedgerDetail.Remove(selectedDoc);
                        hrentity.SaveChanges();
                        TempData["Success"] = "Document deleted successfully!";
                        return RedirectToAction("IndexLedger", new { id = selectedDoc.fk_EmpID });
                    }
                    else
                    {
                        TempData["Error"] = "Document details not found please check it again and try!";
                        return RedirectToAction("IndexLedger", new { id = id });
                    }
                }
                else
                {
                    TempData["Error"] = "Document id should not be null or 0";
                    return RedirectToAction("IndexLedger");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Consultant TDS Upload
        public ActionResult TDSIndex(int id)
        {
            if (Session["descript"] != null)
            {
                //get all month
                List<tblQuraterMas> qutrMas = (from e in hrentity.tblQuraterMas select e).ToList();
                List<tblQuraterMas> NewQutrMas = new List<tblQuraterMas>();
                //get all uploaded doc
                var getLisr = (from e in hrentity.tblConsultantTDS where e.fk_EMPID == id select e).ToList();
                foreach (var quraterMas in qutrMas)
                {
                    foreach (var up in getLisr)
                    {
                        if (quraterMas.intid == up.fk_QuraterMas)
                        {
                            quraterMas.TempBitIsUploaded = true;
                            quraterMas.vchTempDocAddress = up.vchFileAddress;
                            quraterMas.vchTempFileName = up.vchFileName.ToString();
                            quraterMas.vchTempDocMasName = up.vchDocMasName;
                            quraterMas.dtTempUploaded = up.dtUpload;
                            quraterMas.vchTempYear = up.intYear;
                        }
                    }
                    quraterMas.intTempEmpid = id;
                    NewQutrMas.Add(quraterMas);
                }
                return View(NewQutrMas);
            }

            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult uploadTDS(int id, string month)
        {
            var model = new UploadKRAModelView();
            {
                model.DocMasName = month;
                model.empid = id;
            }           
            return PartialView("_PartialTDSUpload", model);
        }

        [HttpPost]
        public ActionResult uploadTDS(UploadKRAModelView objnew)
        {
            if (ModelState.IsValid)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int year = Convert.ToInt32(Session["yr"].ToString());
                tblConsultantTDS objdetail = new tblConsultantTDS();
                int empid = objnew.empid;
                //check is doc uploaded or not
                var checkdoc = (from e in hrentity.tblConsultantTDS where e.fk_EMPID == empid && e.vchDocMasName == objnew.DocMasName select e).FirstOrDefault();
                if (checkdoc == null)
                {
                    if (objnew.newpdfFile != null)
                    {
                        //string empid = empid.ToString();
                        string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                        string Final_datetime = datetime.Replace(" ", "");
                        string extension = Path.GetExtension(objnew.newpdfFile.FileName);
                        if (extension != ".pdf")
                        {
                            TempData["Error"] = "Please select .pdf file for upload!";
                            return RedirectToAction("TDSIndex", new { id = empid });
                        }
                        else
                        {
                            string filename = Path.GetFileNameWithoutExtension(objnew.newpdfFile.FileName);
                            string final_FileName = filename.Replace(" ", "");
                            string newfilename = final_FileName + Final_datetime + empid.ToString() + extension;
                            string finalfilename = newfilename.Replace(" ", "");
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/ConsultantTDS/" + finalfilename));
                            //save file in upload folder
                            objnew.newpdfFile.SaveAs(path);
                            objdetail.fk_EMPID = empid;
                            //qurater mas
                            var getMas = (from e in hrentity.tblQuraterMas where e.vchMonth == objnew.DocMasName select e).FirstOrDefault();
                            objdetail.vchDocMasName = getMas.vchMonth;
                            objdetail.fk_QuraterMas = getMas.intid;
                            objdetail.vchDocMasName = objnew.DocMasName;
                            objdetail.vchFileName = finalfilename;
                            objdetail.vchFileAddress = path.ToString();
                            objdetail.vchUpBy = Session["descript"].ToString();
                            objdetail.dtUpload = DateTime.Now;
                            objdetail.intYear = objnew.intYear;
                            //create GUID
                            objdetail.UID = Guid.NewGuid();
                            objdetail.intCode = code;
                            try
                            {
                                hrentity.tblConsultantTDS.Add(objdetail);
                                hrentity.SaveChanges();
                                TempData["Success"] = "Document upload successfully!";
                                return RedirectToAction("TDSIndex", new { id = empid });
                            }
                            catch (DbEntityValidationException ex)
                            {
                                //Retrieve the validation errors
                                foreach (var validationErrors in ex.EntityValidationErrors)
                                {
                                    foreach (var validationError in validationErrors.ValidationErrors)
                                    {
                                        TempData["Error"] = ("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                        return RedirectToAction("TDSIndex", new { id = empid });
                                    }
                                }
                            }
                            return RedirectToAction("TDSIndex", new { id = empid });
                        }

                    }
                    else
                    {
                        TempData["Error"] = "Upload an pdf file!";
                        return RedirectToAction("TDSIndex", new { id = empid });
                    }
                }
                else
                {
                    TempData["Error"] = "Document already present in database check in view!";
                    return RedirectToAction("TDSIndex", new { id = empid });
                }
            }
            else
            {
                ModelState.AddModelError("vchFileName", "Model Error Generated please check it again!");
                return View();
            }
        }

        public ActionResult DeleteTDS(int id, string docname)
        {
            if (Session["descript"] != null)
            {
                if (id != 0 && docname != null)
                {
                    var selectedDoc = (from e in hrentity.tblConsultantTDS where e.fk_EMPID == id && e.vchFileName == docname select e).FirstOrDefault();
                    if (selectedDoc != null)
                    {
                        System.IO.File.Delete(selectedDoc.vchFileAddress);
                        hrentity.tblConsultantTDS.Remove(selectedDoc);
                        hrentity.SaveChanges();
                        TempData["Success"] = "Document deleted successfully!";
                        return RedirectToAction("TDSIndex", new { id = selectedDoc.fk_EMPID });
                    }
                    else
                    {
                        TempData["Error"] = "Document details not found please check it again and try!";
                        return RedirectToAction("TDSIndex", new { id = id });
                    }
                }
                else
                {
                    TempData["Error"] = "Document id should not be null or 0";
                    return RedirectToAction("TDSIndex");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
        #endregion

        #region TDS Bulk Upload
        // GET: Upload Page
        public ActionResult BulkUploadTDS()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckPANMatch(IEnumerable<HttpPostedFileBase> files, int selectedQuarter, string intYear)
        {
            if (files == null || !files.Any())
            {
                return Json(new { success = false, message = "No files selected." });
            }
            HRMEntities db = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            var matchedFiles = new List<string>();
            var unmatchedFiles = new List<string>();
            var validFiles = new List<HttpPostedFileBase>();

            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string[] parts = fileName.Split('_');

                if (parts.Length < 2) continue;

                string panNumber = parts[1];
                var consultant = db.tblEmpAssesmentMas.FirstOrDefault(e => e.vchPANNUmber == panNumber && e.intcode==code);

                if (consultant != null)
                {
                    matchedFiles.Add(file.FileName);
                    validFiles.Add(file);
                }
                else
                {
                    unmatchedFiles.Add(file.FileName);
                }
            }
            return Json(new
            {
                success = true,
                matchedPANs = matchedFiles,
                unmatchedPANs = unmatchedFiles
            });
        }

        [HttpPost]
        public ActionResult UploadConfirmedTDS(IEnumerable<HttpPostedFileBase> matchedFiles, int selectedQuarter, string intyear)
        {
            if (matchedFiles == null || !matchedFiles.Any())
            {
                return Json(new { success = false, message = "No matched files available for upload." });
            }
            HRMEntities db = new HRMEntities();
            string uploadPath = Server.MapPath("~/Content/Uploads/ConsultantTDS/");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            foreach (var file in matchedFiles)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                string fileName = Path.GetFileName(file.FileName);
                string[] parts = Path.GetFileNameWithoutExtension(fileName).Split('_');
                if (parts.Length < 2) continue; // Skip invalid format files
                string panNumber = parts[1];
                var consultant = db.tblEmpAssesmentMas.FirstOrDefault(e => e.vchPANNUmber == panNumber && e.intcode==code);
                if (consultant != null)
                {
                    //Remove existing TDS file
                    var existingFile = db.tblConsultantTDS.FirstOrDefault(e => e.fk_EMPID == consultant.intid && e.fk_QuraterMas == selectedQuarter);
                    if (existingFile != null)
                    {
                        string existingFilePath = Path.Combine(uploadPath, existingFile.vchFileName);
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                        db.tblConsultantTDS.Remove(existingFile);
                    }
                    //Save new file
                    string newFilePath = Path.Combine(uploadPath, fileName);
                    file.SaveAs(newFilePath);
                    //Update Database
                    var newTDS = new tblConsultantTDS
                    {
                        fk_EMPID = consultant.intid,
                        vchFileAddress = newFilePath,
                        vchFileName = fileName,
                        fk_QuraterMas = selectedQuarter,
                        vchUpBy = "System", // Replace with logged-in user
                        dtUpload = DateTime.Now,
                        intYear = intyear,
                        UID = Guid.NewGuid()
                    };
                    db.tblConsultantTDS.Add(newTDS);
                    db.SaveChanges();
                }
            }
            return Json(new { success = true, message = "TDS forms uploaded successfully against matched PAN number only!" });
        }
        #endregion

        #region Enter consultant PAN number

        public ActionResult GetDetailForPAN(int id)
        {
            int code = Convert.ToInt32(base.Session["id"].ToString());
            if (id != 0)
            {
                AddPANNumber objPAN = new AddPANNumber();
                var getEmp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                if (getEmp != null)
                {
                    objPAN.id = getEmp.intid;                    
                    return PartialView("_PartialUpPAN", objPAN); 
                }
                else
                {
                    TempData["Error"] = "Consultant detail not found please check it again and try!";
                    return RedirectToAction("FullConsultView");
                }
            }
            else
            {
                TempData["Error"] = "Consultant detail not found please check it again and try!";
                return RedirectToAction("FullConsultView");
            }
        }

        public ActionResult _PartialUpPAN()
        {
            if (base.Session["UserId"] != null)
            {
                return base.View();
            }
            return base.RedirectToAction("_SessionError1", "Home");
        }
        [HttpPost]
        public ActionResult UpdatePAN(AddPANNumber ojmas)
        {
            HRMEntities db = new HRMEntities();
            int id = ojmas.id;
            var data = db.tblEmpAssesmentMas.Find(id);
            if (data != null)
            {
                data.vchPANNUmber = ojmas.PanNumber;
                db.SaveChanges();
                TempData["Success"] = "PAN number saved successfully!";
            }
            return RedirectToAction("FullConsultView",new { id = id });
        }
        #endregion

        #region more upload for active consultant
        public ActionResult MoreDocUpload(int id)
        {
            if (Session["descript"] != null)
            {
                //mas object
                List<tblDoctorDocMas> objdoc = new List<tblDoctorDocMas>();
                var getOtherDocumentMas = (from e in hrentity.tblDoctorDocMas select e).ToList();
                var getDocDetail = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == id select e).ToList();
                if (getOtherDocumentMas.Count() != 0)
                {
                    foreach (var doc in getOtherDocumentMas)
                    {
                        foreach (var uploaded in getDocDetail)
                        {
                            if (doc.intid == uploaded.fk_DocMasId)
                            {
                                doc.bitIsSelected = true;
                                doc.vchCompareFileAdd = uploaded.vchDocAddress;
                                doc.dtTempUploaded = uploaded.dtCreated;
                                doc.vchTempFileName = uploaded.vchFileName;                                
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

        public ActionResult UpMoreDoc(int id, int docid)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                var selectedDoc = (from e in hrentity.tblDoctorDocMas where e.intid == docid select e).FirstOrDefault();
                if (selectedDoc != null)
                {
                    ViewBag.Docname = selectedDoc.vchDocMasName.ToString();
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
        public ActionResult UpMoreDoc(UploadOtherDocument objup)
        {
            if (Session["descript"] != null)
            {
                //if (ModelState.IsValid)
                //{
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int year = Convert.ToInt32(Session["yr"].ToString());
                    tblConsultantDocDetail objdocdetail = new tblConsultantDocDetail();
                    int empid = objup.empid;
                    //get all emp id qualifications
                    int docid = objup.fk_docid;
                    //get mas doc
                    var MasDoc = (from e in hrentity.tblDoctorDocMas where e.intid == docid select e).FirstOrDefault();
                    //get mas data
                    var empmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    //check is doc uploaded or not
                    var checkdoc = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == empid && e.fk_DocMasId == docid && e.vchDocName == objup.Name select e).FirstOrDefault();
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
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + finalfilename));
                            //save file in upload folder
                            objup.newpdfFile.SaveAs(path);
                            objdocdetail.fk_DoctorID = empid;
                            objdocdetail.fk_DocMasId = docid;
                            objdocdetail.vchDocName = MasDoc.vchDocMasName.ToString();
                            //objdocdetail.vchDocName = objup.Name;
                            objdocdetail.vchFileName = finalfilename.ToString();
                            objdocdetail.vchDocAddress = path.ToString();
                            objdocdetail.vchCreatedBy = Session["descript"].ToString();
                            objdocdetail.dtCreated = DateTime.Now;
                            objdocdetail.vchCretaedIP = Session["ipused"].ToString();
                            objdocdetail.vchCreatedHost = Session["hostname"].ToString();
                            hrentity.tblConsultantDocDetail.Add(objdocdetail);
                            hrentity.SaveChanges();
                            TempData["Success"] = "Document upload successfully!";
                            return RedirectToAction("MoreDocUpload", new { id = empid });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Select pdf file for upload!";
                        return View("MoreDocUpload", new { id = objup.empid });
                    }
                //}

                //else
                //{
                //    ModelState.AddModelError("Name", "Model Error generated try again or contact to administrator!");
                //    return View();
                //}
            }
            else
            {
                return RedirectToAction("SessionError1", "Home");
            }
        }
        #endregion

        #region Consultant experience
        public ActionResult ViewConsultantExpAll()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var LetterList = (from e in hrentity.tblExperienceDetail join m in hrentity.tblEmpAssesmentMas on e.intHRMSid equals m.intid
                                  where m.bitIsConsultant == true && m.bitExpLetter == true && m.intcode == code orderby e.dtCreated descending select e).ToList();
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

        public ActionResult NewConsultantExp()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                //select employee code which not red flag or not active employee
                var allConsuktantForExp = (from e in hrentity.tblEmpAssesmentMas
                                        join d in hrentity.tblDoctorUploadDetail on e.intid equals d.fk_ConsultMasId 
                 where e.bitIsConsultant==true && d.bitIsLeft==true && e.bitExpLetter!=true && e.intcode==code select e).ToList();
                List<SelectListItem> allConsultant = new List<SelectListItem>();
                if (allConsuktantForExp != null)
                {
                    foreach (var emp in allConsuktantForExp)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = emp.intid.ToString(),
                            Text = emp.vchEmpFcode
                        };
                        allConsultant.Add(selectListItem);
                    }
                    ViewBag.Consultants = new SelectList(allConsultant, "Value", "Text");
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
                var selectMaster = (from e in hrentity.tblExperienceMas select e).ToList();
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
                var selDept = (from e in hrentity.tblDeptMas select e).ToList();
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
                var selectDesig = (from e in hrentity.tblDesignationMas select e).ToList();
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
                var checkletter = (from e in hrentity.tblExperienceDetail where e.intHRMSid == id select e).FirstOrDefault();
                if (checkletter == null)
                {
                    var selectedMas = (from e in hrentity.tblEmpAssesmentMas
                                       join d in hrentity.tblEmpDetails on e.intid equals d.fk_intempid
                                       where e.intid == id
                                       select new { e.vchName, e.vchEmpFcode, e.fk_intdeptid, e.fk_intdesiid, d.vchFatherName, d.vchpcity, d.vchpstate, d.vchpaddress, d.vchsex, e.dtDOJ, e.dtDOL }).FirstOrDefault();
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
                            dtDOJ = selectedMas.dtDOJ,
                            dtDOL = selectedMas.dtDOL
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
                var selectedMas = (from e in hrentity.tblExperienceMas where e.intid == newid select e).FirstOrDefault();
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
        public ActionResult NewConsultantExp(tblExperienceDetail newobj)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                //Select master from HRMS for update status as experience letter is prepared to employee
                tblEmpAssesmentMas objempmas = new tblEmpAssesmentMas();
                if (newobj.intHRMSid != 0 && newobj.intHRMSid != null)
                {
                    var Selected_HrmsEmp = (from e in hrentity.tblEmpAssesmentMas where e.intid == newobj.intHRMSid select e).FirstOrDefault();
                    if (Selected_HrmsEmp != null)
                    {
                        objempmas = Selected_HrmsEmp;
                    }
                }
                //for select master type letter used in dropdownlist return view
                var selectMaster = (from e in hrentity.tblExperienceMas select e).ToList();
                //Declare for master selected object
                tblExperienceMas objExpMas = new tblExperienceMas();
                //Get Master Experience Selected
                if (newobj != null && newobj.fk_Masid != 0)
                {
                    int expMasId = Convert.ToInt32(newobj.fk_Masid.ToString());
                    objExpMas = (from e in hrentity.tblExperienceMas where e.intid == expMasId select e).FirstOrDefault();
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
                    var selDept = (from e in hrentity.tblDeptMas select e).ToList();
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
                    var selectDesig = (from e in hrentity.tblDesignationMas select e).ToList();
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
                    objdesi = (from e in hrentity.tblDesignationMas where e.intid == desiID select e).FirstOrDefault();
                }
                int deptID = Convert.ToInt32(objdesi.intdeptid);
                var getDeptCode = (from e in hrentity.tblDeptMas where e.intid == deptID select e).FirstOrDefault();
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
                    var selectDesig = (from e in hrentity.tblDesignationMas select e).ToList();
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
                var getCode = (from e in hrentity.tblLetterNumberMas where e.intcode == code && e.intYear == year1 select e).FirstOrDefault();
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
                    if (objempmas != null)
                    {
                        objempmas.bitExpLetter = true;
                        newobj.vchEmpCode = objempmas.vchEmpFcode;
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
                        hrentity.tblExperienceDetail.Add(newobj);
                        hrentity.SaveChanges();
                        TempData["Success"] = "Certificate saved successfully!";
                        return RedirectToAction("NewConsultantExp");
                    }
                    catch (DbEntityValidationException ex)
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
                        return RedirectToAction("NewConsultantExp");
                    }
                }
                else
                {
                    //Select All Department For selection department
                    var selectedDept = (from e in hrentity.tblDeptMas select e).ToList();
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
                    return RedirectToAction("NewConsultantExp");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ConsultantExpEdit(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                if (id != 0)
                {
                    var selecetd = (from e in hrentity.tblExperienceDetail where e.intid == id select e).FirstOrDefault();
                    if (selecetd != null)
                    {
                        int fk_EmpAssId = Convert.ToInt32(selecetd.intHRMSid);
                        //select employee code which not red flag or not active employee
                        var allConsultantForExp = (from e in hrentity.tblEmpAssesmentMas join d in hrentity.tblDoctorUploadDetail on e.intid equals d.fk_ConsultMasId
                                                 where d.bitIsLeft==true && e.intcode == code
                                                 select e).ToList();
                        List<SelectListItem> allConsultants = new List<SelectListItem>();
                        if (allConsultantForExp != null)
                        {
                            foreach (var emp in allConsultantForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allConsultants.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allConsultants, "Value", "Text");
                        }
                        //for department selection
                        var selDept = (from e in hrentity.tblDeptMas select e).ToList();
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
                        var selectDesig = (from e in hrentity.tblDesignationMas select e).ToList();
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
                        var selectMaster = (from e in hrentity.tblExperienceMas select e).ToList();
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
                        var allConsultantForExp = (from e in hrentity.tblEmpAssesmentMas
                                                   join d in hrentity.tblDoctorUploadDetail on e.intid equals d.fk_ConsultMasId
                                                   where d.bitIsLeft == true && e.intcode == code
                                                   select e).ToList();
                        List<SelectListItem> allconsultant = new List<SelectListItem>();
                        if (allConsultantForExp != null)
                        {
                            foreach (var emp in allConsultantForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allconsultant.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allconsultant, "Value", "Text");
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
                        var selDept = (from e in hrentity.tblDeptMas select e).ToList();
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
                        var selectDesig = (from e in hrentity.tblDesignationMas select e).ToList();
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
                        var selectMaster = (from e in hrentity.tblExperienceMas select e).ToList();
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
                    var allConsultantForExp = (from e in hrentity.tblEmpAssesmentMas
                                               join d in hrentity.tblDoctorUploadDetail on e.intid equals d.fk_ConsultMasId
                                               where d.bitIsLeft == true && e.intcode == code
                                               select e).ToList();
                    List<SelectListItem> allConsultant = new List<SelectListItem>();
                    if (allConsultantForExp != null)
                    {
                        foreach (var emp in allConsultantForExp)
                        {
                            SelectListItem selectListItem = new SelectListItem
                            {
                                Value = emp.intid.ToString(),
                                Text = emp.vchEmpFcode
                            };
                            allConsultant.Add(selectListItem);
                        }
                        ViewBag.Employees = new SelectList(allConsultant, "Value", "Text");
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
                    var selDept = (from e in hrentity.tblDeptMas select e).ToList();
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
                    var selectDesig = (from e in hrentity.tblDesignationMas select e).ToList();
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
                    var selectMaster = (from e in hrentity.tblExperienceMas select e).ToList();
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
        public ActionResult ConsultantExpEdit(tblExperienceDetail objedit)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                //HrmsEmpMas Object
                tblEmpAssesmentMas objEmpmas = new tblEmpAssesmentMas();
                tblEmpAssesmentMas OldHRMSEmp = new tblEmpAssesmentMas();
                if (objedit != null)
                {
                    var deptMas = (from e in hrentity.tblDeptMas where e.intid == objedit.fk_department select e).FirstOrDefault();
                    var selectedObj = (from e in hrentity.tblExperienceDetail where e.intid == objedit.intid select e).FirstOrDefault();
                    if (selectedObj != null)
                    {

                        //get empHRMS Mas and update letter details if  
                        if (objedit.intHRMSid != 0 && objedit.intHRMSid != null)
                        {
                            objEmpmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == objedit.intHRMSid select e).FirstOrDefault();
                            if (objEmpmas != null)
                            {
                                //check employee updated?
                                if (objEmpmas.intid != selectedObj.intHRMSid)
                                {
                                    if (selectedObj.intHRMSid != null && selectedObj.intHRMSid != 0)
                                    {
                                        //now change old hrms emp status
                                        OldHRMSEmp = (from e in hrentity.tblEmpAssesmentMas where e.intid == selectedObj.intHRMSid select e).FirstOrDefault();
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
                        hrentity.SaveChanges();
                        TempData["Success"] = "Certificate detail updated successfully!";
                        return RedirectToAction("ViewConsultantExpAll");
                    }
                    else
                    {
                        var allConsultantForExp = (from e in hrentity.tblEmpAssesmentMas
                                                 where e.bittempstatusactive == true && e.bitstatusdeactive == false && e.BitIsRedFlagging == false
                                                 && e.isHrmsEmployee == true && e.intcode == code
                                                 select e).ToList();
                        List<SelectListItem> allConsultant = new List<SelectListItem>();
                        if (allConsultantForExp != null)
                        {
                            foreach (var emp in allConsultantForExp)
                            {
                                SelectListItem selectListItem = new SelectListItem
                                {
                                    Value = emp.intid.ToString(),
                                    Text = emp.vchEmpFcode
                                };
                                allConsultant.Add(selectListItem);
                            }
                            ViewBag.Employees = new SelectList(allConsultant, "Value", "Text");
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
                        var selDept = (from e in hrentity.tblDeptMas select e).ToList();
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
                        var selectDesig = (from e in hrentity.tblDesignationMas select e).ToList();
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
                        var selectMaster = (from e in hrentity.tblExperienceMas select e).ToList();
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
                    var allEmployeeForExp = (from e in hrentity.tblEmpAssesmentMas
                                             join d in hrentity.tblDoctorUploadDetail on e.intid equals d.fk_ConsultMasId
                                             where d.bitIsLeft == true && e.intcode == code
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
                    var selDept = (from e in hrentity.tblDeptMas select e).ToList();
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
                    var selectDesig = (from e in hrentity.tblDesignationMas select e).ToList();
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
                    var selectMaster = (from e in hrentity.tblExperienceMas select e).ToList();
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

        public ActionResult ConsultantExperiencePrint(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selectedCertificate = (from e in hrentity.tblExperienceDetail
                                               join m in hrentity.tblExperienceMas on e.fk_Masid equals m.intid
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
                        if (selectedCertificate.vchGender == "Male" && selectedCertificate.tblExperienceMas.vchType == "Admin")
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
                        var selectedobj = (from e in hrentity.spEmpExpLetter(id) select e).ToList();
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
    }
}