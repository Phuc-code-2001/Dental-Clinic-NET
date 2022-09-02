using DataLayer.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic_NET.API.Models.GroupMemberModels
{
    public class CreateGroupMemberModel : GroupMember
    {
        private new int Id { get; set; }

        public GroupMember GetEntity()
        {
            return new GroupMember
            {
                MemberCode = this.MemberCode,
                Name = this.Name,
                Description = this.Description,
                BirthDay = this.BirthDay
            };
        }
    }
}
