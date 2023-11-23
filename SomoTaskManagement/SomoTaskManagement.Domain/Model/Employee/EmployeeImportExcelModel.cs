using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Employee
{
    public class EmployeeImportExcelModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        public string Code { get; set; }
        [StringLength(10, ErrorMessage = "Phone number must be exactly 10 digits.", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only digits.")]
        public string PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }
        [Required]
        public string Gender { get; set; }
        public string ImageUrl { get; set; }
        public int FarmId { get; set; }
        public DateTime DateOfBirth { get; set; }

        public List<int> TaskTypeIds { get; set; }
    }
}
