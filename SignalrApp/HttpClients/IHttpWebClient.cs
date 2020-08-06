using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalrApp.HttpClients
{
    public interface IHttpWebClient
    {
        Task<string> GetAsync(string requestUri, ApiParameters parameters, string token);

        Task<string> PostAsync(string requestUri, ApiParameters parameters, string token);
    }

   
}
