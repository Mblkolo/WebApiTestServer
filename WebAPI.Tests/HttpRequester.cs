using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebAPI.Tests
{
    public class HttpRequester : IHttpRequester
    {
        private readonly HttpMessageHandler _handler;
        private readonly Uri _baseAddress;

        public HttpRequester(HttpMessageHandler handler, Uri baseAddress)
        {
            _handler = handler;
            _baseAddress = baseAddress;
        }

        public HttpClient CreateClient(string token = null)
        {
            var client = new HttpClient(_handler) {BaseAddress = _baseAddress};
            if (token != null)
                //client.DefaultRequestHeaders.Add("Authorization", token);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public async Task<TResult> GetAsync<TResult>(string url, string token)
        {
            using (var client = CreateClient(token))
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode == false)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(result);
            }
        }

        public async Task<TResult> PostAsync<T, TResult>(string url, string token, T dto)
        {
            using (var client = CreateClient(token))
            {
                var jsonFormatter = new JsonMediaTypeFormatter();
                Startup.ConfigureJsonFormatter(jsonFormatter);

                var response = await client.PostAsync(url,
                    new ObjectContent(typeof(T), dto, jsonFormatter, "application/json"));
                if (response.IsSuccessStatusCode == false)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(result);
            }
        }

        public async Task DeleteAsync(string url, string token)
        {
            using (var client = CreateClient(token))
            {
                var response = await client.DeleteAsync(url);
                if (response.IsSuccessStatusCode == false)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                await Task.CompletedTask;
            }
        }

    }
}