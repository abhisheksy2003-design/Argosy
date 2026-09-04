using MVCBOND.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using BA= MvcBondBusinessLayer.MvcBondBusinessLayer;
using BE = MvcBondEntities.BusinessEntities;
using BM = MvcBondBusinessLayer;
using CD = MvcBondDataLayer.Helper;
using HC = MvcBondDataLayer.Helper;

namespace MVCBOND.Controllers.Export
{
    [UserAuthenticationFilter]
    public class ExportController : Controller
    {
        BA.BLDataManager.GSTSummary GS = new BA.BLDataManager.GSTSummary();
        BM.Export.ExportBL reportprovider = new BM.Export.ExportBL();

        #region Edit
        [HttpPost]
        public JsonResult SBCartingEntryDetails(int id)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = db.sub_GetDatatable(
              "USP_SBCartingEntryDetailsGrid " + id   
            );
            string json = JsonConvert.SerializeObject(dt);

            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        #endregion


        #region Carting Allow
        public ActionResult CartingAllow()
        {

            List<BE.ExportEnt> Equipment = new List<BE.ExportEnt>();
            Equipment = reportprovider.getEquipment();
            ViewBag.Equipment = new SelectList(Equipment, "Equipmentid", "Equipment");
            return View();
        }

        public ActionResult SaveCartingAllow(List<BE.CartingAllow> Debitdata, string CartingNo,string SbEntryID)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("SBNo");
            //dataTable.Columns.Add("SBDate");
            //dataTable.Columns.Add("SBQty");
            //dataTable.Columns.Add("SBWeight");
            dataTable.Columns.Add("VehicelNo");
            dataTable.Columns.Add("CartedQty");
            dataTable.Columns.Add("CartingDate");
            dataTable.Columns.Add("EquipmentID");
            dataTable.TableName = "PT_Export_Carting_Allow";

            foreach (BE.CartingAllow item in Debitdata)
            {
                DataRow row = dataTable.NewRow();

                row["SBNo"] = item.SBNo;
                //row["SBDate"] = item.SBDate;
                //row["SBQty"] = item.SBQty;
                //row["SBWeight"] = item.SBWeight;
                row["VehicelNo"] = item.VehicelNo;
                row["CartedQty"] = item.CartedQty;
                row["CartingDate"] = item.CartingDate;
                row["EquipmentID"] = item.CartingEquipmentID;

                dataTable.Rows.Add(row);
            }


            int Userid = Convert.ToInt32(Session["userid"]);
            string message = reportprovider.SaveCartingAllow(dataTable, Userid, CartingNo, SbEntryID);
            return Json(message);

        }

        public JsonResult GetCartingAllowSummary(string fromDate, string toDate)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();



            dt = db.sub_GetDatatable("usp_GetCartingAllowSummary'" + fromDate + "','" + toDate + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult CartingEntryList(string SearchType)
        {

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_CartingEntryList");

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion


        #region Caerting tally 
        public ActionResult CartingTallyEntry()
        {
            List<BE.ExportEnt> Export = new List<BE.ExportEnt>();
            Export = reportprovider.getCargoType();
            ViewBag.ExportEntry = new SelectList(Export, "Cargotypeid", "Cargotype");

            List<BE.ExportEnt> Equipment = new List<BE.ExportEnt>();
            Equipment = reportprovider.getEquipment();
            ViewBag.Equipment = new SelectList(Equipment, "Equipmentid", "Equipment");

            List<BE.ExportEnt> Export1 = new List<BE.ExportEnt>();
            Export1 = reportprovider.getContainerType();
            ViewBag.ContainerTypeEntry = new SelectList(Export1, "ContainerTypeID", "ContainerType");

            BE.IGMManualEntities IGMManualList = new BE.IGMManualEntities();
            IGMManualList = reportprovider.GetDropDownListIGMManual();
            ViewBag.PackageIGMList = new SelectList(IGMManualList.PackageIGMList, "CodeID", "Package");
            return View();
        }


        public JsonResult CartingAllowList(string SearchType)
        {

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_CartingAllowList");

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult CartingEntryDetails(string SearchType)
        {

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            //dt = db.sub_GetDatatable("USP_CartingEntryList");
            dt = db.sub_GetDatatable("USP_CartingEntryDetailsGrid '" + SearchType + "'");

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion


        // GET: Export
        public ActionResult CartingEntry()
        {
            List<BE.ExportEnt> Export = new List<BE.ExportEnt>();
            Export = reportprovider.getCargoType();
            ViewBag.ExportEntry = new SelectList(Export, "Cargotypeid", "Cargotype");

            List<BE.ExportEnt> Equipment = new List<BE.ExportEnt>();
            Equipment = reportprovider.getEquipment();
            ViewBag.Equipment = new SelectList(Equipment, "Equipmentid", "Equipment");

            List<BE.ExportEnt> Export1 = new List<BE.ExportEnt>();
            Export1 = reportprovider.getContainerType();
            ViewBag.ContainerTypeEntry = new SelectList(Export1, "ContainerTypeID", "ContainerType");


            
            BE.IGMManualEntities IGMManualList = new BE.IGMManualEntities();
            IGMManualList = reportprovider.GetDropDownListIGMManual();
            ViewBag.PackageIGMList = new SelectList(IGMManualList.PackageIGMList, "CodeID", "Package");
            return View();
        }

        [HttpPost]
        public ActionResult SaveCarting(List<BE.CartingEntry> Debitdata, string EntryType, int EntryId)
        {
            DataTable dataTable = new DataTable();
           
            dataTable.Columns.Add("SBNo");
            dataTable.Columns.Add("SBDate");
            dataTable.Columns.Add("SBQty");
            dataTable.Columns.Add("SBWeight");
            dataTable.Columns.Add("VehicelNo");
            dataTable.Columns.Add("CartedQty");

            dataTable.Columns.Add("CartingDate");
            dataTable.Columns.Add("CHAID");
            dataTable.Columns.Add("CUSTOMERID");
            dataTable.Columns.Add("CargoDescrition");

            dataTable.Columns.Add("CargoTypeID");
            dataTable.Columns.Add("EquipmentID"); 
            dataTable.Columns.Add("ContainerNo");
            dataTable.Columns.Add("size");
            dataTable.Columns.Add("Type");
            dataTable.Columns.Add("Remarks");
            dataTable.Columns.Add("UANNo");
            dataTable.Columns.Add("Class");
            dataTable.Columns.Add("PackingGroupID");
            dataTable.Columns.Add("PackageTypeID");

            dataTable.Columns.Add("Exporter");
            dataTable.Columns.Add("ExporterAddress");
            dataTable.Columns.Add("Consignee");
            dataTable.Columns.Add("ConsigneeAddress");
            dataTable.Columns.Add("PODID");
            dataTable.Columns.Add("FPDID");
            dataTable.Columns.Add("FOBValue");
            dataTable.Columns.Add("Temp");
            dataTable.Columns.Add("Humidity");
            dataTable.Columns.Add("Vent");

            dataTable.TableName = "PT_Export_Carting";

            foreach (BE.CartingEntry item in Debitdata)
            {
                DataRow row = dataTable.NewRow();

                
                row["SBNo"] = item.SBNo;
                row["SBDate"] = item.SBDate;
                row["SBQty"] = item.SBQty;
                row["SBWeight"] = item.SBWeight;
                row["VehicelNo"] = item.VehicelNo;
                row["CartedQty"] = item.CartedQty;

                row["CartingDate"] = item.CartingDate;
                row["CargoDescrition"] = item.CargoDescrition;
                row["CHAID"] = item.CHAID;

                row["CUSTOMERID"] = item.CUSTOMERID;
                row["CargoTypeID"] = item.CargoTypeID;
                row["EquipmentID"] = item.CartingEquipmentID;

                row["ContainerNo"] = item.ContainerNo;
                row["size"] = item.size;
                row["Type"] = item.Type;
                row["Remarks"] = item.Remarks;
                row["UANNo"] = item.UANNo;
                row["Class"] = item.Class;
                row["PackingGroupID"] = item.PackingGroupID;
                row["PackageTypeID"] = item.PackageTypeID;
                row["Exporter"] = item.Exporter;
                row["ExporterAddress"] = item.ExporterAddress;
                row["Consignee"] = item.Consignee;
                row["ConsigneeAddress"] = item.ConsigneeAddress;
                row["PODID"] = item.PODID;
                row["FPDID"] = item.FPDID;
                row["FOBValue"] = item.FOBValue;
                row["Temp"] = item.Temp;
                row["Humidity"] = item.Humidity;
                row["Vent"] = item.Vent;
                dataTable.Rows.Add(row);
            }


            int Userid = Convert.ToInt32(Session["userid"]);
            string message = reportprovider.SaveCartingEntry(dataTable, Userid, EntryType, EntryId);
            return Json(message);

        }
        public ActionResult SaveCartingTally(List<BE.CartingTallyEntry> Debitdata, string EntryType, string CartingAllowID)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("CartingNo");
            dataTable.Columns.Add("SBNo");
            dataTable.Columns.Add("SBDate");
            dataTable.Columns.Add("SBQty");
            
            dataTable.Columns.Add("SBWeight");
            
            dataTable.Columns.Add("VehicelNo");
            dataTable.Columns.Add("CartedQty");
            dataTable.Columns.Add("TallyQty");
            dataTable.Columns.Add("TallyWeight");
            dataTable.Columns.Add("Area");
            dataTable.Columns.Add("CartingDate");
            dataTable.Columns.Add("CHAID");
            dataTable.Columns.Add("CUSTOMERID");
            dataTable.Columns.Add("CargoDescrition");
             

            dataTable.Columns.Add("CargoTypeID");
            dataTable.Columns.Add("EquipmentID");


            dataTable.Columns.Add("ContainerNo");
            dataTable.Columns.Add("size");
            dataTable.Columns.Add("Type");
            dataTable.Columns.Add("Remarks");
            dataTable.Columns.Add("UANNo");
            dataTable.Columns.Add("Class");
            dataTable.Columns.Add("PackingGroupID");
            dataTable.Columns.Add("CartingAllowID", typeof(int)); // FIRST
            dataTable.Columns.Add("PackageTypeID");

            dataTable.TableName = "PT_Export_CartingTally";

            foreach (BE.CartingTallyEntry item in Debitdata)
            {
                DataRow row = dataTable.NewRow();
                row["CartingNo"] = item.CartingNo;
                row["SBNo"] = item.SBNo;
                row["SBDate"] = item.SBDate;
                row["SBQty"] = item.SBQty;
                row["SBWeight"] = item.SBWeight;
                row["VehicelNo"] = item.VehicelNo;
                row["CartedQty"] = item.CartedQty;
                row["TallyQty"] = item.TallyQty;
                row["TallyWeight"] = item.TallyWeight;
                row["Area"] = item.Area;

                row["CartingDate"] = item.CartingDate;
                row["CargoDescrition"] = item.CargoDescrition;
                row["CHAID"] = item.CHAID;
                row["CUSTOMERID"] = item.CUSTOMERID;
                row["CargoTypeID"] = item.CargoTypeID;
                row["EquipmentID"] = item.CartingEquipmentID;

                row["ContainerNo"] = item.ContainerNo;
                row["size"] = item.size;
                row["Type"] = item.Type;
                row["Remarks"] = item.Remarks;
                row["UANNo"] = item.UANNo;
                row["Class"] = item.Class;
                row["PackingGroupID"] = item.PackingGroupID;
                row["CartingAllowID"] = item.CartingAllowID;
                row["PackageTypeID"] = item.PackageTypeID;
                dataTable.Rows.Add(row);
            }


            int Userid = Convert.ToInt32(Session["userid"]);
            string message = reportprovider.SaveCartingTallyEntry(dataTable, Userid, EntryType, CartingAllowID);
            return Json(message);

        }

        public ActionResult EmptyGateIn()
        {
            List<BE.ISOCodes> ISOCodes = new List<BE.ISOCodes>();
            ISOCodes = reportprovider.getISOCodes();
            ViewBag.ISOCodes = new SelectList(ISOCodes, "ISOID", "ISOCode");

            List<BE.ExportEnt> Export = new List<BE.ExportEnt>();
            Export = reportprovider.getContainerType();
            ViewBag.ContainerTypeEntry = new SelectList(Export, "ContainerTypeID", "ContainerType");

            List<BE.ExportEnt> Loaction = new List<BE.ExportEnt>();
            Loaction = reportprovider.getLocation();
            ViewBag.Loaction = new SelectList(Loaction, "LocationID", "Location");
            return View();
        }
        public JsonResult getLineName(string prefixText, string Mode)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataTable dataTable = new DataTable();
            List<BE.Customer> Customerlst = new List<BE.Customer>();
            dataTable = db.sub_GetDatatable("USP_GetLineName '" + Mode + "','" + prefixText + "'");

            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    BE.Customer customer = new BE.Customer();
                    customer.AGID = Convert.ToInt32(row["GSTID"]);
                    customer.AGName = Convert.ToString(row["GSTName"]);
                    Customerlst.Add(customer);
                }
            }

            var jsonResult = Json(Customerlst, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public JsonResult getVendorName(string prefixText, string Mode)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataTable dataTable = new DataTable();
            List<BE.Customer> Customerlst = new List<BE.Customer>();
            dataTable = db.sub_GetDatatable("USP_GetVendorName '" + Mode + "','" + prefixText + "'");

            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    BE.Customer customer = new BE.Customer();
                    customer.AGID = Convert.ToInt32(row["GSTID"]);
                    customer.AGName = Convert.ToString(row["GSTName"]);
                    Customerlst.Add(customer);
                }
            }

            var jsonResult = Json(Customerlst, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult SaveEmptyGate(List<BE.GateEntry> Debitdata,string EmptyDate)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ContainerNo");
            dataTable.Columns.Add("Size");
            dataTable.Columns.Add("InDate");
            dataTable.Columns.Add("TareWeight");
            dataTable.Columns.Add("VehicelNo");
            dataTable.Columns.Add("BkNo");
             
            dataTable.Columns.Add("LineId");

            dataTable.Columns.Add("CustomerId");
            dataTable.Columns.Add("TransporterId");
            dataTable.Columns.Add("TypeID");
            dataTable.Columns.Add("AgentSeal");
            dataTable.Columns.Add("ISOCodeID");

            dataTable.TableName = "PT_EmptyGate_In";

            foreach (BE.GateEntry item in Debitdata)
            {
                DataRow row = dataTable.NewRow();

                row["ContainerNo"] = item.ContainerNo;
                row["Size"] = item.Size;
                row["InDate"] = item.InDate;
                row["TareWeight"] = item.TareWeight;
                row["VehicelNo"] = item.VehicelNo;
                row["BkNo"] = item.BkNo;

                row["LineId"] = item.LineId;
                row["CustomerId"] = item.CustomerId;
                row["TransporterId"] = item.TransporterId; 
                row["TypeID"] = item.TypeID;
                row["AgentSeal"] = item.AgentSeal;
                row["ISOCodeID"] = item.ISOCodeID;

                dataTable.Rows.Add(row);
            }


            int Userid = Convert.ToInt32(Session["userid"]);
            string message = reportprovider.SaveEmptyEntry(dataTable, Userid, EmptyDate);
            return Json(message);

        }
        public ActionResult Stuffing()
        {
            List<BE.ExportEnt> Export = new List<BE.ExportEnt>();
            Export = reportprovider.getCargoType();
            ViewBag.ExportEntry = new SelectList(Export, "Cargotypeid", "Cargotype");

            List<BE.ExportEnt> Equipment = new List<BE.ExportEnt>();
            Equipment = reportprovider.getEquipment();
            ViewBag.Equipment = new SelectList(Equipment, "Equipmentid", "Equipment");


            return View();
        }
        public ActionResult SaveStuffing(List<BE.StuffingEntry> Debitdata, string GateInNo, string sbentryID)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("SBNo");
            dataTable.Columns.Add("ContainerNo");
            dataTable.Columns.Add("StuffedQty");
            dataTable.Columns.Add("StuffedWeight");
            dataTable.Columns.Add("StuffedDate");
            dataTable.Columns.Add("CustomSeal");

            dataTable.Columns.Add("AgentSeal");

            dataTable.Columns.Add("CargoTypeID");
             
            dataTable.TableName = "PT_Export_Stuffing";

            foreach (BE.StuffingEntry item in Debitdata)
            {
                DataRow row = dataTable.NewRow();

                row["SBNo"] = item.SBNo;
                row["ContainerNo"] = item.ContainerNo;
                row["StuffedQty"] = item.StuffedQty;
                row["StuffedWeight"] = item.StuffedWeight;
                row["StuffedDate"] = item.StuffedDate;
                row["CustomSeal"] = item.CustomSeal;

                row["AgentSeal"] = item.AgentSeal;
                row["CargoTypeID"] = item.CargoTypeID;
                
                dataTable.Rows.Add(row);
            }


            int Userid = Convert.ToInt32(Session["userid"]);
            string message = reportprovider.SaveStuffingEntry(dataTable, Userid, GateInNo, sbentryID);
            return Json(message);

        }
        public ActionResult GateOut()
        {
            List<BE.ExportEnt> Export = new List<BE.ExportEnt>();
            Export = reportprovider.getPortName();
            ViewBag.PortName = new SelectList(Export, "PortID", "PortName");

            List<BE.ExportEnt> Equipment = new List<BE.ExportEnt>();
            Equipment = reportprovider.getVesselName();
            ViewBag.VesselName = new SelectList(Equipment, "VesselID", "VesselName");

            return View();
        }
        public ActionResult SaveGateOut(string ContainerNo, string PortID, string VesselID,
            string YogageNo,string VehicleNo, int TransporterID, string DriverName, string DriverMobile, string Remarks)
        {
            string message = "";

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_INSERT_EXPORTGATE_OUT '" + 
                ContainerNo + "','" + 
                PortID + "','" + 
                VesselID    + "','" + Convert.ToInt32(Session["userid"]) +  "','"+ 
                YogageNo + "','" +
                VehicleNo + "','" +
                TransporterID + "','" +
                DriverName + "','" +
                DriverMobile + "','" + 
                Remarks + "'");
            if (dt.Rows.Count > 0)
            {
                message = Convert.ToString(dt.Rows[0][0]);
            }

            return Json(message);
        }
        public ActionResult ExportTariffMaster()
        {
            List<BE.exporterShipping> ShippingLine = new List<BE.exporterShipping>();
            ShippingLine = reportprovider.GetExportShippingDetails();
            ViewBag.ShippingLineList = new SelectList(ShippingLine, "Exportershippingid", "ExporterShippingname");
            List<BE.CHA> CHA = new List<BE.CHA>();
            CHA = reportprovider.getCHA();
            ViewBag.CHA = new SelectList(CHA, "CHANo", "CHAName");

            List<BE.Customer> Customer = new List<BE.Customer>();
            Customer = reportprovider.GetCustomer();
            ViewBag.CustomerList = new SelectList(Customer, "AGID", "AGName");

            List<BE.Consignee> Consignee = new List<BE.Consignee>();
            Consignee = reportprovider.GetImporter();
            ViewBag.Consignee = new SelectList(Consignee, "ImporterID", "ImporterName");

            List<BE.TariffGroup> TariffGroup = new List<BE.TariffGroup>();
            TariffGroup = reportprovider.GettaiffGroup();
            ViewBag.TariffGroup = new SelectList(TariffGroup, "Group_ID", "Group_Name");


            //List<BE.importtariffdetails> importtariffdetails = new List<BE.importtariffdetails>();
            //importtariffdetails = reportprovider.Getimporttariffdetails();
            //ViewBag.importtariffdetails = new SelectList(importtariffdetails, "TariffID", "TariffDescription");


            List<BE.ExportEnt> ContainerType = new List<BE.ExportEnt>();
            ContainerType = reportprovider.getContainerType();
            ViewBag.ContainerType = new SelectList(ContainerType, "ContainerTypeID", "ContainerTypeName");



            List<BE.ImportAccountMaster> AccountHead = new List<BE.ImportAccountMaster>();
            AccountHead = reportprovider.GetAccountHead();
            ViewBag.AccountHead = new SelectList(AccountHead, "AccountID", "AccountName");

            List<BE.SalesPersonM> SalesList = new List<BE.SalesPersonM>();
            SalesList = reportprovider.GetSalesmamDetails();
            ViewBag.SalesListD = new SelectList(SalesList, "SalesPerson_ID1", "SalesPerson_Name");
            return View();
        }
        public JsonResult CheckExportTariffMasterAlready(string TariffDescription)
        {
            string message = "";

            //HC.DBOperation object = new HC.DBOperations(); From Helper
            DataTable SvaDT = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            //Code For Insert Data Sequence Should Be Same As Created SP.
            SvaDT = db.sub_GetDatatable("SP_SaveExportTariffMaster_Check_EXISTS '" + TariffDescription + "'");

            if (SvaDT.Rows.Count > 0)
            {
                message = Convert.ToString(SvaDT.Rows[0]["Message"]);
            }

            return Json(message);
        }
        public ActionResult SaveExportTariffmaster(string TariffID, string txtDescription, string ddlshippingline, int ddlCHA, string ddlcustomer, string txtFromDate, string txtToDate, string txtday,
        string StorageDay, string EmptyDay, int isactive, string AttchmentID, string salesid)

        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            string message = "";
            string strSQL = "";
            Int64 intEntryID = 0;
            DataTable dt = new DataTable();
            DataTable SvaDT = new DataTable();
            int UserID = Convert.ToInt16(Session["userid"]);

            if (TariffID == "")
            {
                strSQL = "";
                strSQL = "select ISNULL(MAX(TariffID),0) as TariffID from exp_tariffmaster with(xlock)";
                dt = db.sub_GetDatatable(strSQL);
                if (dt.Rows.Count > 0)
                {
                    intEntryID = Convert.ToInt64(dt.Rows[0].Field<Int64>("TariffID") + 1);
                }
                else
                {
                    intEntryID = 1;
                }


                SvaDT = db.sub_GetDatatable("exec SP_SUExpTariffmaster '" + intEntryID + "', '" + txtDescription + "', '" + Convert.ToDateTime(txtFromDate).ToString("yyyyMMdd") + "', '" + Convert.ToDateTime(txtToDate).ToString("yyyyMMdd") + "', '" + isactive + "', '" + Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "', '" + UserID + "', '" + ddlcustomer + "', '" + ddlshippingline + "', '" + ddlCHA + "', '" + txtday + "', " + EmptyDay + ", " + StorageDay + ",'" + AttchmentID + "','" + salesid + "'");

            }
            else
            {
                SvaDT = db.sub_GetDatatable("exec SP_SUExpTariffmaster '" + TariffID + "', '" + txtDescription + "', '" + Convert.ToDateTime(txtFromDate).ToString("yyyyMMdd") + "', '" + Convert.ToDateTime(txtToDate).ToString("yyyyMMdd") + "', '" + isactive + "', '" + Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "', '" + UserID + "', '" + ddlcustomer + "', '" + ddlshippingline + "', '" + ddlCHA + "', '" + txtday + "', " + EmptyDay + ", " + StorageDay + ",'" + salesid + "'");

            }
            if (SvaDT.Rows.Count > 0)
            {
                message = Convert.ToString(SvaDT.Rows[0]["Message"]);
            }
            return Json(message);
        }
        public ActionResult EmptyOut()
        {
             
            return View();
        }
        public ActionResult SavEmptyOut(string ContainerNo, string OutDate, string FromId, string VehicleNo, string Remarks)
        {
            string message = "";

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_INSERT_EMPTYOUT '" + ContainerNo + "','" + OutDate + "','" + Convert.ToInt32(Session["userid"]) + "','" + FromId + "','" + VehicleNo + "','" + Remarks + "'");
            if (dt.Rows.Count > 0)
            {
                message = Convert.ToString(dt.Rows[0][0]);
            }

            return Json(message);
        }
        public ActionResult ExportAccountMaster()
        {
            ViewBag.Date = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");

            List<BE.ExpHeadMasterEnt> InvoiceType = new List<BE.ExpHeadMasterEnt>();
            InvoiceType = reportprovider.InvoiceTypeDDL();
            ViewBag.InvoiceDDL = new SelectList(InvoiceType, "InvTId", "InvType");

            List<BE.ExpHeadMasterEnt> HSNSelect = new List<BE.ExpHeadMasterEnt>();
            HSNSelect = reportprovider.HSNCodeDDL();
            ViewBag.HSNDDList = new SelectList(HSNSelect, "HSNID", "HSNCodeL");

            List<BE.ExpHeadMasterEnt> TaxName = new List<BE.ExpHeadMasterEnt>();
            TaxName = reportprovider.getTaxName();
            ViewBag.TaxName = new SelectList(TaxName, "TaxID", "TaxName");

            List<BE.ExpHeadMasterEnt> IMPGroup = new List<BE.ExpHeadMasterEnt>();
            IMPGroup = reportprovider.IMPGroupDDl();
            ViewBag.importg = new SelectList(IMPGroup, "IMPGID", "IMPGName");
            return View();

        }
        public JsonResult ChecktheExportAccountmasterAlready(BE.ExpHeadMasterEnt ExpHeadMasterEnt)
        {
            string message = "";
            var EntryDate = ExpHeadMasterEnt.EntryDate;
            //HC.DBOperation object = new HC.DBOperations(); From Helper
            DataTable SvaDT = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            //Code For Insert Data Sequence Should Be Same As Created SP.
            SvaDT = db.sub_GetDatatable("SP_SaveExportAccountMaster_Check_EXISTS '" + ExpHeadMasterEnt.EntryID + "','" + ExpHeadMasterEnt.AcName + "','" + ExpHeadMasterEnt.TallyName + "','" + ExpHeadMasterEnt.disp + "','" + ExpHeadMasterEnt.IsActive + "','" + ExpHeadMasterEnt.IMPGID + "','" + ExpHeadMasterEnt.HSNCodeL + "','" + Convert.ToInt32(Session["Tracker_userID"]) + "','" + ExpHeadMasterEnt.InvTId + "','" + ExpHeadMasterEnt.TaxID + "'");

            if (SvaDT.Rows.Count > 0)
            {
                message = Convert.ToString(SvaDT.Rows[0]["Message"]);
            }

            return Json(message);
        }

        public JsonResult SaveExportAccountmaster(BE.ExpHeadMasterEnt ExpHeadMasterEnt)
        {
            string message = "";
            string strSQL = "";
            Int64 intEntryID = 0;
            int UserID = Convert.ToInt16(Session["Tracker_userID"]);
            var EntryDate = ExpHeadMasterEnt.EntryDate;

            DataTable SvaDT = new DataTable();
            DataTable dt = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();



            if (ExpHeadMasterEnt.EntryID == 0)
            {
                strSQL = "";
                strSQL = "select ISNULL(MAX(AccountID),0) as maxID from exp_accountmaster with(xlock)";
                dt = db.sub_GetDatatable(strSQL);
                if (dt.Rows.Count > 0)
                {
                    intEntryID = Convert.ToInt64(dt.Rows[0].Field<Int64>("maxID") + 1);
                }
                else
                {
                    intEntryID = 1;
                }


                SvaDT = db.sub_GetDatatable("SP_SUExportAccountmaster '" + intEntryID + "','" + ExpHeadMasterEnt.AcName + "','" + ExpHeadMasterEnt.TallyName + "','" + ExpHeadMasterEnt.IMPGID + "','" + ExpHeadMasterEnt.HSNCodeL + "','" + ExpHeadMasterEnt.disp + "','" + ExpHeadMasterEnt.IsActive + "','" + UserID + "','" + ExpHeadMasterEnt.InvTId + "','" + ExpHeadMasterEnt.TaxID + "'");

            }
            else
            {
                SvaDT = db.sub_GetDatatable("SP_SUExportAccountmaster '" + ExpHeadMasterEnt.EntryID + "','" + ExpHeadMasterEnt.AcName + "','" + ExpHeadMasterEnt.TallyName + "','" + ExpHeadMasterEnt.IMPGID + "','" + ExpHeadMasterEnt.HSNCodeL + "','" + ExpHeadMasterEnt.disp + "','" + ExpHeadMasterEnt.IsActive + "','" + UserID + "','" + ExpHeadMasterEnt.InvTId + "','" + ExpHeadMasterEnt.TaxID + "'");

            }
            if (SvaDT.Rows.Count > 0)
            {
                message = Convert.ToString(SvaDT.Rows[0]["Message"]);
            }

            return Json(message);
        }

        public JsonResult GetExportAcList(string search)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("USP_SearchExportAccountDetails'" + search + "'");

            var summaryDet = JsonConvert.SerializeObject(dt);
            if (dt.Rows.Count > 0)
            {
                dt.Columns.Remove("EDIT");
            }

            Session["SearchAccountDetails"] = dt;
            Session["SearchAccountDetailssearch"] = search;
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult ExportTariffSetting()
        {
            BE.ImportTraiffSettingEntities ImportTariffSettingList = new BE.ImportTraiffSettingEntities();
            ImportTariffSettingList = reportprovider.ExporttrariffSettingDropDown();
            ViewBag.ImportaccountMaster = new SelectList(ImportTariffSettingList.ImportaccountMaster, "AccountID", "AccountName");
            ViewBag.CommodityMaster = new SelectList(ImportTariffSettingList.CommodityMaster, "Commodity_Group_ID", "Commodity_Group_Name");
            ViewBag.ImportInvoiceType = new SelectList(ImportTariffSettingList.ImportInvoiceType, "ID", "InvoiceType");
            ViewBag.ChagresBasedOn = new SelectList(ImportTariffSettingList.ChagresBasedOn, "Chargeid", "BasedOn");
            ViewBag.SettingTax = new SelectList(ImportTariffSettingList.SettingTax, "Settingid", "TaxName");
            ViewBag.TransportType_m = new SelectList(ImportTariffSettingList.TransportType_m, "TransportID", "TransportType");
            ViewBag.ImportJoType = new SelectList(ImportTariffSettingList.ImportJoType, "Jo_type_id", "Jo_type");
            ViewBag.PortsEntites = new SelectList(ImportTariffSettingList.PortsEntites, "Portid", "PortName");
            ViewBag.CargoEntititesList = new SelectList(ImportTariffSettingList.CargoEntititesList, "cargoid", "cargoname");
            List<BE.ExportEnt> Location = new List<BE.ExportEnt>();
            Location = reportprovider.getLocation();
            ViewBag.Location = new SelectList(Location, "LocationID", "Location");


            List<BE.ContainerSize> Containersize = new List<BE.ContainerSize>();
            Containersize = reportprovider.GetSizeDetails();
            ViewBag.ContainerSize = new SelectList(Containersize, "ContainerSizeID", "ContainerSizeName");

            List<BE.DeliveryTypeDetails> DeliveryType = new List<BE.DeliveryTypeDetails>();
            DeliveryType = reportprovider.GetDeliveryDetails();
            ViewBag.DeliveryType = new SelectList(DeliveryType, "DeliveryID", "DeliveryType");


            List<BE.SlabDetails> SlabDetails = new List<BE.SlabDetails>();
            SlabDetails = reportprovider.GetSlabDetails();
            ViewBag.SlabDetailsList = new SelectList(SlabDetails, "SlabId", "SlabId");




            List<BE.importtariffdetails> importtariffdetails = new List<BE.importtariffdetails>();
            importtariffdetails = reportprovider.Getimporttariffdetails();
            ViewBag.importtariffdetails = new SelectList(importtariffdetails, "TariffID", "TariffDescription");


            List<BE.ExportEnt> ContainerType = new List<BE.ExportEnt>();
            ContainerType = reportprovider.getContainerType();
            ViewBag.ContainerType = new SelectList(ContainerType, "ContainerTypeID", "ContainerType");



            List<BE.ImportAccountMaster> AccountHead = new List<BE.ImportAccountMaster>();
            AccountHead = reportprovider.GetAccountHead();
            ViewBag.AccountHead = new SelectList(AccountHead, "AccountID", "AccountName");
            return View();


        }
        public JsonResult ExportTariffSettingDetailsForUserdelete()
        {

            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            dt = db.sub_GetDatatable("USP_DeleteExporttariff '" + Userid + "'");

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult GetImportTariffSettingSummary()
        {
            DataTable dtscList = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dtscList = db.sub_GetDatatable("SP_ExportTariffDetails");

            string json = JsonConvert.SerializeObject(dtscList);
            dtscList.Columns.Remove("Edit");
            Session["Exporttariffmaster"] = dtscList;
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult getPendingReceipt(string chargesBasedOn, string size, string accountingHead)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataSet dataTable = new DataSet();
            DataTable tblDetails = new DataTable();
            int Userid = Convert.ToInt32(Session["userid"]);

            dataTable = db.sub_GetDataSets("[USP_EXP_SLABS] '" + chargesBasedOn + "','" + size + "','" + accountingHead + "'");

            List<BE.Slab> ddl = new List<BE.Slab>();
            if (dataTable.Tables[0] != null)
            {
                foreach (DataRow row in dataTable.Tables[0].Rows)
                {
                    BE.Slab details = new BE.Slab();


                    details.SlabID = Convert.ToString(row["slabid"]);
                    details.SlabName = Convert.ToString(row["slabid"]);
                    ddl.Add(details);
                }
            }
            var jsonResult = Json(ddl, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetSlabExportTariffDetails(string slabID)
        {

            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("SELECT slabon as [Slab ON],fromslab as [From Slab],toslab as [To Slab],value as [Value] FROM Exp_slabs WHERE SlabId='" + slabID + "'");

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetExportTariffDetails(string SearchType)
        {

            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_Search_Exp_Tariff '" + SearchType + "'");

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult TariffValidation(List<BE.SlabDetailsEntites> Invoicedata)
        {
            string message = "";
            string message1 = "";

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("SlabOn");
            dataTable.Columns.Add("RangeFrom");
            dataTable.Columns.Add("RangeTo");
            dataTable.Columns.Add("Value");
            dataTable.Columns.Add("SlabSize");
            dataTable.Columns.Add("SlabCargoType");

            foreach (BE.SlabDetailsEntites item in Invoicedata)
            {
                if (item.SlabCargoType != "ALL")
                {
                    DataRow row = dataTable.NewRow();

                    row["SlabOn"] = item.SlabOn;
                    row["RangeFrom"] = item.RangeFrom;
                    row["RangeTo"] = item.RangeTo;
                    row["Value"] = item.Value;
                    row["SlabSize"] = item.SlabSize;
                    row["SlabCargoType"] = item.SlabCargoType;

                    dataTable.Rows.Add(row);
                }
            }








            message = reportprovider.ImportValidation(dataTable);
            if (message != "")
            {

                message1 += message;
            }
            else
            {
                message1 = "New";
            }
            return new JsonResult() { Data = message1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
        [HttpPost]
        public ActionResult SaveExportSlabDetails(List<BE.SlabDetailsEntites> Invoicedata, string TariffID, List<BE.TariffAddDetailsEntites> DeliveryType1,
           string Accounting, string AccountingID,
           string Location, string StuffLocation, string Trailertype)
        {


            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("SlabOn");
            dataTable.Columns.Add("RangeFrom");
            dataTable.Columns.Add("RangeTo");
            dataTable.Columns.Add("Value");
            dataTable.Columns.Add("SlabSize");
            dataTable.Columns.Add("SlabCargoType");
            dataTable.Columns.Add("Location");

            foreach (BE.SlabDetailsEntites item in Invoicedata)
            {
                DataRow row = dataTable.NewRow();
                row["SlabOn"] = item.SlabOn;
                row["RangeFrom"] = item.RangeFrom;
                row["RangeTo"] = item.RangeTo;
                row["Value"] = item.Value;
                row["SlabSize"] = item.SlabSize;
                row["SlabCargoType"] = item.SlabCargoType;
                row["Location"] = item.Location;
                dataTable.Rows.Add(row);
            }

            int Userid = Convert.ToInt32(Session["userid"]);
            List<BE.TariffAddDetailsEntites> Getdetails = new List<BE.TariffAddDetailsEntites>();
            List<BE.TariffAddDetailsEntites> Getdetails1 = new List<BE.TariffAddDetailsEntites>();
            DataTable dataTable1 = new DataTable();

            dataTable1.Columns.Add("DeliveryType");
            foreach (BE.TariffAddDetailsEntites item in DeliveryType1)
            {
                DataRow row = dataTable1.NewRow();

                row["DeliveryType"] = item.DeliveryType;
                dataTable1.Rows.Add(row);
            }

            Getdetails = reportprovider.SaveExportCargoDetails(dataTable, Userid, TariffID, dataTable1, Accounting, AccountingID, Location, StuffLocation);


            //Data To insert into 
            DataTable dataTableadd = new DataTable();


            dataTableadd.Columns.Add("TariffID");
            dataTableadd.Columns.Add("DeliveryType11");
            dataTableadd.Columns.Add("From");
            dataTableadd.Columns.Add("TO");
            dataTableadd.Columns.Add("Accounting");
            dataTableadd.Columns.Add("Size");
            dataTableadd.Columns.Add("Type1");
            dataTableadd.Columns.Add("Ftype");
            dataTableadd.Columns.Add("Slabid");
            dataTableadd.Columns.Add("Insurance");
            dataTableadd.Columns.Add("FixedAmt");
            dataTableadd.Columns.Add("rate");
            dataTableadd.Columns.Add("Tax");
            dataTableadd.Columns.Add("InvoiceType");
            dataTableadd.Columns.Add("CurrencyType");
            dataTableadd.Columns.Add("TransType");
            dataTableadd.Columns.Add("Port");
            dataTableadd.Columns.Add("Freedays");
            dataTableadd.Columns.Add("Location");
            dataTableadd.Columns.Add("StuffLocation");
            dataTableadd.Columns.Add("Jotype");
            dataTableadd.Columns.Add("ScanType");
            dataTableadd.Columns.Add("AccountingID");
            dataTableadd.Columns.Add("Days"); ;


            foreach (BE.TariffAddDetailsEntites item in Getdetails)
            {
                DataRow row1 = dataTableadd.NewRow();

                row1["TariffID"] = item.TariffID;
                row1["DeliveryType11"] = item.DeliveryType;
                row1["From"] = item.From;
                row1["TO"] = item.TO;
                row1["Accounting"] = item.Accounting;
                row1["Size"] = item.Size;
                row1["Type1"] = item.Type;
                row1["Ftype"] = item.Ftype;
                row1["Slabid"] = item.Slabid;
                row1["Insurance"] = item.Insurance;
                row1["FixedAmt"] = item.FixedAmt;
                row1["rate"] = item.rate;
                row1["Tax"] = item.Tax;
                row1["InvoiceType"] = item.InvoiceType;
                row1["CurrencyType"] = item.CurrencyType;
                row1["TransType"] = item.TransType;
                row1["Port"] = item.Port;
                row1["Freedays"] = item.Freedays;
                row1["Location"] = item.Location;
                row1["StuffLocation"] = item.StuffLocation;
                row1["Jotype"] = StuffLocation;
                row1["ScanType"] = item.ScanType;
                row1["AccountingID"] = item.AccountingID;
                row1["Days"] = item.Days;

                dataTableadd.Rows.Add(row1);
            }
            int userId = Convert.ToInt32(Session["userid"]);
            Getdetails1 = reportprovider.Export1SavedataForGetdata(dataTableadd, userId, Trailertype);

            //data to end insert

            var jsonResult = Json(Getdetails1, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        [HttpPost]
        public ActionResult SaveExportTariffSettingDetails(List<BE.TariffAddDetailsEntites> ImportData, string commodity, string Fromdate, string Todate)
        {


            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("TariffID");
            dataTable.Columns.Add("DeliveryType");
            dataTable.Columns.Add("From");
            dataTable.Columns.Add("TO");
            dataTable.Columns.Add("Accounting");
            dataTable.Columns.Add("Size");
            dataTable.Columns.Add("Type");
            dataTable.Columns.Add("Ftype");
            dataTable.Columns.Add("Slabid");
            dataTable.Columns.Add("Insurance");
            dataTable.Columns.Add("FixedAmt");
            dataTable.Columns.Add("rate");
            dataTable.Columns.Add("Tax");
            dataTable.Columns.Add("InvoiceType");
            dataTable.Columns.Add("CurrencyType");
            dataTable.Columns.Add("TransType");
            dataTable.Columns.Add("Port");
            dataTable.Columns.Add("Freedays");
            dataTable.Columns.Add("Location");
            dataTable.Columns.Add("StuffLocation");
            dataTable.Columns.Add("Jotype");
            dataTable.Columns.Add("ScanType");
            dataTable.Columns.Add("AccountingID");
            dataTable.Columns.Add("IsSplit");

            foreach (BE.TariffAddDetailsEntites item in ImportData)
            {
                DataRow row = dataTable.NewRow();

                row["TariffID"] = item.TariffID;
                row["DeliveryType"] = item.DeliveryType;
                row["From"] = item.From;
                row["TO"] = item.TO;
                row["Accounting"] = item.Accounting;
                row["Size"] = item.Size;
                row["Type"] = item.Type;
                row["Ftype"] = item.Ftype;
                row["Slabid"] = item.Slabid;
                row["Insurance"] = item.Insurance;
                row["FixedAmt"] = item.FixedAmt;
                row["rate"] = item.rate;
                row["Tax"] = item.Tax;
                row["InvoiceType"] = item.InvoiceType;
                row["CurrencyType"] = item.CurrencyType;
                row["TransType"] = item.TransType;
                row["Port"] = item.Port;
                row["Freedays"] = item.Freedays;
                row["Location"] = item.Location;
                row["StuffLocation"] = item.StuffLocation;
                row["Jotype"] = item.Jotype;
                row["ScanType"] = item.ScanType;
                row["AccountingID"] = item.AccountingID;
                row["IsSplit"] = item.IsSplit;
                dataTable.Rows.Add(row);
            }

            int Userid = Convert.ToInt32(Session["userid"]);
            string message = reportprovider.ExportCheckEffective(dataTable, Userid);

            if (message == "")
            {
                message = reportprovider.SaveExportSettingTariff(dataTable, Userid, commodity, Fromdate, Todate);
            }
            else
            {

            }



            return Json(message);

        }
        public JsonResult GetInvoiceTyprForSlabDetailsExport(int AccountID)
        {

            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            dt = db.sub_GetDatatable("USP_ShowAccountDetailsExport '" + AccountID + "'");
            string message = "";
            string message1 = "";

            if (dt.Rows.Count > 0)
            {
                message = Convert.ToString(dt.Rows[0].Field<int>("InvoiceTypeID"));
                message1 = Convert.ToString(dt.Rows[0].Field<int>("TaxID"));
            }



            string Getdetails = " " + message + " ," + message1 + "";


            var jsonResult = Json(Getdetails, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult AjaxGetExportTariffDetailsForaddtable(string TariffID, string From, string TO, string Accounting
, string Size, string ChargesBased, string FixedAmt, string Days, string rate
, string Slabid, string ScanType, string Location, string StuffLocation, string Jotype, string Commodity, string TransType,
string Port, string Insurance, string AccountingID, List<BE.TariffAddDetailsEntites> DeliveryType1, List<BE.TariffAddDetailsEntites> Type1, string TaxID, string InvoiceType, string IsSplit)
        {

            List<BE.TariffAddDetailsEntites> Getdetails = new List<BE.TariffAddDetailsEntites>();
            List<BE.TariffAddDetailsEntites> Getdetails1 = new List<BE.TariffAddDetailsEntites>();
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("DeliveryType");


            foreach (BE.TariffAddDetailsEntites item in DeliveryType1)
            {
                DataRow row = dataTable.NewRow();

                row["DeliveryType"] = item.DeliveryType;


                dataTable.Rows.Add(row);
            }
            DataTable dataTable1 = new DataTable();

            dataTable1.Columns.Add("Type");


            foreach (BE.TariffAddDetailsEntites item in Type1)
            {
                DataRow row = dataTable1.NewRow();

                row["Type"] = item.Type;


                dataTable1.Rows.Add(row);
            }
            Getdetails = reportprovider.GetExportAddTabledetails(TariffID, From, TO, Accounting, Size, ChargesBased, FixedAmt, Days, rate, Slabid, ScanType, Location, StuffLocation,
            Jotype, Commodity, TransType, Port, Insurance, AccountingID, dataTable, dataTable1, TaxID, InvoiceType, IsSplit);


            //Data To insert into 
            DataTable dataTableadd = new DataTable();

            dataTableadd.Columns.Add("TariffID");
            dataTableadd.Columns.Add("DeliveryType11");
            dataTableadd.Columns.Add("From");
            dataTableadd.Columns.Add("TO");
            dataTableadd.Columns.Add("Accounting");
            dataTableadd.Columns.Add("Size");
            dataTableadd.Columns.Add("Type1");
            dataTableadd.Columns.Add("Ftype");
            dataTableadd.Columns.Add("Slabid");
            dataTableadd.Columns.Add("Insurance");
            dataTableadd.Columns.Add("FixedAmt");
            dataTableadd.Columns.Add("rate");
            dataTableadd.Columns.Add("Tax");
            dataTableadd.Columns.Add("InvoiceType");
            dataTableadd.Columns.Add("CurrencyType");
            dataTableadd.Columns.Add("TransType");
            dataTableadd.Columns.Add("Port");
            dataTableadd.Columns.Add("Freedays");
            dataTableadd.Columns.Add("Location");
            dataTableadd.Columns.Add("StuffLocation");
            dataTableadd.Columns.Add("Jotype");
            dataTableadd.Columns.Add("ScanType");
            dataTableadd.Columns.Add("AccountingID");
            dataTableadd.Columns.Add("Days");
            dataTableadd.Columns.Add("IsSplit");


            foreach (BE.TariffAddDetailsEntites item in Getdetails)
            {
                DataRow row1 = dataTableadd.NewRow();

                row1["TariffID"] = item.TariffID;
                row1["DeliveryType11"] = item.DeliveryType;
                row1["From"] = item.From;
                row1["TO"] = item.TO;
                row1["Accounting"] = item.Accounting;
                row1["Size"] = item.Size;
                row1["Type1"] = item.Type;
                row1["Ftype"] = item.Ftype;
                row1["Slabid"] = item.Slabid;
                row1["Insurance"] = item.Insurance;
                row1["FixedAmt"] = item.FixedAmt;
                row1["rate"] = item.rate;
                row1["Tax"] = item.Tax;
                row1["InvoiceType"] = item.InvoiceType;
                row1["CurrencyType"] = item.CurrencyType;
                row1["TransType"] = item.TransType;
                row1["Port"] = item.Port;
                row1["Freedays"] = item.Freedays;
                row1["Location"] = item.Location;
                row1["StuffLocation"] = item.StuffLocation;
                row1["Jotype"] = item.Jotype;
                row1["ScanType"] = item.ScanType;
                row1["AccountingID"] = item.AccountingID;
                row1["Days"] = item.Days;
                row1["IsSplit"] = item.IsSplit;
                dataTableadd.Rows.Add(row1);
            }
            int userId = Convert.ToInt32(Session["userid"]);
            Getdetails1 = reportprovider.ExportSavedataForGetdata(dataTableadd, userId);

            //data to end insert

            var jsonResult = Json(Getdetails1, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult ExporttariffSettingDetails(string TariffID, string Deliverytype, string Containertype)
        {
            List<BE.TariffAddDetailsEntites> CancelDetails = new List<BE.TariffAddDetailsEntites>();
            CancelDetails = reportprovider.GetexporttariffDetails(TariffID, Deliverytype, Containertype);

            var jsonResult = Json(CancelDetails, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public ActionResult CancelDetailsForExportTariff(List<BE.TariffAddDetailsEntites> TariffNo)
        {


            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Entryid");



            foreach (BE.TariffAddDetailsEntites item in TariffNo)
            {
                DataRow row = dataTable.NewRow();

                row["Entryid"] = item.Entryid;


                dataTable.Rows.Add(row);
            }

            int Userid = Convert.ToInt32(Session["Tracker_userID"]);
            string message = reportprovider.CancelDetailsTariff(dataTable, Userid);
            return Json(message);

        }
        public ActionResult ExportProformaInvoice()
        {
            BE.ExportProformaInvoice GetDetails = new BE.ExportProformaInvoice();
            GetDetails = reportprovider.ProformaFillDropDownList();

            ViewBag.Customer = new SelectList(GetDetails.ExportProformaCusomter, "CustomerID", "CustomerName");
            ViewBag.Shipper = new SelectList(GetDetails.ExportProformaShipper, "shipperID", "shippername");
            ViewBag.Cha = new SelectList(GetDetails.ExportProformaCha, "ChaID", "ChaName");
            ViewBag.Transtype = new SelectList(GetDetails.ExportProformaTransType, "Transport_Type_ID", "Transport_Type");
            ViewBag.BillType = new SelectList(GetDetails.ExportProformaBillType, "TypeID", "BillType");
            ViewBag.Tariffmaster = new SelectList(GetDetails.ExportProformaTariffmaster, "TariffID", "TariffDescription");
            ViewBag.Accountmaster = new SelectList(GetDetails.ExportProformaAccountmaster, "AccountID", "AccountName");
            ViewBag.Location = new SelectList(GetDetails.ExportProformaLocation, "LocationID", "LocationName");
            ViewBag.Allotment = new SelectList(GetDetails.ExportProformaAllotment, "ID", "Name");
            ViewBag.Comodity = new SelectList(GetDetails.ExportProformaCommodity, "Commodity_Group_ID", "Commodity");
            ViewBag.Tax_Services = new SelectList(GetDetails.ExportProformaTax_Service, "ID", "Tax_Type_Desc");
            ViewBag.InvoiceType = new SelectList(GetDetails.ExportProformaInvoiceType, "InvoiceTypeID", "InvoiceType");

            List<BE.Company> Company = new List<BE.Company>();
            Company = reportprovider.GetCompanydetails();
            ViewBag.Company = new SelectList(Company, "CompanyID", "CompanyName");


            List<BE.CargoTypes> CargoList = new List<BE.CargoTypes>();
            CargoList = reportprovider.CargoType();
            ViewBag.CargoList = new SelectList(CargoList, "Cargotypeid", "Cargotype");
            ViewBag.CargotypeList = JsonConvert.SerializeObject(CargoList);
            return View();
        }
        public ActionResult ExportProformaGSTSearch()
        {

            return PartialView();
        }
        public JsonResult ExportGSTSearch(string SearchText)
        {
            List<BE.ImportProformaSearchGST> GstList = new List<BE.ImportProformaSearchGST>();
            GstList = reportprovider.GetExportGSTList(SearchText);
            var jsonResult = Json(GstList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult SBNumberStuffing(string SbNumber)
        {

            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dataTable = new DataTable();
            List<BE.StuffingEntry> receiptEntries = new List<BE.StuffingEntry>();
            dataTable = db.sub_GetDatatable("usp_show_SBNo '" + SbNumber + "'");
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    BE.StuffingEntry receiptEntry = new BE.StuffingEntry();

                    receiptEntry.StuffedQty = Convert.ToString(row["SBQty"]);
                    receiptEntry.StuffedWeight = Convert.ToString(row["SBWeight"]);
                    receiptEntry.CartedQty = Convert.ToString(row["CartedQty"]);
                    receiptEntry.CusName = Convert.ToString(row["Name"]);
                    receiptEntry.SbEntryID = Convert.ToString(row["sbentryID"]);
                    receiptEntry.CargoTypeID = Convert.ToString(row["cargotypeid"]);

                    receiptEntries.Add(receiptEntry);
                }
            }

            var jsonResult = Json(receiptEntries, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public JsonResult ContainerNoStuffing(string ContainerNo)
        {

            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dataTable = new DataTable();
            List<BE.StuffingEntry> receiptEntries = new List<BE.StuffingEntry>();
            dataTable = db.sub_GetDatatable("usp_show_ContainerNo '" + ContainerNo + "'");
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    BE.StuffingEntry receiptEntry = new BE.StuffingEntry();

                    receiptEntry.ContainerNo = Convert.ToString(row["ContainerNo"]);
                    receiptEntry.Size = Convert.ToString(row["Size"]);
                    receiptEntry.ContainerType = Convert.ToString(row["ContainerType"]);
                    receiptEntry.CusName = Convert.ToString(row["Name"]);
                    receiptEntry.TareWeight = Convert.ToString(row["TareWeight"]);
                    receiptEntry.GateInNo = Convert.ToString(row["gateInNo"]);
                    receiptEntry.AgentSeal = Convert.ToString(row["AgentSeal"]);

                    receiptEntries.Add(receiptEntry);
                }
            }

            var jsonResult = Json(receiptEntries, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public JsonResult CheckContaineralready_in_invoiceDone(string Containerno)
        {
            string strSQL = "";
            string Message = "";
            DataTable dt = new DataTable();
            DataTable CheclDL = new DataTable();
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["Tracker_userID"]);
            dt = db.sub_GetDatatable("USP_Check_If_generated_DOckInv  '" + Containerno + "'");
            BE.TariffDetailsForExport Obj = new BE.TariffDetailsForExport();
            BE.ImportProformaSearchGST oper = new BE.ImportProformaSearchGST();
            if (dt.Rows.Count > 0)
            {

                strSQL = "SELECT TOP 1 tariffid,  assessdate, gstid, gstname, GSTIn_uniqID  FROM Exp_assessM I inner join party_gst_m M on i.partyid=m.gstid WHERE invno= '" + dt.Rows[0]["Invoice No"] + "' AND IsCancel=0 order by assessdate desc";
                CheclDL = db.sub_GetDatatable(strSQL);

                if (CheclDL.Rows.Count > 0)
                {
                    foreach (DataRow row in CheclDL.Rows)
                    {

                        oper.GSTIn_uniqID = Convert.ToString(row["GSTIn_uniqID"]);
                        oper.GSTName = Convert.ToString(row["GSTName"]);

                        oper.GSTID = Convert.ToString(row["GSTID"]);
                        oper.TariffIDSaved = Convert.ToString(row["tariffid"]);

                    }
                }

                Message = "For this Container No Assessment has generated for no " + (dt.Rows[0]["Invoice No"]) + " on dated " + Convert.ToDateTime(dt.Rows[0]["Invoice Date"]).ToString("dd-MMM-yyyy HH:mm") + " and valid upto " + Convert.ToDateTime(dt.Rows[0]["ValidUptoDate"]).ToString("dd-MMM-yyyy HH:mm") + ". Click Yes to another invoice And No to exit?";


            }

            var returnField = new { ContainerDetails = oper, message = Message };
            return new JsonResult() { Data = returnField, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult GetContainerWiseProformaDetails(string Containerno, string MovementType)
        {

            List<BE.ContainerWiseProformaDetails> GetDetails = new List<BE.ContainerWiseProformaDetails>();
            GetDetails = reportprovider.GetContainerWiseDetailForExportProforma(Containerno, MovementType);

            List<BE.ShippingBIllDetailsForExportProforma> Shippingbill = new List<BE.ShippingBIllDetailsForExportProforma>();
            ///Shippingbill = reportprovider.GetContainerWiseDetailForExportProformaShippingbill(Containerno);



            var returnField = new { ContainerDetails = GetDetails, ShippingDetails = Shippingbill };
            return new JsonResult() { Data = returnField, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
        public ActionResult GetSBWiseProformaDetails(string SBNumber, string BillType)
        {

            List<BE.ContainerWiseProformaDetails> GetDetails = new List<BE.ContainerWiseProformaDetails>();
            GetDetails = reportprovider.GetsbWiseDetailForExportProforma(SBNumber, BillType);

            List<BE.ShippingBIllDetailsForExportProforma> Shippingbill = new List<BE.ShippingBIllDetailsForExportProforma>();
            Shippingbill = reportprovider.GetsbWiseDetailForExportProformaShippingbill(SBNumber, BillType);



            var returnField = new { ContainerDetails = GetDetails, ShippingDetails = Shippingbill };
            return new JsonResult() { Data = returnField, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
        [HttpPost]
        public ActionResult CalculateExportPerformaDetails(string InvoiceDate, string BlNo, string GSTNO, string GSTID, string GSTName, string TransType, string PickUP, string StuffLocation,
    string Commodity, string RRno, string RRDate, string Customer, string CHA, string Importer, string Line, string CargoDesc, string Portname, string Remakrs, string TariffID, string TariffDesc,
    string FreeDays, string Empty, string Storage, string StateCode, string Movementtype, string SBNO, string TAxID, string Containerno, string ValidUpto, string InvoiceType, string MovementtypeName, bool additionalcheck,
    List<BE.ContainerWiseProformaDetails> tablearrayCont, List<BE.additionalAccountDetails> tableAccountadditional, string GSTtype)
        {

            try
            {
                HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
                DataTable dataTable = new DataTable();
                Dictionary<object, object> parameterList = new Dictionary<object, object>();
                string strSQL = "";

                dataTable.Columns.Add("ContainerNo");
                dataTable.Columns.Add("Size");
                dataTable.Columns.Add("ContainerType");
                dataTable.Columns.Add("CargoType");
                dataTable.Columns.Add("indate");
                dataTable.Columns.Add("StuffingDate");
                dataTable.Columns.Add("Gpdate");
                dataTable.Columns.Add("EmptyDays");
                dataTable.Columns.Add("LoadedDays");
                dataTable.Columns.Add("PickUP");
                dataTable.Columns.Add("Transport_Type");
                dataTable.Columns.Add("NetWeight");
                dataTable.Columns.Add("TareWeight");
                dataTable.Columns.Add("GrossWeight");
                dataTable.Columns.Add("MovementType");
                dataTable.Columns.Add("SBNumber");
                dataTable.Columns.Add("LineID");
                dataTable.Columns.Add("SLName");
                dataTable.Columns.Add("Port");
                dataTable.Columns.Add("PortName");
                dataTable.Columns.Add("PickupLocID");
                dataTable.Columns.Add("StuffLocID");
                dataTable.Columns.Add("NoOfStuffLoc");
                dataTable.Columns.Add("entryID");

                foreach (BE.ContainerWiseProformaDetails item in tablearrayCont)
                {
                    DataRow row = dataTable.NewRow();

                    row["ContainerNo"] = item.ContainerNo;
                    row["Size"] = item.Size;
                    row["ContainerType"] = item.ContainerType;
                    row["CargoType"] = item.CargoType;
                    row["indate"] = item.indate;
                    row["StuffingDate"] = item.StuffingDate;
                    row["Gpdate"] = item.Gpdate;
                    row["EmptyDays"] = item.EmptyDays;
                    row["LoadedDays"] = item.LoadedDays;
                    row["PickUP"] = item.PickUP;
                    row["Transport_Type"] = item.Transport_Type;

                    row["NetWeight"] = item.NetWeight;
                    row["TareWeight"] = item.TareWeight;
                    row["GrossWeight"] = item.GrossWeight;
                    row["MovementType"] = item.MovementType;
                    row["SBNumber"] = item.SBNumber;
                    row["LineID"] = item.LineID;
                    row["SLName"] = item.SLName;
                    row["Port"] = item.Port;
                    row["PortName"] = item.PortName;
                    row["PickupLocID"] = item.PickupLocID;
                    row["StuffLocID"] = item.StuffLocID;
                    row["NoOfStuffLoc"] = item.NoOfStuffLoc;
                    row["entryID"] = item.entryID;

                    dataTable.Rows.Add(row);
                }


#pragma warning disable CS0168 // The variable 'intChargesCounter' is declared but never used
                int intChargesCounter;
#pragma warning restore CS0168 // The variable 'intChargesCounter' is declared but never used
                double dblNetAmount_IND;

#pragma warning disable CS0168 // The variable 'blnContainerFound' is declared but never used
                bool blnContainerFound;
#pragma warning restore CS0168 // The variable 'blnContainerFound' is declared but never used
                int intContCounter;
                DataTable dtp = new DataTable();
#pragma warning disable CS0168 // The variable 'dblAssessNo' is declared but never used
                double dblAssessNo;
#pragma warning restore CS0168 // The variable 'dblAssessNo' is declared but never used
#pragma warning disable CS0168 // The variable 'dblCartingAssessno' is declared but never used
                double dblCartingAssessno;
#pragma warning restore CS0168 // The variable 'dblCartingAssessno' is declared but never used
#pragma warning disable CS0168 // The variable 'dblMainAssessNo' is declared but never used
                double dblMainAssessNo;
#pragma warning restore CS0168 // The variable 'dblMainAssessNo' is declared but never used

#pragma warning disable CS0168 // The variable 'dblTempSTax' is declared but never used
                double dblTempSTax;
#pragma warning restore CS0168 // The variable 'dblTempSTax' is declared but never used

#pragma warning disable CS0168 // The variable 'dblDestuffDate' is declared but never used
                DateTime dblDestuffDate;
#pragma warning restore CS0168 // The variable 'dblDestuffDate' is declared but never used
#pragma warning disable CS0168 // The variable 'dblSQM' is declared but never used
                double dblSQM;
#pragma warning restore CS0168 // The variable 'dblSQM' is declared but never used
#pragma warning disable CS0168 // The variable 'dblDestuffDays' is declared but never used
                double dblDestuffDays;
#pragma warning restore CS0168 // The variable 'dblDestuffDays' is declared but never used
                DataTable dtQ = new DataTable();
                DataTable AddDatable = new DataTable();
#pragma warning disable CS0168 // The variable 'dblDestuffWeek' is declared but never used
                double dblDestuffWeek;
#pragma warning restore CS0168 // The variable 'dblDestuffWeek' is declared but never used
#pragma warning disable CS0168 // The variable 'dblweight' is declared but never used
                double dblweight;
#pragma warning restore CS0168 // The variable 'dblweight' is declared but never used
#pragma warning disable CS0168 // The variable 'blIsIGMWise' is declared but never used
                bool blIsIGMWise;
#pragma warning restore CS0168 // The variable 'blIsIGMWise' is declared but never used
#pragma warning disable CS0168 // The variable 'dblPerc' is declared but never used
                double dblPerc;
#pragma warning restore CS0168 // The variable 'dblPerc' is declared but never used
#pragma warning disable CS0168 // The variable 'intRowCount' is declared but never used
                int intRowCount;
#pragma warning restore CS0168 // The variable 'intRowCount' is declared but never used
#pragma warning disable CS0168 // The variable 'dblIGST' is declared but never used
                double dblIGST;
#pragma warning restore CS0168 // The variable 'dblIGST' is declared but never used
#pragma warning disable CS0168 // The variable 'dblCGST' is declared but never used
                double dblCGST;
#pragma warning restore CS0168 // The variable 'dblCGST' is declared but never used
#pragma warning disable CS0168 // The variable 'dblSGST' is declared but never used
                double dblSGST;
#pragma warning restore CS0168 // The variable 'dblSGST' is declared but never used
#pragma warning disable CS0168 // The variable 'dblSumSGSTAmt' is declared but never used
                double dblSumSGSTAmt;
#pragma warning restore CS0168 // The variable 'dblSumSGSTAmt' is declared but never used
#pragma warning disable CS0168 // The variable 'dblSumCGSTAmt' is declared but never used
                double dblSumCGSTAmt;
#pragma warning restore CS0168 // The variable 'dblSumCGSTAmt' is declared but never used
#pragma warning disable CS0168 // The variable 'dblSumIGSTAmt' is declared but never used
                double dblSumIGSTAmt;
#pragma warning restore CS0168 // The variable 'dblSumIGSTAmt' is declared but never used
#pragma warning disable CS0168 // The variable 'dblSumNetAmtTotal' is declared but never used
                double dblSumNetAmtTotal;
#pragma warning restore CS0168 // The variable 'dblSumNetAmtTotal' is declared but never used
                int CurrentYear = DateTime.Today.Year;
                int PreviousYear = DateTime.Today.Year - 1;
                int NextYear = DateTime.Today.Year + 1;
                string FinYear = null;

                if (DateTime.Today.Month > 3)
                {
                    FinYear = CurrentYear.ToString() + "-" + NextYear.ToString().Remove(0, 2);
                }
                else
                {
                    FinYear = PreviousYear.ToString() + "-" + CurrentYear.ToString().Remove(0, 2);
                }
                string workyear = FinYear.Trim();
                double dblIGSTax = 0;
                double dblCGSTax = 0;
                double dblSGSTax = 0;
                double dblKCFTax = 0;
#pragma warning disable CS0219 // The variable 'dbltaxgroupid' is assigned but its value is never used
                double dbltaxgroupid = 11;
#pragma warning restore CS0219 // The variable 'dbltaxgroupid' is assigned but its value is never used

                DataTable additionaldataTable = new DataTable();
                DataTable CmtdataTable = new DataTable();
                if (tableAccountadditional != null)
                {

                    additionaldataTable.Columns.Add("AccountNameAdditional");
                    additionaldataTable.Columns.Add("ContainernoAdditional");
                    additionaldataTable.Columns.Add("AmountAdditional");
                    additionaldataTable.Columns.Add("Accountadditional");


                    foreach (BE.additionalAccountDetails item in tableAccountadditional)
                    {
                        DataRow row = additionaldataTable.NewRow();

                        row["AccountNameAdditional"] = item.AccountNameAdditional;
                        row["ContainernoAdditional"] = item.ContainernoAdditional;
                        row["AmountAdditional"] = item.AmountAdditional;
                        row["Accountadditional"] = item.Accountadditional;

                        additionaldataTable.Rows.Add(row);
                    }

                }


                int Userid = Convert.ToInt32(Session["userID"]);
                strSQL = "";
                strSQL = " Delete From  Temp_Export_assessD Where UserID=" + Userid + "";
                dtp = db.sub_GetDatatable(strSQL);
                strSQL = "";
                strSQL = " Delete From Temp_Export_assessDII Where UserID=" + Userid + "";
                dtp = db.sub_GetDatatable(strSQL);
                string message = "";

                CmtdataTable = db.sub_GetDatatable("GETcommodityWise_TaxID '" + Commodity + "'");
                string CmdWise = "";

                // to check commodity


                if (CmtdataTable.Rows.Count > 0)
                {
                    CmdWise = Convert.ToString(CmtdataTable.Rows[0][3]);

                }

                for (int k = 0; k <= dataTable.Rows.Count - 1; k++)
                {

                    dtQ.Clear();
                    strSQL = "SELECT TOP 1 * FROM Export_tariffdetails WHERE TariffID='" + TariffID + "' and " + " deliverytype='" + dataTable.Rows[k].Field<string>("MovementType") + "' and " + Convert.ToDateTime(InvoiceDate).ToString("yyyyMMdd") + " BETWEEN EffectiveFrom and EffectiveUpto  AND IsApproved=1  " + "  AND (Containertype='" + dataTable.Rows[k].Field<string>("ContainerType") + "' or Containertype='All') AND Containersize='" + dataTable.Rows[k].Field<string>("Size") + "'";
                    dtQ = db.sub_GetDatatable(strSQL);

                    if (dtQ.Rows.Count == 0)

                        message = "Tariff ID: " + TariffDesc + " not found in database for Cargo Type-size " + dataTable.Rows[k].Field<string>("CargoType") + "  " + dataTable.Rows[k].Field<string>("Size") + "  . Please contact your administrator!";

                }

                if (Movementtype != "SSR")
                {
                    string strSQL3;
                    for (intContCounter = 0; intContCounter <= dataTable.Rows.Count - 1; intContCounter++)
                    {

                        DataTable dtRSFetch = new DataTable();
                        DataTable GetInDcharges = new DataTable();
                        strSQL3 = "";
                        strSQL3 = "   USP_Export_TARIFF_FETCH_ALL '" + TariffID + "' ,'" + Movementtype + "'  ,'" + dataTable.Rows[intContCounter].Field<string>("MovementType") + "' , " + "   '" + dataTable.Rows[intContCounter].Field<string>("CargoType") + "', " + dataTable.Rows[intContCounter].Field<string>("Size") + "" + " ,'" + dataTable.Rows[intContCounter].Field<string>("Port") + "' ";
                        dtRSFetch = db.sub_GetDatatable(strSQL3);
                        if (dtRSFetch.Rows.Count > 0)
                        {
                            for (int j = 0; j <= dtRSFetch.Rows.Count - 1; j++)
                            {
                                dblNetAmount_IND = 0;
                                DataTable dtRSTemp1 = new DataTable();
                                int Getasscountid = Convert.ToInt32(dtRSFetch.Rows[j].Field<string>("AccountID"));
                                strSQL = "";
                                strSQL = " SELECT DISTINCT AccountName  FROM Exp_AccountMaster WHERE AccountID=" + Convert.ToInt32(dtRSFetch.Rows[j].Field<string>("AccountID"));
                                dtRSTemp1 = db.sub_GetDatatable(strSQL);
                                if (dtRSTemp1.Rows.Count > 0)
                                {


                                    strSQL = "";
                                    strSQL = "USP_IMP_SUB_FETCHCHARGES_IND_export '" + dataTable.Rows[intContCounter].Field<string>("entryID") + "','" + dataTable.Rows[intContCounter].Field<string>("ContainerNo") + "',";
                                    strSQL += "'" + dataTable.Rows[intContCounter].Field<string>("Cargotype") + "','" + dataTable.Rows[intContCounter].Field<string>("Size") + "','" + Convert.ToInt32(dtRSFetch.Rows[j].Field<string>("AccountID")) + "','" + Convert.ToInt32(dataTable.Rows[intContCounter].Field<string>("LoadedDays")) + "','" + dataTable.Rows[intContCounter].Field<string>("GrossWeight") + "',";
                                    strSQL += "'" + Convert.ToDateTime(dataTable.Rows[intContCounter].Field<string>("InDate")).ToString("dd MMM yyyy HH:mm") + "','" + dataTable.Rows[intContCounter].Field<string>("TareWeight") + "','" + dataTable.Rows[intContCounter].Field<string>("MovementType") + "',";
                                    strSQL += "'" + Convert.ToInt32(dataTable.Rows[intContCounter].Field<string>("Port")) + "','" + Convert.ToInt32(dataTable.Rows[intContCounter].Field<string>("PickupLocID")) + "','" + dataTable.Rows[intContCounter].Field<string>("StuffLocID") + "',";
                                      strSQL += "'" + Convert.ToInt32(dataTable.Rows[intContCounter].Field<string>("NoOfStuffLoc")) + "','" + TariffID + "','" + 0 + "','" + TransType + "','" + Commodity + "','" + Convert.ToDateTime(ValidUpto).ToString("dd MMM yyyy HH:mm") + "','" + InvoiceType + "'";
                                    GetInDcharges = db.sub_GetDatatable(strSQL);

                                    double dblAmount = 0;
                                    if (GetInDcharges.Rows.Count > 0)
                                    {
                                        dblNetAmount_IND = Convert.ToDouble(GetInDcharges.Rows[0][0]);
                                    }

                                    string COntainerant = dataTable.Rows[intContCounter].Field<string>("ContainerNo");
                                    //'''**************************** ADDITIONAL CHARGES ''

                                    if (additionalcheck == true)
                                    {
                                        if (additionaldataTable.Rows.Count > 0)
                                        {
                                            for (int e = 0; e < additionaldataTable.Rows.Count; e++)
                                            {
                                                if (additionaldataTable.Rows[e].Field<string>("AmountAdditional") == "")
                                                {
                                                    if (additionalcheck == true && Getasscountid == Convert.ToDouble(additionaldataTable.Rows[e].Field<string>("Accountadditional")))
                                                    {
                                                        dblAmount = 0;
                                                    }
                                                }
                                                if (additionaldataTable.Rows[e].Field<string>("AmountAdditional") != "")
                                                {
                                                    if (additionaldataTable.Rows[e].Field<string>("ContainernoAdditional") == null)
                                                    {
                                                        if (additionalcheck == true && Getasscountid == Convert.ToDouble(additionaldataTable.Rows[e].Field<string>("Accountadditional")))
                                                        {
                                                            if (additionaldataTable.Rows[e].Field<string>("AccountNameAdditional") != "" && Getasscountid == Convert.ToDouble(additionaldataTable.Rows[e].Field<string>("Accountadditional")))
                                                            {
                                                                dblAmount = dblAmount + Convert.ToDouble(additionaldataTable.Rows[e].Field<string>("AmountAdditional"));
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (additionalcheck == true && Getasscountid == Convert.ToDouble(additionaldataTable.Rows[e].Field<string>("Accountadditional")) && additionaldataTable.Rows[e].Field<string>("ContainernoAdditional") == COntainerant)
                                                        {
                                                            if (additionaldataTable.Rows[e].Field<string>("AccountNameAdditional") != "" && Getasscountid == Convert.ToDouble(additionaldataTable.Rows[e].Field<string>("Accountadditional")))
                                                            {
                                                                dblAmount = dblAmount + Convert.ToDouble(additionaldataTable.Rows[e].Field<string>("AmountAdditional"));
                                                            }

                                                        }

                                                    }
                                                }

                                            }
                                        }
                                    }

                                    dblNetAmount_IND = dblNetAmount_IND + dblAmount;
                                }
#pragma warning disable CS0219 // The variable 'Datetime' is assigned but its value is never used
                                DateTime Datetime = new DateTime();
#pragma warning restore CS0219 // The variable 'Datetime' is assigned but its value is never used


                                if (dblNetAmount_IND > 0)
                                {


                                    DataTable DTRSfetchForGST = new DataTable();
                                    strSQL3 = "";

                                    strSQL = "";
                                    strSQL = "  select * from exp_accountmaster WHERE accountid='" + Convert.ToInt32(dtRSFetch.Rows[0].Field<string>("AccountID")) + "'";
                                    dtp = db.sub_GetDatatable(strSQL);


                                    DataTable dtget = new DataTable();
                                    if (dtp.Rows.Count > 0)
                                    {
                                        strSQL = "";
                                        strSQL = "SELECT TOP 1 * FROM settings_taxes WHERE  settingsID='" + dtp.Rows[0].Field<int>("taxid") + "' and " + Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + " BETWEEN EffectiveFrom and EffectiveUpto";
                                        dtget = db.sub_GetDatatable(strSQL);

                                    }
                                    if (dtget.Rows.Count > 0)
                                    {

                                        dblSGSTax = Convert.ToInt16(dtget.Rows[0].Field<double>("SGST"));
                                        dblCGSTax = Convert.ToInt16(dtget.Rows[0].Field<double>("SGST"));
                                        dblIGSTax = Convert.ToInt16(dtget.Rows[0].Field<double>("IGST"));
                                        ///dblKCFTax = Convert.ToInt16(dtget.Rows[0].Field<double>("KFC"));
                                        TAxID = Convert.ToString(dtget.Rows[0].Field<Int16>("settingsID"));
                                    }

                                    strSQL = "";
                                    strSQL = "SELECT TOP 1 * FROM Settings";
                                    dtget = db.sub_GetDatatable(strSQL);

                                    if (GSTtype == "Registered")
                                    {
                                        dblKCFTax = 0;
                                    }


                                    if (StateCode == dtget.Rows[0].Field<string>("tinnumber"))
                                    {
                                        dblIGSTax = 0;
                                    }
                                    else
                                    {
                                        dblSGSTax = 0;
                                        dblCGSTax = 0;
                                    }

                                    if (CmdWise == "False")
                                    {
                                        dblIGSTax = 0;
                                        dblSGSTax = 0;
                                        dblCGSTax = 0;

                                    }



                                    int intTotalDay = 0;
                                    int FreeDays1 = 0;
                                    int inrchargable = 0;
                                    double dblemptydays = 0;

                                    intTotalDay = Convert.ToInt32(dataTable.Rows[intContCounter].Field<string>("LoadedDays"));
                                    dblemptydays = Convert.ToInt32(dataTable.Rows[intContCounter].Field<string>("EmptyDays"));

                                    if (FreeDays1 - Convert.ToInt32(dataTable.Rows[intContCounter].Field<string>("LoadedDays")) > 0)
                                        inrchargable = 0;
                                    else
                                        inrchargable = Convert.ToInt32(dataTable.Rows[intContCounter].Field<string>("LoadedDays")) - FreeDays1;
                                    string strSQL2;
                                    strSQL2 = "";
                                    strSQL2 = " Exec USP_Insert_Temp_Export_assessD 0,'" + workyear + "','" + dataTable.Rows[intContCounter].Field<string>("entryID") + "','" + dataTable.Rows[intContCounter].Field<string>("ContainerNo") + "' , ";
                                    strSQL2 += " '" + dataTable.Rows[intContCounter].Field<string>("GrossWeight") + "','" + dataTable.Rows[intContCounter].Field<string>("MovementType") + "', ";
                                    strSQL2 += " '" + Convert.ToDateTime(dataTable.Rows[intContCounter].Field<string>("indate")).ToString("dd-MMM-yyyy HH:mm") + "','" + intTotalDay + "','" + FreeDays1 + "','" + inrchargable + "','" + Convert.ToInt32(dtRSFetch.Rows[j].Field<string>("AccountID")) + "',";
                                    strSQL2 += " '" + dblNetAmount_IND + "','" + (Convert.ToDecimal(dblNetAmount_IND) * Convert.ToDecimal(dblSGSTax)) / 100 + "','" + (Convert.ToDecimal(dblNetAmount_IND) * Convert.ToDecimal(dblCGSTax)) / 100 + "',";
                                    strSQL2 += " '" + (Convert.ToDecimal(dblNetAmount_IND) * Convert.ToDecimal(dblIGSTax)) / 100 + "','" + TAxID + "', ";
                                    strSQL2 += " '" + SBNO + "','" + SBNO + "','" + Userid + "','" + dataTable.Rows[intContCounter].Field<string>("TareWeight") + "','" + dataTable.Rows[intContCounter].Field<string>("NetWeight") + "','', " + dblemptydays + " , '" + Convert.ToDateTime(dataTable.Rows[intContCounter].Field<string>("StuffingDate")).ToString("dd-MMM-yyyy HH:mm") + "','" + (Convert.ToDecimal(dblNetAmount_IND) * Convert.ToDecimal(dblKCFTax)) / 100 + "' ";
                                    AddDatable = db.sub_GetDatatable(strSQL2);

                                }
                            }
                        }

                    }
                }

                int intcntcount;
                for (intcntcount = 0; intcntcount <= dataTable.Rows.Count - 1; intcntcount++)
                {

                    dtp.Clear();
                    DataTable Amount1 = new DataTable();
                    strSQL = "";
                    strSQL = "  select e.accountid, SUM(amount) as amount,ContainerNo, em.taxid as taxcode  FROM exp_additional e inner join exp_accountmaster em on em.accountid=e.accountid  WHERE containerno = '" + dataTable.Rows[intcntcount].Field<string>("ContainerNo") + "' AND  ";
                    strSQL += " e.entryid='" + dataTable.Rows[intcntcount].Field<string>("entryID") + "' and ReceiptNo=0 and TransNo=0 and IsCancel=0 and ContainerNo is not null  GROUP BY e.accountID, Containerno, em.taxid  ";
                    Amount1 = db.sub_GetDatatable(strSQL);
                    if (Amount1.Rows.Count > 0)
                    {
                        for (int j = 0; j <= Amount1.Rows.Count - 1; j++)
                        {
                            if (Convert.ToInt64(Amount1.Rows[j].Field<Int64>("AccountID")) != 0)
                            {
                                DataTable DTfetch = new DataTable();
                                DataTable DTfetch1 = new DataTable();
                                strSQL = "";
                                strSQL = " select settingsid taxid, taxname from  settings_taxes   where  settingsid= " + Convert.ToInt64(Amount1.Rows[j].Field<Int32>("taxcode")) + " ";
                                DTfetch = db.sub_GetDatatable(strSQL);


                                strSQL = "";
                                strSQL = " select * from Commodity_Group_M where Commodity_Group_ID=" + Commodity + "";
                                DTfetch1 = db.sub_GetDatatable(strSQL);

                                int intTotalDay = 0;
                                int FreeDays1 = 0;
                                int inrchargable = 0;
                                double dblemptydays = 0;

                                intTotalDay = Convert.ToInt32(dataTable.Rows[intcntcount].Field<string>("LoadedDays"));
                                dblemptydays = Convert.ToInt32(dataTable.Rows[intcntcount].Field<string>("EmptyDays"));

                                if (FreeDays1 - Convert.ToInt32(dataTable.Rows[intcntcount].Field<string>("LoadedDays")) > 0)
                                    inrchargable = 0;
                                else
                                    inrchargable = Convert.ToInt32(dataTable.Rows[intcntcount].Field<string>("LoadedDays")) - FreeDays1;


                                if (DTfetch.Rows.Count > 0)
                                {

                                    DataTable DTRSfetchForGST = new DataTable();


                                    strSQL = "";
                                    strSQL = "  select * from exp_accountmaster WHERE accountid='" + Convert.ToInt64(Amount1.Rows[j].Field<Int64>("AccountID")) + "'";
                                    dtp = db.sub_GetDatatable(strSQL);


                                    DataTable dtget = new DataTable();
                                    if (dtp.Rows.Count > 0)
                                    {
                                        strSQL = "";
                                        strSQL = "SELECT TOP 1 * FROM settings_taxes WHERE  settingsID='" + dtp.Rows[0].Field<int>("taxid") + "' and " + Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + " BETWEEN EffectiveFrom and EffectiveUpto";
                                        dtget = db.sub_GetDatatable(strSQL);

                                    }
                                    if (dtget.Rows.Count > 0)
                                    {

                                        dblSGSTax = Convert.ToInt16(dtget.Rows[0].Field<double>("SGST"));
                                        dblCGSTax = Convert.ToInt16(dtget.Rows[0].Field<double>("SGST"));
                                        dblIGSTax = Convert.ToInt16(dtget.Rows[0].Field<double>("IGST"));
                                        dblKCFTax = Convert.ToInt16(dtget.Rows[0].Field<double>("KFC"));
                                        TAxID = Convert.ToString(dtget.Rows[0].Field<Int16>("settingsID"));
                                    }

                                    strSQL = "";
                                    strSQL = "SELECT TOP 1 * FROM Settings";
                                    dtget = db.sub_GetDatatable(strSQL);
                                    if (GSTtype == "Registered")
                                    {
                                        dblKCFTax = 0;
                                    }
                                    if (StateCode == dtget.Rows[0].Field<string>("tinnumber"))
                                    {
                                        dblIGSTax = 0;
                                    }
                                    else
                                    {
                                        dblSGSTax = 0;
                                        dblCGSTax = 0;
                                    }

                                    if (CmdWise == "False")
                                    {
                                        dblIGSTax = 0;
                                        dblSGSTax = 0;
                                        dblCGSTax = 0;

                                    }
                                    strSQL = "";
                                    strSQL = " Exec USP_Insert_Temp_Export_assessD 0,'" + workyear + "','" + dataTable.Rows[intcntcount].Field<string>("entryID") + "','" + dataTable.Rows[intcntcount].Field<string>("ContainerNo") + "' , ";
                                    strSQL += " '" + dataTable.Rows[intcntcount].Field<string>("GrossWeight") + "','" + dataTable.Rows[intcntcount].Field<string>("MovementType") + "', ";
                                    strSQL += " '" + Convert.ToDateTime(dataTable.Rows[intcntcount].Field<string>("indate")).ToString("dd-MMM-yyyy HH:mm") + "','" + intTotalDay + "','" + FreeDays1 + "','" + inrchargable + "','" + Convert.ToInt32(Amount1.Rows[j].Field<string>("AccountID")) + "',";
                                    strSQL += " '" + Convert.ToInt64(Amount1.Rows[j][1]) + "','" + (Convert.ToDecimal(Convert.ToInt64(Amount1.Rows[j][1]) * Convert.ToDecimal(dblSGSTax)) / 100) + "','" + (Convert.ToDecimal(Convert.ToInt64(Amount1.Rows[j][1]) * Convert.ToDecimal(dblCGSTax)) / 100) + "',";
                                    strSQL += " '" + (Convert.ToDecimal(Convert.ToInt64(Amount1.Rows[j][1]) * Convert.ToDecimal(dblIGSTax)) / 100) + "','" + TAxID + "', ";
                                    strSQL += " '" + SBNO + "','" + SBNO + "','" + Userid + "','" + dataTable.Rows[intcntcount].Field<string>("TareWeight") + "','" + dataTable.Rows[intcntcount].Field<string>("NetWeight") + "','', " + dblemptydays + " , '" + Convert.ToDateTime(dataTable.Rows[intcntcount].Field<string>("StuffingDate")).ToString("dd-MMM-yyyy HH:mm") + "','" + (Convert.ToDecimal(Amount1.Rows[j][1]) * Convert.ToDecimal(dblKCFTax)) / 100 + "'";
                                    AddDatable = db.sub_GetDatatable(strSQL);
                                }
                            }
                        }
                    }

                }
                

                dtp.Clear();


                strSQL = "";
                strSQL = " Exec sp_export_inv_charges '" + SBNO + "','" + Userid + "'";
                dtp = db.sub_GetDatatable(strSQL);

                string json = JsonConvert.SerializeObject(dtp);



                BE.TextBoXValuesForImportPerforma GetContainerAmtdetails = new BE.TextBoXValuesForImportPerforma();
                strSQL = "";
                strSQL = "get_sum_charges_Export_TMT '" + SBNO + "','" + workyear + "'," + Userid + "   ";
                dtp = db.sub_GetDatatable(strSQL);
                if (dtp.Rows.Count > 0)
                {

                    foreach (DataRow row in dtp.Rows)
                    {

                        GetContainerAmtdetails.SGST = Convert.ToDouble(row["SGST"]);
                        GetContainerAmtdetails.CGST = Convert.ToDouble(row["CGST"]);
                        GetContainerAmtdetails.IGST = Convert.ToDouble(row["IGST"]);
                        GetContainerAmtdetails.Amount = Convert.ToDouble(row["Amount"]);
                        GetContainerAmtdetails.KFC = Convert.ToDouble(row["KFC"]);
                        GetContainerAmtdetails.nettotal = Convert.ToDouble(row["Amount"]) + Convert.ToDouble(row["SGST"]) + Convert.ToDouble(row["CGST"]) + Convert.ToDouble(row["IGST"]);
                    }

                }
                var returnField = new { GetContainerSHowList = json, ContainerAmtShowList = GetContainerAmtdetails };
                return new JsonResult() { Data = returnField, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public ActionResult SaveExpoerinvoicePerformaDetails(string InvoiceDate, string BlNo, string GSTNO, string GSTID, string GSTName, string TransType, string PickUP, string StuffLocation,
    string Commodity, string RRno, string RRDate, string Customer, string CHA, string Importer, string Line, string CargoDesc, string Portname, string Remakrs, string TariffID, string TariffDesc,
    string FreeDays, string Empty, string Storage, string StateCode, string Movementtype, string SBNO, string TAxID, string Containerno,
             string netamount, string SGST, string CGST, string IGST, string GrandTotal, string shipperID,
             string ValidDate, List<BE.ShippingBIllDetailsForExportProforma> tableShippingbill, string KFC, string Company)
        {
            try
            {
                DataTable dataTable = new DataTable();
                Dictionary<object, object> parameterList = new Dictionary<object, object>();
                dataTable.Columns.Add("SBNumber");
                dataTable.Columns.Add("SBDate");
                dataTable.Columns.Add("CartingDate");
                dataTable.Columns.Add("StuffingDate");
                dataTable.Columns.Add("TotalDays");
                dataTable.Columns.Add("CartingQty");
                dataTable.Columns.Add("CartingWeight");
                dataTable.Columns.Add("Area");
                dataTable.Columns.Add("Space");
                dataTable.Columns.Add("CargoDescriptions");
                dataTable.Columns.Add("VehicleNo");
                dataTable.Columns.Add("entryid");
                dataTable.Columns.Add("CargoWeight");
                dataTable.Columns.Add("TotalPKGS");

                if (tableShippingbill != null)
                {
                    foreach (BE.ShippingBIllDetailsForExportProforma item in tableShippingbill)
                    {
                        DataRow row = dataTable.NewRow();

                        row["SBNumber"] = item.SBNumber;
                        row["SBDate"] = item.SBDate;
                        row["CartingDate"] = item.CartingDate;
                        row["StuffingDate"] = item.StuffingDate;
                        row["TotalDays"] = item.TotalDays;
                        row["CartingQty"] = item.CartingQty;
                        row["CartingWeight"] = item.CartingWeight;
                        row["Area"] = item.Area;
                        row["Space"] = item.Space;
                        row["CargoDescriptions"] = item.CargoDescriptions;
                        row["VehicleNo"] = item.VehicleNo;

                        row["entryid"] = item.entryid;
                        row["CargoWeight"] = item.CargoWeight;
                        row["TotalPKGS"] = item.TotalPKGS;


                        dataTable.Rows.Add(row);
                    }
                }



                Int64 intid = 0;
                string strinvoiceNo = "";
                Int64 txtassessno = 0;
                DataTable dtwo = new DataTable();
                DataTable dt12 = new DataTable();
                DataTable DTfetch1 = new DataTable();
#pragma warning disable CS0219 // The variable 'dbltaxcategoryid' is assigned but its value is never used
                double dbltaxcategoryid = 0;
#pragma warning restore CS0219 // The variable 'dbltaxcategoryid' is assigned but its value is never used
                HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();


                string strSQL1;
                string strSQL;



                int CurrentYear = DateTime.Today.Year;
                int PreviousYear = DateTime.Today.Year - 1;
                int NextYear = DateTime.Today.Year + 1;
                string FinYear = null;

                if (DateTime.Today.Month > 3)
                {
                    FinYear = CurrentYear.ToString() + "-" + NextYear.ToString().Remove(0, 2);
                }
                else
                {
                    FinYear = PreviousYear.ToString() + "-" + CurrentYear.ToString().Remove(0, 2);
                }
                string strWorkYear = FinYear.Trim();




                int Userid = Convert.ToInt32(Session["Tracker_userID"]);
                string strSGSTPer = "";
                string strCGSTPer = "";
                string strIGSTPer = "";
                strSQL = "";
                strSQL = " select * from Commodity_Group_M where Commodity_Group_ID=" + Commodity + "";
                DTfetch1 = db.sub_GetDatatable(strSQL);
                if (Convert.ToInt32(DTfetch1.Rows[0].Field<Int32>("TaxGroupID")) != 0)
                {
                    DataTable dtgroupid = new DataTable();
                    strSQL = "";
                    strSQL = "UPDATE Temp_export_AssessD SEt taxgroupid=11 WHERE  UserID=" + Userid + " and IGMNo='" + SBNO + "' ";
                    dt12 = db.sub_GetDatatable(strSQL);
                }
                string strctrsb = "";
                strctrsb = Containerno + SBNO;

                strSGSTPer = "SGST" + " @ " + 9 + "%";
                strCGSTPer = "CGST" + " @ " + 9 + "%";
                strIGSTPer = "IGST" + " @ " + 18 + "%";
                strSQL = "";
                strSQL = "SELECT isnull(MAX(AssessNo),0)+1 as[exp_assessno] FROM export_ProformaM ";
                DataTable dt1 = new DataTable();
                dt1 = db.sub_GetDatatable(strSQL);
                intid = Convert.ToInt64(dt1.Rows[0].Field<Int64>("exp_assessno"));
                txtassessno = Convert.ToInt64(dt1.Rows[0].Field<Int64>("exp_assessno"));
#pragma warning disable CS0168 // The variable 'strinvyear' is declared but never used
                string strinvyear;
#pragma warning restore CS0168 // The variable 'strinvyear' is declared but never used
                int allowcount = (int)(Math.Log10(intid) + 1);
                string str = strWorkYear;
                str = str.Remove(0, 5);


                if (allowcount == 1)
                {
                    strinvoiceNo = "EXP/" + "0000" + intid + "/" + str;
                }
                else if (allowcount == 2)
                {
                    strinvoiceNo = "EXP/" + "000" + intid + "/" + str;
                }
                else if (allowcount == 3)
                {
                    strinvoiceNo = "EXP/" + "00" + intid + "/" + str;
                }
                else if (allowcount == 4)
                {
                    strinvoiceNo = "EXP/" + "0" + intid + "/" + str;
                }
                else if (allowcount == 5)
                {
                    strinvoiceNo = "EXP/" + "" + intid + "/" + str;
                }


                //if (Commodity == "1" || Commodity == "2")
                //{
                //    dbltaxcategoryid = Convert.ToDouble(Commodity);
                //}

                //else
                //{
                //    dbltaxcategoryid = 0;
                //}




                strSQL1 = "";
                strSQL1 = " EXEC USP_INSERT_EXP_ASSESSM '" + txtassessno + "','" + strWorkYear + "', '" + strinvoiceNo + "', ";
                strSQL1 += " '" + Convert.ToDateTime(InvoiceDate).ToString("dd-MMM-yyyy HH:mm") + "','" + Movementtype + "',";
                strSQL1 += " '" + TariffID + "','" + Customer + "','" + shipperID + "','" + CHA + "','" + 0 + "','" + SBNO + "','" + Convert.ToDateTime(ValidDate).ToString("dd-MMM-yyyy HH:mm") + "' ,";
                strSQL1 += " '" + netamount + "','" + 0 + "','" + 0 + "','" + GrandTotal + "','" + "" + "','" + 0 + "','" + 0 + "','" + 0 + "','" + Remakrs + "',";
                strSQL1 += " '" + Userid + "','" + Convert.ToDateTime(DateTime.Now).ToString("dd-MMM-yyyy HH:mm") + "','" + "P" + "','" + strSGSTPer + "','" + strCGSTPer + "', '" + strIGSTPer + "',";
                strSQL1 += " '" + 0 + "','" + Convert.ToDateTime(DateTime.Now).ToString("dd-MMM-yyyy HH:mm") + "','" + "NULL" + "','" + 0 + "','" + 0 + "','" + Convert.ToDateTime(DateTime.Now).ToString("dd-MMM-yyyy HH:mm") + "','" + Importer + "','" + "Null" + "','" + 0 + "','" + 0 + "','" + 0 + "','" + 0 + "','" + 0 + "','" + 0 + "','" + SGST + "','" + IGST + "','" + CGST + "','" + GSTID + "','" + 0 + "','" + TAxID + "', '" + CargoDesc + "', '" + Portname + "'," + Commodity + " ," + 0 + " ,'" + RRno + "', '" + RRDate + "','" + BlNo + "','" + KFC + "','" + Company + "'";
                dt1 = db.sub_GetDatatable(strSQL1);


                DataTable dt = new DataTable();
                strSQL = "";
                strSQL = "UPDATE settings_assess SEt EXP_ProformaNo=" + intid + " WHERE EXP_ProformaWY='" + strWorkYear + "'";
                dt1 = db.sub_GetDatatable(strSQL);

                strSQL = "";
                strSQL = " exec USP_Insert_Export_assessD " + Userid + ",'" + SBNO + "' , " + intid + ", '" + strWorkYear + "' ,'" + strctrsb + "'";
                dt1 = db.sub_GetDatatable(strSQL);

                //   sb details
                for (int k = 0; k <= dataTable.Rows.Count - 1; k++)
                {
                    strSQL = "";
                    strSQL = " Exec USP_INSERT_EXPORT_ASSESSSB '" + txtassessno + "','" + strWorkYear + "',0,'" + dataTable.Rows[k].Field<string>("SBNumber") + "',";
                    strSQL += " '" + Convert.ToDateTime(dataTable.Rows[k].Field<string>("CartingDate")).ToString("dd-MMM-yyyy HH:mm") + "', '" + dataTable.Rows[k].Field<string>("CargoDescriptions") + "', '" + dataTable.Rows[k].Field<string>("CartingWeight") + "',  '" + dataTable.Rows[k].Field<string>("CartingQty") + "',";
                    strSQL += " '" + dataTable.Rows[k].Field<string>("Area") + "', '" + dataTable.Rows[k].Field<string>("Space") + "', '" + Convert.ToDateTime(dataTable.Rows[k].Field<string>("StuffingDate")).ToString("dd-MMM-yyyy HH:mm") + "',  '" + dataTable.Rows[k].Field<string>("TotalDays") + "',0,  '" + dataTable.Rows[k].Field<string>("VehicleNo") + "', '" + Convert.ToDateTime(dataTable.Rows[k].Field<string>("SBDate")).ToString("dd-MMM-yyyy HH:mm") + "'";
                    dt1 = db.sub_GetDatatable(strSQL);
                }

                string Messageget = "Record Saved Successfully!";
                string message = Messageget + ',' + strinvoiceNo;

                return Json(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult ExportInvoicePerformaPrint(string InvoiceNo)
        {
            DataSet getJobOrderSet = new DataSet();
            DataTable lblGetassessno = new DataTable();
            DataTable tblInvoiceDetails = new DataTable();
            DataTable tblContainerDetails = new DataTable();
            DataTable tblchargesDetails = new DataTable();
            DataTable tblshippingdetails = new DataTable();


            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            double Amount = 0;
            double Discount = 0;
            double NetAmount = 0;
            string Srate = "";
            double SGST = 0;
            string CRate = "";
            double CGST = 0;
            string IRate = "";
            double IGST = 0;

            getJobOrderSet = db.sub_GetDataSets("USP_GetExportProformaPrint '" + InvoiceNo + "'");

            if (getJobOrderSet.Tables.Count > 0)
            {
                lblGetassessno = getJobOrderSet.Tables[0];
                tblInvoiceDetails = getJobOrderSet.Tables[1];
                tblContainerDetails = getJobOrderSet.Tables[2];
                tblchargesDetails = getJobOrderSet.Tables[3];
                tblshippingdetails = getJobOrderSet.Tables[4];



                foreach (DataRow dr in lblGetassessno.Rows)
                {
                    ViewBag.AssessNo = dr["AssessNo"];
                    ViewBag.WorkYear = dr["WorkYear"];
                    ViewBag.InvNo = dr["InvNo"];
                    ViewBag.AssessDate = dr["AssessDate"];
                    ViewBag.assesstype = dr["assesstype"];
                    ViewBag.Port = dr["Port"];
                    ViewBag.CHAName = dr["CHAName"];
                    ViewBag.UserName = dr["UserName"];
                    ViewBag.shippername = dr["shippername"];
                    ViewBag.con_Name = dr["con_Name"];

                    ViewBag.AddressI = dr["AddressI"];
                    ViewBag.CINNo = dr["CINNo"];
                    ViewBag.IMP_AC_EMAILID = dr["IMP_AC_EMAILID"];
                    ViewBag.RegdOffice = dr["RegdOffice"];
                    ViewBag.GSTName = dr["GSTName"];
                    ViewBag.GSTAddress = dr["GSTAddress"];
                    ViewBag.State = dr["State"];
                    ViewBag.state_Code = dr["state_Code"];
                    ViewBag.GSTIn_uniqID = dr["GSTIn_uniqID"];
                    ViewBag.TotalAmountInWords = dr["TotalAmountInWords"];
                    ViewBag.BankName = dr["BankName"];
                    ViewBag.AccountNo = dr["AccountNo"];
                    ViewBag.BranchName = dr["BranchName"];
                    ViewBag.IFSCCode = dr["IFSCCode"];
                    ViewBag.remarks = dr["remarks"];
                    ViewBag.NoteVI = dr["NoteVI"];
                    ViewBag.GSTIN = dr["GSTIN"];
                    ViewBag.Panno = dr["Panno"];
                    ViewBag.con_regOffice = dr["con_regOffice"];


                    ViewBag.TotalAmount = dr["GrandTotal"];
                    ViewBag.AmountWithoutTax = dr["NetTotal"];
                    ViewBag.SGSTAmount = dr["SGST"];
                    ViewBag.CGSTAmount = dr["CGST"];
                    ViewBag.IGSTAmount = dr["IGST"];
                    ViewBag.TaxAmount = dr["TaxTotal"];
                    ViewBag.Datetime = Convert.ToDateTime(DateTime.Now).ToString("dd MM yyyy");
                    ViewBag.TIme = Convert.ToDateTime(DateTime.Now).ToString("HH:mm");
                }


            }

            ViewBag.InvoiceItemList = tblInvoiceDetails.AsEnumerable();
            ViewBag.ContainerDetailsList = tblContainerDetails.AsEnumerable();
            ViewBag.chargesDetails = tblchargesDetails.AsEnumerable();
            ViewBag.ShippingDetails = tblshippingdetails.AsEnumerable();
            foreach (DataRow data in tblchargesDetails.Rows)
            {
                Amount = Amount + Convert.ToDouble(data["Amount"]);
                Discount = Convert.ToDouble(data["Discount"]);
                NetAmount = NetAmount + Convert.ToDouble(data["NetAmount"]);
                Srate = Convert.ToString(data["Srate"]);
                CRate = Convert.ToString(data["CRate"]);
                IRate = Convert.ToString(data["IRate"]);
                SGST = SGST + Convert.ToDouble(data["SGST"]);
                CGST = CGST + Convert.ToDouble(data["CGST"]);
                IGST = IGST + Convert.ToDouble(data["IGST"]);
            }

            ViewBag.Amount = Amount;
            ViewBag.Discount = Discount;
            ViewBag.NetAmount = NetAmount;
            ViewBag.Srate = Srate;
            ViewBag.CRate = CRate;
            ViewBag.IRate = IRate;
            ViewBag.SGST = SGST;
            ViewBag.CGST = CGST;
            ViewBag.IGST = IGST;



            return PartialView();

        }

        public JsonResult GetExportInvoiceporformadetails(string fromdate, string Todate, string searchCerteria, string Searchtext, string Searchtext1)
        {

            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("SP_ExportPerformaInvoiceList '" + fromdate + "','" + Todate + "','" + searchCerteria + "','" + Searchtext + "','" + Searchtext1 + "'");
            dt.Columns.Remove("Sr No");

            string json = JsonConvert.SerializeObject(dt);
            dt.Columns.Remove("Print");
            Session["ExportToExcelPerformaInvoice"] = dt;
            Session["fromdate"] = fromdate;
            Session["Todate"] = Todate;
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult PendingExportProformaForInvoice()
        {
            List<BE.Customer> Customer = new List<BE.Customer>();
            Customer = reportprovider.getParty();
            ViewBag.Customer = new SelectList(Customer, "AGID", "AGName");
            return View();

        }


        public JsonResult GetExportPendingproformaDetails(string SearchCriteria, string Search, string Search1)
        {

            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("SP_PendingExportPerformaInvoiceList '" + SearchCriteria + "','" + Search + "','" + Search1 + "'");

            //CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            //var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            //var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string json = JsonConvert.SerializeObject(dt);
            dt.Columns.Remove("View");
            dt.Columns.Remove("Submit");
            dt.Columns.Remove("Cancel");
            Session["ListOfPendingProformaInvoiceforFinalConfirm"] = dt;
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetExportPendingInvoiceToday(string fromdate, string Todate, string searchCerteria, string Searchtext)
        {

            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("SP_ExportAssessList '" + fromdate + "','" + Todate + "','" + searchCerteria + "','" + Searchtext + "'");
            dt.Columns.Remove("Sr No");
            string json = JsonConvert.SerializeObject(dt);
            dt.Columns.Remove("Print");
            dt.Columns.Remove("Cancel");
            Session["importAssessListPending"] = dt;
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpPost]
        public ActionResult CancelExportinvoiceProforma(string AssessNo, string workyear)
        {
            string message = "";
            int userId = Convert.ToInt32(Session["Tracker_userID"]);
            message = reportprovider.CancelExportInvoicePorforma(AssessNo, workyear, userId);
            return Json(message);
        }
        [HttpPost]
        public ActionResult SubmitExportDetailsEntry(string AssessNo, string workyear, string assesstype, string CompanyID)
        {
            string message = "";
            string strinvoiceNo = "";
            int GetLocation = 0;
            string strsql = "";
            DataTable dt = new DataTable();
            int userId = Convert.ToInt32(Session["userID"]);
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            //if (assesstype == "Dock Stuff")
            //{
            //    message = "ECFS/";
            //}
            //else
            //{
            //    message = reportprovider.SubmitExportDetailsforporforma(AssessNo, workyear, userId);
            //}




           
           
                message = "22441";
                strsql = "";
                strsql = "select isnull(max(AssessNo),0)+ 1 as Exp_assessno from exp_assessm";
                dt = db.sub_GetDatatable(strsql);
                if (dt.Rows.Count > 0)
                {
                    GetLocation = Convert.ToInt32(dt.Rows[0]["Exp_assessno"]);
                }
#pragma warning disable CS0219 // The variable 'maxallowid' is assigned but its value is never used
                int maxallowid = 0;
#pragma warning restore CS0219 // The variable 'maxallowid' is assigned but its value is never used
                //maxallowid = reportprovider.getExportallowid(AssessNo, workyear, userId, CompanyID);
                int allowind = GetLocation;
                int newConAssessNo = allowind;
                string str = workyear;
                //str = str.Remove(0, 5);
                str = "2021";
                int allowcount = (int)(Math.Log10(newConAssessNo) + 1);
                if (allowcount == 1)
                {
                    strinvoiceNo = message + "/" + str + "/" + "0000" + newConAssessNo;
                }
                else if (allowcount == 2)
                {
                    strinvoiceNo = message + "/" + str + "/" + "000" + newConAssessNo;
                }
                else if (allowcount == 3)
                {
                    strinvoiceNo = message + "/" + str + "/" + "00" + newConAssessNo;
                }
                else if (allowcount == 4)
                {
                    strinvoiceNo = message + "/" + str + "/" + "0" + newConAssessNo;
                }
                else if (allowcount == 5)
                {
                    strinvoiceNo = message + "/" + str + "/" + newConAssessNo;
                }

                else if (allowcount > 5)
                {
                    strinvoiceNo = message + "/" + str + "/" + newConAssessNo;
                }


                message = reportprovider.SubmitExportFinalDetails(AssessNo, workyear, userId, strinvoiceNo, allowind, newConAssessNo, CompanyID);

                message = strinvoiceNo;
            
            return Json(message);
        }
        public ActionResult ExportInvoiceTaxPrint(string InvoiceNo)
        {
            DataSet getJobOrderSet = new DataSet();
            DataTable lblGetassessno = new DataTable();
            DataTable tblInvoiceDetails = new DataTable();
            DataTable tblContainerDetails = new DataTable();
            DataTable tblchargesDetails = new DataTable();
            DataTable tblshippingdetails = new DataTable();


            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            double Amount = 0;
            double Discount = 0;
            double NetAmount = 0;
            string Srate = "";
            double SGST = 0;
            string CRate = "";
            double CGST = 0;
            string IRate = "";
            double IGST = 0;

            getJobOrderSet = db.sub_GetDataSets("USP_GetExportTAXInvoicePrint '" + InvoiceNo + "'");

            if (getJobOrderSet.Tables.Count > 0)
            {
                lblGetassessno = getJobOrderSet.Tables[0];
                tblInvoiceDetails = getJobOrderSet.Tables[2];
                tblContainerDetails = getJobOrderSet.Tables[1];
                tblchargesDetails = getJobOrderSet.Tables[3];
                tblshippingdetails = getJobOrderSet.Tables[4];



                foreach (DataRow dr in lblGetassessno.Rows)
                {
                    ViewBag.AssessNo = dr["AssessNo"];
                    ViewBag.con_ID = dr["con_ID"];
                    ViewBag.WorkYear = dr["WorkYear"];
                    ViewBag.InvNo = dr["InvNo"];
                    ViewBag.AssessDate = dr["AssessDate"];
                    ViewBag.assesstype = dr["assesstype"];
                    ViewBag.Port = dr["Port"];
                    ViewBag.CHAName = dr["CHAName"];
                    ViewBag.UserName = dr["UserName"];
                    ViewBag.shippername = dr["shippername"];
                    ViewBag.con_Name = dr["con_Name"];

                    ViewBag.AddressI = dr["AddressI"];
                    ViewBag.CINNo = dr["CINNo"];
                    ViewBag.IMP_AC_EMAILID = dr["IMP_AC_EMAILID"];
                    ViewBag.RegdOffice = dr["RegdOffice"];

                    ViewBag.GSTName = dr["GSTName"];
                    ViewBag.GSTAddress = dr["GSTAddress"];
                    ViewBag.State = dr["State"];
                    ViewBag.state_Code = dr["state_Code"];
                    ViewBag.GSTIn_uniqID = dr["GSTIn_uniqID"];
                    ViewBag.UPINumber = dr["UPINO"];
                    ViewBag.TotalAmountInWords = dr["TotalAmountInWords"];
                    ViewBag.BankName = dr["BankName"];
                    ViewBag.AccountNo = dr["AccountNo"];
                    ViewBag.BranchName = dr["BranchName"];
                    ViewBag.IFSCCode = dr["IFSCCode"];
                    ViewBag.remarks = dr["remarks"];
                    ViewBag.NoteVI = dr["NoteVI"];
                    ViewBag.GSTIN = dr["GSTIN"];
                    ViewBag.UPINumber = dr["UPINO"];
                    ViewBag.Panno = dr["Panno"];
                    ViewBag.INVheader = dr["INVheader"];
                    ViewBag.Irn = dr["Irn"];
                    ViewBag.AckNo = dr["AckNo"];
                    ViewBag.AckDt = dr["AckDt"];
                    ViewBag.SignedQRcode = dr["SignedQRcode"];

                    ViewBag.TotalAmount = dr["GrandTotal"];
                    ViewBag.AmountWithoutTax = dr["NetTotal"];
                    ViewBag.SGSTAmount = dr["SGST"];
                    ViewBag.CGSTAmount = dr["CGST"];
                    ViewBag.IGSTAmount = dr["IGST"];
                    ViewBag.TaxAmount = dr["TaxTotal"];
                    ViewBag.AGName = dr["AGName"];
                    ViewBag.con_NameI = dr["con_NameI"];
                    ViewBag.con_regOffice = dr["con_regOffice"];

                    ViewBag.Datetime = Convert.ToDateTime(DateTime.Now).ToString("dd MM yyyy");
                    ViewBag.TIme = Convert.ToDateTime(DateTime.Now).ToString("HH:mm");



                }


            }

            ViewBag.InvoiceItemList = tblInvoiceDetails.AsEnumerable();
            ViewBag.ContainerDetailsList = tblContainerDetails.AsEnumerable();
            ViewBag.chargesDetails = tblchargesDetails.AsEnumerable();
            ViewBag.ShippingDetails = tblshippingdetails.AsEnumerable();


            foreach (DataRow data in tblchargesDetails.Rows)
            {
                Amount = Amount + Convert.ToDouble(data["Amount"]);
                Discount = Convert.ToDouble(data["Discount"]);
                NetAmount = NetAmount + Convert.ToDouble(data["NetAmount"]);
                Srate = Convert.ToString(data["Srate"]);
                CRate = Convert.ToString(data["CRate"]);
                IRate = Convert.ToString(data["IRate"]);
                SGST = SGST + Convert.ToDouble(data["SGST"]);
                CGST = CGST + Convert.ToDouble(data["CGST"]);
                IGST = IGST + Convert.ToDouble(data["IGST"]);
            }



            ViewBag.Amount = Amount;
            ViewBag.Discount = Discount;
            ViewBag.NetAmount = NetAmount;
            ViewBag.Srate = Srate;
            ViewBag.CRate = CRate;
            ViewBag.IRate = IRate;
            ViewBag.SGST = SGST;
            ViewBag.CGST = CGST;
            ViewBag.IGST = IGST;



            return View();

        }
        [HttpPost]
        public ActionResult ApproveDetailsForExportTariff(List<BE.TariffAddDetailsEntites> TariffNo)
        {


            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Entryid");



            foreach (BE.TariffAddDetailsEntites item in TariffNo)
            {
                DataRow row = dataTable.NewRow();

                row["Entryid"] = item.Entryid;


                dataTable.Rows.Add(row);
            }

            int Userid = Convert.ToInt32(Session["Tracker_userID"]);
            string message = reportprovider.ApproveDetailsExportTariff(dataTable, Userid);
            return Json(message);

        }
        public ActionResult Checkemptyinvenotry(string ContainerNo )
        {
            string message = "";

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_CheckContanierNo '" + ContainerNo  + "'");
            if (dt.Rows.Count > 0)
            {
                message = Convert.ToString(dt.Rows[0][0]);
            }

            return Json(message);
        }
        public ActionResult CheckCONTAINER_DIGIT(string ContainerNo)
        {
            string message = "";

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_CHECK_CONTAINER_DIGIT '" + ContainerNo + "'");
            if (dt.Rows.Count > 0)
            {
                message = Convert.ToString(dt.Rows[0][0]);
            }

            return Json(message);
        }
        public ActionResult InvoicePosting()
        {
            ViewBag.Date = DateTime.Now.ToLocalTime().ToString("dd MMM yyyy");
            return View();
        }


       
        [HttpPost]
        public ActionResult LockInvoiceData(List<BE.CategorywiseInvoice> Invoicedata, String CategoryName)
        {


            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("ID");
            dataTable.Columns.Add("InvoiceNo");
            dataTable.Columns.Add("WorkYear");
            dataTable.Columns.Add("Category");


            dataTable.TableName = "PT_InvoiceLock";

            int Count = 1;
            foreach (BE.CategorywiseInvoice item in Invoicedata)
            {
                DataRow row = dataTable.NewRow();

                row["ID"] = Count++;
                row["InvoiceNo"] = item.InvoiceNo;
                row["WorkYear"] = item.WorkYear;
                row["Category"] = item.Category;
                dataTable.Rows.Add(row);
            }


            string message = reportprovider.LockInvoiceData(dataTable, CategoryName);
            return Json(message);

        }

        public JsonResult GetExporttoExcelData(string Date, string Category)
        {

            List<BE.CategorywiseInvoice> Invoicelist = new List<BE.CategorywiseInvoice>();

            Invoicelist = reportprovider.GetExporttoExcelData(Date, Category);

            return new JsonResult() { Data = Invoicelist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
        public JsonResult FetchePostingPendingInvoice(string Date, string Category)
        {

            List<BE.CategorywiseInvoice> Invoicelist = new List<BE.CategorywiseInvoice>();

            Invoicelist = reportprovider.getePostingPendingInvoice(Date, Category);

            return new JsonResult() { Data = Invoicelist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        public JsonResult GetLedgerDetails()
        {
            List<BE.LedgerDetails> Invoicelist = new List<BE.LedgerDetails>();
            Invoicelist = reportprovider.GetLedgerDetails();

            //return new JsonResult() { Data = Invoicelist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //jsonResult.MaxJsonLength = int.MaxValue;

            var json = JsonConvert.SerializeObject(Invoicelist);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult LockLedgerData(List<BE.LedgerDetails> Invoicedata)
        {

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("LedgerID");
            dataTable.TableName = "LedgerDetails";
            foreach (BE.LedgerDetails item in Invoicedata)
            {
                DataRow row = dataTable.NewRow();

                row["LedgerID"] = item.LedgerID;

                dataTable.Rows.Add(row);
            }
            string message = reportprovider.LockLedgerData(dataTable);
            return Json(message);

        }
#pragma warning disable CS0414 // The field 'ExportController.OutBoundProcessPath' is assigned but its value is never used
        string OutBoundProcessPath = "~/Uploads/ExportToTally/";
#pragma warning restore CS0414 // The field 'ExportController.OutBoundProcessPath' is assigned but its value is never used
        /*public ActionResult XMLDETAILSForLedger()
        {
            //string OutBoundProcessPath = "~/Uploads/ExportToTally/";
            //List<BE.ImportReceipt> Receiptdata = JsonConvert.DeserializeObject(TempData["TallyM"]);
            string datetime = DateTime.Now.ToString("ddMMMyyyyHHmmss");
            string filename = @"\Ledger" + datetime + "_" + ".xml";
            ///
            OutBoundProcessPath = Server.MapPath(OutBoundProcessPath);
            //
            if (!Directory.Exists(OutBoundProcessPath + @"\XML"))
            {
                Directory.CreateDirectory(OutBoundProcessPath + @"\XML");
            }
            DirectoryInfo dir = new DirectoryInfo(OutBoundProcessPath + @"\XML");
            foreach (FileInfo f1 in dir.GetFiles())
            {
                //f1.Delete();//DELETE ALL FILES
            }
            string filePath = OutBoundProcessPath + @"\XML" + filename;



            string strComa = ",";
            string message = "";
            string strFileName = "";
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataTable dtGetLineFileExt = new DataTable();
            string attachment = "";
            StringBuilder strb = new StringBuilder();
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "text/csv";
            Response.AddHeader("Pragma", "public");

            DataTable CompanyMaster = new DataTable();


            string xml = "";
            CompanyMaster = reportprovider.GetLedgerXMLdetails();

            string Strfst = "";
            //for (int i = 0; i <= CompanyMaster.Rows.Count - 1; i++)
            //{ Strfst = Strfst + CompanyMaster.Rows[i][1];
            //    strb.Append(CompanyMaster.Rows[i][1].ToString());
            //    strb.AppendLine();

            //    xml += Convert.ToString(strb);


            //}
            xml = "";
            foreach (DataRow row in CompanyMaster.Rows)
            {
                xml = xml + Convert.ToString(row[1]);
            }



            XmlDocument doc = new XmlDocument();

            xml = UnescapeXMLValue(xml);
            doc.LoadXml(xml.Replace("&apos;", "'").Replace("&quot;", "\"").Replace("&gt;", ">").Replace("&lt;", "<").Replace("&amp;", "&").Replace("\r\n", "").Replace("&", "&amp;"));


            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            // Save the document to a file and auto-indent the output.
            XmlWriter writer = XmlWriter.Create(filePath, settings);
            doc.Save(writer);

            string Message = "";
            {
                FileInfo xmlFile = new FileInfo(OutBoundProcessPath + @"\XML" + filename);
                if (xmlFile.Length > 60)
                {
                    var rarResult = true;
                    if (rarResult == true)
                    {
                        string copyPath = "~/Uploads/ExportToTally/";
                        copyPath = Server.MapPath(copyPath);
                        if (!Directory.Exists(copyPath + @"\PO"))
                        {
                            Directory.CreateDirectory(copyPath + @"\PO");
                        }
                        System.IO.File.Copy(filePath, copyPath + @"\PO\" + filename);
                        byte[] fileBytes = System.IO.File.ReadAllBytes(copyPath + @"\PO\" + filename);
                        return Json(filename);
                    }
                }
                else
                {

                    Message = "This file has nothing to send...";
                    return View("ErrorPage", Message);
                }
                return View("ErrorPage", Message);
            }

        }*/
        public string UnescapeXMLValue(string xml)
        {
            if (xml == null)
                throw new ArgumentNullException("xml");

            return xml.Replace("&apos;", "'").Replace("&quot;", "\"").Replace("&gt;", ">").Replace("&lt;", "<").Replace("&amp;", "&");
        }

        /*public JsonResult ePostingInvoiceGstPortal(List<BE.CategorywiseInvoice> list)
        {

            string message = "";
            string message1 = "";

            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();


            foreach (BE.CategorywiseInvoice item in list)
            {

                DataTable dt = db.sub_GetDatatable("USP_Export_ACK_GST '" + item.InvoiceNo + "','" + 1 + "'");

                message = dt.Rows[0]["msg"].ToString();

                if (message != "")
                {

                    message1 += message;
                }

            }

            return new JsonResult() { Data = message1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
       */


        public JsonResult ePostingInvoiceGstPortal(List<BE.InvoicePosting> list)
        {

            string message = "";
            string message1 = "";

            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();


            foreach (BE.InvoicePosting item in list)
            {

                DataTable dt = db.sub_GetDatatable("USP_Import_ACK_GST '" + item.invno + "','" + 1 + "'");

                message = dt.Rows[0]["msg"].ToString();

                if (message != "")
                {

                    message1 += message;
                }

            }

            return new JsonResult() { Data = message1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #region Generate Export CLP

        public ActionResult GetCLPSummary(string fromdate, string todate)
        {
            DataTable GetCLPSummary = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();

            string fromDateFormatted = Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd HH:mm");
            string toDateFormatted = Convert.ToDateTime(todate).ToString("yyyy-MM-dd HH:mm");

            GetCLPSummary = db.sub_GetDatatable("USP_CLP_SUMMARY_MVC '" + fromDateFormatted + "','" + toDateFormatted + "'");

            Session["GetCreditNoteSummary"] = GetCLPSummary;
            Session["fromdate"] = fromdate;
            Session["todate"] = todate;

            var json = JsonConvert.SerializeObject(GetCLPSummary);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult GenerateCLPSummary(string fromdate, string todate)
        {
            DataTable GenerateCLPSummary = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();

            string fromDateFormatted = Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd HH:mm");
            string toDateFormatted = Convert.ToDateTime(todate).ToString("yyyy-MM-dd HH:mm");

            GenerateCLPSummary = db.sub_GetDatatable("USP_Generate_CLP_SUMMARY_MVC '");

            Session["GenerateCLPSummary"] = GenerateCLPSummary;
            Session["fromdate"] = fromdate;
            Session["todate"] = todate;

            var json = JsonConvert.SerializeObject(GenerateCLPSummary);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public ActionResult GenerateCLP()
        {
            List<BE.ExportEnt> Export = new List<BE.ExportEnt>();
            Export = reportprovider.getCargoType();
            ViewBag.ExportEntry = new SelectList(Export, "Cargotypeid", "Cargotype");

            List<BE.ExportEnt> Equipment = new List<BE.ExportEnt>();
            Equipment = reportprovider.getEquipment();
            ViewBag.Equipment = new SelectList(Equipment, "Equipmentid", "Equipment");


            return View();
        }

        public JsonResult SearchPenndingCLP(string search)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("SearchPendingCLP '" + search + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

  
        public JsonResult GenerateCLPByID(int ID)
        {
            BE.StuffingEntry CustomerData = new BE.StuffingEntry();
            //CustomerData = GS.GenerateCLPDetails(ID);
            DataTable DT = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DT = db.sub_GetDatatable("USP_GenerateCLPDetails  " + ID + "");

            var summaryDet = JsonConvert.SerializeObject(DT);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        
        public JsonResult GenerateCLPSummaryById(int ID)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("GenerateCLPSummaryById '" + ID + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult SaveGeneratedCLPByID(BE.StuffingEntry StuffingEntry)
        {
            string message = "";

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("SaveGeneratedCLPByID '" + StuffingEntry.SBNo + "','" + StuffingEntry.ContainerNo + "','" + StuffingEntry.ViaNumber + "','" + StuffingEntry.VoyageNo + "'");

            if (dt.Rows.Count >= 0)
            {
                message = "SUCCESS";
            }

            return Json(message);
        }

        public JsonResult SearchGeneratedCLPSummary(string search)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("SearchGeneratedCLPSummary '" + search + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportGeneratedCLPPrint(string StuffingNo)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataSet ds = db.sub_GetDataSets("uspPrintGeneratedCLP '" + StuffingNo + "'");
            DataTable tblSBDetails = new DataTable();
            tblSBDetails = ds.Tables[2];
            // ================= TABLE 0 : Con_Details ===================
            if (ds.Tables[0].Rows.Count > 0)
            {
                ViewBag.con_Name = ds.Tables[0].Rows[0]["con_Name"];
                ViewBag.AddressI = ds.Tables[0].Rows[0]["AddressI"];
            }

            // ================= TABLE 1 : Joined Export_Stuffing + Carting ===================
            if (ds.Tables[1].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[1].Rows[0];

                ViewBag.size = dr["size"];
                ViewBag.UANNo = dr["UANNo"];
                ViewBag.Class = dr["Class"];
                ViewBag.Acc_Holder = dr["Acc_Holder"];
                ViewBag.Package = dr["Package"];
                ViewBag.Exporter = dr["Exporter"];
                ViewBag.Consignee = dr["Consignee"];
                ViewBag.CHA = dr["CHA"];
                ViewBag.CargoDescription = dr["CargoDescription"];
                ViewBag.CartedQty = dr["CartedQty"];
                ViewBag.Remarks = dr["Remarks"];
                ViewBag.Line = dr["Line"];
                ViewBag.ContainerType = dr["ContainerType"];
                //ViewBag.VesselName = dr["VesselName"];
                ViewBag.PODID = dr["PODID"];
                ViewBag.FPDID = dr["FPDID"];
                //ViewBag.VoyageNo = dr["VoyageNo"];
                ViewBag.SBQty = dr["SBQty"];
            }

            // ================= TABLE 2 : Export_Stuffing ===================
            if (ds.Tables[2].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[2].Rows[0];

                ViewBag.SBNo = dr["SBNo"];
                ViewBag.AddedOn = Convert.ToDateTime(dr["AddedOn"]).ToString("dd-MMM-yyyy");

                ViewBag.StuffingNo = dr["StuffingNo"];
                ViewBag.ContainerNo = dr["ContainerNo"];
                ViewBag.StuffedQty = dr["StuffedQty"];
                ViewBag.StuffedWeight = dr["StuffedWeight"];
                ViewBag.TareWeight = dr["TareWeight"];
                ViewBag.StuffedDate = Convert.ToDateTime(dr["StuffedDate"]).ToString("dd-MMM-yyyy");

                ViewBag.Cargotype = dr["Cargotype"];
                ViewBag.CustomSeal = dr["CustomSeal"];
                ViewBag.AgentSeal = dr["AgentSeal"];

                ViewBag.ViaNo = dr["ViaNo"];
                ViewBag.VoyageNo = dr["VoyageNo"];
                ViewBag.FOBValue = dr["FOBValue"];
                ViewBag.VesselName = dr["VesselName"];

                decimal totalStuffedQty = 0m;
                decimal totalStuffedWeight = 0m;

                foreach (DataRow row in ds.Tables[2].Rows)
                {
                    if (row["StuffedQty"] != DBNull.Value &&
                        decimal.TryParse(row["StuffedQty"].ToString(), out decimal qty))
                    {
                        totalStuffedQty += qty;
                    }

                    if (row["StuffedWeight"] != DBNull.Value &&
                        decimal.TryParse(row["StuffedWeight"].ToString(), out decimal wt))
                    {
                        totalStuffedWeight += wt;
                    }
                }

                ViewBag.TotalStuffedQty = totalStuffedQty;
                ViewBag.TotalStuffedWeight = totalStuffedWeight;

                ViewBag.SBDetailsList = tblSBDetails.AsEnumerable();
            }

           

            return PartialView();
        }
        //public ActionResult ExportGeneratedCLPPrint(int ID)
        //{

        //    DataTable Stuffing = new DataTable();


        //    HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();


        //    Stuffing = db.sub_GetDatatable("uspPrintGeneratedCLP '" + ID + "'");

        //    if (Stuffing.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in Stuffing.Rows)
        //        {
        //            ViewBag.ID = dr["ID"];
        //            ViewBag.StuffingNo = dr["StuffingNo"];
        //            ViewBag.SBNo = dr["SBNo"];
        //            ViewBag.ContainerNo = dr["ContainerNo"];
        //            ViewBag.StuffedQty = dr["StuffedQty"];
        //            ViewBag.StuffedWeight = dr["StuffedWeight"];
        //            ViewBag.StuffedDate = dr["StuffedDate"];
        //            ViewBag.CustomSeal = dr["CustomSeal"];
        //            ViewBag.AgentSeal = dr["AgentSeal"];
        //            ViewBag.CargoTypeID = dr["CargoTypeID"];
        //            ViewBag.AddedOn = dr["AddedOn"];
        //            ViewBag.AddedBy = dr["AddedBy"];
        //            ViewBag.GateInNo = dr["GateInNo"];
        //            ViewBag.SbEntryID = dr["SbEntryID"];
        //            ViewBag.ViaNo = dr["ViaNo"];
        //            ViewBag.VoyageNo = dr["VoyageNo"];
        //            ViewBag.IsCLP = dr["IsCLP"];
        //            ViewBag.CLPOn = dr["CLPOn"];

        //        }
        //    }

        //    return PartialView();

        //}
        #endregion

        public JsonResult GetDetailsBySBNo(string SBNo, string SBDate)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataTable dt = new DataTable();

             dt = db.sub_GetDatatable("USP_GetDetailsBySBNo '" + SBNo + "','" + SBDate + "'");

            BE.CartingEntry model = new BE.CartingEntry();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                
                model.SBNo = Convert.ToString(row["SBNo"]);
                model.SBDate = Convert.ToString(row["SBDate"]);
                model.SBQty = Convert.ToString(row["SBQty"]);
                model.SBWeight = Convert.ToString(row["SBWeight"]);
                model.UANNo = Convert.ToString(row["UANNo"]);
                model.Class = Convert.ToString(row["Class"]);
                model.PackingGroupID = Convert.ToString(row["PackingGroupID"]);
                model.PackageTypeID = Convert.ToString(row["PackageTypeID"]);
                model.VehicelNo = Convert.ToString(row["VehicelNo"]);
                model.CartedQty = Convert.ToString(row["CartedQty"]);
                model.CartingDate = Convert.ToString(row["CartingDate"]);
                model.CHAID = Convert.ToString(row["CHAID"]);
                model.CHA = Convert.ToString(row["CHA"]);
                model.CUSTOMERID = Convert.ToString(row["CUSTOMERID"]);
                model.CUSTOMER = Convert.ToString(row["CUSTOMER"]);
                model.CargoTypeID = Convert.ToString(row["CargoTypeID"]);
                model.CartingEquipmentID = Convert.ToString(row["EquipmentID"]);
                model.EntryType = Convert.ToString(row["EntryType"]);
                model.Temp = Convert.ToString(row["Temp"]);
                model.Humidity = Convert.ToString(row["Humidity"]);
                model.Vent = Convert.ToString(row["Vent"]);
             
            }

            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }



        public JsonResult ShowPendingGateOut()
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("usp_GetGateOutPendency");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public JsonResult ShowGateOutSummary(DateTime fromDate, DateTime toDate)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            string Fromdate = fromDate.ToString("yyyy-MM-dd");
            string Todate = toDate.ToString("yyyy-MM-dd");
            dt = db.sub_GetDatatable("usp_ShowGateOutSummary'" + Fromdate + "','" + Todate + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportPrintGateOut(string GateOutNo)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            DataSet ds = db.sub_GetDataSets("uspPrintExportGateOut '" + GateOutNo + "'");

            if (ds != null && ds.Tables.Count > 0)
            {
                // ---------------- 0) COMPANY DETAILS ----------------
                DataTable dtCompany = ds.Tables[0];
                if (dtCompany.Rows.Count > 0)
                {
                    DataRow dr = dtCompany.Rows[0];

                    ViewBag.CompanyName = dr["CompanyName"];
                    ViewBag.CompanyAddress = dr["CompanyAddress"];
                }

                // ---------------- 1) HEADER DETAILS ----------------
                if (ds.Tables.Count > 1)
                {
                    DataTable dtHeader = ds.Tables[1];
                    if (dtHeader.Rows.Count > 0)
                    {
                        DataRow dr = dtHeader.Rows[0];

                        ViewBag.GateOutNo = dr["GateOutNo"];
                        ViewBag.OutDate = dr["OutDate"];
                        ViewBag.PortName = dr["PortName"];
                        ViewBag.VesselName = dr["VesselName"];
                        ViewBag.Transporter = dr["Transporter"];
                        ViewBag.Acc_Holder = dr["Acc_Holder"];
                        ViewBag.VehicleNo = dr["VehicleNo"];
                        ViewBag.DriverName = dr["DriverName"];
                        ViewBag.CHA = dr["CHA"];
                        ViewBag.Exporter = dr["Exporter"];
                        ViewBag.IGMNO = dr["SBNo"];
                        ViewBag.ITEMNO = dr["SBDate"];
                    }
                }

                // ---------------- 2) CONTAINER DETAILS ----------------
                if (ds.Tables.Count > 2)
                {
                    ViewBag.ContainerDetails = ds.Tables[2];
                }

                // ---------------- 3) ADDITIONAL VALUES ----------------
                ViewBag.UserName = Session["UserName"];
                ViewBag.PrintDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            }

            return PartialView();
        }



        //public ActionResult ExportPrintGateOut(string GateOutNo)
        //{ 
        //    DataTable Stuffing = new DataTable();

        //    HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

        //    Stuffing = db.sub_GetDatatable("uspPrintExportGateOut '" + GateOutNo + "'");

        //    if (Stuffing.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in Stuffing.Rows)
        //        {
        //            ViewBag.ID = dr["ID"];
        //            ViewBag.GateOutNo = dr["GateOutNo"];
        //            ViewBag.ContainerNo = dr["ContainerNo"];
        //            ViewBag.OutDate = dr["OutDate"];
        //            ViewBag.PortName = dr["PortName"];
        //            ViewBag.VesselName = dr["VesselName"];
        //            ViewBag.AddedOn = dr["AddedOn"];
        //            ViewBag.AddedBy = dr["AddedBy"];
        //            ViewBag.GateInNo = dr["GateInNo"];
        //            ViewBag.VoyageNo = dr["YogageNo"];
        //            ViewBag.VehicleNo = dr["VehicleNo"];
        //            ViewBag.Transporter = dr["Transporter"];
        //            ViewBag.DriverName = dr["DriverName"];
        //            ViewBag.DriverMobile = dr["DriverMobile"];
        //            ViewBag.Remarks = dr["Remarks"];


        //        }

        //    } 
        //    return PartialView();

        //}



        public JsonResult GetExporterAddressByID(int ExporterID)
        {
            string Address = "";

            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataTable dt = new DataTable(); 
            dt = db.sub_GetDatatable("USP_GetExporterAddressByID '" + ExporterID + "'");
           
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Address = Convert.ToString(row["Address"]); 
            } 
            var jsonResult = Json(Address, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public JsonResult GetCartingEntrySummary(string fromDate, string toDate)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();



            dt = db.sub_GetDatatable("usp_GetCartingEntrySummary'" + fromDate + "','" + toDate + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetCartingTallyEntrySummary(string fromDate, string toDate)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();



            dt = db.sub_GetDatatable("usp_GetCartingTallyEntrySummary'" + fromDate + "','" + toDate + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetGateInSummary(string fromDate, string toDate)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();

            dt = db.sub_GetDatatable("usp_GetGateInSummary'" + fromDate + "','" + toDate + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GetStuffingReqSummary(string fromDate, string toDate)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();

            dt = db.sub_GetDatatable("usp_GetStuffingReqSummary'" + fromDate + "','" + toDate + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetEmptyOutSummary(string fromDate, string toDate)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();

            dt = db.sub_GetDatatable("usp_GetEmptyOutSummary'" + fromDate + "','" + toDate + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult ExportPrintStuffing(string StuffingNo)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            DataSet ds = db.sub_GetDataSets("uspPrintExportStuffing '" + StuffingNo + "'");

            if (ds != null && ds.Tables.Count >= 3)
            {
               
                DataTable dtCompany = ds.Tables[0];
                if (dtCompany.Rows.Count > 0)
                {
                    ViewBag.CompanyName = dtCompany.Rows[0]["CompanyName"];
                    ViewBag.CompanyAddress = dtCompany.Rows[0]["CompanyAddress"];
                }


                DataTable dtHeader = ds.Tables[1];
                if (dtHeader.Rows.Count > 0)
                {
                    var dr = dtHeader.Rows[0];

                    ViewBag.StuffingNo = dr["StuffingNo"];
                    ViewBag.SBNo = dr["SBNo"];
                    ViewBag.ContainerNo = dr["ContainerNo"];
                    ViewBag.StuffingDate = dr["StuffingDate"];
                    ViewBag.Acc_Holder = dr["Acc_Holder"];
                    ViewBag.Line = dr["Line"];
                    ViewBag.Exporter = dr["Exporter"];
                    ViewBag.CHA = dr["CHA"];
                    ViewBag.CargoType = dr["CargoType"];
          
                }


                
                ViewBag.Details = ds.Tables[2];


                
                ViewBag.UserName = Session["UserName"];
                ViewBag.PrintDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            }

            return PartialView("ExportPrintStuffing");
        }

        public ActionResult ExportPrintCartingEntry(string CartingNo)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            DataSet ds = db.sub_GetDataSets("uspPrintExportCarting '" + CartingNo + "'");

            if (ds != null && ds.Tables.Count > 0)
            {
                // Company Details
                DataTable dtCompany = ds.Tables[0];
                if (dtCompany.Rows.Count > 0)
                {
                    DataRow dr = dtCompany.Rows[0];
                    ViewBag.CompanyName = dr["CompanyName"];
                    ViewBag.CompanyAddress = dr["CompanyAddress"];
                }

                // PrintStuffing
                DataTable dtHeader = ds.Tables[1];
                if (dtHeader.Rows.Count > 0)
                {
                    DataRow dr = dtHeader.Rows[0];
                    ViewBag.CartingNo = dr["CartingNo"];
                    ViewBag.SBNo = dr["SBNo"];
                    ViewBag.SBDate = dr["SBDate"];
                    ViewBag.CHA = dr["CHA"];
                    ViewBag.Acc_Holder = dr["Acc_Holder"];
                    ViewBag.Exporter = dr["Exporter"];
                    ViewBag.CargoType = dr["CargoType"];
                    ViewBag.Consignee = dr["Consignee"];
                }

                // Details
                ViewBag.Details = ds.Tables[2];

                ViewBag.UserName = Session["UserName"];
                ViewBag.PrintDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            }

            /*return PartialView(ExportPrintCartingEntry);*/
            return PartialView("ExportPrintCartingEntry");

        }
        public ActionResult ExportPrintEmptyOut(string EmptyOutNo)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            DataSet ds = db.sub_GetDataSets("usp_PrintEmptyOut '" + EmptyOutNo + "'");

            if (ds != null && ds.Tables.Count > 0)
            {
                // Company Details - Table 0
                DataTable dtCompany = ds.Tables[0];
                if (dtCompany.Rows.Count > 0)
                {
                    DataRow dr = dtCompany.Rows[0];
                    ViewBag.CompanyName = dr["CompanyName"];
                    ViewBag.CompanyAddress = dr["CompanyAddress"];
                }

                // Header - Table 1
                DataTable dtHeader = ds.Tables[1];
                if (dtHeader.Rows.Count > 0)
                {
                    DataRow dr = dtHeader.Rows[0];
                    ViewBag.EmptyOutNo = dr["EmptyOutNo"];
                    ViewBag.ContainerNo = dr["ContainerNo"];
                    ViewBag.OutDate = dr["OutDate"];
                    ViewBag.Location = dr["Location"];
                    ViewBag.VehicleNo = dr["VehicleNo"];
                    ViewBag.Remarks = dr["Remarks"];
                }

                // Details - Table 2
                ViewBag.Details = ds.Tables[2];

                ViewBag.UserName = Session["UserName"];
                ViewBag.PrintDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            }

            return PartialView("ExportPrintEmptyOut");
        }
        [HttpPost]
        public JsonResult CancelCartingEntry(string cartingNo)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = db.sub_GetDatatable($"usp_CancelCartingAllow '{cartingNo}', '{userId}'");

            return Json(new { status = "success", message = "Cancelled Successfully" },
                JsonRequestBehavior.AllowGet);
        } 
        [HttpPost]
        public JsonResult CancelSBDdetails(int ID)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = db.sub_GetDatatable($"usp_CancelCartingEntry '{ID}', '{userId}'");

            return Json(new { status = "success", message = "Cancelled Successfully" },
                JsonRequestBehavior.AllowGet);
        }


        public JsonResult CancelCartingTallyEntry(int ID)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            db.sub_ExecuteNonQuery($"usp_CancelCartingTallyEntry '{ID}', '{userId}'");

            return Json(new { status = "success", message = "Cancelled Successfully" },
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelGateINEntry(int ID)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();

            DataTable dt = db.sub_GetDatatable($"usp_CancelGateINEntry '{ID}', '{userId}'");

            return Json(new { status = "success", message = "Gate In Entry Cancelled Successfully" },
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult CancelStuffing(int ID)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = db.sub_GetDatatable($"usp_CancelStuffing '{ID}', '{userId}'");

            return Json(new { status = "success", message = "Stuffing Cancelled Successfully" },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportOrderSSr()
        {

            BE.WorkOrderEntities WorkOrderList = new BE.WorkOrderEntities();
            WorkOrderList = GS.GetDropDownListExportWorkOrder();
            ViewBag.WOType = new SelectList(WorkOrderList.WOTypeList, "Wo_Type", "Wo_Type");
            ViewBag.EXType = new SelectList(WorkOrderList.EQWOList, "Id", "Name");
            ViewBag.AccountList = new SelectList(WorkOrderList.ImportAccountMasterList, "AccountID", "AccountName");
            ViewBag.VendorList = new SelectList(WorkOrderList.VendorWOList, "VendorId", "Name");
            ViewBag.CUSTOMERList = new SelectList(WorkOrderList.CUSTOMERList, "CUSTOMERNo", "CUSTOMERName");

            return View();
        }

        public JsonResult getExpWoSSr(string FromDate, string ToDate)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("GetExportSSRWOrkOrederSSSummary '" + FromDate + "','" + ToDate + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            dt.Columns.Remove("Action");
            Session["GetExportSSRWOrkOrederSSSummary"] = dt;
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /* public JsonResult getInvoiceWorkOrderDet(string ContainerNo, *//*string IGMNo, string ItemNo,*//* string SSRType,string SBNo)
         {
             string Message = "";
             HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

             DataSet ds = new DataSet();
             DataTable dt = new DataTable();
             DataTable dt1 = new DataTable();
             DataTable dt2 = new DataTable();
             BE.CHAMaster cHA = new BE.CHAMaster();
             List<BE.WorkOrderEntities> ContainerList = new List<BE.WorkOrderEntities>();
             dt = db.sub_GetDatatable(
                 "SELECT TOP 1 * FROM Export_SSR " +
                 "WHERE Type='" + SSRType + "' " +
                 *//*"AND ((IGMNo='" + IGMNo + "' AND ItemNo='" + ItemNo + "') " +*//*
                 "OR ContainerNo='" + ContainerNo + "' " +
                 "OR SBNo='" + SBNo + "') " +
                 "AND iscancel=0"
             );


             if (dt != null)
             {
                 if (dt.Rows.Count > 0)
                 {
                     Message = "SBNo No is already exit for selected equipments type. are you sure to generate again SSR. ?";
                 }
             }

             ds = db.sub_GetDataSets("GetExportSSRData '" + IGMNo + "','" + ItemNo + "','" + ContainerNo + "','" + SBNo + "'");

             if (ds.Tables[0].Rows.Count <= 0)
             {
                 cHA.errorMessage = "IGM/Item No is Not Valid. Please try again.";
             }
             else
             {
                 if (ds != null)
                 {
                     dt1 = ds.Tables[0];
                     dt2 = ds.Tables[1];

                     cHA.CHAID = Convert.ToInt32(dt1.Rows[0]["CHAID"]);
                     cHA.ContactNo1 = dt1.Rows[0]["ContactNo"].ToString();
                     cHA.CHAName = dt1.Rows[0]["ChaName"].ToString();
                     cHA.ContactPerson = dt1.Rows[0]["IGMNo"].ToString();
                     cHA.City = dt1.Rows[0]["ItemNo"].ToString();
                     cHA.ContactNo2 = dt1.Rows[0]["ContainerNo"].ToString();
                     cHA.errorMessage = "";
                 }

                 foreach (DataRow row in dt2.Rows)
                 {
                     BE.WorkOrderEntities workOrderEntities = new BE.WorkOrderEntities();
                     workOrderEntities.ContainerNo = row["ContainerNo"].ToString();
                     workOrderEntities.Type = row["ContainerType"].ToString();
                     workOrderEntities.Size = row["Size"].ToString();
                     workOrderEntities.ManifestQty = row["IGM_Qty"].ToString();
                     workOrderEntities.Weight = row["Weight"].ToString();
                     workOrderEntities.JoNo = Convert.ToInt64(row["JONo"]);
                     workOrderEntities.IGMNo = row["IGMNo"].ToString();
                     workOrderEntities.ItemNo = row["ItemNo"].ToString();

                     ContainerList.Add(workOrderEntities);
                 }
             }

             var returnField = new { workOrder = cHA, ContainerData = ContainerList, Message = Message };

             return new JsonResult() { Data = returnField, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

         }*/
        public JsonResult getInvoiceWorkOrderDet(string ContainerNo, string SSRType, string SBNo)
        {
            string Message = "";
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            DataSet ds = new DataSet();
            BE.CHAMaster cHA = new BE.CHAMaster();
            List<BE.WorkOrderEntities> ContainerList = new List<BE.WorkOrderEntities>();

            // 🔹 Duplicate SSR check
            DataTable dt = db.sub_GetDatatable(
                "SELECT TOP 1 * FROM Export_SSR " +
                "WHERE Type='" + SSRType + "' " +
                "AND (ContainerNo='" + ContainerNo + "' OR SBNo='" + SBNo + "') " +
                "AND iscancel=0"
            );

            if (dt.Rows.Count > 0)
            {
                Message = "SSR already exists for this SB / Container. Do you want to generate again?";
            }

            // 🔹 Main Data
            ds = db.sub_GetDataSets(
                "EXEC GetExportSSRDataAbhi " +
                "@ContainerNo = '" + (ContainerNo ?? "") + "', " +
                "@SBNo = '" + (SBNo ?? "") + "'"
            );

            if (ds == null || ds.Tables.Count < 2 || ds.Tables[1].Rows.Count == 0)
            {
                Message = "No data found.";
            }
            else
            {
                // 🔹 Header
                DataTable header = ds.Tables[0];
                cHA.CUSTOMERID = Convert.ToInt32(header.Rows[0]["CUSTOMERID"]);

                // 🔹 Grid data
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    ContainerList.Add(new BE.WorkOrderEntities
                    {
                        SBNo = row["SBNo"].ToString(),
                        SBDate = Convert.ToDateTime(row["SBDate"]).ToString("dd-MMM-yyyy"),
                        Description = row["Description"].ToString(),

                        ContainerNo = row["ContainerNo"].ToString(),
                        Size = row["Size"] == DBNull.Value ? "" : row["Size"].ToString(),
                        Type = row["Type"] == DBNull.Value ? "" : row["Type"].ToString(),

                        Amount = row["Amount"] == DBNull.Value ? 0 : Convert.ToDouble(row["Amount"]),
                        Narration = row["Narration"].ToString()
                    });
                }
            }

            return Json(new
            {
                workOrder = cHA,
                ContainerData = ContainerList,
                Message = Message
            }, JsonRequestBehavior.AllowGet);
        }



    }
}