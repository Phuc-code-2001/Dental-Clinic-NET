using DataLayer.Schemas;
using Dental_Clinic_NET.API.Permissions;
using System.Collections.Generic;

namespace Dental_Clinic_NET.API.Serializers
{
    public class UserSerializer
    {

        public PermissionOnBaseUser permission;

        public UserSerializer(PermissionOnBaseUser permission)
        {
            this.permission = permission;
        }

        public Dictionary<string, object> Serialize()
        {
            var userInfo = new Dictionary<string, object>();

            userInfo.Add("id", permission.Entity.Id);
            userInfo.Add("fullname", permission.Entity.FullName);
            userInfo.Add("birthday", permission.Entity.BirthDate);
            userInfo.Add("phone", permission.Entity.PhoneNumber);
            userInfo.Add("image_url", permission.Entity.ImageURL);
            userInfo.Add("role", permission.Entity.Type.ToString());

            if (permission.IsOwner || permission.IsAdmin) userInfo.Add("username", permission.Entity.UserName);
            if (permission.IsOwner || permission.IsAdmin) userInfo.Add("email", permission.Entity.Email);
            if (permission.IsOwner || permission.IsAdmin) userInfo.Add("facebook_id", permission.Entity.FbConnectedId);

            return userInfo;
        }
    }
}
