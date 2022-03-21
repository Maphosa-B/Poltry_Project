using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Poltry_Project.Poultry_Hub_SR;

namespace Poltry_Project.Controllers
{
    public class DriverController : Controller
    {
        Poltry_HubClient client = new Poltry_HubClient();
      
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Get_Orders()
        {
            return View(client.Get_Orders_To_Be_Delevered());
        }

        [HttpGet]
        public ActionResult View_Single_Order(int? Id )
        {
            if(Id!=null)
            {
                return View(client.Get_Single_Transaction(Convert.ToInt32(Id)));
            }
            else
            {
                return RedirectToAction("Get_Orders","Driver");
            }
            
        }

        [HttpPost]
        public ActionResult Accept_Delevery(FormCollection fc)
        {
            var trans = client.Get_Single_Transaction(Convert.ToInt32(fc["Transaction_Id"]));

            Trasportation t = new Trasportation()
            {
                Add_Date = DateTime.Now,
                Customer_Id = trans.Customer_Id,
                Driver_Id = Convert.ToInt32(Session["User_Id"]),
                Is_Cancelled =0,
                Is_Delivered = 0,
                Transaction_Id = trans.Id,
                Unique_Transaction_Id = trans.Unique_Transaction_Id,          
            };

            if(client.Make_Delivery(t)==1)
            {
                TempData["status"] = "Delivery has been taken";
                return RedirectToAction("index","Driver");
            }else
            {
                TempData["error"] = "There was some techical errors";
                return RedirectToAction("Get_Orders","Driver");
            }        
        }

        [HttpGet]
        public ActionResult Get_Accepted_Orders()
        {
            TempData["status"] = Convert.ToInt32(Session["User_Id"]);

            return View(client.Get_Driver_Deliveies(Convert.ToInt32(Session["User_Id"])));
        }

        [HttpGet]
        public ActionResult Get_Single_Delevery(int Id)
        {
            return View(client.Get_Single_Transaction(Id));
        }

        [HttpGet]
        public ActionResult Sign_Out()
        {
            Session.Clear();
            return RedirectToAction("Index","Home");
        }


        [HttpPost]
        public ActionResult Delevered(int Id)
        {
            if(client.Delivered(Convert.ToInt32(Id))==1)
            {
                TempData["status"] = "Thank you for making the delevery";
                return RedirectToAction("Get_Accepted_Orders","Driver");
            }

            TempData["error"] = "There was some technical errors";
            return View();
        }

        [HttpGet]
        public ActionResult Get_Driver_Delivery_History()
        {          
            return View(client.Get_Driver_Deliveies_History(Convert.ToInt32(Session["User_Id"])));
        }

         [HttpGet]
        public ActionResult Get_Driver_Single_Delivery_History(int Id)
        {          
            return View(client.Get_Single_Delivery(Id));
        }
    }
}