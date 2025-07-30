namespace SpaceInvadersWithUno.Models;

    using Microsoft.UI.Xaml.Shapes;
    public class Barreira
    {
        public double PosicaoX { get; set; }
        public double PosicaoY { get; set; }

        public double Largura { get; set; } = 20;
        public double Altura { get; set; } = 15;
        public int Resistencia { get; set; } = 15;
        public Rectangle? ElementoUI { get; set; }
    }
