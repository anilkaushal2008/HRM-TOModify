using HRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 

namespace HRM.Controllers
{
    public class PermissionMasterController : Controller
    {

        HRMEntities hrentity = new HRMEntities();
        // GET: PermissionMaster
        public ActionResult CreatePMaster()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var gplist = (from e in hrentity.tblGroupMaster select e.vchGpName).ToList();
            ViewBag.GpName = new SelectList(gplist, "vchGpName");
            return View();
        }

        [HttpPost]
        public ActionResult CreatePMaster(tblPermissionMaster objmas)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int yr = Convert.ToInt32(Session["yr"].ToString());
                    string pname = objmas.vchPermissionName.ToString();
                    var checkexistance = (from e in hrentity.tblPermissionMaster where e.vchPermissionName == pname  select e).FirstOrDefault();
                    if (checkexistance != null)
                    {
                        ModelState.AddModelError("fk_Gpname", "Permission name already existing please check in view for updates");

                    }
                    else
                    {
                        var selectedgpname = (from e in hrentity.tblGroupMaster where e.vchGpName == objmas.fk_GName  select e).FirstOrDefault();
                        objmas.fk_Gpid = selectedgpname.intid;
                        objmas.fk_GName = selectedgpname.vchGpName;
                        objmas.dtCreated = DateTime.Now;
                        objmas.vchCreatedBy = Session["descript"].ToString();
                        objmas.vchHostname = Session["hostname"].ToString();
                        objmas.vchipused = Session["ipused"].ToString();
                        objmas.intcode = code;
                        objmas.intyr = yr;
                        hrentity.tblPermissionMaster.Add(objmas);
                        hrentity.SaveChanges();
                        return RedirectToAction("ViewAllPMaster");
                    }
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            return View();
        }

        public ActionResult ViewAllPMaster()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var PList = (from e in hrentity.tblPermissionMaster select e).ToList();
            return View(PList);
        }

        public ActionResult EditPMaster(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var gplist = (from e in hrentity.tblGroupMaster select e.vchGpName).ToList();
            ViewBag.NewGpName = new SelectList(gplist, "vchGpName");
            var selectedper = (from e in hrentity.tblPermissionMaster where e.intid == id select e).FirstOrDefault();
            return View(selectedper);
        }

        [HttpPost]
        public ActionResult EditPMaster(tblPermissionMaster objmas)
        {
            if (ModelState.IsValid)
            {
                if (Session["descript"] != null)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    var selectedP = (from e in hrentity.tblPermissionMaster where e.intid == objmas.intid select e).FirstOrDefault();
                    var selectedGp = (from e in hrentity.tblGroupMaster where e.vchGpName == objmas.fk_GName  select e).FirstOrDefault();
                    selectedP.fk_Gpid = selectedGp.intid;
                    selectedP.fk_GName = selectedGp.vchGpName;
                    selectedP.vchPermissionName = objmas.vchPermissionName;
                    selectedP.dtUpdated = DateTime.Now;
                    selectedP.vchUpdatedBy = Session["descript"].ToString();
                    selectedP.vchupdatedHostname = Session["hostname"].ToString();
                    selectedP.vchUpdatedipused = Session["ipused"].ToString();
                    hrentity.SaveChanges();
                    return RedirectToAction("ViewAllPMaster");
                }
                else
                {
                    return RedirectToAction("_SessionError1", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "");
            }
            return View();
        }
    }
}