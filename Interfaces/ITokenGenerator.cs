using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public interface ITokenGenerator
{
    string GenerateToken(IConfiguration configuration, UserModelDTO userModel);
    string GenerateToken(UserModelDTO userModel);

    TokenValidationParameters CreateTokenValidationParameter(IConfiguration configuration);
}