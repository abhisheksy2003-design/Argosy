using MVCBOND.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using BO = MvcBondEntities.BusinessEntities;
using DP = MvcBondBusinessLayer.Login;
using HC = MvcBondDataLayer.Helper;

namespace MVCBOND.Controllers
{
    public class HomeController : Controller
    {
        DP.LoginDataProvider objloginDataProvider = new DP.LoginDataProvider();
        HttpCookie sessionInfo = new HttpCookie("sessionInfo");
        public ActionResult Index()
        {

            BO.UserDetail logindata = new BO.UserDetail();
            string User_name = string.Empty;
            //string User_color = string.Empty;
            HttpCookie reqCookies = Request.Cookies["userInfoBTC"];

            if (reqCookies != null)
            {
                logindata.UserName = reqCookies["UserName"].ToString();
                logindata.Password = reqCookies["Password"].ToString();
                logindata.rememberme = true;

            }

            return View("index", logindata);


        }

        [HttpPost]
        //[UserAuthenticationFilter]
        public ActionResult SubmitLogin(string username, string password)
        {
            string Message = "successful";

            //var name = loginEntities.UserName;
            //var pass = loginEntities.Password;
            //var rememberme = loginEntities.rememberme;
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            string mac = nics[0].GetPhysicalAddress().ToString();
            BO.UserDetail logindata = new BO.UserDetail();
            string ip = Request.UserHostAddress;
            logindata = objloginDataProvider.LogingetData(username, password);
            if (logindata.ID != 0)
            {
                //RememberMe(name, pass, rememberme);
                Session["usertype"] = logindata.UserType;
                Session["userid"] = logindata.ID;
                Session["username"] = logindata.UserName;
                Session["tokenpath"] = logindata.Token;
                Session["toUserType"] = logindata.UserType.Trim();
                Session["UserEmailID"] = logindata.EmailID;

                //Session["UserImage"] = "temp"; // need to ask

                HttpCookie MediSoft_userCookies = new HttpCookie("MediSoft_userCookies");
                MediSoft_userCookies["usertype"] = Convert.ToString(logindata.UserType);
                MediSoft_userCookies["userid"] = Convert.ToString(logindata.ID);
                MediSoft_userCookies["username"] = logindata.UserName;
                MediSoft_userCookies["mode"] = "default";
                MediSoft_userCookies["toUserType"] = Convert.ToString(logindata.UserType);
                MediSoft_userCookies["UserEmailID"] = logindata.EmailID;

                Response.Cookies.Add(MediSoft_userCookies);

            

                return Json(logindata);
            }
            else
            {
                Message = "Wrong Username or Password";
                return Json(Message);
            }


            ///you can use int txtId  here 

        }


        public ActionResult Dashboard()
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataSet ds = db.sub_GetDataSets("uspGetGenericDashboardData");

            // Summary Table
            DataTable dtSummary = ds.Tables[0];

            // Chart Tables
            DataTable dtGateIn = ds.Tables[1];
            DataTable dtGateOut = ds.Tables[2];
            DataTable dtEmptyIn = ds.Tables[3];
            DataTable dtLoadedOut = ds.Tables[4];

            // Create Lists
            List<BO.DashboarData> GateInList = new List<BO.DashboarData>();
            List<BO.DashboarData> GateOutList = new List<BO.DashboarData>();
            List<BO.DashboarData> EmptyInList = new List<BO.DashboarData>();
            List<BO.DashboarData> LoadedOutList = new List<BO.DashboarData>();


            //-------------------------
            //  CHART DATA FILL
            //-------------------------

            foreach (DataRow row in dtGateIn.Rows)
                GateInList.Add(new BO.DashboarData
                {
                    Date = Convert.ToDateTime(row["Date"]),
                    figure = row["Count"].ToString()
                });

            foreach (DataRow row in dtGateOut.Rows)
                GateOutList.Add(new BO.DashboarData
                {
                    Date = Convert.ToDateTime(row["Date"]),
                    figure = row["Count"].ToString()
                });

            foreach (DataRow row in dtEmptyIn.Rows)
                EmptyInList.Add(new BO.DashboarData
                {
                    Date = Convert.ToDateTime(row["Date"]),
                    figure = row["Count"].ToString()
                });

            foreach (DataRow row in dtLoadedOut.Rows)
                LoadedOutList.Add(new BO.DashboarData
                {
                    Date = Convert.ToDateTime(row["Date"]),
                    figure = row["Count"].ToString()
                });

            // Assign lists to ViewBag
            ViewBag.GateInDaily = GateInList;
            ViewBag.GateOutDaily = GateOutList;
            ViewBag.EmptyInDaily = EmptyInList;
            ViewBag.LoadedOutDaily = LoadedOutList;


            //-------------------------
            // SUMMARY DATA (from DB)
            //-------------------------

            ViewBag.JobOrderCount = dtSummary.Rows[0]["JobOrder"];
            ViewBag.GetInCount = dtSummary.Rows[0]["GateIn"];
            ViewBag.InvoiceCount = dtSummary.Rows[0]["Invoice"];
            ViewBag.ReceiptCount = dtSummary.Rows[0]["Receipt"];


            //-------------------------
            // EXTRA BASIC DETAILS (Dummy Data)
            //-------------------------

            ViewBag.Customer = 14;
            ViewBag.Importer = 16;
            ViewBag.CHA = 60;
            ViewBag.Exporter = 90;
            ViewBag.SalesPerson = 10;

            //-------------------------
            // EXTRA IMPORT DETAILS (Dummy)
            //-------------------------

            ViewBag.ImportJobs = 42;         // Dummy
            ViewBag.GateInCount = 18;        // Already assigned above (you can overwrite)
            ViewBag.GateOutCount = 15;       // Dummy

            //-------------------------
            // EXTRA EXPORT DETAILS (Dummy)
            //-------------------------

            ViewBag.EmptyInCount = 26;       // Dummy
            ViewBag.CartingCount = 19;       // Dummy
            ViewBag.StuffingCount = 22;      // Dummy
            ViewBag.LoadedOutCount = 14;     // Dummy

            return View();
        }
        public ActionResult AdminDashboard()
        {
            return View();
        }


        public ActionResult logout()
        {
            Session.Abandon();
            return RedirectToAction("index", "Home");
        }


        private void RememberMe(String name, String password, Boolean rememberme)
        {

            if (rememberme)
            {
                HttpCookie userInfo = new HttpCookie("userInfoBTC");
                userInfo["UserName"] = name;
                userInfo["Password"] = password;
                userInfo.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(userInfo);

            }
            else
            {
                HttpCookie userInfo = new HttpCookie("userInfo");
                userInfo.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(userInfo);
            }



        }

        [UserAuthenticationFilter]
        public ActionResult SideMenu()
        {
            List<BO.SubmenuInfo> MenuList = new List<BO.SubmenuInfo>();

            int SessionuserID = Convert.ToInt32(Session["userid"]);
            MenuList = objloginDataProvider.UserMenuList(SessionuserID);
            Session["MenuList"] = MenuList;
            return PartialView("SideMenu", MenuList);


            //return PartialView("SideMenu", MenuList);
        }


        public JsonResult GetDashboardCards()
        {
            HC.DBOperationsForMvcBond db = new HC.DBOperationsForMvcBond();
            DataSet ds = db.sub_GetDataSets("usp_GetCustomerCountFromCommonMaster");

            DataTable dt = ds.Tables[0];  

            var data = new
            {
                Customer = dt.Rows[0]["Customer"],
                Impoter = dt.Rows[0]["Impoter"],
                Exporter = dt.Rows[0]["Exporter"],
                CHA = dt.Rows[0]["CHA"],
                TotalSalesPerson = dt.Rows[0]["TotalSalesPerson"],
                TotalJobs = dt.Rows[0]["TotalJobs"],
                GetIn = dt.Rows[0]["GetIn"],
                GetOut = dt.Rows[0]["GetOut"],
                Carting = dt.Rows[0]["Carting"],
                Stuffing = dt.Rows[0]["Stuffing"],
                Loadedout = dt.Rows[0]["Loadedout"], 
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}