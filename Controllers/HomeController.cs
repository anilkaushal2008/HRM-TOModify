using HRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc; 
using System.Web.UI.WebControls;
//using Microsoft.AspNetCore.Http;
using System.Net;
using System.Management;
//using Microsoft.AspNetCore.Http.Features;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.IO;
//using Microsoft.AspNet.HttpFeature;

namespace HRM.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        //check login credentials
        [HttpPost]
        public ActionResult Login(tblUserMaster NewUser, IndusCompanies SelectedCompany)
        {
            if (ModelState.IsValid)
            {

                //using (TradeEntities objentity = new TradeEntities())
                using (IndusGroupEntities objgroup = new IndusGroupEntities())

                using (HRMEntities hrentity = new HRMEntities())
                {
                    int tempcode = SelectedCompany.intPK;                  
                    string pass = NewUser.Passcode.ToString();
                    string username = NewUser.vchUsername.ToString();
                    //check user name
                    var checkusername = (from e in hrentity.tblUserMaster where e.vchUsername == username && e.intcode == tempcode select e).FirstOrDefault();
                    if (checkusername != null)
                    {
                        //check passcode with uname and other credential.
                        var selectedusr = (from e in hrentity.tblUserMaster where e.vchUsername == username && e.Passcode == pass && e.intcode == tempcode select e).FirstOrDefault();
                        if (selectedusr != null)
                        {
                            if (selectedusr.bitActive != true)
                            {
                                ModelState.AddModelError("vchUsername", "InActive user contact to application Administrator!");
                                return View("Index");
                            }
                            //get selected Branch HR ID
                            var getHrID = (from e in hrentity.tblUserMaster where e.fk_intDesignationid == 12 && e.intcode == tempcode && e.bitActive == true select e).FirstOrDefault();
                            if (getHrID != null)
                            {
                                Session["IS_BHR"] = getHrID.intid.ToString();
                            }                            
                            //check user allowed in hr module or not
                            var chekusr = (from e in hrentity.tblUserPermission where e.vchUserName == username && e.intcode == tempcode select e).FirstOrDefault();
                            if (chekusr != null)
                            {                                
                                var name = selectedusr.vchUsername.ToString();                     
                                var groupid = objgroup.IndusCompanies.Where(g => g.intPK.Equals(SelectedCompany.intPK)).FirstOrDefault();
                                if (groupid != null)
                                {
                                    if (checkusername.vchProfileName != null)
                                    {
                                        Session["ProfilePic"] = checkusername.vchProfileName.ToString();
                                    }
                                    else
                                    {
                                        Session["ProfilePic"] = "N/A";
                                    }
                                    if (checkusername.bitISHOD == true)
                                    {
                                        Session["DeptHOD"] = "True";
                                        Session["HOD_ID"] = checkusername.intid.ToString();
                                    }
                                    else
                                    {
                                        Session["DeptHOD"] = "False";
                                    }
                                    Session["descript"] = selectedusr.vchUsername.ToString();
                                    Session["UserEmail"] = selectedusr.vchEmail.ToString();
                                    Session["Compname"] = groupid.descript.ToString();
                                    if (Session["descript"] == null)
                                    {
                                        return RedirectToAction("SessionError");
                                    }
                                    Session["id"] = groupid.intPK.ToString();
                                    Session["yr"] = groupid.ses_code.ToString();
                                    Session["usrid"] = selectedusr.intid.ToString();
                                    Session["usrdesi"] = selectedusr.fk_intDesignationid.ToString();
                                    Session["userdept"] = selectedusr.fk_intDeptid.ToString();
                                    //Get love selection from user table
                                    int uid = selectedusr.intid;
                                    var love = (from e in hrentity.tblUserLove where e.fk_intUserId == uid && e.intcode == tempcode select e).FirstOrDefault();
                                    if (love != null)
                                    {
                                        if (love.bitNABHAccount == true)
                                        {
                                            if(love.dtFromSession!=null)
                                            {
                                                DateTime SessionDate = Convert.ToDateTime(love.dtFromSession.ToString());
                                                Session["SessionDate"] = SessionDate.ToString();
                                            }
                                            else
                                            {
                                                Session["SessionDate"] = null;
                                            }
                                            
                                        }
                                        else
                                        {
                                            Session["SessionDate"] = null;
                                        }
                                        Session["HrAdmin"] = love.BitHrAdmin.ToString();
                                        Session["MainAdmin"] = love.BitMainAdmin.ToString();
                                        Session["ChangeBranch"] = love.BitOtherOption2.ToString();
                                        Session["ISVPHR"] = love.IsVPHR.ToString();
                                        
                                    }
                                    else
                                    {
                                        Session["HrAdmin"] = "False";
                                        Session["MainAdmin"] = "False";
                                        Session["ChangeBranch"] = "False";
                                        Session["ISVPHR"] = "False";
                                    }
                                }                                
                                //get host name
                                string strHostName = System.Net.Dns.GetHostName();
                                //new code for client ip details                                
                                string checkip = string.Empty;
                                string hostname = string.Empty;
                                IPHostEntry Host1 = default(IPHostEntry);                               
                                hostname = System.Environment.MachineName;
                                Host1 = Dns.GetHostEntry(hostname);
                                foreach (IPAddress IP in Host1.AddressList)
                                {
                                    if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                    {
                                        checkip = Convert.ToString(IP);
                                    }
                                }
                                Session["hostname"] = hostname.ToString();
                                Session["ipused"] = checkip.ToString();
                                //Old code for client details
                                System.Net.IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
                                //Session["hostname"] = strHostName.ToString();
                                //get ip address
                                System.Net.IPAddress[] addr = ipEntry.AddressList;
                                string ip = addr[1].ToString();
                                //Session["ipused"] = ip.ToString();
                                //user authorization range
                                var salrange = (from e in hrentity.tblUserAuthorize where e.vchUserName == name && e.intcode == tempcode select e).FirstOrDefault();
                                if (salrange != null)
                                {
                                    Session["Sfrom"] = salrange.intSalaryfrom.ToString();
                                    Session["Sto"] = salrange.intSalaryTo.ToString();
                                    Session["Authoriser"] = salrange.bitAuthorization.ToString();
                                }
                                //set branch code a session code
                                if (groupid.intPK == 2)
                                {
                                    Session["branchCode"] = "HY";
                                }
                                if (groupid.intPK == 3)
                                {
                                    Session["branchCode"] = "IS";
                                }
                                if (groupid.intPK == 4)
                                {
                                    Session["branchCode"] = "IH";
                                }
                                if (groupid.intPK == 14)
                                {
                                    Session["branchCode"] = "II";
                                }
                                if (groupid.intPK == 15)
                                {
                                    Session["branchCode"] = "IF";
                                }
                                if (groupid.intPK == 16)
                                {
                                    Session["branchCode"] = "KH";
                                }
                                if (groupid.intPK == 21)
                                {
                                    Session["branchCode"] = "MH";
                                }
                                if (groupid.intPK == 22)
                                {
                                    Session["branchCode"] = "HS";
                                }
                                if (groupid.intPK == 23)
                                {
                                    Session["branchCode"] = "MY";
                                }
                                if (groupid.intPK == 24)
                                {
                                    Session["branchCode"] = "KS";
                                }
                                if (groupid.intPK == 25)
                                {
                                    Session["branchCode"] = "BH";
                                }
                                if (groupid.intPK == 26)
                                {
                                    Session["branchCode"] = "SK";
                                }
                                //user permission
                                var permissions = (from e in hrentity.tblUserPermission where e.vchUserName == name && e.intcode == tempcode select e).ToList();
                                if (permissions != null)
                                {
                                    foreach (var per in permissions)
                                    {
                                        if (per.fk_permissionname == "Add/Edit")
                                        {
                                            Session["AddEdit"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View Newly Employee")
                                        {
                                            Session["ViewNewlyEmp"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Assign authorization")
                                        {
                                            Session["AssignAuthor"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View Authorized Employee")
                                        {
                                            Session["AuthorizedView"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Authorization Final Update")
                                        {
                                            Session["FinalUpdate"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Pending Authorization")
                                        {
                                            Session["PendingAuthor"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Allow For Authorization")
                                        {
                                            Session["AllowAuthorization"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View All Active Employee")
                                        {
                                            Session["ViewActiveEmp"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Deactivate Employee")
                                        {
                                            Session["DeactiveEmp"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Allow For Deactivation")
                                        {
                                            Session["DeactiveAllow"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View Left Employee")
                                        {
                                            Session["ViewLeftEmp"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Report1")
                                        {
                                            Session["Report1"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Report2")
                                        {
                                            Session["Report2"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "R1")
                                        {
                                            Session["R1"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "R2")
                                        {
                                            Session["R2"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "R3")
                                        {
                                            Session["R3"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View Left Employee")
                                        {
                                            Session["ViewLeftEmp"] = per.bitAllowed;
                                        }
                                        //All Master
                                        if (per.fk_permissionname == "Name Title Master")
                                        {
                                            Session["NameTitle"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View All Title Master")
                                        {
                                            Session["ViewTitle"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Edit Title Master")
                                        {
                                            Session["EditTitle"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Department Master")
                                        {
                                            Session["DepartmentMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View All Department Master")
                                        {
                                            Session["ViewDepartmentMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Edit Department Master")
                                        {
                                            Session["EditDepartmentMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Designation Master")
                                        {
                                            Session["DesignationMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View All Designation Master")
                                        {
                                            Session["ViewDesignationMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Edit Designation Master")
                                        {
                                            Session["EditDesignationMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Qualification/Doc Master")
                                        {
                                            Session["QualiDocMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View All Qualification/Doc Master")
                                        {
                                            Session["ViewQualiDocMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Edit Qualification/Doc Master")
                                        {
                                            Session["EditQualiDocMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Create Group Master")
                                        {
                                            Session["GpMaster"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View All Group Master")
                                        {
                                            Session["ViewGpMaster"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Edit Group Master")
                                        {
                                            Session["EditGpMaster"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Create Permission Master")
                                        {
                                            Session["CreatePerMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View All Permission Master")
                                        {
                                            Session["ViewPerMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Edit Permission Master")
                                        {
                                            Session["EditPerMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Assign User Permission")
                                        {
                                            Session["AssignUserPerMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View All User Permission")
                                        {
                                            Session["ViewUserPerMas"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Edit User Permission")
                                        {
                                            Session["EditUserPerMas"] = per.bitAllowed;
                                        }
                                        #region All Assessment Permission
                                        if (per.fk_permissionname == "New")
                                        {
                                            Session["AssNew"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View all")
                                        {
                                            Session["AssView"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Allow Send")
                                        {
                                            Session["AssSend"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Status view")
                                        {
                                            Session["AssStatus"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Status Details ")
                                        {
                                            Session["AssStatusDetail"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Assigned View")
                                        {
                                            Session["AssAssignedView"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View Completed")
                                        {
                                            Session["AssCompletedView"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Allow for upload doc")
                                        {
                                            Session["AllowDocUpload"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Pending upload view")
                                        {
                                            Session["PendingDocView"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Allow to upload pending")
                                        {
                                            Session["AllowDocPenUpload"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Allow to fill official details")
                                        {
                                            Session["AllowFillOfficial"] = per.bitAllowed;
                                        }
                                        #endregion

                                        #region Position Category Master Permission

                                        if (per.fk_permissionname == "Position New")
                                        {
                                            Session["PosNew"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Position Edit")
                                        {
                                            Session["PosEdit"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Position Delete")
                                        {
                                            Session["PosDelete"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Position View")
                                        {
                                            Session["PosView"] = per.bitAllowed;
                                        }
                                        #endregion

                                        #region Document Master Permission
                                        if (per.fk_permissionname == "New Doc")
                                        {
                                            Session["Docnew"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Edit Doc")
                                        {
                                            Session["Docedit"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "View Doc")
                                        {
                                            Session["Docview"] = per.bitAllowed;
                                        }
                                        #endregion

                                        #region Position Designation Mapping Permission
                                        if (per.fk_permissionname == "PD New")
                                        {
                                            Session["Pdnew"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "PD Edit")
                                        {
                                            Session["Pdedit"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "PD Delete")
                                        {
                                            Session["Pddelete"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "PD View")
                                        {
                                            Session["Pdview"] = per.bitAllowed;
                                        }
                                        #endregion

                                        #region Position Skill Mapping Permission
                                        if (per.fk_permissionname == "PS New")
                                        {
                                            Session["Psnew"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "PS Edit")
                                        {
                                            Session["Psedit"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "PS Delete")
                                        {
                                            Session["Psdelete"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "PS View")
                                        {
                                            Session["Psview"] = per.bitAllowed;
                                        }

                                        #endregion

                                        #region Position Document Mapping Permission

                                        if (per.fk_permissionname == "PDOC New")
                                        {
                                            Session["Pdocnew"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "PDOC Edit")
                                        {
                                            Session["Pdocedit"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "PDOC Delete")
                                        {
                                            Session["Pdocdelete"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "PDOC View")
                                        {
                                            Session["Pdocview"] = per.bitAllowed;
                                        }
                                        #endregion

                                        #region Assessment quest master permission

                                        if (per.fk_permissionname == "ASQ New")
                                        {
                                            Session["Asqnew"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "ASQ Edit")
                                        {
                                            Session["Asqedit"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "ASQ Delete")
                                        {
                                            Session["Asqdelete"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "ASQ View")
                                        {
                                            Session["Asqview"] = per.bitAllowed;
                                        }
                                        #endregion

                                        #region Assessment user wise permission
                                        if (per.fk_permissionname == "Assigned View")
                                        {
                                            Session["AssAssigned"] = per.bitAllowed;
                                        }
                                        #endregion

                                        #region Requisition Permission

                                        if (per.fk_permissionname == "AllowRequisition")
                                        {
                                            Session["Requisition"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "AssignedRequisition")
                                        {
                                            Session["AssignedRequisition"] = per.bitAllowed;
                                        }
                                        #endregion

                                        #region For Consultant KRA upload
                                        if(per.fk_permissionname== "Consultant KRA KPA Upload")
                                        {
                                            Session["KRAUpload"] = per.bitAllowed;
                                        }
                                        if(per.fk_permissionname== "Consultant KRA KPA Update")
                                        {
                                            Session["KRAUpdate"] = per.bitAllowed;
                                        }
                                        #endregion

                                        #region For Consultant Ledger Uploads
                                        if (per.fk_permissionname == "Consultant Ledger Upload")
                                        {
                                            Session["LedgerUpload"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Consultant Ledger Update")
                                        {
                                            Session["LedgerUpdate"] = per.bitAllowed;
                                        }

                                        #endregion

                                        #region Consultant TDS Uploads
                                        if(per.fk_permissionname== "ConsultantActiveView")
                                        {
                                            Session["ConsultantActive"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Consultant TDS Upload")
                                        {
                                            Session["TDSUpload"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "Consultant TDS Update")
                                        {
                                            Session["TDSUpdate"] = per.bitAllowed;
                                        }

                                        #endregion

                                        #region Leave management
                                        if(per.fk_permissionname== "Assigned Leave")
                                        {
                                            Session["AssignedLeave"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "ViewAllEmpUser")
                                        {
                                            base.Session["ViewAllEmpUser"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "UpdateMobile")
                                        {
                                            base.Session["UpdateMobile"] = per.bitAllowed;
                                        }
                                        #endregion

                                        #region DNB Permissions
                                        if (per.fk_permissionname == "DNBCORNER")
                                        {
                                            Session["DNBCORNER"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "FEE-STRUCTURE")
                                        {
                                            Session["FEE-STRUCTURE"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "FEE-VERIFICATION")
                                        {
                                            Session["FEE-VERIFICATION"] = per.bitAllowed;
                                        }
                                        if (per.fk_permissionname == "DNB-REPORTING")
                                        {
                                            Session["DNB-REPORTING"] = per.bitAllowed;
                                        }

                                        #endregion


                                    }
                                }
                                return RedirectToAction("Index", "Dashboard");
                            }
                            else
                            {
                                ModelState.AddModelError("intPK", "Permission not assigned current user!");
                                return View("Index");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Passcode", "Entered password is incorrect please check it and try again!");
                            return View("Index");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("vchUsername", "User name incorrect please check it and try again!");
                        return View("Index");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("intPK", "Model error generated!");
                return View("Index");
            }
        }

        //convert password
        public string ConvertPassword(String Password)
        {
            int PKey = 78;
            string P1;
            char[] ch;
            P1 = "";
            for (int i = 0; i < Password.Length; i++)
            {
                ch = Password.Substring(i, 1).ToCharArray();
                P1 = P1 + ((int)ch[0]) * PKey;
            }

            return P1;
        }

        //partial company view
        public ActionResult CompanyView()
        {
            return View();
        }

        public ActionResult Permission()
        {

            return View();
        }

        public ActionResult SessionError()
        {
            return View();
        }

        //parial session Error
        public ActionResult _SessionError1()
        {
            return View();
        }

        public ActionResult ChangeProfile()
        {
            if (Session["descript"] != null)
            {
                int uid = Convert.ToInt32(Session["usrid"].ToString());
                int bcode = Convert.ToInt32(Session["id"].ToString());
                if (uid != 0 && bcode != 0)
                {
                    HRMEntities hrentity = new HRMEntities();
                    ChangeProfile objpass = new ChangeProfile();
                    var selectedUid = (from e in hrentity.tblUserMaster where e.intid == uid && e.intcode == bcode select e).FirstOrDefault();
                    if (selectedUid != null)
                    {
                        objpass.id = selectedUid.intid;
                        return View(objpass);
                    }
                    else
                    {
                        TempData["Error"] = "Select user not found in database!";
                        return View();
                    }
                }
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1");
            }           
            
        }

        [HttpPost]
        public ActionResult ChangeProfile(ChangeProfile objup,string CroppedImageData)
        {
            if (Session["descript"] != null)
            {
                //check for pdf null
                if (objup.filepath != null)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    //get user seleceted
                    HRMEntities hrentity = new HRMEntities();
                    var getUserDetail = (from e in hrentity.tblUserMaster where e.intid == objup.id && e.intcode==code select e).FirstOrDefault();
                    //filename                       
                    string empid = objup.id.ToString();
                    string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                    //remove space
                    string extension = Path.GetExtension(objup.filepath.FileName);
                    if (extension != ".jpg")
                    {
                        //ModelState.AddModelError("filename", "Please select .jpg file for upload!");
                        TempData["Error"] = "Please select .jpg file for upload!";
                        return RedirectToAction("ChangeProfile");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CroppedImageData))
                        {
                            int userId = Convert.ToInt32(Session["usrid"].ToString());
                            var getUserDetails = (from e in hrentity.tblUserMaster where e.intcode == code && e.intid == userId select e).FirstOrDefault();
                            if (getUserDetail.bitProfileComplete == true)
                            {
                                System.IO.File.Delete(getUserDetail.vchProfilePath);
                            }
                            // Remove the data:image/png;base64, part
                            var base64Data = CroppedImageData.Split(',')[1];
                            var imageBytes = Convert.FromBase64String(base64Data);
                            string filename = Path.GetFileNameWithoutExtension(objup.filepath.FileName);
                            string newfilename = filename + datetime + empid + extension;
                            string finalfilename = newfilename.Replace(" ", "");
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/EmpUserProfile/" + finalfilename));
                            //get only bytes selected
                            System.IO.File.WriteAllBytes(path, imageBytes);
                            //save file in upload folder
                            //objup.filepath.SaveAs(path);
                            getUserDetail.vchProfilePath = path.ToString();
                            getUserDetail.vchProfileName = finalfilename;
                            getUserDetail.bitProfileComplete = true;
                            hrentity.SaveChanges();
                            TempData["Success"] = "Profile picture uploaded successfully!";
                            return View();
                        }
                        else
                        {
                            TempData["Error"] = "Image will not cropped try again!";
                            return RedirectToAction("ChangeProfile");
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "System generated error contact to administrator!";
                    return RedirectToAction("ChangeProfile");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1");
            }
        }

        public ActionResult ChangePassword()
        {
            if (Session["descript"] != null)
            {
                int uid = Convert.ToInt32(Session["usrid"].ToString());
                int bcode = Convert.ToInt32(Session["id"].ToString());
                if (uid != 0 && bcode != 0)
                {
                    HRMEntities hrentity = new HRMEntities();
                    UpdatePassModelView objpass = new UpdatePassModelView();
                    var selectedUid = (from e in hrentity.tblUserMaster where e.intid == uid && e.intcode == bcode select e).FirstOrDefault();
                    if (selectedUid != null)
                    {
                        objpass.userid = selectedUid.intid;
                        return View(objpass);
                    }
                    else
                    {
                        TempData["Error"] = "Select user not found in database!";
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
                return RedirectToAction("_SessionError1");
            }

        }

        public JsonResult UpdatePass(string old, string naya1, string naya2, string usrid)
        {
            if (Session["descript"] != null)
            {
                HRMEntities hrentity = new HRMEntities();
                int uid = Convert.ToInt32(Session["usrid"].ToString());
                int bcode = Convert.ToInt32(Session["id"].ToString());
                if (old != "" && old != null)
                {
                    var selectedusr = (from e in hrentity.tblUserMaster where e.intid == uid && e.intcode == bcode select e).FirstOrDefault();
                    if (selectedusr != null)
                    {
                        string currentpass = selectedusr.Passcode.ToString();
                        if (currentpass == old)
                        {
                            //check enter and confirm password is same
                            if (naya1 == naya2)
                            {
                                selectedusr.Passcode = naya2.ToString();
                                hrentity.SaveChanges();
                                var output1 = "Password updated successfully, please login with new password!";
                                return Json(new { success = "1", output1, JsonRequestBehavior.AllowGet });
                            }
                            else
                            {
                                var output2 = "New password and confirm password does not match, so try again";
                                return Json(new { success = "2", output2, JsonRequestBehavior.AllowGet });
                            }
                        }
                        else
                        {
                            var output3 = "Old password does not match please try again!";
                            return Json(new { success = "3", output3, JsonRequestBehavior.AllowGet });
                        }

                    }
                    else
                    {
                        var output4 = "Model error generated please contact to admin!";
                        return Json(new { success = "4", output4, JsonRequestBehavior.AllowGet });
                    }

                }
                else
                {
                    var output5 = "Old password should not be null, please contact to admin!";
                    return Json(new { success = "5", output5, JsonRequestBehavior.AllowGet });
                }
            }
            else
            {
                var output6 = "Session Error Generated Please login again and try!";
                return Json(new { success = "6", output6, JsonRequestBehavior.AllowGet });
            }
        }

        public ActionResult ChangeSession(int id)
        {
            if (Session["descript"] != null)
            {
                //using (IndusGroupEntities objgroup = new IndusGroupEntities())
                using (HRMEntities hrentity = new HRMEntities())
                using (IndusGroupEntities objgroup = new IndusGroupEntities())
                {
                    var groupid = objgroup.IndusCompanies.Where(g => g.intPK.Equals(id)).FirstOrDefault();
                    string uName = Session["descript"].ToString();
                    var selectedusr = hrentity.tblUserMaster.Where(h => h.vchUsername == uName && h.intcode == id).FirstOrDefault();
                    if (groupid != null)
                    {
                        Session["descript"] = selectedusr.vchUsername.ToString();
                        Session["Compname"] = groupid.descript.ToString();
                        if (Session["descript"] == null)
                        {
                            return RedirectToAction("SessionError");
                        }
                        Session["id"] = groupid.intPK.ToString();
                        Session["yr"] = groupid.ses_code.ToString();
                        Session["usrid"] = selectedusr.intid.ToString();
                        Session["usrdesi"] = selectedusr.fk_intDesignationid.ToString();
                        Session["userdept"] = selectedusr.fk_intDeptid.ToString();
                        Session["HOD_ID"] = selectedusr.intid.ToString();
                        //get selected Branch HR ID
                        var getHrID = (from e in hrentity.tblUserMaster where e.fk_intDesignationid == 12 && e.intcode == id && e.bitActive == true select e).FirstOrDefault();
                        if (getHrID != null)
                        {
                            Session["IS_BHR"] = getHrID.intid.ToString();
                        }
                        //set branch code a session code
                        if (groupid.intPK == 2)
                        {
                            Session["branchCode"] = "HY";
                        }
                        if (groupid.intPK == 3)
                        {
                            Session["branchCode"] = "IS";
                        }
                        if (groupid.intPK == 4)
                        {
                            Session["branchCode"] = "IH";
                        }
                        if (groupid.intPK == 14)
                        {
                            Session["branchCode"] = "II";
                        }
                        if (groupid.intPK == 15)
                        {
                            Session["branchCode"] = "IF";
                        }
                        if (groupid.intPK == 16)
                        {
                            Session["branchCode"] = "KH";
                        }
                        if (groupid.intPK == 21)
                        {
                            Session["branchCode"] = "MH";
                        }
                        if (groupid.intPK == 22)
                        {
                            Session["branchCode"] = "HS";
                        }
                        if (groupid.intPK == 23)
                        {
                            Session["branchCode"] = "MY";
                        }
                        if (groupid.intPK == 24)
                        {
                            Session["branchCode"] = "KS";
                        }
                        string msg = "Indus Company changed successfully!";
                        return RedirectToAction("Index", "Dashboard", new { smsg = msg });
                    }
                    else
                    {
                        string msg = "Indus Company does not changed, please contact to super admin!";
                        return RedirectToAction("Index", "Dashboard", new { smsg = msg });
                    }
                }
            }
            else
            {
                return RedirectToAction("_SessionError1");
            }
        }

        public ActionResult Testing()
        {
            string PCName = "";           
            string HostName = "";
            string RemoteUser = "";
            string InstanceID = "";
            string HostAddress = "";
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                //return only neburo tree ip address
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //PCName = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"].ToString();
                //HostName = System.Web.HttpContext.Current.Request.UserHostName.ToString();
                //RemoteUser = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_USER"].ToString();
                //InstanceID = System.Web.HttpContext.Current.Request.ServerVariables["INSTANCE_ID"].ToString();
                //HostAddress = System.Web.HttpContext.Current.Request.UserHostAddress.ToString();

            }
            //another trail
            PCName = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"].ToString();
            HostName = System.Web.HttpContext.Current.Request.UserHostName.ToString();
            RemoteUser = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_USER"].ToString();
            InstanceID = System.Web.HttpContext.Current.Request.ServerVariables["INSTANCE_ID"].ToString();
            HostAddress = System.Web.HttpContext.Current.Request.UserHostAddress.ToString();
            ViewBag.ClientIp = ip;
            ViewBag.PCName = PCName;          
            ViewBag.HostName = HostName;
            ViewBag.RemoteUser = RemoteUser;
            ViewBag.InstanceID = InstanceID;
            ViewBag.HostAddress = HostAddress;
            return View();
        }               
    }
}


