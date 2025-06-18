using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using VEHABANK.WebApi.Context;
using VEHABANK.WebApi.Entities;
using VEHABANK.WebApi.Service;
using VEHABANK.WebApi.Enums;
using Microsoft.CodeAnalysis.Operations;
using VEHABANK.Shared.Dto;

namespace VEHABANK.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        

        // Doğrulama kodu ve durumunu saklamak için yeni yapı
        private static Dictionary<string, (string Code, bool IsVerified,string NameSurname)> verificationStore = new();
        private static Dictionary<string, (string Code, bool IsVerified,string emplPassword)> verificationStore2 = new();

        public AuthsController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // 1. Doğrulama kodu gönderme
        [HttpPost("send-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] SendVerifactionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { Message = "E-posta adresi boş olamaz." });

            var code = new Random().Next(100000, 999999).ToString();

           
            verificationStore[dto.Email] = (code, false,dto.NameSurname);
            try 
            {
                await _emailService.SendVerificationCodeAsync(dto.Email, code, dto.NameSurname);

            }
            catch (Exception ex) 
            {
                // Hata detayını konsola yazdır
                Console.WriteLine($"Mail gönderme hatası: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { message = $"Mail gönderilemedi: {ex.Message}" });
            }


            return Ok(new { Message = "Doğrulama kodu gönderildi." });
        }
        // 1. Doğrulama kodu gönderme
        [HttpPost("send-password")]
        public async Task<IActionResult> SendEmployeePassword([FromBody] SendVerifactionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { Message = "E-posta adresi boş olamaz." });

            //sonradan
            var code = new Random().Next(100000, 999999).ToString();
            var employeePassword = "1j6m" + new Random().Next(100000, 999999).ToString() + "l1k3s";  
            verificationStore2[dto.Email] = (code, false, employeePassword);  // sonradan
           
            await _emailService.SendVerificationCodeAsync(dto.Email, code, false,employeePassword);

            return Ok(new { Message = "Parola gönderildi." });
        }

        // 2. Doğrulama kodu kontrol etme
        [HttpPost("verify-code")]
        public IActionResult VerifyCode([FromBody] VerifyCodeDto dto)
        {
            if (verificationStore.TryGetValue(dto.Email, out var entry))
            {
                if (dto.Code == entry.Code)
                {
                    verificationStore[dto.Email] = (entry.Code, true,""); // Kod doğrulandı


                    // Kullanıcı veritabanına daha önce eklendiyse güncelle
                    var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
                    if (user != null)
                    {
                        user.isActive = true;
                        user.IsVerified = true;
                        _context.SaveChanges();
                    }

                    return Ok(new { Message = "Doğrulama başarılı." });
                }
            }

            return BadRequest(new { Message = "Kod geçersiz veya süresi dolmuş." });
        }
       
        // 3. Kayıt işlemi
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Doğrulama yapılmış mı kontrol et
            if (!verificationStore.TryGetValue(dto.Email, out var entry) || !entry.IsVerified)
                return BadRequest(new { Message = "Lütfen önce e-posta doğrulamasını yapın." });

            DateOnly birthDate;
            bool isValidDate = DateOnly.TryParseExact(dto.BirthDate, "dd-MM-yyyy", out birthDate);

            if (!isValidDate)
                return BadRequest(new { Message = "Geçersiz tarih formatı. Lütfen 'dd-MM-yyyy' formatında girin." });

            if (!Enum.TryParse<AuthenticateQuestion>(dto.AuthenticateQuestion, out var parsedQuestion))
            {
                throw new ArgumentException("Geçersiz güvenlik sorusu.");
            }

            if (!Enum.TryParse<Enums.Branch>(dto.BranchName, out var parsedBranchId))
            {
                throw new ArgumentException("Geçersiz şube.");
            }
            var user = new User
            {
                Name = dto.Name,
                Surname = dto.Surname,
                BirthDate = birthDate,
                IdentityNumber = dto.IdentityNumber,
                Email = dto.Email,
                Phone = dto.Phone,
                BranchId = (int)parsedBranchId,
                isActive = true,
                IsVerified = true,
                CreatedAt = DateTime.Now,
                Logins = new List<Login>
                {
                    new Login
                    {
                        CustomerNumber = GenerateCustomerNumber(),
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                        Authorization = Enums.Authorization.Customer,
                        AuthenticateQuestion = parsedQuestion,
                        AuthenticateAnswerForQuestion = dto.AuthenticateAnswerForQuestion
                    }
                }
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            verificationStore.Remove(dto.Email); // doğrulama sürecini temizle

            return Ok(new { Message = "Kayıt başarılı" });
        }
        private string GenerateCustomerNumber()
        {
            var random = new Random();
            string number;
            do
            {
                number = random.Next(10000, 99999).ToString();
            }
            while (_context.Logins.Any(x => x.CustomerNumber == number));

            return number;
        }

        [HttpGet("GenerateAndGetCustomerNumber")]
        public IActionResult GenerateAndGetCustomerNumber()
        {
            string number = GenerateCustomerNumber(); // örneğin "12984"
            return Ok(number);
        }

    }
}
