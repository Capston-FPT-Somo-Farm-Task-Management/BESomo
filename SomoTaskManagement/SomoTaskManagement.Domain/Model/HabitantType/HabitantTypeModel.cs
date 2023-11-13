using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.HabitantType
{
    public class HabitantTypeModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public string Status { set; get; }
        public bool IsActive { get; set; }
        public int Quantity { set; get; }
        public int FarmId { get; set; }
        public string? Origin { get; set; }
        public string? Environment { get; set; }
        public string? Description { get; set; }
    }
}
