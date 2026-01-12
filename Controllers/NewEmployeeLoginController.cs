using ClosedXML.Excel;
using HRM.Models;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace HRM.Controllers
{
    public class NewEmployeeLoginController : Controller
    {
        //GET: NewEmployeeLogin       
        HRMEntities hrentity = new HRMEntities();
        IndusGroupEntities gpentity = new IndusGroupEntities();
        public ActionResult GenerateOTP()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GenerateOTP(tblEmpLoginUser objtable)
        {
            if (objtable.vchmobile != null)
            {
                //int code = Convert.ToInt32(Session["id"].ToString());
                var selected = (from e in hrentity.tblEmpLoginUser where e.vchmobile == objtable.vchmobile select e).FirstOrDefault();
                Session["MobileNo"] = objtable.vchmobile.ToString();
                if (selected != null)
                {
                    string formatting = "0000"; //Will pad out to four digits if under 1000   
                    int _min = 0;
                    int _max = 9999;
                    Random randomNumber = new Random();
                    var randomNumberString = randomNumber.Next(_min, _max).ToString(formatting);
                    //int newcode = Convert.ToInt32(randomNumberString);
                    selected.vchOTP = randomNumberString.ToString();
                    hrentity.SaveChanges();
                    string mob = selected.vchmobile.ToString();
                    string OTP = randomNumberString.ToString();
                    var uri = "http://164.52.195.161/API/SendMsg.aspx?uname=20170839&pass=Indus2017&send=HOSPIN&dest=" + mob + "&msg=Please login HRMS INDUS using OTP : " + OTP + "&priority=1";
                    //var uri = AppSettings.SMSApiUrl;
                    //var apiKey = AppSettings.SMSApiKey;
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

                            #region New SMS API
                            //new api method
                            //var requestData = new
                            //{
                            //    sender = "HOSPIN",
                            //    templateName = "OTP",
                            //    smsReciever = new[]
                            //        {
                            //        new {
                            //            mobileNo = mob,
                            //            templateParams = OTP
                            //        }
                            //    }
                            //};
                            //using (var client = new HttpClient())
                            //{
                            //    client.DefaultRequestHeaders.Add("apiKey", apiKey);
                            //    var json = JsonConvert.SerializeObject(requestData);
                            //    var content = new StringContent(json, Encoding.UTF8, "application/json");
                            //    var response = await client.PostAsync(uri, content);
                            //    if (response.IsSuccessStatusCode)
                            //    {
                            //        //Session["OTP"] = otp;  // Store OTP for verification
                            //        return Json(new { status = "Success", message = "OTP Sent Successfully" });
                            //    }
                            //    else
                            //    {
                            //        return Json(new { status = "Failed", message = "Failed to Send OTP" });
                            //    }
                            //}
                            #endregion
                        }
                        catch
                        {

                        }
                        return RedirectToAction("EmpLogin");
                    }
                    else
                    {
                        TempData["Success"] = "OTP sent to your mobile.";
                        return View("Employee");
                    }
                }
                else
                {
                    TempData["Error"] = "Mobile number does not registered with us!";
                    return View();
                }
            }
            else
            {
                //TempData["Error"] = "Invalid mobile number contact to administrator!";
                return View();
            }
        }

        public ActionResult selcetbranch()
        {
            return View();
        }

        public ActionResult EmpLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmpLogin(tblEmpLoginUser newobj, IndusCompanies SelectedCompany)
        {
            if (Session["MobileNo"] != null)
            {
                var mobile = Session["MobileNo"].ToString();
                Session["id"] = SelectedCompany.intPK.ToString();
                if (SelectedCompany.intPK != 0)
                {
                    int compid = Convert.ToInt32(SelectedCompany.intPK);
                    var selectedcomp = (from e in gpentity.IndusCompanies where e.intPK == compid select e).FirstOrDefault();
                    if (selectedcomp != null)
                    {
                        Session["year"] = selectedcomp.ses_code.ToString();
                    }
                    else
                    {

                    }
                }
                //Session["year"] = SelectedCompany.ses_code.ToString();
                int code = SelectedCompany.intPK;
                string otp = newobj.vchOTP;
                var selecetdemp = (from e in hrentity.tblEmpLoginUser where e.vchmobile == mobile && e.intcode == code select e).FirstOrDefault();
                if (selecetdemp != null)
                {
                    var checkotp = (from e in hrentity.tblEmpLoginUser
                                    join d in hrentity.tblEmpAssesmentMas on e.fk_intEmpID equals d.intid
                                    where e.vchmobile == mobile && e.vchOTP == otp && e.intcode == code
                                    select e).FirstOrDefault();
                    if (checkotp != null)
                    {
                        Session["empid"] = checkotp.fk_intEmpID.ToString();
                        int getempid = Convert.ToInt32(checkotp.fk_intEmpID);
                        var getempname = (from e in hrentity.tblEmpAssesmentMas where e.intid == getempid select e).FirstOrDefault();
                        Session["EmpName"] = getempname.vchName.ToString();
                        //get host name
                        string strHostName = System.Net.Dns.GetHostName();
                        System.Net.IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
                        Session["hostname"] = strHostName.ToString();
                        //get hr id
                        var getHrID = (from e in hrentity.tblUserMaster where e.fk_intDesignationid == 12 && e.intcode == code select e).FirstOrDefault();
                        if (getHrID != null)
                        {
                            Session["BHR_UID"] = getHrID.ToString();
                        }
                        //get ip address
                        System.Net.IPAddress[] addr = ipEntry.AddressList;
                        string ip = addr[1].ToString();
                        Session["ipused"] = ip.ToString();
                        selecetdemp.dtlastlogin = DateTime.Now;
                        hrentity.SaveChanges();
                        return RedirectToAction("EmpDetailActions", new { id = checkotp.fk_intEmpID });
                    }
                    else
                    {
                        ModelState.AddModelError("vchOTP", "OTP not valid");
                        return View();
                    }

                }
                else
                {
                    ModelState.AddModelError("vchmobile", "Mobile number not valid");
                    return View();
                }
            }
            ModelState.AddModelError("vchOTP", "Mobile number not found!");
            return View();
        }

        public ActionResult EmpDetailActions(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            List<tblEmpAssesmentMas> selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).ToList();
            if (selectedemp.Count != 0)
            {
                return View(selectedemp);
            }
            else
            {
                TempData["Error"] = "Error generated while login please try again!";
                return View();
            }

        }

        #region Compulsory document
        public ActionResult UpCompDoc(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
            ViewBag.Mobile = selectedemp.vchMobile.ToString();
            ViewBag.Empname = selectedemp.vchName.ToString();
            ViewBag.EmpID = id.ToString();
            //For compulsory document
            DocCompulsoryModel objmodel = new DocCompulsoryModel();
            int fk_possiid = selectedemp.fk_PositionId;
            var compdoc = (from e in hrentity.tblPosDocMap
                           where e.fk_PosCatid == fk_possiid && e.IsSelected == true
                           select e).ToList();
            List<SelectListItem> compdocument = new List<SelectListItem>();
            foreach (var codoc in compdoc)
            {
                SelectListItem selectListItem = new SelectListItem()
                {
                    Text = codoc.tblDocMas.vchdocname.ToString(),
                    Value = codoc.tblDocMas.intid.ToString()
                };
                compdocument.Add(selectListItem);
            }
            objmodel.compdoc = compdocument;
            objmodel.BitIsCompleted = selectedemp.bitCompDocP;
            return View(objmodel);
        }

        [HttpPost]
        public ActionResult UpCompDoc(DocCompulsoryModel objmdel, FormCollection fmcolect)
        {
            if (Session["EmpName"] != null)
            {
                if (ModelState.IsValid)
                {
                    //docdetails table object
                    tblDocDetails objdocdetail = new tblDocDetails();
                    //get emp id
                    int id1 = Convert.ToInt32(fmcolect.Get("hdempid"));
                    //int id = Convert.ToInt32(newdoc.empid);
                    //get all emp id qualifications
                    int docid = Convert.ToInt32(objmdel.compdocname);
                    //get mas data
                    int code = Convert.ToInt32(Session["id"].ToString());
                    var empmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == id1 && e.intcode == code select e).FirstOrDefault();
                    if (objmdel.BitIsCompleted == true)
                    {
                        empmas.bitCompDocP = true;
                        hrentity.SaveChanges();
                        //TempData["Success"] = "Status updated successfully";
                        //return RedirectToAction("EmpLogin");
                    }
                    else
                    {
                        empmas.bitCompDocT = true;
                        hrentity.SaveChanges();
                    }
                    //get comp doc name
                    var selecteddoc = (from e in hrentity.tblPosDocMap where e.fk_docid == docid select e).FirstOrDefault();
                    //get selected doc details
                    var docdetails = (from e in hrentity.tblDocMas where e.intid == docid select e).FirstOrDefault();
                    //check is doc uploaded or not
                    var checkdoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id1 && e.fk_intdocid == docid select e).FirstOrDefault();
                    if (checkdoc != null)
                    {
                        string docname = checkdoc.vchdocname.ToString();
                        TempData["Error"] = "Against" + " " + docname + " " + "document already uploaded";
                        return RedirectToAction("UpCompDoc", new { id = id1 });
                    }
                    else
                    {
                        if (objmdel.compdocument != null)
                        {
                            if (objmdel.BitIsCompleted == true)
                            {
                                string empid = id1.ToString();
                                string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                                string extension = Path.GetExtension(objmdel.compdocument.FileName);
                                if (extension != ".pdf")
                                {
                                    TempData["Error"] = "Please select .pdf file for upload!";
                                    return RedirectToAction("employeeupload", new { id = id1 });
                                }
                                else
                                {
                                    string filename = Path.GetFileNameWithoutExtension(objmdel.compdocument.FileName);
                                    string newfilename = filename + datetime + empid + extension;
                                    string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + newfilename));
                                    //save file in upload folder
                                    objmdel.compdocument.SaveAs(path);
                                    objdocdetail.fk_empAssid = id1;
                                    objdocdetail.fk_intdocid = docid;
                                    objdocdetail.vchdocname = selecteddoc.tblDocMas.vchdocname.ToString();
                                    objdocdetail.vchfilename = newfilename.ToString();
                                    objdocdetail.vchdocadd = path.ToString();
                                    objdocdetail.vchcreatedby = Session["EmpName"].ToString();
                                    objdocdetail.dtcreated = DateTime.Now;
                                    objdocdetail.vchipused = Session["ipused"].ToString();
                                    objdocdetail.vchhostname = Session["hostname"].ToString();
                                    objdocdetail.intcode = code;
                                    objdocdetail.intyr = Convert.ToInt32(Session["year"]);
                                    objdocdetail.BitIsCompDoc = true;
                                    empmas.bitCompDocT = true;
                                    hrentity.tblDocDetails.Add(objdocdetail);
                                    hrentity.SaveChanges();
                                    TempData["Success"] = "Document final upload saved successfully!";
                                    return RedirectToAction("UpCompDoc", new { id = id1 });
                                }
                            }
                            else
                            {
                                string empid = id1.ToString();
                                string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                                string extension = Path.GetExtension(objmdel.compdocument.FileName);
                                if (extension != ".pdf")
                                {
                                    TempData["Error"] = "Please select .pdf file for upload!";
                                    return RedirectToAction("employeeupload", new { id = id1 });
                                }
                                else
                                {
                                    string filename = Path.GetFileNameWithoutExtension(objmdel.compdocument.FileName);
                                    string newfilename = filename + datetime + empid + extension;
                                    string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + newfilename));
                                    //save file in upload folder
                                    objmdel.compdocument.SaveAs(path);
                                    objdocdetail.fk_empAssid = id1;
                                    objdocdetail.fk_intdocid = docid;
                                    objdocdetail.vchdocname = selecteddoc.tblDocMas.vchdocname.ToString();
                                    objdocdetail.vchfilename = newfilename.ToString();
                                    objdocdetail.vchdocadd = path.ToString();
                                    objdocdetail.vchcreatedby = Session["EmpName"].ToString();
                                    objdocdetail.dtcreated = DateTime.Now;
                                    objdocdetail.vchipused = Session["ipused"].ToString();
                                    objdocdetail.vchhostname = Session["hostname"].ToString();
                                    objdocdetail.intcode = code;
                                    objdocdetail.intyr = Convert.ToInt32(Session["year"]);
                                    objdocdetail.BitIsCompDoc = true;
                                    empmas.bitCompDocT = true;
                                    hrentity.tblDocDetails.Add(objdocdetail);
                                    hrentity.SaveChanges();
                                    TempData["Success"] = "Document upload successfully!";
                                    return RedirectToAction("UpCompDoc", new { id = id1 });
                                }
                            }
                        }
                        else
                        {
                            return View("EmpLogin");
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
                return RedirectToAction("EmpLogin");
            }
        }

        public ActionResult ViewCompDoc()
        {
            if (Session["empid"] != null)
            {
                int id = Convert.ToInt32(Session["empid"].ToString());
                int code = Convert.ToInt32(Session["id"].ToString());
                var selecteddoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompDoc == true && e.intcode == code select e).ToList();
                if (selecteddoc.Count != 0)
                {
                    return View(selecteddoc);
                }
                else
                {
                    TempData["Empty"] = "No document avilable in database!";
                    return View();
                }
            }
            else
            {
                TempData["Empty"] = "Invalid employee id!";
                return View();
            }
        }

        #endregion

        #region Qualification document

        public ActionResult employeeupload(int id)
        {
            if (id != 0)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                ViewBag.Mobile = selectedemp.vchMobile.ToString();
                ViewBag.Empname = selectedemp.vchName.ToString();
                var empallquali = (from e in hrentity.tblQualiMas select e).ToList();
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
                objmodel.empid = selectedemp.intid;
                objmodel.docnamelist = quali;
                int fk_possiid = selectedemp.fk_PositionId;
                objmodel.BitCompleted = selectedemp.BitIsUploadCompleted;
                //TempData["docname"] = quali;
                ViewBag.EmpID = id.ToString();
                return View(objmodel);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult employeeupload(DocumentViewModel objnew, FormCollection formdata)
        {
            if (Session["EmpName"] != null)
            {
                if (ModelState.IsValid)
                {
                    int code = Convert.ToInt32(Session["id"].ToString());
                    //docdetails table object
                    tblDocDetails objdocdetail = new tblDocDetails();
                    //get emp id
                    int id1 = Convert.ToInt32(formdata.Get("hdempid"));
                    //int id = Convert.ToInt32(newdoc.empid);
                    //get all emp id qualifications
                    int docid = Convert.ToInt32(objnew.filename);
                    var selecteddoc = (from e in hrentity.tblQualiMas where e.intqualiid == docid select e).FirstOrDefault();
                    //get all master document from docmas and check if doc is present in doc table or not
                    var checktable = (from e in hrentity.tblDocDetails where e.fk_empAssid == id1 && e.fk_qualiid == docid && e.intcode == code select e).FirstOrDefault();
                    if (checktable != null)
                    {
                        var selecetdmasemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id1 && e.intcode == code select e).FirstOrDefault();
                        if (selecetdmasemp.bittempcontdetails == false || selecetdmasemp.bitcontdetails == false)
                        {
                            selecetdmasemp.bittempcontdetails = true;
                            selecetdmasemp.bitcontdetails = true;
                            //selecetdmasemp.BitIsUploadCompleted = true;
                            hrentity.SaveChanges();
                        }
                        string selecetddocname = checktable.vchdocname.ToString();
                        TempData["Error"] = "Against" + " " + selecetddocname + " " + "uploaded against current name!";
                        return RedirectToAction("employeeupload", new { id = id1 });
                    }
                    else
                    {

                        //check for pdf null
                        if (objnew.pdfFile != null)
                        {
                            //filename new format filename+datetime+empid+extension                         
                            string empid = id1.ToString();
                            string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                            string extension = Path.GetExtension(objnew.pdfFile.FileName);
                            if (extension != ".pdf")
                            {
                                //ModelState.AddModelError("filename", "Please select .pdf file for upload!");
                                TempData["Error"] = "Please select .pdf file for upload!";
                                return RedirectToAction("employeeupload", new { id = empid });
                            }
                            else
                            {
                                string filename = Path.GetFileNameWithoutExtension(objnew.pdfFile.FileName);
                                string newfilename = filename + datetime + empid + extension;
                                string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + newfilename));
                                //save file in upload folder
                                objnew.pdfFile.SaveAs(path);
                                //Check for is final submission or not
                                var selecetdmasemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id1 select e).FirstOrDefault();
                                if (objnew.BitCompleted == true)
                                {
                                    selecetdmasemp.bittempcontdetails = true;
                                    selecetdmasemp.bitcontdetails = true;
                                    //selecetdmasemp.BitIsUploadCompleted = true;
                                    objdocdetail.fk_empAssid = id1;
                                    objdocdetail.fk_qualiid = docid;
                                    objdocdetail.vchdocname = selecteddoc.vchqname;
                                    objdocdetail.vchdocadd = path.ToString();
                                    objdocdetail.vchfilename = newfilename.ToString();
                                    objdocdetail.vchcreatedby = Session["EmpName"].ToString();
                                    objdocdetail.dtcreated = DateTime.Now;
                                    objdocdetail.vchipused = Session["ipused"].ToString();
                                    objdocdetail.vchhostname = Session["hostname"].ToString();
                                    objdocdetail.intcode = code;
                                    objdocdetail.intyr = Convert.ToInt32(Session["year"]);
                                    objdocdetail.BitIsCompQuali = true;
                                    hrentity.tblDocDetails.Add(objdocdetail);
                                    hrentity.SaveChanges();
                                    TempData["Success"] = "Final upload successfully!";
                                    return RedirectToAction("employeeupload", new { id = empid });
                                }
                                else
                                {
                                    //save path and emp id in database
                                    selecetdmasemp.bittempcontdetails = true;
                                    objdocdetail.fk_empAssid = id1;
                                    objdocdetail.fk_qualiid = docid;
                                    objdocdetail.vchdocname = selecteddoc.vchqname;
                                    objdocdetail.vchdocadd = path.ToString();
                                    objdocdetail.vchfilename = newfilename.ToString();
                                    objdocdetail.vchcreatedby = Session["EmpName"].ToString();
                                    objdocdetail.dtcreated = DateTime.Now;
                                    objdocdetail.vchipused = Session["ipused"].ToString();
                                    objdocdetail.vchhostname = Session["hostname"].ToString();
                                    objdocdetail.intcode = code;
                                    objdocdetail.intyr = Convert.ToInt32(Session["year"]);
                                    objdocdetail.BitIsCompQuali = true;
                                    hrentity.tblDocDetails.Add(objdocdetail);
                                    hrentity.SaveChanges();
                                    TempData["Success"] = "Document upload successfully!";
                                    return RedirectToAction("employeeupload", new { id = empid });
                                }
                            }
                        }
                        else
                        {
                            //null pdf file return error
                            return View();
                        }
                    }
                }
                else
                {
                    //model error return
                    return View();
                }
            }
            else
            {
                //session error return
                return RedirectToAction("EmpLogin");
            }
        }

        public ActionResult ViewEmpDoc()
        {
            if (Session["empid"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int id = Convert.ToInt32(Session["empid"]);
                var selecteddoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompQuali == true && e.intcode == code select e).ToList();
                if (selecteddoc.Count != 0)
                {
                    return View(selecteddoc);
                }
                else
                {
                    TempData["Empty"] = "No document avilable in database!";
                    return View();
                }
            }
            else
            {
                TempData["Error"] = "Invalid employee id!";
                return View();
            }
        }

        public ActionResult ViewEmpDocDetails(int id)
        {
            if (id != 0)
            {
                //int id = Convert.ToInt32(Session["empid"]);
                int code = Convert.ToInt32(Session["id"].ToString());
                var selecteddoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompQuali == true && e.intcode == code select e).ToList();
                if (selecteddoc.Count != 0)
                {
                    return View(selecteddoc);
                }
                else
                {
                    TempData["Empty"] = "No document avilable in database!";
                    return View();
                }
            }
            else
            {
                TempData["Error"] = "Invalid employee id!";
                return View();
            }
        }

        #endregion

        public ActionResult AddEmpDetails(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectempcode = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
            ViewBag.SelectedEmpID = selectempcode.intid.ToString();
            int positionid = selectempcode.fk_PositionId;
            var positionname = (from e in hrentity.tblPositionCategoryMas where e.intid == positionid select e).FirstOrDefault();
            ViewBag.Position = positionname.vchPosCatName.ToString();
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

        [HttpPost]
        public ActionResult AddEmpDetails(tblEmpDetails newdata, FormCollection yvalaform)
        {
            if (ModelState.IsValid)
            {
                int empid = Convert.ToInt32(yvalaform.Get("empIDD"));
                int code = Convert.ToInt32(Session["id"].ToString());
                int yr = Convert.ToInt32(Session["year"].ToString());
                //select master emp
                var selectedemployee = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                if (empid != 0)
                {
                    DateTime DOB = Convert.ToDateTime(newdata.dtDob);
                    newdata.dtDob = DOB;
                    newdata.fk_intempid = empid;
                    if (newdata.vchmaritalststus == "1")
                    {
                        newdata.vchmaritalststus = "Single";
                    }
                    else if (newdata.vchmaritalststus == "2")
                    {
                        newdata.vchmaritalststus = "Married";
                    }
                    else if (newdata.vchmaritalststus == "3")
                    {
                        newdata.vchmaritalststus = "Widowed";
                    }
                    if (newdata.vchsex == "1")
                    {
                        newdata.vchsex = "Male";
                    }
                    else if (newdata.vchsex == "2")
                    {
                        newdata.vchsex = "Female";
                    }
                    if (newdata.vchspouse != null)
                    {
                        newdata.vchspouse = newdata.vchspouse;
                    }
                    else
                    {
                        newdata.vchspouse = "N/A";
                    }
                    newdata.vchcreatedby = Session["EmpName"].ToString();
                    newdata.dtcreated = DateTime.Now;
                    newdata.vchipused = Session["ipused"].ToString();
                    newdata.vchhostname = Session["hostname"].ToString();
                    //Code for Temp Emp Code
                    //string tcode = selectedemployee.intid.ToString();
                    //string code = "T000";
                    //string compcode = code + tcode;
                    //newdata.vchEmpTcode = compcode.ToString();
                    newdata.intcode = code;
                    newdata.intyr = yr;
                    hrentity.tblEmpDetails.Add(newdata);
                    hrentity.SaveChanges();
                    if (newdata.BitCompleted == false)
                    {
                        selectedemployee.bittempperdetails = true;
                        hrentity.SaveChanges();
                    }
                    else if (newdata.BitCompleted == true)
                    {
                        selectedemployee.bittempperdetails = true;
                        selectedemployee.bitperdetails = true;
                        hrentity.SaveChanges();
                    }

                    TempData["Success"] = "Your personal details saved successfully!";
                    return RedirectToAction("EmpDetailActions", new { id = empid });
                }
                else
                {
                    TempData["Error"] = "ID Error generated!";
                    return RedirectToAction("EmpDetailActions", new { id = empid });
                }
            }
            else
            {
                return RedirectToAction("AddEmpDetails");
            }
        }

        public ActionResult EditDetail(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var selectedemp = (from e in hrentity.tblEmpDetails where e.fk_intempid == id && e.intcode == code select e).FirstOrDefault();
            if (selectedemp != null)
            {
                var selectempcode = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                int positionid = selectempcode.fk_PositionId;
                ViewBag.SelectedEmpID = selectempcode.intid.ToString();
                var positionname = (from e in hrentity.tblPositionCategoryMas where e.intid == positionid select e).FirstOrDefault();
                ViewBag.Position = positionname.vchPosCatName.ToString();
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
                                      Value = "3"}
                              };
                ViewBag.MaritalStatus = new SelectList(Maritalstatus, "Value", "Text");
                IEnumerable<tblTitleMas> selecttit = (from e in hrentity.tblTitleMas select e).ToList();
                List<SelectListItem> Title = new List<SelectListItem>();
                foreach (var tit in selecttit)
                {
                    SelectListItem selectListItem = new SelectListItem()
                    {
                        Value = tit.intid.ToString(),
                        Text = tit.vchname.ToString(),
                        Selected = Convert.ToBoolean(tit.IsSelected)
                    };
                    Title.Add(selectListItem);
                }
                ViewBag.Title1 = new SelectList(Title, "Value", "Text");
                return View(selectedemp);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditDetail(tblEmpDetails objupdate, FormCollection formcol)
        {
            int empid = Convert.ToInt32(formcol.Get("empIDD"));
            if (ModelState.IsValid)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                var check = formcol.GetValue("chkpaddress");
                var seleceted = (from e in hrentity.tblEmpDetails where e.fk_intempid == empid && e.intcode == code select e).FirstOrDefault();
                if (seleceted != null)
                {
                    seleceted.fk_titid = objupdate.fk_titid;
                    seleceted.vchfname = objupdate.vchfname;
                    if (objupdate.vchmname != null)
                    {
                        seleceted.vchmname = objupdate.vchmname;
                    }
                    seleceted.vchlname = objupdate.vchlname;
                    if (objupdate.vchmaritalststus == "1")
                    {
                        seleceted.vchmaritalststus = "Single";
                    }
                    else if (objupdate.vchmaritalststus == "2")
                    {
                        seleceted.vchmaritalststus = "Married";
                    }
                    else if (objupdate.vchmaritalststus == "3")
                    {
                        objupdate.vchmaritalststus = "Widowed";
                    }
                    if (objupdate.vchsex == "1")
                    {
                        seleceted.vchsex = "Male";
                    }
                    else if (objupdate.vchsex == "2")
                    {
                        seleceted.vchsex = "Female";
                    }
                    seleceted.vchFatherName = objupdate.vchFatherName;
                    seleceted.vchmothername = objupdate.vchmothername;
                    if (objupdate.vchspouse != null)
                    {
                        seleceted.vchspouse = objupdate.vchspouse;
                    }
                    else
                    {
                        seleceted.vchspouse = "N/A";
                    }
                    seleceted.vchNominee = seleceted.vchNominee;
                    seleceted.vchRelation = objupdate.vchRelation;
                    seleceted.vchtaddress = objupdate.vchtaddress;
                    seleceted.vchtcity = objupdate.vchtcity;
                    seleceted.vchtstate = objupdate.vchtstate;
                    seleceted.vchtmobile = objupdate.vchtmobile;
                    seleceted.vchpaddress = objupdate.vchpaddress;
                    seleceted.vchpcity = objupdate.vchpcity;
                    seleceted.vchpstate = objupdate.vchpstate;
                    seleceted.vchpmobile = objupdate.vchpmobile;
                    seleceted.vchupdatedby = Session["EmpName"].ToString();
                    seleceted.dtupdated = DateTime.Now;
                    seleceted.vchupdatedip = Session["ipused"].ToString();
                    seleceted.vchupdatedhost = Session["hostname"].ToString();
                    hrentity.SaveChanges();
                    //select mas table for update data
                    var empmas = (from e in hrentity.tblEmpAssesmentMas where e.intid == empid && e.intcode == code select e).FirstOrDefault();
                    if (objupdate.BitCompleted == false)
                    {
                        empmas.bittempperdetails = true;
                        hrentity.SaveChanges();
                    }
                    else if (objupdate.BitCompleted == true)
                    {
                        empmas.bittempperdetails = true;
                        empmas.bitperdetails = true;
                        hrentity.SaveChanges();
                        TempData["Success"] = "Final submission personal details updated successfully!";
                        return RedirectToAction("EmpDetailActions", new { id = empid });
                    }
                    TempData["Success"] = "Personal details updated successfully!";
                    return RedirectToAction("EmpDetailActions", new { id = empid });
                }
                else
                {
                    TempData["Error"] = "Selected id not foung in current database please check it again!";
                    return RedirectToAction("EmpDetailActions", new { id = empid });
                }

            }
            else
            {
                TempData["Error"] = "Model error generated please contact to administrator";
                return RedirectToAction("EmpDetailActions", new { id = empid });
            }


        }

        public ActionResult ViewDetails(int id)
        {
            int code = Convert.ToInt32(Session["id"].ToString());
            var detail = (from e in hrentity.tblEmpDetails where e.fk_intempid == id && e.intcode == code select e).FirstOrDefault();
            if (detail != null)
            {
                return View(detail);
            }
            else
            {
                TempData["Error"] = "Error generated please check it again!";
                return RedirectToAction("EmpDetailActions", new { id = id });
            }

        }

        public ActionResult ProfilePic(int id)
        {
            if (id != 0)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                ProfileViewModel objmodel = new ProfileViewModel();
                var selectedemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id && e.intcode == code select e).FirstOrDefault();
                ViewBag.Mobile = selectedemp.vchMobile.ToString();
                ViewBag.Empname = selectedemp.vchName.ToString();
                ViewBag.EmpProfileName = "Profile Picture";
                ViewBag.EmpID = id.ToString();
                objmodel.BitIsCompleted = Convert.ToBoolean(selectedemp.bitProfileComplete);
                return View(objmodel);

            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ProfilePic(ProfileViewModel objpic, FormCollection fromform)
        {
            if (ModelState.IsValid)
            {
                //docdetails table object
                tblDocDetails objdocdetail = new tblDocDetails();
                int code = Convert.ToInt32(Session["id"].ToString());
                int yr = Convert.ToInt32(Session["year"].ToString());
                //get emp id
                int id1 = Convert.ToInt32(fromform.Get("hdempid"));
                string selectedDoc = objpic.picname.ToString();
                //int id = Convert.ToInt32(newdoc.empid);
                //get all emp id qualifications
                //int docid = Convert.ToInt32(objpic.profilepic);
                //var selecteddoc = (from e in hrentity.tblQualiMas where e.intqualiid == docid select e).FirstOrDefault();
                //get all master document from docmas and check if doc is present in doc table or not
                var checktable = (from e in hrentity.tblDocDetails where e.fk_empAssid == id1 && e.BitIsProfilePic == true && e.intcode == code select e).FirstOrDefault();
                if (checktable != null)
                {
                    string selecetddocname = checktable.vchdocname.ToString();
                    TempData["Error"] = "Against" + " " + selecetddocname + " " + "uploaded against current name!";
                    return RedirectToAction("ProfilePic", new { id = id1 });
                }
                else
                {
                    //check for pdf null
                    if (objpic.profilepic != null)
                    {
                        //filename new format filename+datetime+empid+extension                         
                        string empid = id1.ToString();
                        string datetime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
                        string extension = Path.GetExtension(objpic.profilepic.FileName);
                        if (extension != ".jpg")
                        {
                            //ModelState.AddModelError("filename", "Please select .jpg file for upload!");
                            TempData["Error"] = "Please select .jpg file for upload!";
                            return RedirectToAction("ProfilePic", new { id = empid });
                        }
                        else
                        {
                            string filename = Path.GetFileNameWithoutExtension(objpic.profilepic.FileName);
                            string newfilename = filename + datetime + empid + extension;
                            string path = Path.Combine(Server.MapPath("~/Content/Uploads/" + newfilename));
                            //save file in upload folder
                            objpic.profilepic.SaveAs(path);
                            var selecetdmasemp = (from e in hrentity.tblEmpAssesmentMas where e.intid == id1 select e).FirstOrDefault();
                            selecetdmasemp.bitProfileComplete = true;
                            selecetdmasemp.BitIsUploadCompleted = true;
                            objdocdetail.fk_empAssid = id1;
                            objdocdetail.vchdocname = selectedDoc;
                            objdocdetail.vchdocadd = path.ToString();
                            objdocdetail.vchfilename = newfilename.ToString();
                            objdocdetail.vchcreatedby = Session["EmpName"].ToString();
                            objdocdetail.dtcreated = DateTime.Now;
                            objdocdetail.vchipused = Session["ipused"].ToString();
                            objdocdetail.vchhostname = Session["hostname"].ToString();
                            objdocdetail.BitIsProfilePic = true;
                            objdocdetail.intcode = code;
                            objdocdetail.intyr = yr;
                            hrentity.tblDocDetails.Add(objdocdetail);
                            hrentity.SaveChanges();
                            TempData["Success"] = "Photograph upload successfully!";
                            return RedirectToAction("EmpDetailActions", new { id = empid });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Please select profile file for upload!";
                        return RedirectToAction("ProfilePic", new { id = id1 });
                    }
                }
            }
            else
            {
                int id1 = Convert.ToInt32(fromform.Get("hdempid"));
                TempData["Error"] = "Model Error Generated!";
                return RedirectToAction("ProfilePic", new { id = id1 });
            }
        }

        public ActionResult viewProfile()
        {
            if (Session["empid"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                int empid = Convert.ToInt32(Session["empid"].ToString());
                var selecteddoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == empid && e.BitIsProfilePic == true && e.intcode == code select e).ToList();
                if (selecteddoc.Count != 0)
                {
                    return View(selecteddoc);
                }
                else
                {
                    TempData["Empty"] = "No document avilable in database!";
                    return View();
                }
            }
            else
            {
                TempData["Empty"] = "Invalid employee id!";
                return View();
            }
        }

        //New Work for employee dashboard
        [AllowAnonymous]
        public ActionResult Employee()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> genOTP(string mobile, string branch)
        {
            if (mobile != null && branch != null)
            {
                int code = Convert.ToInt32(branch);
                var selected = (from e in hrentity.tblEmpLoginUser where e.vchmobile == mobile && e.intcode == code select e).FirstOrDefault();
                Session["MobileNo"] = mobile.ToString();
                if (selected != null)
                {
                    string formatting = "0000"; //Will pad out to four digits if under 1000   
                    int _min = 0;
                    int _max = 9999;
                    Random randomNumber = new Random();
                    var randomNumberString = randomNumber.Next(_min, _max).ToString(formatting);
                    //int newcode = Convert.ToInt32(randomNumberString);
                    selected.vchOTP = randomNumberString.ToString();
                    hrentity.SaveChanges();
                    string mob = selected.vchmobile.ToString();
                    string OTP = randomNumberString.ToString();
                    var uri = "http://164.52.195.161/API/SendMsg.aspx?uname=20170839&pass=Indus2017&send=HOSPIN&dest=" + mob + "&msg=Please login HRMS INDUS using OTP : " + OTP + "&priority=1";
                    //var uri = AppSettings.SMSApiUrl;
                    //var apiKey = AppSettings.SMSApiKey;
                    if (uri != null)
                    {
                        try
                        {
                            #region Old api
                            HttpWebRequest createrequest = (HttpWebRequest)WebRequest.Create(uri);
                            // getting response of sms
                            HttpWebResponse myResp = (HttpWebResponse)createrequest.GetResponse();
                            System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                            string responseString = _responseStreamReader.ReadToEnd();
                            _responseStreamReader.Close();
                            myResp.Close();
                            #endregion

                            #region new api
                            //var requestData = new
                            //{
                            //    sender = "HOSPIN",
                            //    templateName = "OTP",
                            //    smsReciever = new[]
                            //       {
                            //        new {
                            //            mobileNo = mob,
                            //            templateParams = OTP
                            //        }
                            //    }
                            //};
                            //using (var client = new HttpClient())
                            //{
                            //    client.DefaultRequestHeaders.Add("apiKey", apiKey);  // Add header if required

                            //    var json = JsonConvert.SerializeObject(requestData);
                            //    var content = new StringContent(json, Encoding.UTF8, "application/json");
                            //    var response = await client.PostAsync(uri, content);
                            //    if (response.IsSuccessStatusCode)
                            //    {
                            //        //Session["OTP"] = otp;  // Store OTP for verification
                            //        return Json(new { status = "Success", message = "OTP Sent Successfully" });
                            //    }
                            //    else
                            //    {
                            //        return Json(new { status = "Failed", message = "Failed to Send OTP" });
                            //    }
                            //}
                            #endregion
                        }
                        catch
                        {

                        }
                        return RedirectToAction("Employee");
                    }
                    else
                    {
                        var msg = "OTP send successfully on your mobile.";
                        return Json(new { otp = msg }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    TempData["Error"] = "Mobile number does not registered with us!";
                    return RedirectToAction("Employee");
                }
            }
            else
            {
                //TempData["Error"] = "Invalid mobile number contact to administrator!";
                return View();
            }
        }

        [HttpPost]
        public ActionResult Employee(EmployeeLoginModelView objmodel, IndusCompanies objcompany)
        {
            if (ModelState.IsValid)
            {
                //match user credentials match or not
                string mobile = objmodel.vchmobile;
                string OTP = objmodel.OTP.ToString();
                int id = objcompany.intPK;
                Session["id"] = id.ToString();
                var selectedEmp = (from e in hrentity.tblEmpLoginUser where e.vchmobile == mobile && e.vchOTP == OTP && e.intcode == id select e).FirstOrDefault();
                if (selectedEmp != null)
                {
                    //declare all session variable for emplyee here
                    selectedEmp.dtlastlogin = DateTime.Now;
                    hrentity.SaveChanges();
                    var getFullDetail = (from e in hrentity.tblEmpAssesmentMas where e.intid == selectedEmp.fk_intEmpID select e).FirstOrDefault(); //&&e.bittempstatusactive == true
                    if (getFullDetail != null)
                    {
                        // check it active employee or partial authorised or is left employee or is an consultant
                        if (getFullDetail.bittempstatusactive == true || getFullDetail.bitIsPartialAuthorised == true || getFullDetail.bitIsConsultant == true)
                        {
                            var getbranch = (from e in gpentity.IndusCompanies where e.intPK == id select e).FirstOrDefault();
                            if (getbranch != null)
                            {
                                //calculate DOJ to till now days
                                DateTime DOJ = Convert.ToDateTime(getFullDetail.dtDOJ);
                                DateTime Aaj = DateTime.Now;
                                var Totaldays = (Aaj - DOJ).TotalDays;
                                Session["RequiredDays"] = "180";
                                Session["TotalDays"] = (Convert.ToInt32(Totaldays)).ToString();
                                if (selectedEmp.vchProfileName != null)
                                {
                                    Session["UserProfilePic"] = selectedEmp.vchProfileName.ToString();
                                }
                                Session["BrnachCode"] = getbranch.intPK.ToString();
                                Session["BranchName"] = getbranch.descript.ToString();
                                Session["Empid"] = getFullDetail.intid.ToString();
                                Session["EmpDeptID"] = getFullDetail.fk_intdeptid.ToString();
                                Session["UserId"] = selectedEmp.intid.ToString();
                                Session["IsConsultant"] = getFullDetail.bitIsConsultant.ToString();
                                if (getbranch.intPK == 2)
                                {
                                    Session["branchCodeName"] = "HY";
                                }
                                if (getbranch.intPK == 3)
                                {
                                    Session["branchCodeName"] = "IS";
                                }
                                if (getbranch.intPK == 4)
                                {
                                    Session["branchCodeName"] = "IH";
                                }
                                if (getbranch.intPK == 14)
                                {
                                    Session["branchCodeName"] = "II";
                                }
                                if (getbranch.intPK == 15)
                                {
                                    Session["branchCodeName"] = "IF";
                                }
                                if (getbranch.intPK == 16)
                                {
                                    Session["branchCodeName"] = "KH";
                                }
                                if (getbranch.intPK == 21)
                                {
                                    Session["branchCodeName"] = "MH";
                                }
                                if (getbranch.intPK == 22)
                                {
                                    Session["branchCodeName"] = "HS";
                                }
                                if (getbranch.intPK == 23)
                                {
                                    Session["branchCodeName"] = "MY";
                                }
                                if (getbranch.intPK == 24)
                                {
                                    Session["branchCodeName"] = "KS";
                                }
                                if (getbranch.intPK == 25)
                                {
                                    Session["branchCodeName"] = "BH";
                                }
                                //get host name
                                string strHostName = System.Net.Dns.GetHostName();
                                System.Net.IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
                                Session["hostname"] = strHostName.ToString();
                                //get ip address
                                System.Net.IPAddress[] addr = ipEntry.AddressList;
                                string ip = addr[1].ToString();
                                Session["ipused"] = strHostName.ToString();
                                //get departmental HOD
                                int code = getbranch.intPK;
                                if (getFullDetail.bitIsConsultant != true)
                                {
                                    var getHOD = (from e in hrentity.tblUserMaster where e.fk_intDeptid == getFullDetail.fk_intdeptid && e.bitISHOD == true && e.bitActive == true && e.intcode == code select e).FirstOrDefault();
                                    if (getHOD != null)
                                    {
                                        Session["ISHOD"] = "True";
                                        Session["HOD_ID"] = getHOD.intid.ToString();
                                    }
                                    else
                                    {
                                        Session["ISHOD"] = "False";
                                        Session["HOD_ID"] = 0.ToString();
                                    }
                                }
                                else
                                {
                                    var getVP = (from e in hrentity.tblUserMaster where e.fk_intDesignationid == 3175 && e.intcode == code select e).FirstOrDefault();
                                    if (getVP != null)
                                    {
                                        Session["ISVP"] = "True";
                                        Session["VPID"] = getVP.intid;
                                    }
                                }
                                //get selected Branch HR ID
                                var getHrID = (from e in hrentity.tblUserMaster where e.fk_intDesignationid == 12 && e.intcode == id && e.bitActive == true select e).FirstOrDefault();
                                if (getHrID != null)
                                {
                                    Session["BHR_UID"] = getHrID.intid.ToString();
                                }
                            }
                            Session["Ename"] = getFullDetail.vchName.ToString();
                            // Check if employee exists in DNB student table
                            bool isDnbStudent = hrentity.TblDnbStudent.Any(x => x.fk_EmployeeId == selectedEmp.fk_intEmpID);
                            // Pass info to the view                         
                            Session["IsDnbStudent"] = isDnbStudent.ToString();
                            return RedirectToAction("EmpDashboard", "NewEmployeeLogin");
                        }
                        else
                        {
                            ModelState.AddModelError("vchmobile", "User are not an active employee, contact to admin!");
                            return View("Employee");
                        }
                    }
                    ModelState.AddModelError("vchmobile", "Your detail not found in database, please contact to admin!");
                    return View("Employee");
                }
                else
                {
                    ModelState.AddModelError("vchmobile", "User not found, contact to admin!");
                    return View("Employee");
                }
            }
            else
            {
                ModelState.AddModelError("vchmobile", "Model error generated, contact to admin!");
                return View("Employee");
            }
        }

        public ActionResult EmpDashboard()
        {
            if (Session["Ename"] != null)
            {
                if (Session["Doctor"] != null)
                {
                    ViewBag.ISDoctor = Session["Doctor"].ToString();
                }
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError");
            }
        }

        [AllowAnonymous]
        public ActionResult _SessionError()
        {
            return View();
        }

        public ActionResult EmpUserProfile()
        {
            if (Session["Ename"] != null)
            {
                int uid = Convert.ToInt32(Session["UserId"].ToString());
                int bcode = Convert.ToInt32(Session["BrnachCode"].ToString());
                if (uid != 0 && bcode != 0)
                {
                    HRMEntities hrentity = new HRMEntities();
                    ChangeProfile objpass = new ChangeProfile();
                    var selectedUid = (from e in hrentity.tblEmpLoginUser where e.intid == uid && e.intcode == bcode select e).FirstOrDefault();
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
        public ActionResult EmpUserProfile(ChangeProfile objup, string CroppedImageData)
        {
            if (Session["Ename"] != null)
            {
                //check for pdf null
                if (objup.filepath != null)
                {
                    int code = Convert.ToInt32(Session["BrnachCode"].ToString());
                    //get user seleceted
                    HRMEntities hrentity = new HRMEntities();
                    var getUserDetail = (from e in hrentity.tblEmpLoginUser where e.intid == objup.id && e.intcode == code select e).FirstOrDefault();
                    //delete old file
                    if (getUserDetail.bitProfileComplete == true)
                    {
                        //Success when already uploaded
                        System.IO.File.Delete(getUserDetail.vchProfilePath);
                    }
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

        #region profile view detail

        public ActionResult ProfileDetail()
        {
            if (Session["Ename"] != null || Session["UserId"] != null)
            {
                int uid = Convert.ToInt32(Session["UserId"].ToString());
                int bcode = Convert.ToInt32(Session["BrnachCode"].ToString());
                if (uid != 0 && bcode != 0)
                {
                    HRMEntities hrentity = new HRMEntities();
                    ChangeProfile objpass = new ChangeProfile();
                    var selectedUid = (from e in hrentity.tblEmpLoginUser where e.intid == uid && e.intcode == bcode select e).FirstOrDefault();
                    if (selectedUid != null)
                    {
                        objpass.id = Convert.ToInt32(selectedUid.fk_intEmpID);
                        //check emp full detail and photos is uploaded
                        var getEMpdetail = (from e in hrentity.tblEmpDetails where e.fk_intempid == objpass.id select e).FirstOrDefault();
                        if (getEMpdetail == null)
                        {
                            TempData["Error"] = "Your full detail not found contact to your Manager HR!";
                        }

                        //MasDetail(selectedUid.fk_intEmpID);
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

        public ActionResult MasDetail(int id)
        {
            if (Session["UserId"] != null)
            {
                var personaldetails = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                //get detail
                var detail = (from e in hrentity.tblEmpDetails where e.fk_intempid == id select e).FirstOrDefault();
                if (personaldetails != null)
                {
                    if (detail != null)
                    {
                        ViewBag.Qualification = detail.vchQualifications;
                    }
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

        public ActionResult PictureDetail(int id)
        {
            if (Session["UserId"] != null)
            {
                if (id != 0)
                {
                    string isHrAdmin = string.Empty;
                    string isAuthorizer = string.Empty;
                    string isMainAdmin = string.Empty;
                    if (Session["HrAdmin"] != null)
                    {
                        isHrAdmin = Session["HrAdmin"].ToString();
                    }
                    if (Session["AllowAuthorization"] != null)
                    {
                        isAuthorizer = Session["AllowAuthorization"].ToString();
                    }
                    if (Session["MainAdmin"] != null)
                    {
                        isMainAdmin = Session["MainAdmin"].ToString();
                    }
                    var docdetails = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == id && e.bitIsProfile == true select e).FirstOrDefault();
                    var selectedimage = (from e in hrentity.tblEmpAssesmentMas where e.intid == id select e).FirstOrDefault();
                    if (docdetails != null)
                    {
                        ViewBag.ImgEdit = "Not-Allowed";
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

        public ActionResult PartialEmpDetail(int id)
        {

            if (Session["UserId"] != null)
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

        public ActionResult PCompDoc(int id)
        {
            if (Session["UserId"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                List<SelectListItem> mylist = new List<SelectListItem>();
                var compdoc = (from e in hrentity.tblDocDetails where e.fk_empAssid == id && e.BitIsCompDoc == true select e).ToList();
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

        public ActionResult ConsultantDoc(int id)
        {
            if (Session["UserId"] != null)
            {
                int code = Convert.ToInt32(Session["id"].ToString());
                List<SelectListItem> mylist = new List<SelectListItem>();
                var compdoc = (from e in hrentity.tblConsultantDocDetail where e.fk_DoctorID == id select e).ToList();
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
                    ViewBag.ConsultantDoc = mylist;
                }
                else
                {
                    ViewBag.Empty = "Document not found please chect it!";
                }
                return View();

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        [HttpPost]
        public JsonResult UpdateQualification(int employeeId, string quali)
        {

            var getdetail = (from e in hrentity.tblEmpDetails where e.fk_intempid == employeeId select e).FirstOrDefault();
            if (getdetail != null)
            {
                getdetail.vchQualifications = quali.ToString();
                hrentity.SaveChanges();
                return Json(new { success = true, message = "Qualification updated successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Employee detail not found." });
            }
        }

        #endregion

        #region Consultant Salary slips request and prints

        public ActionResult IndexSlips()
        {
            if (Session["UserId"] != null && Session["empid"] != null)
            {
                int empid = Convert.ToInt32(Session["empid"]);

                // 1. Use .Include() if using Entity Framework to ensure the Employee/Doctor name is loaded
                // 2. Fetch the list
                var selected = hrentity.tblConsultantSlips
                                       .Where(e => e.fk_DoctorId == empid)
                                       .ToList();

                ViewBag.ID = empid; // Set this regardless of count so the "New Request" button works

                if (selected.Count == 0)
                {
                    TempData["Empty"] = "0 records found in database!";
                    // Return an empty list so the View doesn't break
                    return View(new List<HRM.Models.tblConsultantSlips>());
                }

                return View(selected);
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }


        public ActionResult RequestSlip(int id)
        {
            ConsultantSlipRequestModel objslip = new ConsultantSlipRequestModel();
            var getAllQurater = (from e in hrentity.tblQuraterMas select e).ToList();
            var getAllMonth = (from e in hrentity.tblMonthMaster select e).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Select year quarter", Value = "Select year quarter" });
            foreach (var month in getAllQurater)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = month.vchMonth,
                    Value = month.intid.ToString()
                };
                list.Add(selectListItem);
            }
            ViewBag.AllQurater = new SelectList(list, "Text", "Value");
            List<SelectListItem> Months = new List<SelectListItem>();
            Months.Add(new SelectListItem { Text = "Select month", Value = "0" });
            foreach (var month in getAllMonth)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = month.Month,
                    Value = month.intid.ToString()
                };
                Months.Add(selectListItem);
            }
            objslip.fk_empid = id;
            ViewBag.AllMonths = new SelectList(Months, "Text", "Value");
            return PartialView("_PartialRequestNew", objslip);
            //model error resolve
        }

        [HttpPost]
        public ActionResult RequestSlip(ConsultantSlipRequestModel objslip)
        {
            if (Session["UserId"] != null)
            {
                int code = Convert.ToInt32(Session["BranchCode"].ToString());
                tblConsultantSlips objdetail = new tblConsultantSlips();
                int checkYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                var getEmpMaster = (from e in hrentity.tblEmpAssesmentMas where e.intid == objslip.fk_empid select e).FirstOrDefault();
                if (objslip.selectionType == "Quarter")
                {
                    var getMasterMonth = (from e in hrentity.tblQuraterMas where e.intid == objslip.fk_quarterid select e).FirstOrDefault();
                    //check slip already exists or not

                    var checlslip = (from e in hrentity.tblConsultantSlips where e.fk_Quarter == objslip.fk_quarterid && e.intYear == checkYear && e.fk_DoctorId == objslip.fk_empid select e).FirstOrDefault();
                    if (checlslip == null)
                    {
                        //for ref number
                        string branchCode = Session["branchCodeName"].ToString();
                        string CharHR = "HR";
                        var selectedDept = (from e in hrentity.tblDeptMas where e.intid == getEmpMaster.fk_intdeptid select e).FirstOrDefault();
                        string deptCode = selectedDept.vchdepCode;
                        string CodeYear = DateTime.Now.ToString("yy");
                        string CertificateCode = "SS";
                        int certificateCurrentcount = 0;
                        int year = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                        var getCode = (from e in hrentity.tblLetterNumberMas where e.intcode == code && e.intYear == year select e).FirstOrDefault();
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

                                //Format = Branch/HR/EC/EmpDpt/yr/series
                                string Ref_Code = branchCode + "/" + CharHR + "/" + CertificateCode + "/" + deptCode + "/" + CodeYear + "/" + finalnumber;
                                objdetail.vchCreatedBy = Ref_Code;
                                objdetail.UNid = Guid.NewGuid();
                                objdetail.fk_DoctorId = objslip.fk_empid;
                                objdetail.fk_Quarter = Convert.ToInt32(objslip.fk_quarterid);
                                objdetail.fk_MonthNames = getMasterMonth.vchMonth;
                                objdetail.dtRequest = DateTime.Now;
                                objdetail.vchRequestedBy = Session["Ename"].ToString();
                                objdetail.dtCreated = DateTime.Now;
                                objdetail.vchRefNumber = Ref_Code;
                                objdetail.bitIsRequest = true;
                                objdetail.bitIsPending = true;
                                objdetail.bitIsQuraterRequest = true;
                                objdetail.intYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                                objdetail.vchCreatedBy = Session["Ename"].ToString();
                                objdetail.vchBranchName = Session["BranchName"].ToString();
                                objdetail.intcode = code;
                                hrentity.tblConsultantSlips.Add(objdetail);
                                hrentity.SaveChanges();
                                TempData["Success"] = "Request sent successfully!";
                                return RedirectToAction("IndexSlips", "NewEmployeeLogin", new { id = objslip.fk_empid });
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
                                return RedirectToAction("IndexSlips", "NewEmployeeLogin", new { id = objslip.fk_empid });
                            }
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Selected qurater request already sent/completed so check it!";
                        return RedirectToAction("IndexSlips");
                    }
                }
                if (objslip.selectionType == "MonthRange")
                {
                    //get from month
                    int fromMonth = Convert.ToInt32(objslip.fromMonth);
                    int toMonth = Convert.ToInt32(objslip.toMonth);
                    var checlslip = (from e in hrentity.tblConsultantSlips where e.intFromMonth == fromMonth && e.intToMonth == toMonth && e.intYear == checkYear select e).FirstOrDefault();
                    if (checlslip == null)
                    {
                        //for ref number
                        string branchCode = Session["branchCodeName"].ToString();
                        string CharHR = "HR";
                        var selectedDept = (from e in hrentity.tblDeptMas where e.intid == getEmpMaster.fk_intdeptid select e).FirstOrDefault();
                        string deptCode = selectedDept.vchdepCode;
                        string CodeYear = DateTime.Now.ToString("yy");
                        string CertificateCode = "SS";
                        int certificateCurrentcount = 0;
                        int year = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                        var getCode = (from e in hrentity.tblLetterNumberMas where e.intcode == code && e.intYear == year select e).FirstOrDefault();
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

                                //find solution for qurater mas foregion key faliure
                                //Format = Branch/HR/EC/EmpDpt/yr/series
                                string Ref_Code = branchCode + "/" + CharHR + "/" + CertificateCode + "/" + deptCode + "/" + CodeYear + "/" + finalnumber;
                                objdetail.vchCreatedBy = Ref_Code;
                                objdetail.UNid = Guid.NewGuid();
                                objdetail.fk_DoctorId = objslip.fk_empid;
                                objdetail.intFromMonth = fromMonth;
                                objdetail.intToMonth = toMonth;
                                objdetail.fk_MonthNames = "N/A";
                                objdetail.dtRequest = DateTime.Now;
                                objdetail.vchRequestedBy = Session["Ename"].ToString();
                                objdetail.dtCreated = DateTime.Now;
                                objdetail.vchRefNumber = Ref_Code;
                                objdetail.bitIsRequest = true;
                                objdetail.bitIsPending = true;
                                objdetail.bitIsMonthRequest = true;
                                objdetail.intYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                                objdetail.vchCreatedBy = Session["Ename"].ToString();
                                objdetail.vchBranchName = Session["BranchName"].ToString();
                                objdetail.intcode = code;
                                hrentity.tblConsultantSlips.Add(objdetail);
                                hrentity.SaveChanges();
                                TempData["Success"] = "Request sent successfully!";
                                return RedirectToAction("IndexSlips", "NewEmployeeLogin", new { id = objslip.fk_empid });
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
                                return RedirectToAction("IndexSlips", "NewEmployeeLogin", new { id = objslip.fk_empid });
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Code master series not found contact to administrator!";
                            return RedirectToAction("IndexSlips", "NewEmployeeLogin", new { id = objslip.fk_empid });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Same request already present/completed in database!";
                        return RedirectToAction("IndexSlips", "NewEmployeeLogin", new { id = objslip.fk_empid });
                    }
                }

                return View();

            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult _PartialRequestNew()
        {
            if (Session["UserId"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        public ActionResult PrintSlip(Guid id)
        {
            if (Session["UserId"] != null)
            {
                var getSlip = (from e in hrentity.spGetConsultantSlip(id) select e).ToList();
                var getslipDetails = (from e in hrentity.spGetConsultantSlipDetail(id) select e).ToList();
                if (getSlip != null && getslipDetails.Count() != 0)
                {
                    LocalReport lr = new LocalReport();
                    string filepath = String.Empty;
                    HttpClient _client = new HttpClient();

                    //get path
                    filepath = Path.Combine(Server.MapPath("~/Content/Report"), ("ConsultantSlip.rdl"));
                    //open streams
                    using (var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        lr.LoadReportDefinition(filestream);
                        lr.DataSources.Add(new ReportDataSource(name: "DataSet1", getSlip));
                        lr.DataSources.Add(new ReportDataSource(name: "DataSet2", getslipDetails));
                        ReportParameter ptr = new ReportParameter("UID", id.ToString());
                        lr.SetParameters(ptr);
                        byte[] pdfData = lr.Render("PDF");
                        return File(pdfData, contentType: "Application/pdf");
                    }
                }
                else
                {
                    TempData["Error"] = "Slip detail not found please check it again!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region KRA View
        public ActionResult IndexKRA()
        {
            // 1. Session Validation
            if (Session["UserId"] == null || Session["empid"] == null)
            {
                return RedirectToAction("_SessionError1", "Home");
            }

            int empid = Convert.ToInt32(Session["empid"]);

            // 2. Fetch all uploaded documents for this employee
            var selectedDocs = hrentity.tblKRADocDetail
                                       .Where(e => e.fk_EmpID == empid)
                                       .ToList();

            if (!selectedDocs.Any())
            {
                TempData["Empty"] = "0 records found in database!";
                return View(new List<tblMonthMaster>());
            }

            // 3. Extract unique years from document names to handle previous year data
            var distinctYears = selectedDocs
                .Select(d => d.vchDocMasName.Split('-').Last())
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            var monthMasters = hrentity.tblMonthMaster.ToList();
            List<tblMonthMaster> finalModel = new List<tblMonthMaster>();

            // 4. Build the model by matching months and years
            foreach (var year in distinctYears)
            {
                foreach (var mMaster in monthMasters)
                {
                    string lookupKey = $"{mMaster.Month}-{year}";
                    var doc = selectedDocs.FirstOrDefault(d => d.vchDocMasName == lookupKey);

                    if (doc != null)
                    {
                        var entry = new tblMonthMaster
                        {
                            Month = lookupKey,
                            bitIsUploaded = true,
                            vchTempDocAddress = doc.vchFileAddress,
                            vchTempFileName = doc.vchFileName,
                            vchTempDocMasName = doc.vchDocMasName,
                            dtTempUploaded = doc.dtUpload,
                            decMaxScore = doc.decMaxScore,
                            decTempScore = doc.decFinalScore,
                            intTempEmpID = empid
                        };

                        // Fetch cohort score
                        var getCohort = hrentity.spGetCohort(doc.vchDocMasName, empid, Convert.ToInt32(doc.fk_intDeptid)).FirstOrDefault();
                        entry.decCohortScore = getCohort?.ContributionPercentage ?? 0;

                        finalModel.Add(entry);
                    }
                }
            }

            // 5. CRITICAL: Sort chronologically for the Trend Line Chart
            var sortedModel = finalModel
                .OrderBy(m => DateTime.ParseExact(m.Month, "MMMM-yyyy", CultureInfo.InvariantCulture))
                .ToList();

            return View(sortedModel);
        }

        //public ActionResult IndexKRA()
        //{
        //    if (Session["UserId"] != null)
        //    {
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
        //        int empid = Convert.ToInt32(Session["empid"].ToString());
        //        var selected = (from e in hrentity.tblKRADocDetail where e.fk_EmpID == empid select e).ToList();
        //        if (selected.Count() != 0)
        //        {
        //            foreach (var masterName in NameWithYear)
        //            {
        //                foreach (var up in selected)
        //                {
        //                    if (masterName.Month == up.vchDocMasName)
        //                    {
        //                        masterName.bitIsUploaded = true;
        //                        masterName.vchTempDocAddress = up.vchFileAddress;
        //                        masterName.vchTempFileName = up.vchFileName;
        //                        masterName.vchTempDocMasName = up.vchDocMasName;
        //                        masterName.dtTempUploaded = up.dtUpload;
        //                        masterName.decMaxScore = up.decMaxScore;
        //                        masterName.decTempScore = up.decFinalScore;
        //                        spGetCohort_Result getCohort = (from e in this.hrentity.spGetCohort(up.vchDocMasName.ToString(), new int?(Convert.ToInt32(up.fk_EmpID)), new int?(Convert.ToInt32(up.fk_intDeptid)))
        //                                                        select e).FirstOrDefault<spGetCohort_Result>();
        //                        if (getCohort != null)
        //                        {
        //                            masterName.decCohortScore = getCohort.ContributionPercentage;
        //                        }
        //                        else
        //                        {
        //                            masterName.decCohortScore = 0;
        //                        }
        //                        masterName.intTempEmpID = empid;
        //                        objMonth.Add(masterName);
        //                    }
        //                }

        //            }
        //            return View(objMonth);
        //        }
        //        else
        //        {
        //            TempData["Empty"] = "0 records found in database!";
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("_SessionError1", "Home");
        //    }
        //}

        #endregion

        #region Consultant Ledger detail
        public ActionResult LedgerIndex()
        {
            if (Session["UserId"] != null)
            {
                int empid = Convert.ToInt32(Session["empid"].ToString());
                var selected = (from e in hrentity.tblConsultantLedgerDetail where e.fk_EmpID == empid select e).ToList();
                if (selected.Count() != 0)
                {
                    return View(selected);
                }
                else
                {
                    TempData["Empty"] = "0 records found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region Consultant TDS detail
        public ActionResult IndexTDS()
        {
            if (Session["UserId"] != null)
            {
                int empid = Convert.ToInt32(Session["empid"].ToString());
                var selected = (from e in hrentity.tblConsultantTDS where e.fk_EMPID == empid select e).ToList();
                if (selected.Count() != 0)
                {
                    return View(selected);
                }
                else
                {
                    TempData["Empty"] = "0 records found in database!";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("_SessionError1", "Home");
            }
        }

        #endregion

        #region For DNB STudent
        public ActionResult StudentView()
        {
            try
            {
                //get employee dnb data id from session
                int empId = Convert.ToInt32(Session["empid"].ToString());
                var getDnbData = (from e in hrentity.TblDnbStudent where e.fk_EmployeeId == empId select e).FirstOrDefault();
                if (getDnbData == null)
                {
                    ViewBag.Message = "No DNB data found for this employee.";
                    return View(new List<DnbFeeDashboardVM>());
                }
                else
                {
                    int dnbID = getDnbData.DnbStudentId;
                    var dashboardData = hrentity.Database
                                   .SqlQuery<DnbFeeDashboardVM>("EXEC sp_GetDnbFeeDashboard @EmployeeId",
                                       new SqlParameter("@EmployeeId", dnbID))
                                   .ToList();

                    return View(dashboardData);
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected errors gracefully
                ViewBag.Error = "Error loading candidate dashboard: " + ex.Message;
                return View(new List<DnbFeeDashboardVM>());
            }
        }

        //upload fee proofs or submission fee slips

        public ActionResult UploadFeeProof(int id)
        {
            var structure = hrentity.TblDnbFeeStructure.FirstOrDefault(x => x.FeeStructureId == id);
            if (structure == null)
                return HttpNotFound();

            ViewBag.FeeStructureId = id;
            ViewBag.Amount = structure.Amount;
            ViewBag.PayableTo = structure.PayableTo;
            ViewBag.DueDate = structure.DueDate.ToString("dd/MM/yyyy");
            return View(); // Or redirect to StudentView if modal-only
        }

        [HttpPost]
        public JsonResult UploadFeeProof(DnbFeeSubmissionVM model, HttpPostedFileBase PaymentScreenshot)
        {
            try
            {
                // Get posted form fields
                var feeStructureId = Convert.ToInt32(model.FeeStructureId);
                var paymentReferenceNo = model.PaymentReferenceNo;
                var paymentMode = model.PaymentMode;
                var paymentDate = Convert.ToDateTime(Request["PaymentDate"]);
                var remarks = Request["Remarks"];

                // File upload handling
                HttpPostedFileBase file = Request.Files["PaymentScreenshot"];
                string fullPath = null;
                string finalName = null;

                if (file != null && file.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Content/Uploads/FeeProofs/");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string ext = Path.GetExtension(file.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    finalName = $"{fileName}_{DateTime.Now.Ticks}{ext}";
                    fullPath = Path.Combine(folderPath, finalName);
                    file.SaveAs(fullPath);

                }

                // Save in TblDnbFeeSubmission
                TblDnbFeeSubmission submission = new TblDnbFeeSubmission
                {
                    FeeStructureId = feeStructureId,
                    PaymentReferenceNo = paymentReferenceNo,
                    PaymentMode = paymentMode,
                    PaymentDate = paymentDate,
                    PaymentScreenshotPath = fullPath,//put here full path
                    FileName = finalName,
                    SubmissionRemarks = remarks,
                    SubmittedBy = Session["descript"]?.ToString() ?? "Student",
                    SubmittedDate = DateTime.Now,
                    Status = "Pending Verification"
                };

                hrentity.TblDnbFeeSubmission.Add(submission);
                hrentity.SaveChanges();

                // Update fee structure status
                var structure = hrentity.TblDnbFeeStructure.FirstOrDefault(x => x.FeeStructureId == feeStructureId);
                if (structure != null)
                {
                    structure.PaymentStatus = "Submitted";
                    hrentity.Entry(structure).State = System.Data.Entity.EntityState.Modified;
                    hrentity.SaveChanges();
                }

                return Json(new { success = true, message = "Fee proof uploaded successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetDNBSlipDetail(int id)
        {
            try
            {
                var slipData = hrentity.TblDnbFeeSubmission
                    .Where(x => x.FeeStructureId == id)
                    .OrderByDescending(x => x.SubmissionId)
                    .FirstOrDefault();

                if (slipData == null)
                    return Json(new { success = false, message = "No slip details found." }, JsonRequestBehavior.AllowGet);

                var slip = new
                {
                    slipData.PaymentReferenceNo,
                    // ✅ Format after fetching (avoids undefined)
                    PaymentDate = slipData.PaymentDate != default(DateTime)
                    ? slipData.PaymentDate.ToString("dd/MM/yyyy") : "-",
                    slipData.PaymentMode,
                    slipData.VerifiedRemarks,
                    slipData.Status,
                    slipData.PaymentScreenshotPath,
                    slipData.SubmissionRemarks,
                    slipData.FileName
                };

                return Json(new { success = true, data = slip }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //Print Slip only verified slips
        //public ActionResult FeeSlipPrint(int id)
        //{
        //    if (id != 0)
        //    {
        //        var slip = (from s in hrentity.TblDnbFeeSubmission
        //                    join f in hrentity.TblDnbFeeStructure on s.FeeStructureId equals f.FeeStructureId
        //                    join st in hrentity.TblDnbStudent on f.DnbStudentId equals st.DnbStudentId
        //                    where s.FeeStructureId == id && s.Status == "Verified"
        //                    select new
        //                    {
        //                        Submission = s,
        //                        FeeStructure = f,
        //                        Student = st
        //                    })
        //     .AsEnumerable()
        //     .Select(x =>
        //     {
        //         x.Submission.FeeStructureId = x.FeeStructure.FeeStructureId;
        //         x.Submission.TblDnbFeeStructure.DnbStudentId = x.Student.DnbStudentId;
               
        //         return x.Submission;
        //     })
        //     .FirstOrDefault();
        //        if (slip == null)
        //        {
        //            ViewBag.Error = "Fee slip not verified yet.";
        //            return View("ErrorView");
        //        }
        //        else
        //        {
        //            return View("PrintDNBFeeSlip", slip);
        //        }
        //    }
        //    else
        //    {
        //        return View();
        //    }
           
        //}

        public ActionResult PrintDNBFeeSlip(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Home");

            var slip = (from s in hrentity.TblDnbFeeSubmission
                        join f in hrentity.TblDnbFeeStructure on s.FeeStructureId equals f.FeeStructureId
                        join st in hrentity.TblDnbStudent on f.DnbStudentId equals st.DnbStudentId
                        where s.FeeStructureId == id && s.Status == "Verified"
                        select new PrintDNBSlipVM
                        {
                            vchName = st.tblEmpAssesmentMas != null ? st.tblEmpAssesmentMas.vchName : "",
                            vchEmpFcode = st.tblEmpAssesmentMas != null ? st.tblEmpAssesmentMas.vchEmpFcode : "",
                            CourseName = st.CourseName,
                            Amount = f.Amount,
                            PayableTo = f.PayableTo,
                            DueDate = f.DueDate,
                            PaymentReferenceNo = s.PaymentReferenceNo,
                            PaymentMode = s.PaymentMode,
                            PaymentDate = s.PaymentDate,
                            SubmittedDate = s.SubmittedDate,
                            VerifiedBy = s.VerifiedBy,
                            VerifiedRemarks = s.VerifiedRemarks,
                            Status = s.Status
                        }).FirstOrDefault();

            if (slip == null)
                return Content("No verified slip found or invalid ID.");

            return new ViewAsPdf("PrintDNBFeeSlip", slip)
            {
                FileName = $"FeeSlip_{slip.vchEmpFcode}.pdf",
                PageSize = Rotativa.Options.Size.A4,
                CustomSwitches = "--enable-local-file-access"
            };
        }

        #endregion
    }
}