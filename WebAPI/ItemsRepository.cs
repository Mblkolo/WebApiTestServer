using System.Collections.Generic;
using WebAPI.Dto;

namespace WebAPI
{
    public interface IItemsRepository
    {
        List<ItemDto> GetItems();
        void CreateItem(ItemDto item);
        void DeleteItem(int itemId);
    }

    public class ItemsRepository : IItemsRepository
    {
        protected readonly List<ItemDto> Items = new List<ItemDto>();

        public List<ItemDto> GetItems()
        {
            return Items;
        }

        public void CreateItem(ItemDto item)
        {
            Items.Add(item);
        }

        public void DeleteItem(int itemId)
        {
            var index = Items.FindIndex(x => x.Id == itemId);
            Items.RemoveAt(index);
        }
    }
}