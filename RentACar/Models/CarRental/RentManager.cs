using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentACar.Models.CarRental
{
    public class RentManager
    {
        public int RentManagerId { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string UserFirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string UserLastName { get; set; }

        public string Name
        {
            get { return string.Format("{0} {1}", this.UserFirstName, this.UserLastName); }
        }

        public ICollection<Rent> Rents { get; set; }

        public virtual ApplicationUser User { get; set; }
        public string ApplicationUserId { get; set; }
    }
}