using BiblioMit.Data;
using BiblioMit.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public class PostService : IPost
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Post post)
        {
            _context.Add(post);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task AddReply(PostReply reply)
        {
            _context.PostReply.Add(reply);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task EditPostContent(int id, string newContent)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetAll()
        {
            return _context.Post
                .Include(p => p.User)
                .Include(p => p.Replies)
                    .ThenInclude(r => r.User)
                .Include(p => p.Forum)
                .Where(p => p.UserId != null);
        }

        public Post GetById(int id)
        {
            return _context.Post
                .Where(p => p.Id == id)
                .Include(p => p.User)
                .Include(p => p.Replies)
                    .ThenInclude(r => r.User)
                .Include(p => p.Forum)
                .First();
        }

        public IEnumerable<Post> GetFilteredPosts(Forum forum, string searchQuery)
        {
            return string.IsNullOrEmpty(searchQuery) 
                ? forum?.Posts 
                : forum?.Posts
                    .Where(p => p.Title.Contains(searchQuery, StringComparison.InvariantCultureIgnoreCase)
                    || p.Content.Contains(searchQuery, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<Post> GetFilteredPosts(string searchQuery)
        {
            return GetAll().Where(p => p.Title.Contains(searchQuery, StringComparison.InvariantCultureIgnoreCase)
                    || p.Content.Contains(searchQuery, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<Post> GetLatestsPosts(int n)
        {
            return GetAll().OrderByDescending(p => p.Created).Take(n);
        }

        public IEnumerable<Post> GetPostsByForum(int Id)
        {
            return _context.Forum
                .Where(f => f.Id == Id)
                .FirstOrDefault()
                .Posts;
        }
    }
}
