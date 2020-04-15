using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using me.bellacall.Core.Data;
using me.bellacall.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace me.bellacall.Core.Controllers
{
    [SwaggerTag("<code style='color:red'>Аутентификация</code>")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public AuthController(
            UserManager<AspNetUser> userManager,
            SignInManager<AspNetUser> signInManager,
            IEmailSender emailSender,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает 403 Forbidden с сообщением
        /// </summary>
        [NonAction]
        public virtual ObjectResult Forbidden(object value)
        {
            return StatusCode(StatusCodes.Status403Forbidden, value);
        }

        /// <summary>
        /// Возвращает 403 Forbidden
        /// </summary>
        [NonAction]
        public virtual ForbidResult Forbidden()
        {
            return Forbid();
        }

        /// <summary>
        /// Возвращает токен аутентификации
        /// </summary>
        /// <param name="auth">Учетные данные</param>
        /// <param name="signingEncodingKey">Ключ</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/Auth
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Post(AuthModel auth, [FromServices] IJwtSigningEncodingKey signingEncodingKey)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(auth.Email);
                var result = user == null ? Microsoft.AspNetCore.Identity.SignInResult.NotAllowed : await _signInManager.PasswordSignInAsync(user, auth.Password, /*model.RememberMe*/false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Создаем утверждения для токена
                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.Integer64),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(AuthOptions.Company, (user.Level == AspNetUserLevel.Company ? user.Company_Id : auth.Company_Id ?? user.Company_Id).ToString()),
                        new Claim(AuthOptions.Level, user.Level.ToString())
                    };

                    // Генерируем JWT
                    var token = new JwtSecurityToken(
                        issuer: AuthOptions.Issuer,
                        audience: AuthOptions.Audience,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(AuthOptions.Lifetime),
                        signingCredentials: new SigningCredentials(signingEncodingKey.GetKey(), signingEncodingKey.SigningAlgorithm));

                    string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                    return jwtToken;
                }

                if (result.IsLockedOut) return Forbidden("User account locked out");
                else return Forbidden("Invalid login attempt");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    // Ключ для создания подписи (приватный)
    public interface IJwtSigningEncodingKey
    {
        string SigningAlgorithm { get; }

        SecurityKey GetKey();
    }

    // Ключ для проверки подписи (публичный)
    public interface IJwtSigningDecodingKey
    {
        SecurityKey GetKey();
    }

    public class SigningSymmetricKey : IJwtSigningEncodingKey, IJwtSigningDecodingKey
    {
        private readonly SymmetricSecurityKey _secretKey;

        public string SigningAlgorithm { get; } = SecurityAlgorithms.HmacSha256;

        public SigningSymmetricKey(string key)
        {
            this._secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        public SecurityKey GetKey() => this._secretKey;
    }

    public class AuthOptions
    {
        public const string Issuer = "me.bellacall.Core"; // издатель токена
        public const string Audience = "http://lk.bellacall.me/"; // потребитель токена
        public const int Lifetime = 60 * (8 + 1); // время жизни токена (в минутах)
        public const string SchemeName = "JwtBearer";
        public const string Company = "Company";
        public const string Level = "Level";

        const string Key = "105c8a80-4745-4b31-9021-cedb5e238fff";   // ключ
        public static SigningSymmetricKey SigningSymmetricKey { get => new SigningSymmetricKey(Key); }
    }
}
