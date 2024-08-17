using Elastic.Clients.Elasticsearch;
using Elasticsearch.Api.DTOs;
using Elasticsearch.Api.Models;
using Elasticsearch.Api.Repositories;
using System;
using System.Collections.Immutable;
using System.Net;

namespace Elasticsearch.Api.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repository;
        private readonly ILogger<ProductService> _logger;   
        public ProductService(ProductRepository repository, ILogger<ProductService> logger) 
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResponseDto<ProductDto>> SaveASync(ProductCreateDto request)
        {
            var responseProduct = await _repository.SaveAsync(request.CreateProduct());

            if (responseProduct is null)
            {
                return ResponseDto<ProductDto>.Fail(new List<string> { "Kayıt esnasında hata meydana geldi." }, HttpStatusCode.InternalServerError);
            }

            return ResponseDto<ProductDto>.Success(responseProduct.CreateDto(), HttpStatusCode.Created);
        }

        public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            var productListDto = new List<ProductDto>();

            foreach (var x in products)
            {
                if (x.Feature is null)
                {
                    productListDto.Add(new ProductDto(x.Id, x.Name, x.Price, x.Stock, null));
                    continue;
                }
                productListDto.Add(new ProductDto(x.Id, x.Name, x.Price, x.Stock,
                        new(
                        x.Feature!.Width,
                        x.Feature!.Height,
                        x.Feature!.Color.ToString())
                        ));
            }

            return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode.OK);
        }


        public async Task<ResponseDto<ProductDto>> GetByIdAsync(string id)
        {
            var hasProduct = await _repository.GetByIdAsync(id);

            if (hasProduct is null)
            {
                return ResponseDto<ProductDto>.Fail("ürün bulunamadı", HttpStatusCode.OK);
            }

            return ResponseDto<ProductDto>.Success(hasProduct.CreateDto(), HttpStatusCode.OK);
        }

        public async Task<ResponseDto<bool>> UpdateASync(ProductUpdateDto request)
        {
            var isSuccess = await _repository.UpdateAsync(request);

            if (!isSuccess)
            {
                return ResponseDto<bool>.Fail(new List<string> { "Update esnasında hata meydana geldi." }, HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }

        public async Task<ResponseDto<bool>> DeleteASync(string id)
        {
            var deleteResponse= await _repository.DeleteAsync(id);

            if (!deleteResponse.IsValidResponse && deleteResponse.Result==Result.NotFound)
            {
                return ResponseDto<bool>.Fail(new List<string> { "Silmeye çalıştığınız ürün bulunamamıştır" }, HttpStatusCode.NotFound);
            }


            if (!deleteResponse.IsValidResponse)
            {
                deleteResponse.TryGetOriginalException(out Exception? exception);
                _logger.LogError(exception, deleteResponse.ElasticsearchServerError?.Error.ToString()); //for developer
                return ResponseDto<bool>.Fail(new List<string> { "Silme esnasında hata meydana geldi." }, HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }

    }
}
