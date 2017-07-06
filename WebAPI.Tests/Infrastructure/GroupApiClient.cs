using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.Dto;

namespace WebAPI.Tests.Infrastructure
{

    public class GroupApiClient
    {
        private readonly IHttpRequester _requester;

        public GroupApiClient(IHttpRequester requester)
        {
            _requester = requester;
        }

        public async Task<GroupDto[]> GetGroups(string token)
        {
            return await _requester.GetAsync<GroupDto[]>("/api/group", token);
        }

        public async Task<GroupDto> CreateGroup(string token, CreateGroupDto dto)
        {
            return await _requester.GetAsync<GroupDto>("/api/group", token);
        }

    }
}
