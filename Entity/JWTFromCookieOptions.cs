using Microsoft.Extensions.Configuration;

public class JWTFromCookieOptions
{
    public IConfiguration Configuration {get;set;} 
    public   ITokenGenerator TokenGenerator {get;set;}
}