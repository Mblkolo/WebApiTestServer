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
        Task<long> CreateItem(long userId, string name);
        Task<Group> GetById(long groupId);
        Task<Group[]> GetMyGroups(long userId);
    }

    public class GroupRepository : IGroupRepository
    {
        private static long Id;
        private static object Locker = new object();
        private static List<Group> groups = new List<Group>();

        public Task<long> CreateItem(long userId, string name)
        {
            lock (Locker)
            {
                var group = new Group
                {
                    Id = ++Id,
                    Name = name,
                    UserId = userId
                };

                groups.Add(group);

                return Task.FromResult(group.Id);
            }
        }

        public Task<Group> GetById(long groupId)
        {
            lock (Locker)
            {
                return Task.FromResult(groups.SingleOrDefault(x => x.Id == groupId));
            }
        }

        public Task<Group[]> GetMyGroups(long userId)
        {
            lock (Locker)
            {
                return Task.FromResult(groups.Where(x => x.UserId == userId).ToArray());
            }
        }
    }
}