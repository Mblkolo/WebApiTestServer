using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using WebAPI.Authentication;

namespace WebAPI.Tests
{
    [TestFixture]
    public class ItemsInMemoryTests : InMemoryTestBase
    {

        [Test]
        public async Task GetItemsTest()
        {
            var items = await ApiClient.Items.GetItems();

            Assert.That(items, Is.Empty);
            Assert.That(items.Length, Is.EqualTo(0));
        }

        [Test]
        public async Task ItemsCreateTest()
        {
            var user1 = new User {Login = "123", Password = "456"};
            var token = await ApiClient.Account.GetToken(user1.Login, user1.Password);

            var item1 = ItemGenerator.Default;
            await ApiClient.Items.CreateItem(token, item1);

            var items = await ApiClient.Items.GetItems();

            Assert.That(items, Is.Not.Null);
            Assert.That(items.Length, Is.EqualTo(1));
            Assert.That(items.Single().Id, Is.EqualTo(item1.Id));
            Assert.That(items.Single().Name, Is.EqualTo(item1.Name));
        }

        [Test]
        public async Task ItemsDeleteTest()
        {
            const string user1Name = "123";
            const string user1Password = "456";
            var token = await ApiClient.Account.GetToken(user1Name, user1Password);

            var item1 = ItemGenerator.Default;
            var item2 = ItemGenerator.Empty.WithId(2).WithName("Item2");
            await ApiClient.Items.CreateItem(token, item1);
            await ApiClient.Items.CreateItem(token, item2);

            var items = await ApiClient.Items.GetItems();

            Assert.That(items.Length, Is.EqualTo(2));

            await ApiClient.Items.DeleteItem(token, item2.Id);

            items = await ApiClient.Items.GetItems();
            Assert.That(items.Length, Is.EqualTo(1));
            Assert.That(items.Single().Id, Is.EqualTo(item1.Id));
            Assert.That(items.Single().Name, Is.EqualTo(item1.Name));
        }

        [Test]
        public async Task ItemsCountTest()
        {
            const string user1Name = "123";
            const string user1Password = "456";
            var token = await ApiClient.Account.GetToken(user1Name, user1Password);

            var item1 = ItemGenerator.Default;
            var item2 = ItemGenerator.Empty.WithId(2).WithName("Item2");
            await ApiClient.Items.CreateItem(token, item1);
            await ApiClient.Items.CreateItem(token, item2);

            var items = await ApiClient.Items.GetItems();

            Assert.That(items.Length, Is.EqualTo(2));
        }
    }
}