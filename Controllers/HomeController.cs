using Castle.Core.Internal;
using Shop_Management_System.Helper;
using Shop_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Shop_Management_System.Controllers
{
    public class HomeController : Controller
    {
        [AuthorizationFilter]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public JsonResult CheckLogin(string username, string password)
        {
            ShopManagementSystemEntities db = new ShopManagementSystemEntities();
            string md5StringPassword = AppHelper.GetMd5Hash(password);
            var dataItem = db.Users.Where(x => x.Username == username && x.Password == md5StringPassword).SingleOrDefault();
            bool isLogged = true;
            if (dataItem != null)
            {
                Session["Username"] = dataItem.Username;
                Session["Role"] = dataItem.Role;
                isLogged = true;
            }
            else
            {
                isLogged = false;
            }
            return Json(isLogged, JsonRequestBehavior.AllowGet);
        }

        [AuthorizationFilter]
        public ActionResult UserCreate()
        {
            ShopManagementSystemEntities db = new ShopManagementSystemEntities();
            var dataList = db.Users.Where(x => x.Status == 1).ToList();
            return View(dataList);
        }

        [HttpPost]
        public JsonResult SaveUser(User user)
        {
            ShopManagementSystemEntities db = new ShopManagementSystemEntities();
            bool isSuccess = true;

            if (user.UserId > 0)
            {
                db.Entry(user).State = EntityState.Modified;
            }
            else
            {
                user.Status = 1;
                user.Password = AppHelper.GetMd5Hash(user.Password);
                db.Users.Add(user);
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [AuthorizationFilter]
        [HttpGet]
        public ActionResult UserCreateEdit(int id)
        {
            using(ShopManagementSystemEntities db = new ShopManagementSystemEntities())
            {

                return View(db.Users.Where(x => x.UserId == id).FirstOrDefault());
            }
        }
        [HttpPost]
        public ActionResult UserCreateEdit(int id, User User)
        {
            try
            {
                using (ShopManagementSystemEntities db = new ShopManagementSystemEntities())
                {

                    db.Entry(User).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("UserCreate");
            }
            catch
            {
                return View();
            }
            
            
        }

        [AuthorizationFilter]

        public ActionResult UserCreateDelete(int id)
        {
            using (ShopManagementSystemEntities db = new ShopManagementSystemEntities())
            {

                return View(db.Users.Where(x => x.UserId == id).FirstOrDefault());
            }
        }
        [HttpPost]
        public ActionResult UserCreateDelete(int id, FormCollection  collection)
        {
            try
            {
                using (ShopManagementSystemEntities db = new ShopManagementSystemEntities())
                {

                   User user = db.Users.Where(x => x.UserId == id).FirstOrDefault();
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
                return RedirectToAction("UserCreate");
            }
            catch
            {
                return View();
            }


        }

        public ActionResult Logout()
        {
            Session["Username"] = null;
            Session["Role"] = null;
            return RedirectToAction("Login");
        }

        [AuthorizationFilter]
        public ActionResult Category()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveCategory(Category cat)
        {
            ShopManagementSystemEntities db = new ShopManagementSystemEntities();
            bool isSuccess = true;
            if (cat.CategoryId > 0)
            {
                db.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat.Status = 1;
                db.Categories.Add(cat);
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllGetegory()
        {
            ShopManagementSystemEntities db = new ShopManagementSystemEntities();
            var dataList = db.Categories.Where(x => x.Status == 1).ToList();
            var data = dataList.Select(x => new {
                CategoryId = x.CategoryId,
                Name = x.Name,
                Status = x.Status
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AuthorizationFilter]
        public ActionResult Product()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveProduct(Product product)
        {
            ShopManagementSystemEntities db = new ShopManagementSystemEntities();
            bool isSuccess = true;

            if (product.ProductId > 0)
            {
                db.Entry(product).State = EntityState.Modified;
            }
            else
            {
                product.CreatedDate = DateTime.Now;
                db.Products.Add(product);
            }
            try
            {
                
                db.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllProduct()
        {
            ShopManagementSystemEntities db = new ShopManagementSystemEntities();
            var dataList = db.Products.Include("Category").ToList();
            var modefiedData = dataList.Select(x => new
            {
                ProductId = x.ProductId,
                CategoryId = x.CategoryId,
                ProductName = x.ProductName,
                BatchId = x.BatchId,
                Price = x.Price,
                Status = x.Status,
                CreateDate = x.CreatedDate.ToString(),
                CategoryName = x.Category.Name
            }).ToList();
            return Json(modefiedData, JsonRequestBehavior.AllowGet);
        }

        [AuthorizationFilter]
        public ActionResult Batch()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveBatch(Batch batch)
        {
            Shop_Management_System.Helper.AppHelper.ReturnMessage retMessage = new AppHelper.ReturnMessage();
            ShopManagementSystemEntities db = new ShopManagementSystemEntities();
            retMessage.IsSuccess = true;

            if (batch.BatchId > 0)
            {
                db.Entry(batch).State = EntityState.Modified;
                retMessage.Messagae = "Update Success!";
            }
            else
            {
                batch.BatchName = batch.BatchName + db.Batches.Count();
                var batchData = db.Batches.Where(x => x.BatchName.Equals(batch.BatchName)).SingleOrDefault();
                if (batchData == null)
                {
                    db.Batches.Add(batch);
                    retMessage.Messagae = "Save Success!";
                }
                else
                {
                    retMessage.IsSuccess = false;
                    retMessage.Messagae = "This batch already exist!Please refresh and again try!";
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                retMessage.IsSuccess = false;
            }

            return Json(retMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllBatch()
        {
            ShopManagementSystemEntities db = new ShopManagementSystemEntities();
            var dataList = db.Batches.ToList();
            var modefiedData = dataList.Select(x => new
            {
                BatchId = x.BatchId,
                BatchName = x.BatchName,
            }).ToList();
            return Json(modefiedData, JsonRequestBehavior.AllowGet);
        }

        [AuthorizationFilter]
        public ActionResult ProductStock()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveProductStock(ProductStock stock)
        {
            Shop_Management_System.Helper.AppHelper.ReturnMessage retMessage = new AppHelper.ReturnMessage();
            ShopManagementSystemEntities db = new ShopManagementSystemEntities();
            retMessage.IsSuccess = true;

            if (stock.ProductQtyId > 0)
            {
                db.Entry(stock).State = EntityState.Modified;
                retMessage.Messagae = "Update Success!";
            }
            else
            {
                db.ProductStocks.Add(stock);
                retMessage.Messagae = "Save Success!";
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                retMessage.IsSuccess = false;
            }

            return Json(retMessage, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllProductStocks()
        {
            ShopManagementSystemEntities db = new ShopManagementSystemEntities();
            var dataList = db.ProductStocks.Include("Product").Include("Batch").ToList();
            var modefiedData = dataList.Select(x => new
            {
                ProductQtyId = x.ProductQtyId,
                ProductId = x.ProductId,
                ProductName = x.Product.ProductName,
                Quantity = x.Quantity,
                BatchId = x.BatchId,
                BatchName = x.Batch.BatchName,
                PurchasePrice = x.PurchasePrice,
                SalesPrice = x.SalesPrice
            }).ToList();
            return Json(modefiedData, JsonRequestBehavior.AllowGet);
        }

        [AuthorizationFilter]
        public ActionResult AllProductTable()
        {
            return View();
        }

    }
}