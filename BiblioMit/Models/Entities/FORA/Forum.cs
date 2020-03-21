using System;
using System.Collections.Generic;

namespace BiblioMit.Models
{
    public class Forum
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public Uri ImageUrl { get; set; }

        public virtual IEnumerable<Post> Posts { get; set; }
    }
}
