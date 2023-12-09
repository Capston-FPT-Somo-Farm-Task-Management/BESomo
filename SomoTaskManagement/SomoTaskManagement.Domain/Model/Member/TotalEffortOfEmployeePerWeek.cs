using SomoTaskManagement.Domain.Model.Employee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Member
{
    public class TotalEffortOfEmployeePerWeek
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public string NameCode { get; set; }
        public string Code { get; set; }
        [Required]
        public string Status { set; get; }
        [StringLength(10, ErrorMessage = "Phone number must be exactly 10 digits.", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only digits.")]
        public string PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }
        public string Gender { set; get; }
        public int FarmId { get; set; }
        public string TaskTypeName { get; set; }
        public List<int> TaskTypeId { get; set; }
        public string Avatar { get; set; }
        public DateTime DateOfBirth { get; set; }

        public double TotalEffort {  get; set; }   
    }
}
