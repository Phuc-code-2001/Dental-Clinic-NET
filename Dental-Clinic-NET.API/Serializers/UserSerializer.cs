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
                isOwner = false;
                isAdmin = false;
            }
            else
            {
                isOwner = authorizeUser.Id == entity.Id;
                isAdmin = authorizeUser.Type == UserType.Administrator;
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
            userInfo.Add("permission", entity.Type.ToString());

            if (isOwner || isAdmin) userInfo.Add("username", entity.UserName);
            if (isOwner || isAdmin) userInfo.Add("email", entity.Email);
            if (isOwner || isAdmin) userInfo.Add("facebook_id", entity.FbConnectedId);

            return userInfo;
        }
    }
}
