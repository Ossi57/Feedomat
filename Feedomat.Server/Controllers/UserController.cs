using System.Threading.Tasks;
using Feedomat.Server.Model;
using Feedomat.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Feedomat.Server.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private FeedomatContext Context { get; set; }
        public UserController(FeedomatContext context)
        {
            Context = context;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult> LoginUser(UserModel user)
        {
            UserModel loggedUser = await Context.Users.FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);          
            if (loggedUser != null)
            {
                //create a claim
                var claim = new Claim(ClaimTypes.Name, loggedUser.Username);
                //create claimsIdentity
                var claimsIdentity = new ClaimsIdentity(new[] { claim }, "serverAuth");
                //create claimsPrincipal
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                //Sign In User
                await HttpContext.SignInAsync(claimsPrincipal);

                return Ok(loggedUser);
            }
            else
            {
                return Unauthorized("Username or Password is invalid");
            }
        }

        [HttpGet("getcurrentuser")]
        public async Task<ActionResult<UserModel>> GetUser()
        {
            UserModel loggedUser = new();
            if (User.Identity.IsAuthenticated)
            {
                string username = User.FindFirstValue(ClaimTypes.Name);
                loggedUser = await Context.Users.FirstOrDefaultAsync(u => u.Username == username);
            }
            return await Task.FromResult(loggedUser);
        }
         
        [HttpGet("logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync();
        }
    }
}
