using LoginUser.Data;
using LoginUser.Models;
using LoginUser.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginUser.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly EmailService _email;

        public AuthController(
            AppDbContext context,
            EmailService email)
        {
            _context = context;
            _email = email;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Usuário criado");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == request.Email);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash))
            {
                return Unauthorized("Email ou senha inválidos");
            }

            var key = Encoding.UTF8.GetBytes(
                HttpContext.RequestServices
                    .GetRequiredService<IConfiguration>()
                    ["Jwt:Key"]!);

            var tokenDescriptor =
                new SecurityTokenDescriptor
                {
                    Subject =
                        new ClaimsIdentity(
                        [
                            new Claim(
                        ClaimTypes.NameIdentifier,
                        user.Id.ToString()),

                    new Claim(
                        ClaimTypes.Email,
                        user.Email)
                        ]),

                    Expires =
                        DateTime.UtcNow.AddHours(2),

                    Issuer =
                        HttpContext.RequestServices
                            .GetRequiredService<IConfiguration>()
                            ["Jwt:Issuer"],

                    Audience =
                        HttpContext.RequestServices
                            .GetRequiredService<IConfiguration>()
                            ["Jwt:Audience"],

                    SigningCredentials =
                        new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256)
                };

            var handler =
                new JwtSecurityTokenHandler();

            var token =
                handler.CreateToken(
                    tokenDescriptor);

            return Ok(new
            {
                token =
                    handler.WriteToken(token)
            });
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset(PasswordResetRequest request)
        {
            var user =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Email == request.Email);

            if (user == null)
            {
                return Ok();
            }

            var code =
                Random.Shared
                    .Next(100000, 999999)
                    .ToString();

            user.PasswordResetCode = code;

            user.PasswordResetExpiresAt =
                DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            await _email.SendAsync(
                user.Email,
                "Reset de senha",
                $"Seu código é: {code}");

            return Ok(
                "Código enviado");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ConfirmPasswordResetRequest request)
        {
            var user =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Email == request.Email);

            if (user == null)
                return BadRequest();

            if (
                user.PasswordResetCode != request.Code
                ||
                user.PasswordResetExpiresAt < DateTime.UtcNow
               )
            {
                return BadRequest(
                    "Código inválido");
            }

            user.PasswordHash =
                BCrypt.Net.BCrypt
                    .HashPassword(
                        request.NewPassword);

            user.PasswordResetCode = null;

            user.PasswordResetExpiresAt = null;

            await _context.SaveChangesAsync();

            return Ok("Senha alterada");
        }
    }
}