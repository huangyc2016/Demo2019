using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalrApi.Hubs
{
    
    public class ChatHub:Hub
    {
        public async Task Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            await Clients.All.SendAsync("broadcastMessage", name, message);
        }

        /// <summary>
        /// 客户端连接的时候调用
        /// </summary>
        /// <returns></returns>
        [Authorize]//添加Authorize标签，可以加在方法上，也可以加在类上
        public override async Task OnConnectedAsync()
        {
            System.Diagnostics.Trace.WriteLine("客户端连接成功");


            await base.OnConnectedAsync();
        }//所有链接的客户端都会在这里

        /// <summary>
        /// 连接终止时调用。
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            System.Diagnostics.Trace.WriteLine("连接终止");
            return base.OnDisconnectedAsync(exception);
        }

    }
}
