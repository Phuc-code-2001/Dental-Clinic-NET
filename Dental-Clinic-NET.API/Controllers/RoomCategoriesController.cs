using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataLayer.DataContexts;
using DataLayer.Domain;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomCategoriesController : ControllerBase
    {
        AppDbContext _context;

        public RoomCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/RoomCategories
        [HttpGet]
        public async Task<IActionResult> GetRoomCategories()
        {
            try
            {
                List<RoomCategory> dataset = await _context.RoomCategories.ToListAsync();
                return Ok(dataset);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/RoomCategories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomCategory(int id)
        {
            try
            {
                var roomCategory = await _context.RoomCategories.FindAsync(id);

                if (roomCategory == null)
                {
                    return NotFound();
                }

                return Ok(roomCategory);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/RoomCategories
        [HttpPost]
        public async Task<IActionResult> PostRoomCategory(RoomCategory roomCategory)
        {
            try
            {
                if(_context.RoomCategories.Any(x => x.Name == roomCategory.Name))
                {
                    return BadRequest($"The Name '{roomCategory.Name}' already exist!");
                }
                _context.RoomCategories.Add(roomCategory);
                await _context.SaveChangesAsync();

                return Ok(roomCategory);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/RoomCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomCategory(int id)
        {
            try
            {
                var roomCategory = await _context.RoomCategories.FindAsync(id);
                if (roomCategory == null)
                {
                    return NotFound();
                }

                _context.RoomCategories.Remove(roomCategory);
                await _context.SaveChangesAsync();

                return Ok("Remove succeeded!");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
