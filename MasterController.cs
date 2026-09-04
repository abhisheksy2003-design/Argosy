using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using MVCBOND.Filters;
using Newtonsoft.Json;
using BE = MvcBondEntities.BusinessEntities;
using BM = MvcBondBusinessLayer.MvcBondBusinessLayer;
using CD = MvcBondDataLayer.Helper;
using Newtonsoft.Json;

namespace TrackerMVC.Controllers.BL
{
    [UserAuthenticationFilter] 
    public class MasterController : Controller
    {
        BM.BLDataManager.GSTSummary GS = new BM.BLDataManager.GSTSummary();
        // GET: Master
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MasterSummary()
        {
            
            return View();
        }
        public JsonResult GlobalSearch(String SearchText,String Master)
        {
            List<BE.GSTEntities> GSTList = new List<BE.GSTEntities>();
            GSTList = GS.getShippers(SearchText, Master);
            return Json(GSTList);
        }

        public ActionResult CustomerMaster()
        {
           ViewBag.Message= TempData["MessageValue"];
            return View();
        }

        public ActionResult ImportHold()
        {
            List<BE.SelectListModel> GetHoldType = GS.getImportHoldType();
            ViewBag.HoldType = new SelectList(GetHoldType, "ID", "Name");

            return View();
        }

        public JsonResult GetGlobalGSTList(String SearchText)
        {
             
            List<BE.GSTEntities> GSTList = new List<BE.GSTEntities>();
            GSTList = GS.getGlobalGSTList(SearchText);
            return Json(GSTList);
           // return View();
        }

        [HttpPost]
        public ActionResult EditCustomerDetails(Int64 ID)
        {
            BE.MasterEntities CustomerData = new BE.MasterEntities();
            CustomerData = GS.GetCutomerData(ID);
            return PartialView(CustomerData);
           // return Json(CustomerData);
        }

        [HttpPost]
        public ActionResult UpdateMasterData(BE.MasterEntities CustomerData)
        {
            string Message = "";
            int userId = Convert.ToInt32(Session["userid"]);
            Message = GS.UpdateMasterData(CustomerData, userId);
            ViewBag.Message = Message;
            TempData["MessageValue"] = Message;
            return RedirectToAction("CustomerMaster");
           // return RedirectToAction("GlobalSearchSummary", new { SearchText = CustomerData.SearchText });
            //return RedirectResult("");
           // return View("GlobalSearchSummary"); ;
        }

        [HttpPost]
        public ActionResult AddCustomerDetails()
        {
            return PartialView("EditCustomerDetails");
        }

        [HttpPost]
        public JsonResult CheckExisitMasterCode(string Code)
        {
#pragma warning disable CS0219 // The variable 'isCodeExisiting' is assigned but its value is never used
            bool isCodeExisiting = false;
#pragma warning restore CS0219 // The variable 'isCodeExisiting' is assigned but its value is never used
            int res=1;
            if (Code != "")
            {
                //isCodeExisiting = GS.GetExisitingCode(Code);
                res = GS.GetExisitingCode(Code);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckExisitMasterName(string Name, string ID)
        {
#pragma warning disable CS0219 // The variable 'isNameExisiting' is assigned but its value is never used
            bool isNameExisiting = false;
#pragma warning restore CS0219 // The variable 'isNameExisiting' is assigned but its value is never used
            Int64 masterID;
            if (ID == "")
            {
                 masterID = 0;
            }
            else {
                 masterID = Convert.ToInt64(ID);
            
            }
            
            int res = 1;
            if (Name != "")
            {
              //  isNameExisiting = GS.GetExisitingName(Name);
                res = GS.GetExisitingName(Name, masterID);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        // Codes by Arti

        [HttpPost]
        public ActionResult LocationDetails(Int64 ID, string Name)
        {
            // BE.MasterEntities CustomerData = new BE.MasterEntities();
            //CustomerData = GS.GetCutomerData(ID);

            List<BE.Ext_location_Master> LocationList = new List<BE.Ext_location_Master>();
            LocationList = GS.GetLocationList();
            ViewBag.LocationList = new SelectList(LocationList, "LocationID", "Location");

            List<BE.GST_Registration_Type> RegTypeList = new List<BE.GST_Registration_Type>();
            RegTypeList = GS.GetGSTRegistrationType();
            ViewBag.RegTypeList = new SelectList(RegTypeList, "RGID", "RGType");

            List<BE.Ext_location_Master> CustomerLocationList = new List<BE.Ext_location_Master>();
            CustomerLocationList = GS.GetCustomerLocationList(ID);
            ViewBag.CustomerLocationList = CustomerLocationList;

            ViewBag.CommonID = ID;
            ViewBag.Name = Name;
           
            //if (Convert.ToString(TempData["MessageValue"]) != null)
            //{
            //    //string msg = TempData["url"].ToString();
            //    string msg1 = Convert.ToString(TempData["MessageValue"]);
            //    ViewBag.Message = msg1;
            //}
            return PartialView();

        }


        [HttpPost]
        public ActionResult SaveLocationDetails(BE.LocationMaster LocationDetails)
        {
            string Message = "";
            int userId = Convert.ToInt32(Session["userid"]);
            Message = GS.AddLocationDetails(LocationDetails, userId);

            //Int64 id = LocationDetails.Common_ID;
            //ViewBag.Message = Message;

            //List<BE.Ext_location_Master> LocationList = new List<BE.Ext_location_Master>();
            //LocationList = GS.GetLocationList();
            //ViewBag.LocationList = new SelectList(LocationList, "LocationID", "Location");

            //List<BE.GST_Registration_Type> RegTypeList = new List<BE.GST_Registration_Type>();
            //RegTypeList = GS.GetGSTRegistrationType();
            //ViewBag.RegTypeList = new SelectList(RegTypeList, "RGID", "RGType");

            //List<BE.Ext_location_Master> CustomerLocationList = new List<BE.Ext_location_Master>();
            //CustomerLocationList = GS.GetCustomerLocationList(id);
            //ViewBag.CustomerLocationList = CustomerLocationList;

            //ViewBag.CommonID = LocationDetails.Common_ID;
            //ViewBag.Name = LocationDetails.GSTName;

            //return PartialView("LocationDetails");
            //TempData["MessageValue"] = Message;
            //return RedirectToAction("LocationDetails", new { ID = LocationDetails.Common_ID, Name=LocationDetails.GSTName });

            return Json(Message, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult getStateCode(string GSTNO)
        {
            List<BE.StateMaster> stateList = new List<BE.StateMaster>();
            stateList = GS.getStateCode(GSTNO);



            //ViewBag.Message = Message;
            //TempData["MessageValue"] = Message;
            //return RedirectToAction("GlobalSearchSummary");

            return Json(stateList, JsonRequestBehavior.AllowGet);

        }
        
        [HttpPost]
        public JsonResult GetLocationWiseData(Int32 id, Int64 Common_ID, Int64 GSTID)
        {
            BE.LocationMaster lm = new BE.LocationMaster();
            lm = GS.getLocationDataCustomerWise(id, Common_ID, GSTID);

            return Json(lm, JsonRequestBehavior.AllowGet); 
        
        }
        //Codes by Rahul
        [HttpPost]
        public ActionResult DeliveryAddresses(Int64 ID, string Name)
        {
            List<BE.DeliveryAddresses> LocationList = new List<BE.DeliveryAddresses>();
            LocationList = GS.GetDeliveryAddresses();
            ViewBag.LocationList = new SelectList(LocationList, "LocationID", "Location");

            List<BE.DeliveryAddresses> CustomerLocationList = new List<BE.DeliveryAddresses>();
            CustomerLocationList = GS.GetPreviousDeliveryAddresses(ID);
            ViewBag.CustomerLocationList = CustomerLocationList;

            ViewBag.CommonID = ID;
            ViewBag.Name = Name;

            return PartialView();
        }
        [HttpPost]
        public JsonResult GetDeliveryAddresswiseData(Int32 id, Int64 Common_ID, Int64 GSTID)
        {
            BE.DeliveryAddresses lm = new BE.DeliveryAddresses();
            lm = GS.getLocationWiseDeliveryAddress(id, Common_ID, GSTID);

            return Json(lm, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult SaveDeliveryAddresses(BE.DeliveryAddresses AddressDetails)
        {
            string Message = "";
            int userId = Convert.ToInt32(Session["userid"]);
            Message = GS.AddDeliveryAddresses(AddressDetails, userId);        
            return Json(Message, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ValidationforDuplicate(BE.DeliveryAddresses AddressDetails1)
        {
            BE.DeliveryAddresses lm = new BE.DeliveryAddresses();
            lm = GS.getDuplicateValidation(AddressDetails1);
            return Json(lm, JsonRequestBehavior.AllowGet);

        }
        public ActionResult PartyWiseHoldEntry()
        {
            List<BE.ActivityMaster> Activity = GS.getPartyWiseActivity();
            ViewBag.Activity = new SelectList(Activity, "ID", "TYPE");
            return View();
        }
        public JsonResult SaveHoldDetails(BE.PartyWiseHold HoldDetails)
        {

            int i = 0;
            ////int userId = Convert.ToInt32(Session["Tracker_userID"]);
            int UserID = Convert.ToInt16(Session["userid"]);
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            int retVal = 0;
            retVal = db.sub_ExecuteNonQuery("USP_INSERT_PARTY_WISE_HOLD '" + HoldDetails.HoldDate + "','" + HoldDetails.Activity + "','" + HoldDetails.Hold_To + "','" + HoldDetails.Hold_TO_ID + "','" + HoldDetails.Hold_Reason + "','" + HoldDetails.HoldReamrks + "','" + UserID + "'");
            return Json(i);
        }
        public ActionResult HoldDetailsList()
        {

            List<BE.PartyWiseHold> HoldLists = new List<BE.PartyWiseHold>();
            HoldLists = GS.getHoldDetailsLists();
            return PartialView(HoldLists);

        }
  
        public ActionResult ReleaseDetailsList()
        {

            //List<BE.PartyWiseHold> ReleaseLists = new List<BE.PartyWiseHold>();
            //ReleaseLists = GS.getReleaseDetailsList(FromDate, ToDate);
            return PartialView();

        }
        public ActionResult ReleaseDetailSummary(string FromDate, string ToDate)
        {

            List<BE.PartyWiseHold> ReleaseLists = new List<BE.PartyWiseHold>();
            ReleaseLists = GS.getReleaseDetailsList(FromDate, ToDate);
            return Json(ReleaseLists);

        }

        //SalesPersonMaster

        public ActionResult SalesPersonMaster(BE.SalesPerson salesPerson)
        {
            if (salesPerson.SalesPerson_ID1 != 0)
            {
                BE.SalesPerson data = new BE.SalesPerson();
                data = GS.GetSingleData(salesPerson.SalesPerson_ID1);
                ViewBag.SalesPerson_ID1 = data.SalesPerson_ID1;
                ViewBag.EmailID = data.EmailID;
                ViewBag.SalesPerson_Code = data.SalesPerson_Code;
                ViewBag.SalesPerson_Name = data.SalesPerson_Name;
                ViewBag.IsEdit = 1;

            }
            else
            {

                ViewBag.IsEdit = 0;
                ViewBag.SalesPerson_ID1 = 0;
                ViewBag.EmailID = "";
                ViewBag.SalesPerson_Code = "";
                ViewBag.SalesPerson_Name = "";
            }
            return View();

        }
        public JsonResult SaveSalesPersonMaster(BE.SalesPerson elements)
        {

            int i = 0;
            ////int userId = Convert.ToInt32(Session["userid"]);
            int UserID = Convert.ToInt16(Session["userid"]);
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            int retVal = 0;
            retVal = db.sub_ExecuteNonQuery("USP_INSERT_SaveSalesPersonMaster '" + elements.SalesPerson_ID1 + "','" + elements.SalesPerson_Code + "','" + elements.SalesPerson_Name + "','" + elements.EmailID + "','" + elements.IsActive + "','" + UserID + "'");
            return Json(i);
        }
        public ActionResult SalesPersonSummary()
        {

            List<BE.SalesPerson> SalesLists = new List<BE.SalesPerson>();
            SalesLists = GS.getSalesPersonSummary();

            return PartialView(SalesLists);

        }
        //CODE END BY RAHUL

        public JsonResult WithReason(string State, string PinCode)
        {
            DataTable tblInvoiceList = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            int userId = Convert.ToInt32(Session["userid"]);
            string Message = "";

            tblInvoiceList = db.sub_GetDatatable("USP_Validate_PIN_Code '" + State + "','" + PinCode + "'");
            if (tblInvoiceList.Rows.Count > 0)
            {
                Message = tblInvoiceList.Rows[0]["message"].ToString();
            }


            return new JsonResult() { Data = Message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult ThirdPartyVehicle()
        {
            ViewBag.Date = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");
            List<BE.TransporterEntities> Transpoter = new List<BE.TransporterEntities>();
            Transpoter = GS.getTranspoter();
            ViewBag.Transpoter = new SelectList(Transpoter, "TransID", "TransName");
            return View();
        }
        public JsonResult SaveVehicleDetails(string Vehicleno, string TransID, string vehiclegroup)
        {
            DataTable tblInvoiceList = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            int userId = Convert.ToInt32(Session["userid"]);
            var retVal = 0;
            string Message = "";

            retVal = db.sub_ExecuteNonQuery("USP_ThirdPartyVehicle '" + Vehicleno + "','" + TransID + "','" + vehiclegroup + "','" + userId + "'");

            if (retVal > 0)
            {
                Message = "Record Saved Successfully.";
            }
            else
            {
                Message = "Records Not Saved Successfully.";
            }

            return new JsonResult() { Data = Message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult Search(string search)
        {
            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("GetThirdPartyVDetails'" + search + "'");
            Session["GetThirdPartyVDetails"] = dt;
            var jsonResult = Json(JsonConvert.SerializeObject(dt), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }


        public ActionResult ExportToExcelThirdPartyReport()
        {
            DataTable dt = (DataTable)Session["GetThirdPartyVDetails"];
            DataTable CompanyMaster = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "Third Party Vehicles";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=Third Party Vehicle Details.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Third Party Vehicle Details <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
               
                    // render the GridView to the HtmlTextWriter
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            return View();
        }
        [HttpPost]
        public JsonResult AjaxCheckTrailerNumber(string TrailerNumber)
        {
            string Message = "";
            Message = GS.CheckTrailerNumber(TrailerNumber);

            return Json(Message);
        }
        public ActionResult ItemMaster()
        {
            List<BE.ItemMasterE> ItemGroup = new List<BE.ItemMasterE>();
            ItemGroup = GS.getItemGroupM();
            ViewBag.ItemGroup = new SelectList(ItemGroup, "ItemGroupID", "ItemGroup_Name");

            List<BE.ItemMasterE> ItemUnit = new List<BE.ItemMasterE>();
            ItemGroup = GS.getUnitM();
            ViewBag.UnitM = new SelectList(ItemGroup, "PurchaseUnitID", "PurchaseUnit");

           
            return View();
        }
        public ActionResult NewVesselDetails()
        {

            return View();
        }
        public JsonResult SaveVesselDetails(string VesselName )
        {
            DataTable tblInvoiceList = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            int userId = Convert.ToInt32(Session["userid"]);
            var retVal = 0;
            string Message = "";
            string strSQL1 = "";
            int VESSELID = 0;
            DataTable dt = new DataTable();
            strSQL1 = "SELECT isnull (max(Cast(VesselID as int ) ),0)+1 as VesselID FROM vessels";
            dt = db.sub_GetDatatable(strSQL1);

            if (dt.Rows.Count > 0)
            {
                VESSELID = Convert.ToInt32(dt.Rows[0]["VesselID"]);
            }
            retVal = db.sub_ExecuteNonQuery("USP_INSERT_VESSELS '" + VESSELID + "','" + VesselName + "','" + userId + "'");

            if (retVal > 0)
            {
                Message = "Record Saved Successfully.";
            }
            else
            {
                Message = "Records Not Saved Successfully.";
            }

            return new JsonResult() { Data = Message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #region Package Master
        public ActionResult Packages()
        {
            return View();

        }
        public JsonResult SavePKGSDetails(BE.PKGSMaster element)
        {
            BE.ResponseMessage message = new BE.ResponseMessage();
            element.AddedBy = Convert.ToInt32(Session["userID"]);
            message = GS.SavePKGSDetails(element);
            return Json(message);
        }
        public JsonResult PKGSList(string search)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("SP_PortList'" + search + "'");
            Session["Session_PKGSList"] = dt;
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult PackgesSummry(string search)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("sp_PackgesSummry'" + search + "'");
            Session["Session_PackgesSummry"] = dt;
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportPackages()
        {
            DataTable CompanyMaster = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            DataTable getMovementICDNew = (DataTable)Session["Session_PKGSList"];
            string Tittle = ""; // "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = getMovementICDNew;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=Packages.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))

                {

                    // render the GridView to the HtmlTextWriter
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Bond NOC Register Summary<strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> * </h6></td></tr>");
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            return View();
        }
        public JsonResult GetUpdatePackageMaster(int CodeID)
        {
            DataTable dt = new DataTable();
            BE.PKGSMaster list = new BE.PKGSMaster();
            list = GS.GetUpdatePackageMaster(CodeID);
            var JsonResult = Json(list, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }

        #endregion

        #region Port Master
        public ActionResult Ports()
        {
            return View();

        }
        public JsonResult SavePortDetails(BE.PKGSMaster element)
        {
            BE.ResponseMessage message = new BE.ResponseMessage();
            element.AddedBy = Convert.ToInt32(Session["userID"]);
            message = GS.SavePKGSDetails(element);
            return Json(message);
        }
        public JsonResult PortList(string search)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("SP_PKGList'" + search + "'");
            Session["Session_PKGSList"] = dt;
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportPorts()
        {
            DataTable CompanyMaster = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            DataTable getMovementICDNew = (DataTable)Session["Session_PKGSList"];
            string Tittle = ""; // "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = getMovementICDNew;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=Port.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))

                {

                    // render the GridView to the HtmlTextWriter
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Bond NOC Register Summary<strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> * </h6></td></tr>");
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            return View();
        }
        public JsonResult GetUpdatePortMaster(int CodeID)
        {
            DataTable dt = new DataTable();
            BE.PKGSMaster list = new BE.PKGSMaster();
            list = GS.GetUpdatePackageMaster(CodeID);
            var JsonResult = Json(list, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }

        #endregion

        #region EquipmentMaster
        public ActionResult EquipmentMaster()
        {
            return View();
        }
        public ActionResult SaveEquipment(BE.Equipment EquipmentMaster)
        {
            string message = "";
            ///var EntryDate = LocationMaster.EntryDate;
            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("Insert_Equipment_M '" + EquipmentMaster.EquipmentID + "','" + EquipmentMaster.EquipmentName + "','" + Convert.ToInt32(Session["userid"]) + "'");
            if (dt.Rows.Count > 0)
            {
                message = Convert.ToString(dt.Rows[0][0]);
            }

            return Json(message);
        }




        [HttpPost]
        public ActionResult AjaxGetEquipmentDetails()
        {
            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();

            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_EquipmentMaster");
            Session["EquipmentMaster"] = dt;

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            //var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            //var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public ActionResult ExportToExcelEquipment()
        {
            DataTable dt = (DataTable)Session["EquipmentMaster"];
            DataTable CompanyMaster = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            DataTable FuelStockSummary = (DataTable)Session["FuelStockSummary"];
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=VoucherDetails.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Equipment Summary  <strong></td></tr>");
                    //   htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> * </h6></td></tr>");
                    // render the GridView to the HtmlTextWriter
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            return View();
        }

        #endregion Equipmentmaster


        #region PortMaster
        public ActionResult PortMaster()
        {
            return View();
        }
        public ActionResult SavePortMaster(BE.PortMaster PortMaster)
        {
            string message = "";
            ///var EntryDate = LocationMaster.EntryDate;
            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("Insert_Port_Master '" + PortMaster.PortName + "','" + Convert.ToInt32(Session["userid"]) + "'");
            if (dt.Rows.Count > 0)
            {
                message = Convert.ToString(dt.Rows[0][0]);
            }

            return Json(message);
        }

        [HttpPost]
        public ActionResult AjaxGetPortDetails()
        {
            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();

            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_PortMaster");
            Session["PortMaster"] = dt;

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        #endregion



        [HttpPost]
        public JsonResult SavePort(int PORTID, string PortName, string P_Code)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            int userId = Convert.ToInt32(Session["userid"]);
            int retVal = 0;

            try
            {
                 retVal = db.sub_ExecuteNonQuery(
                    $"USP_SavePortDetails {PORTID}, '{PortName}', '{P_Code}', {userId}"
                );

                var result = new
                {
                    Status = retVal > 0,
                    Message = retVal > 0 ?
                              (PORTID == 0 ? "Record Inserted Successfully." : "Record Updated Successfully.")
                              : "Operation Failed."
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult FetchPortDetails(int PortID)
        {
            try
            {
                CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();

                 DataTable dt = db.sub_GetDatatable($"EXEC USP_GetEditPortMaster @PortID={PortID}");

                if (dt == null || dt.Rows.Count == 0)
                    return Json(null, JsonRequestBehavior.AllowGet);

                 var rows = dt.AsEnumerable().Select(row => new
                {
                    PortID = row["PortID"],
                    PortName = row["PortName"],
                    P_Code = row["P_Code"]
                }).ToList();

                return Json(rows, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult SavePackage(int CodeID, string Package, string PackageDesk)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            int userId = Convert.ToInt32(Session["userid"]);
            int retVal = 0;

            try
            {
                retVal = db.sub_ExecuteNonQuery(
                   $"USP_SavePkgsDetail {CodeID}, '{Package}', '{PackageDesk}', {userId}"
               );

                var result = new
                {
                    Status = retVal > 0,
                    Message = retVal > 0 ?
                              (CodeID == 0 ? "Record Inserted Successfully." : "Record Updated Successfully.")
                              : "Operation Failed."
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult FetchPacgesDetails(int CodeID)
        {
            try
            {
                CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();

                DataTable dt = db.sub_GetDatatable(
                    $"EXEC USP_GetEditPacgesMaster @CodeID={CodeID}"
                );
                 
                var json = JsonConvert.SerializeObject(dt);
                 
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}