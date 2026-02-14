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
    // Renamed from UserController2 to UserController so routing with {controller=User} works.
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
            // Return the conventional view so Views/User/Index.cshtml is used.
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetStates(string term)
        {
            try
            {
                var list = await _db.States
                    .Where(s => string.IsNullOrEmpty(term) || s.Name.Contains(term))
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
                // Remove Id modelstate entry because form may send empty string which doesn't bind to int
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
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<JsonResult> List()
        {
            try
            {
                var items = await _db.UserDatas.ToListAsync();
                var data = items.Select(u => new
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    MobileNo = u.MobileNo,
                    Address = u.Address,
                    Gender = u.Gender,
                    State = u.State,
                    Hobbies = u.Hobbies
                }).ToList();

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
                if (item == null) return NotFound();
                _db.UserDatas.Remove(item);
                await _db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] UserData model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var existing = await _db.UserDatas.FindAsync(model.Id);
                if (existing == null) return NotFound();
                existing.Name = model.Name;
                existing.Email = model.Email;
                existing.MobileNo = model.MobileNo;
                existing.Address = model.Address;
                existing.Gender = model.Gender;
                existing.State = model.State;
                existing.Hobbies = model.Hobbies;
                await _db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<JsonResult> Details(int id)
        {
            try
            {
                var item = await _db.UserDatas.FindAsync(id);
                if (item == null) return Json(new { });
                return Json(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching details");
                return Json(new { });
            }
        }
    }
}
