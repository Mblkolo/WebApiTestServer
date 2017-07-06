using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace WebAPI.Tests.Infrastructure
{

    public class ApiClientAccount
    {
        private readonly IHttpRequester _requester;

        public ApiClientAccount(IHttpRequester requester)
        {
            _requester = requester;
        }

        public async Task<string> GetToken(string userName, string password)
        {
            var td = await GetTokenDictionary(userName, password);
            if (td.ContainsKey("error"))
            {
                throw new ApplicationException(td["error"]);
            }
            var token = td["access_token"];
            return token;
        }

        // получение токена
        private async Task<Dictionary<string, string>> GetTokenDictionary(string login, string password)
        {
            var nameValueCollection = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "custom"),
                new KeyValuePair<string, string>("login", login),
                new KeyValuePair<string, string>("password", password)
            };
            var formUrlEncodedContent = new FormUrlEncodedContent(nameValueCollection);

            using (var client = _requester.CreateClient())
            {
                var response = await client.PostAsync("/api/account/token", formUrlEncodedContent);

                var status = response.StatusCode;
                if (status!=HttpStatusCode.OK)
                {
                    throw new HttpException("Unathorized!");
                }

                var result = await response.Content.ReadAsStringAsync();
                // Десериализация полученного JSON-объекта
                var tokenDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                return tokenDictionary;
            }
        }
    }
}
