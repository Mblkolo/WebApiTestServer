using WebAPI.Dto;

namespace WebAPI.Tests
{
    public static class ItemGenerator
    {

        public static ItemDto Empty => new ItemDto();
        
        public static ItemDto WithId(this ItemDto item, int id)
        {
            item.Id = id;
            return item;
        }

        public static ItemDto WithName(this ItemDto item, string name)
        {
            item.Name = name;
            return item;
        }

        public static ItemDto Default => Empty
            .WithId(1)
            .WithName("Item1");

    }
}