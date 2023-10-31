using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Zone
{
    public class ZoneModel
    {
        public int Id { get; set; }

        [Required]
        //[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name must contain only letters.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string AreaName { get; set; }
        public int AreaId { get; set; }
        [Required(ErrorMessage = "Area is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "FarmArea must be greater than 0.")]
        public double FarmArea { get; set; }
        public string ZoneTypeName { get; set; }
        public int ZoneTypeId { get; set; }
        public string NameCode { get; set; }
        public string Name { get; set; }

        public string Status { get; set; }
        public string Code { get; set; }
    }
}
