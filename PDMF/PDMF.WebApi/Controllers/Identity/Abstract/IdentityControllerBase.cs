using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PDMF.WebApi.Models.Identity.Data;

namespace PDMF.WebApi.Controllers.Identity.Abstract
{
    public class IdentityControllerBase : ControllerBase
    {
        protected readonly UserManager<PDMFUser> UserManager;
        protected readonly SignInManager<PDMFUser> SignInManager;

        public IdentityControllerBase(UserManager<PDMFUser> userManager, SignInManager<PDMFUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        protected async Task<ClaimsIdentity> GetIdentity(string email, string password)
        {
            var user = await UserManager.FindByEmailAsync(email);
            
            if (user == null)
            {
                return null;
            }
            
            var passwordCheck = await SignInManager.CheckPasswordSignInAsync(user, password, false);

            if (!passwordCheck.Succeeded)
            {
                return null;
            }
            
            var claims = new List<Claim>
            {
                new(ClaimsTypes.UserName, user.UserName),
                new(ClaimsTypes.Id, user.Id)
            };
            
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, 
                "Token", 
                ClaimsIdentity.DefaultNameClaimType, 
                ClaimsIdentity.DefaultRoleClaimType);
            
            return claimsIdentity;
        }
    }
}