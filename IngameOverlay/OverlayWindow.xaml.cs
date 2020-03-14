using System.Windows;

namespace IngameOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        public ViewModel.OverlayViewModel viewmodel;
        public OverlayWindow()
        {
            InitializeComponent();
            this.DataContext = viewmodel;
        }

        private void Window_Initialized(object sender, System.EventArgs e)
        {
            this.viewmodel = (ViewModel.OverlayViewModel)this.DataContext;
        }
    }
}