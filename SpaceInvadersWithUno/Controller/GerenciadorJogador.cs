namespace SpaceInvadersWithUno;
    using Models;
    public class GerenciadorJogador
    {
        private readonly PaginaJogo _paginaJogo;
        private readonly SoundPlayer _som;
        public Jogador Jogador { get; private set; }
        private bool _movendoEsquerda = false;
        private bool _movendoDireita = false;
        private const int VelocidadeJogador = 5;
        private GerenciadorProjeteis _gerenciadorProjeteis;

        public GerenciadorJogador(PaginaJogo paginaJogo, SoundPlayer som, GerenciadorProjeteis gerenciadorProjeteis)
    {
        Jogador = new Jogador();
        _paginaJogo = paginaJogo;
        _som = som;
        _gerenciadorProjeteis = gerenciadorProjeteis;
    }
        public void Inicializar()
        {
            Jogador = new Jogador
            {
                PosicaoX = 180,
                PosicaoY = 550,
                Vidas = 3
            };

            _paginaJogo.DefinirPosicaoJogador(Jogador.PosicaoX, Jogador.PosicaoY);
        }

        public void Atualizar()
        {
            MoverJogador();
        }

        private void MoverJogador()
        {
            double novaPosicao = Jogador.PosicaoX;

            if (_movendoEsquerda)
            {
                novaPosicao = Math.Max(0, Jogador.PosicaoX - VelocidadeJogador);
            }
            else if (_movendoDireita)
            {
                novaPosicao = Math.Min(_paginaJogo.LarguraCanvas - Jogador.Largura,
                                      Jogador.PosicaoX + VelocidadeJogador);
            }

            Jogador.PosicaoX = novaPosicao;
            _paginaJogo.DefinirPosicaoJogador(Jogador.PosicaoX, Jogador.PosicaoY);
        }

        public void Atirar()
        {
            if (_gerenciadorProjeteis.ProjetilJogador != null) return;

            var projetil = new Projetil
            {
                X = Jogador.PosicaoX + Jogador.Largura / 2 - 2.5,
                Y = Jogador.PosicaoY - Jogador.Altura,
                Largura = 5,
                Altura = 15,
                EhDoJogador = true
            };

            _gerenciadorProjeteis.AdicionarProjetilJogador(projetil);
            _som.TocarSom("Assets/shoot.wav");
        }
        

        public void ReceberDano()
        {
            Jogador.Vidas--;
            _paginaJogo.PiscarJogador();
        }

        public void VerificarVidaExtra(int pontuacao)
        {
            int vidasGanhas = pontuacao / 1000;
            int vidasExtras = vidasGanhas - (Jogador.Vidas - 3);

            if (vidasExtras > 0 && Jogador.Vidas < 6)
            {
                Jogador.Vidas += vidasExtras;
                _som.TocarSom("Assets/vida_extra.wav");
            } // arrumando erro, quando recebe o primeiro dano o verify esta recebendo +1 antes dos 1000
        }

        public void TeclaPressionada(Windows.System.VirtualKey tecla)
        {
            switch (tecla)
            {
                case Windows.System.VirtualKey.Left:
                    _movendoEsquerda = true;
                    break;
                case Windows.System.VirtualKey.Right:
                    _movendoDireita = true;
                    break;
                case Windows.System.VirtualKey.Space:
                    Atirar();
                    break;
            }
        }

        public void TeclaLiberada(Windows.System.VirtualKey tecla)
        {
            switch (tecla)
            {
                case Windows.System.VirtualKey.Left:
                    _movendoEsquerda = false;
                    break;
                case Windows.System.VirtualKey.Right:
                    _movendoDireita = false;
                    break;
            }
        }
    }