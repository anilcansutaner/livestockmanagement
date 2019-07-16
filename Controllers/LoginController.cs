using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace myFirstApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogHandler _logger;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILoginManager _loginManager;
        public LoginController(ILogHandler logger, ITokenGenerator tokenGenerator, ILoginManager loginManager)
        {
            _logger = logger;
            _tokenGenerator = tokenGenerator;
            _loginManager = loginManager;
        }

        [HttpPost]
        public IActionResult Post([FromBody] LoginModelDTO loginModel)
        {
            IActionResult response = Unauthorized();

            try
            {
                var user = _loginManager.Login(loginModel);
                var tokenString = string.Empty;

                if (user != null)
                {
                    tokenString = _tokenGenerator.GenerateToken(user);
                    response = NoContent();

                    var token = new AccessTokenDTO { access_token = tokenString, token_type = "JWT" };
                    var serializedObject = JsonConvert.SerializeObject(token);

                    Response.Cookies.Append("data-access-token", serializedObject,
                     new CookieOptions { HttpOnly = true, Expires = DateTime.UtcNow.AddMinutes(10) });
                }
                else
                {
                    response = Unauthorized();
                }

            }
            catch (System.Exception)
            {
                response = StatusCode(500);
            }

            return response;

        }

    }
}
