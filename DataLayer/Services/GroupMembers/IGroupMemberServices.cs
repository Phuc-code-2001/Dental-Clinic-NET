using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DAOs;
using DataLayer.Schemas;

namespace DataLayer.Services.GroupMembers
{
    public interface IGroupMemberServices
    {
        public IEnumerable<GroupMember> GetAll();
        public GroupMember GetById(int id);
        public GroupMember GetByCode(string code);
        public int Insert(GroupMember groupMember);
        public GroupMember Update(GroupMember entity);
        public void Remove(int id);
    }
}
