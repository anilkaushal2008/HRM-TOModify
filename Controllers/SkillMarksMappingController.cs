using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
using HRM.Models;

namespace HRM.Controllers
{
    public class SkillMarksMappingController : Controller
    {
        HRMEntities hrentity = new HRMEntities();
        // GET: SkillMarksMapping
        public ActionResult Index()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            var getallmapping = (from e in hrentity.tblSkillMarksAndPositionMappMas select e).ToList();
            if (getallmapping.Count != 0)
            {
                return View(getallmapping);
            }
            else
            {
                TempData["Empty"] = "No record mapping found in database";
                return View();
            }
        }

        // GET: SkillMarksMapping/Create
        public ActionResult Create()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            var allposii = (from e in hrentity.tblPositionCategoryMas where e.BitDesiMapping == true && e.BitSkillMapping==false select e).ToList();
            if (allposii.Count != 0)
            {
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
                return View();
            }
            else
            {
                TempData["PossiEmpty"] = "No new position avilable for mapping in database!";
                return View();
            }
        }

        // POST: SkillMarksMapping/Create
        [HttpPost]
        public ActionResult Create(tblSkillMarksAndPositionMappMas newobj)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            if (ModelState.IsValid)
            {
                //check already existing
                int positionid = Convert.ToInt32(newobj.fk_PositionID);
                var check = (from e in hrentity.tblSkillMarksAndPositionMappMas where e.fk_PositionID == positionid select e).FirstOrDefault();
                var selectedPositionMap = (from e in hrentity.tblPositionCategoryMas where e.intid == positionid select e).FirstOrDefault();
                if (check != null)
                {
                    TempData["Error"] = "Selected position already mapped with skill, update it from edit view";
                    return RedirectToAction("Create");
                }
                else
                {
                    newobj.BitMapped = true;
                    newobj.dtCreated = DateTime.Now;
                    newobj.vchCreatedBy = Session["descript"].ToString();
                    newobj.vchHostname = Session["hostname"].ToString();
                    newobj.vchIpUsed = Session["ipused"].ToString();
                    newobj.intCode = code;
                    newobj.intYear = year;
                    hrentity.tblSkillMarksAndPositionMappMas.Add(newobj);
                    hrentity.SaveChanges();
                    TempData["Success"] = "Skill marks mapping saved succesfully!";                
                    //Master Position entry
                    if(selectedPositionMap != null)
                    {
                        selectedPositionMap.BitSkillMapping = true;
                        hrentity.SaveChanges();
                    }
                    return RedirectToAction("Create");

                }
            }
           else
            {
                return View();
            }
        }

        // GET: SkillMarksMapping/Edit/5
        public ActionResult Edit(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            var selectedskill = (from e in hrentity.tblSkillMarksAndPositionMappMas where e.intid == id select e).FirstOrDefault();
            if(selectedskill!=null)
            {
                return View(selectedskill);
            }
            else
            {
                TempData["Error"] = "Selected position id not found";
                return View();
            }
        }

        // POST: SkillMarksMapping/Edit/5
        [HttpPost]
        public ActionResult Edit(tblSkillMarksAndPositionMappMas objupdated)
        {
            int id = Convert.ToInt32(objupdated.fk_PositionID);
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            if (id != 0)
            {
                var selected = (from e in hrentity.tblSkillMarksAndPositionMappMas where e.fk_PositionID == id select e).FirstOrDefault();
                if (selected != null)
                {
                    selected.intSkillMarksFm = objupdated.intSkillMarksFm;
                    selected.intSkillMarksTo = objupdated.intSkillMarksTo;
                    selected.dtUpdated = DateTime.Now;
                    selected.vchUpdatedBy = Session["descript"].ToString();
                    selected.vchUpdatedHost = Session["hostname"].ToString();
                    selected.vchUpdatedIpUsed = Session["ipused"].ToString();
                    hrentity.SaveChanges();
                    TempData["Success"] = "Skill mapping updated succesfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                TempData["Error"] = "Error generated";
                return View();
            }
        }

        // GET: SkillMarksMapping/Delete/5
        public ActionResult Delete(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            int year = Convert.ToInt32(Session["yr"].ToString());
            if (id != 0)
            {
                var selectedrecord = (from e in hrentity.tblSkillMarksAndPositionMappMas where e.intid == id select e).FirstOrDefault();
                if(selectedrecord!=null)
                {
                    hrentity.tblSkillMarksAndPositionMappMas.Remove(selectedrecord);
                    hrentity.SaveChanges();
                    TempData["Success"] = "Mapping record deleted successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Error"] = "Error generated to delete it please conntact to admin!";
                return RedirectToAction("Index");
            }
        }
    }
}
