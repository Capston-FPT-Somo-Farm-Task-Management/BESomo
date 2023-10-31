using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Field
{
    public class FieldModel
    {
        public int Id { get; set; }
        [Required]
        //[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name must contain only letters.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string NameCode{ get; set; }
        public string Name{ get; set; }

        [Required]
        public string Status { set; get; }
        [Required(ErrorMessage = "Area is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "FarmArea must be greater than 0.")]
        public double Area { set; get; }
        public string Code { set; get; }
        public string ZoneName { set; get; }
        public int ZoneId { get; set; }
        public string AreaName { set; get; }
        public int AreaId { set; get; }
        public bool IsDelete { set; get; }
    }
}
