using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.Web.Models;
using Elasticsearch.Web.Models.ViewModels;

namespace Elasticsearch.Web.Repositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        //tuple ile geriye 2 değer dönderdik.
        //biri arana değerler listesi, (page ve pagesize olduğundan aranan kadar) diğer ise toplam aramaya uyan count kadar.
        public async Task<(List<ECommerce>list, long count)> SearchAsync(ECommerceSearchVM searchVM, int page, int pageSize)
        {
            //Delegetes özünde metodları işaret eder ama geriye bir şey dönmez.
            List<Action<QueryDescriptor<ECommerce>>> listQuery = new();

            if (searchVM is null)
            {
                //ilk açılışta boş olduğundan tüm datayı getir.
                listQuery.Add(g => g.MatchAll(new MatchAllQuery()));

                return await CalculateResultSet(page, pageSize,listQuery);
            }

            if (!string.IsNullOrEmpty(searchVM.Category))
            {
                listQuery.Add((q) => q
                    .Match(m => m //fulltextquery  -- 1e1 eşleşenleri getirir.
                        .Field(f => f.Category)
                        .Query(searchVM.Category)));
            }


            if (!string.IsNullOrEmpty(searchVM.CustomerFullName))
            {
                listQuery.Add((q) => q
                    .Match(m => m //fulltextquery  -- 1e1 eşleşenleri getirir.
                        .Field(f => f.CustomerFullName)
                        .Query(searchVM.CustomerFullName)));
            }

            if (searchVM.OrderStartDate.HasValue) //değeri varsa (hasValue)
            {
                listQuery.Add((q) => q
                .Range(r => r
                    .DateRange(dr => dr
                    .Field(f => f.OrderDate)
                        .Gte(searchVM.OrderStartDate.Value))));
            }


            if (searchVM.OrderEndDate.HasValue) //değeri varsa (hasValue)
            {
                listQuery.Add((q) => q
                .Range(r => r
                    .DateRange(dr => dr
                    .Field(f => f.OrderDate)
                        .Lte(searchVM.OrderEndDate.Value))));
            }

            if (!string.IsNullOrEmpty(searchVM.Gender))
            {
                listQuery.Add((q) => q
                    .Term(t => t
                        .Field(f => f.Gender).Value(searchVM.Gender).CaseInsensitive()));
            }

            if (!listQuery.Any())
            {//sorgulamak için yukarıdaki hiçbir şart sağlanmadı ise tüm datayı getir.
                listQuery.Add(g => g.MatchAll(new MatchAllQuery()));
            }

            return await CalculateResultSet(page, pageSize, listQuery);
        }

        private async Task<(List<ECommerce> list, long count)> CalculateResultSet(int page, int pageSize, List<Action<QueryDescriptor<ECommerce>>> listQuery)
        {
            var pageFrom = (page - 1) * pageSize;

            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName)
                .Size(pageSize).From(pageFrom)
                  .Query(q => q
                      .Bool(b => b
                          .Must(listQuery.ToArray()
                          ))));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return (list: result.Documents.ToList(), result.Total);
        }
    }
}
