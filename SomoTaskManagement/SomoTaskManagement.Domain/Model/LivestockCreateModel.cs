using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model
{
    public class LivestockCreateModel
    {
        
        [Required]
        //[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name must contain only letters.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "ExternalId is required.")]
        [StringLength(20, ErrorMessage = "ExternalId cannot exceed 20 characters.")]
        public string ExternalId { set; get; }

        [Required(ErrorMessage = "Weight is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
        public int Weight { set; get; }

        [Required(ErrorMessage = "DateOfBirth is required.")]
        public DateTime DateOfBirth { set; get; }

        [Required(ErrorMessage = "Gender is required.")]
        public bool Gender { set; get; }

        public int HabitantTypeId { set; get; }

        public int FieldId { set; get; }

    }
}
