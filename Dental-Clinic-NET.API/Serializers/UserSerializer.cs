using DataLayer.Schemas;
using System.Collections.Generic;

namespace Dental_Clinic_NET.API.Serializers
{
    public class UserSerializer : BaseSerializer<BaseUser>
    {
        public UserSerializer(BaseUser authorizeUser, BaseUser entity) : base(authorizeUser, entity)
        {
            if (authorizeUser == null)
            {
                IsOwner = false;
                IsAdmin = false;
            }
            else
            {
                IsOwner = authorizeUser.Id == entity.Id;
                IsAdmin = authorizeUser.Type == UserType.Administrator;
            }

        }

        public Dictionary<string, object> Serialize()
        {
            var userInfo = new Dictionary<string, object>();

            userInfo.Add("id", entity.Id);
            userInfo.Add("fullname", entity.FullName);
            userInfo.Add("birthday", entity.BirthDate);
            userInfo.Add("phone", entity.PhoneNumber);
            userInfo.Add("image_url", entity.ImageURL);
            userInfo.Add("role", entity.Type.ToString());

            if (IsOwner || IsAdmin) userInfo.Add("username", entity.UserName);
            if (IsOwner || IsAdmin) userInfo.Add("email", entity.Email);
            if (IsOwner || IsAdmin) userInfo.Add("facebook_id", entity.FbConnectedId);

            return userInfo;
        }
    }
}
