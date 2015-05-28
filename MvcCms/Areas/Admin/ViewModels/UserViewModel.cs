using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCms.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string DisplayName { get; set; }
        public string CurrentPassword { get; set; }
        [Compare("ConfirmPassword", ErrorMessage = "The passwords must match")]
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
