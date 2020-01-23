using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiblioMit.Models;
using BiblioMit.Data;

namespace BiblioMit.Services
{
    public class AppUserService : IAppUser
    {
        private readonly ApplicationDbContext _context;

        public AppUserService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<AppUser> GetAll()
        {
            return _context.AppUser;
        }

        public AppUser GetById(string id)
        {
            return GetAll().FirstOrDefault(
                u => u.Id == id);
        }

        public async Task UpdateUserRating(string id, System.Type type)
        {
            var user = GetById(id);
            user.Rating = CalculateUserRating(type, user.Rating);
            await _context.SaveChangesAsync();
        }

        private int CalculateUserRating(System.Type type, int userRating)
        {
            var inc = 0;
            if (type == typeof(Post))
                inc = 1;

            if (type == typeof(PostReply))
                inc = 3;

            return userRating + inc;
        }

        public async Task SetProfileImage(string id, Uri uri)
        {
            var user = GetById(id);
            user.ProfileImageUrl = uri.AbsoluteUri;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
