using System.Text.Json.Serialization;

namespace Elasticsearch.Web.Models.ViewModels
{
    public class ECommerceVM
    {
        public string Id { get; set; } = null!;
        public string CustomerFirstName { get; set; } = null!;
        public string CustomerLastName { get; set; } = null!;
        public string CustomerFullName { get; set; } = null!;
        public string Gender { get; set; }
        public string Category { get; set; } = null!;
        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        public double TaxFulTotalPrice { get; set; }
    }
}
