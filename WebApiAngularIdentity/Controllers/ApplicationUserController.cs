using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApiAngularIdentity.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

namespace WebApiAngularIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private AuthenticationContext _authenticationContext;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<ApplicationSettings> appSettings, AuthenticationContext authenticationContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _authenticationContext = authenticationContext;
        }
        [HttpPost]
        [Route("Register")]
        //POST : api/ApplicationUser/Register
        public async Task<Object> PostApplicationUser(ApplicationUserModel model)
        {
            model.Role = "Customer";
            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                InviteCode = model.Code
            };
            List<Invite> checkInvites = _authenticationContext.Invites.Where(p => p.Code == model.Code.ToString()).ToList();
            if (checkInvites.Count == 1 && checkInvites[0].Status == "Active")
            {
                try
                {
                    var rusult = await _userManager.CreateAsync(applicationUser, model.Password);
                    await _userManager.AddToRoleAsync(applicationUser, model.Role);
                    checkInvites[0].Status = "Passed";
                    _authenticationContext.Invites.Update(checkInvites[0]);
                    _authenticationContext.SaveChanges();
                    return Ok(rusult);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return BadRequest(new { message = "Incorect data" });
            }         
        }
        [HttpPost]
        [Route("Login")]
        //POST : api/ApplicationUser/Login
        public async Task<ActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (user.DnTDelete == null)
                {
                    //Get role
                    var role = await _userManager.GetRolesAsync(user);
                    IdentityOptions options = new IdentityOptions();

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim(options.ClaimsIdentity.RoleClaimType, role.FirstOrDefault())
                        }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);
                    return Ok(new { token });
                }
                else               
                    return BadRequest();              
            }
            else
                return NotFound();
        }

        [HttpPost]
        [Route("RestoreAcc")]
        //POST : api/ApplicationUser/RestoreAcc
        public async Task<ActionResult> RestoreAcc(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (user.DnTDelete != null)
                {
                    user.DnTDelete = null;
                    using (_authenticationContext)
                    {
                        _authenticationContext.ApplicationUsers.Update(user);
                        _authenticationContext.SaveChanges();
                    }
                    return Ok();
                }
                else
                    return BadRequest();
            }
            else
                return NotFound();
        }

        [HttpPost]
        [Route("DeleteAcc")]
        //POST : api/ApplicationUser/DeleteAcc
        public async Task<ActionResult> DeleteAcc(LoginModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);           
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (user.DnTDelete == null)
                {
                    user.DnTDelete = DateTime.Now.ToString("dd/MM/yyyy");
                    using (_authenticationContext)
                    {
                        _authenticationContext.ApplicationUsers.Update(user);
                        _authenticationContext.SaveChanges();
                    }
                    return Ok();
                }
                else
                    return BadRequest();
            }
            else
                return NotFound();
        }
    }
}