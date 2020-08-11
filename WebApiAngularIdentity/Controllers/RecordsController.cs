using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAngularIdentity.Models;

namespace WebApiAngularIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordsController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private AuthenticationContext _authenticationContext;
        public RecordsController(UserManager<ApplicationUser> userManager, AuthenticationContext authenticationContext)
        {
            _userManager = userManager;
            _authenticationContext = authenticationContext;
        }

        // GET: api/Records
        [HttpGet]
        [Authorize]  
        public async Task<Object> GetRecords()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var records = _authenticationContext.Records.Where(p => p.UserId == userId).ToList();
            List<RecordStringViewModel> recordStringViewModels = new List<RecordStringViewModel>();
            foreach (var item in records)
            {
                RecordStringViewModel rsvm = new RecordStringViewModel() { Id = Encrypter(Convert.ToString(item.Id), true), Title = EnigmaCoder(item.Title, false), Text = EnigmaCoder(item.Text, false), DnT = item.DnT, Image = item.Image };
                recordStringViewModels.Add(rsvm);
            }
            return recordStringViewModels;
        }

        // GET: api/Records/5
        [HttpGet("{id}")]
        [Authorize]     
        public async Task<Object> GetRecord([FromRoute]string id)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            Record record = await _authenticationContext.Records.FindAsync(Convert.ToInt32(Encrypter(Convert.ToString(id), false)));
            if (record != null && record.UserId == userId)
            {          
                RecordViewModel rvm = new RecordViewModel() { Id = Encrypter(Convert.ToString(record.Id), true), Title = EnigmaCoder(record.Title, false), Text = EnigmaCoder(record.Text, false), Image = record.Image };
                return rvm;
            }
            else
                return BadRequest(new { message = "Record not found" });
        }

        // PUT: api/Records
        [HttpPut]
        [Authorize]
        public async Task<Object> PutRecord([FromForm(Name = "image")] List<IFormFile> files, [FromForm(Name = "Title")] string title, [FromForm(Name = "Text")] string text, [FromForm(Name = "Id")] string id)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            Record record = await _authenticationContext.Records.FindAsync(Convert.ToInt32(Encrypter(Convert.ToString(id), false)));
            byte[] imageData = null;
            using (var binaryReader = new BinaryReader(files[0].OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)files[0].Length);
            }
            if (record != null && record.UserId == userId && Convert.ToInt32(Encrypter(Convert.ToString(id), false)) != 0)
            {
                if (imageData != null)
                    record.Image = Convert.ToBase64String(imageData);

                record.Title = EnigmaCoder(title, true);
                record.Text = EnigmaCoder(text, true);
                using (_authenticationContext)
                {
                    _authenticationContext.Records.Update(record);
                    _authenticationContext.SaveChanges();
                }
                return record;
            }
            else
                return BadRequest(new { message = "Record not found" });
        }

        // POST: api/Records
        [HttpPost]
        [Authorize]      
        public async Task<Object> PostRecord([FromForm(Name = "image")] List<IFormFile> files, [FromForm(Name = "Title")] string title, [FromForm(Name = "Text")] string text)
        {
            if (files != null && title != null && text != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(files[0].OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)files[0].Length);
                }

                if (imageData.Length > 5250000)
                {
                    using (MagickImage image = new MagickImage(imageData))
                    {
                        image.Format = image.Format;
                        image.Resize(100, 80);
                        image.Quality = 50;
                        imageData = image.ToByteArray();
                    }
                }

                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = await _userManager.FindByIdAsync(userId);
                Record record = new Record()
                {
                    Title = EnigmaCoder(title, true),
                    Text = EnigmaCoder(text, true),
                    DnT = DateTime.Now.ToString("dd/MM/yyyy"),
                    UserId = userId,
                    User = user,
                    Image = Convert.ToBase64String(imageData)

                };

                using (_authenticationContext)
                {
                    await _authenticationContext.Records.AddAsync(record);
                    await _authenticationContext.SaveChangesAsync();
                }
                return Ok(record);
            }
            else
            {
                return BadRequest(new { message = "Record not found" });
            }
        }

        // DELETE: api/Records/5
        [HttpDelete("{id}")]
        [Authorize]      
        public async Task<Object> DeleteRecord(string id)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            Record record = await _authenticationContext.Records.FindAsync(Convert.ToInt32(Encrypter(Convert.ToString(id), false)));
            if (record != null && record.UserId == userId)
            {
                if (CheckDnT(record.DnT, DateTime.Now.ToString("dd/MM/yyyy")))
                {
                    using (_authenticationContext)
                    {
                        _authenticationContext.Records.Remove(record);
                        _authenticationContext.SaveChanges();
                    }
                    return Ok("deleted");
                }
                else
                    return BadRequest(new { message = "Older than 2 days. Can't be deleted" });
            }
            else
                return BadRequest(new { message = "Incorect data" });
        }
      

        private bool RecordExists(int id)
        {
            return _authenticationContext.Records.Any(e => e.Id == id);
        }

        private bool CheckDnT(string dnt1, string dnt2)
        {
            int[] arr_dnt1 = new int[3];
            int[] arr_dnt2 = new int[3];


            string[] elements1 = dnt1.Split(new char[] { '.' });
            string[] elements2 = dnt2.Split(new char[] { '.' });

            for (int i = 0; i < 3; i++)
            {
                arr_dnt1[i] = Convert.ToInt32(elements1[i]);
                arr_dnt2[i] = Convert.ToInt32(elements2[i]);
            }

            DateTime firstDate = new DateTime(arr_dnt1[2], arr_dnt1[1], arr_dnt1[0]);
            DateTime secondDate = new DateTime(arr_dnt2[2], arr_dnt2[1], arr_dnt2[0]);
            TimeSpan diff1 = secondDate - firstDate;

            int daydiff = Convert.ToInt32((secondDate - firstDate).TotalDays.ToString());

            if (daydiff < 2)
            {
                return true;
            }
            else
                return false;
        }
        private string EnigmaCoder(string text, bool type)
        {
            string arr_chars = null;
            string arr_rotor = null;
            string res = null;
            if (type)
            {
                arr_chars = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
                arr_rotor = "tuvakqrs4LbIJQhwMmnoPKDcdEFGH0ABC5RSTUlVWXijefNOpz 6789xYZy123g";
            }
            else
            {              
                arr_chars = "tuvakqrs4LbIJQhwMmnoPKDcdEFGH0ABC5RSTUlVWXijefNOpz 6789xYZy123g";
                arr_rotor = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
            }
     
            for (int i = 0; i < text.Length; i++)
            {
                if (arr_chars.Contains(text[i]))
                {
                    res += arr_rotor[arr_chars.IndexOf(text[i])];
                }
                else
                {
                    res += text[i];
                }
            }
            return res;
        }
        public string Encrypter(string str, bool mode)
        {
            byte[] b;
            if (mode)
            {
                b = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
                return Convert.ToBase64String(b);
            }
            else
            {
                b = Convert.FromBase64String(str);
                return System.Text.ASCIIEncoding.ASCII.GetString(b);
            }

        }

    }
}