namespace SpaceInvadersWithUno;

using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;

    public class Particula
    {
        public Point Posicao { get; set; }
        public Point Velocidade { get; set; }
        public double Raio { get; set; }
        public Color Cor { get; set; }
        public Ellipse? Elemento { get; set; }
    }