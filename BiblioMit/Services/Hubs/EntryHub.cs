using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public class EntryHub : Hub
    {
        public async Task Send(string user, string function, string message)
        {
            await Clients.User(user).SendAsync("Update", function, message).ConfigureAwait(false);
        }
    }
}
