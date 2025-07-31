namespace SpaceInvadersWithUno;

    using Windows.Foundation;
    using Microsoft.UI.Xaml.Shapes;
public class GerenciadorJogo
{
    private readonly PaginaJogo _paginaJogo;
    private readonly GerenciadorJogador _gerenciadorJogador;
    private readonly GerenciadorInimigos _gerenciadorInimigos;
    private readonly GerenciadorProjeteis _gerenciadorProjeteis;
    private readonly GerenciadorBarreiras _gerenciadorBarreiras;

    public int Pontuacao { get; private set; }
    public int NumeroOnda { get; private set; }
    public bool JogoTerminado { get; private set; }

    public GerenciadorJogo(PaginaJogo paginaJogo, GerenciadorJogador gerenciadorJogador, GerenciadorInimigos gerenciadorInimigos, GerenciadorProjeteis gerenciadorProjeteis, GerenciadorBarreiras gerenciadorBarreiras)
    {
        _paginaJogo = paginaJogo;
        _gerenciadorJogador = gerenciadorJogador;
        _gerenciadorInimigos = gerenciadorInimigos;
        _gerenciadorProjeteis = gerenciadorProjeteis;
        _gerenciadorBarreiras = gerenciadorBarreiras;
    }

    public void InicializarJogo()
    {
        Pontuacao = 0;
        NumeroOnda = 1;
        JogoTerminado = false;

        _gerenciadorJogador.Inicializar();
        _gerenciadorInimigos.InicializarOnda(NumeroOnda);
        _gerenciadorBarreiras.InicializarBarreiras();
        _gerenciadorProjeteis.LimparProjeteis();

        AtualizarUI();
    }

    public void LoopJogo()
    {
        if (JogoTerminado) return;

        _gerenciadorJogador.Atualizar();
        _gerenciadorInimigos.Atualizar();
        _gerenciadorProjeteis.Atualizar();

        VerificarColisoes();
        VerificarOndaCompleta();
        VerificarCondicoesFimDeJogo();
        AtualizarUI();
    }

    private void VerificarColisoes()
    {
        var projetilJogador = _gerenciadorProjeteis.ProjetilJogador;
        if (projetilJogador != null)
        {
            var retProjetil = new Rect(projetilJogador.X, projetilJogador.Y,
                                    projetilJogador.Largura, projetilJogador.Altura);

        
            foreach (var inimigo in _gerenciadorInimigos.Inimigos.ToList())
            {
                var retInimigo = new Rect(inimigo.PosicaoX, inimigo.PosicaoY,
                                        inimigo.Largura, inimigo.Altura);

                var intersec = retProjetil; 
                intersec.Intersect(retInimigo);

                if (!intersec.IsEmpty)
                {
                    _gerenciadorInimigos.RemoverInimigo(inimigo);
                    _gerenciadorProjeteis.RemoverProjetilJogador();
                    AdicionarPontuacao(inimigo.Pontuacao);
                    break;
                }

            }

            foreach (var barreira in _gerenciadorBarreiras.Barreiras.ToList())
            {
                var retBarreira = new Rect(barreira.PosicaoX, barreira.PosicaoY,
                                        barreira.Largura, barreira.Altura);

                var intersec = retProjetil;
                intersec.Intersect(retBarreira);

                if (!intersec.IsEmpty && projetilJogador.ElementoUI is Rectangle elementoUI)
                {
                    double escalaX = (double)barreira.Bitmap!.PixelWidth / barreira.Largura;
                    double escalaY = (double)barreira.Bitmap.PixelHeight / barreira.Altura;

                    double impactoLocalX = (projetilJogador.X - barreira.PosicaoX) * escalaX;
                    double impactoLocalY = (projetilJogador.Y - barreira.PosicaoY) * escalaY;

                    Point pontoImpactoLocal = new Point(impactoLocalX, impactoLocalY);

                    _gerenciadorBarreiras.ReceberDano(barreira, pontoImpactoLocal);
                    _gerenciadorProjeteis.RemoverProjetilJogador();
                    break;
                }
            }
        }

        foreach (var projetil in _gerenciadorProjeteis.ProjeteisInimigos.ToList())
        {
            var retProjetil = new Rect(projetil.X, projetil.Y,
                                    projetil.Largura, projetil.Altura);

            foreach (var barreira in _gerenciadorBarreiras.Barreiras.ToList())
            {
                var retBarreira = new Rect(barreira.PosicaoX, barreira.PosicaoY,
                                        barreira.Largura, barreira.Altura);

                var intersec = retProjetil;
                intersec.Intersect(retBarreira);

                if (!intersec.IsEmpty && projetil.ElementoUI is Rectangle elementoUI)
                {
                    double escalaX = (double)barreira.Bitmap!.PixelWidth / barreira.Largura;
                    double escalaY = (double)barreira.Bitmap.PixelHeight / barreira.Altura;

                    double impactoLocalX = (projetil.X - barreira.PosicaoX) * escalaX;
                    double impactoLocalY = (projetil.Y - barreira.PosicaoY) * escalaY;

                    Point pontoImpactoLocal = new Point(impactoLocalX, impactoLocalY);

                    _gerenciadorBarreiras.ReceberDano(barreira, pontoImpactoLocal);
                    _gerenciadorProjeteis.RemoverProjetilInimigo(projetil);
                    break;
                }
            }

            if (_gerenciadorProjeteis.ProjeteisInimigos.Contains(projetil))
            {
                var jogador = _gerenciadorJogador.Jogador;
                var retJogador = new Rect(jogador.PosicaoX, jogador.PosicaoY,
                                        jogador.Largura, jogador.Altura);

                var intersec = retProjetil;
                intersec.Intersect(retJogador);
                if (!intersec.IsEmpty)
                {
                    _gerenciadorProjeteis.RemoverProjetilInimigo(projetil);
                    _gerenciadorJogador.ReceberDano();

                    if (jogador.Vidas <= 0)
                    {
                        FimDeJogo("Vidas Insuficientes");
                        return;
                    }
                }
            }
        }
    }

    private void VerificarOndaCompleta()
    {
        if (_gerenciadorInimigos.Inimigos.Count == 0)
        {
            IniciarNovaOnda();
        }
    }

    private void IniciarNovaOnda()
    {
        NumeroOnda++;
        _gerenciadorInimigos.InicializarOnda(NumeroOnda);
        _gerenciadorBarreiras.InicializarBarreiras();
        _gerenciadorProjeteis.LimparProjeteis();
        AtualizarUI();
    }

    private void VerificarCondicoesFimDeJogo()
    {
        if (!_gerenciadorJogador.Jogador.EstaVivo)
        {
            FimDeJogo("Você morreu!");
        }
        else if (_gerenciadorInimigos.InimigosChegaramAbaixo())
        {
            FimDeJogo("Inimigos passaram suas defesas!");
        }
    }

    public void FimDeJogo(string motivo)
    {
        JogoTerminado = true;
        _paginaJogo.MostrarDialogoFimDeJogo(motivo, Pontuacao);
    }

    public void AdicionarPontuacao(int pontos)
    {
        Pontuacao += pontos;
        _gerenciadorJogador.VerificarVidaExtra(Pontuacao);
        AtualizarUI();
    }

    private void AtualizarUI()
    {
        _paginaJogo.AtualizarTextoPontuacao(Pontuacao);
        _paginaJogo.AtualizarTextoVidas(_gerenciadorJogador.Jogador.Vidas);
        _paginaJogo.AtualizarTextoOnda(NumeroOnda);
    }
}