using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiblioMit.Models
{
    public interface IForum
    {
        Forum GetbyId(int id);
        IEnumerable<Forum> GetAll();
        Task Create(Forum forum);
        Task Delete(int forumId);
        Task UpdateForumTitle(int forumId, string newTitle);
        Task UpdateForumDescription(int forumId, string newDescription);
        IEnumerable<AppUser> GetActiveUsers(int id);
        bool HasRecentPost(int id);
    }
}
