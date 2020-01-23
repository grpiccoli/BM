using BiblioMit.Models.PostViewModels;
using System.Collections.Generic;

namespace BiblioMit.Models.ForumViewModels
{
    public class ForumTopicModel
    {
        public ForumListingModel Forum { get; set; }
        public IEnumerable<PostListingModel> Post { get; set; }
        public string SearchQuery { get; set; }
    }
}
