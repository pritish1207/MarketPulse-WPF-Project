using System.Windows;
using MarketPulse.ViewModels; 

namespace MarketPulse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
            this.DataContext = new MainViewModel();
        }
    }
}