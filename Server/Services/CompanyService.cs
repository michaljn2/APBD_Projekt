using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Server.Models;
using BlazorApp1.Server.Data;
using System;
using BlazorApp1.Shared;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace BlazorApp1.Server.Services
{
    public class CompanyService : ICompanyService
    {
        private static DateTime begin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration; 
        public CompanyService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task AddCompany(CompanyDetails company)
        {
            await _context.Companies.AddAsync(new Company
            {
                IdCompany = company.IdCompany,
                Name = company.Name,
                City = company.City,
                Logo = company.Logo,
                Description = company.Description
            });
        }

        public async Task<bool> DoesCompanyExist(string companyId)
        {
            return await _context.Companies.AnyAsync(e => e.IdCompany == companyId);
        }

        public async Task<bool> DoesCompanyExistInDb(string ticker)
        {
            return await _context.Companies.AnyAsync(e => e.IdCompany == ticker);
        }

        public async Task<bool> DoesUserExist(string userId)
        {
            return await _context.Users.AnyAsync(e => e.Id == userId);
        }

        public async Task<ApplicationUser> GetApplicationUser(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.Id == userId);
        }

        public async Task<List<CompanyName>> GetCompaniesByName(string ticker)
        {
            List<CompanyName> companyNames = new List<CompanyName>();
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://api.polygon.io/v3/reference/tickers?search=" + ticker + "&active=true&sort=ticker&order=asc&limit=1000&apiKey=" + _configuration.GetConnectionString("PolygonKey"));
                    var responseBody = await response.Content.ReadAsStringAsync();
                    JsonDocument jsonDocument = JsonDocument.Parse(responseBody);
                
                    JsonElement companies = jsonDocument.RootElement.GetProperty("results");
                    foreach (var company in companies.EnumerateArray())
                    {
                        try
                        {

                            companyNames.Add(new CompanyName
                            {
                                Ticker = company.GetProperty("ticker").ToString(),
                                Name = company.GetProperty("name").ToString()
                            });
                        }
                        catch (Exception ex) when (ex is InvalidOperationException || ex is KeyNotFoundException) { continue; }
                    }
                }
                catch (Exception) 
                {
                    if (ticker != null && await _context.Companies.AnyAsync(e => e.IdCompany.ToUpper().Contains(ticker.ToUpper())))
                    {
                        companyNames = await _context.Companies.Where(e => e.IdCompany.ToUpper().Contains(ticker.ToUpper())).Select(e => new CompanyName
                        {
                            Name = e.Name,
                            Ticker = e.IdCompany
                        }).ToListAsync();
                    }
                }
            }
            return companyNames;
        }

        public async Task<Company> GetCompany(string companyId)
        {
            return await _context.Companies.FirstOrDefaultAsync(e => e.IdCompany == companyId);
        }

        public async Task<CompanyDetails> GetCompanyDetails(string ticker)
        {

            CompanyDetails companyDetails = new CompanyDetails();
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://api.polygon.io/v3/reference/tickers/" + ticker + "?apiKey=" + _configuration.GetConnectionString("PolygonKey"));
                    var responseBody = await response.Content.ReadAsStringAsync();
                    JsonDocument jsonDocument = JsonDocument.Parse(responseBody);
                    JsonElement results = jsonDocument.RootElement.GetProperty("results");
                    string city = "";
                    string logo = "";
                    string description = "";
                    try
                    {
                        city = results.GetProperty("address").GetProperty("city").ToString();
                    }
                    catch (Exception)
                    {
                        city = "No city provided";
                    }

                    try
                    {
                        logo = results.GetProperty("branding").GetProperty("logo_url").ToString();
                    }
                    catch (Exception)
                    {
                        logo = "No logo provided";
                    }

                    try
                    {
                        description = results.GetProperty("sic_description").ToString();
                    }
                    catch (Exception)
                    {
                        description = "No description provided";
                    }



                    companyDetails = new CompanyDetails
                    {
                        IdCompany = results.GetProperty("ticker").ToString(),
                        Name = results.GetProperty("name").ToString(),
                        City = city,
                        Logo = logo,
                        Description = description
                    };
                }
                catch (Exception) { }
            }
            return companyDetails;
        }

        public async Task<CompanyDetails> GetCompanyDetailsFromDb(string ticker)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(e => e.IdCompany == ticker);
            return new CompanyDetails
            {
                IdCompany = company.IdCompany,
                Name = company.Name,
                Logo = company.Logo,
                City = company.City,
                Description = company.Description
            };
        }

        public async Task<StockChartsGet> GetDailyStockCharts(string ticker)
        {

            StockChartsGet stocks = new StockChartsGet();
            stocks.StockCharts = new List<StockCharts>();
            JsonDocument jsonDocument;
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://api.polygon.io/v2/aggs/ticker/" + ticker.ToUpper() + "/range/5/minute/" + DateTime.Now.Date.ToString("yyyy-MM-dd") +
                        "/" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "?adjusted=true&sort=asc&limit=120&apiKey=" + _configuration.GetConnectionString("PolygonKey"));
                    var responseBody = await response.Content.ReadAsStringAsync();
                    jsonDocument = JsonDocument.Parse(responseBody);

                    // zapisuje jsona do bazy danych
                    if (!await _context.CompanyDailyTrades.AnyAsync(e => e.IdCompany == ticker && e.DateTime == DateTime.Now.Date))
                    {
                        var charts = new CompanyDailyTrades
                        {
                            DateTime = DateTime.Now.Date,
                            IdCompany = ticker,
                            Json = responseBody
                        };
                        await _context.CompanyDailyTrades.AddAsync(charts);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var charts = await _context.CompanyDailyTrades.FirstOrDefaultAsync(e => e.IdCompany == ticker && e.DateTime == DateTime.Now.Date);
                        charts.Json = responseBody;
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    var chartsFromDb = await _context.CompanyDailyTrades.FirstOrDefaultAsync(e => e.IdCompany == ticker && e.DateTime == DateTime.Now.Date);
                    if (chartsFromDb == null)
                    {
                        return new StockChartsGet
                        {
                            ResultsCount = 0,
                            StockCharts = null
                        };
                    }
                    else
                    {
                        jsonDocument = JsonDocument.Parse(chartsFromDb.Json);
                    }
                }


                JsonElement status = jsonDocument.RootElement.GetProperty("status");
                JsonElement count = jsonDocument.RootElement.GetProperty("resultsCount");
                stocks.ResultsCount = int.Parse(jsonDocument.RootElement.GetProperty("resultsCount").ToString()); 
                if (/*status.ToString() == "OK" && */stocks.ResultsCount > 0)
                {
                    JsonElement results = jsonDocument.RootElement.GetProperty("results");
                    foreach (var chart in results.EnumerateArray())
                    {
                        try
                        {
                            stocks.StockCharts.Add(new StockCharts
                            {
                                DateTime = DateTimeFromUnixTimestampMillis(long.Parse(chart.GetProperty("t").ToString())),
                                Open = double.Parse(chart.GetProperty("o").ToString(), CultureInfo.InvariantCulture),
                                Close = double.Parse(chart.GetProperty("c").ToString(), CultureInfo.InvariantCulture),
                                High = double.Parse(chart.GetProperty("h").ToString(), CultureInfo.InvariantCulture),
                                Low = double.Parse(chart.GetProperty("l").ToString(), CultureInfo.InvariantCulture),
                                Volume = double.Parse(chart.GetProperty("v").ToString(), CultureInfo.InvariantCulture)
                            });
                        }
                        catch (Exception ex) when (ex is InvalidOperationException || ex is KeyNotFoundException) { continue; }
                    }
                }
            }
            return stocks;
        }

        public async Task SaveDatabase()
        {
            await _context.SaveChangesAsync();
        }

        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return begin.AddMilliseconds(millis);
        }

        public async Task<StockChartsGet> GetStockCharts(string ticker, string start, string end)
        {
            StockChartsGet stocks = new StockChartsGet();
            stocks.StockCharts = new List<StockCharts>();
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://api.polygon.io/v2/aggs/ticker/" + ticker.ToUpper() + "/range/12/hour/" + start +
                        "/" + end + "?adjusted=true&sort=asc&&apiKey=" + _configuration.GetConnectionString("PolygonKey"));
                    var responseBody = await response.Content.ReadAsStringAsync();
                    JsonDocument jsonDocument = JsonDocument.Parse(responseBody);
                    stocks.ResultsCount = int.Parse(jsonDocument.RootElement.GetProperty("resultsCount").ToString());
                    if (stocks.ResultsCount != 0)
                    {
                        JsonElement results = jsonDocument.RootElement.GetProperty("results");
                        stocks.StockCharts = new List<StockCharts>();
                        foreach (var chart in results.EnumerateArray())
                        {
                            try
                            {
                                stocks.StockCharts.Add(new StockCharts
                                {
                                    DateTime = DateTimeFromUnixTimestampMillis(long.Parse(chart.GetProperty("t").ToString())),
                                    Open = double.Parse(chart.GetProperty("o").ToString(), CultureInfo.InvariantCulture),
                                    Close = double.Parse(chart.GetProperty("c").ToString(), CultureInfo.InvariantCulture),
                                    High = double.Parse(chart.GetProperty("h").ToString(), CultureInfo.InvariantCulture),
                                    Low = double.Parse(chart.GetProperty("l").ToString(), CultureInfo.InvariantCulture),
                                    Volume = double.Parse(chart.GetProperty("v").ToString(), CultureInfo.InvariantCulture)
                                });
                            }
                            catch (Exception ex) when (ex is InvalidOperationException || ex is KeyNotFoundException) { continue; }
                        }
                    }
                }
                catch (Exception)
                {
                    stocks.ResultsCount = 0;
                }
            }
            return stocks;
        }
    }
}

