using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBrokerApp.Application.DTO
{
    public class UserRegisterDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public  string Password { get; set; }

        [Required]
        public bool IsBroker { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }
}
