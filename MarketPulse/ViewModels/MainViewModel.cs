// ViewModels/MainViewModel.cs

using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel; 
using System.Threading.Tasks;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MarketPulse.Models;
using MarketPulse.Services;
using SkiaSharp;
using System.Windows; 

namespace MarketPulse.ViewModels
{
    
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly AlphaVantageService _alphaVantageService;
        private string _stockSymbol = "";

        public MainViewModel()
        {
            _alphaVantageService = new AlphaVantageService();
            Series = new ObservableCollection<ISeries>();
            
            XAxes = new[]
            {
    new Axis
    {
        Name = "Date",
        Labeler = value =>
        {
           
            if (value >= 0)
            {
                try
                {
                    return new DateTime((long)value).ToShortDateString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    
                    return "";
                }
            }
            
            return "";
        },
        UnitWidth = TimeSpan.FromDays(1).Ticks,
        MinStep = TimeSpan.FromDays(1).Ticks
    }
};

            AddStockCommand = new RelayCommand(async (param) => await LoadStockData(), (param) => !string.IsNullOrWhiteSpace(StockSymbol));
        }

        public string StockSymbol
        {
            get => _stockSymbol;
            set
            {
                _stockSymbol = value;
                OnPropertyChanged(nameof(StockSymbol));
            }
        }

        public ObservableCollection<ISeries> Series { get; set; }
        public Axis[] XAxes { get; set; }

        public ICommand AddStockCommand { get; }

        private async Task LoadStockData()
        {
            var dataPoints = (await _alphaVantageService.GetTimeSeriesDailyAsync(StockSymbol)).ToList();

            
            Application.Current.Dispatcher.Invoke(() =>
            {
                var lineSeries = new LineSeries<StockDataPoint>
                {
                    Name = StockSymbol.ToUpper(),
                    Values = dataPoints,
                    Mapping = (stockDataPoint, index) => new Coordinate(stockDataPoint.Date.Ticks, stockDataPoint.Close),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 2 },
                    GeometryStroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 4 },
                    GeometrySize = 8
                };

                Series.Clear();
                Series.Add(lineSeries);
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // --- HELPER CLASS --- //
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object? parameter) => _execute(parameter);
    }
}