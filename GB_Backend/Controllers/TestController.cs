using CsvHelper;
using GB_Backend.Data;
using GB_Backend.Models;
using GB_Backend.Models.APIforms;
using GB_Backend.Models.APIModels;
using idCard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        [HttpPost]
        public async Task<IActionResult> test([FromForm] FilesModel file)
        {
            var fileextension = Path.GetExtension(file.files.FileName);
            var filename = Guid.NewGuid().ToString() + fileextension;
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "files", filename);
            using (FileStream fs = System.IO.File.Create(filepath))
            {
                file.files.CopyTo(fs);
            }
            if (fileextension == ".csv")
            {
                using (var reader = new StreamReader(filepath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<TestModel>();
                    foreach (var record in records)
                    {

                        if (string.IsNullOrWhiteSpace(record.Question))
                        {
                            break;
                        }
                        Test test;
                        test = _db.Tests.Where(s => s.Question == record.Question).FirstOrDefault();

                        if (test == null)
                        {
                            test = new Test();
                        }
                        if (record.LabelA == "P")
                        {
                            test.Question = record.Question;
                            test.LabelA = record.LabelB;
                            test.LabelB = record.LabelA;
                            test.AnswerB = record.A;
                            test.AnswerA = record.B;
                        }
                        else
                        {
                            test.Question = record.Question;
                            test.LabelA = record.LabelA;
                            test.LabelB = record.LabelB;
                            test.AnswerB = record.B;
                            test.AnswerA = record.A;
                        }
                        await _db.Tests.AddAsync(test);

                    }
                    _db.SaveChanges();
                }
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ranking([FromForm] FilesModel file)
        {
            var fileextension = Path.GetExtension(file.files.FileName);
            var filename = Guid.NewGuid().ToString() + fileextension;
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "files", filename);
            using (FileStream fs = System.IO.File.Create(filepath))
            {
                file.files.CopyTo(fs);
            }
            if (fileextension == ".csv")
            {
                using (var reader = new StreamReader(filepath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<MPTIModel>();
                    foreach (var record in records)
                    {

                        if (string.IsNullOrWhiteSpace(record.personality))
                        {
                            break;
                        }
                        MPTIModel model;
                        model = _db.MPTIModels.Where(s => s.personality == record.personality).FirstOrDefault();

                        if (model == null)
                        {
                            model = new MPTIModel();
                        }

                        model = record;

                        await _db.MPTIModels.AddAsync(model);

                    }
                    _db.SaveChanges();
                }
            }
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> MTPI([FromForm] FilesModel file)
        {
            var fileextension = Path.GetExtension(file.files.FileName);
            var filename = Guid.NewGuid().ToString() + fileextension;
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "files", filename);
            using (FileStream fs = System.IO.File.Create(filepath))
            {
                file.files.CopyTo(fs);
            }
            if (fileextension == ".csv")
            {
                using (var reader = new StreamReader(filepath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<MBTIType>();
                    foreach (var record in records)
                    {

                        if (string.IsNullOrWhiteSpace(record.Type))
                        {
                            break;
                        }
                        MBTIType mbti;
                        mbti = _db.MBTITypes.Where(s => s.Type == record.Type).FirstOrDefault();

                        if (mbti == null)
                        {
                            mbti = new MBTIType();
                        }



                        mbti = record;

                        await _db.MBTITypes.AddAsync(mbti);

                    }
                    _db.SaveChanges();
                }
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult getTestQuestions()
        {
            return Ok(_db.Tests.ToList());
        }

        [HttpGet]
        public IActionResult getMPTI()
        {
            return Ok(_db.MBTITypes.Select(obj => new
            {
                obj.Type,
                obj.Nickname,
                obj.Definition,
                obj.Introduction,
                obj.StrengthsandWeaknesses,
                obj.CareerPaths,
                obj.WorkplaceHabits,
                obj.Description
            }
            ).ToList());
        }

        [HttpGet]
        public IActionResult getModel()
        {
            return Ok(_db.MPTIModels.Select(obj => new
            {
                obj.personality,
                obj.KindredSpirits,
                obj.ChallengingOpposites,
                obj.PotentialComplements,
                obj.IntriguingDifferences,
            }).ToList());
        }


        [HttpPost]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> postAnswer([FromBody] TestForm testForm)
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
            /*
            if (!_db.Jobs.Any(obj => obj.Id == testForm.JobId))
            {
                return BadRequest("Wrong Job Id");
            }
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
                response = await client.GetAsync($"{twitterAccount.Data.Id}/tweets?max_results=50");
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
                        text += tweets.Data.Aggregate("", (total, next) => total += " " + next.Text);
                        countOfTweets += tweets.Data.Count;
                    }
                    else
                    {
                        return BadRequest(response.Content.ReadAsStringAsync().Result);
                    }

                    response = await client.GetAsync($"{twitterAccount.Data.Id}/tweets?max_results=50&pagination_token={tweets.Meta.Next_token}");
                } while (tweets.Meta.Next_token != null && countOfTweets < 50);
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
