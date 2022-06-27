using BlazorApp1.Server.Models;
using BlazorApp1.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Projekt.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompaniesController (ICompanyService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCompaniesByName(string ticker)
        {
            
            if(! ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _service.GetCompaniesByName(ticker));
        }

        [HttpGet("{ticker}")]
        [Authorize]
        public async Task<IActionResult> GetCompanyDetails(string ticker)
        {
            ticker = ticker.ToUpper();
            if(await _service.DoesCompanyExistInDb(ticker))
            {
                return Ok(await _service.GetCompanyDetailsFromDb(ticker));
            }
            else
            {
                var company = await _service.GetCompanyDetails(ticker);
                if (company.IdCompany != null)
                {
                    await _service.AddCompany(company);
                    await _service.SaveDatabase();
                }
                return Ok(company);
            }
        }
        [HttpGet("{ticker}/daily")]
        public async Task<IActionResult> GetDailyStockCharts(string ticker)
        {
            return Ok(await _service.GetDailyStockCharts(ticker));
        }

        [HttpGet("{ticker}/{start}/{end}")]
        public async Task<IActionResult> GetStocksCharts(string ticker, string start, string end)
        {
            return Ok(await _service.GetStockCharts(ticker, start, end));
        }
    }
}
