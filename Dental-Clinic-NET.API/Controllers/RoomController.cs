using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Contacts;
using Dental_Clinic_NET.API.Models.Rooms;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private AppDbContext _context;
        private ServicesManager _servicesManager;
        public RoomController(AppDbContext context, ServicesManager servicesManager)
        {
            _context = context;
            _servicesManager = servicesManager;
        }
        /// <summary>
        ///     List all rooms by admin
        /// </summary>
        /// <returns>
        ///     200: Request success
        ///     500: Server Handle Error
        /// </returns>
        [HttpGet]
        public IActionResult GetAll(int page = 1)
        {
            try
            {
                var rooms = _context.Rooms
                    .Include(r => r.Devices);
                Paginated<Room> paginatedRooms = new Paginated<Room>(rooms, page);

                RoomDTO[] roomDTOs = _servicesManager.AutoMapper.Map<RoomDTO[]>(paginatedRooms.Items.ToArray());

                return Ok(new
                {
                    page = page,
                    per_page = paginatedRooms.PageSize,
                    total = paginatedRooms.QueryCount,
                    total_pages = paginatedRooms.PageCount,
                    data = roomDTOs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Create new room from any actor
        /// </summary>
        /// <param name="request">Room Info</param>
        /// <returns>
        ///     200: Create success
        ///     500: Server handle error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(CreateRoom request)
        {
            try
            {
                Room room = _servicesManager.AutoMapper.Map<Room>(request);

                // Check room code
                bool duplicateRoomCode = _context.Rooms.Any(r => r.RoomCode == room.RoomCode);
                if (duplicateRoomCode) 
                    return BadRequest("Duplicate room code");

                _context.Rooms.Add(room);
                _context.SaveChanges();

                room = _context.Rooms
                    .Include(r => r.Devices)
                    .FirstOrDefault(r => r.Id == room.Id);

                RoomDTO roomDTO = _servicesManager.AutoMapper.Map<RoomDTO>(room);

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Room-Create", room, result =>
                    {

                    });


                return Ok(roomDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        ///     Get a Room details
        /// </summary>
        /// <param name="id">room id</param>
        /// <returns>
        ///     200: Request success
        ///     500: Server handle error
        /// </returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Room room = _context.Rooms
                    .Include(r => r.Devices)
                    .FirstOrDefault(r => r.Id == id);

                if (room == null) return NotFound("Room not found.");

                RoomDTO roomDTO = _servicesManager.AutoMapper.Map<RoomDTO>(room);

                return Ok(roomDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        ///     Remove room out of database
        /// </summary>
        /// <param name="id">room id</param>
        /// <returns>
        ///     200: Request success
        ///     500: Server handle error
        /// </returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            try
            {
                Room room = _context.Rooms.Find(id);
                if (room == null)
                {
                    return NotFound("Room not found");
                }

                _context.Entry(room).State = EntityState.Deleted;
                _context.SaveChanges();

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Room-Delete", new { Id = room.Id }, result =>
                    {
                        Console.WriteLine("Push event done at: " + DateTime.Now);
                    });

                return Ok($"You just have completely delete room with id='{id}' success");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        ///     Update room info
        /// </summary>
        /// <param name="request">new room info</param>
        /// <returns>
        ///     200: Success
        ///     404: Not found room
        ///     500: Server handle error
        /// </returns>
        [HttpPut]
        public IActionResult Update(UpdateRoom request)
        {
            try
            {
                Room room = _context.Rooms.Find(request.Id);
                if (room == null)
                {
                    return NotFound("Room not found");
                }

                _servicesManager.AutoMapper.Map<UpdateRoom, Room>(request, room);

                _context.Entry(room).State = EntityState.Modified;
                _context.SaveChanges();

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Room-Update", room, result =>
                    {
                        Console.WriteLine("Push event done at: " + DateTime.Now);
                    });

                RoomDTO roomDTO = _servicesManager.AutoMapper.Map<RoomDTO>(room);

                return Ok(roomDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
