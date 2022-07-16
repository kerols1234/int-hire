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
            try
            {
                var jobs = _db.Jobs.Include(obj => obj.Tags).Include(obj => obj.RecruiterUser).ToList();
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
                    companyId = obj.RecruiterUser.Company == null ? 0 : obj.RecruiterUser.Company.Id,
                    tags = obj.Tags.Select(t => t.Name).ToList(),
                    recruiterEmail = obj.RecruiterUser.Email,
                    numberOfApplicants = _db.JobApplicants.Where(obj1 => obj1.JobId == obj.Id).Count(),
                }).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
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
                companyId = obj.RecruiterUser.Company == null ? 0 : obj.RecruiterUser.Company.Id,
                tags = obj.Tags.Select(t => t.Name).ToList(),
                recruiterEmail = obj.RecruiterUser.Email,
                numberOfApplicants = _db.JobApplicants.Where(obj1 => obj1.JobId == obj.Id).Count(),
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
                companyId = obj.RecruiterUser.Company == null ? 0 : obj.RecruiterUser.Company.Id,
                tags = obj.Tags.Select(t => t.Name).ToList(),
                recruiterEmail = obj.RecruiterUser.Email,
                numberOfApplicants = _db.JobApplicants.Where(obj1 => obj1.JobId == obj.Id).Count(),
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
                companyId = obj.RecruiterUser.Company == null ? 0 : obj.RecruiterUser.Company.Id,
                tags = obj.Tags.Select(t => t.Name).ToList(),
                recruiterEmail = obj.RecruiterUser.Email,
                numberOfApplicants = _db.JobApplicants.Where(obj1 => obj1.JobId == obj.Id).Count(),
            }).ToList());
        }

        [HttpGet]
        [Authorize(Roles = "Applicant")]
        public IActionResult getApplicantJobs()
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var user = _db.ApplicantUsers.FirstOrDefault(obj => obj.Email == claim.Value);
            if (user == null)
            {
                return BadRequest("Wrong User email");
            }

            return Ok(_db.Jobs.Include(obj => obj.Tags).Include(obj => obj.RecruiterUser).Where(obj => _db.JobApplicants.Any(obj1 => obj1.JobId == obj.Id && obj1.ApplicantUserId == user.Id)).Select(obj => new
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
                companyId = obj.RecruiterUser.Company == null ? 0 : obj.RecruiterUser.Company.Id,
                tags = obj.Tags.Select(t => t.Name).ToList(),
                recruiterEmail = obj.RecruiterUser.Email,
                numberOfApplicants = _db.JobApplicants.Where(obj1 => obj1.JobId == obj.Id).Count(),
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
                    tag = new Tag { Name = item };
                    _db.Tags.Add(tag);
                }
                newJob.Tags.Add(tag);
            }

            _db.Jobs.Add(newJob);
            await _db.SaveChangesAsync();
            return Ok(new { id = newJob.Id });
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

        [HttpPost]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> applyToJob([FromBody] int id)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var user = _db.ApplicantUsers.FirstOrDefault(obj => obj.Email == claim.Value);
            if (user == null)
            {
                return BadRequest("Wrong User email");
            }

            if (!_db.Jobs.Any(obj => obj.Id == id))
            {
                return BadRequest("No job whith this Id");
            }

            if (_db.JobApplicants.Any(obj => obj.ApplicantUserId == user.Id && obj.JobId == _db.Jobs.FirstOrDefault(obj => obj.Id == id).Id))
            {
                return BadRequest("already applied to this job");
            }

            var application = new JobApplicant
            {
                TestPersonality = user.TestPersonality,
                TweeterPersonality = user.TwitterPersonality,
                JobId = id,
                ApplicantUserId = user.Id
            };

            _db.JobApplicants.Add(application);
            await _db.SaveChangesAsync();
            return Ok("Done successfully");
        }

        [HttpGet]
        [Authorize(Roles = "Recruiter")]
        public IActionResult getInformationByJobId(int id)
        {
            if (!_db.Jobs.Any(obj => obj.Id == id))
            {
                return BadRequest("No job whith this Id");
            }
            
            return Ok(_db.JobApplicants.Where(obj => obj.JobId == id).Include(obj => obj.ApplicantUser).Include(obj => obj.ApplicantUser.Tags).Select(obj => new
            {
                userType = "applicant",
                obj.ApplicantUser.Email,
                obj.ApplicantUser.Name,
                obj.ApplicantUser.PhoneNumber,
                obj.ApplicantUser.BirthDay,
                obj.ApplicantUser.Gender,
                obj.ApplicantUser.MilitaryStatus,
                obj.ApplicantUser.EducationLevel,
                obj.ApplicantUser.City,
                obj.ApplicantUser.Street,
                obj.ApplicantUser.Country,
                obj.ApplicantUser.TwitterUsername,
                obj.ApplicantUser.skills,
                obj.ApplicantUser.TestPersonality,
                obj.ApplicantUser.TwitterPersonality,
                tags = obj.ApplicantUser.Tags.Select(obj => obj.Name).ToList()
            }).ToList());
        }

    }
}
