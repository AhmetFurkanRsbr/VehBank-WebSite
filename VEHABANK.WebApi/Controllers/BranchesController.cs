using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VEHABANK.WebApi.Context;
using VEHABANK.Shared.Dto;
using Microsoft.AspNetCore.Authorization;


namespace VEHABANK.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "BranchManager,SeniorManager")] // JWT'deki role bazlı kontrol
    public class BranchesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BranchesController(AppDbContext context)
        {
            _context = context;
        }
       

        [HttpGet("GetAllBranches")]
        //[AllowAnonymous]
        public IActionResult GetAllBranches()
        {
            var branches = _context.Branches.Select(b => new BranchDto
            {
                Id = b.Id,
                Name = b.Name
            }).ToList();

            return Ok(branches);
        }

        [HttpPost("UpdateBranch")]
        public async Task<IActionResult> UpdateBranch([FromBody] UpdateBranchDto dto)
        {
            var branch = await _context.Branches.FindAsync(dto.Id);
            if (branch == null)
            {
                return NotFound(new { Message = "Güncellenmek istenen Şube bulunamadı." });
            }

            branch.City = dto.City;
            branch.Email = dto.Email;
            branch.Phone = dto.Phone;
            branch.Address = dto.Address;

            
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Şube başarıyla güncellendi." });
        }


        [HttpGet("GetTotalBranchCount")]
        public IActionResult GetTotalBranchCount()
        {
            var count = _context.Branches.Count();
            return Ok(count);
        }
    }
}
