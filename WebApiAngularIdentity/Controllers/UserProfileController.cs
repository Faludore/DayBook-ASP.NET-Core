using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApiAngularIdentity.Models;

using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;
using System.IO.Compression;
using ImageMagick;

namespace WebApiAngularIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private AuthenticationContext _authenticationContext;
        public UserProfileController(UserManager<ApplicationUser> userManager, AuthenticationContext authenticationContext)
        {
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

                    Invite invite = new Invite() { Code = RandomCode(), Email = email, DnT = DateTime.Now.ToString("dd/MM/yyyy"), Status = "Active" };

                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("Site admin", "realmail@gmail.com"));
                    emailMessage.To.Add(new MailboxAddress("", email));
                    emailMessage.Subject = "Register on day book";
                    emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = "Your link to register: "+ "http://localhost:4200/user/registration?code="+ invite.Code};

                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync("smtp.gmail.com", 587, false);
                        await client.AuthenticateAsync("realmail@gmail.com", "qwerty123");
                        await client.SendAsync(emailMessage);
                        await client.DisconnectAsync(true);
                    }
            

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