// Services/AlphaVantageService.cs
using MarketPulse.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MarketPulse.Services
{
    public class AlphaVantageService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        
        private const string ApiKey = "EUMJCQTF6PKSSRM7";

        public async Task<IEnumerable<StockDataPoint>> GetTimeSeriesDailyAsync(string symbol)
        {
            var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={ApiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<StockDataPoint>();
            }

            string jsonString = await response.Content.ReadAsStringAsync();
            var rawData = JsonConvert.DeserializeObject<dynamic>(jsonString);

            if (rawData == null || rawData["Error Message"] != null)
            {
                return Enumerable.Empty<StockDataPoint>();
            }

            var timeSeriesData = rawData["Time Series (Daily)"];
            if (timeSeriesData == null)
            {
                return Enumerable.Empty<StockDataPoint>();
            }

            var result = new List<StockDataPoint>();
            foreach (var item in timeSeriesData)
            {
                var dataPoint = new StockDataPoint
                {
                    
                    Date = DateTime.Parse(item.Name),
                    Close = (double)item.Value["4. close"]
                };
                result.Add(dataPoint);
            }

            return result.OrderBy(dp => dp.Date).ToList();
        }
    }
}