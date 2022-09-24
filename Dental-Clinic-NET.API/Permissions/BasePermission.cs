using DataLayer.Schemas;

namespace Dental_Clinic_NET.API.Permissions
{

    public class BasePermission<T>
    {
        public delegate bool HandleOwnerSelector();

        public bool IsAuthenticated { get; protected set; } = false;
        public bool IsOwner { get; protected set; } = false;
        public bool IsAdmin { get; protected set; } = false;

        public BaseUser LoggedUser;
        public T Entity;

        public BasePermission() { }

        public BasePermission(BaseUser loggedUser, T entity)
        {
            if (loggedUser == null) return;

            IsAuthenticated = true;
            IsAdmin = loggedUser.Type == UserType.Administrator;

            LoggedUser = loggedUser;
            Entity = entity;
        }

        public BasePermission<T> HandleOwnerPermission(HandleOwnerSelector handler)
        {
            IsOwner = handler();
            return this;
        }

    }
}
