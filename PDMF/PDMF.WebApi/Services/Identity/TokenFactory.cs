using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using PDMF.WebApi.Models.Identity.Data;

namespace PDMF.WebApi.Services.Identity
{
    public static class TokenFactory
    {
        public static string CreateJWT(ClaimsIdentity identity)
        {
            var createDate = DateTime.UtcNow;
            
            var jwt = new JwtSecurityToken(
                JWTOptions.Issuer,
                JWTOptions.Audience,
                notBefore: createDate,
                claims: identity.Claims,
                expires: createDate.Add(TimeSpan.FromMinutes(JWTOptions.LifeTime)),
                signingCredentials: new SigningCredentials(JWTOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}