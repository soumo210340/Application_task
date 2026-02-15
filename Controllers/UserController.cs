using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application_task.Data;
using Application_task.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application_task.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext db, ILogger<UserController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetStates(string term)
        {
            try
            {
                var list = await _db.States
                    .Where(s => string.IsNullOrWhiteSpace(term) || s.Name.Contains(term))
                    .OrderBy(s => s.Name)
                    .Select(s => new { id = s.Id, text = s.Name })
                    .ToListAsync();

                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching states");
                return Json(new List<object>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserData model)
        {
            try
            {
                if (ModelState.ContainsKey("Id")) ModelState.Remove("Id");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _db.UserDatas.Add(model);
                await _db.SaveChangesAsync();
                return Ok(new { success = true, id = model.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "Unable to save user right now.");
            }
        }

        [HttpGet]
        public async Task<JsonResult> List()
        {
            try
            {
                var data = await _db.UserDatas
                    .OrderByDescending(u => u.Id)
                    .Select(u => new
                    {
                        u.Id,
                        u.Name,
                        u.Email,
                        u.MobileNo,
                        u.Address,
                        u.Gender,
                        u.State,
                        u.Hobbies
                    })
                    .ToListAsync();

                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing users");
                return Json(new List<object>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _db.UserDatas.FindAsync(id);
                if (item == null) return NotFound("User not found.");

                _db.UserDatas.Remove(item);
                await _db.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, "Unable to delete user right now.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] UserData model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var existing = await _db.UserDatas.FindAsync(model.Id);
                if (existing == null) return NotFound("User not found.");

                existing.Name = model.Name;
                existing.Email = model.Email;
                existing.MobileNo = model.MobileNo;
                existing.Address = model.Address;
                existing.Gender = model.Gender;
                existing.State = model.State;
                existing.Hobbies = model.Hobbies;

                await _db.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, "Unable to update user right now.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var item = await _db.UserDatas.FindAsync(id);
                if (item == null) return NotFound(new { error = "User not found." });

                return Json(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching details");
                return StatusCode(500, new { error = "Unable to fetch user details right now." });
            }
        }
    }
}
