using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HRM.Utilities;

namespace HRM.Controllers
{
    public class AuthorizationController : Controller
    {
        HRMEntities hrentity = new HRMEntities();
        // GET: Authorization
        public ActionResult NewAuthorization()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var authemp = (from e in hrentity.tblEmpAssesmentMas
                               where e.bitofficialdetails == true && e.bitIsByPassEntry != true
                               && e.bittofficialdetails == true && e.bitgoauthor == false && e.intcode == code
                               select e).ToList();
                if (authemp.Count != 0)
                {
                    return View(authemp);
                }
                else
                {
                    TempData["Empty"] = "Currently no new employee avilable for send authorization!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

    //fill all official details for authorization employee
    public ActionResult FillOfficial(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                    if (selectedemp != null)
                    {
                        return View(selectedemp);
                    }
                    else
                    {
                        TempData["Invalid"] = "Select employee details not found please check it again!";
                        return RedirectToAction("NewAuthorization");
                    }
                }
                else
                {
                    TempData["Invalid"] = "Selected employee id error generated please contact to admin!";
                    return RedirectToAction("NewAuthorization");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult SetAuthorization(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult SetAuthorization(ReSendAuthorViewModel objremarks)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    //master entry
                    tblEmpAssesmentMas objmas = new tblEmpAssesmentMas();
                    //set first date send for authorization
                    var getMasEntry = (from e in hrentity.tblEmpAssesmentMas where e.intid == objremarks.id select e).FirstOrDefault();
                    if (getMasEntry.dtFirstAuthor == null)
                    {
                        getMasEntry.dtFirstAuthor = DateTime.Now;
                    }
                    tblConversation objconversation = new tblConversation();
                    int userid = Convert.ToInt32(Session["usrid"]);
                    string username = Session["descript"].ToString();
                    string host = Session["hostname"].ToString();
                    string ipused = Session["ipused"].ToString();
                    int id = objremarks.id;
                    int bcode = Convert.ToInt32(Session["id"].ToString());
                    objconversation.fk_intEmpid = id;
                    objconversation.vchMsg = objremarks.vchHrSolRemark;
                    objconversation.fk_uid = userid;
                    objconversation.fk_UserName = username;
                    objconversation.vchHost = host;
                    objconversation.vchIpused = ipused;
                    objconversation.dtMsgDate = DateTime.Now;
                    hrentity.tblConversation.Add(objconversation);
                    hrentity.SaveChanges();
                    string compName = Session["branchCode"].ToString();
                    string compFullName = Session["Compname"].ToString();
                    #region company old name
                    //if (bcode == 3)
                    //{
                    //    compName = "ISSH";
                    //}
                    //else if (bcode == 4)
                    //{
                    //    compName = "IH";
                    //}
                    //else if (bcode == 14)
                    //{
                    //    compName = "IIH";
                    //}
                    //else if (bcode == 15)
                    //{
                    //    compName = "IFSH";
                    //}
                    //else if (bcode == 16)
                    //{
                    //    compName = "KS Homecare";
                    //}
                    //else if (bcode == 2)
                    //{
                    //    compName = "Hygiea";
                    //}
                    //else if (bcode == 21)
                    //{
                    //    compName = "Mehndiratta";
                    //}
                    //else if (bcode == 22)
                    //{
                    //    compName = "HealthSure";
                    //}
                    //else if (bcode == 23)
                    //{
                    //    compName = "My Hospital";
                    //}
                    //else if (bcode == 24)
                    //{
                    //    compName = "KS Pharma";
                    //}
                    //else if (bcode == 24)
                    //{
                    //    compName = "Velmed";
                    //}
                    #endregion
                    int code = Convert.ToInt32(Session["id"].ToString());
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                    if (selectedemp != null)
                    {
                        selectedemp.bitgoauthor = true;
                        hrentity.SaveChanges();
                        int salaryrange = Convert.ToInt32(selectedemp.intsalary);
                        var selecteduser = (from e in hrentity.tblUserAuthorize where e.intSalaryTo >= salaryrange select e).FirstOrDefault();                        
                        if (selecteduser != null)
                        {
                            string name = selecteduser.vchUserName;
                            var userMas = (from e in hrentity.tblUserMaster where e.vchUsername == name select e).FirstOrDefault();
                            //call sms api if mobile not null
                            var senderEmail = Session["UserEmail"].ToString();
                            var mob = userMas.vchMobile;
                            var email = userMas.vchEmail;                           
                            if (Session["UserEmail"] != null)
                            {
                                senderEmail = Session["UserEmail"].ToString();
                            }
                            string candidate = selectedemp.vchName.ToString();
                            if (mob != null)
                            {                                
                                var uri = "http://164.52.195.161/API/SendMsg.aspx?uname=20170839&pass=Indus2017&send=HOSPIN&dest=" + mob + "&msg=" + candidate + " Candidate authorization assigned to you in " + compName + ".Indus Healthcare Services Pvt. Ltd.&priority=1";
                                if (uri != null)
                                {
                                    try
                                    {
                                        HttpWebRequest createrequest = (HttpWebRequest)WebRequest.Create(uri);
                                        // getting response of sms
                                        HttpWebResponse myResp = (HttpWebResponse)createrequest.GetResponse();
                                        System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                                        string responseString = _responseStreamReader.ReadToEnd();
                                        _responseStreamReader.Close();
                                        myResp.Close();
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            #region send email success
                            //send email notification
                            if (email != null)
                            {
                                string to = email;
                                string subject1 = "New candidate authorization";
                                string bodymsg = candidate + " Candidate authorization assigned to you in " + compFullName + ". Indus Healthcare Services Pvt.LTD.";
                                string body = $@"
                                         <p>Dear Sir/Madam,</p>
                                         <p>We hope this message finds you well.</p>
                                         <p><strong>{bodymsg}</strong></p>
                                         <p>To view the full details, please log in to the HRMS portal using the link below:</p>
                                         <p><a href='http://hrms.indusafrica.org/' style='color:#1a73e8;'>Click to login HRMS</a></p>
                                         <p>If you have any questions or need assistance, feel free to contact the HR team.                                        
                                         <br/>
                                         <p><strong>Best regards,<br/>
                                          HRMS Dashboard<br/>
                                          Indus Healthcare Services Pvt. Ltd.<br/>
                                          </strong>                                          
                                          <a href='https://www.indushospital.in/'>Indus Hospitals on web</a></p>
                                          <hr />
                                          <small>This is an automated message from HRMS Dashboard. Please do not reply to this email.</small><hr/>";
                                Boolean success = EmailHelper.SendEmail(to, subject1, body);
                                if (success)
                                {
                                    TempData["Success"] = "Authorization send successfully with SMS and email notification!";
                                }
                                else
                                {
                                    TempData["Success"] = "Authorization send successfully with SMS notification!";
                                }
                            }
                            #endregion
                        }                        
                        return RedirectToAction("NewAuthorization");
                    }
                    TempData["Error"] = "Employee complete data not found please try again!";
                    return RedirectToAction("NewAuthorization");
                }
                TempData["Error"] = "Employee id not selected please assign again or contact to application administrator!";
                return View("NewAuthorization");
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult ViewAssignAuthorization()
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    if (Session["Sfrom"] != null)
                    {

                        int salfrom = Convert.ToInt32(Session["Sfrom"].ToString());
                        int salto = Convert.ToInt32(Session["Sto"].ToString());
                        var assignedauthor = (from e in hrentity.tblEmpAssesmentMas                                             
                                              where e.bitofficialdetails == true && e.bittofficialdetails == true
                                              && e.bitgoauthor == true && e.bitauthorised == false
                                              && e.bitauthorcancel == false && e.bitIsLeft!=true && e.BitIsFlaggingEmp!=true
                                              && e.intsalary >= salfrom && e.intsalary <= salto
                                              && e.intcode == code
                                              select e).ToList();
                        if (assignedauthor.Count != 0)
                        {
                            return View(assignedauthor);
                        }
                        TempData["Empty"] = "Currently no new authorization avilable for you!";
                        return View();
                    }
                    else if (Session["HrAdmin"].ToString() != null)
                    {
                        string ishr = Session["HrAdmin"].ToString();
                        if (ishr == "True")
                        {
                            var assignedauthor = (from e in hrentity.tblEmpAssesmentMas
                                                  where e.bitgoauthor == true && e.bitauthorised == false && e.bitauthorcancel == false && e.intcode == code
                                                  select e).ToList();
                            if (assignedauthor.Count != 0)
                            {
                                return View(assignedauthor);

                            }
                        }
                        TempData["Empty"] = "Currently no new authorization avilable for you!";
                        return View();
                    }
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
            return View();
        }

        //Update HR/HR Assistance Document on request authoriser
        public ActionResult UpdateDocument(int id)
        {
            if (Session["descript"] != null)
            {
                PartialReconsiderationRemarks(id);
                if (id != 0)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    var selection = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).ToList();
                    if (selection != null)
                    {
                        //return RedirectToAction("UpdateDocument","Authorization",new { id = id });
                        return View(selection);
                    }
                    else
                    {
                        TempData["Error"] = "Selected candidate document not found please contact to admin!";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "Selected candidate id not found please contact to admin!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult Updocdetail(int id)
        {
            if (id != 0)
            {
                return RedirectToAction("ViewCompDocEmp", "Assesment", new { id = id });
            }
            else
            {
                TempData["Error"] = "Id should not be zero or null!";
                return View();
            }

        }

        public ActionResult EditOfficial(int id)
        {

            if (id != 0)
            {
                return RedirectToAction("EditEmpl", "AddNewEmp", new { id = id });
            }
            else
            {
                TempData["Error"] = "Id should not be zero or null!";
                return View();
            }
        }

        public ActionResult ViewFullDetails(int id)
        {
            PartialReconsiderationRemarks(id);
            PartialAssessmentRemarks(id);
            //PartialEmpMas(id);
            //PartialEmpDetails(id);
            //PartialDocComp(id);
            //PartialDocQuali(id);
            PartialAutorizationAction(id);
            return View();
        }

        public ActionResult PartialReconsiderationRemarks(int id)
        {
            if (Session["descript"] != null || Session["UserId"] != null)
            {
                var isAdmin = string.Empty;
                if (Session["MainAdmin"] != null)
                {
                    isAdmin = Session["MainAdmin"].ToString();
                }
                var selectedremarks = (from e in hrentity.tblConversation where e.fk_intEmpid == id orderby e.dtMsgDate descending select e).ToList();
                var oldremarks = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                if (selectedremarks.Count() != 0)
                {
                    return View(selectedremarks);
                }
                if (oldremarks != null)
                {
                    {
                        if (oldremarks != null && oldremarks.vchauthorisedmsg != null)
                        {
                            ViewBag.OldMsg = oldremarks.vchauthorisedmsg.ToString();
                        }
                        else
                        {
                            ViewBag.Empty1 = "Authorizer remarks not avilable";
                        }
                        if (oldremarks != null && oldremarks.vchHrSolRemark != null)
                        {
                            ViewBag.HrOldMsg = oldremarks.vchHrSolRemark.ToString();
                        }
                        else
                        {
                            ViewBag.Empty2 = "Hr remarks not avilable";
                        }
                        return View();
                    }
                }
                else
                {
                    ViewBag.Empty = "Remerks not found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PartialAssessmentRemarks(int id)
        {
            if (Session["descript"] != null || Session["UserId"]!=null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                List<tblAssesmentQuestDetails> tbllist = new List<tblAssesmentQuestDetails>();
                var selecteddata = (from e in hrentity.spGetAssesmentRemarks(id, code) select e).ToList();
                if (selecteddata.Count()!=0)
                {
                    return View(selecteddata);
                }
                else
                {
                    ViewBag.Empty = "Assessment not found coz payroll/bypass employee";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PartialEmpMas(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var personaldetails = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).ToList();
                return View(personaldetails);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PartialEmpDetails(int id)
        {

            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var contactdet = (from e in hrentity.tblEmpDetails where e.fk_intempid == id && e.intcode == code select e).ToList();
                return View(contactdet);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PartialDocComp(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                //var docdetails = (from e in hrentity.tblDocDetails where e.fk_empid == id select e).ToList();
                var docdetails = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompDoc == true && e.intcode == code select e).ToList();
                return View(docdetails);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PartialDocQuali(int id)
        {
            if (Session["descript"] != null)
            {
                //var docdetails = (from e in hrentity.tblDocDetails where e.fk_empid == id select e).ToList();
                int code = Convert.ToInt32(Session["id"].ToString());
                var docdetails = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompQuali == true && e.intcode == code select e).ToList();
                return View(docdetails);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PartialProfile(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                if (id != 0)
                {
                    var docdetails = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsProfilePic == true && e.intcode == code select e).ToList();
                    return View(docdetails);
                }
                else
                {
                    //Partially used so don't use temp data TempData["Empty"] = "Profile picture not avilable!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult SendAuthorizationWithMsg(int id)
        {
            return View();
        }

        public ActionResult PartialResendAuthorization(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult PartialResendAuthorization(ReSendAuthorViewModel obmodel)
        {
            if (Session["descript"] != null)
            {
                string username = Session["descript"].ToString();
                string ipused = Session["ipused"].ToString();
                string host = Session["hostname"].ToString();
                int userid = Convert.ToInt32(Session["usrid"].ToString());
                int empid = obmodel.id;
                tblConversation objmessage = new tblConversation();
                var selected = (from e in hrentity.tblEmpAssesmentMas where e.intid == obmodel.id select e).FirstOrDefault();
                string status = obmodel.bitAuthorBack.ToString();
                int code = Convert.ToInt32(Session["id"].ToString());
                if (status == "True")
                {
                    selected.bitAuthorBack = false;
                    hrentity.SaveChanges();
                    if (obmodel.vchHrSolRemark != null)
                    {
                        objmessage.vchMsg = obmodel.vchHrSolRemark;
                        objmessage.fk_intEmpid = empid;
                        objmessage.fk_uid = userid;
                        objmessage.fk_UserName = username;
                        objmessage.vchIpused = ipused;
                        objmessage.vchHost = host;
                        objmessage.dtMsgDate = DateTime.Now;
                        objmessage.intcode = code;
                        hrentity.tblConversation.Add(objmessage);
                        hrentity.SaveChanges();
                    }
                    int bcode = Convert.ToInt32(Session["id"].ToString());
                    string compName = "";
                    if (bcode == 3)
                    {
                        compName = "ISSH";
                    }
                    else if (bcode == 4)
                    {
                        compName = "IH";
                    }
                    else if (bcode == 14)
                    {
                        compName = "IIH";
                    }
                    else if (bcode == 15)
                    {
                        compName = "IFSH";
                    }
                    else if (bcode == 16)
                    {
                        compName = "KS Homecare";
                    }
                    else if (bcode == 2)
                    {
                        compName = "Hygiea";
                    }
                    else if (bcode == 21)
                    {
                        compName = "Mehndiratta";
                    }
                    else if (bcode == 22)
                    {
                        compName = "HealthSure";
                    }
                    else if (bcode == 23)
                    {
                        compName = "MyHospital";
                    }
                    else if (bcode == 24)
                    {
                        compName = "KSPharma";
                    }
                    else if (bcode == 25)
                    {
                        compName = "Velmed";
                    }

                    int salaryrange = Convert.ToInt32(selected.intsalary);
                    var selecteduser = (from e in hrentity.tblUserAuthorize where e.intSalaryTo >= salaryrange select e).FirstOrDefault();
                    if (selecteduser != null)
                    {
                        string name = selecteduser.vchUserName;
                        var userMaster = (from e in hrentity.tblUserMaster where e.vchUsername == name select e).FirstOrDefault();
                        //call sms api if mobile not null
                        string mob = userMaster.vchMobile;
                        string email = userMaster.vchEmail;
                        string senderEmail = String.Empty;
                        if (Session["UserEmail"] != null)
                        {
                            senderEmail = Session["UserEmail"].ToString();
                        }
                        if (mob != null)
                        {
                            string candidate = selected.vchName.ToString();
                            var uri = "http://164.52.195.161/API/SendMsg.aspx?uname=20170839&pass=Indus2017&send=HOSPIN&dest=" + mob + "&msg=" + candidate + " Candidate authorization assigned to you in " + compName + ".Indus Healthcare Services Pvt. Ltd.&priority=1";
                            if (uri != null)
                            {
                                try
                                {
                                    HttpWebRequest createrequest = (HttpWebRequest)WebRequest.Create(uri);
                                    // getting response of sms
                                    HttpWebResponse myResp = (HttpWebResponse)createrequest.GetResponse();
                                    System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                                    string responseString = _responseStreamReader.ReadToEnd();
                                    _responseStreamReader.Close();
                                    myResp.Close();
                                }
                                catch
                                {

                                }
                            }
                            #region Mail siccess but google change aunthetication
                            //send email notification
                            //if (email != null)
                            //{
                            //    string to = email;
                            //    string subject1 = "Re-assign candidate authorization";
                            //    string bodymsg = candidate + " Candidate authorization re-assigned to you in " + compName + " Indus Healthcare Services Pvt.LTD.";
                            //    string body = $@"
                            //            <p>Dear Sir/Madam,</p>
                            //            <p>We hope this message finds you well.</p>
                            //            <p><strong>{bodymsg}</strong></p>
                            //            <p>To view the full details, please log in to the HRMS portal using the link below:</p>
                            //            <p><a href='http://hrms.indusafrica.org/' style='color:#1a73e8;'>Login your HRMS dashboard</a></p>
                            //            <p>If you have any questions or need assistance, feel free to contact the HR team at 
                            //            <a href='mailto:{senderEmail}'>Email</a>.</p>
                            //            <br/>
                            //            <p><strong>Best regards,<br/>
                            //             HRMS Dashboard<br/>
                            //             Indus Healthcare Services Pvt. Ltd.<br/>
                            //             </strong>
                            //             +91-**********<br/>
                            //             <a href='https://www.indushospital.in/'>Indus Hospitals on web</a></p>
                            //             <hr />
                            //             <small>This is an automated message from HRMS Dashboard. Please do not reply to this email.</small><hr/>";
                            //    Boolean success = EmailHelper.SendEmail(to, subject1, body);
                            //    if (success)
                            //    {

                            //    }
                            //    else
                            //    {

                            //    }
                            //}
                            #endregion
                            TempData["Success"] = "Authorization re-sent successfully with SMS notification and email!";
                            return RedirectToAction("ViewAssignAuthorization");
                        }
                        else
                        {

                            TempData["Error"] = "User mobile number not found, notification message not sent to HR!";
                            return RedirectToAction("ViewAssignAuthorization", TempData["Error"]);
                        }
                    }
                    else
                    {
                        TempData["Error"] = "HR user not found please contact to administrator!";
                        return RedirectToAction("ViewAssignAuthorization", TempData["Error"]);
                    }
                }
                else
                {
                    TempData["Error"] = "Please select yes if you want re-send authorization!";
                    return RedirectToAction("ViewAssignAuthorization");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1");
            }
        }

        public ActionResult PartialBackAuthorization(int id)
        {
            return View();
        }

        public ActionResult AuthoriserUpdateSalary(int id)
        {
            if (Session["descript"] != null)
            {
                UpdateSalaryViewModel objnew = new UpdateSalaryViewModel();
                int code = Convert.ToInt32(Session["id"].ToString());
                if (code != 0)
                {
                    var objtbl = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    if (objtbl != null)
                    {
                        objnew.empid = objtbl.intid;
                        objnew.salary = Convert.ToInt32(objtbl.intsalary);
                        return View(objnew);
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

        public JsonResult UpdateSalary(string empid, string salary)
        {
            if (Session["descript"] != null)
            {
                if (empid != null && empid != "")
                {
                    int id = Convert.ToInt32(empid.ToString());
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    if (selectedemp != null)
                    {
                        if (salary != null)
                        {
                            selectedemp.intsalary = Convert.ToInt32(salary.ToString());
                            hrentity.SaveChanges();
                            var output1 = "Salary updated successfully!";
                            return Json(new { success = "1", output1, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            var output2 = "Salary sohuld not be 0 or empty!";
                            return Json(new { success = "2", output2, JsonRequestBehavior.AllowGet });
                        }
                    }
                    else
                    {
                        var output3 = "Selected employee not found!";
                        return Json(new { success = "3", output3, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    var output4 = "Candidate id not found!";
                    return Json(new { success = "4", output4, JsonRequestBehavior.AllowGet });
                }
            }
            else
            {
                var output6 = "Session Error Generated Please login again and try it!";
                return Json(new { success = "6", output6, JsonRequestBehavior.AllowGet });
            }
        }

        public ActionResult PartialAutorizationAction(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var empmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                PartialAuthorizationActionViewModel objmodel = new PartialAuthorizationActionViewModel();
                objmodel.id = empmas.intid;
                objmodel.salaryCheck = Convert.ToInt32(empmas.intsalary);
                return View(objmodel);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
        
        public ActionResult PartialOtherDocument(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var othedoc = (from e in hrentity.tblOtherDocDetail where e.fk_empAssid == id select e).ToList();                
                return View(othedoc);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult ViewFullDetails(PartialAuthorizationActionViewModel objauthormodel) //, AuthorBackViewModel objmodelResend)
        {
            if (Session["descript"] != null)
            {
                //Get session variables
                string username = Session["descript"].ToString();
                string ipused = Session["ipused"].ToString();
                string host = Session["hostname"].ToString();
                int userid = Convert.ToInt32(Session["usrid"].ToString());
                tblConversation objmessage = new tblConversation();
                //if candidate will be authorised
                if (objauthormodel.bitAuthorization == "Yes")
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int empid = Convert.ToInt32(objauthormodel.id);
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    selectedemp.vchauthorisedby = username;
                    selectedemp.vchipdusedauthor = ipused;
                    selectedemp.bitauthorised = true;
                    selectedemp.dtauthorised = DateTime.Now;
                    selectedemp.vchautorisedhost = host;
                    if (selectedemp.dtApprovedAuthor == null)
                    {
                        selectedemp.dtApprovedAuthor = DateTime.Now;
                    }
                    if (objauthormodel.variablePart != 0)
                    {
                        selectedemp.intVariable = objauthormodel.variablePart;
                        selectedemp.intFixed = objauthormodel.fixedPart;
                        selectedemp.intTotal = objauthormodel.salaryCheck;
                    }
                    if (objauthormodel.vchRemarks != null)
                    {
                        objmessage.vchMsg = objauthormodel.vchRemarks;
                        objmessage.fk_intEmpid = empid;
                        objmessage.fk_uid = userid;
                        objmessage.fk_UserName = username;
                        objmessage.vchIpused = ipused;
                        objmessage.vchHost = host;
                        objmessage.dtMsgDate = DateTime.Now;
                        objmessage.intcode = code;
                        objmessage.bitIsAutorizationMsg = true;
                        hrentity.tblConversation.Add(objmessage);
                        hrentity.SaveChanges();
                    }
                    else
                    {
                        selectedemp.vchauthorisedmsg = "Not Provided";
                    }
                    hrentity.SaveChanges();
                    var HRuser = (from e in hrentity.tblUserMaster where e.fk_intDesignationid == 12 && e.intcode == code select e).FirstOrDefault();
                    if (HRuser != null)
                    {
                        string candidate = selectedemp.vchName.ToString();
                        string HRmobileNo = HRuser.vchMobile;
                        string HREmail = HRuser.vchEmail;
                        string senderEmail = string.Empty;
                        if (Session["UserEmail"] != null)
                        {
                            senderEmail = Session["UserEmail"].ToString();
                        }
                        var uri = "http://164.52.195.161/API/SendMsg.aspx?uname=20170839&pass=Indus2017&send=HOSPIN&dest=" + HRmobileNo + "&msg=" + candidate + " Candidate authorization completed successfully please check your HRMS dashboard.Indus Healthcare Services Pvt. Ltd.&priority=1";
                        if (uri != null)
                        {
                            try
                            {
                                HttpWebRequest createrequest = (HttpWebRequest)WebRequest.Create(uri);
                                // getting response of sms
                                HttpWebResponse myResp = (HttpWebResponse)createrequest.GetResponse();
                                System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                                string responseString = _responseStreamReader.ReadToEnd();
                                _responseStreamReader.Close();
                                myResp.Close();
                            }
                            catch
                            {

                            }
                        }
                        #region send email notification success
                        //send email notification
                        //if (HREmail != null)
                        //{
                        //    string to = HREmail;
                        //    string subject1 = "New candidate authorization";
                        //    string bodymsg = candidate + "Candidate authorization completed successfully please check your HRMS dashboard";
                        //    string body = $@"
                        //                <p>Dear Sir/Madam,</p>
                        //                <p>We hope this message finds you well.</p>
                        //                <p><strong>{bodymsg}</strong></p>
                        //                <p>To view the full details, please log in to the HRMS portal using the link below:</p>
                        //                <p><a href='http://hrms.indusafrica.org/' style='color:#1a73e8;'>Login your HRMS dashboard</a></p>
                        //                <p>If you have any questions or need assistance, feel free to contact the HR team at 
                        //                <a href='mailto:{senderEmail}'>Email</a>.</p>
                        //                <br/>
                        //                <p><strong>Best regards,<br/>
                        //                 HRMS Dashboard<br/>
                        //                 Indus Healthcare Services Pvt. Ltd.<br/>
                        //                 </strong>
                        //                 +91-**********<br/>
                        //                 <a href='https://www.indushospital.in/'>Indus Hospitals on web</a></p>
                        //                 <hr />
                        //                 <small>This is an automated message from HRMS Dashboard. Please do not reply to this email.</small><hr/>";
                        //    Boolean success = EmailHelper.SendEmail(to, subject1, body);
                        //    if (success)
                        //    {

                        //    }
                        //    else
                        //    {

                        //    }
                        //}
                        #endregion

                        TempData["Success"] = "Candidate authorization submit successfully!";
                        return RedirectToAction("ViewAssignAuthorization", TempData["Success"]);
                    }
                    return RedirectToAction("ViewAssignAuthorization");
                }
                //if candidate authorization simply cancel
                else if (objauthormodel.bitAuthorization == "No")
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int empid = Convert.ToInt32(objauthormodel.id);
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    selectedemp.bitauthorcancel = true;
                    selectedemp.vchauthorisedby = username;
                    selectedemp.vchipdusedauthor = ipused;
                    selectedemp.dtauthorised = DateTime.Now;
                    selectedemp.vchhostname = host;
                    if (objauthormodel.variablePart != 0)
                    {
                        selectedemp.intVariable = objauthormodel.variablePart;
                        selectedemp.intFixed = objauthormodel.fixedPart;
                        selectedemp.intTotal = objauthormodel.salaryCheck;
                    }
                    if (selectedemp.bitIsByPassEntry == true)
                    {
                        selectedemp.BitIsUploadCompleted = false;
                    }
                    if (objauthormodel.vchRemarks != null)
                    {
                        objmessage.vchMsg = objauthormodel.vchRemarks;
                        objmessage.fk_intEmpid = empid;
                        objmessage.fk_uid = userid;
                        objmessage.fk_UserName = username;
                        objmessage.vchIpused = ipused;
                        objmessage.vchHost = host;
                        objmessage.dtMsgDate = DateTime.Now;
                        objmessage.bitIsAuthorCancelReason = true;
                        objmessage.intcode = code;
                        hrentity.tblConversation.Add(objmessage);
                        hrentity.SaveChanges();
                    }
                    else
                    {
                        selectedemp.vchcancelreason = "Not Provided";
                    }
                    hrentity.SaveChanges();
                    var getHRUser = (from e in hrentity.tblUserMaster where e.fk_intDesignationid == 12 && e.intcode == code select e).FirstOrDefault();
                    string HRmobileNo = getHRUser.vchMobile;
                    string email = getHRUser.vchEmail;
                    string senderEmail = string.Empty;
                    if (Session["UserEmail"] != null)
                    {
                        senderEmail = Session["UserEmail"].ToString();
                    }
                    if (HRmobileNo != null)
                    {
                        string candidate = selectedemp.vchName.ToString();
                        var uri = "http://164.52.195.161/API/SendMsg.aspx?uname=20170839&pass=Indus2017&send=HOSPIN&dest=" + HRmobileNo + "&msg=" + candidate + " Candidate authorization completed successfully please check your HRMS dashboard.Indus Healthcare Services Pvt. Ltd.&priority=1";
                        if (uri != null)
                        {
                            try
                            {
                                HttpWebRequest createrequest = (HttpWebRequest)WebRequest.Create(uri);
                                // getting response of sms
                                HttpWebResponse myResp = (HttpWebResponse)createrequest.GetResponse();
                                System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                                string responseString = _responseStreamReader.ReadToEnd();
                                _responseStreamReader.Close();
                                myResp.Close();
                            }
                            catch
                            {

                            }
                        }
                        TempData["Success"] = "Candidate autorization cancelled successfully!";
                        return RedirectToAction("ViewAssignAuthorization", TempData["Success"]);
                    }
                    else
                    {
                        TempData["Error"] = "User mobile number not found so candidate authorization canclled successfully but notification message will not sent!";
                        return RedirectToAction("ViewAssignAuthorization", TempData["Error"]);
                    }
                }
                //if authorization will send back for re-consideration message can be send for re-consideration here
                else if(objauthormodel.bitAuthorization == "Reconsider")
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int empid = Convert.ToInt32(objauthormodel.id);
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    selectedemp.bitAuthorBack = true;
                    if (objauthormodel.variablePart != 0)
                    {
                        selectedemp.intVariable = objauthormodel.variablePart;
                        selectedemp.intFixed = objauthormodel.fixedPart;
                        selectedemp.intTotal = objauthormodel.salaryCheck;
                    }
                    hrentity.SaveChanges();
                    if (objauthormodel.vchRemarks != null)
                    {
                        objmessage.vchMsg = objauthormodel.vchRemarks;
                        objmessage.fk_intEmpid = empid;
                        objmessage.fk_uid = userid;
                        objmessage.fk_UserName = username;
                        objmessage.vchIpused = ipused;
                        objmessage.vchHost = host;
                        objmessage.dtMsgDate = DateTime.Now;
                        objmessage.intcode = code;
                        hrentity.tblConversation.Add(objmessage);
                        hrentity.SaveChanges();
                    }
                    else
                    {
                        objmessage.vchMsg = "Not provided";
                        objmessage.fk_intEmpid = empid;
                        objmessage.fk_uid = userid;
                        objmessage.fk_UserName = username;
                        objmessage.vchIpused = ipused;
                        objmessage.vchHost = host;
                        objmessage.dtMsgDate = DateTime.Now;
                        objmessage.intcode = code;
                        hrentity.tblConversation.Add(objmessage);
                        hrentity.SaveChanges();
                    }
                    TempData["Success"] = "Candiate sent successfully for re-consideration to HR team!";
                    return RedirectToAction("ViewAssignAuthorization", TempData["Success"]);
                }
                else if (objauthormodel.bitAuthorization == "PartialAuthorised")
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int empid = Convert.ToInt32(objauthormodel.id);
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    selectedemp.bitauthorised = true;
                    selectedemp.bitIsPartialAuthorised = true;
                    if (selectedemp.dtApprovedAuthor == null)
                    {
                        selectedemp.dtApprovedAuthor = DateTime.Now;
                    }
                    if (objauthormodel.variablePart != 0)
                    {
                        selectedemp.intVariable = objauthormodel.variablePart;
                        selectedemp.intFixed = objauthormodel.fixedPart;
                        selectedemp.intTotal = objauthormodel.salaryCheck;
                    }
                    hrentity.SaveChanges();
                    if (objauthormodel.vchRemarks != null)
                    {
                        objmessage.vchMsg = objauthormodel.vchRemarks;
                        objmessage.fk_intEmpid = empid;
                        objmessage.fk_uid = userid;
                        objmessage.fk_UserName = username;
                        objmessage.vchIpused = ipused;
                        objmessage.vchHost = host;
                        objmessage.dtMsgDate = DateTime.Now;
                        objmessage.intcode = code;
                        hrentity.tblConversation.Add(objmessage);
                        hrentity.SaveChanges();
                    }
                    else
                    {
                        objmessage.vchMsg = "Not provided";
                        objmessage.fk_intEmpid = empid;
                        objmessage.fk_uid = userid;
                        objmessage.fk_UserName = username;
                        objmessage.vchIpused = ipused;
                        objmessage.vchHost = host;
                        objmessage.dtMsgDate = DateTime.Now;
                        objmessage.intcode = code;
                        hrentity.tblConversation.Add(objmessage);
                        hrentity.SaveChanges();
                    }
                    TempData["Success"] = "Candiate Partial Authorised successfully!";
                    return RedirectToAction("ViewAssignAuthorization", TempData["Success"]);
                }
                //if authorization will be cancel from re-considertation view
                else if (objauthormodel.bitAuthorization == "Forward")
                {
                    //add forward table detail
                    tblVPForward objtable = new tblVPForward();
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int empid = Convert.ToInt32(objauthormodel.id);
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    objtable.vchForwardBy = Session["descript"].ToString();
                    objtable.dtForwarded = DateTime.Now;
                    objtable.bitStatus = true;
                    objtable.fk_EmpID = selectedemp.intid;
                    selectedemp.bitIsFwardVp = true;
                    selectedemp.bitIsAprvdVp = false;
                    hrentity.tblVPForward.Add(objtable);
                    hrentity.SaveChanges();
                    if (objauthormodel.vchRemarks != null)
                    {
                        objmessage.vchMsg = objauthormodel.vchRemarks;
                        objmessage.fk_intEmpid = empid;
                        objmessage.fk_uid = userid;
                        objmessage.fk_UserName = username;
                        objmessage.vchIpused = ipused;
                        objmessage.vchHost = host;
                        objmessage.dtMsgDate = DateTime.Now;
                        objmessage.intcode = code;
                        hrentity.tblConversation.Add(objmessage);
                        hrentity.SaveChanges();
                    }
                    else
                    {
                        objmessage.vchMsg = "Not provided";
                        objmessage.fk_intEmpid = empid;
                        objmessage.fk_uid = userid;
                        objmessage.fk_UserName = username;
                        objmessage.vchIpused = ipused;
                        objmessage.vchHost = host;
                        objmessage.dtMsgDate = DateTime.Now;
                        objmessage.intcode = code;
                        hrentity.tblConversation.Add(objmessage);
                        hrentity.SaveChanges();
                    }
                    //send text information to VP HR designation id=3175
                    var getVp = (from e in hrentity.tblUserMaster where e.fk_intDesignationid == 3175 && e.intcode == code select e).FirstOrDefault();                    
                    string mob = getVp.vchMobile;
                    string VPEmail = getVp.vchEmail;
                    string senderEmail = string.Empty;
                    if (Session["UserEmail"] != null)
                    {
                        senderEmail = Session["UserEmail"].ToString();
                    }
                    if (mob != null)
                    {
                        string CompCode = Session["branchCode"].ToString();
                        string candidate = selectedemp.vchName.ToString();
                        var uri = "http://164.52.195.161/API/SendMsg.aspx?uname=20170839&pass=Indus2017&send=HOSPIN&dest=" + mob + "&msg=" + candidate + " Candidate authorization assigned to you in " + CompCode + ".Indus Healthcare Services Pvt. Ltd.&priority=1";
                        if (uri != null)
                        {
                            try
                            {
                                HttpWebRequest createrequest = (HttpWebRequest)WebRequest.Create(uri);
                                // getting response of sms
                                HttpWebResponse myResp = (HttpWebResponse)createrequest.GetResponse();
                                System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                                string responseString = _responseStreamReader.ReadToEnd();
                                _responseStreamReader.Close();
                                myResp.Close();
                            }
                            catch
                            {

                            }
                        }
                        #region Send email to VP success
                        //send email notification
                        //if (VPEmail != null)
                        //{
                        //    string to = VPEmail;
                        //    string subject1 = "Forwarded authorization";
                        //    string bodymsg = candidate + "Candidate authorization forwarded to you in HRMS dashboard";
                        //    string body = $@"
                        //                <p>Dear Sir/Madam,</p>
                        //                <p>We hope this message finds you well.</p>
                        //                <p><strong>{bodymsg}</strong></p>
                        //                <p>To view the full details, please log in to the HRMS portal using the link below:</p>
                        //                <p><a href='http://hrms.indusafrica.org/' style='color:#1a73e8;'>Login your HRMS dashboard</a></p>
                        //                <p>If you have any questions or need assistance, feel free to contact the HR team at 
                        //                <a href='mailto:{senderEmail}'>Email</a>.</p>
                        //                <br/>
                        //                <p><strong>Best regards,<br/>
                        //                 HRMS Dashboard<br/>
                        //                 Indus Healthcare Services Pvt. Ltd.<br/>
                        //                 </strong>
                        //                 +91-**********<br/>
                        //                 <a href='https://www.indushospital.in/'>Indus Hospitals on web</a></p>
                        //                 <hr />
                        //                 <small>This is an automated message from HRMS Dashboard. Please do not reply to this email.</small><hr/>";
                        //    Boolean success = EmailHelper.SendEmail(to, subject1, body);
                        //    if (success)
                        //    {

                        //    }
                        //    else
                        //    {

                        //    }
                        //}
                        #endregion
                        TempData["Success"] = "Notification message sent and Authorization Forwarded successfully To VP HR!";
                        return RedirectToAction("ViewAssignAuthorization");
                    }
                    TempData["Success"] = "Authorization Forwarded successfully To VP HR!";
                    return RedirectToAction("ViewAssignAuthorization");
                }
                else
                {
                    TempData["Success"] = "Authorization Forwarded successfully To VP HR!";
                    return RedirectToAction("ViewAssignAuthorization");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //view for HR team when an eomplyee is authorised by authorise person 
        public ActionResult ViewAuthorisedEmp()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var authorisedemp = (from e in hrentity.tblEmpAssesmentMas
                                     where e.bitauthorised == true && e.bittempstatusactive == false && e.bitstatusdeactive != true
                                      && e.bitIsUnjoined != true && e.intcode == code && e.bitIsTransferred==false
                                     orderby e.dtDOJ descending
                                     select e).ToList();
                return View(authorisedemp);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //NOT USED for search date wise authorization
        //public ActionResult DateWiseAuthorised(FormCollection formdata)
        //{
        //    if (Session["descript"] != null)
        //    {
        //        if (formdata["dtraneguser"] != null)
        //        {
        //            int code = Convert.ToInt32(Session["id"].ToString());
        //            var dtrange = formdata["dtraneguser"].ToString();
        //            string NSdate = dtrange.Split(' ')[0];
        //            String[] Edate = dtrange.Split('-');
        //            string NEdate = string.Empty;
        //            foreach (String word in Edate)
        //            {
        //                NEdate = (word);

        //            }
        //            DateTime NewSdate1 = DateTime.ParseExact(NSdate.Trim().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //            DateTime NewEdate1 = DateTime.ParseExact(NEdate.Trim().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //            string fmdate = NewSdate1.ToString("dd/MM/yyyy");
        //            string todate = NewEdate1.ToString("dd/MM/yyyy");
        //            var FSdate = NewSdate1.ToString("yyyy-MM-dd HH:mm");
        //            var FEdate = NewEdate1.ToString("yyyy-MM-dd HH:mm");
        //            var authorisedemp = (from e in hrentity.tblEmpAssesmentMas
        //                                 where e.bitauthorised == true && e.bittempstatusactive == false && e.bitstatusdeactive != true
        //                                  && e.bitIsUnjoined != true && e.dtDOJ >= Convert.ToDateTime(fmdate) && e.dtDOJ <= Convert.ToDateTime(todate) && e.intcode == code
        //                                 orderby e.dtDOJ descending
        //                                 select e).ToList();
        //            if (authorisedemp != null)
        //            {
        //                TempData["SelectedDate"] = +authorisedemp.Count() + " employee from " + fmdate + " to" + todate;
        //                return View(authorisedemp);
        //            }
        //            else
        //            {
        //                TempData["Empty"] = "No record found in selected date!";
        //                return RedirectToAction("ViewAuthorisedEmp");
        //            }
        //        }
        //        else
        //        {
        //            TempData["NullDate"] = "Daterange should not be null please try again!";
        //            return RedirectToAction("ViewAuthorisedEmp");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }        
        //}           


        //for hr full details view when final update when employee autorised and update for final date of joining

        public ActionResult ViewHrFullDetails()
        {           
            return View();
        }

        #region Vp HR Actions
        public ActionResult ViewVPFullDetails(int id)
        {
            _PartialVPAction(id);
            return View();
        }

        public ActionResult _PartialVPAction(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var empmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
            PartialVPActionView objmodel = new PartialVPActionView();
            objmodel.id = empmas.intid;
            objmodel.salary = Convert.ToInt32(empmas.intsalary);
            return View(objmodel);            
        }

        [HttpPost]
        public ActionResult ViewVPFullDetails(PartialVPActionView obj)
        {
            if (Session["descript"] != null)
            {
                //Get session variables
                string username = Session["descript"].ToString();
                string ipused = Session["ipused"].ToString();
                string host = Session["hostname"].ToString();
                int userid = Convert.ToInt32(Session["usrid"].ToString());
                tblConversation objmessage = new tblConversation();
                //get selected VP Assign detail
                var getAssignment = (from e in hrentity.tblVPForward where e.fk_EmpID == obj.id select e).FirstOrDefault();
                if (obj.bitIsAuthorised == "Yes")
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int empid = Convert.ToInt32(obj.id);
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    selectedemp.bitIsAprvdVp = true;
                    getAssignment.dtApproved = DateTime.Now;
                    getAssignment.vchApprovedBy = username;

                    if (obj.vchRemarks != null)
                    {
                        objmessage.vchMsg = obj.vchRemarks;
                        objmessage.fk_intEmpid = empid;
                        objmessage.fk_uid = userid;
                        objmessage.fk_UserName = username;
                        objmessage.vchIpused = ipused;
                        objmessage.vchHost = host;
                        objmessage.dtMsgDate = DateTime.Now;
                        objmessage.intcode = code;
                        hrentity.tblConversation.Add(objmessage);
                        hrentity.SaveChanges();
                    }
                    else
                    {
                        selectedemp.vchauthorisedmsg = "Not Provided";
                    }
                    hrentity.SaveChanges();
                    int salaryrange = obj.salary;
                    var selecteduser = (from e in hrentity.tblUserAuthorize where e.intSalaryTo >= salaryrange select e).FirstOrDefault();
                    string name = selecteduser.vchUserName;
                    var mob = (from e in hrentity.tblUserMaster where e.vchUsername == name select e.vchMobile).FirstOrDefault();
                    if (mob != null)
                    {
                        string candidate = selectedemp.vchName.ToString();
                        var uri = "http://164.52.195.161/API/SendMsg.aspx?uname=20170839&pass=Indus2017&send=HOSPIN&dest=" + mob + "&msg=" + candidate + " Candidate authorization completed successfully please check your HRMS dashboard.Indus Healthcare Services Pvt. Ltd.&priority=1";
                        if (uri != null)
                        {
                            try
                            {
                                HttpWebRequest createrequest = (HttpWebRequest)WebRequest.Create(uri);
                                // getting response of sms
                                HttpWebResponse myResp = (HttpWebResponse)createrequest.GetResponse();
                                System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                                string responseString = _responseStreamReader.ReadToEnd();
                                _responseStreamReader.Close();
                                myResp.Close();
                            }
                            catch
                            {

                            }
                        }
                        TempData["Success"] = "Candidate approval submit successfully!";
                        return RedirectToAction("ViewAssignAuthorization", TempData["Success"]);
                    }
                    return RedirectToAction("ViewAssignAuthorization");
                }

                else if (obj.bitIsAuthorised == "No")
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int empid = Convert.ToInt32(obj.id);
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    selectedemp.bitIsAprvdVp = true;
                    getAssignment.dtApproved = DateTime.Now;
                    getAssignment.vchApprovedBy = username;
                    getAssignment.bitCancel = true;
                    if (obj.vchRemarks != null)
                    {
                        objmessage.vchMsg = obj.vchRemarks;
                        objmessage.fk_intEmpid = empid;
                        objmessage.fk_uid = userid;
                        objmessage.fk_UserName = username;
                        objmessage.vchIpused = ipused;
                        objmessage.vchHost = host;
                        objmessage.dtMsgDate = DateTime.Now;
                        objmessage.intcode = code;
                        hrentity.tblConversation.Add(objmessage);
                        hrentity.SaveChanges();
                    }
                    else
                    {
                        selectedemp.vchauthorisedmsg = "Not Provided";
                    }
                    hrentity.SaveChanges();
                    int salaryrange = obj.salary;
                    var selecteduser = (from e in hrentity.tblUserAuthorize where e.intSalaryTo >= salaryrange select e).FirstOrDefault();
                    string name = selecteduser.vchUserName;
                    var mob = (from e in hrentity.tblUserMaster where e.vchUsername == name select e.vchMobile).FirstOrDefault();
                    if (mob != null)
                    {
                        string candidate = selectedemp.vchName.ToString();
                        var uri = "http://164.52.195.161/API/SendMsg.aspx?uname=20170839&pass=Indus2017&send=HOSPIN&dest=" + mob + "&msg=" + candidate + " Candidate authorization completed successfully please check your HRMS dashboard.Indus Healthcare Services Pvt. Ltd.&priority=1";
                        if (uri != null)
                        {
                            try
                            {
                                HttpWebRequest createrequest = (HttpWebRequest)WebRequest.Create(uri);
                                // getting response of sms
                                HttpWebResponse myResp = (HttpWebResponse)createrequest.GetResponse();
                                System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                                string responseString = _responseStreamReader.ReadToEnd();
                                _responseStreamReader.Close();
                                myResp.Close();
                            }
                            catch
                            {

                            }
                        }
                        TempData["Success"] = "Candidate approval submit successfully!";
                        return RedirectToAction("ViewAssignAuthorization", TempData["Success"]);
                    }
                    return RedirectToAction("ViewAssignAuthorization");
                }
                else
                {
                    return RedirectToAction("ViewAssignAuthorization");
                }

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region auto generate employee final code after final authorization HR view
        public ActionResult FinalUpdateHr(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                EmpFinalUpdateViewModel objmodel = new EmpFinalUpdateViewModel();
                ViewBag.id = selectedemp.intid.ToString();
                ViewBag.tcode = selectedemp.vchEmpTcode;
                ViewBag.Edatejoin = selectedemp.dtDOJ.Value.ToString("dd/MM/yyyy");
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult FinalUpdateHr(EmpFinalUpdateViewModel objmas)
        {
            if (Session["descript"] != null)
            {
                EmpFinalUpdateViewModel newobjmodel = new EmpFinalUpdateViewModel();
                int code = Convert.ToInt32(Session["id"].ToString());
                int id = Convert.ToInt32(objmas.id);
                newobjmodel.id = id;
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                var getDptCode = (from e in hrentity.tblDeptMas where e.intid == selectedemp.fk_intdeptid select e).FirstOrDefault();
                var getCodeMas = (from e in hrentity.tblEmpCodeMas where e.intid == selectedemp.fk_intdeptid select e).FirstOrDefault();
                if (selectedemp != null)
                {
                    //check DOJ is equal or greater from Expected DOJ
                    DateTime edate = Convert.ToDateTime(selectedemp.dtDOJ);//.Value.ToString("dd/MM/yyyy");
                    if (objmas.dtDOJ >= edate)
                    {
                        //int DOJYEAR = Convert.ToInt32(objmas.dtDOJ.ToString("yyyy"));

                        //check final code are in used or not
                        if (objmas.bitIsCorporateemp == true)
                        {
                            int codeYear = 2023;
                            var getcode = (from e in hrentity.tblEmpCodeMas where e.intBranchCode == 0 && e.intJoinYear == codeYear select e).FirstOrDefault();
                            //Generate corporate code                            
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
                            DateTime today = DateTime.Now;
                            //string yearCode = today.ToString("yy");
                            string yearCode = objmas.dtDOJ.ToString("yy");
                            string finalCompleteCode = branchcode + "-" + yearCode + "-" + fdeptcode + "-" + finalnumber;
                            getcode.intCurrentCode = newcode;
                            selectedemp.vchEmpFcode = finalCompleteCode;
                            selectedemp.dtDOJ = objmas.dtDOJ;
                            selectedemp.bitIsCorporateemp = true;
                            selectedemp.bitIsUnitEmp = false;
                            selectedemp.vchFinalUpdatedBy = Session["descript"].ToString();
                            selectedemp.vchFinalHostname = Session["hostname"].ToString();
                            selectedemp.vchipdusedauthor = Session["ipused"].ToString();
                            selectedemp.dtFinalUpdated = DateTime.Now;
                            selectedemp.bittempstatusactive = true;
                            hrentity.SaveChanges();
                            TempData["Success"] = "Final Update saved successfully! System generated employee code is:" + finalCompleteCode;
                            return RedirectToAction("ViewAuthorisedEmp");
                        }
                        else if (objmas.bitIsCorporateemp == false)
                        {
                            //Generate Unit code
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
                            DateTime today = DateTime.Now;
                            //string yearCode = today.ToString("yy");
                            string yearCode = objmas.dtDOJ.ToString("yy");
                            string finalCompleteCode = branchcode + "-" + yearCode + "-" + fdeptcode + "-" + finalnumber;
                            getcode.intCurrentCode = newcode;
                            selectedemp.dtDOJ = objmas.dtDOJ;
                            selectedemp.bitIsCorporateemp = false;
                            selectedemp.bitIsUnitEmp = true;
                            selectedemp.vchEmpFcode = finalCompleteCode;
                            selectedemp.vchFinalUpdatedBy = Session["descript"].ToString();
                            selectedemp.vchFinalHostname = Session["hostname"].ToString();
                            selectedemp.vchipdusedauthor = Session["ipused"].ToString();
                            selectedemp.dtFinalUpdated = DateTime.Now;
                            selectedemp.bittempstatusactive = true;
                            hrentity.SaveChanges();
                            TempData["Success"] = "Final Update saved successfully! System generated employee code is :" + finalCompleteCode;
                            return RedirectToAction("ViewAuthorisedEmp");
                        }
                    }
                    else
                    {
                        //ModelState.AddModelError("doj", "DOJ should not be less than Expected DOJ");
                        TempData["Error"] = "DOJ should not be less than Expected DOJ";
                        ViewBag.Edatejoin = selectedemp.dtDOJ.Value.ToString("dd/MM/yyyy");
                        ViewBag.empid = selectedemp.intid.ToString();
                        ViewBag.tcode = selectedemp.vchEmpTcode.ToString();
                        // return RedirectToAction("FinalUpdateHr", new { id = selectedEmpid });
                        return View();
                    }
                }
                TempData["Error"] = "Model error generated please contact to administrator!";
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }


        }

        #endregion

        //new view work
        public ActionResult PartialEmpMas1(int id)
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

        //PartialEmpDetails1 for Personal details
        public ActionResult PartialEmpDetails1(int id)
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
                    ViewBag.Empty = "Contact details not found! Employee may be payroll employee/By Pass Employee";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PartialProfile1(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                if (id != 0)
                {
                    string isHrAdmin=string.Empty;
                    string isAuthorizer = string.Empty;
                    string isMainAdmin = string.Empty;
                    if (Session["HrAdmin"] != null)
                    {
                        isHrAdmin = Session["HrAdmin"].ToString();
                    }
                    if(Session["AllowAuthorization"] != null)
                    {
                        isAuthorizer = Session["AllowAuthorization"].ToString();
                    }
                    if (Session["MainAdmin"] != null)
                    {
                        isMainAdmin= Session["MainAdmin"].ToString();
                    }
                    var docdetails = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsProfilePic == true select e).FirstOrDefault();
                    var selectedimage = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    if (docdetails != null)
                    {
                        if (isHrAdmin != null && isMainAdmin != null && isMainAdmin != null)
                        {
                            if (isHrAdmin == "True" && isMainAdmin != "True" && selectedimage.bitgoauthor == true && selectedimage.bitAuthorBack == true && docdetails.BitIsProfilePic == true && selectedimage.bitauthorised == false && selectedimage.bitstatusdeactive == false)
                            {
                                ViewBag.ImgEdit = "Allowed";
                            }
                            else
                            {
                                ViewBag.ImgEdit = "Not-Allowed";
                            }
                        }
                        else
                        {
                            ViewBag.ImgEdit = "Not-Allowed";
                        }
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

        public ActionResult PartialDocComp1(int id)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                List<SelectListItem> mylist = new List<SelectListItem>();
                var compdoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompDoc==true  select e).ToList();
                if (compdoc != null)
                {
                    foreach (var per in compdoc)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = per.vchdocname.ToString(),
                            Value = per.vchfilename.ToString(),
                        };
                        mylist.Add(selectListItem);
                    }
                    ViewBag.CompDoc = mylist;
                }
                else
                {
                    ViewBag.Empty = "Compulsory document not found please chect it!";                   
                }               
                //get quali document
                List<SelectListItem> QualiDocList = new List<SelectListItem>();
                var qualidoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompQuali == true select e).ToList();
                if (qualidoc != null)
                {
                    foreach (var per in qualidoc)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = per.vchdocname.ToString(),
                            Value = per.vchfilename.ToString(),
                        };
                        QualiDocList.Add(selectListItem);
                    }
                    ViewBag.QualiDoc = QualiDocList;
                }                
                else
                {
                    ViewBag.Empty = "Qualification document not found please chect it!";
                    return View();
                }
                //Get all other document
                List<SelectListItem> OtherDocList = new List<SelectListItem>();
                var OtherDoc = (from e in hrentity.tblOtherDocDetail where e.fk_empAssid == id select e).ToList();
                if (OtherDoc != null)
                {
                    foreach (var per in OtherDoc)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = per.vchDocName.ToString(),
                            Value = per.vchfilename.ToString(),
                        };
                        OtherDocList.Add(selectListItem);
                    }
                    ViewBag.OtherDoc = OtherDocList;
                }
                else
                {
                    ViewBag.Empty = "The other official document has not been uploaded!";
                }
                return View();

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult UpdateProfile(int id)
        {
            if (id != 0)
            {
                return RedirectToAction("viewProfile", "Assesment", new { id = id });
            }
            else
            {
                return View();
            }
        }

        public ActionResult FlaggingEMp(int id)
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
                    TempData["Empty"] = "Flagging detail not found!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //CancelCandidate those candidate who is failed in provession period and any other reason dint join
        public ActionResult CancelCandidate(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selected = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    if (selected != null)
                    {
                        string username = Session["descript"].ToString();
                        string ipused = Session["ipused"].ToString();
                        string host = Session["hostname"].ToString();
                        int userid = Convert.ToInt32(Session["usrid"].ToString());
                        selected.dtUnjoined = DateTime.Now;
                        selected.vchUnjoinBy = username;
                        selected.bitIsUnjoined = true;
                        hrentity.SaveChanges();
                        TempData["Success"] = "Candidate successfully  sent out from the hrms dashboard!";
                        return RedirectToAction("ViewAuthorisedEmp");
                    }
                    else
                    {
                        TempData["Error"] = "Candidate not found in database!";
                        return RedirectToAction("ViewAuthorisedEmp");
                    }
                }
                else
                {
                    TempData["Error"] = "Candidate id not selected, please try again!";
                    return RedirectToAction("ViewAuthorisedEmp");
                }

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #region Generate existing employee code or HR view
        public ActionResult GenCodeExisting()
        {
            if (Session["descript"] != null)
            {
                GenExistingEmpCodeModel objmodel = new GenExistingEmpCodeModel();
                int code = Convert.ToInt32(Session["id"].ToString());
                List<SelectListItem> EmployeeList = new List<SelectListItem>();
                var emplist = (from e in hrentity.tblEmpAssesmentMas
                               where e.bitIsCorporateemp == null && e.bitIsUnitEmp == null
                               && e.bittempstatusactive == true && e.intcode == code
                               orderby e.vchName ascending
                               select e).ToList();
                if (emplist.Count() != 0)
                {
                    EmployeeList.Add(new SelectListItem { Text = "Select employee", Value = "0" });
                    foreach (var emp in emplist)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Text = emp.vchName,
                            Value = emp.intid.ToString()
                            //Selected=pos
                        };
                        EmployeeList.Add(selectListItem);
                    }
                }

                objmodel.AllEmployee = new SelectList(emplist, "Text", "Value");
                ViewBag.EmpList = new SelectList(EmployeeList, "Text", "Value");
                return View(objmodel);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpGet]
        public ActionResult GetEmpData(string EmployeeId, string EmployeeName)
        {
            if (User.Identity.Name != null)
            {
                if (EmployeeId != null)
                {
                    int empid = Convert.ToInt32(EmployeeId);
                    int code = Convert.ToInt32(Session["id"].ToString());
                    var selectedemp = (from e in hrentity.spGetEmpData(empid, code) select e).ToList();
                    if (selectedemp != null)
                    {
                        var result = (from f in selectedemp
                                      select new
                                      {
                                          empid = f.intid,
                                          empname = f.vchname,
                                          fathername = f.vchFatherName,
                                          dtdoj = f.dtdoj,
                                          depart = f.depart,
                                          desi = f.desi
                                      }).ToList();
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var output1 = "Emplyee details not found in database!";
                        return Json(new { success = 1, output1, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    var output2 = "Emplyee ID should not be null please contact to administrator!";
                    return Json(new { success = 2, output2, JsonRequestBehavior.AllowGet });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public JsonResult GenCodeExisting(string employeeid, string iscop)
        {
            if (User.Identity.Name != null)
            {
                if (employeeid != null && iscop != null)
                {
                    //get company code
                    int code = Convert.ToInt32(Session["id"].ToString());
                    //get employee code
                    int id = Convert.ToInt32(employeeid);
                    //get employee details
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    //get employee department
                    var selecetddeprt = (from e in hrentity.tblDeptMas where e.intid == selectedemp.fk_intdeptid select e).FirstOrDefault();
                    DateTime DOJ = Convert.ToDateTime(selectedemp.dtDOJ);
                    if (DOJ != null)
                    {
                        string year = DOJ.ToString("yyyy");
                        string finalyear = DOJ.ToString("yy");
                        //check is corporate employee
                        if (iscop == "Yes")
                        {
                            int Joinyear = Convert.ToInt16(year.ToString());
                            //Generate corporate code
                            var getcode = (from e in hrentity.tblEmpCodeMas where e.intBranchCode == 0 && e.intJoinYear == Joinyear select e).FirstOrDefault();
                            int currentcode = getcode.intCurrentCode;
                            int newcode = currentcode + 1;
                            int number = newcode;
                            int counter = 0;
                            string finalnumber = "";
                            int fnumber = 0;
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
                                fnumber = newcode;
                            }
                            //Get Dept Code.
                            string fdeptcode = selecetddeprt.vchdepCode.ToString();
                            //Get Branch Code.
                            string branchcode = getcode.vchUnitCode.ToString();
                            string finalCompleteCode = branchcode + "-" + finalyear + "-" + fdeptcode + "-" + finalnumber;
                            getcode.intCurrentCode = newcode;
                            selectedemp.vchEmpOldCode = selectedemp.vchEmpFcode.ToString();
                            selectedemp.vchEmpFcode = finalCompleteCode;
                            selectedemp.bitIsCorporateemp = true;
                            selectedemp.bitIsUnitEmp = false;
                            selectedemp.vchFinalUpdatedBy = Session["descript"].ToString();
                            selectedemp.vchFinalHostname = Session["hostname"].ToString();
                            selectedemp.vchipdusedauthor = Session["ipused"].ToString();
                            selectedemp.dtFinalUpdated = DateTime.Now;
                            hrentity.SaveChanges();
                            var output1 = "Employee code " + finalCompleteCode + " generated successfully!";
                            return Json(new { success = 1, output1, JsonRequestBehavior.AllowGet });
                        }
                        else if (iscop == "No")
                        {
                            int intJyear = Convert.ToInt16(year.ToString());
                            var getcode = (from e in hrentity.tblEmpCodeMas where e.intBranchCode == code && e.intJoinYear == intJyear select e).FirstOrDefault();
                            int currentcode = getcode.intCurrentCode;
                            int newcode = currentcode + 1;
                            int number = newcode;
                            int counter = 0;
                            string finalnumber = "";
                            int fnumber = 0;
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
                                fnumber = newcode;
                            }
                            //Get Dept Code.
                            string fdeptcode = selecetddeprt.vchdepCode.ToString();
                            //Get Branch Code.
                            string branchcode = getcode.vchUnitCode.ToString();
                            string finalCompleteCode = branchcode + "-" + finalyear + "-" + fdeptcode + "-" + finalnumber;
                            getcode.intCurrentCode = newcode;
                            selectedemp.vchEmpOldCode = selectedemp.vchEmpFcode.ToString();
                            selectedemp.vchEmpFcode = finalCompleteCode;
                            selectedemp.bitIsCorporateemp = false;
                            selectedemp.bitIsUnitEmp = true;
                            selectedemp.vchFinalUpdatedBy = Session["descript"].ToString();
                            selectedemp.vchFinalHostname = Session["hostname"].ToString();
                            selectedemp.vchipdusedauthor = Session["ipused"].ToString();
                            selectedemp.dtFinalUpdated = DateTime.Now;
                            hrentity.SaveChanges();
                            var output5 = "Employee code " + finalCompleteCode + " generated successfully!";
                            return Json(new { success = 5, output5, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            var output4 = "Please select employee type!";
                            return Json(new { success = 4, output4, JsonRequestBehavior.AllowGet });
                        }
                    }
                    else
                    {
                        var output6 = "Please select employee type!";
                        return Json(new { success = 6, output6, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    var output2 = "Employee id should not be null!";
                    return Json(new { success = 2, output2, JsonRequestBehavior.AllowGet });
                }
            }
            else
            {
                var output3 = "Session error generated please login and try again!";
                return Json(new { success = 3, output3, JsonRequestBehavior.AllowGet });
            }
        }

        #endregion

        #region Enter existing payroll employee for new code generation
        public ActionResult PayrollEmployee()
        {
            if (Session["id"] != null)
            {
                //Get payroll employee company wise          
                int code = Convert.ToInt16(Session["id"].ToString());
                var selectedlist = (from e in hrentity.tblPayrollData where e.intCode == code && e.BitInHRMS == false orderby e.Name descending select e).Take(1500);

                if (selectedlist.Count() != 0)
                {
                    return View(selectedlist);
                }
                else
                {
                    TempData["Empty"] = "Payroll database is empty!";
                    return View(selectedlist);
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }

        }

        public ActionResult PayrollDetailEmp(string id)
        {
            if (Session["id"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                if (id != null)
                {
                    PayrollEmpModelView objmodel = new PayrollEmpModelView();
                    var selectedemp = (from e in hrentity.tblPayrollData where e.vchEMpCode == id select e).FirstOrDefault();
                    if (selectedemp != null)
                    {
                        //check mobile in use or not
                        if (selectedemp.vchMobile != null)
                        {
                            var mobiledata = (from e in hrentity.tblEmpAssesmentMas where e.vchMobile == selectedemp.vchMobile && e.bittempstatusactive == true select e).FirstOrDefault();
                            if (mobiledata != null)
                            {
                                // selectedemp.BitInHRMS = true;
                                // hrentity.SaveChanges();
                                TempData["Error"] = "Mobile number already using in HRMS active employee database,data updated now, this employee will be disappear in payroll list.";
                                return RedirectToAction("PayrollEmployee");
                            }
                        }
                        //Check employee code in use or not
                        if (selectedemp.vchEMpCode != null)
                        {
                            //get 13 charachter of payroll emp code
                            string comparestring = string.Empty;
                            if (selectedemp.vchEMpCode.Length > 13)
                            {
                                string getcode = selectedemp.vchEMpCode.ToString();
                                comparestring = getcode.Substring(0, 13);
                            }
                            else
                            {
                                comparestring = selectedemp.vchEMpCode.ToString();
                            }
                            var codedata = (from e in hrentity.tblEmpAssesmentMas
                                            where (e.vchEmpFcode == comparestring || e.vchEmpOldCode == comparestring) && e.bittempstatusactive == true
                                            select e).FirstOrDefault();
                            if (codedata != null)
                            {
                                codedata.isPayrollEmployee = true;
                                selectedemp.BitInHRMS = true;
                                hrentity.SaveChanges();
                                TempData["Error"] = "Employee code is already used in HRMS, data updated now, this employee will be disappear in payroll list.";
                                return RedirectToAction("PayrollEmployee");
                            }
                        }
                        //objmodel.string_id = selectedemp.intid.ToString();
                        objmodel.Name = selectedemp.Name;
                        string date1 = Convert.ToDateTime(selectedemp.dtDOJ).ToString("dd/MMM/yyyy");
                        objmodel.DOJ = date1;
                        objmodel.vchMobile = selectedemp.vchMobile;
                        objmodel.vchOldCode = selectedemp.vchEMpCode;
                        objmodel.vchgender = selectedemp.vchGender;
                        objmodel.AadharNo = selectedemp.AadhaarNo;
                    }
                    //Select branch
                    List<SelectListItem> Branch = new List<SelectListItem>
                             {
                                     new SelectListItem{Text = "Select Branch",Value = "0" },
                                     new SelectListItem { Text="Indus Hospital", Value="4"},
                                     new SelectListItem { Text="Indus Super Speciality Hospital", Value="3"},
                                     new SelectListItem{ Text="Indus Hygiea",Value="2"},
                                     new SelectListItem {Text= "Indus International Hospital", Value="14" },
                                     new SelectListItem{ Text="Indus Fatehgarh Sahib Hospital",Value="15"},
                                     new SelectListItem{ Text="KS Homecare",Value="16"},
                                     new SelectListItem{ Text="Mehndiratta Hospital",Value="21"},
                                     new SelectListItem{ Text="Healthsure Hospital",Value="22"},
                                     new SelectListItem{ Text="My Hospital",Value="23"},
                                     new SelectListItem{ Text="KS Pharma Solutions",Value="24"},
                                     new SelectListItem{ Text="Bharat Heart and Super Specialty Hospital",Value="25"},
                    };
                    ViewBag.Branch = new SelectList(Branch, "Value", "Text");
                    //Select all department master
                    var deptlist = (from e in hrentity.tblDeptMas select e).ToList();
                    List<SelectListItem> newlist = new List<SelectListItem>();
                    //newlist.Add(new SelectListItem { Text = "Select department", Value = "0" });
                    foreach (var dpt in deptlist)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = dpt.intid.ToString(),
                            Text = dpt.vchdeptname
                        };
                        newlist.Add(selectListItem);
                    }
                    ViewBag.DeptList = new SelectList(newlist, "Value", "Text");
                    //Select all position master
                    var position = (from e in hrentity.tblPositionCategoryMas select e).ToList();
                    List<SelectListItem> poslist = new List<SelectListItem>();
                    //poslist.Add(new SelectListItem { Text = "Select position", Value = "0" });
                    foreach (var poj in position)
                    {
                        SelectListItem selectListItem = new SelectListItem
                        {
                            Value = poj.intid.ToString(),
                            Text = poj.vchPosCatName
                        };
                        poslist.Add(selectListItem);
                    }
                    ViewBag.PosList = new SelectList(poslist, "Value", "Text");
                    //FOr Title selection
                    IEnumerable<tblTitleMas> selecttit = (from e in hrentity.tblTitleMas select e).ToList();
                    List<SelectListItem> Title = new List<SelectListItem>();
                    foreach (var tit in selecttit)
                    {
                        SelectListItem selectListItem = new SelectListItem()
                        {
                            Value = tit.intid.ToString(),
                            Text = tit.vchname.ToString()
                        };
                        Title.Add(selectListItem);
                    }
                    ViewBag.Title2 = new SelectList(Title, "Value", "Text");

                    return View(objmodel);
                }
                else
                {
                    TempData["Error"] = "Employee code should not be empty!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpGet]
        public ActionResult DesiListSelect(string dept_id)
        {
            hrentity = new HRMEntities();
            int code = Convert.ToInt32(Session["id"].ToString());
            int deptid = Convert.ToInt32(dept_id);
            List<tblDesignationMas> desilist = new List<tblDesignationMas>();
            desilist = (from e in hrentity.tblDesignationMas where e.intdeptid == deptid select e).ToList();

            var result = (from d in desilist
                          select new
                          {
                              id = d.intid,
                              designation = d.vchdesignation
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //public JsonResult SaveEmp(string titleid, string position, string depttid, string desiiid, string Fempname, string DOJ, string Foldcode, string Fgender, string Fmobile,string Empco) //, string fathername, string mothername, string spouseName)  //PayrollEmpModelView objdata, tblDeptMas deptobj, tblDesignationMas desiobj)
        public ActionResult PayrollDetailEmp(PayrollEmpModelView objmodel, FormCollection fc)
        {

            if (Session["id"] != null)
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                tblEmpAssesmentMas objmas = new tblEmpAssesmentMas();
                int code = Convert.ToInt32(Session["id"].ToString());
                int deptid = Convert.ToInt16(objmodel.fk_deptid);
                int desiid = Convert.ToInt32(fc.Get("selecteddesi"));
                if (ModelState.IsValid)
                {
                    var selecteddept = (from e in hrentity.tblDeptMas where e.intid == deptid select e).FirstOrDefault();
                    if (objmodel.fkBranch != 0)
                    {
                        objmas.intcode = objmodel.fkBranch;
                    }
                    if (objmodel.fk_titid != 0)
                    {
                        objmas.fk_inttitid = objmodel.fk_titid;
                    }
                    if (selecteddept != null)
                    {
                        objmas.fk_intdeptid = selecteddept.intid;
                    }
                    if (objmodel.fk_position != 0)
                    {
                        objmas.fk_PositionId = objmodel.fk_position;
                    }
                    if (desiid != 0)
                    {
                        objmas.fk_intdesiid = desiid;
                    }
                    if (objmodel.Name != null)
                    {
                        objmas.vchName = objmodel.Name;
                    }
                    if (objmodel.DOJ != null)
                    {
                        objmas.dtDOJ = Convert.ToDateTime(objmodel.DOJ);
                    }

                    if (objmodel.vchOldCode != null)
                    {
                        objmas.vchEmpOldCode = objmodel.vchOldCode.ToString();
                    }
                    if (objmodel.vchOldCode != null)
                    {
                        //update entred payroll emp in hrms                                           
                        var selectedpayrol = (from e in hrentity.tblPayrollData where e.vchEMpCode == objmodel.vchOldCode select e).FirstOrDefault();
                        selectedpayrol.BitInHRMS = true;
                    }
                    if (objmodel.vchMobile != null)
                    {
                        objmas.vchMobile = objmodel.vchMobile;
                    }
                    if (objmodel.vchgender != null)
                    {
                        objmas.vchgender = objmodel.vchgender;
                    }
                    DateTime DOJ1 = Convert.ToDateTime(objmodel.DOJ);
                    //for all emp code continue series 
                    int year =2023;
                    string finalyear = DOJ1.ToString("yy");
                    if (objmodel.bitIsCorporateemp == true)
                    {                        
                        //Generate corporate code
                        var getcode = (from e in hrentity.tblEmpCodeMas where e.intBranchCode == 0 && e.intJoinYear == year select e).FirstOrDefault();
                        int currentcode = getcode.intCurrentCode;
                        int newcode = currentcode + 1;
                        int number = newcode;
                        int counter = 0;
                        string finalnumber = "";
                        int fnumber = 0;
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
                            fnumber = newcode;
                        }
                        //Get Dept Code.
                        string fdeptcode = selecteddept.vchdepCode.ToString();
                        //Get Branch Code.
                        string branchcode = getcode.vchUnitCode.ToString();
                        string finalCompleteCode = branchcode + "-" + finalyear + "-" + fdeptcode + "-" + finalnumber;
                        //update current code
                        getcode.intCurrentCode = newcode;
                        //add mas table entry
                        objmas.vchEmpFcode = finalCompleteCode;
                        objmas.bitIsCorporateemp = true;
                        objmas.vchAadharNo = objmodel.AadharNo;
                    }
                    if (objmodel.bitIsCorporateemp == false)
                    {
                        int intJyear = Convert.ToInt16(year.ToString());
                        var getcode = (from e in hrentity.tblEmpCodeMas where e.intBranchCode == code && e.intJoinYear == year select e).FirstOrDefault();
                        int currentcode = getcode.intCurrentCode;
                        int newcode = currentcode + 1;
                        int number = newcode;
                        int counter = 0;
                        string finalnumber = "";
                        int fnumber = 0;
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
                            fnumber = newcode;
                        }
                        //Get Dept Code.
                        string fdeptcode = selecteddept.vchdepCode.ToString();
                        //Get Branch Code.
                        string branchcode = getcode.vchUnitCode.ToString();
                        string finalCompleteCode = branchcode + "-" + finalyear + "-" + fdeptcode + "-" + finalnumber;
                        //update getcode current code
                        getcode.intCurrentCode = newcode;
                        //add mas table entry
                        objmas.vchEmpFcode = finalCompleteCode;
                        objmas.bitIsUnitEmp = true;
                        objmas.vchAadharNo = objmodel.AadharNo;
                    }
                    objmas.decExperience = 0;
                    objmas.bitIsReplacement = false;
                    objmas.vchRplcmntName = "Payroll Emp";
                    objmas.vchWorkedArea = "Payroll Emp";
                    objmas.bittempstatusactive = true;
                    objmas.intsalary = Convert.ToInt32(objmodel.salary);
                    objmas.isPayrollEmployee = true;
                    objmas.dtcreated = DateTime.Now;
                    objmas.vchcreatedby = Session["descript"].ToString();
                    objmas.vchcreatedhost = Session["hostname"].ToString();
                    objmas.vchcreatedipused = Session["ipused"].ToString();
                    //select branch code from user selection
                    objmas.intcode = code;
                    objmas.intyr = Convert.ToInt32(Session["yr"].ToString());                    
                    hrentity.tblEmpAssesmentMas.Add(objmas);
                    hrentity.SaveChanges();
                    //var output1 = "Employee saved successfully in HRMS database!";
                    //return Json(new { success = 1, output1, JsonRequestBehavior.AllowGet });
                    TempData["Success"] = "Employee saved successfully, check in active employee list";
                    return RedirectToAction("PayrollEmployee");
                }
                else
                {
                    return View();
                    //var output8 = "Session expired, login again!";
                    //return Json(new { success = 8, output8, JsonRequestBehavior.AllowGet });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //Get All dept
        public ActionResult _getdeptlist()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblDeptMas> deptlist = hrentity.tblDeptMas.ToList();
            ViewBag.deptlist = new SelectList(deptlist, "intid", "vchdeptname");
            return View();
        }

        //get all designation
        public ActionResult _getdesilist()
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblDesignationMas> desilist = hrentity.tblDesignationMas.ToList();
            ViewBag.desilist = new SelectList(desilist, "intid", "vchdesignation");
            return View();
        }

        //enter aadhaar number
        public ActionResult AadhaarNo(int id)
        {
            if (Session["descript"] != null)
            {
                 UpAadhaarNo objupdate = new UpAadhaarNo();
                int code = Convert.ToInt32(Session["id"].ToString());
                var selected = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                objupdate.empid = selected.intid;
                objupdate.name = selected.vchName;
                return View(objupdate);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult AadhaarNo(UpAadhaarNo objmodel)
        {
            if (Session["descript"] != null)
            {
                var selecetdEmp = (from e in hrentity.tblEmpAssesmentMas where e.intid == objmodel.empid select e).FirstOrDefault();
                if (selecetdEmp != null)
                {
                    //check aadhaar is in used
                    var checkcard = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.aadharno select e).ToList();
                    if (checkcard.Count() != 0)
                    {
                        int id = objmodel.empid;
                        foreach (var card in checkcard)
                        {
                            //check active or deactive employee
                            if (card.bittempstatusactive == true)
                            {
                                ViewBag.Error = "Entered adhar number is already in use in active employee list";
                                return View(objmodel);
                            }
                            if (card.bitstatusdeactive == true)
                            {
                                if (card.BitIsFlaggingEmp == true)
                                {
                                    //check card flag
                                    if (card.BitIsGreenFlagging == true)
                                    {
                                        ViewBag.Error = "Entered adhar number is already in use and it is an green flag card";
                                    }
                                    if (card.BitIsOrangeFlagging == true)
                                    {
                                        ViewBag.Error = "Entered adhar number is already in use and it is an orange flag card";
                                    }
                                    if (card.BitIsRedFlagging == true)
                                    {
                                        ViewBag.Error = "Entered adhar number is already in use and it is an red flag card";
                                    }
                                    return View(objmodel);
                                }
                            }                           
                        }
                        ViewBag.Error = "Aadhar number is already in use!";
                        return View(objmodel);
                    }
                    else
                    {
                        selecetdEmp.vchAadharNo = objmodel.aadharno;
                        hrentity.SaveChanges();
                        TempData["Success"] = "Aadhar number saved successfully";
                        return RedirectToAction("ViewHrFullDetails", new { id = objmodel.empid });
                    }
                }
                else
                {
                    TempData["Error"] = "Selected employee details not found, please try again";
                    return RedirectToAction("ViewHrFullDetails", new { id = objmodel.empid });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
        #endregion

        #region By pass entry for security and Houskeeping department employee     

        public ActionResult IndexPayroll()
        {
            if (Session["id"] != null)
            {
                // select all position
                var allposii = (from e in hrentity.tblPositionCategoryMas
                                where e.BitDesiMapping == true
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
                //Select all department master
                var deptlist = (from e in hrentity.tblDeptMas where e.bitIsByPassDept == true select e).ToList();
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
                //FOr Title selection
                IEnumerable<tblTitleMas> selecttit = (from e in hrentity.tblTitleMas select e).ToList();
                List<SelectListItem> Title = new List<SelectListItem>();
                foreach (var tit in selecttit)
                {
                    SelectListItem selectListItem = new SelectListItem()
                    {
                        Value = tit.intid.ToString(),
                        Text = tit.vchname.ToString()
                    };
                    Title.Add(selectListItem);
                }
                ViewBag.Title1 = new SelectList(Title, "Value", "Text");
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public ActionResult IndexPayroll(ByPassEntryEmpModel objmodel, FormCollection fc)
        {
            if (Session["id"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int desiid = Convert.ToInt32(fc.Get("selecteddesi"));
                var selecteddesi = (from e in hrentity.tblDesignationMas where e.intid == desiid select e).FirstOrDefault();
                var selecteposi = (from e in hrentity.tblPositionCategoryMas where e.intid == objmodel.fk_positionid select e).FirstOrDefault();
                int deptID = objmodel.fk_deptid;
                //get dept mapping detail
                var GetCounter = (from e in hrentity.tblManPowerDetail where e.intCode == code && e.fk_deptid == deptID select e).FirstOrDefault();
                int ActiveDeptCount = 0;
                int DeptCounter = 0;
                if (GetCounter != null)
                {
                    DeptCounter = Convert.ToInt32(GetCounter.intManPowerCount);
                    if (DeptCounter != 0)
                    {
                        var GetActiveEmpCount = (from e in hrentity.tblEmpAssesmentMas where (e.bittempstatusactive == true || e.bitIsPartialAuthorised == true) && e.bitstatusdeactive != true && e.bitIsLeft != true && e.bitIsUnjoined!=true && e.fk_intdeptid == deptID && e.intcode == code select e).ToList();
                        if (GetActiveEmpCount.Count() != 0)
                        {
                            ActiveDeptCount = GetActiveEmpCount.Count();
                        }
                    }
                    else
                    {


                        //aadhaar number validation
                        tblEmpAssesmentMas objtable = new tblEmpAssesmentMas();
                        tblEmpDetails objdetail = new tblEmpDetails();
                        if (objmodel != null)
                        {
                            var checkAdharRedFlag = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.aadhaarno && e.BitIsFlaggingEmp == true select e).FirstOrDefault();
                            var checkAadhaar = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.aadhaarno && e.bittempstatusactive == true select e).FirstOrDefault();
                            var CheckOnlyAadhar = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.aadhaarno && e.BitStatus == false select e).FirstOrDefault();
                            var CheckAadharAssess = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.aadhaarno && e.BitStatus == true select e).FirstOrDefault();
                            //Check for Flagging
                            if (checkAdharRedFlag != null && checkAdharRedFlag.BitIsFlaggingEmp == true)
                            {

                                //for red flag
                                if (checkAdharRedFlag.BitIsRedFlagging == true)
                                {
                                    TempData["error"] = "Entered aadhaar number is red flag so you cannot hire/re-join this candidate!";
                                    return RedirectToAction("IndexPayroll");
                                }
                                //for orange flag
                                else if (checkAdharRedFlag.BitIsOrangeFlagging == true)
                                {
                                    DateTime aajkidate = DateTime.Now;
                                    var leftdatetime = checkAdharRedFlag.dtDOL;
                                    DateTime newjoindate = leftdatetime.Value.AddMonths(6);
                                    DateTime doj = objmodel.dtDOJ;
                                    if (doj > newjoindate)
                                    {
                                        if (objmodel.bitIsReplacement == true)
                                        {
                                            objtable.bitIsReplacement = true;
                                            objtable.vchRplcmntName = objmodel.vchRplcmntName;
                                        }
                                        else
                                        {
                                            objtable.vchRplcmntName = "N/A";
                                        }
                                        objtable.fk_PositionId = selecteposi.intid;
                                        objtable.fk_intdesiid = desiid;
                                        objtable.fk_intdeptid = objmodel.fk_deptid;
                                        if (objmodel.vchgender == "1")
                                        {
                                            objtable.fk_inttitid = 1;
                                        }
                                        else
                                        {
                                            objtable.fk_inttitid = 7;
                                        }
                                        objtable.vchName = objmodel.vchname;
                                        objtable.decExperience = objmodel.experience;
                                        objtable.vchWorkedArea = objmodel.vchworkedarea;
                                        objtable.dtDOJ = objmodel.dtDOJ;
                                        objtable.vchMobile = objmodel.vchmobile;
                                        objtable.vchArea = objmodel.vchworkedarea;
                                        objtable.vchAadharNo = objmodel.aadhaarno;
                                        objtable.intcode = code;
                                        objtable.vchcreatedby = Session["descript"].ToString();
                                        objtable.vchFinalUpdatedBy = Session["descript"].ToString();
                                        objtable.dtcreated = DateTime.Today;
                                        objtable.dtFinalUpdated = DateTime.Today;
                                        objtable.vchcreatedhost = Session["hostname"].ToString();
                                        objtable.vchFinalHostname = Session["hostname"].ToString();
                                        objtable.vchcreatedipused = Session["ipused"].ToString();
                                        objtable.vchFinalipused = Session["ipused"].ToString();
                                        objtable.intsalary = objmodel.intsalary;
                                        hrentity.tblEmpAssesmentMas.Add(objtable);
                                        hrentity.SaveChanges();
                                    }
                                    else
                                    {
                                        string joiningdate = newjoindate.ToString("dd/MM/yyyy");
                                        TempData["error"] = "Entered aadhaar number is orange flag so you can re-join this candidate after " + joiningdate + "date";
                                        return RedirectToAction("IndexPayroll");
                                    }
                                }
                                else if (checkAdharRedFlag.BitIsGreenFlagging == true)
                                {
                                    DateTime aajkidate = DateTime.Now;
                                    var leftdatetime = checkAdharRedFlag.dtDOL;
                                    DateTime newjoindate = leftdatetime.Value.AddMonths(3);
                                    DateTime doj = objmodel.dtDOJ;
                                    if (doj > newjoindate)
                                    {
                                        if (objmodel.bitIsReplacement == true)
                                        {
                                            objtable.bitIsReplacement = true;
                                            objtable.vchRplcmntName = objmodel.vchRplcmntName;
                                        }
                                        else
                                        {
                                            objtable.vchRplcmntName = "N/A";
                                        }
                                        objtable.fk_PositionId = selecteposi.intid;
                                        objtable.fk_intdesiid = desiid;
                                        objtable.fk_intdeptid = objmodel.fk_deptid;
                                        if (objmodel.vchgender == "1")
                                        {
                                            objtable.fk_inttitid = 1;
                                        }
                                        else
                                        {
                                            objtable.fk_inttitid = 7;
                                        }
                                        objtable.vchName = objmodel.vchname;
                                        objtable.decExperience = objmodel.experience;
                                        objtable.vchWorkedArea = objmodel.vchworkedarea;
                                        objtable.dtDOJ = objmodel.dtDOJ;
                                        objtable.vchMobile = objmodel.vchmobile;
                                        objtable.vchArea = objmodel.vchworkedarea;
                                        objtable.vchAadharNo = objmodel.aadhaarno;
                                        objtable.intcode = code;
                                        objtable.vchcreatedby = Session["descript"].ToString();
                                        objtable.vchFinalUpdatedBy = Session["descript"].ToString();
                                        objtable.dtcreated = DateTime.Today;
                                        objtable.dtFinalUpdated = DateTime.Today;
                                        objtable.vchcreatedhost = Session["hostname"].ToString();
                                        objtable.vchFinalHostname = Session["hostname"].ToString();
                                        objtable.vchcreatedipused = Session["ipused"].ToString();
                                        objtable.vchFinalipused = Session["ipused"].ToString();
                                        objtable.intsalary = objmodel.intsalary;
                                        hrentity.tblEmpAssesmentMas.Add(objtable);
                                        hrentity.SaveChanges();
                                    }
                                    else
                                    {
                                        string joiningdate = newjoindate.ToString("dd/MM/yyyy");
                                        TempData["error"] = "Entered aadhaar number is green flag so you can re-join this candidate after " + joiningdate + "date";
                                        return RedirectToAction("IndexPayroll");
                                    }
                                }
                            }
                            //check is active aadhaar card
                            else if (checkAadhaar != null)
                            {
                                TempData["error"] = "Entered aadhaar number is already in use active employee";
                                return RedirectToAction("IndexPayroll");
                            }
                            else if (CheckOnlyAadhar != null)
                            {
                                TempData["error"] = "Entered aadhaar number is already in use new employee assessment";
                                return RedirectToAction("IndexPayroll");
                            }
                            else if (CheckAadharAssess != null)
                            {
                                TempData["error"] = "Entered aadhaar number is already in use employee completed assessment";
                                return RedirectToAction("IndexPayroll");
                            }
                            else
                            {
                                try
                                {
                                    if (objmodel.bitIsReplacement == true)
                                    {
                                        objtable.bitIsReplacement = true;
                                        objtable.vchRplcmntName = objmodel.vchRplcmntName;
                                    }
                                    else
                                    {
                                        objtable.vchRplcmntName = "N/A";
                                    }
                                    objtable.fk_PositionId = selecteposi.intid;
                                    objtable.fk_intdesiid = desiid;
                                    objtable.fk_intdeptid = objmodel.fk_deptid;
                                    if (objmodel.vchgender == "1")
                                    {
                                        objtable.fk_inttitid = 1;
                                    }
                                    else
                                    {
                                        objtable.fk_inttitid = 7;
                                    }
                                    objtable.vchName = objmodel.vchname;
                                    objtable.decExperience = objmodel.experience;
                                    objtable.vchWorkedArea = objmodel.vchworkedarea;
                                    objtable.dtDOJ = objmodel.dtDOJ;
                                    objtable.vchMobile = objmodel.vchmobile;
                                    objtable.vchArea = objmodel.vchworkedarea;
                                    objtable.vchAadharNo = objmodel.aadhaarno;
                                    objtable.intcode = code;
                                    objtable.vchcreatedby = Session["descript"].ToString();
                                    objtable.vchFinalUpdatedBy = Session["descript"].ToString();
                                    objtable.dtcreated = DateTime.Today;
                                    objtable.dtFinalUpdated = DateTime.Today;
                                    objtable.vchcreatedhost = Session["hostname"].ToString();
                                    objtable.vchFinalHostname = Session["hostname"].ToString();
                                    objtable.vchcreatedipused = Session["ipused"].ToString();
                                    objtable.vchFinalipused = Session["ipused"].ToString();
                                    objtable.intsalary = objmodel.intsalary;
                                    hrentity.tblEmpAssesmentMas.Add(objtable);
                                    hrentity.SaveChanges();
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
                                    return RedirectToAction("IndexPayroll");
                                }
                            }
                            var selectedmasdata = (from e in hrentity.tblEmpAssesmentMas where e.vchName == objmodel.vchname && e.vchMobile == objmodel.vchmobile select e).FirstOrDefault();
                            if (selectedmasdata != null)
                            {
                                var selecteddept = (from e in hrentity.tblDeptMas where e.intid == objmodel.fk_deptid select e).FirstOrDefault();
                                objdetail.fk_intempid = selectedmasdata.intid;
                                objdetail.vchNominee = objmodel.vchNominee;
                                objdetail.vchRelation = objmodel.vchNomRelation;
                                objdetail.intage = objmodel.age;
                                objdetail.fk_titid = objmodel.fk_titid;
                                objdetail.vchfname = objmodel.vchname;
                                objdetail.vchlname = objmodel.vchname;
                                objdetail.vchsex = objmodel.vchgender;
                                objdetail.vchmothername = objmodel.vahmothername;
                                objdetail.vchFatherName = objmodel.vchfathername;
                                objdetail.vchtaddress = objmodel.vchaddress;
                                objdetail.vchtstate = objmodel.vchstate;
                                objdetail.vchtcity = objmodel.vchcity;
                                objdetail.inttpin = Convert.ToInt32(objmodel.vchpincode);
                                objdetail.vchtmobile = objmodel.vchmobile;
                                objdetail.vchpaddress = objmodel.vchaddress;
                                objdetail.vchpstate = objmodel.vchstate;
                                objdetail.vchpcity = objmodel.vchcity;
                                objdetail.intppin = Convert.ToInt32(objmodel.vchpincode);
                                objdetail.vchpmobile = objmodel.vchmobile;
                                objdetail.BitCompleted = false;
                                objdetail.dtcreated = DateTime.Now;
                                objdetail.vchcreatedby = Session["hostname"].ToString();
                                objdetail.vchhostname = Session["hostname"].ToString();
                                objdetail.vchipused = Session["ipused"].ToString();
                                objdetail.dtDob = Convert.ToDateTime(objmodel.dtDOB);
                                objdetail.vchEmpTcode = "T" + selectedmasdata.intid.ToString();
                                selectedmasdata.vchEmpTcode = "T" + selectedmasdata.intid.ToString();
                                selectedmasdata.fk_intdeptid = selecteddept.intid;
                                selectedmasdata.fk_intdesiid = selecteddesi.intid;
                                selectedmasdata.BitCompleteAssesment = true;
                                selectedmasdata.bitIsByPassEntry = true;
                                if (objmodel.vchmaritalststus == "1")
                                {
                                    objdetail.vchmaritalststus = "Single";
                                }
                                else if (objmodel.vchmaritalststus == "2")
                                {
                                    objdetail.vchmaritalststus = "Married";
                                }
                                else if (objmodel.vchmaritalststus == "3")
                                {
                                    objdetail.vchmaritalststus = "Widowed";
                                }
                                if (objmodel.vchgender == "1")
                                {
                                    objdetail.vchsex = "Male";
                                }
                                else if (objmodel.vchgender == "2")
                                {
                                    objdetail.vchsex = "Female";
                                }
                                if (objmodel.vchspousename != null)
                                {
                                    objdetail.vchspouse = objmodel.vchspousename;
                                }
                                else
                                {
                                    objdetail.vchspouse = "N/A";
                                }
                                hrentity.tblEmpDetails.Add(objdetail);
                                hrentity.SaveChanges();
                                TempData["Success"] = "Employee detail saved successfully!";
                                return RedirectToAction("IndexPayroll");
                            }
                        }
                        else
                        {
                            TempData["error"] = "Employee mas details not found please check it again or try again!";
                            return RedirectToAction("IndexPayroll");
                        }

                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "Department manpower mapping not found please contact to administrator!";
                    return RedirectToAction("IndexPayroll");
                }
                if (ActiveDeptCount >= DeptCounter)
                {
                    TempData["Error"] = "Department maximum manpower has reached, Maximum number is : " + DeptCounter.ToString();
                    return RedirectToAction("IndexPayroll");
                }
                else
                {


                    //aadhaar number validation
                    tblEmpAssesmentMas objtable = new tblEmpAssesmentMas();
                    tblEmpDetails objdetail = new tblEmpDetails();
                    if (objmodel != null)
                    {
                        var checkAdharRedFlag = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.aadhaarno && e.BitIsFlaggingEmp == true select e).FirstOrDefault();
                        var checkAadhaar = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.aadhaarno && e.bittempstatusactive == true select e).FirstOrDefault();
                        var CheckOnlyAadhar = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.aadhaarno && e.BitStatus == false select e).FirstOrDefault();
                        var CheckAadharAssess = (from e in hrentity.tblEmpAssesmentMas where e.vchAadharNo == objmodel.aadhaarno && e.BitStatus == true select e).FirstOrDefault();
                        //Check for Flagging
                        if (checkAdharRedFlag != null && checkAdharRedFlag.BitIsFlaggingEmp == true)
                        {

                            //for red flag
                            if (checkAdharRedFlag.BitIsRedFlagging == true)
                            {
                                TempData["error"] = "Entered aadhaar number is red flag so you cannot hire/re-join this candidate!";
                                return RedirectToAction("IndexPayroll");
                            }
                            //for orange flag
                            else if (checkAdharRedFlag.BitIsOrangeFlagging == true)
                            {
                                DateTime aajkidate = DateTime.Now;
                                var leftdatetime = checkAdharRedFlag.dtDOL;
                                DateTime newjoindate = leftdatetime.Value.AddMonths(6);
                                DateTime doj = objmodel.dtDOJ;
                                if (doj > newjoindate)
                                {
                                    if (objmodel.bitIsReplacement == true)
                                    {
                                        objtable.bitIsReplacement = true;
                                        objtable.vchRplcmntName = objmodel.vchRplcmntName;
                                    }
                                    else
                                    {
                                        objtable.vchRplcmntName = "N/A";
                                    }
                                    objtable.fk_PositionId = selecteposi.intid;
                                    objtable.fk_intdesiid = desiid;
                                    objtable.fk_intdeptid = objmodel.fk_deptid;
                                    if (objmodel.vchgender == "1")
                                    {
                                        objtable.fk_inttitid = 1;
                                    }
                                    else
                                    {
                                        objtable.fk_inttitid = 7;
                                    }
                                    objtable.vchName = objmodel.vchname;
                                    objtable.decExperience = objmodel.experience;
                                    objtable.vchWorkedArea = objmodel.vchworkedarea;
                                    objtable.dtDOJ = objmodel.dtDOJ;
                                    objtable.vchMobile = objmodel.vchmobile;
                                    objtable.vchArea = objmodel.vchworkedarea;
                                    objtable.vchAadharNo = objmodel.aadhaarno;
                                    objtable.intcode = code;
                                    objtable.vchcreatedby = Session["descript"].ToString();
                                    objtable.vchFinalUpdatedBy = Session["descript"].ToString();
                                    objtable.dtcreated = DateTime.Today;
                                    objtable.dtFinalUpdated = DateTime.Today;
                                    objtable.vchcreatedhost = Session["hostname"].ToString();
                                    objtable.vchFinalHostname = Session["hostname"].ToString();
                                    objtable.vchcreatedipused = Session["ipused"].ToString();
                                    objtable.vchFinalipused = Session["ipused"].ToString();
                                    objtable.intsalary = objmodel.intsalary;
                                    hrentity.tblEmpAssesmentMas.Add(objtable);
                                    hrentity.SaveChanges();
                                }
                                else
                                {
                                    string joiningdate = newjoindate.ToString("dd/MM/yyyy");
                                    TempData["error"] = "Entered aadhaar number is orange flag so you can re-join this candidate after " + joiningdate + "date";
                                    return RedirectToAction("IndexPayroll");
                                }
                            }
                            else if (checkAdharRedFlag.BitIsGreenFlagging == true)
                            {
                                DateTime aajkidate = DateTime.Now;
                                var leftdatetime = checkAdharRedFlag.dtDOL;
                                DateTime newjoindate = leftdatetime.Value.AddMonths(3);
                                DateTime doj = objmodel.dtDOJ;
                                if (doj > newjoindate)
                                {
                                    if (objmodel.bitIsReplacement == true)
                                    {
                                        objtable.bitIsReplacement = true;
                                        objtable.vchRplcmntName = objmodel.vchRplcmntName;
                                    }
                                    else
                                    {
                                        objtable.vchRplcmntName = "N/A";
                                    }
                                    objtable.fk_PositionId = selecteposi.intid;
                                    objtable.fk_intdesiid = desiid;
                                    objtable.fk_intdeptid = objmodel.fk_deptid;
                                    if (objmodel.vchgender == "1")
                                    {
                                        objtable.fk_inttitid = 1;
                                    }
                                    else
                                    {
                                        objtable.fk_inttitid = 7;
                                    }
                                    objtable.vchName = objmodel.vchname;
                                    objtable.decExperience = objmodel.experience;
                                    objtable.vchWorkedArea = objmodel.vchworkedarea;
                                    objtable.dtDOJ = objmodel.dtDOJ;
                                    objtable.vchMobile = objmodel.vchmobile;
                                    objtable.vchArea = objmodel.vchworkedarea;
                                    objtable.vchAadharNo = objmodel.aadhaarno;
                                    objtable.intcode = code;
                                    objtable.vchcreatedby = Session["descript"].ToString();
                                    objtable.vchFinalUpdatedBy = Session["descript"].ToString();
                                    objtable.dtcreated = DateTime.Today;
                                    objtable.dtFinalUpdated = DateTime.Today;
                                    objtable.vchcreatedhost = Session["hostname"].ToString();
                                    objtable.vchFinalHostname = Session["hostname"].ToString();
                                    objtable.vchcreatedipused = Session["ipused"].ToString();
                                    objtable.vchFinalipused = Session["ipused"].ToString();
                                    objtable.intsalary = objmodel.intsalary;
                                    hrentity.tblEmpAssesmentMas.Add(objtable);
                                    hrentity.SaveChanges();
                                }
                                else
                                {
                                    string joiningdate = newjoindate.ToString("dd/MM/yyyy");
                                    TempData["error"] = "Entered aadhaar number is green flag so you can re-join this candidate after " + joiningdate + "date";
                                    return RedirectToAction("IndexPayroll");
                                }
                            }
                        }
                        //check is active aadhaar card
                        else if (checkAadhaar != null)
                        {
                            TempData["error"] = "Entered aadhaar number is already in use active employee";
                            return RedirectToAction("IndexPayroll");
                        }
                        else if (CheckOnlyAadhar != null)
                        {
                            TempData["error"] = "Entered aadhaar number is already in use new employee assessment";
                            return RedirectToAction("IndexPayroll");
                        }
                        else if (CheckAadharAssess != null)
                        {
                            TempData["error"] = "Entered aadhaar number is already in use employee completed assessment";
                            return RedirectToAction("IndexPayroll");
                        }
                        else
                        {
                            try
                            {
                                if (objmodel.bitIsReplacement == true)
                                {
                                    objtable.bitIsReplacement = true;
                                    objtable.vchRplcmntName = objmodel.vchRplcmntName;
                                }
                                else
                                {
                                    objtable.vchRplcmntName = "N/A";
                                }
                                objtable.fk_PositionId = selecteposi.intid;
                                objtable.fk_intdesiid = desiid;
                                objtable.fk_intdeptid = objmodel.fk_deptid;
                                if (objmodel.vchgender == "1")
                                {
                                    objtable.fk_inttitid = 1;
                                }
                                else
                                {
                                    objtable.fk_inttitid = 7;
                                }
                                objtable.vchName = objmodel.vchname;
                                objtable.decExperience = objmodel.experience;
                                objtable.vchWorkedArea = objmodel.vchworkedarea;
                                objtable.dtDOJ = objmodel.dtDOJ;
                                objtable.vchMobile = objmodel.vchmobile;
                                objtable.vchArea = objmodel.vchworkedarea;
                                objtable.vchAadharNo = objmodel.aadhaarno;
                                objtable.intcode = code;
                                objtable.vchcreatedby = Session["descript"].ToString();
                                objtable.vchFinalUpdatedBy = Session["descript"].ToString();
                                objtable.dtcreated = DateTime.Today;
                                objtable.dtFinalUpdated = DateTime.Today;
                                objtable.vchcreatedhost = Session["hostname"].ToString();
                                objtable.vchFinalHostname = Session["hostname"].ToString();
                                objtable.vchcreatedipused = Session["ipused"].ToString();
                                objtable.vchFinalipused = Session["ipused"].ToString();
                                objtable.intsalary = objmodel.intsalary;
                                hrentity.tblEmpAssesmentMas.Add(objtable);
                                hrentity.SaveChanges();
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
                                return RedirectToAction("IndexPayroll");
                            }
                        }
                        var selectedmasdata = (from e in hrentity.tblEmpAssesmentMas where e.vchName == objmodel.vchname && e.vchMobile == objmodel.vchmobile select e).FirstOrDefault();
                        if (selectedmasdata != null)
                        {
                            var selecteddept = (from e in hrentity.tblDeptMas where e.intid == objmodel.fk_deptid select e).FirstOrDefault();
                            objdetail.fk_intempid = selectedmasdata.intid;
                            objdetail.vchNominee = objmodel.vchNominee;
                            objdetail.vchRelation = objmodel.vchNomRelation;
                            objdetail.intage = objmodel.age;
                            objdetail.fk_titid = objmodel.fk_titid;
                            objdetail.vchfname = objmodel.vchname;
                            objdetail.vchlname = objmodel.vchname;
                            objdetail.vchsex = objmodel.vchgender;
                            objdetail.vchmothername = objmodel.vahmothername;
                            objdetail.vchFatherName = objmodel.vchfathername;
                            objdetail.vchtaddress = objmodel.vchaddress;
                            objdetail.vchtstate = objmodel.vchstate;
                            objdetail.vchtcity = objmodel.vchcity;
                            objdetail.inttpin = Convert.ToInt32(objmodel.vchpincode);
                            objdetail.vchtmobile = objmodel.vchmobile;
                            objdetail.vchpaddress = objmodel.vchaddress;
                            objdetail.vchpstate = objmodel.vchstate;
                            objdetail.vchpcity = objmodel.vchcity;
                            objdetail.intppin = Convert.ToInt32(objmodel.vchpincode);
                            objdetail.vchpmobile = objmodel.vchmobile;
                            objdetail.BitCompleted = false;
                            objdetail.dtcreated = DateTime.Now;
                            objdetail.vchcreatedby = Session["hostname"].ToString();
                            objdetail.vchhostname = Session["hostname"].ToString();
                            objdetail.vchipused = Session["ipused"].ToString();
                            objdetail.dtDob = Convert.ToDateTime(objmodel.dtDOB);
                            objdetail.vchEmpTcode = "T" + selectedmasdata.intid.ToString();
                            selectedmasdata.vchEmpTcode = "T" + selectedmasdata.intid.ToString();
                            selectedmasdata.fk_intdeptid = selecteddept.intid;
                            selectedmasdata.fk_intdesiid = selecteddesi.intid;
                            selectedmasdata.BitCompleteAssesment = true;
                            selectedmasdata.bitIsByPassEntry = true;
                            if (objmodel.vchmaritalststus == "1")
                            {
                                objdetail.vchmaritalststus = "Single";
                            }
                            else if (objmodel.vchmaritalststus == "2")
                            {
                                objdetail.vchmaritalststus = "Married";
                            }
                            else if (objmodel.vchmaritalststus == "3")
                            {
                                objdetail.vchmaritalststus = "Widowed";
                            }
                            if (objmodel.vchgender == "1")
                            {
                                objdetail.vchsex = "Male";
                            }
                            else if (objmodel.vchgender == "2")
                            {
                                objdetail.vchsex = "Female";
                            }
                            if (objmodel.vchspousename != null)
                            {
                                objdetail.vchspouse = objmodel.vchspousename;
                            }
                            else
                            {
                                objdetail.vchspouse = "N/A";
                            }
                            hrentity.tblEmpDetails.Add(objdetail);
                            hrentity.SaveChanges();
                            TempData["Success"] = "Employee detail saved successfully!";
                            return RedirectToAction("IndexPayroll");
                        }
                    }
                    else
                    {
                        TempData["error"] = "Employee mas details not found please check it again or try again!";
                        return RedirectToAction("IndexPayroll");
                    }

                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Upload bypass emp document

        //view all bypass pending for upload
        public ActionResult ViewByPassUpload()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selected = (from e in hrentity.tblEmpAssesmentMas where e.bitIsByPassEntry == true && e.BitIsUploadCompleted!=true && e.bitgoauthor!=true && e.intcode == code select e).ToList();
                if (selected.Count != 0)
                {
                    return View(selected);
                }
                else
                {
                    ViewBag.Empty = "0 employee found in database";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //all compulsory document upload
        public ActionResult UpByPassDoc(int id)
        {
            if (Session["descript"] != null)
            {
                //check emp id
                if (id != 0)
                {
                    var selecetdemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    //check emp details avilable or not
                    if (selecetdemp != null)
                    {
                        int positionid = selecetdemp.fk_PositionId;
                        int code = Convert.ToInt32(Session["id"].ToString());
                        List<tblPosDocMap> New_list = new List<tblPosDocMap>();
                        IEnumerable<tblPosDocMap> New_List2 = new List<tblPosDocMap>();
                        var getCompDoc = (from e in hrentity.tblPosDocMap where e.fk_PosCatid == selecetdemp.fk_PositionId && e.IsSelected == true select e).ToList();
                        var compdoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && (e.BitIsCompDoc == true || e.BitIsProfilePic==true) && e.intcode == code select e).ToList();
                        if (getCompDoc.Count() != 0 && compdoc.Count() != 0)
                        {
                            foreach (var Doc in getCompDoc)
                            {
                                foreach (var uploaded in compdoc)
                                {
                                    if (Doc.fk_docid == uploaded.fk_intdocid )
                                    {
                                        Doc.BitIsUploaded = true;
                                        Doc.ComparedFileName = uploaded.vchfilename;
                                        Doc.fk_TempEmpId = uploaded.fk_empAssid;
                                        if (uploaded.dtcreated != null)
                                        {
                                            Doc.dt_TempUploaded = uploaded.dtcreated;
                                        }
                                    }
                                    Doc.fk_TempEmpId = uploaded.fk_empAssid;
                                }
                                New_list.Add(Doc);
                            }
                        }
                        else
                        {
                            var getAllCompDoc = (from e in hrentity.tblPosDocMap where e.fk_PosCatid == selecetdemp.fk_PositionId && e.IsSelected==true select e).ToList();
                            if (getAllCompDoc.Count() != 0)
                            {
                                foreach (var doc in getAllCompDoc)
                                {
                                    doc.fk_TempEmpId = selecetdemp.intid;                                  
                                }                               
                            }
                            return View(getAllCompDoc);
                        }
                    
                        if (compdoc != null)
                        {
                            int positionID = selecetdemp.fk_PositionId;
                            return View(New_list);
                        }
                        else
                        {
                            TempData["Empty"] = "Compulsory document not found please chect it!";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["Empty"] = "Employee document not found in database!";
                        return View();
                    }
                }
                else
                {
                    TempData["Empty"] = "Compulsory document not found please chect it!";
                    return View();
                }

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult UpNewDoc(int id, int docid)
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                //mast doc id=13(Profile pic)
                if (docid == 13)
                {
                    return RedirectToAction("ByPassProfilePic", new { id = id,docid=docid });
                }
                else
                {
                    var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                    var selectedDoc = (from e in hrentity.tblDocMas where e.intid == docid select e).FirstOrDefault();
                    if (selectedDoc != null)
                    {
                        ViewBag.Docname = selectedDoc.vchdocname.ToString();
                    }
                    ViewBag.Mobile = selectedemp.vchMobile.ToString();
                    ViewBag.Empname = selectedemp.vchName.ToString();
                    //For compulsory document
                    UpByPassDocViewModel objmodel = new UpByPassDocViewModel();
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
        
        [HttpPost]
        public ActionResult UpNewDoc(UpByPassDocViewModel objmodel)
        {
            if (Session["descript"] != null)
            {
                if (ModelState.IsValid)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int year = Convert.ToInt32(Session["yr"].ToString());
                    //docdetails table object
                    tblDocDetails objdocdetail = new tblDocDetails();
                    int empid = objmodel.empid;
                    //get all emp id qualifications
                    int docid = objmodel.fk_docid;
                    //get mas doc
                    var MasDoc = (from e in hrentity.tblDocMas where e.intid == docid select e).FirstOrDefault();
                    //get mas data
                    var empmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    //check is doc uploaded or not
                    var checkdoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == empid && e.fk_intdocid == docid && e.intcode == code select e).FirstOrDefault();
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
                            string newfilename = "New" + "_" + final_FileName + Final_datetime + empid.ToString() + extension;
                            string finalfilename = newfilename.Replace(" ", "");
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + finalfilename));
                            //save file in upload folder
                            objmodel.newpdfFile.SaveAs(path);
                            objdocdetail.fk_empAssid = empid;
                            objdocdetail.fk_intdocid = docid;
                            objdocdetail.vchdocname = MasDoc.vchdocname.ToString();
                            objdocdetail.vchfilename = newfilename.ToString();
                            objdocdetail.vchdocadd = path.ToString();
                            objdocdetail.vchcreatedby = Session["descript"].ToString();
                            objdocdetail.dtcreated = DateTime.Now;
                            objdocdetail.vchipused = Session["ipused"].ToString();
                            objdocdetail.vchhostname = Session["hostname"].ToString();
                            objdocdetail.intcode = code;
                            objdocdetail.intyr = year;
                            objdocdetail.BitIsCompDoc = true;
                            hrentity.tblDocDetails.Add(objdocdetail);
                            hrentity.SaveChanges();
                            TempData["Success"] = "Document upload successfully!";
                            return RedirectToAction("UpByPassDoc", new { id = empid });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Select pdf file for upload!";
                        return View("UpByPassDoc", new { id = objmodel.empid });
                    }

                }
                else
                {
                    //ModelState.AddModelError("newpdfFile", "ModelError Generated!");
                    TempData["Error"] = "Model error generated please try again with document and .pdf file selections or contact to administrator!";
                    return RedirectToAction("UpByPassDoc", new { id = objmodel.empid });
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        //for upload profile pic
        public ActionResult ByPassProfilePic(int id, int docid)
        {
            if (id != 0)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                ProfileViewModel objmodel = new ProfileViewModel();
                var selectedDOc = (from e in hrentity.tblDocMas where e.intid == docid select e).FirstOrDefault();
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                ViewBag.Mobile = selectedemp.vchMobile.ToString();
                ViewBag.Empname = selectedemp.vchName.ToString();
                ViewBag.EmpProfileName = "Profile Picture";
                ViewBag.EmpID = id.ToString();
                ViewBag.Filename = selectedDOc.vchdocname;                
                objmodel.BitIsCompleted = Convert.ToBoolean(selectedemp.bitProfileComplete);
                objmodel.docid = docid;
                objmodel.empid = id;
                objmodel.picname = selectedDOc.vchdocname;
                return View(objmodel);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ByPassProfilePic(ProfileViewModel objpic, FormCollection fromform)
        {
            if (ModelState.IsValid)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int year = Convert.ToInt32(Session["yr"].ToString());
                //docdetails table object
                tblDocDetails objdocdetail = new tblDocDetails();
                //get emp id
                int id1 = objpic.empid;
                string selectedDoc = objpic.picname.ToString();
                var checktable = (from e in hrentity.tblDocDetails where e.fk_empAssid == id1 && e.BitIsProfilePic == true && e.intcode == code select e).FirstOrDefault();
                if (checktable != null)
                {
                    string selecetddocname = checktable.vchdocname.ToString();
                    TempData["Error"] = "Against" + " " + selecetddocname + " " + "uploaded against current name!";
                    return RedirectToAction("ByPassProfilePic", new { id = id1,objpic.docid });
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
                            return RedirectToAction("ByPassProfilePic", new { id = objpic.empid,docid=objpic.docid });
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
                            selecetdmasemp.bitProfileComplete = true;                        
                            objdocdetail.fk_empAssid = id1;
                            objdocdetail.fk_intdocid = objpic.docid;
                            objdocdetail.vchdocname = selectedDoc;
                            objdocdetail.vchdocadd = path.ToString();
                            objdocdetail.vchfilename = finalfilename.ToString();
                            objdocdetail.vchcreatedby = Session["descript"].ToString();
                            objdocdetail.dtcreated = DateTime.Now;
                            objdocdetail.vchipused = Session["ipused"].ToString();
                            objdocdetail.vchhostname = Session["hostname"].ToString();
                            objdocdetail.BitIsProfilePic = true;                        
                            objdocdetail.intcode = code;
                            objdocdetail.intyr = year;
                            hrentity.tblDocDetails.Add(objdocdetail);
                            hrentity.SaveChanges();
                            TempData["Success"] = "Photograph upload successfully!";
                            return RedirectToAction("UpByPassDoc", new { id = objpic.empid });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Please select profile file for upload!";
                        return RedirectToAction("ByPassProfilePic", new { id = objpic.empid, objpic.docid });
                    }
                }
            }
            else
            {                
                TempData["Error"] = "Model Error Generated!";
                return RedirectToAction("ByPassProfilePic", new { id = objpic.empid, docid = objpic.docid });
            }
        }

        public ActionResult CompleteByPassUpload(int id)
        {
            if (Session["descript"] != null)
            {
                if (id != 0)
                {
                    var selected = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    if (selected != null)
                    {
                        //doc object
                        List<tblDocMas> objDoc = new List<tblDocMas>();
                        //get all positional required document that should be uploaded
                        int posID = selected.fk_PositionId;
                        List<int> getRequiredDoc = (from e in hrentity.tblPosDocMap where e.fk_PosCatid == posID && e.bitRequireForAuthorization select e.fk_docid).ToList();
                        //get all employee doc uploaded
                        List<int> uploadedDoc = new List<int>();
                        var GetEmpDoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id select e).ToList();
                        if (GetEmpDoc != null)
                        {
                            foreach(var docid in GetEmpDoc)
                            {
                                int Requiredid = Convert.ToInt32(docid.fk_intdocid);
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
                                    var getPendingDoc = (from e in hrentity.tblDocMas where e.intid == doc select e).FirstOrDefault();
                                    if (getPendingDoc != null)
                                    {
                                        objDoc.Add(getPendingDoc);
                                    }
                                }
                                TempData["PendingDoc"] = objDoc;
                                return RedirectToAction("ViewByPassUpload");
                            }                            
                            else
                            {

                            }
                        }
                        else
                        {
                            selected.BitIsUploadCompleted = true;
                            selected.bitofficialdetails = true;
                            selected.bittofficialdetails = true;
                            hrentity.SaveChanges();
                            TempData["Success"] = "Employee locked successfully!";
                            return RedirectToAction("ViewByPassUpload");
                        }
                        selected.BitIsUploadCompleted = true;
                        selected.bitofficialdetails = true;
                        selected.bittofficialdetails = true;
                        hrentity.SaveChanges();
                        TempData["Success"] = "Employee locked successfully!";
                        return RedirectToAction("ViewByPassUpload");
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

        public ActionResult AllByPassCodeEmp()
        {
            if (Session["descript"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selected = (from e in hrentity.tblEmpAssesmentMas where e.bitIsByPassEntry == true && e.bitgoauthor!=true && e.BitIsUploadCompleted == true && e.bittempstatusactive!=true && e.bitIsLeft!=true && e.intcode == code select e).ToList();
                if (selected.Count()!=0)
                {
                    return View(selected);
                }
                else
                {
                    ViewBag.Empty = "0 employee found in database";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult SendForVerification(string empid)
        {
            if (Session["descript"] != null)
            {
                int id = Convert.ToInt32(empid);
                var getSelected = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                if (getSelected != null)
                {
                    //generate User fist for user dashboard
                    tblEmpLoginUser objuser = new tblEmpLoginUser();
                    objuser.fk_intEmpID = id;
                    objuser.vchmobile = getSelected.vchMobile;
                    objuser.intcode = Convert.ToInt32(getSelected.intcode);
                    objuser.vchOTP = "Temp";
                    objuser.bitProfileComplete = false;
                    objuser.bitSigUploaded = false;
                    objuser.bitISAllowedFirstDayLeave = false;
                    hrentity.tblEmpLoginUser.Add(objuser);
                    getSelected.bitofficialdetails = true;
                    getSelected.bittofficialdetails = true;
                    getSelected.bitgoauthor = true;
                    hrentity.SaveChanges();
                    ViewBag.Success = "Document verification sent successfully!";
                    return View("AllByPassCodeEmp");
                }
                else
                {
                    ViewBag.Error = "Candidate details not found please check it again and try!";
                    return View("AllByPassCodeEmp");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult GenByPassCode(string empid,string isCorporate)
        {
            if (Session["descript"] != null)
            {
                if (empid != null)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    int id = Convert.ToInt32(empid);
                    var selectedmasdata = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    var selecteddept = (from e in hrentity.tblDeptMas where e.intid == selectedmasdata.fk_intdeptid select e).FirstOrDefault();
                    //generate employee code for bypass
                    if (isCorporate == "Yes")
                    {
                        DateTime doj = Convert.ToDateTime(selectedmasdata.dtDOJ);
                        string year = doj.ToString("yyyy");
                        string finalyear = doj.ToString("yy");
                        int codeYear = 2023;
                        //Generate corporate code
                        var getcode = (from e in hrentity.tblEmpCodeMas where e.intBranchCode == 0 && e.intJoinYear == codeYear select e).FirstOrDefault();
                        int currentcode = getcode.intCurrentCode;
                        int newcode = currentcode + 1;
                        int number = newcode;
                        int counter = 0;
                        string finalnumber = "";
                        int fnumber = 0;
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
                            fnumber = newcode;
                        }
                        //Get Dept Code.
                        string fdeptcode = selecteddept.vchdepCode.ToString();
                        //Get Branch Code.
                        string branchcode = getcode.vchUnitCode.ToString();
                        string finalCompleteCode = branchcode + "-" + finalyear + "-" + fdeptcode + "-" + finalnumber;
                        //update current code
                        getcode.intCurrentCode = newcode;
                        //add mas table entry
                        selectedmasdata.vchEmpFcode = finalCompleteCode;
                        selectedmasdata.bitIsCorporateemp = true;
                        selectedmasdata.bittempstatusactive = true;
                        hrentity.SaveChanges();
                        TempData["Success"] = "Code "+finalCompleteCode+" generated successfully, check employee in active list!";
                        return RedirectToAction("AllByPassCodeEmp");
                    }
                    if (isCorporate=="No")
                    {
                        DateTime doj = Convert.ToDateTime(selectedmasdata.dtDOJ);
                        string year = doj.ToString("yyyy");
                        string finalyear = doj.ToString("yy");
                        int CodeYear = 2023;
                        //Generate unit code code
                        var getcode = (from e in hrentity.tblEmpCodeMas where e.intBranchCode == code && e.intJoinYear == CodeYear select e).FirstOrDefault();
                        int currentcode = getcode.intCurrentCode;
                        int newcode = currentcode + 1;
                        int number = newcode;
                        int counter = 0;
                        string finalnumber = "";
                        int fnumber = 0;
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
                            fnumber = newcode;
                        }
                        //Get Dept Code.
                        string fdeptcode = selecteddept.vchdepCode.ToString();
                        //Get Branch Code.
                        string branchcode = getcode.vchUnitCode.ToString();
                        string finalCompleteCode = branchcode + "-" + finalyear + "-" + fdeptcode + "-" + finalnumber;
                        //update current code
                        getcode.intCurrentCode = newcode;
                        //add mas table entry
                        selectedmasdata.vchEmpFcode = finalCompleteCode;
                        selectedmasdata.bitIsUnitEmp = true;
                        selectedmasdata.bittempstatusactive = true;
                        hrentity.SaveChanges();
                        TempData["Success"] = "Code "+finalCompleteCode+" generated successfully, check employee in active list!";
                        return RedirectToAction("AllByPassCodeEmp");
                    }
                    return View();
                }
                else
                {
                    ViewBag.Error = "Employee id should not be null or 0";
                    return View("AllByPassCodeEmp");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult deleteDocument(int id,int empid)
        {
            if (Session["descript"] != null)
            {
                if (id != 0 && empid!=0)
                {
                    var selectedDoc = (from e in hrentity.tblDocDetails where e.fk_empAssid==empid && e.fk_intdocid==id select e).FirstOrDefault();
                    if (selectedDoc != null)
                    {
                        System.IO.File.Delete(selectedDoc.vchdocadd);
                        hrentity.tblDocDetails.Remove(selectedDoc);
                        //if going to delete is profile pic
                        if (id ==13)
                        {
                            //update mas data as profile not complete
                            var selectedMas = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid select e).FirstOrDefault();
                            if (selectedMas != null)
                            {
                                selectedMas.bitProfileComplete = false;                               
                            }
                        }
                        hrentity.SaveChanges();
                        TempData["Success"] = "Document deleted successfully!";
                        return RedirectToAction("UpByPassDoc",new { id = selectedDoc.fk_empAssid });
                    }
                    else
                    {
                        TempData["Error"] = "Document details not found please check it again and try!";
                        return RedirectToAction("UpByPassDoc", new { id = empid });
                    }
                }
                else
                {
                    TempData["Error"] = "Document id should not be null or 0";
                    return RedirectToAction("ViewAssignAuthorization");
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }
        #endregion
    }
}
    






