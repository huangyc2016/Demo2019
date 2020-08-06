

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SignalrApp.HttpClients;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace SignalrApp
{
    class Program
    {
        static TokenModel token;
        static async Task Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole()
                    .AddEventLog();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Example log message");

            var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient("github", c =>
                {
                    c.BaseAddress = new Uri("http://localhost:5000/");
                });
                services.AddTransient<IHttpWebClient, HttpWebClient>();
            }).UseConsoleLifetime();

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var httpclient = services.GetRequiredService<IHttpWebClient>();
                    ApiParameters parameters = new ApiParameters();
                    parameters.UserName = "huangyc";
                    parameters.Password = "aa1234";
                    string uri = "api/Auth/GetToken";
                    var resultstr = await httpclient.GetAsync(uri, parameters, null);

                    //string uri = "api/Auth/PostToken";
                    //var resultstr = await httpclient.PostAsync(uri, parameters, null);
                    token = JsonConvert.DeserializeObject<TokenModel>(resultstr);
                    Console.WriteLine(token.access_token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred.");
                }
            }

            if (token.access_token != null)
            {
                SignalrChat sc = new SignalrChat(token.access_token);
                sc.Connect();

                sc.Send();
                Console.Read();
            }
        }

       

    }

}
