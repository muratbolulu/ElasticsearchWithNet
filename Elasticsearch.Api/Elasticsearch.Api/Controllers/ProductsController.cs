using Elasticsearch.Api.DTOs;
using Elasticsearch.Api.Models;
using Elasticsearch.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace Elasticsearch.Api.Controllers
{
    //[Route("api/[controller]")]   //artık basecontroller dan gelir.
    //[ApiController]   //artık basecontroller dan gelir.
    public class ProductsController : BaseController
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductCreateDto request)
        {
            return CreateActionResult(await _productService.SaveASync(request));
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProductUpdateDto request)
        {
            return CreateActionResult(await _productService.UpdateASync(request));
        }


        [HttpDelete("id")]
        public async Task<IActionResult> Delete(string id)
        {
            return CreateActionResult(await _productService.DeleteASync(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return CreateActionResult(await _productService.GetAllAsync());
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            return CreateActionResult(await _productService.GetByIdAsync(id));
        }
    }
}
