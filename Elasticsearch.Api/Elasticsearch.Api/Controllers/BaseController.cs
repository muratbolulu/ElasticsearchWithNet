using Elasticsearch.Api.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Elasticsearch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        [NonAction] //dış dünyadan bağımsız, endpoint olmayan. Get olmayan.
        public IActionResult CreateActionResult<T>(ResponseDto<T> response)
        {
            if(response.Status==HttpStatusCode.NoContent)
            {
                return new ObjectResult(null) { StatusCode = response.Status.GetHashCode() }; // null olan alan reposne gövdedir.
            }

            return new ObjectResult(response) { StatusCode = response.Status.GetHashCode() };
        }

    }
}
