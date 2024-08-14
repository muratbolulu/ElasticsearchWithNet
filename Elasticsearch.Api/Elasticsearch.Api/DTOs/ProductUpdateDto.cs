using Elasticsearch.Api.Models;

namespace Elasticsearch.Api.DTOs
{
    public record ProductUpdateDto(string Id,string Name, decimal Price, int Stock, ProductFeatureDto Feature)
    {
        public Product UpdateProduct()
        {
            return new Product
            {
                Id = Id,
                Name = Name,
                Price = Price,
                Stock = Stock,
                Feature = new ProductFeature()
                {
                    Width = Feature.Width,
                    Height = Feature.Height,
                    Color = (EColor)int.Parse(Feature.Color)
                }
            };
        }
    }
}
