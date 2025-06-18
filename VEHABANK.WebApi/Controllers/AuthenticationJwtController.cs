using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VEHABANK.Shared.Dto;
using VEHABANK.WebApi.Context;
using VEHABANK.WebApi.Entities;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationJwtController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtSettingDto _jwtOptions;

    public AuthenticationJwtController(AppDbContext context, IOptions<JwtSettingDto> jwtOptions)
    {
        _context = context;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginDto request)
    {
        
        // Kullanıcıyı müşteri numarası veya TC kimlik numarası ile sorgula
        var login = _context.Logins
            .Include(x => x.User)
            .FirstOrDefault(x =>    
                x.CustomerNumber == request.Identifier ||
                x.User.IdentityNumber == request.Identifier
            );


        if (login == null)
            return Unauthorized(new { Message = "Kullanıcı bulunamadı." });

        // User nesnesinin null olup olmadığını kontrol et
        if (login.User == null)
            return Unauthorized(new { Message = "Kullanıcı bilgileri geçersiz." });

        // Şifre doğrulama (hash kontrolü)
        if (!VerifyPasswordHash(request.Password, login.PasswordHash))
            return Unauthorized(new { Message = "Şifre hatalı." });

        var token = GenerateJwtToken(login.User);

        // Kullanıcının ilk aktif hesabını bul
        var account = _context.Accounts.FirstOrDefault(a => a.UserId == login.User.Id && a.IsActive);
        int? accountId = account?.Id;
       
       
        var response = new
        {
            Token = token,
            AccountId = accountId,
            User = new
            {
                Id = login.User.Id,
                Name = login.User.Name,
                Surname = login.User.Surname,
                Email = login.User.Email
            }
        };
        return Ok(new { Token = token, UserId = login.User.Id,Response=response});
    }

    private string GenerateJwtToken(User user)
    {
        // Login tablosundan Authorization rolünü al
        var login = _context.Logins.FirstOrDefault(l => l.User.Id == user.Id);
        if (login == null)
            throw new Exception("Kullanıcının login bilgisi bulunamadı.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claim listesine rol eklendi
        var claims = new[]
        {
        new Claim("UserId", user.Id.ToString()),
        new Claim("Name", user.Name),
        new Claim("Surname", user.Surname),
        new Claim("Email", user.Email),
        new Claim("Phone", user.Phone),
        new Claim("IdentityNumber", user.IdentityNumber),
        new Claim(ClaimTypes.Role, login.Authorization.ToString()) //  Enum ismi role (örn: BankTeller)
    };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    private bool VerifyPasswordHash(string plainPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);
    }
    
}



































/*using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VEHABANK.WebApi.Context;
using VEHABANK.WebApi.Dto;
using VEHABANK.WebApi.Entities;

namespace VEHABANK.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationJwtController : ControllerBase
    {
        private readonly JwtSettingDto _jwtOptions;
        private readonly AppDbContext _context;
        public AuthenticationJwtController(IOptions<JwtSettingDto> jwtOptions, AppDbContext context)
        {
            _jwtOptions= jwtOptions.Value;
            _context = context;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] Login loginData)
        {
            var login = doAuthenticate(loginData);
            if (login == null)
            {
                return Unauthorized(new { Message = "Kullanıcı adı veya şifre hatalı." });
            }
            var token =  GenerateJwtToken(login);
            return Ok(token);
        }

        private string GenerateJwtToken(User user)
        {
            if (_jwtOptions.Key == null)
            {
                throw new Exception("Jwt ayarlarındaki Key değeri null olamaz");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claimArray = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Name", user.Name),
                new Claim("Surname", user.Surname),
                new Claim("IdentityNumber", user.IdentityNumber),
                new Claim("Email", user.Email),
                new Claim("Phone", user.Phone)
            };
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claimArray,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private Login? doAuthenticate(Login loginData)
        {
            return _context.Logins
                .FirstOrDefault(x =>
                    x.CustomerNumber.ToLower() == loginData.CustomerNumber.ToLower()
                    && x.PasswordHash == loginData.PasswordHash
                );
        }
    }
}
*/