using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Field
{
    public class FieldCreateUpdateModel
    {
        [Required(ErrorMessage = "Name is required.")]
        //[RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public int Status { set; get; }
        public string Code { get; set; }

        [Required(ErrorMessage = "Area is required.")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "FarmArea must be greater than 0.")]
        public double Area { set; get; }

        public int ZoneId { set; get; }

    }
}
