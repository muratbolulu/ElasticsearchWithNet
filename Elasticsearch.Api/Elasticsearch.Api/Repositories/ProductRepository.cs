using Elasticsearch.Api.DTOs;
using Elasticsearch.Api.Models;
using Nest;
using System.Collections.Immutable;

namespace Elasticsearch.Api.Repositories
{
    public class ProductRepository
    {
        private readonly ElasticClient _client;
        private const string indexName = "products";

        public ProductRepository(ElasticClient client)
        {
            _client = client;
        }

        public async Task<Product?> SaveAsync(Product newProduct)
        {
            newProduct.Created = DateTime.Now;

            //indexlemek datayı kaydetmek demektir o yüzden SaveAsync metodu yok.
            var response = await _client.IndexAsync(newProduct, x => x.Index("products").Id(Guid.NewGuid().ToString()));
            //var response = await _client.IndexAsync(newProduct, x => x.Index("indexName"));

            // client code fast fail. good.
            if (!response.IsValid) return null;

            newProduct.Id = response.Id;

            return newProduct;

        }


        public async Task<ImmutableList<Product>> GetAllAsync()
        {
            var result = await _client.SearchAsync<Product>(s => s.Index(indexName)
                            .Query(q => q.MatchAll()));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            var result = await _client.GetAsync<Product>(id, s => s.Index(indexName));

            if (!result.IsValid)
            {
                return null;
            }

            result.Source.Id = result.Id;

            return result.Source;
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var result = await _client.UpdateAsync<Product, ProductUpdateDto>(updateProduct.Id, x => x.Index(indexName).Doc(updateProduct));
            
            return result.IsValid;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _client.DeleteAsync<Product>(id, x => x.Index(indexName));

            return result.IsValid;
        }
    }
}
