namespace WebAPI.Tests
{
    public class ApiClient
    {
        private readonly IHttpRequester _requester;

        private ApiClientItems _apiClientItems;
        public ApiClientItems Items => _apiClientItems ?? (_apiClientItems = new ApiClientItems(_requester));

        private ApiClientAccount _apiClientAccount;
        public ApiClientAccount Account => _apiClientAccount ?? (_apiClientAccount = new ApiClientAccount(_requester));

        public ApiClient(IHttpRequester requester)
        {
            _requester = requester;
        }

    }
}