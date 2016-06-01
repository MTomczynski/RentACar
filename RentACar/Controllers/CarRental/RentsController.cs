using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RentACar.Models;
using RentACar.Models.CarRental;
using RentACar.Services;

namespace RentACar.Controllers.CarRental
{
    public class RentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        PaymentService _payment = new PaymentService();

        [Authorize(Roles = "Admin")]
        // GET: Rents
        public ActionResult Index()
        {
            var rents = db.Rents.Include(r => r.Car).Include(r => r.RentManager);
            return View(rents.ToList());
        }

        // GET: Rents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rent rent = db.Rents.Find(id);
            if (rent == null)
            {
                return HttpNotFound();
            }
            return View(rent);
        }

        // GET: Rents/Create
        [Authorize]
        public ActionResult Create(int carId)
        {
            var date = DateTime.Now.ToShortDateString();
            ViewBag.DateRent = date;
            return View();
        }

        // POST: Rents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RentId,PriceToPay,Days,DateRent,DateReturn,DaysDiscount,RentsDiscount,Discount,RentAccepted,CarId,RentManagerId")] Rent rent)
        {
            //var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var rentManagerId = db.RentManagers.First(c => c.ApplicationUserId == userId).RentManagerId;
                var car = db.Cars.First(c => c.CarId == rent.CarId);
                
                DateTime date = DateTime.Now;
                rent.DateRent = date;
                TimeSpan days = rent.DateReturn - rent.DateRent;

                car.IsReserved = true;
                rent.Days = days.Days + 1;
                
                rent.RentManagerId = rentManagerId;

                if (rent.DateReturn <= rent.DateRent)
                {
                    return View("InvalidDate");
                }

                db.Rents.Add(rent);
                db.SaveChanges();

                var service = new RentManagerService(db);
                decimal price = service.Price(days.Days + 1, car.PricePerDay);
                service.RentsDiscountHandler(days.Days + 1, rent.RentId, rent.RentManagerId, price);
                return RedirectToAction("RedirectToPayment", new { id = rent.RentId });
            }
            return View("Index");
        }

        [Authorize(Roles = "Admin")]
        // GET: Rents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rent rent = db.Rents.Find(id);
            if (rent == null)
            {
                return HttpNotFound();
            }
            ViewBag.CarId = new SelectList(db.Cars, "CarId", "Name", rent.CarId);
            ViewBag.RentManagerId = new SelectList(db.RentManagers, "RentManagerId", "UserFirstName", rent.RentManagerId);
            return View(rent);
        }

        // POST: Rents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RentId,PriceToPay,Days,DateRent,DateReturn,DaysDiscount,RentsDiscount,Discount,RentAccepted,CarId,RentManagerId")] Rent rent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CarId = new SelectList(db.Cars, "CarId", "Name", rent.CarId);
            ViewBag.RentManagerId = new SelectList(db.RentManagers, "RentManagerId", "UserFirstName", rent.RentManagerId);
            return View(rent);
        }

        [Authorize(Roles = "Admin")]
        // GET: Rents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rent rent = db.Rents.Find(id);
            if (rent == null)
            {
                return HttpNotFound();
            }
            return View(rent);
        }

        [Authorize(Roles = "Admin")]
        // POST: Rents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rent rent = db.Rents.Find(id);
            db.Rents.Remove(rent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult RedirectToPayment(int id)
        {
            Rent rent = db.Rents.Find(id);
            ViewBag.Price = rent.PriceToPay;
            ViewBag.Days = rent.Days;
            ViewBag.Discount = rent.Discount;
            if (rent.RentsDiscount)
            {
                ViewBag.DiscountMsg = "Congratulations, you had more than 3 rents in last month, we are giving you real nice 20% discount";
            }
            if (rent.DaysDiscount)
            {
                ViewBag.DiscountMsg = "Congratulations, your rental is longer than a week, we are giving you nice 10% discount";
            }
            return View(rent);
        }

        [HttpPost]
        public ActionResult RedirectToPayment(Rent rents)
        {
            var rent = db.Rents.Where(c => c.RentId == rents.RentId).First();
            var service = new RentManagerService(db);
            if (!service.ReservationTimer(rent.DateRent))
            {
                var car = db.Cars.FirstOrDefault(c => c.CarId == rent.CarId);
                car.IsReserved = false;
                car.IsRented = false;
                rent.RentAccepted = false;
                db.SaveChanges();
                return View("ReservationTimerErr");
            } 
            return RedirectToAction("Payment", new {id = rent.RentId});

        }

        // GET: Payment
        [Authorize]
        public ActionResult Payment(int id)
        {
            Rent rent = db.Rents.Find(id);
            var userId = User.Identity.GetUserId();
            var rentManager = db.RentManagers.First(c => c.ApplicationUserId == userId);

            string url = _payment.CreateUrl(rent.PriceToPay, rent.CarId.ToString(), rent.RentId.ToString(), rentManager.UserFirstName, rentManager.UserLastName, rentManager.User.Email);

            return Redirect(url);
        }

        //POST 
        [HttpPost]
        public ActionResult DotpayConfirm()
        {
            string id = Request["id"];
            string operationNumber = Request["operation_number"];
            string operationType = Request["operation_type"];
            string operationStatus = Request["operation_status"];
            string operationAmount = Request["operation_amount"];
            string operationCurrency = Request["operation_currency"];
            string operationWithdrawalAmount = Request["operation_withdrawal_amount"];
            string operationCommissionAmount = Request["operation_commission_amount"];
            string operationOriginalAmount = Request["operation_original_amount"];
            string operationOriginalCurrency = Request["operation_original_currency"];
            string operationDatetime = Request["operation_datetime"];
            string operationRelatedNumber = Request["operation_related_number"];
            string control = Request["control"];
            string description = Request["description"];
            string email = Request["email"];
            string pInfo = Request["p_info"];
            string pEmail = Request["p_email"];
            string channel = Request["channel"];
            string channelCountry = Request["channel_country"];
            string geoipCountry = Request["geoip_country"];
            string signature = Request["signature"];
            string dotpayPin = _payment.DotpayPin;

            string allParameters = dotpayPin + id + operationNumber + operationType + operationStatus + operationAmount +
                                   operationCurrency + operationWithdrawalAmount + operationCommissionAmount
                                   + operationOriginalAmount + operationOriginalCurrency + operationDatetime +
                                   operationRelatedNumber + control + description + email + pInfo + pEmail + channel +
                                   channelCountry + geoipCountry;
            int controlAsInt = Int32.Parse(control);
            Rent rent = db.Rents.Find(controlAsInt);
            var car = db.Cars.First(c => c.CarId == rent.CarId);

            if (_payment.CheckResponseFromDotpay(signature, allParameters))
            {
                if (operationStatus == "rejected")
                {
                    car.IsReserved = false;
                    car.IsRented = false;
                    rent.RentAccepted = false;
                    db.SaveChanges();
                }
                if (operationStatus == "completed")
                {
                    car.IsReserved = false;
                    car.IsRented = true;
                    rent.RentAccepted = true;
                    db.SaveChanges();
                }
                return new HttpStatusCodeResult(200, "OK");
            }
            else
            {
                return new HttpStatusCodeResult(403, "This is not a valid request from dotpay");
            }


        }

        public ActionResult PaymentThanks()
        {
            ViewBag.Message = "Thank you for your payment, check your rented cars to see status of the payment";
            return View("PaymentThanks");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
