using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public class HubMethods<THub> where THub : Hub
    {
        private IHubContext<THub> _hubContext;
        public HubMethods(IHubContext<THub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task HubAll(string method, params object[] args)
        {
            return _hubContext.Clients.All.SendAsync(method, args);
        }

        public Task HubUser(string method, string userId, params object[] args)
        {
            return _hubContext.Clients.User(userId).SendAsync(method, args);
        }
    }
}
