using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NetCalendar.WebUI.Models
{
    public class DateTimeD
    {
        public int id { get; set; }


        [Display(Name = "Price goods")]
        [Required(ErrorMessage = "Field Price must be value")]
        public decimal Price { get; set; }


        [Display(Name = "Date ....")]
        [Required(ErrorMessage = "Field Date must be value")]
        [UIHint("Date1")]
        public DateTime Date { get; set; }
    }

}