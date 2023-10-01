using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model
{
    public class MemberModel
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name must contain only letters.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public int Status { set; get; }
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { set; get; }

        [StringLength(100, ErrorMessage = "UserName cannot exceed 100 characters.")]
        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { set; get; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&+=!])(?!.*\s).{8,}$",
        ErrorMessage = "Password must meet the complexity requirements.")]
        public string Password { set; get; }

        [StringLength(10, ErrorMessage = "Phone number must be exactly 10 digits.", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only digits.")]
        public int PhoneNumber { set; get; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1900-01-01", "2005-01-01", ErrorMessage = "Please enter a valid date.")]
        [Required(ErrorMessage = "Birthday is required.")]
        public DateTime Birthday { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        [Required(ErrorMessage = "Address is required.")]
        public string Address { set; get; }
        public string RoleName { set; get; }
        public string FarmName { set; get; }
    }
}
