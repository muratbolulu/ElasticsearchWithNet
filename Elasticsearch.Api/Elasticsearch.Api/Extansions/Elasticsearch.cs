using Elasticsearch.Net;
using Nest;

namespace Elasticsearch.Api.Extansions
{
    public static class ElasticsearchExt
    {
        public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
        {
            //program.cs kirletmemek için buraya taşıdık.
            var pool = new SingleNodeConnectionPool(new Uri(configuration.GetSection("Elastic")["Uri"]!)); // ! Uri olmayabilir diyordu, biz işaretledik var diye.
            var settings = new ConnectionSettings(pool);
            var client = new ElasticClient(settings);
            //elastic ve redis kendi dökümanlarında singletion belirtir. //efcore scoped. //elastic threadsafe dir. //dbcontext threadsafe değildir. farklı thread den okuyamazsın. 
            services.AddSingleton(client);
        }
    }
}
