namespace SpaceInvadersWithUno;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }
    private void StartGame_Click(object sender, RoutedEventArgs e)
    {
        this.Frame.Navigate(typeof(PaginaJogo));
    }
}
