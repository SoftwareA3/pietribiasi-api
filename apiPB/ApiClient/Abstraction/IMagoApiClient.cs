using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.ApiClient.Abstraction
{
    public interface IMagoApiClient
    {
        Task<HttpResponseMessage> SendPostAsyncWithToken<T>(string endpoint, IEnumerable<T> body, string token);
        Task<HttpResponseMessage> SendPostAsync(string endpoint, object body);
    }
}