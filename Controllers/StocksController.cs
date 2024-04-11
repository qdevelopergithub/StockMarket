    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Newtonsoft.Json.Linq;
    using StockMarket.Models;
    using Alpaca.Markets;

    namespace StockMarket.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class StocksController : ControllerBase
        {
            private readonly HttpClient _httpClient;

            public StocksController(HttpClient httpClient)
            {
                _httpClient = httpClient;
                _httpClient.BaseAddress = new Uri("https://data.alpaca.markets/v2/");
                _httpClient.DefaultRequestHeaders.Add("APCA-API-KEY-ID", "PKPETELG3DY6Z5NUAO7P");
                _httpClient.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", "k0F5O8nzV1wIL0GbK8neSkkDRVeDtJxBTz8xK0UG");
            }

        [HttpGet]
        public async Task<ActionResult<stockModel>> CalculateYearlyReturnAsync()
        {
            try
            {
                var startDate = DateTime.Now.AddYears(-5).ToString("yyyy-MM-dd");
                var endDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                var timeframe = "1M";
                var dia = "DIA";
                var dji = "DJI";

                var response1 = await _httpClient.GetAsync($"stocks/{dji}/bars?timeframe={timeframe}&start={startDate}&end={endDate}");
                var response2 = await _httpClient.GetAsync($"stocks/{dia}/bars?timeframe={timeframe}&start={startDate}&end={endDate}");

                if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    var content1 = await response1.Content.ReadAsStringAsync();
                    var data1 = JObject.Parse(content1)["bars"]?.ToList();
                    var closingPrices1 = data1.Select(bar => (decimal)bar["c"]).ToList();
                    var firstPrice1 = closingPrices1.FirstOrDefault();
                    var lastPrice1 = closingPrices1.LastOrDefault();
                    var result1 = firstPrice1 != 0 ? ((lastPrice1 - firstPrice1) / firstPrice1 * 100) : 0;

                    var content2 = await response2.Content.ReadAsStringAsync();
                    var data2 = JObject.Parse(content2)["bars"]?.ToList();
                    var closingPrices2 = data2.Select(bar => (decimal)bar["c"]).ToList();
                    var firstPrice2 = closingPrices2.FirstOrDefault();
                    var lastPrice2 = closingPrices2.LastOrDefault();
                    var result2 = firstPrice2 != 0 ? ((lastPrice2 - firstPrice2) / firstPrice2 * 100) : 0;

                    var response = Math.Max(result1, result2);
                    var stockData = new stockModel
                    {
                        StockName = response == result1 ? dji : dia,
                        TotalReturn = response
                    };
                    return Ok(stockData);
                }
                else
                {
                    return NotFound("Failed to retrieve stock data. HTTP request returned non-success status code.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return StatusCode(500, "An error occurred while calculating yearly return.");
            }
        }

        [HttpGet("CalculatePotentialGrowth")]
        public IActionResult CalculatePotentialGrowth(decimal principalAmount)
        {
                try
                {
                    decimal P = principalAmount;
                    decimal r = 0.075m; // 7.5%
                    int n = 4;
                    int t = 50;     

                    decimal ratePerPeriod = (r / n);
                    int totalCompoundingPeriods = (n * t);

                    decimal poweredValue = 1 + ratePerPeriod;

                    decimal data = (decimal)Math.Pow((double)poweredValue, totalCompoundingPeriods);

                    decimal futureAmount = P * data;

                    return Ok(new { primcipalAmount =  principalAmount, Amount = futureAmount});
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return StatusCode(500, "An error occurred while calculating potential growth.");
                }
            }

      
    }
    }
