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
        [Required(ErrorMessage = "Tên bắt buộc nhập")]
        //[RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 kí tự")]
        public string Name { get; set; }

        public string Code { get; set; }

        [Required(ErrorMessage = "Phải nhập diện tích")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "FarmArea must be greater than 0.")]
        public double Area { set; get; }

        public int ZoneId { set; get; }

    }
}
