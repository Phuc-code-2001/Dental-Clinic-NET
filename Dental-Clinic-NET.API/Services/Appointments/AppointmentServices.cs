using DataLayer.DataContexts;
using DataLayer.Domain;
using DataLayer.Extensions;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder.Core.V1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dental_Clinic_NET.API.Services.Appointments
{
    public class AppointmentServices
    {

        AppDbContext _context;

        public AppointmentServices(AppDbContext context)
        {
            _context = context;
        }

        public bool CanRead(Appointment entity, BaseUser user)
        {
            var permission = new PermissionOnAppointment(user, entity);
            bool c1 = permission.IsAdmin;
            bool c2 = permission.IsOwner;
            bool c3 = permission.LoggedUser.Type == UserType.Receptionist;
            return c1 || c2 || c3;
        }

        public bool CanWrite(Appointment entity, BaseUser user)
        {
            var permission = new PermissionOnAppointment(user, entity);

            switch (user.Type)
            {
                case UserType.Patient:
                    // Only allowed in upload document
                    return permission.IsOwner;

                case UserType.Doctor:
                    return permission.IsOwner
                        && (entity.State == Appointment.States.Accept
                        || entity.State == Appointment.States.Doing);

                case UserType.Receptionist:
                    return entity.State == Appointment.States.NotYet
                        || entity.State == Appointment.States.Accept;

                case UserType.Technican:
                    return entity.State == Appointment.States.Transfer
                        || entity.State == Appointment.States.TransferDoing
                        || entity.State == Appointment.States.TransferCancel
                        || entity.State == Appointment.States.TransferComplete;

                case UserType.Administrator:
                    return true;
            }

            return false;

        }

        public bool CanUpdateState(Appointment entity, BaseUser user, Appointment.States newState)
        {
            bool checkPermission = CanWrite(entity, user);
            if(checkPermission)
            {
                switch(user.Type)
                {
                    case UserType.Patient:
                        // Only allowed in upload document
                        return false;

                    case UserType.Receptionist:
                        return newState == Appointment.States.Accept
                            || newState == Appointment.States.Cancel
                            || newState == Appointment.States.NotYet;
                                        
                    case UserType.Doctor:
                        return newState == Appointment.States.Complete
                            || newState == Appointment.States.Doing
                            || newState == Appointment.States.Transfer;

                    case UserType.Technican:
                        return newState == Appointment.States.Transfer
                            || newState == Appointment.States.TransferCancel
                            || newState == Appointment.States.TransferDoing
                            || newState == Appointment.States.TransferComplete;

                    case UserType.Administrator:
                        return true;
                }


            }

            return checkPermission;
        }

        public Doctor FindDoctorForAppointment(Appointment appointment)
        {
            List<Doctor> busyDoctors = _context.Appointments
                .Include(apm => apm.Doctor)
                .Where(apm => apm.Date == appointment.Date && apm.Slot == appointment.Slot)
                .Select(apm => apm.Doctor).ToList();

            Doctor[] doctors = _context.Doctors.AsEnumerable()
                .Except(busyDoctors)
                .ToArray();

            if(doctors.Length > 0)
            {
                int randIndex = (new Random()).Next(doctors.Length);
                Doctor doctorSelected = doctors[randIndex];
                return doctorSelected;
            }
            else
            {
                throw new Exception("No have any Doctor!");
            }
        }

        public Room FindRoomForAppointment(Appointment appointment)
        {
            List<Room> busyRooms = _context.Appointments
                .Include(apm => apm.Room)
                .Where(apm => apm.Date == appointment.Date && apm.Slot == appointment.Slot)
                .Select(apm => apm.Room).ToList();

            Room[] rooms = _context.Rooms.AsEnumerable()
                .Where(r => r.RoomType == Room.RoomTypes.GeneralRoom)
                .Except(busyRooms)
                .ToArray();
            if(rooms.Length > 0)
            {
                int randIndex = (new Random()).Next(rooms.Length);
                Room roomSelected = rooms[randIndex];
                return roomSelected;
            }
            else
            {
                throw new Exception("No have any Room!");
            }
        }

        public bool IsFreeTime(DateTime Date, TimeManager.SlotManager slot)
        {
            throw new NotImplementedException();
        }
    }
}
