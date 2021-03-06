﻿using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebAPI.Tests.Infrastructure
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

                await CheckStatusCode(response);
                return await Deserialize<TResult>(response);
            }
        }

        public async Task<TResult> PostAsync<T, TResult>(string url, string token, T dto)
        {
            using (var client = CreateClient(token))
            {
                var jsonFormatter = new JsonMediaTypeFormatter();
                Startup.ConfigureJsonFormatter(jsonFormatter);

                var response = await client.PostAsync(url, new ObjectContent(typeof(T), dto, jsonFormatter, "application/json"));

                await CheckStatusCode(response);
                return await Deserialize<TResult>(response);
            }
        }

        public async Task DeleteAsync(string url, string token)
        {
            using (var client = CreateClient(token))
            {
                var response = await client.DeleteAsync(url);
                await CheckStatusCode(response);
            }
        }

        private async Task CheckStatusCode(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode == false)
                throw new RequestException($"{response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        }

        private async Task<T> Deserialize<T>(HttpResponseMessage response)
        {
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}