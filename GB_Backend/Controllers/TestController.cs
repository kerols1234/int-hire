using GB_Backend.Data;
using GB_Backend.Models.APIforms;
using GB_Backend.Models.APIModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GB_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors]
    // [Authorize]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public TestController(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
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
        public async Task<IActionResult> postAnswer([FromBody] TestForm testForm)
        {
            /*
            if (testForm.Answers.Count != 56)
            {
                return BadRequest("Number of answers must be 56");
            }
            */
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
            /*
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
            */
            /*
             *Api for tweeter
             *Api for machine model
             *combine all to get on type of user
            */
            ResponseModel<TwitterAccount> twitterAccount = null;
            ResponseModel<HashSet<TweetModel>> tweets = null;
            string text = "";
            int countOfTweets = 0;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.twitter.com/2/users/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["Twitter:BearerToken"]);

                HttpResponseMessage response = await client.GetAsync($"by/username/{applicant.TwitterUsername}");
                if (response.IsSuccessStatusCode)
                {
                    twitterAccount = await response.Content.ReadAsAsync<ResponseModel<TwitterAccount>>();
                    if (twitterAccount.Errors.Count > 0)
                    {
                        return BadRequest("Twitter error: " + twitterAccount.Errors.FirstOrDefault().Detail);
                    }
                }
                else
                {
                    return BadRequest(response.Content.ReadAsStringAsync().Result);
                }
                response = await client.GetAsync($"{twitterAccount.Data.Id}/tweets?max_results=100");
                do
                {
                    if (response.IsSuccessStatusCode)
                    {
                        tweets = await response.Content.ReadAsAsync<ResponseModel<HashSet<TweetModel>>>();
                        if (tweets.Errors.Count > 0)
                        {
                            return BadRequest("Twitter error: " + tweets.Errors.FirstOrDefault().Detail);
                        }
                        tweets.Data = tweets.Data.Where(obj => !obj.Text.EndsWith("…")).ToHashSet();
                        text += tweets.Data.Aggregate("", (total,next) => total += " " + next.Text);
                        countOfTweets += tweets.Data.Count;
                    }
                    else
                    {
                        return BadRequest(response.Content.ReadAsStringAsync().Result);
                    }

                    response = await client.GetAsync($"{twitterAccount.Data.Id}/tweets?max_results=100&pagination_token={tweets.Meta.Next_token}");
                } while (tweets.Meta.Next_token != null && countOfTweets < 100);
            }
            PredictTypeModel typeModel = null;
            TweetsData tweetsData = new TweetsData();
            tweetsData.data = text;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                HttpResponseMessage response = await client.PostAsJsonAsync("response", tweetsData);
                if (response.IsSuccessStatusCode)
                {
                    typeModel = await response.Content.ReadAsAsync<PredictTypeModel>();
                }
                else
                {
                    return BadRequest(response.Content.ReadAsStringAsync().Result);
                }
            }

            return Ok(typeModel);
            //return Ok(new { counts = counts, type = type });
        }
    }
}
