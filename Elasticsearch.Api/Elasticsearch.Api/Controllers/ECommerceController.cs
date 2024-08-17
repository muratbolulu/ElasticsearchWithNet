using Elasticsearch.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ECommerceController : ControllerBase
    {
        private readonly ECommerceRepository _repository;

        public ECommerceController(ECommerceRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("customerFirstName")]
        public async Task<IActionResult> TermQueryAsync(string customerFirstName)
        {
            return Ok(await _repository.TermQuery(customerFirstName));
        }

        [HttpPost("TermsQuery")]
        public async Task<IActionResult> TermsQueryAsync(List<string> customerFirstNameList)
        {
            return Ok(await _repository.TermsQuery(customerFirstNameList));
        }

        [HttpPost("TermsQueryTypeSafety")]
        public async Task<IActionResult> TermsQueryTypeSafetyAsync(List<string> customerFirstNameList)
        {
            return Ok(await _repository.TermsQueryTypeSafety(customerFirstNameList));
        }

        [HttpGet("PrefixQuery")]
        public async Task<IActionResult> PrefixQueryAsync(string customerFullName)
        {
            return Ok(await _repository.PrefixQuery(customerFullName));
        }

        [HttpGet("RangeQuery")]
        public async Task<IActionResult> RangeQueryAsync(double fromPrice, double toPrice)
        {
            return Ok(await _repository.RangeQuery(fromPrice, toPrice));
        }

        [HttpGet("MatchAllQuery")]
        public async Task<IActionResult> MatchAllQueryAsync()
        {
            return Ok(await _repository.MatchAllQuery());
        }

        [HttpGet("WildCardQuery")]
        public async Task<IActionResult> WildCardQueryAsync(string customerFullName) //default 1 ve 10 olarak ayarlandı.
        {
            return Ok(await _repository.WildCardQueryAsync(customerFullName));
        }

        [HttpGet("FuzzySortQuery")]
        public async Task<IActionResult> FuzzyQueryAsync(string customerFirstName) 
        {
            return Ok(await _repository.FuzzySortQueryAsync(customerFirstName));
        }

        [HttpGet("MatchQueryFullText")]
        public async Task<IActionResult> MatchQueryFullTextAsync(string categoryName)
        {
            return Ok(await _repository.MatchQueryFullTextAsync(categoryName));
        }

        [HttpGet("MatchBoolPrefixAsync")]
        public async Task<IActionResult> MatchBoolPrefixAsyncAsync(string customerFullName)
        {
            return Ok(await _repository.MatchBoolPrefixAsync(customerFullName));
        }

        [HttpGet("MatchPhrasePrefixAsync")]
        public async Task<IActionResult> MatchPhrasePrefixAsync(string customerFullName)
        {
            return Ok(await _repository.MatchBoolPrefixAsync(customerFullName));
        }

        [HttpGet("CompoundQueryExampleOneAsync")]
        public async Task<IActionResult> CompoundQueryExampleOneAsync(string cityName,
                                double taxfulTotalPrice, string categoryName, string manufacturer)
        {
            return Ok(await _repository.CompoundQueryExampleOneAsync(cityName, taxfulTotalPrice, categoryName, manufacturer));
        }

        [HttpGet("CompoundQueryExampleTwoAsync")]
        public async Task<IActionResult> CompoundQueryExampleTwoAsync(string customerFullName)
        {
            return Ok(await _repository.CompoundQueryExampleTwoAsync(customerFullName));
        }


        [HttpGet("MultiMatchFullTextQueryAsync")]
        public async Task<IActionResult> MultiMatchFullTextQueryAsync(string name)
        {
            return Ok(await _repository.MultiMatchFullTextQuery(name));
        }

    }
}
