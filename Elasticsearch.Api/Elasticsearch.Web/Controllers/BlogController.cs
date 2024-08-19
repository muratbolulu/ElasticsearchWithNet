using Elasticsearch.Web.Models;
using Elasticsearch.Web.Models.ViewModels;
using Elasticsearch.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.Web.Controllers
{
    public class BlogController : Controller
    {
        private BlogService _blogService;

        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
        }

        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(BlogCreateVM blogCreateVM)
        {
            var isSuccess = await _blogService.SaveAsync(blogCreateVM);

            if (!isSuccess)
            {
                TempData["result"] = "kayıt başarısız";

                return RedirectToAction(nameof(BlogController.Save));
            }

            TempData["result"] = "kayıt başarılı";

            return RedirectToAction(nameof(BlogController.Save));
        }

        public async Task<IActionResult> Search()
        {
            //bloVM e çevirilmeli.
            //return View(new List<Blog>());
            return View(await _blogService.SearchAsync(string.Empty));
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchText)
        {
            ViewBag.searchText = searchText;
            return View(await _blogService.SearchAsync(searchText));
        }
    }
}
