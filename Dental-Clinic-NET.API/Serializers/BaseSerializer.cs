using DataLayer.Schemas;
using System.Collections.Generic;
using System.Security.Claims;

namespace Dental_Clinic_NET.API.Serializers
{
    public abstract class BaseSerializer<T>
    {
        protected bool isOwner;
        protected bool isAdmin;

        protected T entity;

        protected UserType authorizeRole;

        public BaseSerializer(BaseUser authorizeUser, T entity)
        {
            this.entity = entity;
            authorizeRole = authorizeUser.Type;
        }

    }
}
