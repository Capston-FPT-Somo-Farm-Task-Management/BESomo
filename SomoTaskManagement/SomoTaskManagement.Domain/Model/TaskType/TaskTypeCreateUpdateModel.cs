using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.TaskType
{
    public class TaskTypeCreateUpdateModel
    {

        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public int Status { set; get; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { set; get; }
    }
}
