using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using WebAPI.Authentication;
using WebAPI.Tests.Infrastructure;
using WebAPI.Dto;

namespace WebAPI.Tests
{
    [TestFixture]
    public class GroupInMemoryTests : InMemoryTestBase
    {

        [Test]
        public async Task GetItemsTest()
        {
            var adminToken = await ApiClient.Account.GetToken("123", "456");

            var group = await ApiClient.Group.CreateGroup(adminToken, new CreateGroupDto
            {
                Name = "Тестовая группа"
            });

            Assert.That(group, Is.Not.Null);
            Assert.That(group.Name, Is.EqualTo("Тестовая группа"));
        }
    }
}