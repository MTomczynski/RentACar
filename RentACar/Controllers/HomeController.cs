using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RentACar.Models;

namespace RentACar.Controllers
{
    
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var cars = db.Cars.Where(c => c.IsRented == false).Where(c => c.IsReserved == false);

            return View(cars.ToList());
        }

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Send us a message";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(string message)
        {
            ViewBag.Message = "Thanks, we got your message!";

            return PartialView("_ContactThanks");
        }
    }
}