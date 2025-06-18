using Microsoft.AspNetCore.Mvc;
using VEHABANK.WebApi.Context;
using VEHABANK.Shared.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using VEHABANK.WebApi.Entities;
using VEHABANK.WebApi.Enums;
using VEHABANK.Shared.Dto;
using VEHABANK.WebApi.Service;

namespace VEHABANK.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "SeniorManager")]  // JWT'deki role bazlı kontrol
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public EmployeesController(AppDbContext context,IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Kullanıcıları listele
        [HttpGet]
        public IActionResult GetEmployees()
        {
            var employeeList = _context.Logins
                .Where(e => e.Authorization != Enums.Authorization.Customer).OrderBy(e =>e.Authorization)
                .Select(e => new EmployeeDto
                {
                    Id = e.User.Id,
                    Name = e.User.Name,
                    Surname =  e.User.Surname,
                    Email = e.User.Email,  
                    Phone = e.User.Phone,   
                    Authorization = (int)e.Authorization,
                    BranchId = e.User.BranchId,
                    BranchName = e.User.Branch.Name,
                    CustomerNumber = e.CustomerNumber            
                }).ToList();

            return Ok(employeeList);
        }

        [HttpPost]
        [Route("Update")]
        public IActionResult UpdateEmployee([FromBody] EmployeeUpdateDto employeeDto)
        {
            // ModelState kontrolü
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kullanıcıyı veritabanında buluyoruz
            var employee = _context.Logins
                .Include(x => x.User)
                .FirstOrDefault(x => x.User.Id == employeeDto.Id);

            if (employee == null || employee.User == null)
            {
                return NotFound(new { Message = "Güncellenmek istenen çalışan bulunamadı." });
            }

            employee.User.Name = employeeDto.Name;
            employee.User.Surname = employeeDto.Surname;
            employee.User.Email = employeeDto.Email;
            employee.User.Phone = employeeDto.Phone;
            employee.User.BranchId = employeeDto.BranchId;
            employee.CustomerNumber = employeeDto.CustomerNumber;
            employee.Authorization = (Authorization)employeeDto.Authorization;

            _context.SaveChanges();

            return Ok(new { Message = "Çalışan başarıyla güncellendi." });
        }

        // Personel silme
        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            
            var employee = _context.Logins
                .Include(x => x.User)
                .FirstOrDefault(e => e.User.Id == id);
            if (employee != null && employee.User!=null)
            {
                _context.Logins.Remove(employee);
                _context.Users.Remove(employee.User);
                _context.SaveChanges();
                return Ok(new { message = "Personel başarıyla silindi." });
            }

            return NotFound(new { message = "Personel bulunamadı." });

        }

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Rastgele şifre üret
            var generatedPassword = Guid.NewGuid().ToString("N")[..8]; // 8 karakterlik parola

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

            // Kullanıcı oluştur
            var user = new User
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                Phone = dto.Phone,
                IdentityNumber = dto.IdentityNumber,
                BirthDate = DateOnly.Parse(dto.BirthDate),
                BranchId = dto.BranchId,
                isActive = true,
                IsVerified = false,
                CreatedAt = DateTime.Now,
                Logins = new List<Login>
        {
            new Login
            {
                CustomerNumber = dto.CustomerNumber,
                PasswordHash = passwordHash,
                Authorization = (Authorization)dto.Authorization,
                AuthenticateQuestion = AuthenticateQuestion.En_Sevdiğin_Gezegen, // güvenlik sorusu kullanılmıyor
                AuthenticateAnswerForQuestion = "--Personel--"
            }
        }
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var code = new Random().Next(100000, 999999).ToString();
            // Şifreyi mail olarak gönder
            await _emailService.SendVerificationCodeAsync(dto.Email, code,false,generatedPassword);

            return Ok(new { Message = "Personel başarıyla eklendi ve şifresi gönderildi." });
        }

    }
}
