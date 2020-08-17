using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DataAccessLibary.Models;
using DataAccessLibary.DataAccess;

using System.Collections.Concurrent;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using WebApiAngularIdentity.Services.TaskQueue;
using WebApiAngularIdentity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiAngularIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        public static string AdminEmail = "justfortestsfaludore@gmail.com";
        public static string AdminPassword = "TestForJust123";

        private UserManager<ApplicationUser> _userManager;
        private AuthenticationContext _authenticationContext;

        private readonly IServiceProvider _services;

        private static BlockingCollection<Mail> q = new BlockingCollection<Mail>();

        public UserProfileController(UserManager<ApplicationUser> userManager, AuthenticationContext authenticationContext, IServiceProvider services)
        {
            _services = services;
            _userManager = userManager;
            _authenticationContext = authenticationContext;         
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("AddInvite")]
        //Post : /api/UserProfile/AddInvite
        public async Task<Object> AddInvite([FromForm(Name = "Email")]string email)
        {
            if (email != null)
            {
                List<Invite> checkInvites = _authenticationContext.Invites.Where(p => p.Email == email).ToList();
                if (checkInvites.Count == 0)
                {
                    //prod-consum mail sender 
                    Mail mail = new Mail() { EmailFrom = AdminEmail, EmailTo = "some@gmail.com", Message = "highkhkny54t", DnT = DateTime.Now.ToString("dd/MM/yyyy") };

                    //Adding email to collection
                    var monitorLoop = _services.GetRequiredService<MonitorLoop>();
                    monitorLoop.StartMonitorLoop(mail);

                    Invite invite = new Invite() { Code = RandomCode(), Email = email, DnT = DateTime.Now.ToString("dd/MM/yyyy"), Status = "Active" };
                    try
                    {
                        using (_authenticationContext)
                        {
                            _authenticationContext.Invites.Add(invite);
                            _authenticationContext.SaveChanges();
                        }
                        return Ok(invite);
                    }
                    catch
                    {
                        return BadRequest( "server error" );
                    }
                }
                else              
                    return BadRequest("already sanded");               
            }
            else        
                return BadRequest("email is null");                       
        }
    
        [HttpGet]
        [Route("CheckCode")]
        //Get : /api/UserProfile/CheckCode
        public bool CheckCode(int code)
        {
            List<Invite> checkInvites = _authenticationContext.Invites.Where(p => p.Code == code.ToString()).ToList();
            if (checkInvites.Count == 1 && checkInvites[0].Status =="Active")
                return true;
            else
                return false;
        }

        public string RandomCode()
        {
            Random rnd = new Random();
            string rndcode = rnd.Next(10000000, 99999999).ToString();
            List<Invite> checkInvites = _authenticationContext.Invites.Where(p => p.Code == rndcode).ToList();
            if (checkInvites.Count == 0)
                return rndcode;
            else
                return RandomCode();
        }
    }
}