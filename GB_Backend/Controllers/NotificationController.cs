using GB_Backend.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GB_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors]
    //[Authorize]
    [ApiController]
    public class NotificationController : ControllerBase
    {/*
        private readonly AppDbContext _db;

        public NotificationController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult getAllCompanies()
        {
            return Ok(_db.Companies.ToList());
        }

        [HttpGet]
        public IActionResult getCompanyById(int id)
        {
            var company = _db.Companies.FirstOrDefault(c => c.Id == id);

            if (company == null)
                return BadRequest("No company whith this Id");

            return Ok(company);
        }

        [HttpPost]
        public IActionResult AddCompany([FromBody] Company company)
        {
            company.Name = company.Name.ToLower();
            if (_db.Companies.Any(obj => obj.Name == company.Name))
                return BadRequest("This company name already exists");

            _db.Companies.Add(company);
            _db.SaveChanges();

            return Ok("Operation is done");
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public IActionResult Edit([FromBody] Company company)
        {
            company.Name = company.Name.ToLower();

            if (!_db.Companies.Any(obj => obj.Id == company.Id))
                return BadRequest("No company whith this Id");

            if (_db.Companies.Any(obj => obj.Name == company.Name && obj.Id != company.Id))
                return BadRequest("This company name already exists");

            _db.Companies.Update(company);
            _db.SaveChanges();

            return Ok("Operation is done");
        }

        [HttpDelete]
        //[Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var obj = _db.Companies.FirstOrDefault(obj => obj.Id == id);

            if (obj == null)
            {
                return BadRequest("No company with this id");
            }

            var result = _db.Companies.Remove(obj);

            _db.SaveChanges();

            return Ok("Operation is done");
        }   */     
    }
}
