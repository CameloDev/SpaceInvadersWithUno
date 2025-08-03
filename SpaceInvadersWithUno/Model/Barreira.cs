namespace SpaceInvadersWithUno.Models;

using Microsoft.UI.Xaml.Media.Imaging;
    public class Barreira
{
    public double PosicaoX { get; set; }
    public double PosicaoY { get; set; }

    public int Largura { get; set; } = 50;
    public int Altura { get; set; } = 25;
    public Image? ElementoUI { get; set; }
    public WriteableBitmap? Bitmap { get; set; }
}
