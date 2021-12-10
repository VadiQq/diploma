using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMF.Data.Enums;
using PDMF.Data.Repositories;
using PDMF.WebApi.Controllers.Identity.Abstract;
using PDMF.WebApi.Controllers.Identity.Models;
using PDMF.WebApi.Models.Identity.Data;
using PDMF.WebApi.Services.Identity;

namespace PDMF.WebApi.Controllers.Identity
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : IdentityControllerBase
    {
        private readonly UserRepository _userRepository;

        public AccountController(
            UserManager<PDMFUser> userManager,
            SignInManager<PDMFUser> signInManager,
            UserRepository userRepository) : base(userManager, signInManager)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var emailUsed = await _userRepository.UsersContextAccess.AnyAsync(u => u.Email == registerModel.Email);

            if (emailUsed)
            {
                return BadRequest(new {Message = "Email is already used."});
            }

            var user = new PDMFUser
            {
                Email = registerModel.Email,
                UserName = registerModel.UserName,
                CreateDate = DateTime.UtcNow,
                PhoneNumber = registerModel.PhoneNumber,
                SpareEmail = registerModel.SpareEmail,
                State = UserStatus.Active
            };

            var result = await UserManager.CreateAsync(user, registerModel.Password);
            if (result.Succeeded)
            {
                var identity = await GetIdentity(registerModel.Email, registerModel.Password);
                return Ok(new {Token = TokenFactory.CreateJWT(identity), username = registerModel.Email});
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest(new {Message = "Unable to register user."});
        }

        [HttpPost("delete")]
        [Authorize]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(id);

                var result = await UserManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }
        
        [HttpPost("edit")]
        [Authorize]
        public async Task<IActionResult> Edit([FromBody] EditModel updatedProfile)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(updatedProfile.Id);
                user.SpareEmail = updatedProfile.SpareEmail;
                user.Email = updatedProfile.Email;
                user.UserName = updatedProfile.UserName;
                user.PhoneNumber = updatedProfile.PhoneNumber;

                if (!string.IsNullOrEmpty(updatedProfile.Password))
                {
                    user.PasswordHash =  UserManager.PasswordHasher.HashPassword(user, updatedProfile.Password);
                }
               
                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);

            var token = JWTHelper.GetJWTFromHeader(authHeader);
            var userId = JWTHelper.GetJWTValue(token, ClaimsTypes.Id);

            var user = await _userRepository.Get(userId);

            return Ok(user);
        }
    }
}