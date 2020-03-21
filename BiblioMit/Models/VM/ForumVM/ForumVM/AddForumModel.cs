using Microsoft.AspNetCore.Http;
using System;

namespace BiblioMit.Models.ForumViewModels
{
    public class AddForumModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri ImageUrl { get; set; }
        public IFormFile ImageUpload { get; set; }
    }
}
