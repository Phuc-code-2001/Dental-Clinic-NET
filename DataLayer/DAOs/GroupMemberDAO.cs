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
                entity.Id = 0;
                var context = AppDbContext.GetTransaction();
                context.GroupMembers.Add(entity);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new Exception($"Exception At: GroupMemberDAO - Insert - {ex.Message}");
            }

            return entity.Id;
        }

        public GroupMember Update(GroupMember entity)
        {
            try
            {
                var context = AppDbContext.GetTransaction();
                context.GroupMembers.Update(entity);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception At: GroupMemberDAO - Update - {ex.Message}");
            }

            return entity;
        }

        public IQueryable<GroupMember> GetAll()
        {
            try
            {
                return AppDbContext.GetTransaction().GroupMembers;
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception At: GroupMemberDAO - GetAll - {ex.Message}");
            }
        }

        public GroupMember GetById(int Id)
        {
            try
            {
                return AppDbContext.GetTransaction().GroupMembers.FirstOrDefault(x => x.Id == Id);
            }
            catch(Exception ex)
            {
                throw new Exception($"Exception At: GroupMemberDAO - GetById - {ex.Message}");
            }
        }

        public GroupMember GetByCode(string code)
        {
            try
            {
                return AppDbContext.GetTransaction().GroupMembers.FirstOrDefault(x => x.MemberCode == code);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception At: GroupMemberDAO - GetByCode - {ex.Message}");
            }
        }

        public void Remove(GroupMember entity)
        {
            try
            {
                var context = AppDbContext.GetTransaction();
                context.GroupMembers.Remove(entity);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception At: GroupMemberDAO - Remove - {ex.Message}");
            }
        }

    }
}
