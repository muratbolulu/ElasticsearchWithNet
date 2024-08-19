using Elasticsearch.Web.Models.ViewModels;
using Elasticsearch.Web.Repositories;

namespace Elasticsearch.Web.Services
{
    public class ECommerceService
    {
        private readonly ECommerceRepository _repository;

        public ECommerceService(ECommerceRepository repository)
        {
            _repository = repository;
        }

        public async Task<(List<ECommerceVM> list, long totalCount, long pageLinkCount)> SearchAsync(ECommerceSearchVM eCommerceSearchVM, int page, int pageSize)
        {
            var (eCommerceList, totalCount) = await _repository.SearchAsync(eCommerceSearchVM, page, pageSize);

            var pageLinkCountCalculate = totalCount % pageSize; //modu alınır, sayfa sayısı bulunur.
            long pageLinkCount = 0;

            if (pageLinkCount == 0)
            {
                pageLinkCount = totalCount / pageSize;
            }
            else
            {
                pageLinkCount = (totalCount / pageSize) + 1;
            }

            var eCommerceListVM = eCommerceList.Select(e => new ECommerceVM()
            {
                Category = String.Join(",", e.Category),
                CustomerFullName = e.CustomerFullName,
                CustomerFirstName = e.CustomerFirstName,
                CustomerLastName = e.CustomerLastName,
                OrderDate = e.OrderDate.ToShortDateString(),
                Gender = e.Gender.ToLower(),
                Id = e.Id,
                OrderId = e.OrderId,
                TaxFulTotalPrice = e.TaxFulTotalPrice
            }).ToList();

            return (eCommerceListVM, totalCount, pageLinkCount);
        }
    }
}
