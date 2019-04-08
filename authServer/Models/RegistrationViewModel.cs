using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class RegistrationViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [MinLength(4)]
        public string Username { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        [Required]
        [MinLength(8)]
        public string SecurityQuestion { get; set; }
        [Required]
        [MinLength(4)]
        public string SecurityAnswer { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
