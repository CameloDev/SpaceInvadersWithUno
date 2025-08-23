namespace SpaceInvadersWithUno;

using Microsoft.UI;
using Microsoft.UI.Xaml.Shapes;

public sealed partial class MainPage : Page
{
    private Random random = new Random();
    private DispatcherTimer particleTimer;
    public MainPage()
    {
        this.InitializeComponent();
        particleTimer = new DispatcherTimer();
        particleTimer.Interval = TimeSpan.FromMilliseconds(100);
        particleTimer.Tick += ParticleTimer_Tick!;
        particleTimer.Start();
    }
    private void ParticleTimer_Tick(object sender, object e)
    {
        var star = new Ellipse
        {
            Width = 2,
            Height = 2,
            Fill = new SolidColorBrush(Colors.White),
            Opacity = 0.8
        };

        double x = random.Next(0, (int)ParticleCanvas.ActualWidth);
        double y = random.Next(0, (int)ParticleCanvas.ActualHeight);

        Canvas.SetLeft(star, x);
        Canvas.SetTop(star, y);

        ParticleCanvas.Children.Add(star);

        Task.Delay(3000).ContinueWith(_ =>
        {
            _ = (Task)Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ParticleCanvas.Children.Remove(star);
            });
        });
    }

    private void StartGame_Click(object sender, RoutedEventArgs e)
    {
        this.Frame.Navigate(typeof(PaginaJogo));
    }
    private void VerControles_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(PaginaControles));
    }
    private void VerPlacar_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(ScoresPage));
    }
}
