using System.Linq;
using NLog;

public class LoginManager : ILoginManager
{
    AzureDBContext _dbContext;
    ILogHandler _logHandler;

    public LoginManager(AzureDBContext dbContext, ILogHandler logHandler)
    {
        _dbContext = dbContext;
        _logHandler = logHandler;
    }
    public UserModelDTO Login(LoginModelDTO login)
    {
        UserModelDTO user = null;

        try
        {
            var dbRecord = _dbContext.UserInfo.FirstOrDefault(i => i.UserId.ToLower().Trim() == login.UserName.ToLower().Trim());

            if (dbRecord != null && !string.IsNullOrWhiteSpace(dbRecord.UserId))
            {
                if (dbRecord.HashedPassword == login.Password)
                {
                    user = new UserModelDTO
                    {
                        UserName = dbRecord.UserId,
                        Email = dbRecord.UserId,
                        Name = dbRecord.Name,
                        LastName = dbRecord.LastName
                    };
                }
            }
        }
        catch (System.Exception exc)
        {
            _logHandler.LogError(exc.Message);
            throw exc;
        }

        return user;
    }


}