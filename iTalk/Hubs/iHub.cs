using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using iTalk.Models;

namespace iTalk.Hubs
{
    public class iHub : Hub
    {
        public static HashSet<iUser> ConnectedUsers { get; set; }

        public static string CurrentMessage { get; set; }

        public void Hello()
        {
            Clients.All.hello();
        }

        /// <summary>
        /// To be called when a new User Connects, Notifies other users that a new User is Connected
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void Connect(string name)
        {
            var id = Context.ConnectionId;
            if(ConnectedUsers == null)
            {
                ConnectedUsers = new HashSet<iUser>();
            }
            if (ConnectedUsers.Count(x => x.ID == id) == 0)
            {
                ConnectedUsers.Add(new iUser() { ID = id, Name = name });
                Clients.Caller.onConnected(id, name, ConnectedUsers.Where( x => x.ID != id));
                Clients.AllExcept(id).onNewUserConnected(id, name);
            }
        }

        public void Broadcast(string id, string name, string message)
        {
            Clients.All.broadcast(id, name, message);
        }

        public void Message(string toUserId, string message)
        {
            string fromUserID = Context.ConnectionId;
            var toUser = ConnectedUsers.FirstOrDefault( x => x.ID == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault( x => x.ID == fromUserID);
            if(fromUser != null && toUser != null)
            {
                Clients.Client(toUserId).message(fromUserID, fromUser.Name, message);
                Clients.Caller.message(toUserId, fromUser.Name, message);
            }
        }

    }
}