using DataLayer.Schemas;
using System.Collections.Generic;
using System.Security.Claims;

namespace Dental_Clinic_NET.API.Serializers
{
    public abstract class BaseSerializer<T>
    {
        public bool IsOwner { get; protected set; }
        public bool IsAdmin { get; protected set; }

        protected T entity;

        protected UserType authorizeRole;

        public BaseSerializer(BaseUser authorizeUser, T entity)
        {
            this.entity = entity;
            IsAdmin = authorizeUser?.Type == UserType.Administrator;
            authorizeRole = authorizeUser.Type;
        }

    }
}
