using DataLayer.Domain;

namespace Dental_Clinic_NET.API.Permissions
{
    public class PermissionOnAppointment : BasePermission<Appointment>
    {

        public PermissionOnAppointment(BaseUser loggedUser, Appointment entity) : base(loggedUser, entity)
        {
            HandleOwnerPermission((entity) =>
            {
                bool c1 = entity.PatientId == loggedUser.Id;
                bool c2 = entity.DoctorId == loggedUser.Id && entity.State > Appointment.States.NotYet;
                return c1 || c2;
            });
        }

    }
}
