using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RequestService.Infrastructure.External
{
    public class RequestStatusHub : Hub
    {
        public async Task JoinRequestGroup(string requestId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, requestId);
        }

        public async Task LeaveRequestGroup(string requestId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, requestId);
        }
    }
}
