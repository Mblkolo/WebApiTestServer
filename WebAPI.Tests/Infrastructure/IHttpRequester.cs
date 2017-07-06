using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebAPI.Tests.Infrastructure
{
    public interface IHttpRequester
    {
        HttpClient CreateClient(string token = null);
        Task<TResult> GetAsync<TResult>(string url, string token);
        Task<TResult> PostAsync<T, TResult>(string url, string token, T dto);
        Task DeleteAsync(string url, string token);
    }
}