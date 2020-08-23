using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CrossingFingers_Wave
{
    public sealed partial class MainPage : Page
    {
        private WaveEngine.Adapter.Application application;

        public MainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            application = new GameRenderer(SwapChainPanel);
            application.Initialize();
        }
    }
}
