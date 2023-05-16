using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SocialMedia;

public class AuthOptions
{
    public const string ISSUER = "SocialMediaServer"; // издатель токена
    public const string AUDIENCE = "SocialMediaClient"; // потребитель токена
    const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
    public const int LIFETIME = 999; // время жизни токена - 1 минута
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}