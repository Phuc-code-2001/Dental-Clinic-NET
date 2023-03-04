using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Permissions;
using System;

namespace Dental_Clinic_NET.API.Serializers
{
    public class UserSerializer
    {

        public PermissionOnBaseUser permission;

        public UserSerializer(PermissionOnBaseUser permission)
        {
            this.permission = permission;
        }

        public UserDTO Serialize(Func<BaseUser, UserDTO> mapperConfiguration)
        {
            var userInfo = mapperConfiguration(permission.Entity);

            if (!(permission.IsOwner || permission.IsAdmin)) userInfo.UserName = null;
            if (!(permission.IsOwner || permission.IsAdmin)) userInfo.Email = null;
            if (!(permission.IsOwner || permission.IsAdmin)) userInfo.PusherChannel = null;

            return userInfo;
        }
    }
}
