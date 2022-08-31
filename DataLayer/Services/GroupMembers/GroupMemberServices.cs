using DataLayer.DAOs;
using DataLayer.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Services.GroupMembers
{
    public class GroupMemberServices : IGroupMemberServices
    {
        public IEnumerable<GroupMember> GetAll()
        {
            return GroupMemberDAO.GetInstance.GetAll();
        }

        public GroupMember GetByCode(string code)
        {
            return GroupMemberDAO.GetInstance.GetByCode(code);
        }

        public GroupMember GetById(int id)
        {
            return GroupMemberDAO.GetInstance.GetById(id);
        }

        public int Insert(GroupMember entity)
        {
            return GroupMemberDAO.GetInstance.Insert(entity);
        }

        public void Remove(int id)
        {
            GroupMemberDAO.GetInstance.Remove(id);
        }

        public GroupMember Update(GroupMember entity)
        {
            return GroupMemberDAO.GetInstance.Update(entity);
        }
    }
}
