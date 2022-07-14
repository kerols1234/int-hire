using GB_Backend.Data;
using GB_Backend.Models;
using GB_Backend.Models.APIforms;
using GB_Backend.Models.APIModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var role = await _userManager.GetRolesAsync(user);
                var token = GenerateJSONWebTokenAsync(model.Email);

                return Ok(new
                {
                    userType = role.FirstOrDefault(),
                    token = new JwtSecurityTokenHandler().WriteToken(await token),
                    expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((await token).ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                    userDate = userInfo(model.Email)
                });
            }
            return Unauthorized("Wrong email or password");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterApplicant([FromBody] ApplicantForm model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);

            if (userExists != null)
            {
                return BadRequest("Email already exists!");
            }

            if (_userManager.Users.Any(obj => obj.PhoneNumber == model.PhoneNumber))
            {
                return BadRequest("PhoneNumber already exists!");
            }

            if (model.TwitterUsername != null)
            {
                ResponseModel<TwitterAccount> responseModel = null;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.twitter.com/2/users/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["Twitter:BearerToken"]);
                    HttpResponseMessage response = await client.GetAsync($"by/username/{model.TwitterUsername}");
                    if (response.IsSuccessStatusCode)
                    {
                        responseModel = await response.Content.ReadAsAsync<ResponseModel<TwitterAccount>>();
                        if (responseModel.Errors.Count > 0)
                        {
                            return BadRequest("Twitter error: " + responseModel.Errors.FirstOrDefault().Detail);
                        }
                    }
                    else
                    {
                        return BadRequest(response.Content.ReadAsStringAsync().Result);
                    }
                }
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
                TwitterUsername = model.TwitterUsername,
                skills = model.Skills,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                addTags(user, model.Tags);
                await _userManager.AddToRoleAsync(user, "Applicant");
                var token = GenerateJSONWebTokenAsync(model.Email);
                return Ok(new
                {
                    userType = "Applicant",
                    token = new JwtSecurityTokenHandler().WriteToken(await token),
                    expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((await token).ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                    userDate = userInfo(model.Email)
                });
            }

            var Descriptions = result.Errors.Select(obj => obj.Description);
            var errorMassege = string.Join(',', Descriptions);
            return BadRequest(errorMassege);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterRecruiter([FromBody] RecruiterForm model)
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
                Company = model.Company,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Recruiter");
                var token = GenerateJSONWebTokenAsync(model.Email);
                return Ok(new
                {
                    userType = "Recruiter",
                    token = new JwtSecurityTokenHandler().WriteToken(await token),
                    expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((await token).ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                    userDate = userInfo(model.Email)
                });
            }

            var Descriptions = result.Errors.Select(obj => obj.Description);
            var errorMassege = string.Join(',', Descriptions);
            return BadRequest(errorMassege);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AdminForm model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return BadRequest("Email already exists!");
            }

            if (_userManager.Users.Any(obj => obj.PhoneNumber == model.PhoneNumber))
            {
                return BadRequest("PhoneNumber already exists!");
            }

            AdminUser user = new AdminUser()
            {
                Email = model.Email,
                UserName = model.Email,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                BirthDay = model.BirthDay,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                var token = GenerateJSONWebTokenAsync(model.Email);
                return Ok(new
                {
                    userType = "Admin",
                    token = new JwtSecurityTokenHandler().WriteToken(await token),
                    expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((await token).ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                    userDate = userInfo(model.Email)
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
            var data = userInfo(claim.Value);
            if(data.ToString() != "Wrong User email")
            {
                return Ok(data);
            }
            else
            {
                return BadRequest(data);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Applicant")]
        public IActionResult addAndUpdateTagesToApplicant(TagesForm userTages)
        {
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
            addTags(applicant, userTages.Tags);
            return Ok("Tags Added");
        }

        [HttpPost]
        [Authorize(Roles = "Recruiter")]
        public IActionResult addAndUpdateCompanyToRecruiter(Company company)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var recruiter = _db.RecruiterUsers.Include(obj => obj.Company).AsNoTracking().FirstOrDefault(obj => obj.Email == claim.Value);
            if (recruiter == null)
            {
                return BadRequest("Wrong User email");
            }
            if (company.Id != 0)
            {
                if (_db.Companies.Any(obj => obj.Id == company.Id))
                {
                    company = _db.Companies.FirstOrDefault(obj => obj.Id == company.Id);
                }
                else
                {
                    company.Id = 0;
                }

            }
            recruiter.Company = company;
            _db.RecruiterUsers.Update(recruiter);
            _db.SaveChanges();
            return Ok("Company Added");
        }

        [HttpPost]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> UpdateApplicantUserInfo([FromBody] UpdateApplicant model)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }

            var user = _db.ApplicantUsers.Include(obj => obj.Tags).FirstOrDefault(obj => obj.Email == claim.Value);

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

            if (model.BirthDay != null && model.BirthDay.Trim() != "")
            {
                user.BirthDay = model.BirthDay;
            }

            if (model.Country != null && model.Country.Trim() != "")
            {
                user.Country = model.Country;
            }

            if (model.City != null && model.City.Trim() != "")
            {
                user.City = model.City;
            }

            if (model.Street != null && model.Street.Trim() != "")
            {
                user.Street = model.Street;
            }

            if (model.TwitterUsername != null && model.TwitterUsername.Trim() != "")
            {
                ResponseModel<TwitterAccount> responseModel = null;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.twitter.com/2/users/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["Twitter:BearerToken"]);
                    HttpResponseMessage response = await client.GetAsync($"by/username/{model.TwitterUsername}");
                    if (response.IsSuccessStatusCode)
                    {
                        responseModel = await response.Content.ReadAsAsync<ResponseModel<TwitterAccount>>();
                        if (responseModel.Errors.Count > 0)
                        {
                            return BadRequest("Twitter error: " + responseModel.Errors.FirstOrDefault().Detail);
                        }
                        user.TwitterUsername = model.TwitterUsername;
                    }
                    else
                    {
                        return BadRequest(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }

            if (model.EducationLevel != null)
            {
                user.EducationLevel = (Models.Enums.EducationLevel)model.EducationLevel;
            }

            if (model.Gender != null)
            {
                user.Gender = (Models.Enums.Gender)model.Gender;
            }

            if (model.MilitaryStatus != null)
            {
                user.MilitaryStatus = (Models.MilitaryStatus)model.MilitaryStatus;
            }

            if (model.Skills != null)
            {
                user.skills = model.Skills;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (model.Tags != null && model.Tags.Count() != 0)
                {
                    addTags(user, model.Tags);
                }
                var token = GenerateJSONWebTokenAsync(user.Email);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(await token),
                    expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((await token).ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                    userDate = userInfo(model.Email)
                });
            }

            var Descriptions = result.Errors.Select(obj => obj.Description);
            var errorMassege = string.Join(',', Descriptions);
            return BadRequest(errorMassege);
        }

        [HttpPost]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateRecruiterUserInfo([FromBody] UpdateRecruiter model)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }

            var user = _db.RecruiterUsers.FirstOrDefault(obj => obj.Email == claim.Value);

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

            if (model.BirthDay != null && model.BirthDay.Trim() != "")
            {
                user.BirthDay = model.BirthDay;
            }

            if (model.Country != null && model.Country.Trim() != "")
            {
                user.Country = model.Country;
            }

            if (model.City != null && model.City.Trim() != "")
            {
                user.City = model.City;
            }

            if (model.Street != null && model.Street.Trim() != "")
            {
                user.Street = model.Street;
            }

            if (model.Gender != null)
            {
                user.Gender = (Models.Enums.Gender)model.Gender;
            }

            if (model.Position != null && model.Position.Trim() != "")
            {
                user.Position = model.Position;
            }

            if (model.Company != null)
            {
                user.Company = model.Company;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var token = GenerateJSONWebTokenAsync(user.Email);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(await token),
                    expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((await token).ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt"),
                    userDate = userInfo(model.Email)
                });
            }

            var Descriptions = result.Errors.Select(obj => obj.Description);
            var errorMassege = string.Join(',', Descriptions);
            return BadRequest(errorMassege);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAdminUserInfo([FromBody] UpdateAdmin model)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }

            var user = _db.AdminUsers.FirstOrDefault(obj => obj.Email == claim.Value);

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

            if (model.BirthDay != null && model.BirthDay.Trim() != "")
            {
                user.BirthDay = model.BirthDay;
            }

            if (model.Gender != null)
            {
                user.Gender = (Models.Enums.Gender)model.Gender;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var token = GenerateJSONWebTokenAsync(user.Email);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(await token),
                    expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((await token).ValidTo, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt")
                });
            }

            var Descriptions = result.Errors.Select(obj => obj.Description);
            var errorMassege = string.Join(',', Descriptions);
            return BadRequest(errorMassege);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
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

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password changed successfully");
            }

            var Descriptions = result.Errors.Select(obj => obj.Description);
            var errorMassege = string.Join(',', Descriptions);
            return BadRequest(errorMassege);
        }

        private async Task<JwtSecurityToken> GenerateJSONWebTokenAsync(string email)
        {
            var claims = new List<Claim> { new Claim("Email", email) };
            var user = await _userManager.FindByEmailAsync(email);
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["JWT:ValidIssuer"],
              _configuration["JWT:ValidAudience"],
              claims,
              expires: DateTime.Now.AddHours(8),
              signingCredentials: credentials);

            return token;
        }

        private Object userInfo(string email)
        {
            var admin = _db.AdminUsers.FirstOrDefault(obj => obj.Email == email);
            if (admin != null)
            {
                return new
                {
                    userType = "Admin",
                    admin.Email,
                    admin.Name,
                    admin.PhoneNumber,
                    admin.BirthDay,
                    admin.Gender,
                };
            }

            var applicant = _db.ApplicantUsers.Include(obj => obj.Tags).FirstOrDefault(obj => obj.Email == email);
            if (applicant != null)
            {
                return new
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
                    applicant.skills,
                    tags = applicant.Tags.Select(obj => obj.Name).ToList()
                };
            }

            var recruiter = _db.RecruiterUsers.Include(obj => obj.Company).FirstOrDefault(obj => obj.Email == email);
            if (recruiter != null)
            {
                return new
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
                };
            };

            return "Wrong User email";
        }

        private void addTags(ApplicantUser applicant, ICollection<string> tags)
        {
            foreach (var item in tags)
            {
                if (!_db.Tags.Any(obj => obj.Name == item))
                {
                    Tag tag = new Tag() { Name = item };
                    var result = _db.Tags.Add(tag);
                    applicant.Tags.Add(tag);
                }
                else if (!applicant.Tags.Any(obj => obj.Name == item))
                {
                    applicant.Tags.Add(_db.Tags.FirstOrDefault(obj => obj.Name == item));
                }
            }
            foreach (var item in applicant.Tags)
            {
                if (!tags.Any(obj => obj == item.Name))
                {
                    applicant.Tags.Remove(item);
                }
            }
            _db.ApplicantUsers.Update(applicant);
            _db.SaveChanges();
        }
    }
}