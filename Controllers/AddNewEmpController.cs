using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Deployment.Internal;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using HRM.Models;
using System.IO;
using System.Globalization;
using System.Web.ModelBinding;
//using Microsoft.Ajax.Utilities;

namespace HRM.Controllers
{
    public class AddNewEmpController : Controller
    {
        HRMEntities objentity = new HRMEntities();

        //Add Basic Details
        public ActionResult OfficialDetails(int id)
        {
            if (Session["descript"] != null)
            {
                partialaddofficial(id);
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult partialaddofficial(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int year = Convert.ToInt32(Session["yr"].ToString());
                if (id != 0)
                {                   
                    var selecetdemp = (from e in objentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code
                                       join d in objentity.tblDeptMas on e.fk_intdeptid equals d.intid select e).FirstOrDefault();
                    if (selecetdemp != null)
                    {
                        ViewBag.Name = selecetdemp.vchName;
                        ViewBag.Position = selecetdemp.tblPositionCategoryMas.vchPosCatName;
                        ViewBag.Contact = selecetdemp.vchMobile;
                        ViewBag.ID = selecetdemp.intid.ToString();
                        ViewBag.EmpTcode = selecetdemp.vchEmpTcode.ToString();
                        ViewBag.Deprt = selecetdemp.tblDeptMas.vchdeptname;
                        ViewBag.DPTID = selecetdemp.fk_intdeptid.ToString();
                        //DateTime dt = Convert.ToDateTime(selecetdemp.dtcreated.Value.ToString("dd/MM/yyyy"));
                        ViewBag.DtCreated = selecetdemp.dtcreated.Value.ToString("dd/MM/yyyy");
                        OfficialDetailsModelView objnew = new OfficialDetailsModelView();
                        objnew.Empid = selecetdemp.intid;
                        objnew.bitoffdetail = selecetdemp.bitofficialdetails;
                        objnew.fk_dptid = selecetdemp.fk_intdeptid.ToString();
                        //call all department
                        var deptlist = objentity.tblDeptMas.ToList();
                        //List<SelectListItem> newlist = new List<SelectListItem>();
                        //newlist.Add(new SelectListItem { Text = "Select department", Value = "0" });
                        //foreach (var dpt in deptlist)
                        //{
                        //    SelectListItem selectListItem = new SelectListItem
                        //    {
                        //        Text = dpt.vchdeptname,
                        //        Value = dpt.intid.ToString()
                        //    };
                        //    newlist.Add(selectListItem);
                        //}
                        //ViewBag.DeptList = new SelectList(newlist, "Text", "Value");
                        //_SelectTitle();
                        //_getdept();
                        //_getdesi();
                        //partialdateview(id);
                        return View();

                    }
                    else
                    {
                        TempData["Error"] = "Selected employee details not found in database please check again it!";
                        return View();
                    }
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

        public ActionResult partialdateview(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var empmas = (from e in objentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                DateRemarksViewModel objmodel = new DateRemarksViewModel();
                ViewBag.EMPID = empmas.intid.ToString();
                ViewBag.CreatedDate = empmas.dtcreated.ToString();
                objmodel.id = empmas.intid;
                return View(objmodel);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //Add post Official Details      
        [HttpPost]
        public ActionResult OfficialDetails(OfficialDetailsModelView newdata, FormCollection fc)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            if (Session["descript"] != null)
            {
                //int empid = Convert.ToInt32(fmcollect.Get("EmpID"));
                int empid = newdata.Empid;
                int formdesiid = Convert.ToInt32(fc.Get("selecteddesi"));
                //int newDeptId = newdata.fk_dptid;
                var selectedDesi = (from e in objentity.tblDesignationMas where e.intid == formdesiid select e).FirstOrDefault();
                var selecetdemp = (from e in objentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                if (selecetdemp != null)
                {
                    string datecreated = selecetdemp.dtcreated.Value.ToString("dd/MM/yyyy");
                    string newdate = newdata.dtDOJ.ToString("dd/MM/yyyy");
                    DateTime dateCreation = DateTime.ParseExact(datecreated.Trim().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime NewDOJ = DateTime.ParseExact(newdate.Trim().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (NewDOJ >= dateCreation)
                    {
                        selecetdemp.dtDOJ = Convert.ToDateTime(newdata.dtDOJ);
                        //selecetdemp.fk_intdeptid = Convert.ToInt32(newdata.fk_dptid);
                        int desiid = selectedDesi.intid;
                        selecetdemp.fk_intdesiid = formdesiid;
                        selecetdemp.intsalary = newdata.intsalary;
                        //selecetdemp.bittofficialdetails = true;
                        if (newdata.bitoffdetail == true)
                        {
                            selecetdemp.bittofficialdetails = true;
                            selecetdemp.bitofficialdetails = true;
                        }
                        else if (newdata.bitoffdetail == false)
                        {
                            selecetdemp.bittofficialdetails = true;
                            selecetdemp.bitofficialdetails = false;
                        }

                        objentity.SaveChanges();
                        TempData["Success"] = "Official details added successfully!";
                        return RedirectToAction("NewAuthorization", "Authorization");
                    }
                    else
                    {
                        //call all department
                        var deptlist = objentity.tblDeptMas.ToList();
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
                        TempData["Error"] = "DOJ should not be less than Created date!";
                        return RedirectToAction("OfficialDetails");
                    }                   
                }
                else
                {
                    //call all department
                    var deptlist = objentity.tblDeptMas.ToList();
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
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        // view all employee newly added
       public ActionResult ViewAllEmp()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            if (ModelState.IsValid)
            {
                var Allemplist = (from e in objentity.tblEmpAssesmentMas
                                              where e.bitofficialdetails == false && e.bittofficialdetails==true && e.intcode==code select e).ToList();
            
                    return View(Allemplist);
            }
            else
            {
                return View();
            }
            
        }

        //Edit newly emp
        public ActionResult EditEmpl(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            var selectedEmp = (from e in objentity.tblEmpAssesmentMas where e.intid == id && e.intcode==code select e).FirstOrDefault();
            string titname = selectedEmp.vchName;
            int titid = Convert.ToInt32(selectedEmp.fk_inttitid);
            int dptid = Convert.ToInt32(selectedEmp.fk_intdeptid);
            int desiid = Convert.ToInt32(selectedEmp.fk_intdesiid);
            if (selectedEmp == null)
            {
                return HttpNotFound();
            }
            else
            {
                //_edittitile(titid, id);
                _editdept(dptid, id);
                _editdesignation(desiid, id);
                //OfficialDetailsModelView objmodel = new OfficialDetailsModelView();
                //objmodel.fk_dptid = Convert.ToInt32(selectedEmp.fk_intdeptid);
                //objmodel.fk_desiid = Convert.ToInt32(selectedEmp.fk_intdesiid);
                //objmodel.dtDOJ = Convert.ToDateTime(selectedEmp.dtDOJ);
                //objmodel.intsalary = Convert.ToInt16(selectedEmp.intsalary);
                //objmodel.bitoffdetail = selectedEmp.bitofficialdetails;
                return View(selectedEmp);
            }
        }
        //edit tittle function
        public ActionResult _edittitile(int qid, int empid)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectdtit = (objentity.tblTitleMas.Where(m => m.intid == qid)).ToList();
            var empmastit = (objentity.tblEmpAssesmentMas.Where(m => m.intid == empid && m.intcode==code)).ToList();
            if (selectdtit == null)
            {
                return HttpNotFound();
            }
            else
            {
                List<string> selctedstring = new List<string>();
                //List<SelectListItem> selectedtit = new List<SelectListItem>();
                List<SelectListItem> listSelectListItem = new List<SelectListItem>();
                foreach (var title in objentity.tblTitleMas)
                {
                    SelectListItem selectListItem = new SelectListItem()
                    {
                        Text = title.vchname.ToString(),
                        Value = title.intid.ToString(),
                        Selected = Convert.ToBoolean(title.IsSelected)
                    };
                    listSelectListItem.Add(selectListItem);

                }
                foreach (var titselctd in empmastit)
                {
                    SelectListItem selectListItem = new SelectListItem()
                    {
                        Text = titselctd.vchName.ToString(),
                        Value = titselctd.intid.ToString(),
                        //Selected = Convert.ToBoolean(titselctd.BitIsSelected=true)
                    };
                    selctedstring.Add(selectListItem.Text);
                    //selectedtit.Add(selectListItem);
                }
                ViewBag.selectnewtitile = new SelectList(listSelectListItem, "Value", "Text", selctedstring);
            }

            return View();
        }

        //edit dept function
        public ActionResult _editdept(int deptid, int empid)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var selecteddept = (objentity.tblDeptMas.Where(m => m.intid == deptid)).FirstOrDefault();
            var empmasdept = (objentity.tblEmpAssesmentMas.Where(m => m.intid == empid && m.intcode==code)).ToList();
            if (selecteddept == null)
            {
                return HttpNotFound();
            }
            else
            {
                List<tblDeptMas> deptlist = new List<tblDeptMas>();
                deptlist = (from e in objentity.tblDeptMas select e).ToList();
                List<string> deptselected = new List<string>();
                List<SelectListItem> listSelectListItem = new List<SelectListItem>();
                foreach (var depts in deptlist)
                {
                    SelectListItem selectListItem = new SelectListItem()
                    {
                        Text = depts.vchdeptname.ToString(),
                        Value = depts.intid.ToString(),
                        Selected = Convert.ToBoolean(depts.IsSelected)
                    };
                    listSelectListItem.Add(selectListItem);
                }

                foreach (var empdept in empmasdept)
                {
                    SelectListItem selectListItem = new SelectListItem()
                    {
                        //Text = empdept.vchdeptname.ToString(),
                        Value = empdept.fk_intdeptid.ToString(),
                        Selected = Convert.ToBoolean(empdept.BitIsselected)
                    };
                    deptselected.Add(selectListItem.Text);
                }

                ViewBag.selectnewdpt = new SelectList(listSelectListItem, "Value", "Text", deptselected);
            }
            return View();
        }

        //edit designation function
        public ActionResult _editdesignation(int desiid, int empid)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var selecteddesi = (objentity.tblDesignationMas.Where(m => m.intid == desiid)).FirstOrDefault();
            var selecetedDesi = (objentity.tblEmpAssesmentMas.Where(m => m.intid == empid && m.intcode==code)).ToList();
            if (selecteddesi == null)
            {
                return HttpNotFound();
            }
            else
            {
                List<tblDesignationMas> desilist = new List<tblDesignationMas>();
                desilist = (from e in objentity.tblDesignationMas select e).ToList();
                List<string> desiEmpmas = new List<string>();
                List<SelectListItem> alldesi = new List<SelectListItem>();
                foreach (var desi in desilist)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Text = desi.vchdesignation.ToString(),
                        Value = desi.intid.ToString(),
                        Selected = Convert.ToBoolean(desi.IsSelected)
                    };
                    alldesi.Add(selectListItem);
                }
                foreach (var desimas in selecetedDesi)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        //Text = desimas.vchdesignation.ToString(),
                        Value = desimas.fk_intdesiid.ToString()
                    };
                    desiEmpmas.Add(selectListItem.Text);
                }

                ViewBag.selectnewdesi = new SelectList(alldesi, "Value", "Text", desiEmpmas, "Text");

            }
            return View();
        }

        //update emp offecial details
        [HttpPost]
        public ActionResult EditEmpl(tblEmpAssesmentMas Empobj, tblDeptMas objdept, tblDesignationMas objdesi)
        {
            if (Empobj.intid!=0)
            {
                if (Session["descript"] != null)
                {

                    var selectedemp = (from e in objentity.tblEmpAssesmentMas where e.intid == Empobj.intid select e).FirstOrDefault();
                    if (selectedemp != null)
                    {
                        if (objdept.vchdeptname != null)
                        {
                            int dptid = Convert.ToInt32(objdept.vchdeptname);
                            var getdept = (from e in objentity.tblDeptMas where e.intid == dptid select e).FirstOrDefault();
                            selectedemp.fk_intdeptid = getdept.intid;
                        }
                        else
                        {
                            TempData["Error"] = "Department name should not be null!";
                            return RedirectToAction("EditEmpl");
                        }
                        if (objdesi.vchdesignation != null)
                        {
                            int desiid = Convert.ToInt32(objdesi.vchdesignation);
                            var getdesi = (from e in objentity.tblDesignationMas where e.intid == desiid select e).FirstOrDefault();
                            selectedemp.fk_intdesiid = getdesi.intid;
                        }
                        else
                        {

                            TempData["Error"] = "Designation name should not be null!";
                            return RedirectToAction("EditEmpl");
                        }
                        if (Empobj.dtDOJ != null)
                        {
                            selectedemp.dtDOJ = Empobj.dtDOJ;
                        }
                        else
                        {

                            TempData["Error"] = "DOJ name should not be null!";
                            return RedirectToAction("EditEmpl");
                        }
                        if (Empobj.intsalary != 0)
                        {
                            selectedemp.intsalary = Empobj.intsalary;
                        }
                        else
                        {

                            TempData["Error"] = "Salary amount should not be null!";
                            return RedirectToAction("EditEmpl");
                        }
                        if (Empobj.bitofficialdetails == true)
                        {
                            selectedemp.bitofficialdetails = true;
                        }
                        else
                        {
                            selectedemp.bitofficialdetails = false;
                        }
                        selectedemp.vchupdatedby = Session["descript"].ToString();
                        selectedemp.dtupdatedby = DateTime.Now;
                        objentity.SaveChanges();
                        TempData["Success"] = "Official details updated successfully!";
                        return RedirectToAction("ViewAllEmp");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            TempData["Error"] = "Model error generated please contact to administrator!";
            return RedirectToAction("ViewAllEmp");
        }

        //Add Qualification Details not used now
        public ActionResult QualificationDetails()
        {
            _allqualiemp();
            _allqualification();
            MasQualificationViewModel obmodel = new MasQualificationViewModel();
            List<tblQualiMas> qualilist = objentity.tblQualiMas.ToList();
            obmodel.AllQualification = new SelectList(qualilist, "intqualiid", "vchqname");
            return View(obmodel);
        }

        //not used now
        [HttpPost]
        public ActionResult QualificationDetails(MasQualificationViewModel objmodel)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    //int id = Convert.ToInt32(form.Get("intempid"));
                    var tcode = objmodel.Tempcode.ToString();
                    var empdetails = (from e in objentity.tblEmpMas where e.vchEmpTcode == tcode select e).FirstOrDefault();
                    //var selectedq = form["getquali"];
                    //String[] newstr = selectedq.Split(',');
                    //string submission = form.Get("bitqualidetails").ToString();
                    string submission = objmodel.bitQualification.ToString();
                    int id = empdetails.intempid;
                    foreach (var nayaid in objmodel.SelectedQualifications)
                    {
                        int newid = Convert.ToInt32(nayaid);
                        var selectnewq = (from e in objentity.tblQualiMas where e.intqualiid == newid select e).FirstOrDefault();
                        var selectemp = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
                        tblQualiDetails objt = new tblQualiDetails();
                        {
                            objt.intempid = selectemp.intempid;
                            objt.fk_intqualiid = selectnewq.intqualiid;
                            objt.vchqname = selectnewq.vchqname;
                            objt.vchcreatedby = Session["descript"].ToString();
                            objt.dtcreated = DateTime.Now;
                            objt.intcode = Convert.ToInt32(Session["id"]);
                            objt.intyr = Convert.ToInt32(Session["yr"]);
                            objt.vchipused = Session["ipused"].ToString();
                            objt.vchhostname = Session["hostname"].ToString();
                            objt.bitissleceted = true;
                        };
                        objentity.tblQualiDetails.Add(objt);
                        objentity.SaveChanges();
                    }
                    if (submission == "True")
                    {
                        // tblEmpMas objmas = new tblEmpMas();
                        empdetails.bittempqualidetails = true;
                        empdetails.bitqualidetails = true;
                        objentity.SaveChanges();
                    }
                    else if (submission == "False")
                    {
                        empdetails.bittempqualidetails = true;
                        objentity.SaveChanges();
                    }
                    else
                    {
                        //for exit
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("intempid", "Model error generated");
                return View();
            }
            TempData["Success"] = "Qualifications added successfully!";
            return RedirectToAction("ViewAllEmp");
        }

        //Add qualifications form view all employee not used now
        public ActionResult Addqualification(int id)
        {
            if (ModelState.IsValid)
            {
                var selectedemp = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
                string empcode = selectedemp.vchEmpTcode.ToString();
                ViewBag.SlectedEmpCode = empcode;
                List<SelectListItem> mlistitems = new List<SelectListItem>();
                MasQualificationViewModel objmedel = new MasQualificationViewModel();
                objmedel.id = id;
                objmedel.Tempcode = empcode;
                List<string> getselcted = new List<string>();
                foreach (var mquali in objentity.tblQualiMas)
                {
                    SelectListItem selectListItem = new SelectListItem()
                    {
                        Text = mquali.vchqname.ToString(),
                        Value = mquali.intqualiid.ToString()
                    };
                    mlistitems.Add(selectListItem);
                }
                objmedel.AllQualification = mlistitems;
                objmedel.SelectedQualifications = getselcted;
                return View(objmedel);
            }
            else
            {

            }
            return View();

        }
        //Save qualifications form view all employeenot used now
        [HttpPost]
        public ActionResult Addqualification(MasQualificationViewModel objmodel, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    //get emp code
                    int id = Convert.ToInt32(objmodel.id);
                    var selectedq = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
                    string empCode = selectedq.vchEmpTcode;
                    tblQualiDetails objquality = new tblQualiDetails();
                    if (objmodel.SelectedQualifications != null)
                    {
                        foreach (var quali in objmodel.SelectedQualifications)
                        {
                            int qid = Convert.ToInt32(quali);
                            var getquali = (from e in objentity.tblQualiMas where e.intqualiid == qid select e).FirstOrDefault();
                            objquality.intempid = id;
                            objquality.fk_intqualiid = qid;
                            objquality.vchqname = getquali.vchqname;
                            objquality.vchcreatedby = Session["descript"].ToString();
                            objquality.dtcreated = DateTime.Now;
                            objquality.intcode = Convert.ToInt32(Session["id"]);
                            objquality.intyr = Convert.ToInt32(Session["yr"]);
                            objquality.vchipused = Session["ipused"].ToString();
                            objquality.vchhostname = Session["hostname"].ToString();
                            objquality.bitissleceted = true;
                            objentity.tblQualiDetails.Add(objquality);
                            objentity.SaveChanges();
                        }

                        selectedq.bittempqualidetails = true;
                        objentity.SaveChanges();
                        var submission = objmodel.bitQualification.ToString();
                        if (submission == "Yes")
                        {
                            selectedq.bitqualidetails = true;
                            objentity.SaveChanges();
                        }
                        TempData["Success"] = "Qualifications added successfully!";
                        return RedirectToAction("ViewAllEmp");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Model error generated");
            }
            return View();
        }

        //Edit Qualification
        public ActionResult EditQualification(int id)
        {
            var selecetdempcode = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
            ViewBag.SelectedCode = selecetdempcode.vchEmpTcode.ToString();
            List<SelectListItem> mlistitems = new List<SelectListItem>();
            MasQualificationViewModel objmedel = new MasQualificationViewModel();
            objmedel.id = id;
            var selecetdemp = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
            objmedel.Tempcode = selecetdemp.vchEmpTcode.ToString();
            List<string> getselcted = new List<string>();
            foreach (var mquali in objentity.tblQualiMas)
            {
                //objmedel.SelectedQualifications=new[]
                SelectListItem selectListItem = new SelectListItem()
                {
                    Text = mquali.vchqname.ToString(),
                    Value = mquali.intqualiid.ToString()
                };
                mlistitems.Add(selectListItem);
            }
            List<tblQualiDetails> empdetail = (from e in objentity.tblQualiDetails where e.intempid == id select e).ToList();
            List<SelectListItem> listSelectListItem = new List<SelectListItem>();
            foreach (var qualification in empdetail)
            {
                SelectListItem selectListItem = new SelectListItem()
                {
                    Text = qualification.vchqname.ToString(),
                    Value = qualification.fk_intqualiid.ToString(),
                    Selected = Convert.ToBoolean(qualification.bitissleceted)
                };
                listSelectListItem.Add(selectListItem);
                getselcted.Add(selectListItem.Value);
            }

            objmedel.AllQualification = mlistitems;
            objmedel.SelectedQualifications = getselcted;
            objmedel.bitQualification = Convert.ToBoolean(selecetdempcode.bitqualidetails);
            return View(objmedel);
        }

        //Update qualifications not used now
        [HttpPost]
        public ActionResult EditQualification(MasQualificationViewModel objmas)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    int id = Convert.ToInt32(objmas.id);
                    List<tblQualiDetails> selectedempquali = (from e in objentity.tblQualiDetails where e.intempid == id select e).ToList();
                    IEnumerable<string> objmaslist = objmas.SelectedQualifications.ToList();
                    List<SelectListItem> newidlist = new List<SelectListItem>();
                    //old selection
                    List<int> newselection = new List<int>();
                    //new selction
                    List<int> oldselection = new List<int>();
                    //new object tblQualiDetails
                    tblQualiDetails objdetails = new tblQualiDetails();
                    //add old selction in formcompare2 list
                    foreach (var dblist in selectedempquali)
                    {
                        oldselection.Add(dblist.fk_intqualiid);
                    }
                    //add all new selected selection in formcompare
                    foreach (string value in objmaslist)
                    {
                        newselection.Add(Convert.ToInt32(value));
                    }

                    var Fselection = newselection.Except(oldselection).Union(newselection).ToList();
                    var oldfordel = oldselection.Except(Fselection).ToList();
                    //delete old selection which is not selected as new selection
                    if (oldfordel != null)
                    {
                        foreach (var quali in oldfordel)
                        {
                            int newid = Convert.ToInt32(quali);
                            var selectedquali = (from e in objentity.tblQualiDetails where e.fk_intqualiid == newid && e.intempid == objmas.id select e).FirstOrDefault();
                            objentity.tblQualiDetails.Remove(selectedquali);
                            objentity.SaveChanges();
                        }
                    }
                    //Add all selection if exist in db then update
                    if (Fselection != null)
                    {
                        foreach (var quali in Fselection)
                        {
                            int newid = Convert.ToInt32(quali);
                            var selectedquali = (from e in objentity.tblQualiDetails where e.fk_intqualiid == newid && e.intempid == objmas.id select e).FirstOrDefault();
                            if (selectedquali != null)
                            {
                                selectedquali.vchupdatedby = Session["descript"].ToString();
                                selectedquali.dtupdated = DateTime.Now;
                                objentity.SaveChanges();
                            }
                            else
                            {
                                var selectnewq = (from e in objentity.tblQualiMas where e.intqualiid == newid select e).FirstOrDefault();
                                var selectemp = (from e in objentity.tblQualiDetails where e.intempid == objmas.id select e).FirstOrDefault();
                                objdetails.intempid = selectemp.intempid;
                                objdetails.fk_intqualiid = selectnewq.intqualiid;
                                objdetails.vchqname = selectnewq.vchqname;
                                objdetails.vchcreatedby = Session["descript"].ToString();
                                objdetails.dtcreated = DateTime.Now;
                                objdetails.intcode = Convert.ToInt32(Session["id"]);
                                objdetails.intyr = Convert.ToInt32(Session["yr"]);
                                objdetails.vchipused = Session["ipused"].ToString();
                                objdetails.vchhostname = Session["hostname"].ToString();
                                objdetails.bitissleceted = true;
                                objentity.tblQualiDetails.Add(objdetails);
                                objentity.SaveChanges();
                            }
                        }
                    }
                    //check is final selection
                    if (objmas.bitQualification == true)
                    {
                        var selcetdempmas = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
                        selcetdempmas.bitqualidetails = true;
                        objentity.SaveChanges();
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Model error generated");
            }
            TempData["Success"] = "Qualifications updated successfully!";
            return RedirectToAction("ViewAllEmp");
        }
        //Enter New Personal details not used now
        public ActionResult PersonalDetails()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblEmpMas> selectempcode = (from e in objentity.tblEmpMas where e.bittempperdetails == false && e.intcode==code select e).ToList();
            ViewBag.Code = new SelectList(selectempcode, "intempid", "vchEmpTcode");
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
                                      Value = "2" },
                                new SelectListItem{
                                      Text = "Widowed",
                                      Value = "3" }
                              };
            ViewBag.MaritalStatus = new SelectList(Maritalstatus, "Value", "Text");

            return View();
        }
        //Save details not used now
        [HttpPost]
        public ActionResult PersonalDetails(tblEmpDetails objtable, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    int code = Convert.ToInt32(form.Get("vchempcode"));
                    var selecetdemployee = (from e in objentity.tblEmpMas where e.intempid == code select e).FirstOrDefault();
                    int fkintempid = selecetdemployee.intempid;
                    objtable.fk_intempid = fkintempid;
                    //for gender
                    string selectedgender = objtable.vchsex;
                    if (selectedgender == "1")
                    {
                        objtable.vchsex = "Male";
                    }
                    else if (selectedgender == "2")
                    {
                        objtable.vchsex = "Female";
                    }
                    else
                    {
                        objtable.vchsex = "not defined";
                    }

                    //for Martial status
                    string selecetedmstatus = objtable.vchmaritalststus;
                    if (selecetedmstatus == "1")
                    {
                        objtable.vchmaritalststus = "Single";
                    }
                    else if (selecetedmstatus == "2")
                    {
                        objtable.vchmaritalststus = "Married";
                    }
                    else if (selecetedmstatus == "3")
                    {
                        objtable.vchmaritalststus = "Widowed";
                    }
                    else
                    {
                        objtable.vchmaritalststus = "not define";
                    }
                    objtable.vchcreatedby = Session["descript"].ToString();
                    objtable.dtcreated = DateTime.Now;
                    objtable.vchipused = Session["ipused"].ToString();
                    objtable.vchhostname = Session["hostname"].ToString();
                    objtable.intcode = Convert.ToInt32(Session["id"].ToString());
                    objtable.intyr = Convert.ToInt32(Session["yr"].ToString());
                    objtable.BitIsSelected = true;
                    if (objtable.BitCompleted == true)
                    {
                        selecetdemployee.bittempperdetails = true;
                        selecetdemployee.bitperdetails = true;
                        objentity.SaveChanges();
                    }
                    else
                    {
                        selecetdemployee.bittempperdetails = true;
                        objentity.SaveChanges();
                    }
                    objentity.tblEmpDetails.Add(objtable);
                    objentity.SaveChanges();
                    TempData["Success"] = "Personal details added successfully!";
                    return RedirectToAction("ViewAllEmp");
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("vchsex", "Database error");
            }
            return View();

        }

        //Add Personal Details not used now
        public ActionResult AddPersonalDetails(int id)
        {
            //for new eomployee selection
            var selectemp = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
            ViewBag.EmpId = (selectemp.intempid).ToString();
            ViewBag.SelectEmployee = selectemp.vchEmpTcode.ToString();
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
                                      Value = "2" },
                                new SelectListItem{
                                      Text = "Widowed",
                                      Value = "3" }
                              };
            ViewBag.MaritalStatus = new SelectList(Maritalstatus, "Value", "Text");
            return View();
        }

        [HttpPost]
        public ActionResult AddPersonalDetails(tblEmpDetails newdetail, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    string code = form.Get("empcode");
                    var selecetdemployee = (from e in objentity.tblEmpMas where e.vchEmpTcode == code select e).FirstOrDefault();
                    newdetail.fk_intempid = selecetdemployee.intempid;
                    //for gender
                    string selectedgender = newdetail.vchsex;
                    if (selectedgender == "1")
                    {
                        newdetail.vchsex = "Male";
                    }
                    else if (selectedgender == "2")
                    {
                        newdetail.vchsex = "Female";
                    }
                    else
                    {
                        newdetail.vchsex = "not defined";
                    }

                    //for Martial status
                    string selecetedmstatus = newdetail.vchmaritalststus;
                    if (selecetedmstatus == "1")
                    {
                        newdetail.vchmaritalststus = "Single";
                    }
                    else if (selecetedmstatus == "2")
                    {
                        newdetail.vchmaritalststus = "Married";
                    }
                    else if (selecetedmstatus == "3")
                    {
                        newdetail.vchmaritalststus = "Widowed";
                    }
                    else
                    {
                        newdetail.vchmaritalststus = "not define";
                    }
                    newdetail.vchcreatedby = Session["descript"].ToString();
                    newdetail.dtcreated = DateTime.Now;
                    newdetail.vchipused = Session["ipused"].ToString();
                    newdetail.vchhostname = Session["hostname"].ToString();
                    newdetail.intcode = Convert.ToInt32(Session["id"].ToString());
                    newdetail.intyr = Convert.ToInt32(Session["yr"].ToString());
                    newdetail.BitIsSelected = true;
                    if (newdetail.BitCompleted == true)
                    {
                        selecetdemployee.bittempperdetails = true;
                        selecetdemployee.bitperdetails = true;
                        objentity.SaveChanges();
                    }
                    else
                    {
                        selecetdemployee.bittempperdetails = true;
                        objentity.SaveChanges();
                    }
                    //objentity = new HRMEntities();
                    objentity.tblEmpDetails.Add(newdetail);
                    objentity.SaveChanges();
                    TempData["Success"] = "Personal details added successfully!";
                    return RedirectToAction("ViewAllEmp");
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("vchsex", "Database error");
            }
            return View();
        }

        //Edit Personal details not used now
        public ActionResult EditPersonalDetails(int id)
        {
            //for new eomployee selection
            var selectemp = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
            ViewBag.EmpId = (selectemp.intempid).ToString();
            ViewBag.SelectEmployee = selectemp.vchEmpTcode.ToString();

            //for new selection
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
                                      Value = "1"
                                },
                                new SelectListItem{
                                      Text = "Married",
                                      Value = "2" },
                                new SelectListItem{
                                      Text = "Widowed",
                                      Value = "3"}
                              };
            ViewBag.MaritalStatus = new SelectList(Maritalstatus, "Value", "Text");

            //for all property return to edit view slecection
            var selectdempdetail = (from e in objentity.tblEmpDetails where e.fk_intempid == id select e).FirstOrDefault();
            return View(selectdempdetail);
        }
        //update perinformation not used now
        [HttpPost]
        public ActionResult EditPersonalDetails(tblEmpDetails objupdate, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    int id = Convert.ToInt32(form.Get("empcode"));
                    var masemp = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
                    var selecteddetail = (from e in objentity.tblEmpDetails where e.fk_intempid == id select e).FirstOrDefault();
                    //for gender
                    string selectedgender = objupdate.vchsex;
                    if (selectedgender == "1")
                    {
                        selecteddetail.vchsex = "Male";
                    }
                    else if (selectedgender == "2")
                    {
                        selecteddetail.vchsex = "Female";
                    }
                    else
                    {
                        selecteddetail.vchsex = "not defined";
                    }

                    //for Martial status
                    string selecetedmstatus = objupdate.vchmaritalststus;
                    if (selecetedmstatus == "1")
                    {
                        selecteddetail.vchmaritalststus = "Single";
                    }
                    else if (selecetedmstatus == "2")
                    {
                        selecteddetail.vchmaritalststus = "Married";
                    }
                    else if (selecetedmstatus == "3")
                    {
                        selecteddetail.vchmaritalststus = "Widowed";
                    }
                    else
                    {
                        selecteddetail.vchmaritalststus = "not define";
                    }
                    selecteddetail.vchupdatedby = Session["descript"].ToString();
                    selecteddetail.dtupdated = DateTime.Now;
                    if (objupdate.BitCompleted == true)
                    {

                        //if(selecteddetail.bit)
                        masemp.bittempperdetails = true;
                        masemp.bitperdetails = true;
                        selecteddetail.BitCompleted = true;
                        objentity.SaveChanges();
                    }
                    else
                    {
                        masemp.bittempperdetails = true;
                        objentity.SaveChanges();
                    }
                    selecteddetail.intage = objupdate.intage;
                    selecteddetail.vchspouse = objupdate.vchspouse;
                    selecteddetail.vchFatherName = objupdate.vchFatherName;
                    selecteddetail.vchmothername = objupdate.vchmothername;
                    selecteddetail.vchtstate = objupdate.vchtstate;
                    selecteddetail.vchtcity = objupdate.vchtcity;
                    selecteddetail.inttpin = objupdate.inttpin;
                    selecteddetail.vchtmobile = objupdate.vchtmobile;
                    selecteddetail.vchpstate = objupdate.vchpstate;
                    selecteddetail.vchpcity = objupdate.vchpcity;
                    selecteddetail.intppin = objupdate.intppin;
                    selecteddetail.vchpmobile = objupdate.vchpmobile;
                    selecteddetail.vchupdatedby = Session["descript"].ToString();
                    selecteddetail.dtupdated = DateTime.Now;
                    objentity.SaveChanges();
                    TempData["Success"] = "Personal details updated successfully!";
                    return RedirectToAction("ViewAllEmp");
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("vchsex", "Database error");
            }
            return View();
        }

        //Add Contact Details not used now
        public ActionResult ContactDetails()
        {
            return View();
        }

        //Partial view to get Name Title 
        public ActionResult _SelectTitle()
        {
            ViewBag.TitleList = new SelectList(_GetTitleMas(), "intid", "vchname");
            return View();
        }

        //List to getting all title name
        public List<tblTitleMas> _GetTitleMas()
        {
            HRMEntities objentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblTitleMas> list = objentity.tblTitleMas.ToList();
            return list;
        }

        //Get All dept
        public ActionResult _getdept()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblDeptMas> deptlist = objentity.tblDeptMas.ToList();
            ViewBag.deptlist = new SelectList(deptlist, "intid", "vchdeptname");
            return View();
        }

        //get all designation
        public ActionResult _getdesi()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblDesignationMas> desilist = objentity.tblDesignationMas.ToList();
            ViewBag.desilist = new SelectList(desilist, "intid", "vchdesignation");
            return View();
        }

        // Get all master qualification
        public ActionResult _allqualification()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblQualiMas> qualilist = objentity.tblQualiMas.ToList();
            ViewBag.quali = new SelectList(qualilist, "intqualiid", "vchqname");
            return View();
        }

        //get employee for qualification details
        public ActionResult _allqualiemp()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblEmpMas> masemp = (from e in objentity.tblEmpMas where e.bittempqualidetails == false && e.intcode==code select e).ToList();
            ViewBag.pquali = new SelectList(masemp, "vchEmpTcode", "vchEmpTcode");
            return View();
        }

        /// <summary>
        /// Updated version 13/07/2020
        /// </summary>
        /// <returns></returns>
        //Varoius partial view used in New Empolyee Controller
        public ActionResult AllqualiPartialModel()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            List<SelectListItem> listSelectListItem = new List<SelectListItem>();
            foreach (var qualification in objentity.tblQualiMas)
            {
                if (qualification.BitIsSlected == null)
                {
                    qualification.BitIsSlected = false;
                }

                SelectListItem selectListItem = new SelectListItem()
                {
                    Text = qualification.vchqname.ToString(),
                    Value = qualification.intqualiid.ToString(),
                    Selected = Convert.ToBoolean(qualification.BitIsSlected)
                };
                listSelectListItem.Add(selectListItem);
            }
            MasQualificationViewModel objtmodel = new MasQualificationViewModel();
            objtmodel.Qualifications = listSelectListItem;
            return View(objtmodel);
        }

        //Add new document not used now
        public ActionResult AddNewDoc(int id)
        {
            var selectedemp = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
            ViewBag.EmptCode = selectedemp.vchEmpTcode.ToString();
            ViewBag.Empname = selectedemp.vchfname.ToString();
            var empallquali = (from e in objentity.tblQualiMas select e).ToList();
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
            objmodel.docnamelist = quali;
            //TempData["docname"] = quali;
            ViewBag.EmpID = id.ToString();
            return View(objmodel);
        }

        //make new model for view and httpost file method not used now
        [HttpPost]
        public ActionResult AddNewDoc(DocumentViewModel newdoc, FormCollection formdata)
        {

            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    //docdetails table object
                    tblDocDetails objdocdetail = new tblDocDetails();
                    //get emp id
                    int id1 = Convert.ToInt32(formdata.Get("hdempid"));
                    //int id = Convert.ToInt32(newdoc.empid);
                    //get all emp id qualifications
                    int docid = Convert.ToInt32(newdoc.filename);
                    var selecteddoc = (from e in objentity.tblQualiMas where e.intqualiid == docid select e).FirstOrDefault();
                    //get all master document from docmas
                    //check for pdf null
                    if (newdoc.pdfFile != null)
                    {
                        //filename new format filename+datetime+empid+extension                         
                        string empid = id1.ToString();
                        string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                        string extension = Path.GetExtension(newdoc.pdfFile.FileName);
                        string filename = Path.GetFileNameWithoutExtension(newdoc.pdfFile.FileName);
                        string newfilename = filename + datetime + empid + extension;
                        string path = Path.Combine(Server.MapPath("~/Uploads/" + newfilename));
                        //save file in upload folder
                        newdoc.pdfFile.SaveAs(path);
                        //Check for is final submission or not
                        tblEmpMas objmasemp = new tblEmpMas();
                        var selecetdmasemp = (from e in objentity.tblEmpMas where e.intempid == id1 select e).FirstOrDefault();
                        if (newdoc.BitCompleted == true)
                        {
                            selecetdmasemp.bittempcontdetails = true;
                            selecetdmasemp.bitcontdetails = true;
                            objdocdetail.fk_empAssid = id1;
                            objdocdetail.fk_intdocid = docid;
                            objdocdetail.vchdocname = selecteddoc.vchqname;
                            objdocdetail.vchdocadd = path.ToString();
                            objdocdetail.vchfilename = newfilename.ToString();
                            objdocdetail.vchcreatedby = Session["descript"].ToString();
                            objdocdetail.dtcreated = DateTime.Now;
                            objdocdetail.vchipused = Session["ipused"].ToString();
                            objdocdetail.vchhostname = Session["hostname"].ToString();
                            objdocdetail.intcode = Convert.ToInt32(Session["id"]);
                            objdocdetail.intyr = Convert.ToInt32(Session["yr"]);
                            objentity.tblDocDetails.Add(objdocdetail);
                            objentity.SaveChanges();
                            TempData["Success"] = "Final upload successfully!";
                            return RedirectToAction("ViewAllEmp");
                        }
                        else
                        {
                            selecetdmasemp.bittempcontdetails = true;
                            //save path and emp id in database
                            objdocdetail.fk_empAssid = id1;
                            objdocdetail.fk_intdocid = docid;
                            objdocdetail.vchdocname = selecteddoc.vchqname;
                            objdocdetail.vchdocadd = path.ToString();
                            objdocdetail.vchfilename = newfilename.ToString();
                            objdocdetail.vchcreatedby = Session["descript"].ToString();
                            objdocdetail.dtcreated = DateTime.Now;
                            objdocdetail.vchipused = Session["ipused"].ToString();
                            objdocdetail.vchhostname = Session["hostname"].ToString();
                            objdocdetail.intcode = Convert.ToInt32(Session["id"]);
                            objdocdetail.intyr = Convert.ToInt32(Session["yr"]);
                            objentity.tblDocDetails.Add(objdocdetail);
                            objentity.SaveChanges();
                            TempData["Success"] = "Document upload successfully!";
                            return RedirectToAction("AddNewDoc");
                            //ModelState.AddModelError("", "");
                            //return RedirectToAction("ViewAllDoc");
                        }

                    }
                    else
                    {
                        //error msgs
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            else
            {
                return View();
            }
            //return View();
        }

        //Edit Document not used now
        public ActionResult ViewAllDoc(int id)
        {
            if (ModelState.IsValid)
            {

                var selectdrecord = (from e in objentity.tblDocDetails where e.fk_empAssid == id select e).ToList();
                if (selectdrecord.Count() != 0)
                {
                    return View(selectdrecord);
                }
                else
                {
                    //update document ststus in master table
                    var selectedemp = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
                    selectedemp.bittempcontdetails = false;
                    objentity.SaveChanges();
                    TempData["Empty"] = "No document found current employee";
                    return RedirectToAction("ViewAllEmp");
                }
            }
            else
            {
                //ModelState.AddModelError("vchdocname", "There is no document found agaist this employee");
                TempData["Error"] = "No document found current employee";
                return View();
            }
        }

        //Delete Document not used now
        public ActionResult DelDoc(int id, int empid)
        {
            if (Session["descript"] != null)
            {
                var deldoc = (from e in objentity.tblDocDetails where e.fk_intdocid == id && e.fk_empAssid == empid select e).FirstOrDefault();
                if (deldoc != null)
                {
                    objentity.tblDocDetails.Remove(deldoc);
                    objentity.SaveChanges();
                }
                var checkformore = (from e in objentity.tblDocDetails where e.fk_empAssid == empid select e).FirstOrDefault();
                if (checkformore != null)
                {
                    TempData["Success"] = "Document deleted successfully!";
                    return RedirectToAction("ViewAllDoc",new { id = empid });
                }
                else
                {
                    //update document ststus in master table
                    var selectedemp = (from e in objentity.tblEmpMas where e.intempid == id select e).FirstOrDefault();
                    selectedemp.bittempcontdetails = false;
                    objentity.SaveChanges();
                    TempData["Success"] = "Document deleted successfully!";
                    return RedirectToAction("ViewAllEmp");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
    }
}