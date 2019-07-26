using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BookingMachine.Models;

namespace BookingMachine.ViewModels
{
    public class BookingViewModel
    {
        [Required]
        [Display(Name = "Amount of places")]
        public int AmountOfPlaces { get; set; }
        [Display(Name = "Room")]
        public Room Room { get; set; }
    }
}