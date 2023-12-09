using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Plant
{
    public class PlantCreateModel
    {

        [Required(ErrorMessage ="Tên bắt buộc nhập")]
        //[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name must contain only letters.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }



        [Required(ErrorMessage = "Mã cây trồng bắt buộc nhập")]
        [StringLength(20, ErrorMessage = "ExternalId cannot exceed 20 characters.")]
        public string ExternalId { set; get; }


        [Required(ErrorMessage = "Chiều cao bắt buộc nhập")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Chiều cao cây trồng phải lớn hơn 0")]
        public int Height { set; get; }

        public int HabitantTypeId { set; get; }

        public int FieldId { set; get; }

    }
}
