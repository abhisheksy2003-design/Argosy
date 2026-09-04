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
using BE = MvcBondEntities.BusinessEntities;
using System.Diagnostics;
using System.Linq;

namespace MediSoft.Controllers.Report
{
    [UserAuthenticationFilter]
    public class ReportController : Controller
    {
        // GET: Report
        train.UpdateDischargeDate trainTrackerProvider = new train.UpdateDischargeDate();
        DP.Report Masteprovider = new DP.Report();
        //NOC Register 
        public ActionResult NOCRegister()
        {
            List<BO.ReportEnities> LineList = new List<BO.ReportEnities>();
            LineList = Masteprovider.getConsignee();
            ViewBag.CustName = new SelectList(LineList, "CustID", "CustName");
            return View();
        }
        RP.Report reportprovider = new RP.Report();
        public ActionResult GetSummary(string fromdate, string todate, string ddlGSTName)
        {
            DataTable GetNocSummary = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            GetNocSummary = db.sub_GetDatatable("USP_gate_BOND_IN_REGISTER '" + Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd HH:mm") + "','" + Convert.ToDateTime(todate).ToString("yyyy-MM-dd HH:mm")  + "','" + ddlGSTName +  "','" + Userid + "'");
            Session["GetNocSummary"] = GetNocSummary;
            Session["fromdate"] = fromdate;
            Session["todate"] = todate;
            var json = JsonConvert.SerializeObject(GetNocSummary);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public ActionResult CustomeExportToExcel()
        {
            DataTable dt = (DataTable)Session["GetNocSummary"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=BondNOCRegister.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Bond NOC Register<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    //htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'>  </h6></td></tr>");
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

// Noc Pendency register
        public ActionResult NOCPendencyRegister()
        {
            List<BO.ReportEnities> LineList = new List<BO.ReportEnities>();
            LineList = Masteprovider.getConsignee();
            ViewBag.CustName = new SelectList(LineList, "CustID", "CustName");
            return View();
        }

        public ActionResult GetNOCPendencyRegister(string fromdate, string todate, string ddlGSTName)
        {
            DataTable GetNOCPendencyRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            GetNOCPendencyRegister = db.sub_GetDatatable("USP_List_of_NOC_for_BOND_INs '" + Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd HH:mm") + "','" + Convert.ToDateTime(todate).ToString("yyyy-MM-dd HH:mm") + "','" + ddlGSTName + "','" + Userid + "'");
            Session["GetNocSummary"] = GetNOCPendencyRegister;
            Session["fromdate"] = fromdate;
            Session["todate"] = todate;
            var json = JsonConvert.SerializeObject(GetNOCPendencyRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public ActionResult PendencyRegisterExportToExcel()
        {
            DataTable dt = (DataTable)Session["GetNOCPendencyRegister"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=NocPendencyregister.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Noc Pendency register></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
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
        //bond In Register
        public ActionResult BONDINRegister()
        {
            List<BO.ReportEnities> LineList = new List<BO.ReportEnities>();
            LineList = Masteprovider.getConsignee();
            ViewBag.CustName = new SelectList(LineList, "CustID", "CustName");
            return View();
        }

        public ActionResult GetBONDINRegister(string fromdate, string todate, string ddlGSTName)
        {
            DataTable GetBONDINRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            GetBONDINRegister = db.sub_GetDatatable("USP_gate_IN_REGISTERS '" + Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd HH:mm") + "','" + Convert.ToDateTime(todate).ToString("yyyy-MM-dd HH:mm") + "','" + ddlGSTName + "','" + Userid + "'");
            Session["GetNocSummary"] = GetBONDINRegister;
            Session["fromdate"] = fromdate;
            Session["todate"] = todate;
            var json = JsonConvert.SerializeObject(GetBONDINRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public ActionResult GetBONDINRegisterExportToExcel()
        {
            DataTable dt = (DataTable)Session["GetBONDINRegister"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=BONDINRegister.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>BOND IN Register<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
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
        //BondExRegister
        public ActionResult BondExRegister()
        {
            List<BO.ReportEnities> LineList = new List<BO.ReportEnities>();
            LineList = Masteprovider.getConsignee();
            ViewBag.CustName = new SelectList(LineList, "CustID", "CustName");
            return View();
        }

        public ActionResult GetBondExRegister(string fromdate, string todate, string ddlGSTName)
        {
            DataTable BondExRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            BondExRegister = db.sub_GetDatatable("USP_gate_Ex_REGISTERS '" + Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd HH:mm") + "','" + Convert.ToDateTime(todate).ToString("yyyy-MM-dd HH:mm") + "','" + ddlGSTName + "','" + Userid + "'");
            Session["GetNocSummary"] = BondExRegister;
            Session["fromdate"] = fromdate;
            Session["todate"] = todate;
            var json = JsonConvert.SerializeObject(BondExRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public ActionResult BondExRegisterExportToExcel()
        {
            DataTable dt = (DataTable)Session["BondExRegister"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=BondExRegister.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>BOND Ex Register<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
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
        //GatePassRegister
        public ActionResult GatePassRegister()
        {
            List<BO.ReportEnities> LineList = new List<BO.ReportEnities>();
            LineList = Masteprovider.getConsignee();
            ViewBag.CustName = new SelectList(LineList, "CustID", "CustName");
            return View();
        }


        public ActionResult GetGatePassRegister(string fromdate, string todate, string ddlGSTName)
        {
            DataTable GatePassRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            GatePassRegister = db.sub_GetDatatable("USP_gate_registerS '" + Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd HH:mm") + "','" + Convert.ToDateTime(todate).ToString("yyyy-MM-dd HH:mm") + "','" + ddlGSTName + "','" + Userid + "'");
            Session["GatePassRegister"] = GatePassRegister;
            Session["fromdate"] = fromdate;
            Session["todate"] = todate;
            var json = JsonConvert.SerializeObject(GatePassRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult GatePassRegisterExportToExcel()
        {
            DataTable dt = (DataTable)Session["GatePassRegister"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=GatePassRegister.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Gate Pass Register<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
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

        public ActionResult SalesRegister()
        {
            List<BO.ReportEnities> LineList = new List<BO.ReportEnities>();
            LineList = Masteprovider.getConsignee();
            ViewBag.CustName = new SelectList(LineList, "CustID", "CustName");
            return View();
        }
        public ActionResult GetSalesRegister(string fromdate, string todate, string ddlGSTName)
        {
            DataTable SalesRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            SalesRegister = db.sub_GetDatatable("USP_gate_registerS '" + Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd HH:mm") + "','" + Convert.ToDateTime(todate).ToString("yyyy-MM-dd HH:mm") + "','" + ddlGSTName + "','" + Userid + "'");
            Session["SalesRegister"] = SalesRegister;
            Session["fromdate"] = fromdate;
            Session["todate"] = todate;
            var json = JsonConvert.SerializeObject(SalesRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public ActionResult SalesRegisterExportToExcel()
        {
            DataTable dt = (DataTable)Session["SalesRegister"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=SalesRegister.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Sales Register<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
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

        public ActionResult CargoInventoryRegister()
        {
            List<BO.ReportEnities> LineList = new List<BO.ReportEnities>();
            LineList = Masteprovider.getConsignee();
            ViewBag.CustName = new SelectList(LineList, "CustID", "CustName");
            return View();
        }

        public ActionResult GetCargoInventoryRegister(string fromdate,  string ddlGSTName)
        {
            DataTable CargoInventoryRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            CargoInventoryRegister = db.sub_GetDatatable("Get_sp_TotalBondStock_news '" + ddlGSTName + "','" + Userid + "'");
            Session["CargoInventoryRegister"] = CargoInventoryRegister;
            Session["fromdate"] = fromdate;
  
            var json = JsonConvert.SerializeObject(CargoInventoryRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public ActionResult CargoInventoryRegisterExportToExcel()
        {
            DataTable dt = (DataTable)Session["CargoInventoryRegister"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=CargoInventoryDetails.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Cargo Inventory Details<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
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

        public ActionResult OutStandingRegister()
        {
            List<BO.ReportEnities> LineList = new List<BO.ReportEnities>();
            LineList = Masteprovider.getConsignee();
            ViewBag.CustName = new SelectList(LineList, "CustID", "CustName");
            return View();
        }
        public ActionResult GetOutStandingRegister(string fromdate, string todate, string ddlGSTName)
        {
            DataTable OutStandingRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            OutStandingRegister = db.sub_GetDatatable("sp_listofinvoice_details '" + Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd HH:mm") + "','" + Convert.ToDateTime(todate).ToString("yyyy-MM-dd HH:mm") + "','" + ddlGSTName + "','" + Userid + "'");
            Session["OutStandingRegister"] = OutStandingRegister;
            Session["fromdate"] = fromdate;
            Session["todate"] = todate;
            var json = JsonConvert.SerializeObject(OutStandingRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public ActionResult OutStandingRegisterExportToExcel()
        {
            DataTable dt = (DataTable)Session["OutStandingRegister"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=OutStandingRegister.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Out Standing Register<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
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

        public ActionResult Generatedinvoice()
        {
            List<BO.ReportEnities> LineList = new List<BO.ReportEnities>();
            LineList = Masteprovider.getConsignee();
            ViewBag.CustName = new SelectList(LineList, "CustID", "CustName");
            return View();
        }

        public ActionResult GetGeneratedinvoice(string fromdate, string ddlGSTName)
        {
            DataTable Generatedinvoice = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            Generatedinvoice = db.sub_GetDatatable("USP_Generated_Invoice_search_Web '" + fromdate + "','" + ddlGSTName + "','" + Userid + "'");
            Session["Generatedinvoice"] = Generatedinvoice;
            Session["fromdate"] = fromdate;

            var json = JsonConvert.SerializeObject(Generatedinvoice);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult GeneratedinvoiceExportToExcel()
        {
            DataTable dt = (DataTable)Session["Generatedinvoice"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=Generatedinvoice.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Generated invoice<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
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

        public ActionResult ProfitabilityRegister()
        {
            List<BO.ReportEnities> LineList = new List<BO.ReportEnities>();
            LineList = Masteprovider.getConsignee();
            ViewBag.CustName = new SelectList(LineList, "CustID", "CustName");
            return View();
        }

        public ActionResult GetProfitabilityRegister(string fromdate, string todate, string DepartmentID, string Search)
        {
            DataTable ProfitabilityRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            ProfitabilityRegister = db.sub_GetDatatable("USP_Profitability_report_new '" + fromdate + "','" + todate + "','" + DepartmentID + "','" + Search + "'");
            Session["ProfitabilityRegister"] = ProfitabilityRegister;
            Session["fromdate"] = fromdate;

            var json = JsonConvert.SerializeObject(ProfitabilityRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public ActionResult ProfitabilityRegisterExportToExcel()
        {
            DataTable dt = (DataTable)Session["ProfitabilityRegister"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=ProfitabilityRegister.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Profitability Register<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
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

/*Add from buffer*/
        public ActionResult TrackContainerList(string ContainerNo)
        {

            if (ContainerNo != "")
            {
                ViewBag.ContainerNo = ContainerNo;
            }
            return View();
        }

        public ActionResult getContainerSearchList(string ContainerNo)
        {
            int userid = Convert.ToInt32(Session["userid"]);
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            db.sub_ExecuteNonQuery("usp_Insert_SearchContainer '" + ContainerNo + "','" + userid + "'");
            List<BE.JobOrderDEntities> ContainerSearchList = new List<BE.JobOrderDEntities>();
            ContainerSearchList = reportprovider.getContainerSearchList(ContainerNo);
            return Json(ContainerSearchList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SalesRegisterReport()
        {
           
            return View();
        }

        /*add from cargo Track*/
        public JsonResult ajaxImportSearchlistdetails(string ContainerNo, string Jono)
        {
            string ContainerNumber = Convert.ToString(ContainerNo);
            string Jonumber = Convert.ToString(Jono);

            BE.ImportSearchEntities ImportSearchdetails = new BE.ImportSearchEntities();
            ImportSearchdetails = trainTrackerProvider.GetImportSearchDetails(ContainerNumber, Jonumber);

            List<BE.GetIGMDetailsOnJONO> ImportSearchdetailsList = new List<BE.GetIGMDetailsOnJONO>();
            ImportSearchdetailsList = trainTrackerProvider.SearchImportSearchList(Jonumber, ContainerNumber);

            //List<BE.GetIGMDetailsOnJONO> TimeLineList = new List<BE.GetIGMDetailsOnJONO>();
            //TimeLineList = trainTrackerProvider.GetTimeLine(Jonumber, ContainerNumber);
            //if (TimeLineList is null)
            //{
            //    ViewBag.TimeLineList = 0;
            //}
            //ViewBag.TimeLineList = TimeLineList;

            var returnField = new { ImportList = ImportSearchdetails, ImportDetails = ImportSearchdetailsList };
            return new JsonResult() { Data = returnField, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public ActionResult ImportTimeline(string ContainerNo, string Jono)
        {
            string ContainerNumber = Convert.ToString(ContainerNo);
            string Jonumber = Convert.ToString(Jono);
            List<BE.ImportSearchEntities> TimeLineList = new List<BE.ImportSearchEntities>();
            TimeLineList = trainTrackerProvider.GetTimeLine(Jonumber, ContainerNumber);
            return PartialView(TimeLineList.ToList());
        }

        public ActionResult GetIGMDeatails(string Jono, string ContainerNo)
        {

            DataTable GetMonthSummary = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            GetMonthSummary = db.sub_GetDatatable("sp_GetIGMDetailsOn_JOConts2  '" + Jono + "','" + ContainerNo + "'");
            Session["IGMDetails"] = GetMonthSummary;
            //Session["SP_ImpInDayWise_Summary"] = GetMonthSummary;
            Session["Ason"] = DateTime.Now;
            var json = JsonConvert.SerializeObject(GetMonthSummary);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult GetIGMInvoiceDeatails(string Jono, string ContainerNo)
        {

            DataTable GetMonthSummary = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            GetMonthSummary = db.sub_GetDatatable("usp_showAssessdetails  '" + Jono + "','" + ContainerNo + "'");
            //Session["SP_ImpInDayWise_Summary"] = GetMonthSummary;
            Session["InvoiceDetails"] = GetMonthSummary;
            Session["Ason"] = DateTime.Now;
            var json = JsonConvert.SerializeObject(GetMonthSummary);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult GetIGMHoldDeatails(string Jono, string ContainerNo)
        {

            DataTable GetMonthSummary = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            GetMonthSummary = db.sub_GetDatatable("usp_Holddet  '" + Jono + "','" + ContainerNo + "'");
            Session["HoldDetails"] = GetMonthSummary;
            Session["Ason"] = DateTime.Now;
            var json = JsonConvert.SerializeObject(GetMonthSummary);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        /*----*/
        public ActionResult GetSalesRegisterReport(string Fdate, string ToDate)
        {
            DataTable ProfitabilityRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            ProfitabilityRegister = db.sub_GetDatatable("SP_SalesRegister1 '" + Fdate + "','" + ToDate + "'");
            Session["SP_SalesRegister1"] = ProfitabilityRegister;
            Session["fromdate"] = Fdate;

            var json = JsonConvert.SerializeObject(ProfitabilityRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public JsonResult IMPSearchTimeline(string Process, string ContainerNo, string Jono)
        {
            DataTable GetMonthSummary = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            GetMonthSummary = db.sub_GetDatatable("USP_IMP_Search_Timeline '" + Process + "','" + Jono + "','" + ContainerNo + "'");
            //Session["SP_ImpInDayWise_Summary"] = GetMonthSummary;
            Session["Ason"] = DateTime.Now;
            var json = JsonConvert.SerializeObject(GetMonthSummary);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;

        }

        public ActionResult SalesRegisterReportExportToExcel()
        {
            DataTable dt = (DataTable)Session["SP_SalesRegister1"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=SalesRegisterReport.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Sales Register Report<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *output generated by JMM </h6></td></tr>");
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
        public ActionResult OutStandingStatement()
        {

            //List<BE.Customer> Customer = new List<BE.Customer>();
            //Customer = BL.getParty();
            //ViewBag.Customer = new SelectList(Customer, "AGID", "AGName");
            return View();
        }
        public ActionResult CartingEntryReport()
        {


            return View();
        }
        public ActionResult GetCartingEntry(string fromdate, string ToDate)
        {
            DataTable ProfitabilityRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            ProfitabilityRegister = db.sub_GetDatatable("USP_CartingReport '" + fromdate + "','" + ToDate + "'");
            Session["USP_CartingReport"] = ProfitabilityRegister;
            Session["fromdate"] = fromdate;

            var json = JsonConvert.SerializeObject(ProfitabilityRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult GetCartingEntryExportToExcel()
        {
            DataTable dt = (DataTable)Session["USP_CartingReport"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=CartingEntryReport.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Carting Entry Report<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *output generated by JMM </h6></td></tr>");
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
        public ActionResult EmptyGateInReport()
        {
             
            return View();
        }

        public ActionResult GetEmptyGateIn(string fromdate, string ToDate)
        {
            DataTable ProfitabilityRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            ProfitabilityRegister = db.sub_GetDatatable("USP_EmptyGateReport '" + fromdate + "','" + ToDate + "'");
            Session["USP_EmptyGateReport"] = ProfitabilityRegister;
            Session["fromdate"] = fromdate;

            var json = JsonConvert.SerializeObject(ProfitabilityRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult EmptyGateInExportToExcel()
        {
            DataTable dt = (DataTable)Session["USP_EmptyGateReport"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=EmptyGateInReport.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Empty Gate In Report<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *output generated by JMM </h6></td></tr>");
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

        public ActionResult EmptyOutReport()
        {

            return View();
        }

        public ActionResult GetEmptyOut(string fromdate, string todate)
        {
            DataTable ProfitabilityRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            ProfitabilityRegister = db.sub_GetDatatable("USP_EmptyOutReport '" + fromdate + "','" + todate + "'");
            Session["USP_EmptyOutReport"] = ProfitabilityRegister;
            Session["fromdate"] = fromdate;

            var json = JsonConvert.SerializeObject(ProfitabilityRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult EmptyOutExportToExcel()
        {
            DataTable dt = (DataTable)Session["SP_SalesRegister1"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=EmptyOutReport.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Empty Out Report<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *output generated by JMM </h6></td></tr>");
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


        public ActionResult StuffingReport()
        {

            return View();
        }

        public ActionResult GetStuffing(string fromdate, string ToDate)
        {
            DataTable ProfitabilityRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            ProfitabilityRegister = db.sub_GetDatatable("USP_StuffingReport '" + fromdate + "','" + ToDate + "'");
            Session["USP_StuffingReport"] = ProfitabilityRegister;
            Session["fromdate"] = fromdate;

            var json = JsonConvert.SerializeObject(ProfitabilityRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult StuffingExportToExcel()
        {
            DataTable dt = (DataTable)Session["USP_StuffingReport"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=StuffingReport.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Stuffing Report<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *output generated by JMM </h6></td></tr>");
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
        public ActionResult ContainerGateoutReport()
        {

            return View();
        }
        public ActionResult GetContainerGateout(string fromdate, string ToDate)
        {
            DataTable ProfitabilityRegister = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            int Userid = Convert.ToInt32(Session["userid"]);
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            ProfitabilityRegister = db.sub_GetDatatable("USP_GateOutReport '" + fromdate + "','" + ToDate + "'");
            Session["USP_GateOutReport"] = ProfitabilityRegister;
            Session["fromdate"] = fromdate;

            var json = JsonConvert.SerializeObject(ProfitabilityRegister);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult ContainerGatExportToExcel()
        {
            DataTable dt = (DataTable)Session["USP_GateOutReport"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=GateOutReport.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Gate Out Report<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *output generated by JMM </h6></td></tr>");
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

        public ActionResult ImportAccountMaster()
        {
            ViewBag.Date = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");

            List<BE.ExpHeadMasterEnt> InvoiceType = new List<BE.ExpHeadMasterEnt>();
            InvoiceType = Masteprovider.InvoiceTypeDDL();
            ViewBag.InvoiceDDL = new SelectList(InvoiceType, "InvTId", "InvType");

            List<BE.ExpHeadMasterEnt> HSNSelect = new List<BE.ExpHeadMasterEnt>();
            HSNSelect = Masteprovider.HSNCodeDDL();
            ViewBag.HSNDDList = new SelectList(HSNSelect, "HSNID", "HSNCodeL");

            List<BE.ExpHeadMasterEnt> TaxName = new List<BE.ExpHeadMasterEnt>();
            TaxName = Masteprovider.getTaxName();
            ViewBag.TaxName = new SelectList(TaxName, "TaxID", "TaxName");

            List<BE.ExpHeadMasterEnt> IMPGroup = new List<BE.ExpHeadMasterEnt>();
            IMPGroup = Masteprovider.IMPGroupDDl();
            ViewBag.importg = new SelectList(IMPGroup, "IMPGID", "IMPGName");
            return View();

        }

        public JsonResult ChecktheAccountmasterAlready(BE.ExpHeadMasterEnt ExpHeadMasterEnt)
        {
            string message = "OK"; // default
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            DataTable SvaDT = db.sub_GetDatatable(
                "SP_SaveAccountMaster_Check_EXISTS '" + ExpHeadMasterEnt.EntryID.ToString() +
                "', '" + ExpHeadMasterEnt.AcName.Replace("'", "''") + "'"
            );

            if (SvaDT.Rows.Count > 0)
            {
                message = SvaDT.Rows[0]["Message"].ToString();
            }

            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveIAM(BE.ExpHeadMasterEnt ExpHeadMasterEnt)
        {
            string message = "";
            var EntryDate = ExpHeadMasterEnt.EntryDate;
            //HC.DBOperation object = new HC.DBOperations(); From Helper
            DataTable SvaDT = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            //Code For Insert Data Sequence Should Be Same As Created SP.
            SvaDT = db.sub_GetDatatable("SP_SaveAccountMaster_New '" + ExpHeadMasterEnt.EntryID + "','" + ExpHeadMasterEnt.AcName + "','" + ExpHeadMasterEnt.TallyName + "','" + ExpHeadMasterEnt.disp + "','" + ExpHeadMasterEnt.IsActive + "','" + ExpHeadMasterEnt.IMPGID + "','" + ExpHeadMasterEnt.HSNCodeL + "','" + Convert.ToInt32(Session["userid"]) + "','" + ExpHeadMasterEnt.isdpd + "','" + ExpHeadMasterEnt.InvTId + "','" + ExpHeadMasterEnt.TaxID + "','" + ExpHeadMasterEnt.chargefors + "'");

            if (SvaDT.Rows.Count > 0)
            {
                message = Convert.ToString(SvaDT.Rows[0]["Message"]);
            }

            return Json(message);
        }

        public JsonResult GetImportAcList(string search)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataTable dt = new DataTable();
            dt = db.sub_GetDatatable("USP_SearchAccountDetails'" + search + "'");

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

        public ActionResult ExportToExcelImportAccountMaster()
        {
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            DataTable getMovementICDNew = (DataTable)Session["SearchAccountDetails"];


            GridView gridview = new GridView();
            gridview.DataSource = getMovementICDNew;


            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=Import_Account_Master_Report.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))

                {

                    // render the GridView to the HtmlTextWriter
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Import Account Master Reportt<strong></td></tr>");

                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'>  </h6></td></tr>");
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            return View();
        }



        /* SalesRegisterReport*/
        public ActionResult DailyCollectionReoprt()
        {
            return View();
        }
        public ActionResult getDailyCollection(string Criteria, string FromDate, string ToDate)
        {
            DataTable getMovementICDNew = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            getMovementICDNew = db.sub_GetDatatable("GetDailyCollection  '" + FromDate + "','" + ToDate + "'");
            //GetDailyActivityTripINOutreport.Columns.Remove("SR_NO");
            Session["GetDailyCollection"] = getMovementICDNew;
            Session["fromdate"] = FromDate;
            Session["todate"] = ToDate;
            var json = JsonConvert.SerializeObject(getMovementICDNew);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult DailyCollectionReport(string fromdate, string todate)
        {
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            // dt = db.sub_GetDatatable("USP_GetContainerSurveyRemarks '" + containerNo + "'");
            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            DataTable getMovementICDNew = (DataTable)Session["GetDailyCollection"];
            string Tittle = "From " + Session["fromdate"] + " To " + Session["todate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = getMovementICDNew;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=DailyCollection.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))

                {

                    // render the GridView to the HtmlTextWriter
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'> Daily Collection Report<strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *system generated output </h6></td></tr>");
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            return View();
        }


        public ActionResult CustomerandAgingWiseOutstanding()
        {

            List<BE.Customer> Customer = new List<BE.Customer>();
            Customer = Masteprovider.getParty();
            ViewBag.Customer = new SelectList(Customer, "AGID", "AGName");

            List<BE.Category> Category = new List<BE.Category>();
            Category = Masteprovider.getCategory();
            ViewBag.Category = new SelectList(Category, "ID", "CategoryName");


            return View();
        }

        [HttpPost]
        public ActionResult AjaxCustomerandInvoicePartyName(string party)
        {
            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_GetCustomerandgingamount_wise_Party '" + party + "'");
            Session["CustomerandpartywiseDetails"] = dt;
            Session["asonpartywise"] = DateTime.Now;
            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public ActionResult GetCustomerandagingreport(string CustomerID, string Datetime)
        {
            if (CustomerID == "" || CustomerID == null)
            {
                CustomerID = "0";
            }

            DataTable getMovementICDNew = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            getMovementICDNew = db.sub_GetDatatable("USP_AGING_CUST_WISE_OS '" + CustomerID + "','" + Datetime + "'");

            Session["Ason"] = DateTime.Now;
            var json = JsonConvert.SerializeObject(getMovementICDNew);
            getMovementICDNew.Columns.Remove("View");
            Session["Customerandageingreport"] = getMovementICDNew;
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public ActionResult GetCustomerandageingDetailsreport()
        {
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond(); 
            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            DataTable getMovementICDNew = (DataTable)Session["Customerandageingreport"];
            string Tittle = "AsOn " + Session["Ason"] + "";

            GridView gridview = new GridView();
            gridview.DataSource = getMovementICDNew;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=Customerandageingoutstandingreport.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))

                {

                    // render the GridView to the HtmlTextWriter
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Customer and Aging Wise Out standing Report<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *system generated output </h6></td></tr>");
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            return View();
        }


        public JsonResult getPartyNameReceipt(string prefixText, string Mode)
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataTable dataTable = new DataTable();
            List<BE.Customer> Customerlst = new List<BE.Customer>();
            dataTable = db.sub_GetDatatable("USP_GetPartyNameReceipt '" + Mode + "','" + prefixText + "'");

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
        public ActionResult CustomerAndInvoiceOutStanding()
        {
            List<BE.PartyNameEntities> CustomerMaster = new List<BE.PartyNameEntities>();
            CustomerMaster = reportprovider.Getpartyname();
            ViewBag.customer = new SelectList(CustomerMaster, "Common_ID", "GSTName");
            return View();
        }

        [HttpPost]
        public ActionResult AjaxCustomerAndInvoiceOutStanding(string Partyname)
        {
            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("USP_CUST_OUTStanding_INV_Wise '" + Partyname + "'");
            Session["CustomerAndInoiveReport"] = dt;
            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public ActionResult ExportToExcelCustomerandInvoiceDetails()
        {
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            DataTable getMovementICDNew = (DataTable)Session["CustomerAndInoiveReport"];
            GridView gridview = new GridView();
            gridview.DataSource = getMovementICDNew;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=Customer & Invoice Wise Outstanding.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))

                {

                    // render the GridView to the HtmlTextWriter
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Customer & Invoice Wise Outstanding Report<strong></td></tr>");
                    // htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *system generated output </h6></td></tr>");
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            return View();
        }

        public ActionResult CategoryWiseTDSSummary()
        {
            List<BE.PartyNameEntities> CustomerMaster = new List<BE.PartyNameEntities>();
            CustomerMaster = reportprovider.Getpartyname();
            ViewBag.customer = new SelectList(CustomerMaster, "Common_ID", "GSTName");
            return View();
        }

        [HttpPost]
        public ActionResult GetTDSSummary(string FromDate, string ToDate, string category)
        {
            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            dt = db.sub_GetDatatable("SP_CategoryWiseTDSAll '" + FromDate + "','" + ToDate + "','" + category + "'");
            Session["TDSSummaryList"] = dt;
            Session["FromDate"] = FromDate;
            Session["ToDate"] = ToDate;
            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        public ActionResult ExportToExcelTDSSummaryDetails()
        {
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            DataTable getMovementICDNew = (DataTable)Session["TDSSummaryList"];
            string Tittle = "FromDate " + Session["FromDate"] + "ToDate " + Session["ToDate"] + "";

            GridView gridview = new GridView();
            gridview.DataSource = getMovementICDNew;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=TDS Summary Report.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))

                {

                    // render the GridView to the HtmlTextWriter
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>TDS Report<strong></td></tr>");
                    htw.Write("<table><tr><td  style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *system generated output </h6></td></tr>");
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            return View();
        }

        public ActionResult CategoryAndHeadWiseReport()
        {
            List<BE.PartyNameEntities> CustomerMaster = new List<BE.PartyNameEntities>();
            CustomerMaster = reportprovider.Getpartyname();
            ViewBag.customer = new SelectList(CustomerMaster, "Common_ID", "GSTName");
            return View();
        }

        [HttpPost]
        public ActionResult GetCategoryAndHeadWiseReport(string FromDate, string ToDate, string category)
        {
            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            if (category == "IMPORT")
            {
                dt = db.sub_GetDatatable("Get_sp_ImportWiseCollectionSummary '" + FromDate + "','" + ToDate + "'");

            }

            if (category == "EXPORT")
            {
                dt = db.sub_GetDatatable("Get_sp_EXPORTWiseCollectionSummary '" + FromDate + "','" + ToDate + "'");

            }
            if (category == "BOND")
            {
                dt = db.sub_GetDatatable("Get_sp_BONDWiseCollectionSummary '" + FromDate + "','" + ToDate + "'");

            }
            if (category == "DOMESTIC")
            {
                dt = db.sub_GetDatatable("Get_sp_DOMESTICWiseCollectionSummary  '" + FromDate + "','" + ToDate + "'");

            }
            if (category == "MISC")
            {
                dt = db.sub_GetDatatable("Get_sp_MISCWiseCollectionSummary '" + FromDate + "','" + ToDate + "'");

            }

            if (category == "MNR")
            {
                dt = db.sub_GetDatatable("Get_sp_MNRWiseCollectionSummary '" + FromDate + "','" + ToDate + "'");

            }

            Session["TDSSummaryList"] = dt;
            Session["FromDate"] = FromDate;
            Session["ToDate"] = ToDate;
            Session["category"] = category;
            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        public ActionResult ExportToExcelCategoryAndHeadWiseReport()
        {
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            DataTable getMovementICDNew = (DataTable)Session["TDSSummaryList"];
            string Tittle = "FromDate " + Session["FromDate"] + "ToDate " + Session["ToDate"] + " " + "category " + Session["category"] + "";

            GridView gridview = new GridView();
            gridview.DataSource = getMovementICDNew;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=CategoryAndHeadWiseReport Report.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))

                {

                    // render the GridView to the HtmlTextWriter
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Category And Head Wise Report<strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *system generated output </h6></td></tr>");
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            return View();
        }
        public ActionResult PartyWiseCollectionDetails()
        {


            return View();
        }
        public ActionResult ExportToExcelPartyWiseCollection()
        {
            DataTable dt = (DataTable)Session["EXPPartyWise"];
            DataTable CompanyMaster = new DataTable();
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            CompanyMaster = db.sub_GetDatatable("USP_COMPANYDETAILS");
            var CompanyName = Convert.ToString(CompanyMaster.Rows[0]["con_Name"]);
            var CompanyAddress = Convert.ToString(CompanyMaster.Rows[0]["AddressI"]);
            string Tittle = "From " + Session["FromDate"] + " To " + Session["ToDate"] + ".";
            GridView gridview = new GridView();
            gridview.DataSource = dt;
            gridview.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=PartyWiseCollectionReport.xls");
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 26px'>" + CompanyName + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + CompanyAddress + " <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>Party Wise Collection Report <strong></td></tr>");
                    htw.Write("<table><tr><td style='font-weight: bold; text-align: center'; colspan ='7'><strong style='font-size: 15px'>" + Tittle + " <strong></td></tr>");
                    htw.Write("<table><tr><td colspan='7'><h6 style='text-align:left'> *system generated output </h6></td></tr>");
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
        public ActionResult GetPartyWiseCollection(string FromDate, string ToDate)
        {
            DataTable dt = new DataTable();
            DataTable CompanyMaster = new DataTable();

            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();

            dt = db.sub_GetDatatable("USP_Party_Wise_Collection'" + Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd HH:mm:ss") + "','" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd HH:mm:ss") + "'");
            Session["EXPPartyWise"] = dt;
            Session["FromDate"] = FromDate;
            Session["ToDate"] = ToDate;

            string json = JsonConvert.SerializeObject(dt);
            var jsonResult = Json(json, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;

        }
        [HttpPost]
        public ActionResult GetCollectionDetailssummary(string GetValue)
        {
            DateTime baseDate = DateTime.Today;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            if (GetValue == "Today")
            {

                //startDate = new DateTime(now.Year, now.Month, 1);
                //endDate = baseDate.AddMonths(1).AddDays(-1);
            }
            if (GetValue == "Yesterday")
            {
                DateTime now = DateTime.Now;
                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = baseDate.AddMonths(1).AddDays(-1);
            }
            if (GetValue == "Week")
            {
                startDate = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                endDate = startDate.AddDays(7).AddSeconds(-1);
            }
            if (GetValue == "Month")
            {
                startDate = baseDate.AddDays(1 - baseDate.Day);
                endDate = startDate.AddMonths(1).AddSeconds(-1);
            }

            string fromdate = Convert.ToDateTime(startDate).ToString("yyyy-MM-dd HH:mm");
            string Todate = Convert.ToDateTime(endDate).ToString("yyyy-MM-dd HH:mm");


            List<BE.CollectionSummaryEntities> MovementReport = new List<BE.CollectionSummaryEntities>();
            MovementReport = reportprovider.GetCollectionSummarydetails(fromdate, Todate);

            var jsonResult = Json(MovementReport, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        public ActionResult CollectionSummaryDetails()
        {
            return View();
        }


    }
}