using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using DP = MvcBondBusinessLayer.Report;
using train = MvcBondBusinessLayer.TrainMaster;
using RP = MvcBondBusinessLayer.Report;
using BO = MvcBondEntities.Report;
using MVCBOND.Filters;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using System.Web.UI;
using DB = MvcBondDataLayer;
using HC = MvcBondDataLayer.Helper;
using BM = MvcBondBusinessLayer.MvcBondBusinessLayer;
using BE = MvcBondEntities.BusinessEntities;
using System.Diagnostics;
using System.Linq;
using CD = MvcBondDataLayer.Helper;
using MvcBondEntities.BusinessEntities;
using System.Data.SqlClient;

namespace MediSoft.Controllers.Report
{
    [UserAuthenticationFilter]
    public class WorkOrderController : Controller
    {
        BM.BLDataManager.GSTSummary GS = new BM.BLDataManager.GSTSummary();
        public ActionResult GenerateWorkOrder()
        {
 
            BE.WorkOrderEntities WorkOrderList = new BE.WorkOrderEntities();
            WorkOrderList = GS.GetDropDownListImportWorkOrder();
            ViewBag.AccountHeadList = new SelectList(WorkOrderList.AccountHeadList, "AccountID", "AccountName");
            ViewBag.GetWotype = new SelectList(WorkOrderList.WOTypeList, "Wo_Type", "Wo_Type");
            ViewBag.EquipmentNoList1 = new SelectList(WorkOrderList.EquipmentNoList1, "Id", "Name");
            ViewBag.EQType = new SelectList(WorkOrderList.EQWOList, "Id", "Equipment");
            ViewBag.VendorName = new SelectList(WorkOrderList.VendorWOList, "VendorId", "Name");
            ViewBag.CustomerName = new SelectList(WorkOrderList.CHAList, "CHANo", "CHAName");
           
            return View();
        }

        public ActionResult GenrateDeliveryNoc()
        {

            BE.WorkOrderEntities WorkOrderList = new BE.WorkOrderEntities();
            WorkOrderList = GS.GetDropDownListImportWorkOrder();
            ViewBag.AccountHeadList = new SelectList(WorkOrderList.AccountHeadList, "AccountID", "AccountName");
            ViewBag.GetWotype = new SelectList(WorkOrderList.WOTypeList, "Wo_Type", "Wo_Type");
            ViewBag.EquipmentNoList1 = new SelectList(WorkOrderList.EquipmentNoList1, "Id", "Name");
            ViewBag.EQType = new SelectList(WorkOrderList.EQWOList, "Id", "Equipment");
            ViewBag.VendorName = new SelectList(WorkOrderList.VendorWOList, "VendorId", "Name");
            ViewBag.CustomerName = new SelectList(WorkOrderList.CHAList, "CHANo", "CHAName");

            return View();
        }

        public JsonResult GetContainerForWorkOrder(string Search, string IGMNo, string ItemNo, string ContainerNo)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("USP_GetContainerForWorkOrder '" + Search + "','" + IGMNo + "','" + ItemNo + "','" + ContainerNo + "'");

            // Convert DataTable to List of Dictionary
            var result = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                result.Add(dict);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
       

        public JsonResult SaveWorkOrder(GenerateSSRModel generatessr, List<BE.ContainerDetails> SelectedData)
        {
            try
            {
                // Validate input parameters
                if (generatessr == null || SelectedData == null || SelectedData.Count == 0)
                {
                    return Json("FAILED: Invalid input data", JsonRequestBehavior.AllowGet);
                }

                string strSQL = "";
                string result = "";
                DataTable InsertDL = new DataTable();
                DataTable dt = new DataTable();

                // Set default values for the model
                generatessr.ADDED_BY = Convert.ToInt32(Session["userid"]);
                //generatessr.CHA = "1";
                //generatessr.VENDOR_NAME = "2";
                //generatessr.SSR_MODE = "3";
                string entryDate = generatessr.SSRDate.ToString("yyyy-MM-dd");
                // Build and execute the main work order insert query
                strSQL = "USP_INSERT_WORK_ORDER_M '" +
                        generatessr.SSR_TYPE + "','" +
                        generatessr.CHA + "','" +
                        generatessr.CUSTOMER + "','" +
                        generatessr.VENDOR_NAME + "','" +
                        generatessr.SSR_MODE + "','" +
                        generatessr.ADDED_BY + "','" +
                            generatessr.IGMNo + "','" +
                                generatessr.ItemNo + "','" +
                                    generatessr.JoNo + "','" +
                        entryDate + "'";
                        //generatessr.SSRDate + "'";

                CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
                dt = db.sub_GetDatatable(strSQL);

                // Get the generated WO_NO
                if (dt != null && dt.Rows.Count > 0)
                {
                    result = Convert.ToString(dt.Rows[0]["WO_NO"]);
                }
                else
                {
                    return Json("FAILED: Could not create work order", JsonRequestBehavior.AllowGet);
                }

                // Prepare container details data table
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("ContainerNo", typeof(string));
                dataTable.Columns.Add("ContainerSize", typeof(string));
                dataTable.Columns.Add("Qty", typeof(string));
                dataTable.Columns.Add("Weight", typeof(string));
                dataTable.Columns.Add("VehicleNo", typeof(string));
                dataTable.Columns.Add("EquipmentType", typeof(string));
                dataTable.Columns.Add("EquipmentNo", typeof(string));
                dataTable.Columns.Add("EntryId", typeof(string));
                dataTable.Columns.Add("GPNo", typeof(string));
                dataTable.Columns.Add("Examine", typeof(string));

                foreach (BE.ContainerDetails item in SelectedData)
                {
                    DataRow row = dataTable.NewRow();
                    row["ContainerNo"] = item.ContainerNo ?? string.Empty;
                    row["ContainerSize"] = item.Size ?? string.Empty;
                    row["Qty"] = item.Qty ?? string.Empty;
                    row["Weight"] = item.Weight ?? string.Empty;
                    row["VehicleNo"] = item.VehicleNo ?? string.Empty;
                    row["EquipmentType"] = item.EquipmentType ?? string.Empty;
                    row["EquipmentNo"] = item.EquipmentNo ?? string.Empty;
                    row["EntryId"] = item.EntryId.ToString();
                    row["GPNo"] = item.GPNo ?? string.Empty;
                    row["Examine"] = item.Examine ?? "0";

                    dataTable.Rows.Add(row);
                }

                // Insert container details
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    strSQL = "USP_INSERT_WORK_ORDER_D '" +
                            result + "','" +
                            dataTable.Rows[i].Field<string>("ContainerNo") + "','" +
                            dataTable.Rows[i].Field<string>("ContainerSize") + "','" +
                            dataTable.Rows[i].Field<string>("Qty") + "','" +
                            dataTable.Rows[i].Field<string>("Weight") + "','" +
                            dataTable.Rows[i].Field<string>("VehicleNo") + "','" +
                            dataTable.Rows[i].Field<string>("EquipmentType") + "','" +
                            dataTable.Rows[i].Field<string>("EquipmentNo") + "','" +
                            dataTable.Rows[i].Field<string>("EntryId") + "','" +
                            dataTable.Rows[i].Field<string>("GPNo") + "','"+
                            dataTable.Rows[i].Field<string>("Examine") + "'";

                    InsertDL = db.sub_GetDatatable(strSQL);

                    if(generatessr.SSR_TYPE == "Loaded" || generatessr.SSR_TYPE == "Destuff")
                    {

                        db.sub_GetDatatable("uspUpdateSealCutDeliveryDetails '" + generatessr.JoNo + "','" + generatessr.IGMNo + "','" + dataTable.Rows[i].Field<string>("ContainerNo") + "','" + generatessr.SSR_TYPE + "'");

                    }

                }

                return Json("SUCCESS", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the error (you should implement proper error logging)
                // System.Diagnostics.Debug.WriteLine(ex.ToString());
                return Json("FAILED: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GenrateDelivery(GenerateSSRModel generatessr, List<BE.ContainerDetails> SelectedData)
        {
            try
            {
                // Validate input parameters
                if (generatessr == null || SelectedData == null || SelectedData.Count == 0)
                {
                    return Json("FAILED: Invalid input data", JsonRequestBehavior.AllowGet);
                }

                string strSQL = "";
                string result = "";
                DataTable InsertDL = new DataTable();
                DataTable dt = new DataTable();

                // Set default values for the model
                generatessr.ADDED_BY = Convert.ToInt32(Session["userid"]);
                //generatessr.CHA = "1";
                //generatessr.VENDOR_NAME = "2";
                //generatessr.SSR_MODE = "3";

                // Build and execute the main work order insert query
                strSQL = "USP_INSERT_Genrate_Delivery_M '" +
                        generatessr.SSR_TYPE + "','" +
                        generatessr.CHA + "','" +
                        generatessr.CUSTOMER + "','" +
                        generatessr.VENDOR_NAME + "','" +
                        generatessr.SSR_MODE + "','" +
                        generatessr.ADDED_BY + "','" +
                            generatessr.IGMNo + "','" +
                                generatessr.ItemNo + "','" +
                                    generatessr.JoNo + "','" +
                        generatessr.SSRDate + "'";

                CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
                dt = db.sub_GetDatatable(strSQL);

                // Get the generated WO_NO
                if (dt != null && dt.Rows.Count > 0)
                {
                    result = Convert.ToString(dt.Rows[0]["WO_NO"]);
                }
                else
                {
                    return Json("FAILED: Could not create work order", JsonRequestBehavior.AllowGet);
                }

                // Prepare container details data table
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("ContainerNo", typeof(string));
                dataTable.Columns.Add("ContainerSize", typeof(string));
                dataTable.Columns.Add("Qty", typeof(string));
                dataTable.Columns.Add("Weight", typeof(string));
                dataTable.Columns.Add("VehicleNo", typeof(string));
                dataTable.Columns.Add("EquipmentType", typeof(string));
                dataTable.Columns.Add("EquipmentNo", typeof(string));
                dataTable.Columns.Add("EntryId", typeof(string));
                dataTable.Columns.Add("GPNo", typeof(string));
                dataTable.Columns.Add("Examine", typeof(string));

                foreach (BE.ContainerDetails item in SelectedData)
                {
                    DataRow row = dataTable.NewRow();
                    row["ContainerNo"] = item.ContainerNo ?? string.Empty;
                    row["ContainerSize"] = item.Size ?? string.Empty;
                    row["Qty"] = item.Qty ?? string.Empty;
                    row["Weight"] = item.Weight ?? string.Empty;
                    row["VehicleNo"] = item.VehicleNo ?? string.Empty;
                    row["EquipmentType"] = item.EquipmentType ?? string.Empty;
                    row["EquipmentNo"] = item.EquipmentNo ?? string.Empty;
                    row["EntryId"] = item.EntryId.ToString();
                    row["GPNo"] = item.GPNo ?? string.Empty;
                    row["Examine"] = item.Examine ?? "0";

                    dataTable.Rows.Add(row);
                }

                // Insert container details
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    strSQL = "USP_INSERT_Delivery_D '" +
                            result + "','" +
                            dataTable.Rows[i].Field<string>("ContainerNo") + "','" +
                            dataTable.Rows[i].Field<string>("ContainerSize") + "','" +
                            dataTable.Rows[i].Field<string>("Qty") + "','" +
                            dataTable.Rows[i].Field<string>("Weight") + "','" +
                            dataTable.Rows[i].Field<string>("VehicleNo") + "','" +
                            dataTable.Rows[i].Field<string>("EquipmentType") + "','" +
                            dataTable.Rows[i].Field<string>("EquipmentNo") + "','" +
                            dataTable.Rows[i].Field<string>("EntryId") + "','" +
                            dataTable.Rows[i].Field<string>("GPNo") + "','"+
                            dataTable.Rows[i].Field<string>("Examine") + "'";

                    InsertDL = db.sub_GetDatatable(strSQL);

                    if(generatessr.SSR_TYPE == "Loaded" || generatessr.SSR_TYPE == "Destuff")
                    {

                        db.sub_GetDatatable("uspUpdateSealCutDeliveryDetails '" + generatessr.JoNo + "','" + generatessr.IGMNo + "','" + dataTable.Rows[i].Field<string>("ContainerNo") + "','" + generatessr.SSR_TYPE + "'");

                    }

                }

                return Json("SUCCESS", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the error (you should implement proper error logging)
                // System.Diagnostics.Debug.WriteLine(ex.ToString());
                return Json("FAILED: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FetchWorkOrderSummary(string FromDate, string ToDate)
        {
            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("uspGetWorkOrderSummary '" + FromDate + "','" + ToDate + "'");



            string json = JsonConvert.SerializeObject(dt);
            // Remove the first two columns (Index 0 and 1)
            if (dt.Columns.Count > 1) // Ensure enough columns exist
            {
                dt.Columns.RemoveAt(0); // Remove first column (index 0)
                dt.Columns.RemoveAt(0); // Remove the second column (now at index 0 after first removal)
            }
            Session["uspGetWorkOrderSummary"] = dt;

            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }


        public ActionResult FetchuspGetDeliverySummary(string FromDate, string ToDate)
        {
            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("uspGetDeliverySummary '" + FromDate + "','" + ToDate + "'");



            string json = JsonConvert.SerializeObject(dt);
            // Remove the first two columns (Index 0 and 1)
            if (dt.Columns.Count > 1) // Ensure enough columns exist
            {
                dt.Columns.RemoveAt(0); // Remove first column (index 0)
                dt.Columns.RemoveAt(0); // Remove the second column (now at index 0 after first removal)
            }
            Session["uspGetDeliverySummary"] = dt;

            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }






        public JsonResult CheckForContainerHold(string Search, string IGMNo, string ItemNo, string ContainerNo)
        {
            int result = 0;
            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_CheckHoldStatus '" + Search + "','" + IGMNo + "','" + ItemNo + "','" + ContainerNo + "'");

            result = Convert.ToInt32(dt.Rows[0][0].ToString());

            if (result > 0)
            {
                string message = "HOLD";
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            else
            {

                string message = "SUCCESS";
                return Json(message, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ExportWorkOrderSummary()
        {
            DataTable CompanyMaster = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            DataTable GetGSTReturnCreditNoteSummary = (DataTable)Session["uspGetWorkOrderSummary"];
            string Tittle = " " + Session["fromdate"] + "  " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = GetGSTReturnCreditNoteSummary;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=WorkOrderSummary.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))

                {

                    // render the GridView to the HtmlTextWriter
                    htw.Write("<table><tr><td colspan ='7'><h1 style='text-align:center'>" + CompanyName + " </h1></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h3 style='text-align:center'>" + CompanyAddress + " </h3></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h3 style='text-align:center'>Work Order Summary</h3></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h3 style='text-align:center'>" + Tittle + " </h3></td></tr>");

                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            return View();
        }
        public ActionResult WorkOrderPrint(string wono, string workyear)
        {
            DataSet MasterTable = new DataSet();
            DataTable tblComDetails = new DataTable();
            DataTable DT = new DataTable();
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();

            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            MasterTable = db.sub_GetDataSets("USP_WORKORDER_PRINT '" + wono + "','" + workyear + "'");
            if (MasterTable.Tables.Count > 0)
            {
                tblComDetails = MasterTable.Tables[0];
                DT = MasterTable.Tables[1];
                DT1 = MasterTable.Tables[2];

                string SignedQRcode = "";
                foreach (DataRow dr in tblComDetails.Rows)
                {
                    ViewBag.CompanyName = dr["con_Name"];
                    ViewBag.CompanyAddress = dr["AddressI"];
                    ViewBag.Con_For = dr["Con_For"];

                }

                foreach (DataRow dr in DT.Rows)
                {

                    ViewBag.WorkOrderNo = dr["WO_NO"];
                    ViewBag.WorkOrderDate = dr["WO_Date"];
                    //ViewBag.VEHICLE_NO = dr["VEHICLE_NO"];
                    ViewBag.EntryType = dr["ACTIVITY_TYPE"];
                    ViewBag.CHAName = dr["CHA"];
                    ViewBag.IGMNO = dr["IGMNo"];
                    ViewBag.ITEMNO = dr["ItemNo"];
                    //ViewBag.JONO = dr["JONo"];
                    ViewBag.REMARKS = dr["REMARKS"];
                    //ViewBag.PACKAGE_TYPE = dr["PACKAGE_TYPE"];
                    /*ViewBag.CARGO_DESC = dr["CARGO_DESC"];*/
                    ViewBag.ImporterName = dr["Importer"];
                    ViewBag.CHA = dr["CHA"];
                    ViewBag.UserName = dr["UserName"];
                    ViewBag.SLIP_TYPE = dr["ACTIVITY_TYPE"];
                    ViewBag.PrintDate = Convert.ToDateTime(DateTime.Now).ToString("dd MM yyyy HH:mm");
                    ViewBag.ShippingLine = dr["ShippingLine"];
                    ViewBag.BOEDate = dr["BOEDate"];
                    ViewBag.BOENo = dr["BOENo"];

                }

            }

            ViewBag.ContainerDetails = DT1.AsEnumerable();

            return PartialView();
        }


        public ActionResult GenerateDeliveryPrint(string wono, string workyear)
        {
            DataSet MasterTable = new DataSet();
            DataTable tblComDetails = new DataTable();
            DataTable DT = new DataTable();
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();

            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            MasterTable = db.sub_GetDataSets("USP_DELIVERYORDER_PRINT '" + wono + "','" + workyear + "'");
            if (MasterTable.Tables.Count > 0)
            {
                tblComDetails = MasterTable.Tables[0];
                DT = MasterTable.Tables[1];
                DT1 = MasterTable.Tables[2];

                string SignedQRcode = "";
                foreach (DataRow dr in tblComDetails.Rows)
                {
                    ViewBag.CompanyName = dr["con_Name"];
                    ViewBag.CompanyAddress = dr["AddressI"];
                    ViewBag.Con_For = dr["Con_For"];

                }

                foreach (DataRow dr in DT.Rows)
                {

                    ViewBag.WorkOrderNo = dr["WO_NO"];
                    ViewBag.WorkOrderDate = dr["WO_Date"];
                    //ViewBag.VEHICLE_NO = dr["VEHICLE_NO"];
                    ViewBag.EntryType = dr["ACTIVITY_TYPE"];
                    ViewBag.CHAName = dr["PARTY_NAME"];
                    ViewBag.IGMNO = dr["IGMNo"];
                    ViewBag.ITEMNO = dr["ItemNo"];
                    //ViewBag.JONO = dr["JONo"];
                    ViewBag.REMARKS = dr["REMARKS"];
                    //ViewBag.PACKAGE_TYPE = dr["PACKAGE_TYPE"];
                    ViewBag.CARGO_DESC = dr["CARGO_DESC"];
                    ViewBag.ImporterName = dr["Importer"];
                    ViewBag.CHA = dr["CHA"];
                    ViewBag.UserName = dr["UserName"];
                    ViewBag.SLIP_TYPE = dr["ACTIVITY_TYPE"];
                    ViewBag.PrintDate = Convert.ToDateTime(DateTime.Now).ToString("dd MM yyyy HH:mm");
                    ViewBag.ShippingLine = dr["ShippingLine"];
                    ViewBag.BOEDate = dr["BOEDate"];
                    ViewBag.BOENo = dr["BOENo"];

                }


            }

            ViewBag.ContainerDetails = DT1.AsEnumerable();

            return PartialView();
        }
        public ActionResult WorkOrderCancel(string wono, string workyear)
        {
            DataTable dt = new DataTable();
            int userId = Convert.ToInt32(Session["userid"]);
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("usp_WorkOrderCancel '" + wono + "','" + workyear + "','" + userId + "' ");
            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #region UnloadingTally

        public ActionResult UnloadingTally()
        {
            BE.WorkOrderEntities WorkOrderList = new BE.WorkOrderEntities();
            WorkOrderList = GS.GetDropDownListImportWorkOrder();
            ViewBag.AccountHeadList = new SelectList(WorkOrderList.AccountHeadList, "AccountID", "AccountName");
            ViewBag.GetWotype = new SelectList(WorkOrderList.WOTypeList, "Wo_Type", "Wo_Type");
            ViewBag.EquipmentNoList1 = new SelectList(WorkOrderList.EquipmentNoList1, "Id", "Name");
            ViewBag.EQType = new SelectList(WorkOrderList.EQWOList, "Id", "Equipment");
            ViewBag.VendorName = new SelectList(WorkOrderList.VendorWOList, "VendorId", "Name");
            ViewBag.CustomerName = new SelectList(WorkOrderList.CHAList, "CHANo", "CHAName");

            return View();
        }


        public JsonResult GateUnloadingPendingList(string SearchType)
        {

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("uspUnloadingPendingList");

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetunloadingSumMARY(string SearchType, string fromdate, string Todate)
        {

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_Unloading_summary '" + fromdate + "','" + Todate + "'");

            Session["USP_Unloading_summary"] = dt;

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportToexcelUnloadingtally()
        {
            DataTable dt = (DataTable)Session["USP_Unloading_summary"];
            DataTable CompanyMaster = new DataTable();

            dt.Columns.Remove("Cancel");

            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();
            var CompanyName = "";
            var CompanyAddress = "";
            string Tittle = "From " + Session["fromdate"] + " To " + Session["Todate"] + ".";
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=UnloadingTallySummary.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'><strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    // htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *output generated by JMM </h6></td></tr>");
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


        public ActionResult UnloadingTallyCancel(string shipment, string ToDate)
        {
            DataTable dt = new DataTable();
            int userid = Convert.ToInt32(Session["userid"]);
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("   '" + shipment + "','" + userid + "'");
            //Session["LocationMaster"] = dt;

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
   
            return jsonResult;
             
        }
        public JsonResult SaveUnLoading(List<BE.ContainerDetails> SelectedData, string EntryDate, string IGMNo,string ItemNo, string JoNo)
        {
            string strSQL = "";
            string result = "";
            DataTable InsertDL = new DataTable();
            DataTable dt = new DataTable();
            //LoginUserDetail loginUserDetail = (LoginUserDetail)Session["LoginUserDetail"];
            var date = DateTime.Now;
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            int userId = Convert.ToInt32(Session["userid"]);
            string message = "";

            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("ContainerNo");
                dataTable.Columns.Add("VehicleNo");

                dataTable.Columns.Add("DestuffPKGS");
                dataTable.Columns.Add("DestuffWeight");
                dataTable.Columns.Add("ShortPKGS");
                dataTable.Columns.Add("ExcessPKGS");
                dataTable.Columns.Add("Location");
                dataTable.Columns.Add("Remarks");
                dataTable.Columns.Add("EntryId");
                dataTable.Columns.Add("GPNo");

                foreach (BE.ContainerDetails item in SelectedData)
                {
                    DataRow row = dataTable.NewRow();
                    row["ContainerNo"] = item.ContainerNo;
                    row["VehicleNo"] = item.VehicleNo;

                    row["DestuffPKGS"] = item.DestuffPKGS;
                    row["DestuffWeight"] = item.DestuffWeight;
                    row["ShortPKGS"] = item.ShortPKGS;
                    row["ExcessPKGS"] = item.ExcessPKGS;
                    row["Location"] = item.Location;
                    row["Remarks"] = item.Remarks;
                    row["EntryId"] = item.EntryId;
                    row["GPNo"] = item.GPNo;

                    dataTable.Rows.Add(row);


                }
                for (int i = 0; i <= dataTable.Rows.Count - 1; i++)
                {
                    strSQL = "";
                    strSQL = "USP_INSERT_UNLOADING_TALLY_SHEET_D '" + dataTable.Rows[i].Field<string>("ContainerNo") + "','" + dataTable.Rows[i].Field<string>("VehicleNo") + "','"
                        + dataTable.Rows[i].Field<string>("DestuffPKGS") + "','"
                        + dataTable.Rows[i].Field<string>("DestuffWeight") + "','" + dataTable.Rows[i].Field<string>("ShortPKGS")
                        + "','" + dataTable.Rows[i].Field<string>("ExcessPKGS")
                        + "','" + dataTable.Rows[i].Field<string>("Location") + "','" + dataTable.Rows[i].Field<string>("Remarks")
                        + "','" + dataTable.Rows[i].Field<string>("EntryId") + "','" + userId
                        + "','" + IGMNo + "','" + ItemNo + "','" + JoNo + "','" + EntryDate + "','" + dataTable.Rows[i].Field<string>("GPNo") + "'";


                    InsertDL = db.sub_GetDatatable(strSQL);


                }


                message = "SUCCESS";
            }
            catch (Exception)
            {
                message = "ERROR";
                throw;
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GateWorkOrderContainerData(string WoNo)
        {

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_WorkOrderDataShow '" + WoNo + "'");

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region Custom Examination
        public ActionResult CustomExaminationTally()
        {
            BE.WorkOrderEntities WorkOrderList = new BE.WorkOrderEntities();
            WorkOrderList = GS.GetDropDownListImportWorkOrder();
            ViewBag.AccountHeadList = new SelectList(WorkOrderList.AccountHeadList, "AccountID", "AccountName");
            ViewBag.GetWotype = new SelectList(WorkOrderList.WOTypeList, "Wo_Type", "Wo_Type");
            ViewBag.EquipmentNoList1 = new SelectList(WorkOrderList.EquipmentNoList1, "Id", "Name");
            ViewBag.EQType = new SelectList(WorkOrderList.EQWOList, "Id", "Equipment");
            ViewBag.VendorName = new SelectList(WorkOrderList.VendorWOList, "VendorId", "Name");
            ViewBag.CustomerName = new SelectList(WorkOrderList.CHAList, "CHANo", "CHAName");

            return View();
        }

        public JsonResult GateCustomEXPendingList(string SearchType)
        {

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("uspCustomExPendingList");

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetCustomExSummary(string SearchType, string fromdate, string Todate)
        {
            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_CustomeEx_summary '" + fromdate + "','" + Todate + "'");

            Session["USP_CustomeEx_summary"] = dt;

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public JsonResult GateCustomExWOContainerData(string WoNo)
        {

            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_CEWorkOrderDataShow '" + WoNo + "'");

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        

         public JsonResult SaveCustomExaminationtally(List<BE.ContainerDetails> SelectedData, string EntryDate, string IGMNo, string ItemNo, string JoNo)
        {
            string strSQL = "";
            string result = "";
            DataTable InsertDL = new DataTable();
            DataTable dt = new DataTable();
            //LoginUserDetail loginUserDetail = (LoginUserDetail)Session["LoginUserDetail"];
            var date = DateTime.Now;
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            int userId = Convert.ToInt32(Session["userid"]);
            string message = "";

            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("ContainerNo");
                dataTable.Columns.Add("VehicleNo");

                dataTable.Columns.Add("CEPKGS");
                dataTable.Columns.Add("CEWeight");
                dataTable.Columns.Add("ShortPKGS");
                dataTable.Columns.Add("ExcessPKGS");
                dataTable.Columns.Add("Location");
                dataTable.Columns.Add("Remarks");
                dataTable.Columns.Add("EntryId");
                dataTable.Columns.Add("GPNo");

                foreach (BE.ContainerDetails item in SelectedData)
                {
                    DataRow row = dataTable.NewRow();
                    row["ContainerNo"] = item.ContainerNo;
                    row["VehicleNo"] = item.VehicleNo;

                    row["CEPKGS"] = item.DestuffPKGS;
                    row["CEWeight"] = item.DestuffWeight;
                    row["ShortPKGS"] = item.ShortPKGS;
                    row["ExcessPKGS"] = item.ExcessPKGS;
                    row["Location"] = item.Location;
                    row["Remarks"] = item.Remarks;
                    row["EntryId"] = item.EntryId;
                    row["GPNo"] = item.GPNo;

                    dataTable.Rows.Add(row);

                }
                for (int i = 0; i <= dataTable.Rows.Count - 1; i++)
                {
                    strSQL = "";
                    strSQL = "USP_INSERT_CustomEx_TALLY_SHEET_D '" + dataTable.Rows[i].Field<string>("ContainerNo") + "','" + dataTable.Rows[i].Field<string>("VehicleNo") + "','"
                        + dataTable.Rows[i].Field<string>("CEPKGS") + "','"
                        + dataTable.Rows[i].Field<string>("CEWeight") + "','" + dataTable.Rows[i].Field<string>("ShortPKGS")
                        + "','" + dataTable.Rows[i].Field<string>("ExcessPKGS")
                        + "','" + dataTable.Rows[i].Field<string>("Location") + "','" + dataTable.Rows[i].Field<string>("Remarks")
                        + "','" + dataTable.Rows[i].Field<string>("EntryId") + "','" + userId
                        + "','" + IGMNo + "','" + ItemNo + "','" + JoNo + "','" + EntryDate + "','" + dataTable.Rows[i].Field<string>("GPNo") + "'";


                    InsertDL = db.sub_GetDatatable(strSQL);


                }


                message = "SUCCESS";
            }
            catch (Exception)
            {
                message = "ERROR";
                throw;
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}