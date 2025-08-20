namespace SpaceInvadersWithUno.Models;

    public class Inimigo
    {
        public double PosicaoX { get; set; }
        public double PosicaoY { get; set; }
        public double Largura { get; set; } = 80;
        public double Altura { get; set; } = 20;
        public int Pontuacao { get; set; } = 100;
    }
