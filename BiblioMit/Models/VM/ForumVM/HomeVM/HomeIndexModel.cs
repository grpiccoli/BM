using BiblioMit.Models.PostViewModels;
using System.Collections.Generic;

namespace BiblioMit.Models
{
    public class HomeIndexModel
    {
        public string SearchQuery { get; set; }
        public IEnumerable<PostListingModel> LatestPosts { get; set; }
    }
}
