namespace SpaceInvadersWithUno;
    using Models;
    using Microsoft.UI.Xaml.Shapes;
    using Microsoft.UI;

    public class GerenciadorBarreiras
    {
        private readonly PaginaJogo _paginaJogo;
        public List<Barreira> Barreiras { get; private set; }

        public GerenciadorBarreiras(PaginaJogo paginaJogo)
        {
            _paginaJogo = paginaJogo;
            Barreiras = new List<Barreira>();
        }

        public void InicializarBarreiras()
        {
            LimparBarreiras();

            double[] posicoesX = { 50, 120, 190, 260 };
            double posicaoY = 500;

            foreach (var posX in posicoesX)
            {
                var barreira = new Barreira
                {
                    PosicaoX = posX,
                    PosicaoY = posicaoY,
                    Resistencia = 3
                };

                var uiBarreira = CriarUIBarreira(barreira);
                _paginaJogo.AdicionarAoCanvas(uiBarreira, barreira.PosicaoX, barreira.PosicaoY);
                barreira.ElementoUI = uiBarreira;
                Barreiras.Add(barreira);
            }
        }

        public void ReceberDano(Barreira barreira)
        {
            barreira.Resistencia--;

            if (barreira.Resistencia <= 0)
            {
                RemoverBarreira(barreira);
            }
            else
            {
                AtualizarAparenciaBarreira(barreira);
            }
        }

        private void RemoverBarreira(Barreira barreira)
        {
            if (barreira.ElementoUI != null)
            {
                _paginaJogo.RemoverDoCanvas(barreira.ElementoUI);
            }
            Barreiras.Remove(barreira);
        }

        private void AtualizarAparenciaBarreira(Barreira barreira)
        {
            if (barreira.ElementoUI != null)
            {
                barreira.ElementoUI.Tag = barreira.Resistencia.ToString();
                barreira.ElementoUI.Fill = new SolidColorBrush(
                    barreira.Resistencia == 2 ? Colors.LightGray :
                    barreira.Resistencia == 1 ? Colors.DarkGray : Colors.White);
            }
        }

        private Rectangle CriarUIBarreira(Barreira barreira)
        {
            return new Rectangle
            {
                Width = barreira.Largura,
                Height = barreira.Altura,
                Fill = new SolidColorBrush(Colors.White),
                Tag = barreira.Resistencia.ToString()
            };
        }

        private void LimparBarreiras()
        {
            foreach (var barreira in Barreiras.ToList())
            {
                RemoverBarreira(barreira);
            }
        }
    }