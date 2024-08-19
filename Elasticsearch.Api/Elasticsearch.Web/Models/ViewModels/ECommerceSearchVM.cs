using System.ComponentModel.DataAnnotations;

namespace Elasticsearch.Web.Models.ViewModels
{
    public class ECommerceSearchVM
    {
        [Display(Name ="Category")]
        public string? Category { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Order Date (Start)")]
        [DataType(DataType.Date)]
        public DateTime? OrderStartDate { get; set; }

        [Display(Name = "Order Date (End)")]
        [DataType(DataType.Date)]
        public DateTime? OrderEndDate { get; set; }

        [Display(Name = "Customer Full Name")]
        public string? CustomerFullName { get; set; }
    }
}
