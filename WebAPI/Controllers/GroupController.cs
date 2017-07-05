using System.Web.Http;
using WebAPI.Dto;
using WebAPI.Dal;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using System.Linq;

namespace WebAPI.Controllers
{
    [Authorize]
    public class GroupController : ApiController
    {
        private readonly IGroupRepository _groupRepository;

        public GroupController(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] CreateGroupDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Name))
                return BadRequest("Name is missing or empty");

            long userId = GetUserId();

            long groupId = await _groupRepository.CreateItem(userId, dto.Name);

            Group storedGroup = await _groupRepository.GetById(groupId);
            return Ok(Bind(storedGroup));
        }

        [HttpPost]
        public async Task<IHttpActionResult> Get([FromBody] CreateGroupDto dto)
        {
            long userId = GetUserId();

            Group[] groups = await _groupRepository.GetMyGroups(userId);

            return Ok(groups.Select(Bind).ToArray());
        }

        private long GetUserId()
        {
            return long.Parse((User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        private GroupDto Bind(Group storedGroup)
        {
            return new GroupDto
            {
                Id = storedGroup.Id,
                Name = storedGroup.Name
            };
        }


    }

    public class CreateGroupDto
    {
        public string Name { get; set; }
    }

    public class GroupDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class Group
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long UserId { get; internal set; }
    }
}
