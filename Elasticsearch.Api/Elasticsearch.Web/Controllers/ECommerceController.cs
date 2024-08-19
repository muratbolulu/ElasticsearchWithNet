using Elasticsearch.Web.Models.ViewModels;
using Elasticsearch.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.Web.Controllers
{
    public class ECommerceController : Controller
    {
        private readonly ECommerceService _eCommerceService;

        public ECommerceController(ECommerceService eCommerceService)
        {
            _eCommerceService = eCommerceService;
        }

        public async Task<IActionResult> Search([FromQuery] SearchPageVM searchPageVM)
        {
            var (eCommerceList,totalCount,pageLinkCount) = await _eCommerceService.SearchAsync(searchPageVM.SearchViewModel, searchPageVM.Page, searchPageVM.PageSize);

            searchPageVM.List = eCommerceList;
            searchPageVM.TotalCount = totalCount;
            searchPageVM.PageLinkCount = pageLinkCount;

            return View(searchPageVM);
        }
    }
}
