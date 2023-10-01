using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model
{
    public class PlantModel
    {
        public int Id { get; set; }
        [Required]
        //[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name must contain only letters.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public string Status { set; get; }

        [Required(ErrorMessage = "ExternalId is required.")]
        [StringLength(20, ErrorMessage = "ExternalId cannot exceed 20 characters.")]
        public string ExternalId { set; get; }

        [Required(ErrorMessage = "CreateDate is required.")]
        public DateTime CreateDate { set; get; }

        [Required(ErrorMessage = "Height is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Height must be greater than 0.")]
        public int Height { set; get; }

        public string HabitantTypeName { set; get; }

        public string FieldName { set; get; }

        public string AreaName { set; get; }
        public string ZoneName { set; get; }
    }
}
