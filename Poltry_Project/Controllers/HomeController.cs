using ClassLibrary1;
using Poltry_Project.Poultry_Hub_SR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Poltry_Project.Controllers
{
    public class HomeController : Controller
    {
        private Poltry_HubClient client = new Poltry_HubClient();

        public ActionResult Index()
        {
            TestClass a = new TestClass();


            ViewBag.Test = a.TestStr;
            return View();
        }


        [HttpGet]
        public ActionResult Sign_In()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Sign_In(FormCollection fc)
        {
            String Email = fc["email"];
            String Password = fc["Password"];

            int Status_Or_Id = client.Login(Email, Password);

            if (Status_Or_Id!=0)
            {
                var user = client.Get_User(Status_Or_Id);
                var role = client.Get_User_Role(Status_Or_Id);
                Session["User_Name"] = user.First_Name;
                Session["User_Id"] = Status_Or_Id;
                Session["User_Role"] = role;
                TempData["status"] = "Welcome";

                if (role ==1)
                {
                    return RedirectToAction("Index", "myAdmin");
                }
                else if(role==2)
                {
                    return RedirectToAction("Index", "Business");
                }
                else if(role==3)
                {
                    return RedirectToAction("Index","Driver");
                }else
                {
              
                    Session["Cart_Items_No"] = client.Number_Of_Items_In_User_Cart(Status_Or_Id);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["error"] = "Wrong Credintials";
                return RedirectToAction("Sign_In", "Home");
            }

        }

        public ActionResult Sign_Out()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public ActionResult Sign_Up()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Sign_Up(FormCollection fc)
        {
            User u = new User()
            {
                Contacts = fc["Cell_No"],
                DOB = Convert.ToDateTime(fc["DOB"]),
                First_Name = fc["First_Name"],
                Email = fc["Email"],
                Last_Name = fc["Last_Name"],
                Password = fc["Password"],
                Gender = Convert.ToInt32(fc["Gender"]),               
            };

            int status_or_Id = client.Register(u);

            if(status_or_Id>4)
            {
                TempData["status"] = "Almost Done, please add your address";
                Session["User_Id"] = status_or_Id;
                Session["User_Name"] = u.First_Name.ToUpper();
                Session["Cart_Items_No"] = 0;

                //Adding User Role
                client.Add_User_Role(new User_Role()
                {
                    User_Id = status_or_Id,
                    Role_Id = 4
                });


                return RedirectToAction("Add_Address", "Home");
           
            }else if(status_or_Id==2)
            {
                TempData["error"] = "Email address has already been taken, please user different email address ";
                return RedirectToAction("Sign_Up", "Home");
            }else if (status_or_Id == 3)
            {
                TempData["error"] = "Cell phone number has already been taken, please user different number";
                return RedirectToAction("Sign_Up", "Home");
            }else
            {
                TempData["error"] = "There was some technical errors, please try again after a while";
                return RedirectToAction("Sign_Up", "Home");
            }

        }

        [HttpGet]
        public ActionResult Add_Address()
        {

            return View(client.Get_All_Provinces());
        }

        [HttpPost]
        public ActionResult Add_Address(FormCollection fc)
        {
            Address a = new Address()
            {
                Postal_Code = fc["Postal_Code"],
                Province_Id = Convert.ToInt32(fc["Province"]),
                City = fc["City"],
                Street = fc["Street"],
                User_Id = Convert.ToInt32(Session["User_Id"]),
            };

            if(client.Add_Address(a)==1)
            {
                TempData["status"] = "Welcome";
                return RedirectToAction("Index", "Home");
            }else
            {
                TempData["error"] = "Address was not added, there was some technical errors";
                return RedirectToAction("Add_Address", "Home");
            }

        }

        public ActionResult Update_Profile()
        {

            return View(client.Get_User(Convert.ToInt32(Session["User_Id"])));
        }

        [HttpPost]
        public ActionResult Update_Profile(FormCollection fc)
        {
            Address a = new Address()
            {
                Id = Convert.ToInt32(fc["Address_Id"]),
                Postal_Code = fc["Postal_Code"],
                Province_Id = Convert.ToInt32(fc["Province"]),
                City = fc["City"],
                Street = fc["Street"],
                User_Id = Convert.ToInt32(Session["User_Id"]),
            };

            User u = new User()
            {
                Id = Convert.ToInt32(fc["User_Id"]),
                Contacts = fc["Cell_No"],
                DOB = Convert.ToDateTime(fc["DOB"]),
                First_Name = fc["First_Name"],
                Email = fc["Email"],
                Last_Name = fc["Last_Name"],
                Password = fc["Password"],
                Gender = Convert.ToInt32(fc["Gender"]),
            };

            if (client.Edit_Address(a)==1 && client.Update_Profile(u)==1)
            {
                TempData["status"] = "Profile has been updated";
                Session["User_Name"] = u.First_Name;
                return RedirectToAction("Update_Profile", "Home");

            }else
            {
                TempData["error"] = "There was some technical errors, please try again after a while";
                return RedirectToAction("Update_Profile", "Home");
            }
        }

        [HttpGet]
        public ActionResult Update_Password()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Update_Password(FormCollection fc)
        {

            User u = new User()
            {
                Id = Convert.ToInt32(Session["User_Id"]),
                Password = fc["Password"]
            };

            if(client.Change_Password(u)==1)
            {
                TempData["status"] = "Password has been updated";
                return RedirectToAction("Index","Home");
            }else
            {
                TempData["error"] = "There was some technical errors, please try again after a while";
                RedirectToAction("Update_Password", "Home");
            }

            return View();
        }


        public ActionResult Shop(int? Id)
        {   
            if(Id!=null)
            {
                return View(client.Get_Item_By_Category_Id(Convert.ToInt32(Id)));
           
            }else
            {
                return View(client.Get_All_Item());
            }
            
           
        }

        public ActionResult View_Single_Item(int? Id)
        {
            if(Id==null)
            {
                return RedirectToAction("Shop","Home");
            }else
            {

                return View(client.Get_Item_By_Id(Convert.ToInt32(Id)));
            }
        }

        public ActionResult Cart()
        {
            return View(client.Get_User_Cart(Convert.ToInt32(Session["User_Id"]))); 
        }

        [HttpPost]
        public ActionResult Add_To_Cart(FormCollection fc)
        {
            int Item_Id = Convert.ToInt32(fc["Item_Id"]);
            var Item = client.Get_Item_By_Id(Item_Id);

            Cart c = new Cart()
            {
                Item_Id = Item.Id,
                Image_Path = Item.Image_Path,
                Price = Item.Price,
                User_Id = Convert.ToInt32(Session["User_Id"]),
                Quantity = Convert.ToInt32(fc["Quantity"]),
                Name = Item.Name,
                Description = Item.Description
                
            };
            c.Image_Path.Replace('~', '/');

            if(client.Add_Item_To_Cart(c)==1)
            {
                TempData["status"] = "Item has been added";
                Session["Cart_Items_No"] = client.Number_Of_Items_In_User_Cart(c.User_Id);
                return RedirectToAction("Shop", "Home");
            }else
            {
                TempData["error"] = "There was some technical errors, item was not added";
                return RedirectToAction("View_Single_Item", "Home", new { Id = Item_Id });
            } 
        }

        public ActionResult Remove_Item_From_Cart(int Id)
        {
            if(client.Remove_Item_From_Cart(Id) == 1)
            {
                Session["Cart_Items_No"] = client.Number_Of_Items_In_User_Cart(Convert.ToInt32(Session["User_Id"]));
                TempData["status"] = "Item has been removed";
                return RedirectToAction("Cart", "Home");
            }
            else
            {
                return RedirectToAction("Cart", "Home");
            }
            
        }


        [HttpPost]
        public ActionResult Update_Cart(FormCollection fc)
        {
            int num = Convert.ToInt32(fc["index"]);
            List<Cart> Cart_List = new List<Cart>();
                 
            for(int i=0;i<=num;i++)
            {
                Cart c = new Cart()
                {
                    Id = Convert.ToInt32(fc["Cart_Id_"+i]),
                    Quantity = Convert.ToInt32(fc["Quantity_" + i]),
                    User_Id = Convert.ToInt32(Session["User_Id"])
                };
                Cart_List.Add(c);
            }

            if(client.Updaate_Cart(Cart_List.ToArray())==1)
            {
                TempData["status"] = "Cart has been updated";
                return RedirectToAction("Cart","Home");
            }else
            {
                TempData["error"] = "There was some technical errors, please try updating again";
                return RedirectToAction("Cart", "Home");
            }
        }

        [HttpGet]
        public ActionResult Checkout()
        {
            decimal Grand_Total = 0;
            decimal Sub_Total = 0;

            if(Request.QueryString["Grand_Total"]!=null && Request.QueryString["Sub_Total"]!=null)
            {
                Grand_Total = Convert.ToDecimal(Request.QueryString["Grand_Total"]);
                Sub_Total = Convert.ToDecimal(Request.QueryString["Sub_Total"]);
            }

            ViewBag.Grand_Total = Grand_Total;
            ViewBag.Sub_Total = Sub_Total;
            return View();
        }


        [HttpPost]
        public ActionResult Checkout_Post(FormCollection fc)
        {
            if(client.Make_Transaction(Convert.ToInt32(Session["User_Id"]))==1)
            {

                Session["Cart_Items_No"] = 0;
                TempData["status"] = "Order has been made";
                return View();
            }
            else
            {
                TempData["error"] = "There was some technical errors, transaction was not processed";
                return RedirectToAction("Cart", "Home");
            }         
        }

        [HttpGet]
        public ActionResult Track_Orders()
        {
            return View(client.Track_Orders(Convert.ToInt32(Session["User_Id"])));
        }

        [HttpGet]
        public ActionResult Transactions_History()
        {
            return View(client.Get_User_Trasnsaction_History(Convert.ToInt32(Session["User_Id"])));
        }

    }
}