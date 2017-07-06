using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using WebAPI.Authentication;
using WebAPI.Tests.Infrastructure;
using WebAPI.Dto;
using System;
using WebAPI.Controllers;

namespace WebAPI.Tests
{
    [TestFixture]
    public class GroupInMemoryTests : InMemoryTestBase
    {
        [Test]
        public async Task CreateGroupTest()
        {
            var adminToken = await ApiClient.Account.GetToken("123", "456");

            var groupMember = await ApiClient.Group.CreateGroup(adminToken, new CreateGroupDto
            {
                Name = "Тестовая группа"
            });

            Assert.That(groupMember, Is.Not.Null);
            Assert.That(groupMember.Group, Is.Not.Null);
            Assert.That(groupMember.Group.Name, Is.EqualTo("Тестовая группа"));
            Assert.That(groupMember.Role, Is.EqualTo(GroupMemberRole.Owner));
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


            GroupMemberDto[] groupMembers = await ApiClient.Group.GetGroups(adminToken);

            Assert.That(groupMembers, Is.Not.Null);
            Assert.That(groupMembers, Has.Length.GreaterThanOrEqualTo(1));
            Assert.That(groupMembers.Last().Group, Is.Not.Null);
            Assert.That(groupMembers.Last().Group.Name, Is.EqualTo(groupName));
            Assert.That(groupMembers.Last().Role, Is.EqualTo(GroupMemberRole.Owner));
        }
    }
}