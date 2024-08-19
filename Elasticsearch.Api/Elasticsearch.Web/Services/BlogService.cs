using Elasticsearch.Web.Models;
using Elasticsearch.Web.Models.ViewModels;
using Elasticsearch.Web.Repositories;

namespace Elasticsearch.Web.Services
{
    public class BlogService
    {
        private readonly BlogRepository _repository;

        public BlogService(BlogRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> SaveAsync(BlogCreateVM blogCreateVM)
        {
            blogCreateVM.UserId = Guid.NewGuid();

            var newBlog = new Blog()
            {
                Title = blogCreateVM.Title,
                Content = blogCreateVM.Content,
                Tags = blogCreateVM.Tags.Split(",").ToArray(),
                UserId = Guid.NewGuid(),
            };

            var isCreated = await _repository.SaveAsync(newBlog);

            return isCreated != null;
        }

        public async Task<List<BlogSearchVM>> SearchAsync(string searchText)
        {

            var blogList = await _repository.SearchAsync(searchText);

            return blogList.Select(b => new BlogSearchVM()
            {
                Id = b.Id,
                Title = b.Title,
                Content = b.Content,
                Created = b.Created.ToShortDateString(),
                Tags = string.Join(",", b.Tags),
                UserId = b.UserId.ToString()
            }).ToList();
        }
    }
}
