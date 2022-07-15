using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace idCard.Models
{
    public class FilesModel
    {
        public IFormFile files { get; set; }
    }
}
