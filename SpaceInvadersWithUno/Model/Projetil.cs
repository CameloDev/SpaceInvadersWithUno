using Microsoft.UI.Xaml.Shapes;
namespace SpaceInvadersWithUno.Models;
    public class Projetil
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool EhDoJogador { get; set; } = true;
        public double Largura { get; set; } = 5;
        public double Altura { get; set; } = 15;
        public Rectangle? ElementoUI { get; set; }
    }