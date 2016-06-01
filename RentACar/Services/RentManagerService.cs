using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RentACar.Models;
using RentACar.Models.CarRental;

namespace RentACar.Services
{
    public class RentManagerService
    {
        private ApplicationDbContext db;

        public RentManagerService(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public void CreateRentManager(string firstName, string lastName, string userId)
        {
            var rentManager = new RentManager { UserFirstName = firstName, UserLastName = lastName, ApplicationUserId = userId };
            db.RentManagers.Add(rentManager);
            db.SaveChanges();
        }

        public decimal Price(int days, decimal pricePerDay)
        {
            return days*pricePerDay;
        }

        //Handler for finishing discount rent action
        public void RentsDiscountHandler(int days, int rentId, int rentManagerId, decimal price)
        {
            bool rentsDiscount = false;
            bool daysDiscount = false;

            var rent = db.Rents.First(c => c.RentId == rentId);

            var baselineDate = DateTime.Now.AddMonths(-1);

            var rentManager = db.Rents.Where(c => c.RentManagerId == rentManagerId)
                .Count(c => c.DateRent > baselineDate);
       
            if (days > 7) daysDiscount = true;
            if (rentManager > 3) rentsDiscount = true;

            //check what discount applies
            if (rentsDiscount && daysDiscount)
            {
                rent.Discount = price * (decimal)0.2;
                price = price - (price * (decimal)0.2);
                rent.RentsDiscount = true;
                rent.DaysDiscount = false;   
            }
            if (!rentsDiscount && daysDiscount)
            {
                rent.Discount = price * (decimal)0.2;
                price = price - (price * (decimal)0.1);
                rent.RentsDiscount = false;
                rent.DaysDiscount = true;  
            }
            if (rentsDiscount && !daysDiscount)
            {
                rent.Discount = price * (decimal)0.2;
                price = price - (price * (decimal)0.2);
                rent.RentsDiscount = true;
                rent.DaysDiscount = false;
            }

            rent.PriceToPay = price;
            db.SaveChanges();
        }



        //Logic for reservation for 3 min, after this time database record is cleared
        public bool ReservationTimer(DateTime dateRent)
        {
            var baselineTime = DateTime.Now;
            TimeSpan timeDiff = baselineTime - dateRent;
            if ( timeDiff.Minutes > 3 )
            {
                return false;
            }
            return true;
        }

/*        public void AvailabilityCheck()
        {
            var baselineTime = DateTime.Now;
            var rents = db.Rents.Where(c => c.DateRent < baselineTime);
            var cars = db.Cars.Where(c => c.CarId == )
        }*/
    }
}