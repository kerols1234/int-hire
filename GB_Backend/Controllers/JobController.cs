using GB_Backend.Data;
using GB_Backend.Models;
using GB_Backend.Models.APIforms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GB_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors]
    [Authorize]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly AppDbContext _db;

        public JobController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult getAllJobs()
        {
            return Ok(_db.Jobs.Include(obj => obj.Tags).Include(obj => obj.RecruiterUser).Select(obj => new
            {
                obj.Id,
                obj.Title,
                obj.Description,
                obj.Salary,
                obj.ExpLevel,
                obj.EducationLevel,
                obj.Career,
                obj.JobType,
                obj.Requirements,
                deadline = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.Deadline, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                postingTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.Posting_Time, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                active = obj.Deadline.CompareTo(DateTime.Now) > 0,
                companyId = obj.RecruiterUser.Company.Id,
                tags = obj.Tags.Select(t => t.Name).ToList(),
                recruiterEmail = obj.RecruiterUser.Email
            }).ToList());
        }

        [HttpGet]
        public IActionResult getJobById(int id)
        {
            var job = _db.Jobs.Include(obj => obj.Tags).Include(obj => obj.RecruiterUser).Select(obj => new
            {
                obj.Id,
                obj.Title,
                obj.Description,
                obj.Salary,
                obj.ExpLevel,
                obj.EducationLevel,
                obj.Career,
                obj.JobType,
                obj.Requirements,
                deadline = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.Deadline, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                postingTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.Posting_Time, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                active = obj.Deadline.CompareTo(DateTime.Now) > 0,
                companyId = obj.RecruiterUser.Company.Id,
                tags = obj.Tags.Select(t => t.Name).ToList(),
                recruiterEmail = obj.RecruiterUser.Email
            }).FirstOrDefault(j => j.Id == id);

            if (job == null)
                return BadRequest("No job whith this Id");

            return Ok(job);
        }

        [HttpGet]
        public IActionResult getActiveJobs()
        {
            return Ok(_db.Jobs.Where(obj => obj.Deadline.CompareTo(DateTime.Now) > 0).Include(obj => obj.Tags).Include(obj => obj.RecruiterUser).Select(obj => new
            {
                obj.Id,
                obj.Title,
                obj.Description,
                obj.Salary,
                obj.ExpLevel,
                obj.EducationLevel,
                obj.Career,
                obj.JobType,
                obj.Requirements,
                deadline = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.Deadline, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                postingTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.Posting_Time, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                active = obj.Deadline.CompareTo(DateTime.Now) > 0,
                companyId = obj.RecruiterUser.Company.Id,
                tags = obj.Tags.Select(t => t.Name).ToList(),
                recruiterEmail = obj.RecruiterUser.Email
            }).ToList());
        }

        [HttpGet]
        [Authorize(Roles = "Recruiter")]
        public IActionResult getRecruiterJobs()
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var user = _db.RecruiterUsers.FirstOrDefault(obj => obj.Email == claim.Value);
            if (user == null)
            {
                return BadRequest("Wrong User email");
            }

            return Ok(_db.Jobs.Include(obj => obj.Tags).Include(obj => obj.RecruiterUser).Where(obj => obj.RecruiterUserId == user.Id).Select(obj => new
            {
                obj.Id,
                obj.Title,
                obj.Description,
                obj.Salary,
                obj.ExpLevel,
                obj.EducationLevel,
                obj.Career,
                obj.JobType,
                obj.Requirements,
                deadline = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.Deadline, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                postingTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(obj.Posting_Time, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                active = obj.Deadline.CompareTo(DateTime.Now) > 0,
                companyId = obj.RecruiterUser.Company.Id,
                tags = obj.Tags.Select(t => t.Name).ToList(),
                recruiterEmail = obj.RecruiterUser.Email
            }).ToList());
        }

        [HttpPost]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> addJob([FromBody] JobForm job)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var user = _db.RecruiterUsers.FirstOrDefault(obj => obj.Email == claim.Value);
            if (user == null)
            {
                return BadRequest("Wrong User email");
            }

            var newJob = new Job
            {
                Title = job.Title,
                Description = job.Description,
                Salary = job.Salary,
                ExpLevel = job.ExpLevel,
                EducationLevel = job.EducationLevel,
                Career = job.Career,
                JobType = job.JobType,
                Requirements = job.Requirements,
                Deadline = DateTime.ParseExact(job.Deadline, "dd-MM-yyyy hh:mm tt", null),
                Posting_Time = DateTime.Now,
                RecruiterUserId = user.Id,
                Tags = new List<Tag>()
            };

            foreach (var item in job.Tags)
            {
                Tag tag = _db.Tags.FirstOrDefault(obj => obj.Name == item);
                if (tag == null)
                {
                    newJob.Tags.Add(tag);
                    tag = new Tag { Name = item };
                }
                newJob.Tags.Add(tag);
            }

            _db.Jobs.Add(newJob);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> updateJob([FromBody] UpdateJob job)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var user = _db.RecruiterUsers.FirstOrDefault(obj => obj.Email == claim.Value);
            if (user == null)
            {
                return BadRequest("Wrong User email");
            }

            var jobToUpdate = _db.Jobs.Include(obj => obj.Tags).FirstOrDefault(obj => obj.Id == job.Id);
            if (jobToUpdate == null)
            {
                return BadRequest("No job whith this Id");
            }

            jobToUpdate.Title = job.Title ?? jobToUpdate.Title;
            jobToUpdate.Description = job.Description ?? jobToUpdate.Description;
            jobToUpdate.Salary = job.Salary == 0 ? jobToUpdate.Salary : job.Salary;
            jobToUpdate.ExpLevel = job.ExpLevel == 0 ? jobToUpdate.ExpLevel : job.ExpLevel;
            jobToUpdate.EducationLevel = job.EducationLevel ?? jobToUpdate.EducationLevel;
            jobToUpdate.Career = job.Career ?? jobToUpdate.Career;
            jobToUpdate.JobType = job.JobType ?? jobToUpdate.JobType;
            jobToUpdate.Requirements = job.Requirements ?? jobToUpdate.Requirements;
            jobToUpdate.Deadline = job.Deadline != null ? DateTime.ParseExact(job.Deadline, "dd-MM-yyyy hh:mm tt", null) : jobToUpdate.Deadline;

            foreach (var item in job.Tags)
            {
                Tag tag = _db.Tags.FirstOrDefault(obj => obj.Name == item);
                if (tag == null)
                {
                    tag = new Tag { Name = item };
                    jobToUpdate.Tags.Add(tag);
                }
                else if (!jobToUpdate.Tags.Any(obj => obj.Name == item))
                {
                    jobToUpdate.Tags.Add(tag);
                }
            }
            foreach (var item in jobToUpdate.Tags)
            {
                if (!job.Tags.Any(obj => obj == item.Name))
                {
                    jobToUpdate.Tags.Remove(item);
                }
            }

            _db.Jobs.Update(jobToUpdate);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> deleteJob(int id)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var user = _db.RecruiterUsers.FirstOrDefault(obj => obj.Email == claim.Value);
            if (user == null)
            {
                return BadRequest("Wrong User email");
            }

            var jobToDelete = _db.Jobs.FirstOrDefault(obj => obj.Id == id);
            if (jobToDelete == null)
            {
                return BadRequest("No job whith this Id");
            }

            _db.Jobs.Remove(jobToDelete);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> DisActiveJob(int id)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var user = _db.RecruiterUsers.FirstOrDefault(obj => obj.Email == claim.Value);
            if (user == null)
            {
                return BadRequest("Wrong User email");
            }

            var jobToDisActive = _db.Jobs.FirstOrDefault(obj => obj.Id == id);
            if (jobToDisActive == null)
            {
                return BadRequest("No job whith this Id");
            }

            jobToDisActive.Deadline = DateTime.Now;
            _db.Jobs.Update(jobToDisActive);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
