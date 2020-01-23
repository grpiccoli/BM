using BiblioMit.Models.PostViewModels;
using System.Collections.Generic;

namespace BiblioMit.Models.HomeViewModels
{
    public class SearchResultModel
    {
        public IEnumerable<PostListingModel> Posts { get; set; }
        public string SearchQuery { get; set; }
        public bool EmptySearchResults { get; set; }
    }
}
