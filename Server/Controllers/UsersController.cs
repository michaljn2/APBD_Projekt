using BlazorApp1.Server.Models;
using BlazorApp1.Server.Services;
using BlazorApp1.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Projekt.Server.Controllers
{
    [Route("api/[controller]/watchlist")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;
        public UsersController(IUsersService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCompanyToWatchList (CompanyPost post)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _service.AddCompanyToWatchList(userId, post.IdCompany);
            await _service.SaveDatabase();
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetWatchList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _service.GetUsersWatchList(userId));
        }
        [HttpDelete("{ticker}")]
        [Authorize]
        public async Task<IActionResult> DeleteCompanyFromWatchList(string ticker)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _service.DeleteCompanyFromWatchList(ticker, userId);
            await _service.SaveDatabase(); 
            return NoContent();
        }

    }
}
