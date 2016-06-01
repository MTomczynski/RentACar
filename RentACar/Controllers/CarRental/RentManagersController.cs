using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RentACar.Models;
using RentACar.Models.CarRental;

namespace RentACar.Controllers.CarRental
{
    [Authorize]
    public class RentManagersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RentManagers/Details/5
        public ActionResult Details()
        {
            var userId = User.Identity.GetUserId();
            var rentManager = db.RentManagers.First(c => c.ApplicationUserId == userId);
            return View(rentManager);
        }
        
        public ActionResult Rented()
        {
            var userId = User.Identity.GetUserId();
            var rentManagerId = db.RentManagers.First(c => c.ApplicationUserId == userId).RentManagerId;
            var rentManager = db.Rents.Where(c => c.RentManagerId == rentManagerId);
            return View(rentManager.ToList());
        }
    }
}
