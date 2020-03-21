using BiblioMit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public interface IAppUser
    {
        AppUser GetById(string id);
        IEnumerable<AppUser> GetAll();
        Task SetProfileImage(string id, Uri uri);
        Task UpdateUserRating(string id, System.Type type);
    }
}
