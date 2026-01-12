using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Linq;
using System.Web;
using System.Web.Mvc; 

namespace HRM.Controllers
{
    public class GroupController : Controller
    {
        HRMEntities hrentity = new HRMEntities();
        // GET: Group
        public ActionResult CreateNewGp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateNewGp(tblGroupMaster objtbl)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int yr = Convert.ToInt32(Session["yr"].ToString());
                    //check if permission already existing in permission master
                    string newname = objtbl.vchGpName.ToString();
                    var newpermission = (from e in hrentity.tblGroupMaster where e.vchGpName == newname && e.intcode==code select e).FirstOrDefault();
                    if (newpermission != null)
                    {
                        ModelState.AddModelError("vchPermission", "Permission name already existing in permission master");
                    }
                    else
                    {
                        objtbl.vchCreatedBy = Session["descript"].ToString();
                        objtbl.dtCreated = DateTime.Now;
                        objtbl.vchHostname = Session["hostname"].ToString();
                        objtbl.vchipused = Session["ipused"].ToString();
                        objtbl.intcode = code;
                        objtbl.intyr = yr;
                        hrentity.tblGroupMaster.Add(objtbl);
                        hrentity.SaveChanges();
                        TempData["Success"] = "New group master saved successfully!";
                        return RedirectToAction("ViewAllGroup");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            return View();
        }

        public ActionResult ViewAllGroup()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectedgp = (from e in hrentity.tblGroupMaster where e.intcode==code select e).ToList();
            return View(selectedgp);
        }

        //Edit Group
        public ActionResult GroupEdit(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectedgp = (from e in hrentity.tblGroupMaster where e.intid == id && e.intcode==code select e).FirstOrDefault();
            return View(selectedgp);
        }

        [HttpPost]
        public ActionResult GroupEdit(tblGroupMaster objgp)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    //check if permission already existing in permission master
                    int id = Convert.ToInt32(objgp.intid);
                    var newpermission = (from e in hrentity.tblGroupMaster where e.intid == id select e).FirstOrDefault();
                    if (newpermission != null)
                    {
                        newpermission.vchGpName = objgp.vchGpName;
                        newpermission.vchUpdatedBy = Session["descript"].ToString();
                        newpermission.dtUpdated = DateTime.Now;
                        newpermission.vchupdatedHostname = Session["hostname"].ToString();
                        newpermission.vchUpdatedipused = Session["ipused"].ToString();
                        hrentity.SaveChanges();
                        TempData["Success"] = "Group master updated successfully!";
                        return RedirectToAction("ViewAllGroup");
                    }
                    TempData["Error"] = "Error generated!";
                    return RedirectToAction("ViewAllGroup");
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            return View();
        }
    }
}