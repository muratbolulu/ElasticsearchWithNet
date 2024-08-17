using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.Api.Models.ECommerce;
using System.Collections.Immutable;

namespace Elasticsearch.Api.Repositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<ImmutableList<ECommerce>> TermQuery(string customerFirstName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(q => q.Term(t => t.Field("customer_first_name.keyword").Value(customerFirstName))));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //type safety  ?? CustomerFirstName neden hata alıyor, araştır.
        //public async Task<ImmutableList<ECommerce>> TermQueryTypeSafety(string customerFirstName)
        //{
        //    var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
        //    .Query(q => q.Term(t => t.CustomerFirstName.Suffix("keyword"), customerFirstName)));

        //    foreach (var hit in result.Hits)
        //        hit.Source.Id = hit.Id;

        //    return result.Documents.ToImmutableList();
        //}


        //type safety
        public async Task<ImmutableList<ECommerce>> TermQueryTypeSafety(string customerFirstName)
        {
            var termQuery = new TermQuery("customer_first_name.keyword")
            {
                Value = customerFirstName,
                CaseInsensitive = true //büyük küçük harf hassasiyeti olmasın.
            };

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //termsQuery
        public async Task<ImmutableList<ECommerce>> TermsQuery(List<string> customerFirstNameList)
        {
            List<FieldValue> terms = new List<FieldValue>();
            customerFirstNameList.ForEach(x =>
            {
                terms.Add(x);
            });

            var termsQuery = new TermsQuery()
            {
                Field = "customer_first_name.keyword",
                Term = new TermsQueryField(terms.AsReadOnly())
            };

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termsQuery));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //termsQuery type safety
        public async Task<ImmutableList<ECommerce>> TermsQueryTypeSafety(List<string> customerFirstNameList)
        {
            List<FieldValue> terms = new List<FieldValue>();
            customerFirstNameList.ForEach(x =>
            {
                terms.Add(x);
            });

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(100) //100 kayıt istendi. Default 10 kayıttır.
            .Query(q => q
            .Terms(t => t
            .Field(f => f.CustomerFirstName
            .Suffix("keyword"))
            .Term(new TermsQueryField(terms.AsReadOnly())))));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //ImmutableList kullanım amacı çekilen datanın değiştirilmesini engellemektir.
        public async Task<ImmutableList<ECommerce>> PrefixQuery(string customerFullName)
        {

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(q => q
            .Prefix(p => p
            .Field(f => f.CustomerFullName
            .Suffix("keyword"))
            .Value(customerFullName))));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> RangeQuery(double FromPrice, double ToPrice)
        {

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(q => q
                .Range(r => r
                    .NumberRange(nr => nr
                    .Field(f => f.TaxFulTotalPrice)
                        .Gte(FromPrice)
                            .Lte(ToPrice)))));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchAllQuery()
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(100)
            //.Query(q=>q.MatchAll())
            );

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PaginationQueryAsync(int page, int pageSize)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(pageSize).From((page - 1) * pageSize));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //? tek bir harf için sorgular
        //* birden fazla harfi sorgular.
        public async Task<ImmutableList<ECommerce>> WildCardQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                    .Query(q => q
                     .Wildcard(w => w
                        .Field(f => f.CustomerFullName.Suffix("keyword"))
                            .Wildcard(customerFullName))));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //FuzzyQuery + Sort
        public async Task<ImmutableList<ECommerce>> FuzzySortQueryAsync(string customerFirstName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                    .Query(q => q
                     .Fuzzy(f => f
                        .Field(fu => fu.CustomerFirstName.Suffix("keyword"))
                            .Value(customerFirstName).Fuzziness(new Fuzziness(2)))) //2 harf/rakamsal hataya karşı tedbirli
                                .Sort(s => s.Field(ff => ff.TaxFulTotalPrice, new FieldSort() { Order = SortOrder.Desc })));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchQueryFullTextAsync(string categoryName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName)
                .Size(1000)
                    .Query(q => q
                        .Match(m => m
                            .Field(f => f.Category)
                                .Query(categoryName).Operator(Operator.And))));
                                //.Query(categoryName).Operator(Operator.Or))));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchBoolPrefixAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName)
                .Size(1000)
                    .Query(q => q
                        .MatchBoolPrefix(m => m
                            .Field(f => f.CustomerFullName)
                                .Query(customerFullName))));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //araya giremez, kelimeyi öbek olarak (komple olarak) tüm data içerisinde arar.
        public async Task<ImmutableList<ECommerce>> MatchPhrasePrefixAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName)
                .Size(1000)
                    .Query(q => q
                        .MatchPhrasePrefix(m => m
                            .Field(f => f.CustomerFullName)
                                .Query(customerFullName))));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> CompoundQueryExampleOneAsync(string cityName,
                                double taxfulTotalPrice, string categoryName, string manufacturer)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(indexName)
                .Size(1000)
                  .Query(q => q
                      .Bool(b => b
                          .Must(m=>m
                              .Term(t=>t
                                  .Field("geoip.city_name"!)
                                    .Value(cityName)))
                          .MustNot(mn=>mn
                              .Range(r=>r
                                  .NumberRange(nr=>nr
                                     .Field(f=>f.TaxFulTotalPrice).Lte(taxfulTotalPrice))))
                          .Should(s=>s
                            .Term(t=>t
                                .Field(f=>f.Category
                                    .Suffix("keyword"))
                                        .Value(categoryName)))
                          .Filter(f=>f
                            .Term(t=>t
                                .Field("manufacturer.keyword"!)
                                    .Value(manufacturer))))));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> CompoundQueryExampleTwoAsync(string customerFullName)
        {
            //var result = await _client.SearchAsync<ECommerce>(s => s
            //.Index(indexName)
            //    .Size(1000)
            //      .Query(q => q
            //          .Bool(b => b
            //              .Should(s=>s  //aşağıdaki match ve ya prefix ten şartı sağlayanları should olduğundan getirir.
            //                .Match(ma=>ma //fulltextquery  -- 1e1 eşleşenleri getirir.
            //                   .Field(f=>f.CustomerFullName)
            //                   .Query(customerFullName))
            //                .Prefix(p=>p //prefix inde eşleşenleri getirir.
            //                  .Field(ff=>ff.CustomerFullName.Suffix("keyword"))  //term level query -- term level query ler full text üzerinden sorgulama yapmaz. keyword kullanılır.
            //                  .Value(customerFullName))))));

            //yukarıda sorgunun aynısındır.
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                    .Size(1000)
                        .Query(q => q
                            .MatchPhrasePrefix(m => m
                                .Field(f => f.CustomerFullName)
                                    .Query(customerFullName))));


            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        //ilgili 3 alanda arama yapar.
        public async Task<ImmutableList<ECommerce>> MultiMatchFullTextQuery(string name)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Size(100)
            .Query(q => q
                    .MultiMatch(mm => mm
                        .Fields(new Field("customer_first_name")
                           .And(new Field("customer_last_name"))
                           .And(new Field("customer_full_name")))
                           .Query(name))));

            foreach (var hit in result.Hits)
                hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }


    }
}
