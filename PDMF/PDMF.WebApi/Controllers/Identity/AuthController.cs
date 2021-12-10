using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Batch.Common;
using PDMF.WebApi.Controllers.Identity.Abstract;
using PDMF.WebApi.Controllers.Identity.Models;
using PDMF.WebApi.Models.Identity.Data;
using PDMF.WebApi.Services.Identity;

namespace PDMF.WebApi.Controllers.Identity
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : IdentityControllerBase
    {
        public AuthController(
            UserManager<PDMFUser> userManager, 
            SignInManager<PDMFUser> signInManager) : base(userManager, signInManager)
        {
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
          

            return BadRequest();
        }

        [HttpPost("ping")]
        [Authorize]
        public async Task<IActionResult> Ping()
        {
            return Ok();
        }
        
                
        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] LoginModel loginModel)
        {
            var identity = await GetIdentity(loginModel.Email, loginModel.Password);

            if (identity == null)
            {
                return BadRequest(new { Message = "Invalid credentials."});
            }

            var encodedJwt = TokenFactory.CreateJWT(identity);
 
            var response = new
            {
                token = encodedJwt,
                username = JWTHelper.GetJWTValue(encodedJwt, ClaimsTypes.UserName)
            };  
 
            return Ok(response);
        }
    }
}