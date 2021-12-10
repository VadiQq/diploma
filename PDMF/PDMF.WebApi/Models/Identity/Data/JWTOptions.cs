using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PDMF.WebApi.Models.Identity.Data
{
    public class JWTOptions
    {
        public const string Issuer = "PDMFissuer";
        public const string Audience = "PDMFaudience";
        public const string Key = "IHzNCHoy4TMLdqpiFFO4";
        public const int LifeTime = 60;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new(Encoding.ASCII.GetBytes(Key));
        }
    }
}