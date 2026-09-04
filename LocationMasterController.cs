using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DP = MvcBondBusinessLayer.MvcBondBusinessLayer.LocationMaster;
using CI = MvcBondEntities.BusinessEntities;
using CD = MvcBondDataLayer.Helper;
using MVCBOND.Filters;
using System.Data;
using System.Web.UI.WebControls;
using System.IO;
using HC = MvcBondDataLayer.Helper;
using Newtonsoft.Json;
using System.Web.UI;

namespace TrackerMVC.Controllers.LocationMaster
{
    [UserAuthenticationFilter]
    public class LocationMasterController : Controller
    {
        DP.LocationMaster reportprovider = new DP.LocationMaster();
        // GET: LocationMaster
        public ActionResult LocationMaster()
        {
          
           
          
            return View();

           
        }
        public ActionResult Save(CI.LocationM LocationMaster)
        {
            string message = "";
            ///var EntryDate = LocationMaster.EntryDate;
            DataTable dt = new DataTable();
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("Insert_Loctaion_M '" + LocationMaster.Location + "','"    + Convert.ToInt32(Session["userid"])  + "'");
            if (dt.Rows.Count > 0)
            {
                message = Convert.ToString(dt.Rows[0][0]);
            }

            return Json(message);
        }

       


        [HttpPost]
        public ActionResult AjaxGetLocationDetails()
        {
            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();

            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_LocationMaster");
            Session["LocationMaster"] = dt;
           
            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            //var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            //var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public ActionResult ExportToExcel()
        {
            DataTable dt = (DataTable)Session["LocationMaster"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
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
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Location Summary Report <strong></td></tr>");
                 //   htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'>  </h6></td></tr>");
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
    
        //  -- State_City_M
        public ActionResult CityMaster()
        {
            ViewBag.Date = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");
            List<CI.CityMaster> List = new List<CI.CityMaster>();
            List = reportprovider.GetState1();
            ViewBag.GroupDDL = new SelectList(List, "StateID", "StateName");
            return View();

        }

        public JsonResult SaveCityDetails(CI.CityMaster CityMaster)
        {

            //HC.DBOperation object = new HC.DBOperations(); From Helper
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            //Code For Insert Data Sequence Should Be Same As Created SP.
            if (CityMaster != null)
                db.sub_ExecuteNonQuery("sp_InserCityMaster '" + CityMaster.StateID + "','" + CityMaster.CityName + "','" + CityMaster.PinCode + "','" + Convert.ToInt32(Session["userid"]) + "','" + CityMaster.IsActive + "'");
            // Master();

            return Json(CityMaster);
        }
        public JsonResult CityList(string search)
        {
            CD.DBOperationsForMvcBond db = new CD.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("sp_CityList'" + search + "'");
            var summaryDet = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(summaryDet, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

      

    }

}