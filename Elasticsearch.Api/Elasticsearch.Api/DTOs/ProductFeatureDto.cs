using Elasticsearch.Api.Models;

namespace Elasticsearch.Api.DTOs
{
    public record ProductFeatureDto(int Width, int Height, string Color)
    {
    }
}
