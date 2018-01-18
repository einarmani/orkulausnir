using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Orkulausnir.Models
{
    public class FileUpload
    {
        [Required]
        [Display(Name = "Title")]
        [StringLength(60, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Public Schedule")]
        public IFormFile File { get; set; }
    }
}