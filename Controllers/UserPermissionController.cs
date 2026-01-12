using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc; 
using HRM.Models;
//using Microsoft.Ajax.Utilities;
using System.Data;

namespace HRM.Controllers
{
    public class UserPermissionController : Controller
    {
        HRMEntities hrentity = new HRMEntities();
        // GET: UserPermission
        public ActionResult AssignNewUserPermission()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            //model object
            UserPermissionViewModel objmodel = new UserPermissionViewModel();
            //select all users from ihms
            //var username = (from e in objtrade.s_users select e.descript).ToList();
            //ViewBag.Users = new SelectList(username, "descript");

            //select all group from gpname
            var allgpname = (from e in hrentity.tblGroupMaster select e).ToList();
            ViewBag.Gpname = new SelectList(allgpname, "vchFpName");

            //Select all department master
            var deptlist = (from e in hrentity.tblDeptMas select e).ToList();
            List<SelectListItem> newlist = new List<SelectListItem>();
            foreach(var dpt in deptlist)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = dpt.vchdeptname,
                    Value = dpt.intid.ToString()
                };
                newlist.Add(selectListItem);
            }
            ViewBag.DeptList = new SelectList(newlist, "Text","Value");

            //select all designations
           // var desilist = (from e in hrentity.tblDesignationMas select e.vchdesignation).ToList();
           // ViewBag.DesiList = new SelectList(desilist, "vchdesignation");

            //all permission 
            var allpermission = (from e in hrentity.tblPermissionMaster select e.vchPermissionName).ToList();
            ViewBag.Allper = new SelectList(allpermission, "vchPermissionName");

            //add all permission an group in model list variable
            List<SelectListItem> GroupName = new List<SelectListItem>();
            List<SelectListItem> PerName = new List<SelectListItem>();
            List<string> user = new List<string>();
            List<bool> selected = new List<bool>();

            //for group name
            foreach (var gp in hrentity.tblPermissionMaster)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = gp.fk_GName.ToString(),
                    Value = gp.fk_Gpid.ToString()
                };
                GroupName.Add(selectListItem);
            }
            //for permission name
            foreach (var per in hrentity.tblPermissionMaster)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = per.vchPermissionName.ToString(),
                    Value = per.intid.ToString(),
                    Selected = Convert.ToBoolean(per.bitIsselected)
                };
                PerName.Add(selectListItem);
            }
            objmodel.AllGpname = GroupName;
            objmodel.AllPermission = PerName;
            objmodel.selectedpermission = selected;
            return View(objmodel);
        }
        //Action for designation selection on department bsis
        [HttpGet]
        public ActionResult DesignationList(int dept_id)
        {
            hrentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblDesignationMas> desilist = new List<tblDesignationMas>();
            desilist = (from e in hrentity.tblDesignationMas where e.intdeptid == dept_id select e).ToList();
            var result = (from d in desilist
                          select new
                          {
                              id = d.intid,
                              designation = d.vchdesignation
                          }).ToList();
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveList(string email, string pertable,string Dept,string Deptid,string Desi,string Desiid,string mobi, string ntable,string selectedusr,string sfrom, string sto,string authyes, string ass,string pwd,string act,string ISHOD)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int year = Convert.ToInt32(Session["yr"].ToString());
                string selecteduser = selectedusr.ToString();
                if (authyes != "false")
                {
                    if (sfrom != "" && sto != "")
                    {
                        int salifrom = Convert.ToInt32(sfrom);
                        int salito = Convert.ToInt32(sto);
                        if (salito <= salifrom)
                        {
                            var output3 = "SalaryTo, should not be lesser from SalaryFrom and should not be equal both!";
                            return Json(new { success = 3, output3, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            //check existing user
                            //var usr = (from e in hrentity.tblUserAuthorize where e.vchUserName == selecteduser select e).FirstOrDefault();
                            //check salary range
                            var checkrange = (from e in hrentity.tblUserAuthorize where e.intcode == code orderby e.intSalaryTo descending select e.intSalaryTo).FirstOrDefault();
                            if (salifrom <= checkrange)
                            {
                                var output4 = "Entered range already assigned, salary from should be greater than "+checkrange;
                                return Json(new { success = 4, output4, JsonRequestBehavior.AllowGet });
                            }
                            tblUserAuthorize obj = new tblUserAuthorize();
                            obj.vchUserName = selecteduser;
                            obj.bitAuthorization = true;
                            obj.intSalaryfrom = salifrom;
                            obj.intSalaryTo = salito;
                            obj.dtcreated = DateTime.Now;
                            obj.vchcreatedby = Session["descript"].ToString();
                            obj.vchipused = Session["ipused"].ToString();
                            obj.vchhostname = Session["hostname"].ToString();
                            obj.intcode = code;
                            obj.intyr = year;
                            hrentity.tblUserAuthorize.Add(obj);
                            hrentity.SaveChanges();
                        }
                    }
                    else
                    {
                        var output6 = "SalaryFrom and SalaryTo should not be null!";
                        return Json(new { success = 6, output6, JsonRequestBehavior.AllowGet });
                    }
                }
                if (selectedusr != "")
                {
                    //master table check and user entry              
                    var checkuser = (from e in hrentity.tblUserMaster where e.vchUsername == selecteduser where e.intcode==code select e).FirstOrDefault();
                    if (checkuser != null)
                    {

                        var output2 = "User already existing in permission master!";
                        return Json(new { success = 2, output2, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        tblUserMaster objmaster = new tblUserMaster();
                        objmaster.vchUsername = selecteduser;
                        objmaster.Passcode = pwd;
                        objmaster.vchEmail = email;
                        objmaster.vchDepartment = Dept.ToString();
                        objmaster.fk_intDeptid = Convert.ToInt32(Deptid);
                        objmaster.vchDesignation = Desi.ToString();
                        objmaster.fk_intDesignationid = Convert.ToInt32(Desiid);
                        objmaster.vchMobile = mobi.ToString();
                        if(ass=="true")
                        {
                            objmaster.bitAllowAssesment = true;
                        }
                        else
                        {
                            objmaster.bitAllowAssesment = false;
                        }
                        if(act=="true")
                        {
                            objmaster.bitActive = true;
                        }
                        else
                        {
                            objmaster.bitActive = false;
                        }
                        if (ISHOD == "true")
                        {
                            objmaster.bitISHOD = true;
                        }
                        else
                        {
                            objmaster.bitISHOD = false;
                        }
                        objmaster.dtcreated = DateTime.Now;
                        objmaster.vchcreatedby = Session["descript"].ToString();
                        objmaster.vchipused = Session["ipused"].ToString();
                        objmaster.vchhostname = Session["hostname"].ToString();
                        objmaster.intcode = code;
                        objmaster.intyr = year;
                        hrentity.tblUserMaster.Add(objmaster);
                        hrentity.SaveChanges();
                    }
                }
                if (pertable != "")
                {
                    string[] selectedpermission = pertable.Split(',');
                    foreach (var per in selectedpermission)
                    {
                        int perid = Convert.ToInt32(per);
                        // select permission
                        var selecetdper = (from e in hrentity.tblPermissionMaster where e.intid == perid select e).FirstOrDefault();
                        tblUserPermission objuser = new tblUserPermission();
                        objuser.vchUserName = selecteduser;
                        objuser.fk_gpid = selecetdper.fk_Gpid;
                        objuser.fk_group = selecetdper.fk_GName;
                        objuser.fk_Permissionid = perid;
                        objuser.fk_permissionname = selecetdper.vchPermissionName;
                        objuser.vchCreatedBy = Session["descript"].ToString();
                        objuser.dtCreated = DateTime.Now;
                        objuser.vchipused = Session["ipused"].ToString();
                        objuser.vchHostname = Session["hostname"].ToString();
                        objuser.intcode = code;
                        objuser.intyr = year;
                        objuser.bitAllowed = true;
                        objuser.bitIsSelected = true;                       
                        hrentity.tblUserPermission.Add(objuser);
                        hrentity.SaveChanges();
                    }
                }
                if (ntable != "")
                {
                    string[] unselectedper = ntable.Split(',');
                    foreach (var per in unselectedper)
                    {
                        int perid = Convert.ToInt32(per);
                        // select permission
                        var unselected = (from e in hrentity.tblPermissionMaster where e.intid == perid select e).FirstOrDefault();
                        tblUserPermission objuser = new tblUserPermission();
                        objuser.vchUserName = selecteduser;
                        objuser.fk_gpid = unselected.fk_Gpid;
                        objuser.fk_group = unselected.fk_GName;
                        objuser.fk_Permissionid = perid;
                        objuser.fk_permissionname = unselected.vchPermissionName;
                        objuser.dtCreated = DateTime.Now;
                        objuser.vchCreatedBy = Session["descript"].ToString();
                        objuser.vchipused = Session["ipused"].ToString();
                        objuser.vchHostname = Session["hostname"].ToString();
                        objuser.intcode = code;
                        objuser.intyr = year;
                        objuser.bitAllowed = false;
                        objuser.bitIsSelected = false;
                        hrentity.tblUserPermission.Add(objuser);
                        hrentity.SaveChanges();
                    }
                }
                var output1 = "User permission added successfully!";
                return Json(new { success = 1, output1, JsonRequestBehavior.AllowGet });
            }
            else
            {
                var output5 = "Your session has expired!";
                return Json(new { success = 5, output5, JsonRequestBehavior.AllowGet });
            }

        }

        //all user permission view
        public ActionResult ViewAllUserPermissionEmp()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var result = (from e in hrentity.tblUserMaster where e.intcode==code select e).ToList();
            return View(result);
        }

        public ActionResult EditUserPermission(int id)
        {
            //selected user and correspond details
            int code = Convert.ToInt32(Session["id"].ToString());
            var usrname = (from e in hrentity.tblUserMaster where e.intid == id && e.intcode==code select e.vchUsername).FirstOrDefault();
            var username = (from e in hrentity.tblUserMaster where e.intid == id && e.intcode==code select e).FirstOrDefault();
            ViewBag.UserName = username.vchUsername.ToString();
            ViewBag.Dept = username.vchDepartment;
            ViewBag.Desi = username.vchDesignation;
            ViewBag.Mobile = username.vchMobile;
            ViewBag.StatusAss = username.bitAllowAssesment.ToString();
            ViewBag.Active = username.bitActive.ToString();
            ViewBag.ISHOD = username.bitISHOD.ToString();
            ViewBag.Pwd = username.Passcode.ToString();
            ViewBag.Email = username.vchEmail.ToString();
            //selected all permission
            List<SelectListItem> mylist = new List<SelectListItem>();
            //var selectedpermission = (from e in hrentity.tblUserPermission where e.vchUserName == usrname && e.intcode==code select e).ToList();          
            var selectedpermission = (from e in hrentity.spGetUserPermission(usrname, code) select e).ToList();
            foreach (var per in selectedpermission)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = per.vchPermissionName.ToString(),
                    Value = per.intid.ToString(),
                    Selected = Convert.ToBoolean(per.bitAllowed)
                };
                mylist.Add(selectListItem);
            }
            ViewBag.AllPer = (mylist);
            var authorization = (from e in hrentity.tblUserAuthorize where e.vchUserName == usrname && e.intcode==code select e).FirstOrDefault();
            if(authorization!=null)
            {
                return View(authorization);
            }
            return View();
        }

        //update user permission
        [HttpPost]
        public JsonResult UpdateList(string newmail, string newper, string mobile, string notselect, string selectedusr, string sfrom, string sto, string authyes, string Ass, string newactive, string newpwd, string newHOD)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int yr = Convert.ToInt32(Session["yr"].ToString());
                string selecteduser = selectedusr.ToString();
                if (authyes != "false" || authyes == "" || authyes == "false")
                {
                    if (sfrom != "" && sto != "")
                    {
                        int salifrom = Convert.ToInt32(sfrom);
                        int salito = Convert.ToInt32(sto);
                        if (salito <= salifrom)
                        {
                            var output1 = "SalaryTo should not be lesser from salaryFrom and should not be equal!";
                            return Json(new { success = 1, output1, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            //check existing user
                            var usr = (from e in hrentity.tblUserAuthorize where e.vchUserName == selecteduser && e.intcode == code select e).FirstOrDefault();
                            if (usr != null)
                            {
                                //check salary range
                                var checkrange = (from e in hrentity.tblUserAuthorize where e.intcode == code orderby e.intSalaryTo descending select e.intSalaryTo).FirstOrDefault();
                                if (salifrom <= checkrange)
                                {
                                    var output7 = "Entered range already assigned, salary from should be greater" + checkrange;
                                    return Json(new { success = 7, output7, JsonRequestBehavior.AllowGet });
                                }
                                else
                                {
                                    usr.vchUserName = selecteduser;
                                    usr.bitAuthorization = true;
                                    usr.intSalaryfrom = salifrom;
                                    usr.intSalaryTo = salito;
                                    usr.dtupdated = DateTime.Now;
                                    usr.vchupdatedby = Session["descript"].ToString();
                                    usr.vchupdatedipdused = Session["ipused"].ToString();
                                    usr.vchupdatedhostname = Session["hostname"].ToString();
                                    hrentity.SaveChanges();
                                }
                            }
                            else
                            {
                                //check salary range
                                var checkrange = (from e in hrentity.tblUserAuthorize where e.intcode == code orderby e.intSalaryTo descending select e.intSalaryTo).FirstOrDefault();
                                if (salifrom <= checkrange)
                                {
                                    var output6 = "Entered range already assigned, salary from should be greater than " + checkrange;
                                    return Json(new { success = 6, output6, JsonRequestBehavior.AllowGet });
                                }
                                else
                                {
                                    tblUserAuthorize obj = new tblUserAuthorize();
                                    obj.vchUserName = selecteduser;
                                    obj.bitAuthorization = true;
                                    obj.intSalaryfrom = salifrom;
                                    obj.intSalaryTo = salito;
                                    obj.dtcreated = DateTime.Now;
                                    obj.vchcreatedby = Session["descript"].ToString();
                                    obj.vchipused = Session["ipused"].ToString();
                                    obj.vchhostname = Session["hostname"].ToString();
                                    obj.intcode = code;
                                    obj.intyr = yr;
                                    hrentity.tblUserAuthorize.Add(obj);
                                    hrentity.SaveChanges();
                                }
                            }
                        }
                    }
                    else if (sfrom == "" && sto == "")
                    {
                        var selctedauthorization = (from e in hrentity.tblUserAuthorize where e.vchUserName == selecteduser && e.intcode == code select e).FirstOrDefault();
                        if (selctedauthorization != null)
                        {
                            hrentity.tblUserAuthorize.Remove(selctedauthorization);
                            hrentity.SaveChanges();
                        }
                    }
                    else
                    {

                        var output4 = "SalaryFrom and SalaryTo should not be null!";
                        return Json(new { success = 4, output4, JsonRequestBehavior.AllowGet });
                    }
                }
                if (selectedusr != "")
                {
                    //master update             
                    var checkuser = (from e in hrentity.tblUserMaster where e.vchUsername == selecteduser && e.intcode == code select e).FirstOrDefault();
                    if (checkuser == null)
                    {

                        var output2 = "User does not existing in permission master";
                        return Json(new { success = 2, output2, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        tblUserMaster objmaster = new tblUserMaster();
                        //checkuser.vchUsername = selecteduser;
                        //checkuser.vchDepartment = Dept;
                        //checkuser.vchDesignation = Desi;
                        //checkuser.fk_intDeptid = Convert.ToInt32(Deptid);
                        //checkuser.fk_intDesignationid = Convert.ToInt32(Desiid);
                        if (Ass == "true")
                        {
                            checkuser.bitAllowAssesment = true;
                        }
                        else if (Ass == "false")
                        {
                            checkuser.bitAllowAssesment = false;
                        }
                        if (newactive == "true")
                        {
                            checkuser.bitActive = true;
                        }
                        else
                        {
                            checkuser.bitActive = false;
                        }
                        if (newHOD == "true")
                        {
                            checkuser.bitISHOD = true;
                        }
                        else
                        {
                            checkuser.bitISHOD = false;
                        }
                        if (newpwd != null)
                        {
                            checkuser.Passcode = newpwd;
                        }
                        checkuser.vchMobile = mobile;
                        checkuser.vchEmail = newmail;
                        checkuser.dtupdated = DateTime.Now;
                        checkuser.vchupdatedby = Session["descript"].ToString();
                        checkuser.vchupdatedipused = Session["ipused"].ToString();
                        checkuser.vchupdatedhostname = Session["hostname"].ToString();
                        hrentity.SaveChanges();
                    }
                }
                if (newper != "")
                {
                    string[] selectedpermission = newper.Split(',');
                    foreach (var per in selectedpermission)
                    {
                        int perid = Convert.ToInt32(per);
                        // select user permission
                        var selectedusrper = (from e in hrentity.tblUserPermission where e.fk_Permissionid == perid && e.vchUserName == selecteduser && e.intcode == code select e).FirstOrDefault();
                        if (selectedusrper != null)
                        {
                            //select permission
                            int masterid = Convert.ToInt32(selectedusrper.fk_Permissionid);
                            var selectpermaster = (from e in hrentity.tblPermissionMaster where e.intid == masterid select e).FirstOrDefault();
                            selectedusrper.vchUserName = selecteduser;
                            selectedusrper.fk_gpid = selectpermaster.fk_Gpid;
                            selectedusrper.fk_group = selectpermaster.fk_GName;
                            selectedusrper.fk_Permissionid = selectpermaster.intid;
                            selectedusrper.fk_permissionname = selectpermaster.vchPermissionName;
                            selectedusrper.vchUpdatedBy = Session["descript"].ToString();
                            selectedusrper.dtUpdated = DateTime.Now;
                            selectedusrper.vchUpdatedipused = Session["ipused"].ToString();
                            selectedusrper.vchupdatedHostname = Session["hostname"].ToString();
                            selectedusrper.bitAllowed = true;
                            selectedusrper.bitIsSelected = true;
                            hrentity.SaveChanges();
                        }
                        else
                        {
                            //get master permission
                            int year = Convert.ToInt32(Session["yr"].ToString());
                            var masPermission = (from e in hrentity.tblPermissionMaster where e.intid == perid select e).FirstOrDefault();
                            tblUserPermission objuser = new tblUserPermission();
                            objuser.vchUserName = selecteduser;
                            objuser.fk_gpid = masPermission.fk_Gpid;
                            objuser.fk_group = masPermission.fk_GName;
                            objuser.fk_Permissionid = perid;
                            objuser.fk_permissionname = masPermission.vchPermissionName;
                            objuser.vchCreatedBy = Session["descript"].ToString();
                            objuser.dtCreated = DateTime.Now;
                            objuser.vchipused = Session["ipused"].ToString();
                            objuser.vchHostname = Session["hostname"].ToString();
                            objuser.intcode = code;
                            objuser.intyr = year;
                            objuser.bitAllowed = true;
                            objuser.bitIsSelected = true;
                            hrentity.tblUserPermission.Add(objuser);
                            hrentity.SaveChanges();
                        }
                    }
                }
                if (notselect != "")
                {
                    string[] unselectedper = notselect.Split(',');
                    foreach (var per in unselectedper)
                    {
                        int perid = Convert.ToInt32(per);
                        // select user permission
                        var selectedusrper = (from e in hrentity.tblUserPermission where e.fk_Permissionid == perid && e.intcode == code && e.vchUserName == selecteduser select e).FirstOrDefault();
                        if (selectedusrper != null)
                        {
                            // select permission master
                            int masid = Convert.ToInt32(selectedusrper.fk_Permissionid);
                            var unselected = (from e in hrentity.tblPermissionMaster where e.intid == masid select e).FirstOrDefault();
                            selectedusrper.dtUpdated = DateTime.Now;
                            selectedusrper.vchUpdatedBy = Session["descript"].ToString();
                            selectedusrper.vchUpdatedipused = Session["ipused"].ToString();
                            selectedusrper.vchupdatedHostname = Session["hostname"].ToString();
                            selectedusrper.bitAllowed = false;
                            selectedusrper.bitIsSelected = false;
                            hrentity.SaveChanges();
                        }
                        else
                        {
                            //get master permission
                            int year = Convert.ToInt32(Session["yr"].ToString());
                            var masPermission = (from e in hrentity.tblPermissionMaster where e.intid == perid select e).FirstOrDefault();
                            tblUserPermission objuser = new tblUserPermission();
                            objuser.vchUserName = selecteduser;
                            objuser.fk_gpid = masPermission.fk_Gpid;
                            objuser.fk_group = masPermission.fk_GName;
                            objuser.fk_Permissionid = perid;
                            objuser.fk_permissionname = masPermission.vchPermissionName;
                            objuser.vchCreatedBy = Session["descript"].ToString();
                            objuser.dtCreated = DateTime.Now;
                            objuser.vchipused = Session["ipused"].ToString();
                            objuser.vchHostname = Session["hostname"].ToString();
                            objuser.intcode = code;
                            if (year != 0)
                            {
                                objuser.intyr = year;
                            }
                            objuser.bitAllowed = false;
                            objuser.bitIsSelected = false;
                            hrentity.tblUserPermission.Add(objuser);
                            hrentity.SaveChanges();
                        }
                    }
                    var output3 = "User permission updated successfully!";
                    return Json(new { success = 3, output3, JsonRequestBehavior.AllowGet });
                }
                else
                {
                    var output5 = "Your session has expired!";
                    return Json(new { success = 5, output5, JsonRequestBehavior.AllowGet });
                }
            }
            else
            {
                var output6 = "Your session has expired!";
                return Json(new { success = output6, JsonRequestBehavior.AllowGet });
            }
        }
        
        public ActionResult DeleteUserPermission(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var user = (from e in hrentity.tblUserMaster where e.intid == id && e.intcode==code select e).FirstOrDefault();
            string username = user.vchUsername.ToString();
            var userauthorize = (from e in hrentity.tblUserAuthorize where e.vchUserName == username && e.intcode==code select e).FirstOrDefault();
            if(userauthorize!=null)
            {
                hrentity.tblUserAuthorize.Remove(userauthorize);
                hrentity.SaveChanges();
            }
            var userpermission = (from e in hrentity.tblUserPermission where e.vchUserName == username && e.intcode==code select e).ToList();
            foreach(var per in userpermission)
            {
                hrentity.tblUserPermission.Remove(per);
                hrentity.SaveChanges();
            }
            if (user != null)
            {
                hrentity.tblUserMaster.Remove(user);
                hrentity.SaveChanges();            
            }
            TempData["Success"] = "User removed successfully!";
            return RedirectToAction("ViewAllUserPermissionEmp");
        }

        //GENERATED TWO TIME CODE
        public ActionResult EmpUserView()
        {
            if (base.Session["descript"] == null)
            {
                return base.RedirectToAction("_SessionError1", "Home");
            }
            int code = Convert.ToInt32(base.Session["id"].ToString());
            List<tblEmpLoginUser> getAllList = (from e in this.hrentity.tblEmpLoginUser
                                                join m in this.hrentity.tblEmpAssesmentMas on e.fk_intEmpID equals m.intid
                                                where (m.bittempstatusactive == true || m.bitIsPartialAuthorised == true) && m.bitstatusdeactive != true && m.bitIsLeft != true && m.intcode == (int?)code
                                                select e).ToList<tblEmpLoginUser>();
            if (getAllList.Count<tblEmpLoginUser>() != 0)
            {
                return base.View(getAllList);
            }
            base.TempData["Empty"] = "0 user found in database!";
            return base.View();
        }

        public ActionResult UpdateMobile(int id)
        {
            int code = Convert.ToInt32(base.Session["id"].ToString());
            var GetEmpData = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
            tblEmpLoginUser objuser = new tblEmpLoginUser();
            tblEmpLoginUser getUser = (from e in this.hrentity.tblEmpLoginUser
                                       where e.fk_intEmpID == id
                                       select e).FirstOrDefault<tblEmpLoginUser>();
            if (getUser == null)
            {
                base.TempData["Error"] = "User detail not found contact to admin!";
                return base.RedirectToAction("EmpUserView");
            }
            List<tblEmpLoginUser> checkMobile = (from e in this.hrentity.tblEmpLoginUser
                                                 where e.vchmobile == getUser.vchmobile && e.intcode == code
                                                 select e).ToList<tblEmpLoginUser>();
            if (checkMobile.Count<tblEmpLoginUser>() > 1)
            {
                base.TempData["Error"] = "Selected mobile number used in multiple employee user so contact to admin!";
                if (GetEmpData.bitIsConsultant == true)
                {
                    return base.RedirectToAction("ConsultantUserView");
                }
                else
                {
                    return base.RedirectToAction("EmployeeUserView");
                }
            }
            return this.PartialView("_PartialUpMObile", getUser);
        }

        [HttpPost]
        public ActionResult UpdateMobile(tblEmpLoginUser user)
        {

            tblEmpLoginUser objtbl = new tblEmpLoginUser();
            if (base.Session["descript"] == null)
            {
                return base.RedirectToAction("_SessionError1", "Home");
            }
            int code = Convert.ToInt32(base.Session["id"].ToString());
            objtbl = (from e in this.hrentity.tblEmpLoginUser
                      where e.vchmobile == user.vchmobile && e.intcode == code
                      select e).FirstOrDefault<tblEmpLoginUser>();
            string checkmobile = user.vchmobile.ToString();
            if (checkmobile == null)
            {
                base.TempData["Error"] = "Entered mobile number already used in database!";
                return base.RedirectToAction("EmpUserView");
            }
            tblEmpLoginUser getSelected = (from e in this.hrentity.tblEmpLoginUser
                                           where e.fk_intEmpID == user.fk_intEmpID && e.intcode == code

                                           select e).FirstOrDefault<tblEmpLoginUser>();
            getSelected.vchmobile = user.vchmobile;
            getSelected.dtUpdated = new DateTime?(DateTime.Now);
            getSelected.vchUpdatedBy = base.Session["descript"].ToString();
            this.hrentity.SaveChanges();
            base.TempData["Success"] = "Mobile updated successfully!";
            var getEMptype = (from e in hrentity.tblEmpAssesmentMas where e.intid == user.fk_intEmpID select e).FirstOrDefault();
            if (getEMptype.bitIsConsultant == true)
            {
                return base.RedirectToAction("ConsultantUserView", "UserPermission");
            }
            else
            {
                return base.RedirectToAction("EmpUserView", "UserPermission");
            }
        }

        public ActionResult _PartialUpMobile()
        {
            if (base.Session["UserId"] != null)
            {
                return base.View();
            }
            return base.RedirectToAction("_SessionError1", "Home");
        }

        public ActionResult ActivateHOD(int id)
        {
            tblEmpLoginUser user = new tblEmpLoginUser();
            int code = Convert.ToInt32(base.Session["id"].ToString());
            user = (from e in this.hrentity.tblEmpLoginUser
                    where e.fk_intEmpID == id && e.intcode == code
                    select e).FirstOrDefault<tblEmpLoginUser>();
            if (user != null)
            {
                user.bitISAllowedFirstDayLeave = true;
                user.dtUpdated = new DateTime?(DateTime.Now);
                user.vchUpdatedBy = base.Session["descript"].ToString();
                this.hrentity.SaveChanges();
                base.TempData["Success"] = "User converted as HOD successfully!";
                return base.RedirectToAction("EmpUserView");
            }
            base.TempData["Error"] = "User not found please check it and try again!";
            return base.RedirectToAction("EmpUserView");
        }

        public ActionResult DeActiveHOD(int id)
        {
            tblEmpLoginUser user = new tblEmpLoginUser();
            int code = Convert.ToInt32(base.Session["id"].ToString());
            user = (from e in this.hrentity.tblEmpLoginUser
                    where e.fk_intEmpID == id && e.intcode == code
                    select e).FirstOrDefault<tblEmpLoginUser>();
            if (user != null)
            {
                user.bitISAllowedFirstDayLeave = false;
                user.dtUpdated = new DateTime?(DateTime.Now);
                user.vchUpdatedBy = base.Session["descript"].ToString();
                this.hrentity.SaveChanges();
                base.TempData["Success"] = "User converted as HOD to normal employee successfully!";
                return base.RedirectToAction("EmpUserView");
            }
            base.TempData["Error"] = "User not found please check it and try again!";
            return base.RedirectToAction("EmpUserView");
        }

        public ActionResult ConsultantUserView()
        {
            if (base.Session["descript"] == null)
            {
                return base.RedirectToAction("_SessionError1", "Home");
            }
            int code = Convert.ToInt32(base.Session["id"].ToString());
            List<tblEmpLoginUser> getAllList = (from e in this.hrentity.tblEmpLoginUser
                                                join m in this.hrentity.tblEmpAssesmentMas on e.fk_intEmpID equals m.intid
                                                join d in this.hrentity.tblDoctorUploadDetail on m.intid equals d.fk_ConsultMasId
                                                where m.bitIsConsultant==true && d.bitIsActive==true && m.intcode==code
                                                select e).ToList();
            if (getAllList.Count<tblEmpLoginUser>() != 0)
            {
                return base.View(getAllList);
            }
            base.TempData["Empty"] = "0 user found in database!";
            return base.View();
        }
    }
}
