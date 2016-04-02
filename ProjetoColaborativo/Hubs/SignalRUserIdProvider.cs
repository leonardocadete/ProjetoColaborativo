using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace ProjetoColaborativo.Hubs
{
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            return request.User.Identity.GetUserId();
        }
    }
}