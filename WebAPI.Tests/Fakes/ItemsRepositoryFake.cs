namespace WebAPI.Tests.Fakes
{
    public class ItemsRepositoryFake : ItemsRepository
    {
        public void Clear()
        {
            Items.Clear();
        }
    }
}