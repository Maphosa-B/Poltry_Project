using Poltry_Project.Poultry_Hub_SR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace Poltry_Project.Controllers
{
    public class MyAdminController : Controller
    {
        private Poltry_HubClient client = new Poltry_HubClient();


        // GET: Admin

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Site_Stats()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Purchases_Stats()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Cart_Stats()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Category(int Id)
        {
            return View(client.Get_Single_Category(Id));
        }

        [HttpPost]
        public ActionResult Edit_Category(FormCollection fc)
        {
            if(client.Edit_Category(new Types_Of_Chick() { Id=Convert.ToInt32(fc["Id"]) ,Name = fc["Cat_Name"]})==1)
            {
                TempData["status"] = "Category has been updated";     
                return RedirectToAction("Add_Category", "myAdmin");
            }
            else
            {
                TempData["error"] = "There was some technical errors, category was not updated";
                return View(client.Get_Single_Category(Convert.ToInt32(fc["Id"])));
            }
        }




        public ActionResult Delete_Category(int Id)
        {
            if(client.Delete_Category(Id)==1)
            {
                TempData["status"] = "Category has been deleted";
                return RedirectToAction("Add_Category", "myAdmin");
            }
            else
            {
                TempData["error"] = "There was some technical errors, category was not deleted";
                return RedirectToAction("Add_Category", "myAdmin");
            }
        }



        [HttpGet]
        public ActionResult Add_Category()
        {
            return View(client.Get_All_Types_Of_Chicks());
        }



        [HttpPost]
        public ActionResult Add_Category(FormCollection fc)
        {
            Types_Of_Chick tc = new Types_Of_Chick()
            {
                Name = fc["Cat_Name"]     
            };

            if(client.Add_Category(tc)==1)
            {
                TempData["status"] = "Category has been added";
                return RedirectToAction("Add_Category","myAdmin");
            }else
            {
                TempData["error"] = "There was some error, category was not added";
                return RedirectToAction("Add_Category", "myAdmin");
            }

        }

        public ActionResult Get_Customers()
        {
            return View(client.Get_Users_By_Role(4));
        }

        [HttpGet]
        public ActionResult Get_Businesses()
        {
            return View(client.Get_Users_By_Role(2));
        }

        [HttpGet]
        public ActionResult Get_Drivers()
        {
            return View(client.Get_Users_By_Role(3));
        }

        public ActionResult Get_Customer_Details(int Id)
        {
            return View(client.Get_User(Id));
        }

        [HttpGet]
        public ActionResult Get_Driver_Details(int Id)
        {
            return View(client.Get_User(Id));
        }

        [HttpGet]
        public ActionResult Get_Business_Details(int Id)
        {
            return View(client.Get_User(Id));
        }


        [HttpGet]
        public ActionResult Add_User()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Add_User(FormCollection fc)
        {
            User u = new User()
            {
                Add_Date = DateTime.Now, 
                Contacts = fc["Cell_No"],
                DOB = Convert.ToDateTime(fc["DOB"]),
                Email = fc["Email"],
                First_Name = fc["Fisrt_Name"],
                Gender = Convert.ToInt32(fc["Gender"]),
                Is_Active = 1,
                Last_Name = fc["Last_Name"],
                Password = fc["Email"],
            };

            Address a = new Address()
            {
                Postal_Code = fc["Postal_Code"],
                Province_Id = Convert.ToInt32(fc["Province"]),
                City = fc["City"],
                Street = fc["Street"],
                User_Id = Convert.ToInt32(Session["User_Id"]),
            };

            int uStatus = client.Register(u);


            if(uStatus==2)
            {
                TempData["error"] = "User was not added, email address has alredy been taken";
                return RedirectToAction("Add_User","myAdmin");
            }else if(uStatus == 3)
            {
                TempData["error"] = "User was not added, cell phone number has alredy been taken";
                return RedirectToAction("Add_User", "myAdmin");
            }else if(uStatus == -1)
            {
                TempData["error"] = "User was not added, there was some technical errors";
                return RedirectToAction("Add_User", "myAdmin");
            }else
            {
                int addressStatus = client.Add_Address(a);
                if(addressStatus==1)
                {
                    client.Add_User_Role(new User_Role() { Role_Id = Convert.ToInt32(fc["UserType"]), User_Id = uStatus });
                    TempData["status"] = "User has been addeda";

                    if(Convert.ToInt32(fc["UserType"])==2)
                    {
                        return RedirectToAction("Get_Businesses", "myAdmin");
                    }
                    else if(Convert.ToInt32(fc["UserType"])==3)
                    {
                        return RedirectToAction("Get_Drivers", "myAdmin");
                    }
                    else
                    {
                        return RedirectToAction("Get_Customers", "myAdmin");
                    }
                 
                }else
                {
                    TempData["error"] = "User was not added, there was some technical errors";
                    return RedirectToAction("Add_User", "myAdmin");
                }

            }
        }
    }
}