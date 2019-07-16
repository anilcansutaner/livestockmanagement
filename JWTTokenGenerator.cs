using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class JWTTokenGenerator : ITokenGenerator
{
    IConfiguration _configuration;

    public static JWTTokenGenerator Instance(IConfiguration configuration)
    {
        return new JWTTokenGenerator(configuration);
    }
    public JWTTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    private EncryptingCredentials _encryptionParam = null;
    private EncryptingCredentials EncryptionParam
    {
        get
        {
            if (this._encryptionParam == null)
            {
                this._encryptionParam = new EncryptingCredentials(this.EncryptionSecurityKey, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512);
            }

            return this._encryptionParam;
        }
    }

    private SymmetricSecurityKey EncryptionSecurityKey
    {
        get
        {
            byte[] bEncryption = this.ConvertHexStringToByteArray(_configuration["Jwt:EncryptionKey"]);
            var retVal = new SymmetricSecurityKey(bEncryption);
            return retVal;
        }
    }

    private SymmetricSecurityKey SigningSecurityKey
    {
        get
        {

            byte[] bSignin = this.ConvertHexStringToByteArray(_configuration["Jwt:SigningKey"]);
            var retVal = new SymmetricSecurityKey(bSignin);
            return retVal;
        }
    }

    private SigningCredentials _signingParam;
    private SigningCredentials SigningParam
    {
        get
        {
            if (this._signingParam == null)
            {
                this._signingParam = new SigningCredentials(this.SigningSecurityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha512);
            }

            return this._signingParam;
        }
    }
    private byte[] ConvertHexStringToByteArray(string hexString)
    {
        if (hexString.Length % 2 != 0)
        {
            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
        }

        byte[] HexAsBytes = new byte[hexString.Length / 2];
        for (int index = 0; index < HexAsBytes.Length; index++)
        {
            string byteValue = hexString.Substring(index * 2, 2);
            HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return HexAsBytes;
    }


    
    public string GenerateToken(UserModelDTO user, bool isEncrypted = false)
    {
        var retVal = string.Empty;
        JwtSecurityToken jwtSecurityToken = null;

        var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
               };


        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

        if (isEncrypted)
           jwtSecurityToken = handler.CreateJwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Issuer"],
             new ClaimsIdentity(claims), DateTime.Now, DateTime.Now.AddMinutes(30), DateTime.Now, SigningParam, this.EncryptionParam);
        else
           jwtSecurityToken = handler.CreateJwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Issuer"],
             new ClaimsIdentity(claims), DateTime.Now, DateTime.Now.AddMinutes(30), DateTime.Now, SigningParam, null);


        retVal = handler.WriteToken(jwtSecurityToken);

        return retVal;
    }


    public void ExctractTokenInfo(string token)
    {
        SecurityToken securityToken = null;
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        handler.ValidateToken(token, this.CreateTokenValidationParameter(), out securityToken);
        var retVal = ((JwtSecurityToken)securityToken).Claims;
    }


    public TokenValidationParameters CreateTokenValidationParameter(bool isEncrypted =false)
    {
        TokenValidationParameters tokenValidationParameters = null;
   

        tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Issuer"],
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = this.SigningSecurityKey
        };

       if(isEncrypted)
         tokenValidationParameters.TokenDecryptionKey = this.EncryptionSecurityKey;

        return tokenValidationParameters;
    }
    public TokenValidationParameters CreateTokenValidationParameter(IConfiguration configuration)
    {
        _configuration = configuration;
        return CreateTokenValidationParameter(_configuration["Jwt:EncryptToken"]=="true");
    }
      public string GenerateToken(IConfiguration configuration,  UserModelDTO user)
    {
        _configuration = configuration;
        return GenerateToken(user,_configuration["Jwt:EncryptToken"]=="true");
    }
    public string GenerateToken(UserModelDTO userModel)
    {
        return GenerateToken(_configuration,userModel);
    }
}