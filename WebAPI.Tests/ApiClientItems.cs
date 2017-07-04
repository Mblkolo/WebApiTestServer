using System.Threading.Tasks;
using WebAPI.Dto;

namespace WebAPI.Tests
{

    public class ApiClientItems
    {
        private readonly IHttpRequester _requester;

        public ApiClientItems(IHttpRequester requester)
        {
            _requester = requester;
        }

        public async Task<ItemDto[]> GetItems()
        {
            return await _requester.GetAsync<ItemDto[]>("/api/items", null);
        }

        public async Task CreateItem(string token, ItemDto item)
        {
            await _requester.PostAsync<ItemDto, object>("/api/items", token, item);
        }

        public async Task DeleteItem(string token, int itemId)
        {
            await _requester.DeleteAsync("/api/items/?itemId=" + itemId, token);
        }

    }
}
