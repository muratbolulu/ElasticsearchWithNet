using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.Api.DTOs;
using Elasticsearch.Api.Models;
using System.Collections.Immutable;
 
namespace Elasticsearch.Api.Repositories
{
    public class ProductRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "products";

        public ProductRepository(ElasticsearchClient client)
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
            if (!response.IsSuccess()) return null;

            newProduct.Id = response.Id;

            return newProduct;

        }


        public async Task<ImmutableList<Product>> GetAllAsync()
        {
            var result = await _client.SearchAsync<Product>(s => s.Index(indexName)); // matchAll hata verdi, kontrol et burayı?? 
                            //.Query(q => q.MatchAll()));


            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            var result = await _client.GetAsync<Product>(id, s => s.Index(indexName));

            if (!result.IsSuccess())
            {
                return null;
            }

            result.Source.Id = result.Id;

            return result.Source;
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto updateProduct)
        {
            //var result = await _client.UpdateAsync<Product, ProductUpdateDto>(updateProduct.Id, x => x.Index(indexName).Doc(updateProduct));
            var result = await _client.UpdateAsync<Product, ProductUpdateDto>(indexName,updateProduct.Id, x => x.Doc(updateProduct));
            
            return result.IsSuccess();
        }

        public async Task<DeleteResponse> DeleteAsync(string id)
        {
            var result = await _client.DeleteAsync<Product>(id, x => x.Index(indexName));

            return result;
        }
    }
}
