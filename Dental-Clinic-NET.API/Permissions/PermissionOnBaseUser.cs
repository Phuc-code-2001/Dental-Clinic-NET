using DataLayer.Domain;

namespace Dental_Clinic_NET.API.Permissions
{
    public class PermissionOnBaseUser : BasePermission<BaseUser>
    {
        public PermissionOnBaseUser() { }

        public PermissionOnBaseUser(BaseUser loggedUser, BaseUser entity) : base(loggedUser, entity)
        {
            HandleOwnerPermission((entity) => entity.Id == loggedUser.Id);
        }
    }
}
