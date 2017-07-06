using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using WebAPI.Authentication;
using WebAPI.Tests.Infrastructure;
using WebAPI.Dto;
using System;

namespace WebAPI.Tests
{
    [TestFixture]
    public class GroupInMemoryTests : InMemoryTestBase
    {
        [Test]
        public async Task CreateGroupTest()
        {
            var adminToken = await ApiClient.Account.GetToken("123", "456");

            var group = await ApiClient.Group.CreateGroup(adminToken, new CreateGroupDto
            {
                Name = "Тестовая группа"
            });

            Assert.That(group, Is.Not.Null);
            Assert.That(group.Name, Is.EqualTo("Тестовая группа"));
        }

        [Test]
        public async Task GetGroupsTest()
        {
            var adminToken = await ApiClient.Account.GetToken("123", "456");

            var groupName = "Тестовая группа " + DateTime.UtcNow.Ticks;
            var group = await ApiClient.Group.CreateGroup(adminToken, new CreateGroupDto
            {
                Name = groupName
            });


            GroupDto[] groups = await ApiClient.Group.GetGroups(adminToken);

            Assert.That(groups, Is.Not.Null);
            Assert.That(groups, Has.Length.GreaterThanOrEqualTo(1));
            Assert.That(groups.Last().Name, Is.EqualTo(groupName));
        }
    }
}