using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public class ProgressHub : Hub
    {
        public async Task SendProgress(string user, int progress)
        {
            await Clients.User(user).SendAsync("Progress", progress);
        }
    }
}
