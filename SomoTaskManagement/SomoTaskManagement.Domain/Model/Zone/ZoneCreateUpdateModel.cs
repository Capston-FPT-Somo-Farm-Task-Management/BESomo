using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Zone
{
    public class ZoneCreateUpdateModel
    {
        [Required(ErrorMessage = "Name is required.")]
        //[RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        [Required(ErrorMessage = "Area is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "FarmArea must be greater than 0.")]
        public double FarmArea { get; set; }

        public int ZoneTypeId { get; set; }

        public int AreaId { get; set; }
    }
}
