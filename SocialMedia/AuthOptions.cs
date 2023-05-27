using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SocialMedia;

public class AuthOptions
{
    public const string ISSUER = "SocialMediaServer";
    public const string AUDIENCE = "SocialMediaClient";
    const string KEY = "mysupersecret_secretkey!123";
    public const int LIFETIME = 999;
    
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}