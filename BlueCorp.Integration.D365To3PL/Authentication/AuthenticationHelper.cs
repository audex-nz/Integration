using System.IdentityModel.Tokens.Jwt;
using BlueCorp.Integration.D365To3PL.Configurations;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlueCorp.Integration.D365To3PL.Authentication
{
    public class AuthenticationHelper
    {
        private readonly OAuthSettings _settings;

        public AuthenticationHelper(IOptions<OAuthSettings> settings)
        {
            _settings = settings.Value;
        }

        // This function checks if the request is authenticated
        // by validating the Bearer token in the Authorization header
        // Recommended to make this in the API Management
        public bool IsAuthenticated(HttpRequestData req)
        {
            var authorizationHeader = req.Headers.GetValues("Authorization").FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return false;
            }

            var token = authorizationHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _settings.Authority,
                ValidAudience = _settings.ClientId,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}