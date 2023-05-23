using System;
using System.Windows;
using ViewModel;
using Microsoft.Win32;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUIServices, FSServices
    {
        ViewData viewData;
        public MainWindow()
        {
            InitializeComponent();
            viewData = new ViewData(this, this);
            DataContext = viewData;
        }

        public void ReportError(string message)
        {
            MessageBox.Show($"Error:\n" + message);
        }

        public string Get_Save_Filename()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
                return dlg.FileName;
            return null;
        }
        public string Get_Load_Filename()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
                return dlg.FileName;
            return null;
        }
    }
}
