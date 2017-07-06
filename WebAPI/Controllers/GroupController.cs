using System.Web.Http;
using WebAPI.Dto;
using WebAPI.Dal;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/group")]
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

            long groupId = await _groupRepository.CreateGroup(userId, dto.Name);

            GroupMember storedGroup = await _groupRepository.GetById(groupId, userId);

            var result = Bind(storedGroup);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            long userId = GetUserId();

            GroupMember[] members = await _groupRepository.GetMyGroups(userId);

            return Ok(members.Select(Bind).ToArray());
        }

        [HttpPost]
        [Route("{groupid:long}/notes")]
        public async Task<IHttpActionResult> Notes(long groupid, [FromBody] CreateNoteDto dto)
        {
            long userId = GetUserId();

            var groupMember = await _groupRepository.GetById(groupid, userId);
            if (groupMember == null)
                return NotFound();

            if (groupMember.Role != GroupMemberRole.Owner && groupMember.Role != GroupMemberRole.Moderator)
                return BadRequest("Access denited");

            if (string.IsNullOrWhiteSpace(dto.Text))
                return BadRequest("Text is missing or empty");

            Note note = await _groupRepository.AddNote(groupid, dto.Text);

            return Ok(Bind(note));
        }

        private NoteDto Bind(Note note)
        {
            return new NoteDto
            {
                Id = note.Id,
                Text = note.Text
            };
        }

        private long GetUserId()
        {
            return long.Parse((User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        private GroupMemberDto Bind(GroupMember groupMember)
        {
            return new GroupMemberDto
            {
                Id = groupMember.Id,
                UserId = groupMember.UserId,
                Role = groupMember.Role,
                Group = Bind(groupMember.Group)
            };
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

    public class Group
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<GroupMember> Members { get; set; } = new List<GroupMember>();
        public List<Note> Notes { get; set; } = new List<Note>();
    }

    public enum GroupMemberRole
    {
        User,
        Owner,
        Moderator
    }

    public class GroupMember
    {
        public GroupMember(Group group)
        {
            Group = group;
        }

        public long Id { get; set; }
        public Group Group {get;}
        public GroupMemberRole Role { get; set; }
        public long UserId { get; internal set; }
    }

    public class Note
    {
        public long Id { get; set; }
        public string Text { get; set; }
    }
}
