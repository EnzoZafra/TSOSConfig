using TSOSConfig.ViewModels;

namespace TSOSConfig
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainViewModel.Instance.OnWindowClosing;
        }
    }
}
