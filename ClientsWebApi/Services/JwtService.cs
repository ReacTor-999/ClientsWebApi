using ClientsWebApi.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClientsWebApi.Services
{
    public class JwtService
    {
        private static JwtSecurityTokenHandler TokenHandler => new();

        private readonly JwtSettings _settings;
        private readonly byte[] _key;

        public JwtService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;

            ArgumentNullException.ThrowIfNull(_settings);
            ArgumentNullException.ThrowIfNull(_settings.SigningKey);
            ArgumentNullException.ThrowIfNull(_settings.Issuer);

            _settings.ExpiryInMinutes ??= 5;

            _key = Convert.FromBase64String(_settings.SigningKey);
        }


        public SecurityToken CreateSecurityToken(ClaimsIdentity identity)
        {
            var tokenDescriptor = GetSecurityTokenDescriptor(identity);

            return TokenHandler.CreateToken(tokenDescriptor);
        }


        public string WriteToken(SecurityToken token)
        {
            return TokenHandler.WriteToken(token);
        }


        private SecurityTokenDescriptor GetSecurityTokenDescriptor(ClaimsIdentity identity)
        {
            return new SecurityTokenDescriptor()
            {
                Subject = identity,
                Expires = DateTime.Now.AddMinutes((double)_settings.ExpiryInMinutes!),
                Issuer = _settings.Issuer,
                SigningCredentials = new SigningCredentials(
                    key: new SymmetricSecurityKey(_key),
                    algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
        }
    }
}
