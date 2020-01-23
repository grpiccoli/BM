using BiblioMit.Data;
using BiblioMit.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public class ForumService : IForum
    {
        private readonly ApplicationDbContext _context;

        public ForumService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(Forum forum)
        {
            _context.Add(forum);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int forumId)
        {
            var forum = GetbyId(forumId);
            _context.Remove(forum);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<AppUser> GetActiveUsers(int id)
        {
            var posts = GetbyId(id).Posts;
            
            if(posts != null || !posts.Any())
            {
                var postUsers = posts.Select(p => p.User);
                var replyUsers = posts.SelectMany(p => p.Replies).Select(r => r.User);
                return postUsers.Union(replyUsers).Distinct();
            }
            return new List<AppUser>();
        }

        public IEnumerable<Forum> GetAll()
        {
            return _context.Forum
                .Include(f => f.Posts);
        }

        public Forum GetbyId(int id)
        {
            var forum = _context.Forum
                .Where(f => f.Id == id)
                .Include(f => f.Posts)
                    .ThenInclude(p => p.User)
                .Include(f => f.Posts)
                    .ThenInclude(p => p.Replies)
                        .ThenInclude(r => r.User)
                .FirstOrDefault();

            return forum;
        }

        public bool HasRecentPost(int id)
        {
            const int hoursAgo = 78;
            var window = DateTime.Now.AddHours(-hoursAgo);
            return GetbyId(id).Posts.Any(p => p.Created > window);
        }

        public Task UpdateForumDescription(int forumId, string newDescription)
        {
            throw new NotImplementedException();
        }

        public Task UpdateForumTitle(int forumId, string newTitle)
        {
            throw new NotImplementedException();
        }
    }
}
