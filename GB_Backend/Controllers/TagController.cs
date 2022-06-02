using GB_Backend.Data;
using GB_Backend.Models;
using GB_Backend.Models.APIforms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GB_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors]
    [Authorize]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TagController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult getAllTags()
        {
            return Ok(_db.Tags.Include(obj => obj.Jobs).Include(obj => obj.ApplicantUsers).Select(obj => new
            {
                name = obj.Name,
                countOfJobs = obj.Jobs.Count,
                countOfApplicants = obj.ApplicantUsers.Count
            }).ToList());
        }

        [HttpGet]
        public IActionResult getTagByName(string name)
        {
            var tag = _db.Tags.Include(obj => obj.Jobs).Include(obj => obj.ApplicantUsers).FirstOrDefault(c => c.Name == name);

            if (tag == null)
                return BadRequest("No tag whith this name");

            return Ok(new
            {
                name = tag.Name,
                countOfJobs = tag.Jobs.Count,
                countOfApplicants = tag.ApplicantUsers.Count
            });
        }

        [HttpPost]
        public IActionResult AddTag([FromBody] TagesForm tages)
        {
            foreach (var item in tages.Tags)
            {
                if (!_db.Tags.Any(obj => obj.Name == item))
                {
                    Tag tag = new Tag() { Name = item };
                    var result = _db.Tags.Add(tag);
                }
            }

            _db.SaveChanges();

            return Ok("Operation is done");
        }

        [HttpDelete]
        public IActionResult Delete(string name)
        {
            var obj = _db.Tags.FirstOrDefault(obj => obj.Name == name);

            if (obj == null)
            {
                return BadRequest("No tag with this name");
            }

            _db.Tags.Remove(obj);

            _db.SaveChanges();

            return Ok("Operation is done");
        }
    }
}
