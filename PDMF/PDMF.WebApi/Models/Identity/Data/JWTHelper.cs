using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PDMF.WebApi.Models.Identity.Exceptions;

namespace PDMF.WebApi.Models.Identity.Data
{
    public static class JWTHelper
    {
        public static string GetJWTValue(string token, string key)
        {
            var handler = new JwtSecurityTokenHandler();
            
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JWTOptions.Key)),
                ValidateIssuer = true,
                ValidIssuer = JWTOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = JWTOptions.Audience
            };

            try
            {

                var claims = handler.ValidateToken(token, validations, out _);

                return claims.Claims.FirstOrDefault(claim => claim.Type == key)?.Value;
            }
            catch (Exception exception)
            {
                throw new InvalidJWTException(token, exception);
            }
        }

        public static string GetJWTFromHeader(string authorizeHeader)
        {
            var tokenValue = authorizeHeader.Replace("Bearer ", "");
            return tokenValue;
        }
    }
}