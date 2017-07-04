using System.Web.Http;
using WebAPI.Dto;

namespace WebAPI.Controllers
{
    public class ItemsController : ApiController
    {
        private readonly IItemsRepository _itemsRepository;

        public ItemsController(IItemsRepository itemsRepository)
        {
            _itemsRepository = itemsRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = _itemsRepository.GetItems();

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public IHttpActionResult Post([FromBody] ItemDto item)
        {
            _itemsRepository.CreateItem(item);

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        public IHttpActionResult Delete([FromUri] int itemId)
        {
            _itemsRepository.DeleteItem(itemId);

            return Ok();
        }
    }
}
