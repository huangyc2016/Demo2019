using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace SignalrApp
{
   public class SignalrChat
    {
        HubConnection connection;
       public  SignalrChat(string access_token)
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chatHub",options=> 
                { 
                    options.AccessTokenProvider = () => Task.FromResult(access_token); 
                })
                .WithAutomaticReconnect()
                //.ConfigureLogging(logging =>
                //{
                //    // Log to the Console
                //    logging.AddConsole();

                //    // This will set ALL logging to Debug level
                //    logging.SetMinimumLevel(LogLevel.Debug);
                //})
                .Build();
            
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }
        public SignalrChat()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chatHub")
                .WithAutomaticReconnect()
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }

        public async void Connect()
        {
            connection.On<string, string>("broadcastMessage", (user, message) =>
            {
                var newMessage = $"{user}: {message}";
                Console.WriteLine(newMessage);
            });

            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void Send()
        {
            try
            {
                var username = "老黄";
                var message = "我是老黄";
                await connection.InvokeAsync("Send",
                    username, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
