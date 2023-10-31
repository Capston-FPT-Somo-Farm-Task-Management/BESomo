using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.HabitantType
{
    public class HabitantTypeCUModel
    {
        [Required(ErrorMessage = "Name is required.")]
        //[RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }
        public string? Origin { get; set; }
        public string? Environment { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        [Required]
        public int Status { set; get; }
    }
}
