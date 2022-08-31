using DataLayer.DataContexts;
using DataLayer.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DAOs
{
    public class GroupMemberDAO
    {
        private static GroupMemberDAO Instance;

        public static GroupMemberDAO GetInstance => Instance ?? (Instance = new GroupMemberDAO());

        private GroupMemberDAO() { }

        public int Insert(GroupMember entity)
        {
            try
            {
                AppDbContext.GetInstance.GroupMembers.Add(entity);
                AppDbContext.GetInstance.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new Exception("Exception At: GroupMemberDAO - Insert");
            }

            return entity.Id;
        }

        public GroupMember Update(GroupMember entity)
        {
            try
            {
                AppDbContext.GetInstance.GroupMembers.Update(entity);
                AppDbContext.GetInstance.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception At: GroupMemberDAO - Update");
            }

            return entity;
        }

        public IEnumerable<GroupMember> GetAll()
        {
            return AppDbContext.GetInstance.GroupMembers;
        }

        public GroupMember GetById(int Id)
        {
            return AppDbContext.GetInstance.GroupMembers.FirstOrDefault(x => x.Id == Id);
        }

        public GroupMember GetByCode(string code)
        {
            return AppDbContext.GetInstance.GroupMembers.FirstOrDefault(x => x.MemberCode == code);
        }

        public void Remove(int Id)
        {
            try
            {
                var entity = GetById(Id);
                AppDbContext.GetInstance.GroupMembers.Remove(entity);
                AppDbContext.GetInstance.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception At: GroupMemberDAO - Remove");
            }
        }

    }
}
