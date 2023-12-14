using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Farm
{
    public class FarmCreateUpdateModel
    {
        [Required(ErrorMessage = "Name is required.")]
        //[RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }
        public double FarmArea { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }

        public string Description { get; set; }
        [FromForm]
        public IFormFile? ImageFile { get; set; }
    }
}
