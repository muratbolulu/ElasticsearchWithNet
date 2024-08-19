using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Elasticsearch.Web.Models.ViewModels
{
    public class BlogCreateVM
    {
        [Required]
        [Display(Name =" Blog Title")]
        public string Title { get; set; } = null!;
        [Required]
        [Display(Name = " Blog Content")]
        public string Content { get; set; } = null!;
        [Display(Name = " Blog Tags")]
        public string Tags { get; set; }=null!;
        public Guid UserId { get; set; }
    }
}
