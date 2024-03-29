﻿using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Rooms;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private ServicesManager _servicesManager;

        public RoomController(ServicesManager servicesManager)
        {
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
        public IActionResult GetAll([FromQuery] RoomFilter filter)
        {
            try
            {
                IQueryable<Room> queries = _servicesManager.DbContext.Rooms
                    .Include(r => r.RoomCategory);
                queries = filter.GetFilteredQuery(queries);
                Paginated<Room> paginated = new Paginated<Room>(queries, filter.Page, filter.PageSize);

                var dataset = paginated.GetData(items => _servicesManager.AutoMapper.Map<RoomDTOLite[]>(items.ToArray()));

                return Ok(dataset);
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

                // Check room categories
                RoomCategory cate = _servicesManager.DbContext.RoomCategories.FirstOrDefault(x => x.Name == request.Category);
                if(cate == null)
                {
                    cate = new RoomCategory()
                    {
                        Name = request.Category,
                    };
                }
                else
                {
                    room.RoomCategory = cate;
                }
                room.RoomCategory = cate;

                // Check room code
                bool duplicateRoomCode = _servicesManager.DbContext.Rooms.Any(r => r.RoomCode == room.RoomCode);
                if (duplicateRoomCode) 
                    return BadRequest("Duplicate room code");

                _servicesManager.DbContext.Rooms.Add(room);
                _servicesManager.DbContext.SaveChanges();

                room = _servicesManager.DbContext.Rooms
                    .Include(r => r.Devices)
                    .Include(r => r.RoomCategory)
                    .FirstOrDefault(r => r.Id == room.Id);

                RoomDTO roomDTO = _servicesManager.AutoMapper.Map<RoomDTO>(room);

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
                Room room = _servicesManager.DbContext.Rooms
                    .Include(r => r.Devices)
                    .Include(r => r.RoomCategory)
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
                Room room = _servicesManager.DbContext.Rooms.Find(id);
                if (room == null)
                {
                    return NotFound("Room not found");
                }

                _servicesManager.DbContext.Entry(room).State = EntityState.Deleted;
                _servicesManager.DbContext.SaveChanges();

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
                Room room = _servicesManager.DbContext.Rooms.Find(request.Id);
                if (room == null)
                {
                    return NotFound("Room not found");
                }

                // Check room categories
                RoomCategory cate = _servicesManager.DbContext.RoomCategories.FirstOrDefault(x => x.Name == request.Category);
                if (cate == null)
                {
                    cate = new RoomCategory()
                    {
                        Name = request.Category,
                    };
                }
                else
                {
                    room.RoomCategory = cate;
                }
                room.RoomCategory = cate;

                bool duplicatedCode = _servicesManager.DbContext.Rooms
                    .Any(r => r.Id != room.Id && r.RoomCode == request.RoomCode);
                
                if(duplicatedCode)
                {
                    return BadRequest($"RoomCode '{request.RoomCode}' have already exist!");
                }

                _servicesManager.AutoMapper.Map<UpdateRoom, Room>(request, room);

                _servicesManager.DbContext.Entry(room).State = EntityState.Modified;
                _servicesManager.DbContext.SaveChanges();

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
