using DataLayer.Schemas;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Permissions;
using System.Collections.Generic;

namespace Dental_Clinic_NET.API.Serializers
{
    public class UserSerializer
    {

        public delegate UserDTO Transformer(BaseUser user);

        public PermissionOnBaseUser permission;

        public UserSerializer(PermissionOnBaseUser permission)
        {
            this.permission = permission;
        }

        public UserDTO Serialize(Transformer mapperHandle)
        {
            var userInfo = mapperHandle(permission.Entity);

            if (!(permission.IsOwner || permission.IsAdmin)) userInfo.Username = null;
            if (!(permission.IsOwner || permission.IsAdmin)) userInfo.Email = null;
            if (!(permission.IsOwner || permission.IsAdmin)) userInfo.FbConnectedId = null;

            return userInfo;
        }
    }
}
