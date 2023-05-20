using System.Windows;
using ViewModel;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUIServices
    {
        ViewData viewData;
        public MainWindow()
        {
            InitializeComponent();
            viewData = new ViewData(this);
            DataContext = viewData;
        }

        public void ReportError(string message)
        {
            MessageBox.Show($"Error:\n" + message);
        }

    }
}
