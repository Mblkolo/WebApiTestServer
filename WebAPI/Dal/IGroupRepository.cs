using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebAPI.Controllers;

namespace WebAPI.Dal
{
    public interface IGroupRepository
    {
        Task<long> CreateGroup(long userId, string name);
        Task<GroupMember> GetById(long groupId, long userId);
        Task<GroupMember[]> GetMyGroups(long userId);
    }

    public class GroupRepository : IGroupRepository
    {
        private static long Id;
        private static long MemberId;
        private static object Locker = new object();
        private static List<Group> groups = new List<Group>();

        public Task<long> CreateGroup(long userId, string name)
        {
            lock (Locker)
            {
                var group = new Group
                {
                    Id = ++Id,
                    Name = name
                };

                var groupMember = new GroupMember(group)
                {
                    Id = ++MemberId,
                    Role = GroupMemberRole.Owner,
                    UserId = userId
                };

                group.Members.Add(groupMember);

                groups.Add(group);

                return Task.FromResult(group.Id);
            }
        }

        public Task<GroupMember> GetById(long groupId, long userId)
        {
            lock (Locker)
            {
                return Task.FromResult(groups.SingleOrDefault(x => x.Id == groupId)?.Members.SingleOrDefault(x => x.UserId == userId));
            }
        }

        public Task<GroupMember[]> GetMyGroups(long userId)
        {
            lock (Locker)
            {
                return Task.FromResult(groups.SelectMany(x => x.Members).Where(x => x.UserId == userId).ToArray());
            }
        }
    }
}