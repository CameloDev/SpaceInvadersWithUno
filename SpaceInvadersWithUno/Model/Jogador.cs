namespace SpaceInvadersWithUno.Models;

    public class Jogador
    {
        public double PosicaoX { get; set; }
        public double PosicaoY { get; set; }
        public double Largura { get; set; } = 40;
        public double Altura { get; set; } = 20;
        private int _vidas { get; set; } = 3;
        public int Vidas
        {
            get => _vidas;
            set => _vidas = Math.Clamp(value, 0, 6);
        }
        public bool EstaVivo => Vidas > 0;
    }