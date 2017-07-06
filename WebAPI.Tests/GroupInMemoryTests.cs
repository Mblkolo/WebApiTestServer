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


        [Test]
        public async Task CreateGroupNoteTest()
        {
            var adminToken = await ApiClient.Account.GetToken("123", "456");

            var member = await ApiClient.Group.CreateGroup(adminToken, new CreateGroupDto
            {
                Name = "Тестовая группа " + DateTime.UtcNow.Ticks
            });

            var noteDto = await ApiClient.Group.AddNote(adminToken, member.Group.Id, "Тестовая заметка");

            Assert.That(noteDto, Is.Not.Null);
            Assert.That(noteDto.Text, Is.EqualTo("Тестовая заметка"));
        }


        [Test]
        public async Task JoinToGroupTest()
        {
            var adminToken = await ApiClient.Account.GetToken("123", "456");

            var groupOwner = await ApiClient.Group.CreateGroup(adminToken, new CreateGroupDto
            {
                Name = "Тестовая группа " + DateTime.UtcNow.Ticks
            });

            var userToken = await ApiClient.Account.GetToken("aaa", "bbb");

            GroupMember userMember = await ApiClient.Group.Join(userToken, groupOwner.Group.Id);

            Assert.That(userMember, Is.Not.Null);
            Assert.That(userMember.Role, Is.EqualTo(GroupMemberRole.User));
            Assert.That(userMember.Group.Id, Is.EqualTo(groupOwner.Group.Id));
        }

        [Test]
        public async Task SetModeratorTest()
        {
            var adminToken = await ApiClient.Account.GetToken("123", "456");

            var groupOwner = await ApiClient.Group.CreateGroup(adminToken, new CreateGroupDto
            {
                Name = "Тестовая группа " + DateTime.UtcNow.Ticks
            });

            var userToken = await ApiClient.Account.GetToken("aaa", "bbb");

            GroupMember userMember = await ApiClient.Group.Join(userToken, groupOwner.Group.Id);

            GroupMember userMemberAsModerator = await ApiClient.Group.SetModerator(adminToken, groupOwner.Group.Id, userMember.Id);

            Assert.That(userMemberAsModerator, Is.Not.Null);
            Assert.That(userMemberAsModerator.Role, Is.EqualTo(GroupMemberRole.Moderator));
        }

        [Test]
        public async Task UserCannotAddNotesTest()
        {
            var adminToken = await ApiClient.Account.GetToken("123", "456");

            var groupOwner = await ApiClient.Group.CreateGroup(adminToken, new CreateGroupDto
            {
                Name = "Тестовая группа " + DateTime.UtcNow.Ticks
            });

            var userToken = await ApiClient.Account.GetToken("aaa", "bbb");

            GroupMember userMember = await ApiClient.Group.Join(userToken, groupOwner.Group.Id);

            Assert.ThrowsAsync<RequestException>(async () => await ApiClient.Group.AddNote(userToken, groupOwner.Group.Id, "Этот текст не должен добавиться"));
        }

        [Test]
        public async Task ModeratoCanAddNotesTest()
        {
            var adminToken = await ApiClient.Account.GetToken("123", "456");

            var groupOwner = await ApiClient.Group.CreateGroup(adminToken, new CreateGroupDto
            {
                Name = "Тестовая группа " + DateTime.UtcNow.Ticks
            });

            var userToken = await ApiClient.Account.GetToken("aaa", "bbb");

            GroupMember userMember = await ApiClient.Group.Join(userToken, groupOwner.Group.Id);
            await ApiClient.Group.SetModerator(adminToken, groupOwner.Group.Id, userMember.Id);

            var note = await ApiClient.Group.AddNote(userToken, groupOwner.Group.Id, "Заметка от модератора");

            Assert.That(note, Is.Not.Null);
            Assert.That(note.Text, Is.EqualTo("Заметка от модератора"));
        }
    }
}