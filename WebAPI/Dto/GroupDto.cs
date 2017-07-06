using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Controllers;

namespace WebAPI.Dto
{
    public class GroupDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class GroupMemberDto
    {
        public long Id;
        public long UserId { get; set; }
        public GroupMemberRole Role { get; set; }
        public GroupDto Group { get; set; }
    }
}