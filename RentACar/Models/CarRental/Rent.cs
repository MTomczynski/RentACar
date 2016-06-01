using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentACar.Models.CarRental
{
    public class Rent
    {
        public int RentId { get; set; }

        [DataType(DataType.Currency)]
        public decimal PriceToPay { get; set; }

        public int Days { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Rent date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateRent { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Return date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateReturn { get; set; }

        public bool DaysDiscount { get; set; }
        public bool RentsDiscount { get; set; }
        [DataType(DataType.Currency)]
        public decimal Discount { get; set; }
        public bool RentAccepted { get; set; }

        public virtual Car Car { get; set; }
        public int CarId { get; set; }

        public int RentManagerId { get; set; }
        public  RentManager RentManager { get; set; }
    }
}