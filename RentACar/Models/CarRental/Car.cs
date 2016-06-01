using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentACar.Models.CarRental
{
    public class Car
    {
        public int CarId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Price per day")]
        public decimal PricePerDay { get; set; }

        public bool IsReserved { get; set; }
        public bool IsRented { get; set; }

        public virtual ICollection<Rent> Rents { get; set; }
    }
}