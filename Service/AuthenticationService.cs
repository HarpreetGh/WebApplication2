using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Models;

namespace Services {
    public interface ITokenInterface
    {
        public JwtPayload GenerateToken(User user);
        // bool ValidateToken(string token);
    }

    public class TokenService: ITokenInterface {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public JwtPayload GenerateToken(User user)
        {
            // var claims = new[]
            // {
            //     new Claim(ClaimTypes.Name, user.Name),
            //     new Claim(ClaimTypes.Email, user.Email),
            //     new Claim(ClaimTypes.Role, "Admin")
            // };

            // var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            // var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            //     _config["Jwt:Issuer"],
            //     claims,
            //     expires: DateTime.Now.AddMinutes(30),
            //     signingCredentials: creds);

            // return new JwtSecurityTokenHandler().WriteToken(token);

            if (_config["Jwt:Key"] is null) {
                throw new ArgumentNullException("Jwt:Key");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourIssuer",
                audience: "yourAudience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtPayload
            {
                { "token", new JwtSecurityTokenHandler().WriteToken(token) },
                { "expiration", token.ValidTo }
            };
        }

        // public bool ValidateToken(string token)
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

        //     tokenHandler.ValidateToken(token, new TokenValidationParameters
        //     {
        //         ValidateIssuerSigningKey = true,
        //         IssuerSigningKey = new SymmetricSecurityKey(key),
        //         ValidateIssuer = false,
        //         ValidateAudience = false,
        //         ClockSkew = TimeSpan.Zero
        //     }, out SecurityToken validatedToken);

        //     return validatedToken != null;
        // }
    }
}