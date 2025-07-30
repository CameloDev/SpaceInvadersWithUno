namespace SpaceInvadersWithUno;

    using Models;
    using Microsoft.UI.Xaml.Shapes;
    using Microsoft.UI;

    public class GerenciadorProjeteis
    {
        private readonly PaginaJogo _paginaJogo;
        private readonly SoundPlayer _som;
        public Projetil? ProjetilJogador { get; private set; }
        public List<Projetil> ProjeteisInimigos { get; private set; }

        public GerenciadorProjeteis(PaginaJogo paginaJogo, SoundPlayer som)
        {
            ProjetilJogador = new Projetil();
            _paginaJogo = paginaJogo;
            _som = som;
            ProjeteisInimigos = new List<Projetil>();
        }

        public void Atualizar()
        {
            MoverProjetilJogador();
            MoverProjeteisInimigos();
        }

        public void AdicionarProjetilJogador(Projetil projetil)
        {
            ProjetilJogador = projetil;
            var uiProjetil = CriarUIProjetil(projetil);
            _paginaJogo.AdicionarAoCanvas(uiProjetil, projetil.X, projetil.Y);
            projetil.ElementoUI = uiProjetil;
        }

        public void AdicionarProjetilInimigo(Inimigo atirador)
        {
            var projetil = new Projetil
            {
                X = atirador.PosicaoX + atirador.Largura / 2 - 2,
                Y = atirador.PosicaoY + atirador.Altura,
                Largura = 4,
                Altura = 10,
                EhDoJogador = false
            };

            var uiProjetil = CriarUIProjetil(projetil);
            _paginaJogo.AdicionarAoCanvas(uiProjetil, projetil.X, projetil.Y);
            projetil.ElementoUI = uiProjetil;

            ProjeteisInimigos.Add(projetil);
        }

        private void MoverProjetilJogador()
        {
            if (ProjetilJogador == null || ProjetilJogador.ElementoUI == null)
            {
                return;
            }

            ProjetilJogador.Y -= 10;
            _paginaJogo.DefinirPosicaoUI(ProjetilJogador.ElementoUI, ProjetilJogador.X, ProjetilJogador.Y);

            if (ProjetilJogador.Y < 0)
            {
                RemoverProjetilJogador();
            }
        }

        private void MoverProjeteisInimigos()
        {
            foreach (var projetil in ProjeteisInimigos.ToList())
            {
                if (projetil.ElementoUI == null)
                    continue;

                projetil.Y += 5;
                _paginaJogo.DefinirPosicaoUI(projetil.ElementoUI, projetil.X, projetil.Y);

                if (projetil.Y > _paginaJogo.AlturaCanvas)
                {
                    RemoverProjetilInimigo(projetil);
                }
            }
        }

        public void RemoverProjetilJogador()
        {
            if (ProjetilJogador != null)
            {
                if (ProjetilJogador.ElementoUI != null)
                {
                    _paginaJogo.RemoverDoCanvas(ProjetilJogador.ElementoUI);
                }
                ProjetilJogador = null;
            }
        }


        public void RemoverProjetilInimigo(Projetil projetil)
        {
            if (projetil.ElementoUI != null)
            {
                _paginaJogo.RemoverDoCanvas(projetil.ElementoUI);
            }
            ProjeteisInimigos.Remove(projetil);
        }

        public void LimparProjeteis()
        {
            RemoverProjetilJogador();

            foreach (var projetil in ProjeteisInimigos.ToList())
            {
                RemoverProjetilInimigo(projetil);
            }
        }

        private Rectangle CriarUIProjetil(Projetil projetil)
        {
            return new Rectangle
            {
                Width = projetil.Largura,
                Height = projetil.Altura,
                Fill = new SolidColorBrush(projetil.EhDoJogador ? Colors.White : Colors.Red),
                Tag = projetil.EhDoJogador ? "projetilJogador" : "projetilInimigo"
            };
        }
    }
