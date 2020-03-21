using System;

namespace BiblioMit.Models.PostViewModels
{
    public class NewPostModel
    {
        public string ForumName { get; set; }
        public int ForumId { get; set; }
        public string AuthorName { get; set; }
        public Uri ForumImageUrl { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
