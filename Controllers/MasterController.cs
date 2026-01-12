using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc; 
using HRM.Models;
using Microsoft.Ajax.Utilities;
using System.Data;
using System.Data.Entity;

namespace HRM.Controllers
{
    public class MasterController : Controller
    {

        #region Table Title Master New, View, Edit, update actions

        // GET: Master
        public ActionResult NameTitleMas()
        {
            return View();
        }

        //create new titile
        [HttpPost]
        public ActionResult NameTitleMas(tblTitleMas newtitle,FormCollection formdata)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    var datetime = Convert.ToDateTime(Request.Form["clock"].ToString());
                    HRMEntities objentity = new HRMEntities();
                    int code = Convert.ToInt32(Session["id"].ToString());
                    string name = newtitle.vchname.ToString();
                    var checkname = (from e in objentity.tblTitleMas where e.vchname == name select e).FirstOrDefault();
                    if (checkname != null)
                    {
                        ModelState.AddModelError("vchname", "Title already present in master");
                    }
                    else
                    {
                        newtitle.vchcreatedby = Session["descript"].ToString();
                        newtitle.dtcreated = DateTime.Now;
                        newtitle.intcode = code;
                        newtitle.intyr = Convert.ToInt32(Session["yr"]);
                        newtitle.vchipused = Session["ipused"].ToString();
                        newtitle.vchhostname = Session["hostname"].ToString();
                        objentity.tblTitleMas.Add(newtitle);
                        objentity.SaveChanges();
                        TempData["Success"] = "New title saved successfully!";
                        return RedirectToAction("NameTitleMas");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            return View();
        }

        //view all title master
        public ActionResult ViewAllTitleMas()
        {
            if (ModelState.IsValid)
            {
                HRMEntities objtt = new HRMEntities();
                int code = Convert.ToInt32(Session["id"].ToString());
                List<tblTitleMas> titlelist = (from e in objtt.tblTitleMas select e).ToList();
                if (titlelist != null)
                {
                    
                    return View(titlelist);
                }
                else
                {
                    TempData["Empty"] = "No record found in titile master!";
                    return View();
                }
            }
            return View();
        }

        //Edit Title master
        public ActionResult EditTitle(int id)
        {
            HRMEntities objentity = new HRMEntities();
            //int code = Convert.ToInt32(Session["id"].ToString());
            var selectedtitile = (from e in objentity.tblTitleMas where e.intid == id select e).FirstOrDefault();
            if (selectedtitile == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(selectedtitile);
            }
        }

        //update title
        [HttpPost]
        public ActionResult EditTitle(tblTitleMas tblobj)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    HRMEntities objentity = new HRMEntities();
                    int code = Convert.ToInt32(Session["id"].ToString());
                    var selecttitile = objentity.tblTitleMas.Single(m => m.intid == tblobj.intid);
                    selecttitile.vchname = tblobj.vchname.ToString();
                    selecttitile.vchupdatedby = Session["descript"].ToString();
                    selecttitile.dtupdate = DateTime.Now;
                    TempData["Success"] = "Titile updated successfully!";
                    objentity.SaveChanges();
                    return RedirectToAction("ViewAllTitleMas");

                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            return RedirectToAction("ViewAllTitleMas");
        }

        #endregion

        #region Table Department Master New, View, Edit, update actions

        //get Department master
        public ActionResult Department()
        {
            return View();
        }

        //create new department
        [HttpPost]
        public ActionResult Department(tblDeptMas newdept)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    HRMEntities objentity = new HRMEntities();
                    int code = Convert.ToInt32(Session["id"].ToString());
                    string ndept = newdept.vchdeptname.ToString();
                    var newdpt1 = (from e in objentity.tblDeptMas where e.vchdeptname == ndept select e).FirstOrDefault();
                    if (newdpt1 != null)
                    {
                        ModelState.AddModelError("vchdeptname", "This department already existing in department master");
                    }
                    else
                    {
                        newdept.vchdeptname = ndept;
                        newdept.vchcreatedby = Session["descript"].ToString();
                        newdept.dtcreated = DateTime.Now;
                        newdept.intcode = code;
                        newdept.intyr = Convert.ToInt32(Session["yr"]);
                        newdept.vchipused = Session["ipused"].ToString();
                        newdept.vchhostname = Session["hostname"].ToString();
                        objentity.tblDeptMas.Add(newdept);
                        objentity.SaveChanges();
                        TempData["Success"] = "New department saved successfully!";
                        return RedirectToAction("Department");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            return View();
        }

        //view all department
        public ActionResult ViewAllDept()
        {
            if (ModelState.IsValid)
            {
                HRMEntities objtt = new HRMEntities();
                int code = Convert.ToInt32(Session["id"].ToString());
                List<tblDeptMas> desiglist = (from e in objtt.tblDeptMas select e).ToList();
                if (desiglist == null || desiglist.Count == 0)
                {
                    TempData["Empty"] = "No records avilable in Designation Master";
                    return RedirectToAction("ViewAllDept");
                }
                else
                {
                    return View("ViewAllDept", desiglist);
                }
            }
            return View();
        }

        //Edit Department
        public ActionResult EditDept(int id)
        {
            HRMEntities objentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectedtitile = (from e in objentity.tblDeptMas where e.intid == id select e).FirstOrDefault();
            if (selectedtitile == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(selectedtitile);
            }
        }

        //update Department
        [HttpPost]
        public ActionResult EditDept(tblDeptMas tblobj)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    HRMEntities objentity = new HRMEntities();
                    int code = Convert.ToInt32(Session["id"].ToString());
                    var selecteddept = objentity.tblDeptMas.Single(m => m.intid == tblobj.intid);
                    selecteddept.vchdeptname = tblobj.vchdeptname.ToString();
                    selecteddept.bitIsByPassDept = tblobj.bitIsByPassDept;
                    selecteddept.bitIsCounterApplied = tblobj.bitIsCounterApplied;
                    if (selecteddept.bitIsActive == true)
                    {
                        selecteddept.bitIsInActive = true;
                    }
                    else
                    {
                        selecteddept.bitIsInActive = false;
                    }
                    selecteddept.bitIsActive = tblobj.bitIsActive;                    
                    selecteddept.vchupdatedby = Session["descript"].ToString();
                    selecteddept.vchupdatedhostname = Session["hostname"].ToString();
                    selecteddept.vchupdatedipused = Session["ipused"].ToString();
                    selecteddept.dtupdated = DateTime.Now;
                    objentity.SaveChanges();
                    TempData["Success"] = "Department updated successfully!";
                    return RedirectToAction("ViewAllDept");
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
        }

        #endregion

        #region Table Designation Master New, View, Edit, update operations
        
        // GET: Master
        public ActionResult DesignationMas()
        {
            HRMEntities objentity = new HRMEntities();
            List<tblDeptMas> getdept = objentity.tblDeptMas.ToList();
            ViewBag.DeptList = new SelectList(getdept, "intid", "vchdeptname");
            return View();
        }

        //enter designation master
        [HttpPost]
        public ActionResult DesignationMas(tblDesignationMas newdesignation)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    HRMEntities objentity = new HRMEntities();                    
                    int deptid = newdesignation.intdeptid;
                    string newdesig = newdesignation.vchdesignation.ToString();
                    var checkdesi = (from e in objentity.tblDesignationMas where e.vchdesignation == newdesig && e.intdeptid==deptid select e).FirstOrDefault();
                    if (checkdesi != null)
                    {
                        ModelState.AddModelError("vchdesignation", "Designation name already existing in designation master");
                    }
                    else
                    {
                        newdesignation.vchcreatedby = Session["descript"].ToString();
                        newdesignation.dtcreated = DateTime.Now;
                        newdesignation.intcode = Convert.ToInt32(Session["id"]);
                        newdesignation.intyr = Convert.ToInt32(Session["yr"]);
                        newdesignation.vchipused = Session["ipused"].ToString();
                        newdesignation.vchhostname = Session["hostname"].ToString();
                        objentity.tblDesignationMas.Add(newdesignation);
                        objentity.SaveChanges();
                        TempData["Success"] = "Designation saved successfully!";
                        return RedirectToAction("DesignationMas");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
                //return View("DesignationMas");
            }
            //return View();
            return RedirectToAction("DesignationMas");
        }

        //View All designation view
        public ActionResult ViewAllDesignationMas()
        {
            if (ModelState.IsValid)
            {
                HRMEntities objtt = new HRMEntities();               
                List<tblDesignationMas> desiglist = (from e in objtt.tblDesignationMas orderby e.tblDeptMas.vchdeptname select e).ToList();
                if (desiglist == null || desiglist.Count == 0)
                {
                    TempData["Empty"] = "No records found in designation master!";
                    return View();
                }
                else
                {
                    return View(desiglist);
                }
            }
            return View();
        }

        //Edit Designation
        public ActionResult EditDesignation(int id, int deptid)
        {
            HRMEntities objentity = new HRMEntities();           
            var selectteddesi = (from e in objentity.tblDesignationMas where e.intid == id select e).FirstOrDefault();
            List<tblDeptMas> getdept = objentity.tblDeptMas.ToList();
            ViewBag.DeptList = new SelectList(getdept, "intid", "vchdeptname");
            return View(selectteddesi);
        }

        //update Designation
        [HttpPost]
        public ActionResult EditDesignation(tblDesignationMas tblobj)// , int deptid)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {

                    HRMEntities objentity = new HRMEntities();
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int deptid = tblobj.intdeptid;
                    var selectteddesi = (from e in objentity.tblDesignationMas where e.intid == tblobj.intid select e).FirstOrDefault();
                    if (selectteddesi != null)
                    {
                        selectteddesi.intdeptid = deptid;
                        selectteddesi.vchdesignation = tblobj.vchdesignation.ToString();
                        selectteddesi.vchupdatedby = Session["descript"].ToString();
                        selectteddesi.dtupdated = DateTime.Now;
                        selectteddesi.vchupdatedhostname = Session["hostname"].ToString();
                        selectteddesi.vchupdatedipused = Session["ipused"].ToString();
                        selectteddesi.intcode = Convert.ToInt32(Session["id"]);
                        selectteddesi.intyr = Convert.ToInt32(Session["yr"]);
                        objentity.SaveChanges();
                        TempData["Success"] = "Designation updated successfully!";
                        return RedirectToAction("ViewAllDesignationMas");
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

            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion
        
        #region Table Qualification Master New, View, Edit, update operations
        
        // GET: Master
        public ActionResult QualificationMas()
        {
            return View();
        }

        //add new qualifaction
        [HttpPost]
        public ActionResult QualificationMas(tblQualiMas newqual)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    HRMEntities objentity = new HRMEntities();
                    int code = Convert.ToInt32(Session["id"].ToString());
                    string newdata = newqual.vchqname.ToString();
                    var checkquali = (from e in objentity.tblQualiMas where e.vchqname == newdata select e).FirstOrDefault();
                    if (checkquali != null)
                    {
                        ModelState.AddModelError("vchqname", "Designation name already  existing in designation master");
                    }
                    else
                    {
                        newqual.vchqname = newdata.ToString();
                        newqual.vchcreatedby = Session["descript"].ToString();
                        newqual.dtcreated = DateTime.Now;
                        newqual.vchipused = Session["ipused"].ToString();
                        newqual.vchhostname = Session["hostname"].ToString();
                        newqual.intcode = Convert.ToInt32(Session["id"]);
                        newqual.intyr = Convert.ToInt32(Session["yr"]);
                        objentity.tblQualiMas.Add(newqual);
                        objentity.SaveChanges();
                        TempData["Success"] = "Qualification saved successfully!";
                        return RedirectToAction("QualificationMas");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            return View();
        }

        //View All Qualification
        public ActionResult ViewAllQualificationMas()
        {
            if (ModelState.IsValid)
            {
                HRMEntities objtt = new HRMEntities();
                int code = Convert.ToInt32(Session["id"].ToString());
                List<tblQualiMas> desiglist = (from e in objtt.tblQualiMas select e).ToList();
                if (desiglist == null || desiglist.Count == 0)
                {
                    ViewBag.Error = "No records avilable in Designation Master";
                    return View();
                }
                else
                {
                    return View("ViewAllQualificationMas", desiglist);
                }
            }
            return View();
        }

        //Edit Qualification
        public ActionResult editquali(int id)
        {
            HRMEntities objentity = new HRMEntities();            
            var selectedquali = (from e in objentity.tblQualiMas where e.intqualiid == id select e).FirstOrDefault();
            if (selectedquali == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(selectedquali);
            }
        }

        //update qualification
        [HttpPost]
        public ActionResult editquali(tblQualiMas tblobj)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    HRMEntities objentity = new HRMEntities();
                    string newqualiname = tblobj.vchqname.ToString();                   
                    var checkdoc = (from e in objentity.tblQualiMas where e.vchqname == newqualiname  select e).FirstOrDefault();
                    if (checkdoc != null)
                    {
                        ModelState.AddModelError("vchqname", "Name already existing in qualification master");
                        return View();
                    }
                    else
                    {
                        var selectedDoc = objentity.tblQualiMas.Single(m => m.intqualiid == tblobj.intqualiid);
                        selectedDoc.vchqname = newqualiname;
                        selectedDoc.vchupdatedby = Session["descript"].ToString();
                        selectedDoc.dtupdated = DateTime.Now;
                        objentity.SaveChanges();
                        TempData["Success"] = "Qualification updated successfully!";
                        return RedirectToAction("ViewAllQualificationMas");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }

            }
            return View();
        }

        #endregion

        #region Table Document Master New, View, Edit, update operations 

        // GET: Master
        public ActionResult DocumentMas()
        {
            return View();
        }

        //Create new Document Master
        [HttpPost]
        public ActionResult DocumentMas(tblDocMas newdoc)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    HRMEntities objentity = new HRMEntities();
                    string newdata = newdoc.vchdocname.ToString();
                    var checkdoc = (from e in objentity.tblDocMas where e.vchdocname == newdata select e).FirstOrDefault();
                    if (checkdoc != null)
                    {
                        ModelState.AddModelError("vchdesignation", "Designation name already  existing in designation master");
                    }
                    else
                    {
                        newdoc.vchdocname = newdata.ToString();
                        newdoc.vchcreatedby = Session["descript"].ToString();
                        newdoc.dtcreated = DateTime.Now;
                        newdoc.vchipused = Session["ipused"].ToString();
                        newdoc.vchhostname = Session["hostname"].ToString();
                        newdoc.intcode = Convert.ToInt32(Session["id"]);
                        newdoc.intyr = Convert.ToInt32(Session["yr"]);
                        objentity.tblDocMas.Add(newdoc);
                        objentity.SaveChanges();
                        return RedirectToAction("ViewAllDoc");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            return View();
        }

        //view all doc mas
        public ActionResult ViewAllDoc()
        {
            if (ModelState.IsValid)
            {
                HRMEntities objtt = new HRMEntities();
                int code = Convert.ToInt32(Session["id"].ToString());
                List<tblDocMas> desiglist = (from e in objtt.tblDocMas select e).ToList();
                if (desiglist == null || desiglist.Count == 0)
                {
                    ViewBag.Error = "No records avilable in Designation Master";
                    return View();
                }
                else
                {
                    return View(desiglist);
                }
            }
            return View();
        }

        //Edit Document
        public ActionResult EditDoc(int id)
        {
            HRMEntities objentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectedDoc = (from e in objentity.tblDocMas where e.intid == id select e).FirstOrDefault();
            if (selectedDoc == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(selectedDoc);
            }
        }

        //update Department
        [HttpPost]
        public ActionResult EditDoc(tblDocMas tblobj)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    HRMEntities objentity = new HRMEntities();
                    string newdocname = tblobj.vchdocname.ToString();
                    var checkdoc = (from e in objentity.tblDocMas where e.vchdocname == newdocname select e).FirstOrDefault();
                    if (checkdoc != null)
                    {
                        ModelState.AddModelError("vchdocname", "Name already existing in document master");
                        return View();
                    }
                    else
                    {
                        var selectedDoc = objentity.tblDocMas.Single(m => m.intid == tblobj.intid);
                        selectedDoc.vchdocname = newdocname;
                        selectedDoc.vchupdatedby = Session["descript"].ToString();
                        selectedDoc.dtupdated = DateTime.Now;
                        objentity.SaveChanges();
                        return RedirectToAction("ViewAllDoc");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            return View();
        }

        #endregion

        #region Table Department Dropdown as partial view

        //select department in designation master.
        public ActionResult _selectdept()
        {
            HRMEntities objent = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblDeptMas> getdept =(from e in  objent.tblDeptMas select e).ToList();
            ViewBag.DeptList = new SelectList(getdept, "intid", "vchdeptname");
            return View();
        }

        public ActionResult _selectdepEditView(int deptid)
        {
            HRMEntities objent = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblDeptMas> selectidlist = objent.tblDeptMas.ToList();
            var selecteddept = (objent.tblDeptMas.Where(m => m.intid == deptid )).FirstOrDefault();
            if (selectidlist == null)
            {
                return HttpNotFound();
            }
            else
            {

                ViewBag.selectnewitem = new SelectList(selectidlist, "intid", "vchdeptname", selecteddept.vchdeptname);

            }
            return View();
        }

        #endregion

        #region for designation qualification add, update, view, delete actions
        public ActionResult ViewDesiQuali()
        {
            HRMEntities objent = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblDesiQualification> getdesi =objent.tblDesiQualification.OrderBy(s=>s.vchQualification).ToList();
            return View(getdesi);
        }

        public ActionResult AddNew()
        {

            HRMEntities objent = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblDesignationMas> getdept = (from e in objent.tblDesignationMas select e).ToList();
            ViewBag.DesiList = new SelectList(getdept, "intid", "vchdesignation");
            return View();
        }

        [HttpPost]
        public ActionResult AddNew(tblDesiQualification objtbl)
        {
            HRMEntities entity = new HRMEntities();
            if(ModelState.IsValid)
            {
                int desiID = objtbl.fk_desiid;
                int code = Convert.ToInt32(Session["id"].ToString());
                string quali = objtbl.vchQualification.ToString();
                var checkrecrd = (from e in entity.tblDesiQualification where e.fk_desiid == desiID && e.vchQualification == quali select e).FirstOrDefault();
                if(checkrecrd==null)
                {
                    objtbl.vchCreatedBy = Session["descript"].ToString();
                    objtbl.dtcreated = DateTime.Now;
                    objtbl.vchipused = Session["ipused"].ToString();
                    objtbl.vchhostname = Session["hostname"].ToString();
                    objtbl.intcode = Convert.ToInt32(Session["id"].ToString());
                    objtbl.intyr = Convert.ToInt32(Session["yr"].ToString());
                    entity.tblDesiQualification.Add(objtbl);
                    entity.SaveChanges();
                    TempData["Success"] = "Record saved successfully";
                    return RedirectToAction("AddNew");
                }
                else
                {
                    ModelState.AddModelError("vchQualification", "qualification already existing in selected designation");
                }

                return View();
            }
            else
            {
                ModelState.AddModelError("vchQualification", "Model error");
            }
            return View();
        }

        public ActionResult EditDesiQuali(int id)
        {
            if (ModelState.IsValid)
            {
                HRMEntities objentity = new HRMEntities();
                int code = Convert.ToInt32(Session["id"].ToString());
                var selected = (from e in objentity.tblDesiQualification where e.intid == id select e).FirstOrDefault();
                List<tblDesignationMas> getdesi = objentity.tblDesignationMas.ToList();
                ViewBag.DesiList = new SelectList(getdesi, "intid", "vchdesignation");
                return View(selected);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditDesiQuali(tblDesiQualification objedit)
        {
            if(ModelState.IsValid)
            {

                HRMEntities objentity = new HRMEntities();
                int code = Convert.ToInt32(Session["id"].ToString());
                var selected = (from e in objentity.tblDesiQualification where e.intid == objedit.intid select e).FirstOrDefault();
                if(selected!=null)
                {
                    selected.fk_desiid = objedit.fk_desiid;
                    selected.vchQualification = objedit.vchQualification;
                    selected.vchupdatedby = Session["descript"].ToString();
                    selected.dtupdated = DateTime.Now;
                    selected.vchupdatedipused = Session["ipused"].ToString();
                    selected.vchupdatedhostname = Session["hostname"].ToString();
                    objentity.SaveChanges();
                    TempData["Success"] = "Record updated successfully";
                    return RedirectToAction("EditDesiQuali");

                }
            }
            else
            {
                ModelState.AddModelError("fk_desiid", "Model error");
            }
            return View();
        }

        public ActionResult DeleteQualiDesi(int id)
        {
            HRMEntities objentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            var selected = (from e in objentity.tblDesiQualification where e.intid == id select e).FirstOrDefault();
            if(selected!=null)
            {
                objentity.tblDesiQualification.Remove(selected);
                objentity.SaveChanges();
                TempData["Success"] = "Record deleted successfully";
            }
            return RedirectToAction("ViewDesiQuali");
        }
        #endregion

        #region for designation setting add, update, view, delete actions
        //View All
        public ActionResult ViewDesiSetting()
        {
            HRMEntities objent = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblDesiSetting> getsetting = objent.tblDesiSetting.OrderBy(s=>s.tblDesignationMas.vchdesignation).ToList();
            if (getsetting.Count != 0)
            {
                //TempData["Empty"] = "No record found in database";
                return View(getsetting);
            }
            else
            {
                TempData["Empty"] = "No record found in database";
                return View();
            }
        }

        //Add New
        public ActionResult AddNewSetting()
        {

            HRMEntities objent = new HRMEntities();
            List<tblDesignationMas> getdept =(from e in objent.tblDesignationMas select e).ToList();
            ViewBag.DesiList = new SelectList(getdept, "intid", "vchdesignation");
            return View();
        }

        [HttpPost]
        public ActionResult AddNewSetting(tblDesiSetting newobj)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    HRMEntities objent = new HRMEntities();
                    int desiid = newobj.fk_intdesiid;
                    decimal minexp = newobj.numExpMin;
                    decimal maxexp = newobj.numExpMax;
                    decimal maxsal = newobj.numSalMax;
                    decimal minsal = newobj.numSalMin;
                    var chkrecord = (from e in objent.tblDesiSetting
                                     where e.numExpMin == minexp && e.numExpMax == maxexp
                                        && e.numSalMin == minsal && e.numSalMax == maxsal
                                        && e.fk_intdesiid == desiid
                                     select e).FirstOrDefault();
                    if (chkrecord == null)
                    {
                        
                        newobj.dtcreated = DateTime.Now;
                        newobj.vchCreatedBy = Session["descript"].ToString();
                        newobj.vchhostname = Session["hostname"].ToString();
                        newobj.vchipused = Session["ipused"].ToString();
                        newobj.intcode = Convert.ToInt32(Session["id"].ToString());
                        newobj.intyr = Convert.ToInt32(Session["yr"].ToString());
                        objent.tblDesiSetting.Add(newobj);
                        objent.SaveChanges();
                        TempData["Success"] = "Record saved successfully!";
                        return RedirectToAction("AddNewSetting");

                    }
                    else
                    {
                        TempData["ModelError"] = "Record already existing in current designation";
                        return RedirectToAction("AddNewSetting");
                    }
                }
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //Edit
        public ActionResult EditSetting(int id)
        {

            HRMEntities objentity = new HRMEntities();
            var selected = (from e in objentity.tblDesiSetting where e.intid == id select e).FirstOrDefault();
            List<tblDesignationMas> getdept = objentity.tblDesignationMas.ToList();
            ViewBag.DesiList = new SelectList(getdept, "intid", "vchdesignation");
            return View(selected);
        }

        [HttpPost]
        public ActionResult EditSetting(tblDesiSetting objedit)
        {
            HRMEntities objentity = new HRMEntities();
            if (ModelState.IsValid)
            {
                var selectedrecord = (from e in objentity.tblDesiSetting where e.intid == objedit.intid select e).FirstOrDefault();
                if(selectedrecord!=null)
                {
                    selectedrecord.numExpMin = objedit.numExpMin;
                    selectedrecord.numExpMax = objedit.numExpMax;
                    selectedrecord.numSalMin = objedit.numSalMin;
                    selectedrecord.numSalMax = objedit.numSalMax;
                    selectedrecord.vchupdatedby = Session["descript"].ToString();
                    selectedrecord.vchupdatedhostname = Session["hostname"].ToString();
                    selectedrecord.vchupdatedipused = Session["ipused"].ToString();
                    objentity.SaveChanges();
                    TempData["Success"] = "Record updated successfully!";
                    return RedirectToAction("EditSetting");

                }
            }
            return View();
        }

        //delete
        public ActionResult DeleteQualiSetting(int id)
        {
            HRMEntities objentity = new HRMEntities();
            var selected = (from e in objentity.tblDesiSetting where e.intid == id select e).FirstOrDefault();
            if (selected != null)
            {
                objentity.tblDesiSetting.Remove(selected);
                objentity.SaveChanges();
                TempData["Success"] = "Record deleted successfully";
            }
            return RedirectToAction("ViewDesiSetting");
        }
        #endregion

        #region for Position Master add, update, view, delete actions.

        //view all
        public ActionResult ViewPosCategory()
        {
            HRMEntities objent = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblPositionCategoryMas> getposCat = objent.tblPositionCategoryMas.OrderBy(s => s.vchPosCatName).ToList();
            if (getposCat.Count != 0)
            {
                //TempData["Empty"] = "No record found in database";
                return View(getposCat);
            }
            else
            {
                TempData["Empty"] = "No record found in database";
                return View();
            }
        }

        //Add New
        public ActionResult AddNewPosCat()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNewPosCat(tblPositionCategoryMas objtbl)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    HRMEntities objent = new HRMEntities();
                    int code = Convert.ToInt32(Session["id"].ToString());
                    var checkpos = (from e in objent.tblPositionCategoryMas where e.vchPosCatName == objtbl.vchPosCatName select e).FirstOrDefault();
                    if (checkpos == null)
                    {
                        objtbl.vchcreatedby = Session["descript"].ToString();
                        objtbl.dtcreated = DateTime.Now;
                        objtbl.vchhostname = Session["hostname"].ToString();
                        objtbl.vchipused = Session["ipused"].ToString();
                        objtbl.intcode = Convert.ToInt32(Session["id"].ToString());
                        objtbl.intyr = Convert.ToInt32(Session["yr"].ToString());
                        objent.tblPositionCategoryMas.Add(objtbl);
                        objent.SaveChanges();
                        TempData["Success"] = "Record saved successfully!";
                        return RedirectToAction("AddNewPosCat");
                    }
                    else
                    {
                        TempData["ModelError"] = "Record already existing in position category master";
                        return RedirectToAction("AddNewPosCat");
                    }
                }
                ModelState.AddModelError("vchPosCatName", "Model error generated name should not be blank");
                return View();
            }
            return RedirectToAction("_SessionError1", "Home");
        }

        //Edit Esisting
        public ActionResult EditPosCategory(int id)
        {
            HRMEntities objent = new HRMEntities();
            var selectedCat = (from e in objent.tblPositionCategoryMas where e.intid == id select e).FirstOrDefault();
            if (selectedCat != null)
            {
                return View(selectedCat);
            }
            else
            {
                TempData["ModelError"] = "Selected category not avilable in database";
                return View();
            }
        }

            [HttpPost]
        public ActionResult EditPosCategory(tblPositionCategoryMas objtbl)
        {
            HRMEntities objentity = new HRMEntities();
            if (ModelState.IsValid)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedrecord = (from e in objentity.tblPositionCategoryMas where e.intid == objtbl.intid select e).FirstOrDefault();
                if (selectedrecord != null)
                {
                    selectedrecord.vchPosCatName = objtbl.vchPosCatName;
                    selectedrecord.dtupdated = DateTime.Now;
                    selectedrecord.vchupdatedby = Session["descript"].ToString();
                    selectedrecord.vchupdatedhostname = Session["hostname"].ToString();
                    selectedrecord.vchupdatedipused = Session["ipused"].ToString();
                    objentity.SaveChanges();
                    TempData["Success"] = "Record updated successfully!";
                    return RedirectToAction("EditPosCategory");

                }
            }
            TempData["Error"] = "Database error generated";
            return RedirectToAction("EditPosCategory");
        }

        //delete
        public ActionResult DeletePosCategory(int id)
        {
            HRMEntities objentity = new HRMEntities();
            var selected = (from e in objentity.tblPositionCategoryMas where e.intid == id select e).FirstOrDefault();
            if (selected != null)
            {
                objentity.tblPositionCategoryMas.Remove(selected);
                objentity.SaveChanges();
                TempData["Success"] = "Record deleted successfully";
            }
            return RedirectToAction("ViewPosCategory");
        }

        #endregion

        #region Approval level (tblPosDesiMap) master add, update, view and delete actions.
        //view all
        public ActionResult ViewAllMap()
        {
            HRMEntities hrmentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            //Select mapped Position from Master
            var list = (from e in hrmentity.tblPositionCategoryMas where e.BitDesiMapping==true select e).ToList();
            if (list.Count != 0)
            {
                TempData["Success"] = "Found" +" " + list.Count + " " + "mapping";
                return View(list);
            }
            else
            {
                TempData["Empty"] = "No Record found in database!";
                return View();
            }
        }

        //add new record
        public ActionResult AddNewMapping()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            NewMapingViewModel objmodel = new NewMapingViewModel();
            HRMEntities hrmentity = new HRMEntities();
            //select only unmapped positions
            var PosiList = (from e in hrmentity.tblPositionCategoryMas where e.BitDesiMapping==false select e).ToList();
            var DesiList = (from e in hrmentity.tblDesignationMas select e).ToList();
            List<SelectListItem> PoList = new List<SelectListItem>();
            List<SelectListItem> DesList = new List<SelectListItem>();
            List<string> SelectedDesi = new List<string>();
            foreach(var posi in PosiList)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = posi.vchPosCatName,
                    Value = posi.intid.ToString()
                };
                PoList.Add(selectListItem);
            }
            foreach(var desi in DesiList)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = desi.vchdesignation,
                    Value = desi.intid.ToString(),
                    Selected=Convert.ToBoolean(desi.IsSelected)
                };
                DesList.Add(selectListItem);
            }
            objmodel.AllPosition = new SelectList(PoList,"Text","Value");
            objmodel.AllDesignation = DesList;
            objmodel.SelectedDesignation = SelectedDesi;
            ViewBag.SelectPosi = new SelectList(PoList, "Text", "Value");
            //ViewBag.SelectDesi = new SelectList(DesList, "Text", "Value");
            return View(objmodel);
        }

        [HttpPost]
        public JsonResult SaveNewMap(string YesValiDesi, string NoValiDesi, string PositionId,string LastAssessment) //,string PositionText)
        {
            if (Session["descript"] != null)
            {
                HRMEntities hrmentity = new HRMEntities();
                tblPosDesiMap objnew = new tblPosDesiMap();
                int code = Convert.ToInt32(Session["id"].ToString());
                int year = Convert.ToInt32(Session["yr"].ToString());
                int PosId = Convert.ToInt32(PositionId);
                //update user allow last assessment
                int designation_id = Convert.ToInt16(LastAssessment);               
                var checkPosDesi = (from e in hrmentity.tblPosDesiMap where e.fk_PosCatid == PosId select e).FirstOrDefault();
                if (checkPosDesi != null)
                {
                    var output3 = "Record already existing in database, edit/update it from view all!";
                    return Json(new { success = 3, output3, JsonRequestBehavior.AllowGet });
                }

                if (YesValiDesi != "" && YesValiDesi != null)// && NoValiDesi != "" && NoValiDesi != null)
                {
                    //add yes vali
                    string[] DYesVali = YesValiDesi.Split(',');
                    foreach (var yes in DYesVali)
                    {
                        int DesiId = Convert.ToInt32(yes);
                        objnew.fk_PosCatid = PosId;
                        objnew.fk_desiid = DesiId;
                        objnew.vchcreatedby = Session["descript"].ToString();
                        objnew.dtcreated = DateTime.Now;
                        objnew.vchhostname = Session["hostname"].ToString();
                        objnew.vchipused = Session["ipused"].ToString();
                        objnew.IsSelected = true;
                        objnew.intcode = code;
                        objnew.intyr = year;
                        hrmentity.tblPosDesiMap.Add(objnew);
                        hrmentity.SaveChanges();
                    }
                    
                    //add no vali
                    if (NoValiDesi != "" && NoValiDesi != null)
                    {
                        string[] DNoVali = NoValiDesi.Split(',');
                        foreach (var yes in DNoVali)
                        {
                            int DesiId = Convert.ToInt32(yes);
                            objnew.fk_PosCatid = PosId;
                            objnew.fk_desiid = DesiId;
                            objnew.vchcreatedby = Session["descript"].ToString();
                            objnew.dtcreated = DateTime.Now;
                            objnew.vchhostname = Session["hostname"].ToString();
                            objnew.vchipused = Session["ipused"].ToString();
                            objnew.IsSelected = false;
                            objnew.intcode = code;
                            hrmentity.tblPosDesiMap.Add(objnew);
                            hrmentity.SaveChanges();
                        }
                        //update mapping info in master table
                        tblPositionCategoryMas objmaster = new tblPositionCategoryMas();
                        //update selected Position
                        var selectedPos = (from e in hrmentity.tblPositionCategoryMas where e.intid == PosId select e).FirstOrDefault();
                        selectedPos.BitDesiMapping = true;
                        //update allow for last assessment desigantion 
                        var allowAssDesi = (from e in hrmentity.tblPosDesiMap where e.fk_desiid == designation_id && e.intcode == code select e).FirstOrDefault();
                        allowAssDesi.bitIsLastAssessment = true;                      
                        hrmentity.SaveChanges();
                    }
                    var output1 = "Record saved successfully!";
                    return Json(new { success = 1, output1, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    var output4 = "Selection should not be null or empty!";
                    return Json(new { success = 4, output4, JsonRequestBehavior.AllowGet });
                }
            }
            else
            {
                var output2 = "Session timeout error!";
                return Json(new { success = 2, output2, JsonRequestBehavior.AllowGet });
            }
        }

        //edit action
        public ActionResult EditMapp(int id)
        {
            HRMEntities hrentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            //get selected Position
            var position = (from e in hrentity.tblPositionCategoryMas where e.intid == id select e).FirstOrDefault();
            //get final assessment allowed
            var finalassessment = (from e in hrentity.tblPosDesiMap where e.fk_PosCatid == id && e.bitIsLastAssessment == true select e).FirstOrDefault();
            ViewBag.SelectedPosi = position.vchPosCatName.ToString();
            ViewBag.SelectedPosiID = position.intid.ToString();          
            var selected = (from e in hrentity.spEditPosDesiMap(id) select e).ToList();
            List<SelectListItem> SList = new List<SelectListItem>();
            List<SelectListItem> AllowedFinalAssessment = new List<SelectListItem>();
            foreach (var pos in selected)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = pos.vchdesignation.ToString(),
                    Value = pos.intid.ToString(),
                    Selected = Convert.ToBoolean(pos.IsSelected)
                    //Group=pos.bitIsLastAssessment.ToString()
                };
                SList.Add(selectListItem);
            }
            ViewBag.SelectedRecrod = SList;
            //foreach(var islastass in selected)
            //{
            //    SelectListItem selectListItem = new SelectListItem
            //    {
            //        Text = islastass.vchdesignation.ToString(),
            //        Value = islastass.intid.ToString(),
            //        Selected = Convert.ToBoolean(islastass.bitIsLastAssessment)
            //        //Group=pos.bitIsLastAssessment.ToString()
            //    };
            //    AllowedFinalAssessment.Add(selectListItem);
            //    ViewBag.AloowedLastAss = AllowedFinalAssessment;
            //}
            if (finalassessment != null)
            {
                ViewBag.bitIsLast = finalassessment.bitIsLastAssessment.ToString();
                ViewBag.Desiid=finalassessment.fk_desiid.ToString();
            }

            return View();
        }

        [HttpPost]
        public JsonResult UpdateMapping(string PositionId,string PositionText,string UpdateYesValiDesi,string UpdateNoValiDesi,string FAssEdit)
        {
            if (Session["descript"] != null)
            {
                HRMEntities hrentity = new HRMEntities();
                int posiid = Convert.ToInt32(PositionId);
                int code = Convert.ToInt32(Session["id"].ToString());
                int year = Convert.ToInt32(Session["yr"].ToString());
                if (FAssEdit != null && FAssEdit != "")
                {
                    int desiid = Convert.ToInt32(FAssEdit);
                    //get old finalassessment
                    var oldassessment = (from e in hrentity.tblPosDesiMap where e.fk_PosCatid == posiid && e.bitIsLastAssessment == true select e).FirstOrDefault();
                    //get bew seleced final assessment Mapping 
                    var selected_Fass = (from e in hrentity.tblPosDesiMap where e.fk_PosCatid == posiid && e.fk_desiid == desiid select e).FirstOrDefault();
                    if (selected_Fass != null)
                    {
                        if (oldassessment != null)
                        {
                            if (oldassessment.fk_desiid == selected_Fass.fk_desiid)
                            {
                                selected_Fass.bitIsLastAssessment = true;
                                hrentity.SaveChanges();
                            }
                            else
                            {
                                oldassessment.bitIsLastAssessment = false;
                                selected_Fass.bitIsLastAssessment = true;
                                hrentity.SaveChanges();
                            }
                        }
                        else
                        {                           
                            selected_Fass.bitIsLastAssessment = true;
                            hrentity.SaveChanges();
                        }
                    }
                }
                if (UpdateYesValiDesi != "" && UpdateYesValiDesi!=null)
                {
                    string[] UDYesVali = UpdateYesValiDesi.Split(',');
                    foreach (var yes in UDYesVali)
                    {
                        int DesiId = Convert.ToInt32(yes);
                        var selectedDessi = (from e in hrentity.tblPosDesiMap where e.fk_PosCatid == posiid && e.fk_desiid == DesiId select e).FirstOrDefault();
                        if (selectedDessi == null)
                        {
                            tblPosDesiMap objnew = new tblPosDesiMap();
                            objnew.fk_PosCatid = posiid;
                            objnew.fk_desiid = DesiId;
                            objnew.vchcreatedby = Session["descript"].ToString();
                            objnew.dtcreated = DateTime.Now;
                            objnew.vchhostname = Session["hostname"].ToString();
                            objnew.vchipused = Session["ipused"].ToString();
                            objnew.IsSelected = true;
                            objnew.intcode = code;
                            objnew.intyr = year;
                            hrentity.tblPosDesiMap.Add(objnew);
                            hrentity.SaveChanges();
                        }
                        else
                        {
                            //selectedDessi.fk_PosCatid = PosId;
                            selectedDessi.fk_desiid = DesiId;
                            selectedDessi.vchupdatedby = Session["descript"].ToString();
                            selectedDessi.dtupdated = DateTime.Now;
                            selectedDessi.vchupdatedhostname = Session["hostname"].ToString();
                            selectedDessi.vchupdatedipused = Session["ipused"].ToString();
                            selectedDessi.IsSelected = true;
                            hrentity.SaveChanges();
                        }
                    }
                }
                if (UpdateNoValiDesi != "" && UpdateNoValiDesi!=null)
                {
                    string[] UpNoVali = UpdateNoValiDesi.Split(',');
                    foreach (var No in UpNoVali)
                    {
                        int DesiId = Convert.ToInt32(No);
                        var selectedDessi = (from e in hrentity.tblPosDesiMap where e.fk_PosCatid == posiid && e.fk_desiid == DesiId select e).FirstOrDefault();
                        if (selectedDessi == null)
                        {
                            tblPosDesiMap objnew = new tblPosDesiMap();
                            objnew.fk_desiid = DesiId;
                            objnew.fk_PosCatid = posiid;
                            objnew.vchupdatedby = Session["descript"].ToString();
                            objnew.dtupdated = DateTime.Now;
                            objnew.vchupdatedhostname = Session["hostname"].ToString();
                            objnew.vchupdatedipused = Session["ipused"].ToString();
                            objnew.IsSelected = false;
                            objnew.intcode = code;
                            objnew.intyr = year;
                            hrentity.tblPosDesiMap.Add(objnew);
                            hrentity.SaveChanges();
                        }
                        else
                        {
                            //selectedDessi.fk_PosCatid = PosId;
                            selectedDessi.fk_desiid = DesiId;
                            selectedDessi.vchupdatedby = Session["descript"].ToString();
                            selectedDessi.dtupdated = DateTime.Now;
                            selectedDessi.vchupdatedhostname = Session["hostname"].ToString();
                            selectedDessi.vchupdatedipused = Session["ipused"].ToString();
                            selectedDessi.IsSelected = false;
                            hrentity.SaveChanges();
                        }
                    }
                }
                var output1 = "Record updated successfully!";
                return Json(new { Success = 1, output1, JsonRequestBehavior.AllowGet });
            }
            else
            {
                var output2 = "Session timeout error!";
                return Json(new { Success = 2, output2, JsonRequestBehavior.AllowGet });
            }
        }

        //delete Mapping
        public ActionResult DeleteMapp(int id)
        {
            HRMEntities hrentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            var selected = (from e in hrentity.tblPosDesiMap where e.fk_PosCatid == id select e).ToList();
            hrentity.tblPosDesiMap.RemoveRange(selected);
            hrentity.SaveChanges();
            TempData["DeleteDone"] = "Record removed successfully!";
            //get change status in master table
            var mastab = (from e in hrentity.tblPositionCategoryMas where e.intid == id select e).FirstOrDefault();
            mastab.BitDesiMapping = false;
            hrentity.SaveChanges();
            return RedirectToAction("ViewAllMap");
        }
        #endregion

        #region Document and position mapping actions View, Add, Delete, Update
        public ActionResult ViewAllDocMap()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            HRMEntities hrentity = new HRMEntities();
            var selectedPosition = (from e in hrentity.tblPositionCategoryMas where e.BitDocMapping == true select e).ToList();
            if(selectedPosition.Count!=0)
            {
                TempData["Success"] = "Found" + " " + selectedPosition.Count + " " + "mapping";
                return View(selectedPosition);
            }
            else
            {
                TempData["Empty"] = "No Document mapping found in database!";
                return View();
            }
        }

        public ActionResult AddnewDocMap()
        {
            if (Session["descript"] != null)
            {
                NewDocMapModelView objmodel = new NewDocMapModelView();
                HRMEntities hrmentity = new HRMEntities();
                int code = Convert.ToInt32(Session["id"].ToString());
                int year = Convert.ToInt32(Session["yr"].ToString());
                //select only unmapped positions
                var PosiList = (from e in hrmentity.tblPositionCategoryMas where e.BitDocMapping == false select e).ToList();
                //var PosiListForAuthor = (from e in hrmentity.tblDocMas select e).ToList();
                //var PosiListForComplete = (from e in hrmentity.tblDocMas select e).ToList();
                var DocMasList = (from e in hrmentity.tblDocMas select e).ToList();

                //For Seleced DOcument
                List<SelectListItem> PoList = new List<SelectListItem>();
                foreach (var posi in PosiList)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Text = posi.vchPosCatName,
                        Value = posi.intid.ToString(),
                        //Selected=pos
                    };
                    PoList.Add(selectListItem);
                }
                //For Selected Must Author
                List<SelectListItem> ForAuthor = new List<SelectListItem>();
                foreach (var author in DocMasList)
                {
                    SelectListItem selectListItem1 = new SelectListItem
                    {
                        Text = author.vchdocname,
                        Value = author.intid.ToString(),
                        //Selected=pos
                    };
                    ForAuthor.Add(selectListItem1);
                }
                //For Seleced DOcument
                List<SelectListItem> CompleteList = new List<SelectListItem>();
                foreach (var posi in DocMasList)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Text = posi.vchdocname,
                        Value = posi.intid.ToString(),
                        //Selected=pos
                    };
                    CompleteList.Add(selectListItem);
                }
                //FOr DOcument Mas list
                List<SelectListItem> DocList = new List<SelectListItem>();
                foreach (var doc in DocMasList)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Text = doc.vchdocname,
                        Value = doc.intid.ToString(),
                        Selected = Convert.ToBoolean(doc.BitIsSelected)
                    };
                    DocList.Add(selectListItem);
                }
                objmodel.AllPosition = new SelectList(PoList, "Text", "Value");
                objmodel.AllDocument = DocList;
                objmodel.AllForComplete = CompleteList;
                objmodel.AllForAuthor = ForAuthor;
                //objmodel.SelectedDocument = SelectedDoc;
                ViewBag.SelectDoc = new SelectList(PoList, "Text", "Value");
                // ViewBag.SelectAuthor = new SelectList(ForAuthor, "Text", "Value");
                //ViewBag.SelectComplete = new SelectList(CompleteList, "Text", "Value");
                //ViewBag.SelectDesi = new SelectList(DesList, "Text", "Value");
                return View(objmodel);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public JsonResult SaveNewDocMap(string YesValiDoc,string NoValiDoc,string YesAuthor,string NoAuthor,string YesComplete,string NoComplete,string PoID)
        {
            if (Session["descript"] != null)
            {
                HRMEntities hrmentity = new HRMEntities();
                tblPosDocMap PosDocNew = new tblPosDocMap();
                int code = Convert.ToInt32(Session["id"].ToString());
                int year = Convert.ToInt32(Session["yr"].ToString());
                int PosId = Convert.ToInt32(PoID);
                var checkPosDoc = (from e in hrmentity.tblPosDocMap where e.fk_PosCatid == PosId select e).FirstOrDefault();
                if (checkPosDoc != null)
                {
                    var output3 = "Record already existing in database, edit/update it from view all!";
                    return Json(new { success = 3, output3, JsonRequestBehavior.AllowGet });
                }
                //Up object of Doc Mapping
                if (YesValiDoc != "" && YesValiDoc != null) // && NoValiDoc != "" && NoValiDoc != null)
                {
                    //add yes vali
                    string[] DYesVali = YesValiDoc.Split(',');
                    foreach (var yes in DYesVali)
                    {
                        int DesiId = Convert.ToInt32(yes);
                        PosDocNew.fk_PosCatid = PosId;
                        PosDocNew.fk_docid = DesiId;
                        PosDocNew.vchcreatedby = Session["descript"].ToString();
                        PosDocNew.dtcreated = DateTime.Now;
                        PosDocNew.vchhostname = Session["hostname"].ToString();
                        PosDocNew.vchipused = Session["ipused"].ToString();
                        PosDocNew.IsSelected = true;
                        PosDocNew.intcode = code;
                        PosDocNew.intyr = year;
                        hrmentity.tblPosDocMap.Add(PosDocNew);
                        hrmentity.SaveChanges();
                    }
                    //update mapping info in master table
                    tblPositionCategoryMas objmaster = new tblPositionCategoryMas();
                    var selectedPos = (from e in hrmentity.tblPositionCategoryMas where e.intid == PosId select e).FirstOrDefault();
                    selectedPos.BitDocMapping = true;
                    hrmentity.SaveChanges();

                    //add no vali
                    if (NoValiDoc != "" && NoValiDoc != null)
                    {
                        string[] DNoVali = NoValiDoc.Split(',');
                        foreach (var yes in DNoVali)
                        {
                            int DesiId = Convert.ToInt32(yes);
                            PosDocNew.fk_PosCatid = PosId;
                            PosDocNew.fk_docid = DesiId;
                            PosDocNew.vchcreatedby = Session["descript"].ToString();
                            PosDocNew.dtcreated = DateTime.Now;
                            PosDocNew.vchhostname = Session["hostname"].ToString();
                            PosDocNew.vchipused = Session["ipused"].ToString();
                            PosDocNew.IsSelected = false;
                            PosDocNew.intcode = code;
                            PosDocNew.intyr = year;
                            hrmentity.tblPosDocMap.Add(PosDocNew);
                            hrmentity.SaveChanges();
                        }
                    }                   
                    //Save doc required at the time for authorization
                    if(YesAuthor!="" && YesAuthor != null)
                    {
                        string[] HanAuthor = YesAuthor.Split(',');
                        foreach(var yes in HanAuthor)
                        {
                            int docid = Convert.ToInt32(yes);
                            var selected = (from e in hrmentity.tblPosDocMap where e.fk_PosCatid == PosId && e.fk_docid == docid select e).FirstOrDefault();
                            if (selected != null)
                            {
                                selected.bitRequireForAuthorization = true;
                                hrmentity.SaveChanges();
                            }
                        }
                       
                    }
                    //Save doc required at the time of complete/fully active employee
                    if(YesComplete!=null && YesComplete != "")
                    {
                        string[] HanComplete = YesComplete.Split(',');
                        foreach(var yes in HanComplete)
                        {
                            int docid = Convert.ToInt32(yes);
                            var selected = (from e in hrmentity.tblPosDocMap where e.fk_PosCatid == PosId && e.fk_docid == docid select e).FirstOrDefault();
                            if (selected != null)
                            {
                                selected.bitRequireForPartialToComplete = true;
                                hrmentity.SaveChanges();
                            }
                        }
                    }
                    var output1 = "Record saved successfully!";
                    return Json(new { success = 1, output1, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    var output4 = "Selection should not be null or empty!";
                    return Json(new { success = 4, output4, JsonRequestBehavior.AllowGet });
                }
                
            }
            else
            {
                var output2 = "Session timeout error!";
                return Json(new { success = 2, output2, JsonRequestBehavior.AllowGet });
            }
        }

        public ActionResult EditDocMapp(int id)
        {
            HRMEntities hrentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            
            //get selected Position
            var position = (from e in hrentity.tblPositionCategoryMas where e.intid == id select e).FirstOrDefault();
            ViewBag.SelectedPosi = position.vchPosCatName.ToString();
            ViewBag.SelectedPosiID = position.intid.ToString();
            //Model object
            List<spEditPosDocMap_Result> objmodel = new List<spEditPosDocMap_Result>();
            //get selcted mapping
            objmodel = (from e in hrentity.spEditPosDocMap(id) select e).ToList();           
            //ViewBag.SelectedRecrod = selected;
            return View(objmodel);
        }

        public JsonResult UpdateDocMapping(string UpdateYValiDesi,string UpdateNValiDesi,string PosId,string UpdateAuthorYes,string UpdateAuthorNo,string UpdateCompleteYes,string UpdateCompleteNo,string PosText)
        {
            if (Session["descript"] != null)
            {
                HRMEntities hrentity = new HRMEntities();
                int posiid = Convert.ToInt32(PosId);
                int code = Convert.ToInt32(Session["id"].ToString());
                int year = Convert.ToInt32(Session["yr"].ToString());
                if (UpdateYValiDesi != "" && UpdateYValiDesi != null)
                {
                    string[] DocYVali = UpdateYValiDesi.Split(',');
                    foreach (var yes in DocYVali)
                    {
                        int docid = Convert.ToInt32(yes);
                        var selectedDessi = (from e in hrentity.tblPosDocMap where e.fk_PosCatid == posiid && e.fk_docid == docid select e).FirstOrDefault();
                        //selectedDessi.fk_PosCatid = PosId;
                        if (selectedDessi == null)
                        {
                            var newselecteddoc = (from e in hrentity.tblDocMas where e.intid == docid select e).FirstOrDefault();
                            if (newselecteddoc != null)
                            {
                                tblPosDocMap objenew = new tblPosDocMap();
                                int positionid = Convert.ToInt16(posiid);
                                objenew.fk_docid = docid;
                                objenew.fk_PosCatid = positionid;
                                objenew.vchupdatedby = Session["descript"].ToString();
                                objenew.dtupdated = DateTime.Now;
                                objenew.vchupdatedhostname = Session["hostname"].ToString();
                                objenew.vchupdatedipused = Session["ipused"].ToString();
                                objenew.IsSelected = true;
                                objenew.intcode = code;
                                objenew.intyr = year;
                                hrentity.tblPosDocMap.Add(objenew);
                                hrentity.SaveChanges();
                            }
                        }
                        else
                        {
                            selectedDessi.fk_docid = docid;
                            selectedDessi.vchupdatedby = Session["descript"].ToString();
                            selectedDessi.dtupdated = DateTime.Now;
                            selectedDessi.vchupdatedhostname = Session["hostname"].ToString();
                            selectedDessi.vchupdatedipused = Session["ipused"].ToString();
                            selectedDessi.IsSelected = true;
                            hrentity.SaveChanges();
                        }
                    }
                }
                if (UpdateNValiDesi != "" && UpdateNValiDesi != null)
                {
                    string[] UpNVali = UpdateNValiDesi.Split(',');
                    foreach (var No in UpNVali)
                    {
                        int Docid = Convert.ToInt32(No);
                        var selectedDessi = (from e in hrentity.tblPosDocMap where e.fk_PosCatid == posiid && e.fk_docid == Docid select e).FirstOrDefault();
                        if (selectedDessi == null)
                        {
                            tblPosDocMap objenew = new tblPosDocMap();
                            int positionid = Convert.ToInt16(posiid);
                            objenew.fk_docid = Docid;
                            objenew.fk_PosCatid = positionid;
                            objenew.vchupdatedby = Session["descript"].ToString();
                            objenew.dtupdated = DateTime.Now;
                            objenew.vchupdatedhostname = Session["hostname"].ToString();
                            objenew.vchupdatedipused = Session["ipused"].ToString();
                            objenew.IsSelected = false;
                            objenew.intcode = code;
                            objenew.intyr = year;
                            hrentity.tblPosDocMap.Add(objenew);
                            hrentity.SaveChanges();
                        }
                        else
                        {
                            //selectedDessi.fk_PosCatid = PosId;
                            selectedDessi.fk_docid = Docid;
                            selectedDessi.vchupdatedby = Session["descript"].ToString();
                            selectedDessi.dtupdated = DateTime.Now;
                            selectedDessi.vchupdatedhostname = Session["hostname"].ToString();
                            selectedDessi.vchupdatedipused = Session["ipused"].ToString();
                            selectedDessi.IsSelected = false;
                            hrentity.SaveChanges();
                        }
                    }
                }
                if (UpdateAuthorYes != "" && UpdateAuthorYes != null)
                {
                    string[] UpAuthorY = UpdateAuthorYes.Split(',');
                    foreach (var yes in UpAuthorY)
                    {
                        int Docid = Convert.ToInt32(yes);
                        var selectedPosDoc = (from e in hrentity.tblPosDocMap where e.fk_PosCatid == posiid && e.fk_docid == Docid select e).FirstOrDefault();
                        if (selectedPosDoc != null)
                        {
                            selectedPosDoc.vchupdatedby = Session["descript"].ToString();
                            selectedPosDoc.dtupdated = DateTime.Now;
                            selectedPosDoc.vchupdatedhostname = Session["hostname"].ToString();
                            selectedPosDoc.vchupdatedipused = Session["ipused"].ToString();
                            selectedPosDoc.bitRequireForAuthorization = true;
                            hrentity.SaveChanges();

                        }
                        else
                        {
                            tblPosDocMap objenew = new tblPosDocMap();
                            int positionid = Convert.ToInt16(posiid);
                            objenew.fk_docid = Docid;
                            objenew.fk_PosCatid = positionid;
                            objenew.vchupdatedby = Session["descript"].ToString();
                            objenew.dtupdated = DateTime.Now;
                            objenew.vchupdatedhostname = Session["hostname"].ToString();
                            objenew.vchupdatedipused = Session["ipused"].ToString();
                            objenew.bitRequireForAuthorization = true;
                            objenew.intcode = code;
                            objenew.intyr = year;
                            hrentity.tblPosDocMap.Add(objenew);
                            hrentity.SaveChanges();
                        }
                    }
                }
                if (UpdateAuthorNo != "" && UpdateAuthorNo != null)
                {
                    string[] UpAuthorN = UpdateAuthorNo.Split(',');
                    foreach (var No in UpAuthorN)
                    {
                        int Docid = Convert.ToInt32(No);
                        var selectedPosDoc = (from e in hrentity.tblPosDocMap where e.fk_PosCatid == posiid && e.fk_docid == Docid select e).FirstOrDefault();
                        if (selectedPosDoc != null)
                        {
                            selectedPosDoc.vchupdatedby = Session["descript"].ToString();
                            selectedPosDoc.dtupdated = DateTime.Now;
                            selectedPosDoc.vchupdatedhostname = Session["hostname"].ToString();
                            selectedPosDoc.vchupdatedipused = Session["ipused"].ToString();
                            selectedPosDoc.bitRequireForAuthorization = false;
                            hrentity.SaveChanges();

                        }
                        else
                        {
                            tblPosDocMap objenew = new tblPosDocMap();
                            int positionid = Convert.ToInt16(posiid);
                            objenew.fk_docid = Docid;
                            objenew.fk_PosCatid = positionid;
                            objenew.vchupdatedby = Session["descript"].ToString();
                            objenew.dtupdated = DateTime.Now;
                            objenew.vchupdatedhostname = Session["hostname"].ToString();
                            objenew.vchupdatedipused = Session["ipused"].ToString();
                            objenew.bitRequireForAuthorization = false;
                            objenew.intcode = code;
                            objenew.intyr = year;
                            hrentity.tblPosDocMap.Add(objenew);
                            hrentity.SaveChanges();
                        }
                    }
                }
                if (UpdateCompleteYes != "" && UpdateCompleteYes != null)
                {
                    string[] CompYes = UpdateCompleteYes.Split(',');
                    foreach (var Yes in CompYes)
                    {
                        int docid = Convert.ToInt32(Yes);
                        var selectedPosDoc = (from e in hrentity.tblPosDocMap where e.fk_PosCatid == posiid && e.fk_docid == docid select e).FirstOrDefault();
                        if (selectedPosDoc != null)
                        {
                            selectedPosDoc.vchupdatedby = Session["descript"].ToString();
                            selectedPosDoc.dtupdated = DateTime.Now;
                            selectedPosDoc.vchupdatedhostname = Session["hostname"].ToString();
                            selectedPosDoc.vchupdatedipused = Session["ipused"].ToString();
                            selectedPosDoc.bitRequireForPartialToComplete = true;
                            hrentity.SaveChanges();

                        }
                        else
                        {
                            tblPosDocMap objenew = new tblPosDocMap();
                            int positionid = Convert.ToInt16(posiid);
                            objenew.fk_docid = docid;
                            objenew.fk_PosCatid = positionid;
                            objenew.vchupdatedby = Session["descript"].ToString();
                            objenew.dtupdated = DateTime.Now;
                            objenew.vchupdatedhostname = Session["hostname"].ToString();
                            objenew.vchupdatedipused = Session["ipused"].ToString();
                            objenew.bitRequireForPartialToComplete = true;
                            objenew.intcode = code;
                            objenew.intyr = year;
                            hrentity.tblPosDocMap.Add(objenew);
                            hrentity.SaveChanges();
                        }
                    }
                }
                if (UpdateCompleteNo != "" && UpdateCompleteNo != null)
                {
                    string[] compNo = UpdateCompleteNo.Split(',');
                    foreach (var no in compNo)
                    {
                        int docid = Convert.ToInt32(no);
                        var selectedPosDoc = (from e in hrentity.tblPosDocMap where e.fk_PosCatid == posiid && e.fk_docid == docid select e).FirstOrDefault();
                        if (selectedPosDoc != null)
                        {
                            selectedPosDoc.vchupdatedby = Session["descript"].ToString();
                            selectedPosDoc.dtupdated = DateTime.Now;
                            selectedPosDoc.vchupdatedhostname = Session["hostname"].ToString();
                            selectedPosDoc.vchupdatedipused = Session["ipused"].ToString();
                            selectedPosDoc.bitRequireForPartialToComplete = false;
                            hrentity.SaveChanges();

                        }
                        else
                        {
                            tblPosDocMap objenew = new tblPosDocMap();
                            int positionid = Convert.ToInt16(posiid);
                            objenew.fk_docid = docid;
                            objenew.fk_PosCatid = positionid;
                            objenew.vchupdatedby = Session["descript"].ToString();
                            objenew.dtupdated = DateTime.Now;
                            objenew.vchupdatedhostname = Session["hostname"].ToString();
                            objenew.vchupdatedipused = Session["ipused"].ToString();
                            objenew.bitRequireForPartialToComplete = false;
                            objenew.intcode = code;
                            objenew.intyr = year;
                            hrentity.tblPosDocMap.Add(objenew);
                            hrentity.SaveChanges();
                        }
                    }
                }
                var output1 = "Record updated successfully!";
                return Json(new { Success = 1, output1, JsonRequestBehavior.AllowGet });
            }
            else
            {
                var output2 = "Session timeout error!";
                return Json(new { Success = 2, output2, JsonRequestBehavior.AllowGet });
            }
        }

        public ActionResult DeleteDocMapp(int id)
        {
            HRMEntities hrentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            var selectedPos = (from e in hrentity.tblPosDocMap where e.fk_PosCatid == id select e).ToList();
            hrentity.tblPosDocMap.RemoveRange(selectedPos);
            hrentity.SaveChanges();
            TempData["DeleteDone"] = "Record removed successfully!";
            //get change status in master table
            var mastab = (from e in hrentity.tblPositionCategoryMas where e.intid == id select e).FirstOrDefault();
            mastab.BitDocMapping = false;
            hrentity.SaveChanges();
            return RedirectToAction("ViewAllDocMap");
        }

        #endregion

        #region Manpower Mapping 
        public ActionResult NewDeptMapping()
        {
            HRMEntities hrentity = new HRMEntities();
            IndusGroupEntities objCompany = new IndusGroupEntities();
            if (Session["descript"] != null)
            {
                List<tblDeptMas> getCounterDept = (from e in hrentity.tblDeptMas where e.bitIsActive==true orderby e.vchdeptname select e).ToList();
                if (getCounterDept.Count() != 0)
                {
                    ViewBag.DeptList = new SelectList(getCounterDept, "intid", "vchdeptname");
                    var allbranches = (from e in objCompany.IndusCompanies  where ((e.intPK >= 2 && e.intPK <= 4) || (e.intPK >= 14 && e.intPK <= 16) || (e.intPK >= 21 && e.intPK <= 26)) select e).ToList();
                    //check is mapped branch
                    var getMapping = (from e in hrentity.tblManPowerMas select e).ToList();
                    ViewBag.MappedBranch = getMapping;
                    List<SelectListItem> allMapping = new List<SelectListItem>();
                    if (getMapping.Count() != 0)
                    {
                        foreach(var map in getMapping)
                        {
                            SelectListItem selectListItem = new SelectListItem()
                            {
                                Text = map.vchBranchName.ToString(),
                                Value = map.fk_Branch.ToString()
                            };
                            allMapping.Add(selectListItem);
                        }
                    }
                    List<SelectListItem> allbranch = new List<SelectListItem>();
                    allbranch.Add(new SelectListItem { Text = "Selection For Branch", Value = "0" });
                    foreach (var branch in allbranches)
                    {
                        SelectListItem selectListItem = new SelectListItem()
                        {
                            Text = branch.descript.ToString(),
                            Value = branch.intPK.ToString()
                        };
                        allbranch.Add(selectListItem);
                    }
                    var CompareList = allbranch.Where(i1 => !allMapping.Any(i2 => i2.Value == i1.Value));
                    //var newList = allbranch.Except(allMapping);
                    ViewBag.AllBranch = new SelectList(CompareList, "Text", "Value");                   
                    return View();
                }
                else
                {
                    ViewBag.Error = "0 Department found for mapping!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
       
        [HttpPost]
        public ActionResult SaveMapping(List<DeptMappingModel> departmentData)
        {
            if (Session["descript"] != null)
            {
                HRMEntities hrentity = new HRMEntities();
                tblManPowerDetail objnew = new tblManPowerDetail();
                tblManPowerMas objMas = new tblManPowerMas();
                //add master entry
                var getSingle = departmentData.FirstOrDefault();
                int branchID = getSingle.intCode;
                string branch = getSingle.branchName;
                objMas.fk_Branch = branchID;
                objMas.vchBranchName = branch;
                objMas.bitIsmapping = true;
                objMas.dtCreated = DateTime.Now;
                objMas.vchCreatedBy = Session["descript"].ToString();
                objMas.vchCreatedHost= Session["hostname"].ToString();
                objMas.vchCreatedIP = Session["ipused"].ToString();
                hrentity.tblManPowerMas.Add(objMas);
                hrentity.SaveChanges();
                //get created mas id
                var getmasid = (from e in hrentity.tblManPowerMas where e.fk_Branch == branchID select e).FirstOrDefault();
                foreach (var getValue in departmentData)
                {
                    //get mas id                    
                    int deptID = getValue.FkDeptId;
                    int counter = getValue.textBoxValues;
                    objnew.fk_masID = getmasid.intid;
                    objnew.fk_deptid = deptID;
                    objnew.intManPowerCount = counter;
                    objnew.intCode = getValue.intCode;
                    objnew.vchCreatedBy = Session["descript"].ToString();
                    objnew.dtCreated = DateTime.Now;
                    objnew.vchCreatedHost = Session["hostname"].ToString();
                    objnew.vchCreatedIP = Session["ipused"].ToString();
                    hrentity.tblManPowerDetail.Add(objnew);
                    hrentity.SaveChanges();                    
                }
                TempData["Success"] = "Mapping saved successfully!";
                return RedirectToAction("NewDeptMapping", "Master");
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult UpdateMapping(int id,int fkid)
        {
            HRMEntities hrentity = new HRMEntities();
            IndusGroupEntities Gpentity = new IndusGroupEntities();
            if (Session["descript"] != null)
            {                
                if (id != 0 && fkid!=0)
                {
                    int branchID = fkid;
                    var getMapDetail = (from e in hrentity.tblManPowerMas where e.fk_Branch == branchID select e).FirstOrDefault();                                  
                    var getMapping = (from e in hrentity.spGetDeptMapping(id, fkid) select e).ToList();
                    if (getMapping.Count() != 0)
                    {
                        ViewBag.BranchID = branchID.ToString();
                        ViewBag.SelectedBranch = getMapDetail.vchBranchName.ToString();
                        return View(getMapping);
                    }
                    else
                    {
                        TempData["Error"] = "Selected mapping not found contact to administrator!";
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
                return RedirectToAction("", "");
            }
        }

        [HttpPost]
        public ActionResult EditMapping(List<UpdateDptMapModel> objUpdate)
        {
            if (Session["descript"] != null)
            {
                HRMEntities hrentity = new HRMEntities();
                tblManPowerDetail objnewDetail = new tblManPowerDetail();
                tblManPowerMas objMas = new tblManPowerMas();
                var getSingle = objUpdate.FirstOrDefault();
                int branchID = getSingle.intCode;
                string branch = getSingle.branchName;
                //get mas entry AND set updated data
                var GetMas = (from e in hrentity.tblManPowerMas where e.fk_Branch == branchID select e).FirstOrDefault();
                if (GetMas != null)
                {
                    GetMas.dtUpdated = DateTime.Now;
                    GetMas.vchUpdatedBy = Session["descript"].ToString();
                    GetMas.vchUpdatedHost = Session["hostname"].ToString();
                    GetMas.vchUpdatedIP = Session["ipused"].ToString();
                    hrentity.SaveChanges();
                }
                //remove old detail entry
                var getDetailList = hrentity.tblManPowerDetail.Where(e => e.intCode == branchID).ToList();
                hrentity.tblManPowerDetail.RemoveRange(getDetailList);
                hrentity.SaveChanges();
                //new entry
                foreach (var newEntry in objUpdate)
                {
                    objnewDetail.fk_masID = GetMas.intid;
                    objnewDetail.fk_deptid = newEntry.FkDeptId;
                    objnewDetail.intManPowerCount = newEntry.textBoxValues;
                    objnewDetail.intCode = branchID;
                    objnewDetail.vchCreatedBy = Session["descript"].ToString();
                    objnewDetail.dtCreated = DateTime.Now;
                    objnewDetail.vchCreatedHost = Session["hostname"].ToString();
                    objnewDetail.vchCreatedIP = Session["ipused"].ToString();
                    hrentity.tblManPowerDetail.Add(objnewDetail);
                    hrentity.SaveChanges();
                }
                //TempData["Success"] = "Mapping update successfully!";
                return RedirectToAction("NewDeptMapping", "Master");                     

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //View Mapping Branch Wise
        public ActionResult ManPowerView(int id)
        {
            if (Session["descript"] != null)
            {
                if (id == 0)
                {                    
                    HRMEntities hrentity = new HRMEntities();
                    int code = Convert.ToInt32(Session["id"].ToString());
                    var getMapping = (from e in hrentity.spGetBranchMapping(code) orderby e.DeptName select e).ToList();
                    if (getMapping.Count() != 0)
                    {
                        return View(getMapping);
                    }
                    else
                    {
                        TempData["Empty"] = "Mapping does not found against your branch!";
                        return View();
                    }
                }
                else
                {
                    HRMEntities hrentity = new HRMEntities();
                    int code = id;
                    var getMapping = (from e in hrentity.spGetBranchMapping(code) orderby e.DeptName select e).ToList();
                    if (getMapping.Count() != 0)
                    {
                        return View(getMapping);
                    }
                    else
                    {
                        TempData["Empty"] = "Mapping does not found against your branch!";
                        return View();
                    }
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }            
        }
        #endregion

        #region Other Document Master
        //get Department master
        public ActionResult OtherDocument()
        {
            return View();
        }

        //create new department
        [HttpPost]
        public ActionResult OtherDocument(tblOtherDocumentMas newOtDoc)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    HRMEntities objentity = new HRMEntities();
                    int code = Convert.ToInt32(Session["id"].ToString());
                    string ndoc = newOtDoc.vchDocName.ToString();
                    var newdpt1 = (from e in objentity.tblOtherDocumentMas where e.vchDocName == ndoc select e).FirstOrDefault();
                    if (newdpt1 != null)
                    {
                        ModelState.AddModelError("vchDocName", "This department already existing in department master");
                    }
                    else
                    {
                        newOtDoc.vchDocName = ndoc;
                        newOtDoc.vchCretaedBy = Session["descript"].ToString();
                        newOtDoc.dtCretaed = DateTime.Now;
                        newOtDoc.intcode = code;                   
                        newOtDoc.vchCretaedIP = Session["ipused"].ToString();
                        newOtDoc.vchCretaedHost = Session["hostname"].ToString();
                        objentity.tblOtherDocumentMas.Add(newOtDoc);
                        objentity.SaveChanges();
                        TempData["Success"] = "New document saved successfully!";
                        return RedirectToAction("OtherDocument");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            return View();
        }

        //view all department
        public ActionResult ViewOtherDoc()
        {
            if (Session["descript"]!=null)
            {
                HRMEntities objtt = new HRMEntities();
                int code = Convert.ToInt32(Session["id"].ToString());
                List<tblOtherDocumentMas> doclist = (from e in objtt.tblOtherDocumentMas select e).ToList();
                if (doclist == null || doclist.Count == 0)
                {
                    TempData["Empty"] = "0 records found in database";
                    return View("ViewOtherDoc");
                }
                else
                {
                    return View("ViewOtherDoc", doclist);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
           
        }

        //Edit Department
        public ActionResult EditOtherDocument(int id)
        {
            HRMEntities objentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectedtitile = (from e in objentity.tblOtherDocumentMas where e.intid == id select e).FirstOrDefault();
            if (selectedtitile == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(selectedtitile);
            }
        }

        //update Department
        [HttpPost]
        public ActionResult EditOtherDocument(tblOtherDocumentMas tblobj)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    HRMEntities objentity = new HRMEntities();
                    int code = Convert.ToInt32(Session["id"].ToString());
                    var selectedMas = objentity.tblOtherDocumentMas.Single(m => m.intid == tblobj.intid);
                    selectedMas.vchDocName = tblobj.vchDocName.ToString();
                    selectedMas.bitISMultipleRecords = tblobj.bitISMultipleRecords;                  
                    selectedMas.vchUpdatedBy = Session["descript"].ToString();
                    selectedMas.vchUpdatedHost = Session["hostname"].ToString();
                    selectedMas.vchUpdatedIP = Session["ipused"].ToString();
                    selectedMas.dtUpdated = DateTime.Now;
                    objentity.SaveChanges();
                    TempData["Success"] = "Updated successfully!";
                    return RedirectToAction("ViewOtherDoc");
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
        }
        #endregion
    }
}

