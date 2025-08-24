// Models/StockDataPoint.cs
using System;

namespace MarketPulse.Models
{
    public class StockDataPoint
    {
        public DateTime Date { get; set; }
        public double Close { get; set; }
    }
}