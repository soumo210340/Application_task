using System.Linq;
using System.Threading.Tasks;
using Application_task.Data;
using Application_task.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application_task.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetStates(string term)
        {
            var list = await _db.States
                .Where(s => string.IsNullOrWhiteSpace(term) || s.Name.Contains(term))
                .OrderBy(s => s.Name)
                .Select(s => new { id = s.Id, text = s.Name })
                .ToListAsync();

            return Json(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserData model)
        {
            if (ModelState.ContainsKey("Id")) ModelState.Remove("Id");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _db.UserDatas.Add(model);
            await _db.SaveChangesAsync();
            return Ok(new { success = true, id = model.Id });
        }

        [HttpGet]
        public async Task<JsonResult> List()
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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.UserDatas.FindAsync(id);
            if (item == null) return NotFound("User not found.");

            _db.UserDatas.Remove(item);
            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] UserData model)
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

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _db.UserDatas.FindAsync(id);
            if (item == null) return NotFound(new { error = "User not found." });

            return Json(item);
        }
    }
}
