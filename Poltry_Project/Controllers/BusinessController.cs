using Poltry_Project.Models;
using Poltry_Project.Poultry_Hub_SR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Poltry_Project.Controllers
{
    public class BusinessController : Controller
    {
        private Poltry_HubClient client = new Poltry_HubClient();

        // GET: Business
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Delete_Item(int Id)
        {
            if (client.Delete_Item(Id) == 1)
            {
                TempData["status"] = "Item has been deleted";
                return RedirectToAction("Get_All_User_Items", "Business");
            }
            else
            {
                TempData["error"] = "There was an error, item was not deleted";
                return RedirectToAction("Get_All_User_Items", "Business");
            }

        }

        public ActionResult Get_All_Orders()
        {
            return View(client.Get_Business_Orders(Convert.ToInt32(Session["User_Id"])));
        }

        public ActionResult Get_Single_Order(int Id)
        {
            return View(client.Get_Business_Single_Orders(Convert.ToInt32(Id)));
        }

        public ActionResult Get_All_To_Be_Delevered_Orders()
        {
            return View(client.Get_Business_To_Be_Delivered_Orders(Convert.ToInt32(Session["User_Id"])));
        }

        public ActionResult Get_Single_To_Be_Delevered_Order(int Id)
        {
            return View(client.Get_Business_Single_Orders_To_Be_Delivered(Convert.ToInt32(Id)));
        }

        public ActionResult Get_All__Delevered_Orders()
        {
            return View(client.Get_Business_Delivered_Orders(Convert.ToInt32(Session["User_Id"])));
        }

        public ActionResult Get_Single_Delevered_Order(int Id)
        {
            return View(client.Get_Business_Single_Delivered_Orders(Convert.ToInt32(Id)));
        }

        public ActionResult Get_All_User_Items()
        {
            return View(client.Get_User_Items(Convert.ToInt32(Session["User_Id"])));
        }

        public ActionResult Edit_Item(int Id)
        {
            return View(client.Get_Item_By_Id(Id));
        }

        public ActionResult Add_Item()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Add_Item(cAdd_Image c, FormCollection fc)
        {
            String ext = Path.GetExtension(c.imageForGallery.FileName);
            String filename = "Craft_Pic_" + Guid.NewGuid().ToString().Substring(0,9) + ext;
            String myPath = "~/Front_Files/images/Products/Chickens/" + filename;
            filename = Path.Combine(Server.MapPath("~/Front_Files/images/Products/Chickens/"), filename);
            c.imageForGallery.SaveAs(filename);

            var user = client.Get_User(Convert.ToInt32(Session["User_Id"]));

            var cat = client.Get_Single_Category(Convert.ToInt32(Convert.ToInt32(fc["Category"])));

            Item Chick = new Item()
            {
                Add_date = DateTime.Now,
                Age = Convert.ToInt32(fc["Age"]),
                Description = fc["Desc"],
                Category_Id = Convert.ToInt32(fc["Category"]),
                Image_Path = myPath,
                Is_Active = 1,
                Name = cat.Name,
                Price = Convert.ToDecimal(fc["Price"]),
                Quantity = Convert.ToInt32(fc["Quantity"]),
                Supplier_Name = user.First_Name,
                User_Id = user.Id,
            };

            if(client.Add_Item(Chick)==1)
            {
                TempData["status"] = "Item has been added";
                return RedirectToAction("Get_All_User_Items", "Business");
            }else
            {
                TempData["error"] = "There was some technical errors, item was not added";
                return RedirectToAction("Add_Item", "Business");
            }
        }

        [HttpPost]
        public ActionResult Edit_Item(FormCollection fc)
        {
            decimal price = Convert.ToDecimal(fc["Price"]);

            if(price<1)
            {
                price = 1;
            }

            Item Chick = new Item()
            {
                Age = Convert.ToInt32(fc["Age"]),
                Description = fc["Desc"],
                Category_Id = Convert.ToInt32(fc["Category"]),
                Name = fc["Name"],
                Price = price,
                Quantity = Convert.ToInt32(fc["Quantity"]),
                Id = Convert.ToInt32(fc["Id"])
            };

            if (client.Edit_Item(Chick) == 1)
            {
                TempData["status"] = "Item has been updated";
                return RedirectToAction("Get_All_User_Items", "Business");
            }
            else
            {
                TempData["error"] = "There was some technical errors, item was not added";
                return RedirectToAction("Add_Item", "Business");
            }
        }
    }
}