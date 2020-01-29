using FilesInfo.ViewModel;
using System.Windows;

namespace FilesInfo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new WindowViewModel();
        }
    }
}
