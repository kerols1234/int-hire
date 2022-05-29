using GB_Backend.Data;
using GB_Backend.Models.APIforms;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using GB_Backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace GB_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(AppDbContext db, IConfiguration configuration, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        /*
         [HttpPost]
         [Authorize]
         public async Task<IActionResult> UpdateUserInfo([FromBody] UserModel model)
         {
             var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
             if (claim == null)
             {
                 return BadRequest("Wrong User email");
             }

             var user = _userManager.Users.FirstOrDefault(obj => obj.Email == claim.Value);

             if (user == null)
             {
                 return BadRequest("No user with this email");
             }

             if (model.PhoneNumber != null && model.PhoneNumber.Trim() != "")
             {
                 if (_userManager.Users.Any(obj => obj.Email != user.Email && obj.PhoneNumber == model.PhoneNumber))
                 {
                     return BadRequest("Phone number already exists!");
                 }
                 user.PhoneNumber = model.PhoneNumber;
             }

             if (model.Email != null && model.Email.Trim() != "" && claim.Value != model.Email)
             {
                 if (_userManager.Users.Any(obj => obj.Email == model.Email))
                 {
                     return BadRequest("Email already exists!");
                 }
                 user.Email = model.Email;
                 user.UserName = model.Email;
             }

             if (model.Name != null && model.Name.Trim() != "")
             {
                 user.Name = model.Name;
             }

             var result = await _userManager.UpdateAsync(user);
             if (result.Succeeded)
             {
                 var token = GenerateJSONWebToken(user.Email);
                 return Ok(new
                 {
                     token = new JwtSecurityTokenHandler().WriteToken(token),
                     expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(token.ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt")
                 });
             }

             var Descriptions = result.Errors.Select(obj => obj.Description);
             var errorMassege = string.Join(',', Descriptions);
             return BadRequest(errorMassege);
         }
        */
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var role = await _userManager.GetRolesAsync(user);
                var token = GenerateJSONWebToken(model.Email);

                return Ok(new
                {
                    userType = role.FirstOrDefault(),
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(token.ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt")
                });
            }
            return Unauthorized("Wrong email or password");
        }
        [HttpPost]
        public async Task<IActionResult> RegisterApplicant([FromBody] RegisterApplicant model)
        {
            //_db.ApplicantUsers.FirstOrDefault(obj => obj.Email == model.Email);
            var userExists = await _userManager.FindByEmailAsync(model.Email);

            if (userExists != null)
            {
                return BadRequest("Email already exists!");
            }

            if (_userManager.Users.Any(obj => obj.PhoneNumber == model.PhoneNumber))
            {
                return BadRequest("PhoneNumber already exists!");
            }

            ApplicantUser user = new ApplicantUser()
            {
                Email = model.Email,
                UserName = model.Email,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                EducationLevel = model.EducationLevel,
                Gender = model.Gender,
                City = model.City,
                BirthDay = model.BirthDay,
                Street = model.Street,
                Country = model.Country,
                MilitaryStatus = model.MilitaryStatus,
                TwitterUsername = model.TwitterUsername
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Applicant");
                var token = GenerateJSONWebToken(model.Email);
                return Ok(new
                {
                    userType = "Applicant",
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(token.ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt")
                });
            }

            var Descriptions = result.Errors.Select(obj => obj.Description);
            var errorMassege = string.Join(',', Descriptions);
            return BadRequest(errorMassege);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterRecruiter([FromBody] RegisterRecruiter model)
        {
            //_db.ApplicantUsers.FirstOrDefault(obj => obj.Email == model.Email);
            var userExists = await _userManager.FindByEmailAsync(model.Email);

            if (userExists != null)
            {
                return BadRequest("Email already exists!");
            }

            if (_userManager.Users.Any(obj => obj.PhoneNumber == model.PhoneNumber))
            {
                return BadRequest("PhoneNumber already exists!");
            }

            RecruiterUser user = new RecruiterUser()
            {
                Email = model.Email,
                UserName = model.Email,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                City = model.City,
                BirthDay = model.BirthDay,
                Street = model.Street,
                Country = model.Country,
                Position = model.Position,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Recruiter");
                var token = GenerateJSONWebToken(model.Email);
                return Ok(new
                {
                    userType = "Recruiter",
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(token.ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt")
                });
            }

            var Descriptions = result.Errors.Select(obj => obj.Description);
            var errorMassege = string.Join(',', Descriptions);
            return BadRequest(errorMassege);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var admin = _db.AdminUsers.FirstOrDefault(obj => obj.Email == claim.Value);
            if (admin != null)
            {
                return Ok(new
                {
                    userType = "Admin",
                    admin.Email,
                    admin.Name,
                    admin.PhoneNumber,
                    admin.BirthDay,
                    admin.Gender,
                });
            }

            var applicant = _db.ApplicantUsers.FirstOrDefault(obj => obj.Email == claim.Value);
            if (applicant != null)
            {
                return Ok(new
                {
                    userType = "applicant",
                    applicant.Email,
                    applicant.Name,
                    applicant.PhoneNumber,
                    applicant.BirthDay,
                    applicant.Gender,
                    applicant.MilitaryStatus,
                    applicant.EducationLevel,
                    applicant.City,
                    applicant.Street,
                    applicant.Country,
                    applicant.TwitterUsername,
                    applicant.Tags,
                });
            }

            var recruiter = _db.RecruiterUsers.FirstOrDefault(obj => obj.Email == claim.Value);
            if (recruiter != null)
            {
                return Ok(new
                {
                    userType = "Recruiter",
                    recruiter.Email,
                    recruiter.Name,
                    recruiter.PhoneNumber,
                    recruiter.BirthDay,
                    recruiter.Gender,
                    recruiter.City,
                    recruiter.Street,
                    recruiter.Country,
                    recruiter.Position,
                    recruiter.Company,
                });
            };

            return BadRequest("Wrong User email");
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = _userManager.Users.FirstOrDefault(obj => obj.Email == email);
            if (user == null)
            {
                return BadRequest("Wrong User email");
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok("User Deleted");
            }
            return BadRequest("User not deleted");
        }
        [HttpPost]
        [Authorize]
        public IActionResult addTagesToApplicant(UserTages userTages)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var applicant = _db.ApplicantUsers.FirstOrDefault(obj => obj.Email == claim.Value);
            if (applicant == null)
            {
                return BadRequest("Wrong User email");
            }
            applicant.Tags.Clear();
            foreach 
            _db.ApplicantUsers.Update(applicant);
            foreach (var item in userTages.Tags)
            {
                Tag tag = null;
                if(!_db.Tags.Any(obj => obj.Name == item))
                {
                    tag = new Tag() { Name = item };
                    var result = _db.Tags.Add(tag);
                }
                else
                {
                    tag = _db.Tags.FirstOrDefault(obj => obj.Name == item);
                }
                applicant.Tags.Add(tag);
            }
            _db.ApplicantUsers.Update(applicant);
            _db.SaveChanges();
            return Ok("Tags Added");
        }

        private JwtSecurityToken GenerateJSONWebToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim("Email", email) };

            var token = new JwtSecurityToken(_configuration["JWT:ValidIssuer"],
              _configuration["JWT:ValidAudience"],
              claims,
              expires: DateTime.Now.AddHours(8),
              signingCredentials: credentials);

            return token;
        }
    }
}