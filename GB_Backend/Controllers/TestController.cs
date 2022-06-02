using GB_Backend.Data;
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
    // [Authorize]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TestController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult getInformationByJobId(int id)
        {
            return Ok(_db.JobApplicants.Where(obj => obj.JobId == id).Include(obj => obj.MBTIType).Include(obj => obj.ApplicantUser).Include(obj => obj.Job).Select(obj => new
            {
                userEmail = obj.ApplicantUser.Email,
                obj.MBTIType,
                obj.Job,
            }).ToList());
        }

        [HttpGet]
        public IActionResult getTestQuestions()
        {
            return Ok(_db.Tests.Select(obj => new
            {
                obj.Id,
                obj.Question,
                obj.AnswerA,
                obj.AnswerB,
            }).ToList());
        }

        [HttpPost]
        [Authorize(Roles = "Applicant")]
        public IActionResult postAnswer([FromBody] TestForm testForm)
        {
            if (testForm.Answers.Count != 56)
            {
                return BadRequest("Number of answers must be 56");
            }

            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var applicant = _db.ApplicantUsers.Include(obj => obj.Tags).FirstOrDefault(obj => obj.Email == claim.Value);
            if (applicant == null)
            {
                return BadRequest("Wrong User email");
            }

            if (!_db.Jobs.Any(obj => obj.Id == testForm.JobId))
            {
                return BadRequest("Wrong Job Id");
            }

            int[] counts = new int[4];
            string type = "ESTP";

            foreach (var item in testForm.Answers)
            {
                var testQuestion = _db.Tests.Where(obj => obj.Id == item.Id).FirstOrDefault();
               
                if (testQuestion == null)
                {
                    return BadRequest("Wrong Question Id");
                }
                
                if (testQuestion.AnswerA == item.answer)
                {
                    counts[item.Id % 4]++;
                }
                else
                {
                    counts[item.Id % 4]--;
                }
            }

            if (counts[1] <= 0)
            {
                type = type.Replace('E', 'I');
            }
            if (counts[2] <= 0)
            {
                type = type.Replace('S', 'N');
            }
            if (counts[3] <= 0)
            {
                type = type.Replace('T', 'F');
            }
            if (counts[0] < 0)
            {
                type = type.Replace('P', 'J');
            }

            /*
             *Api for tweeter
             *Api for machine model
             *combine all to get on type of user
            */
            
            return Ok(new { counts = counts, type = type });
        }
    }
}
