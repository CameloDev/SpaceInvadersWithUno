namespace SpaceInvadersWithUno;

    using Windows.Foundation;
    using Microsoft.UI.Xaml.Shapes;
    using System.Runtime.InteropServices.WindowsRuntime;
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
            var retProjetil = ExpandirRetangulo(new Rect(projetilJogador.X, projetilJogador.Y,
                                                        projetilJogador.Largura, projetilJogador.Altura), 4);

            var ovni = _gerenciadorInimigos.ObterOVNI();
            if (ovni != null)
            {
                var retOVNI = new Rect(ovni.PosicaoX, ovni.PosicaoY, ovni.Largura, ovni.Altura);
                var intersecOVNI = retProjetil;
                intersecOVNI.Intersect(retOVNI);

                if (!intersecOVNI.IsEmpty)
                {
                    double x = ovni.PosicaoX;
                    double y = ovni.PosicaoY;
                    _gerenciadorInimigos.RemoverOVNI();
                    _gerenciadorProjeteis.RemoverProjetilJogador();
                    AdicionarPontuacao(ovni.Pontuacao);
                    _paginaJogo.MostrarExplosao(x, y);
                    return;
                }
            }

            foreach (var inimigo in _gerenciadorInimigos.Inimigos.ToList())
            {
                var retInimigo = new Rect(inimigo.PosicaoX, inimigo.PosicaoY,
                                        inimigo.Largura, inimigo.Altura);

                var intersec = retProjetil;
                intersec.Intersect(retInimigo);

                if (!intersec.IsEmpty)
                {
                    double x = inimigo.PosicaoX;
                    double y = inimigo.PosicaoY;
                    _gerenciadorInimigos.RemoverInimigo(inimigo);
                    _gerenciadorProjeteis.RemoverProjetilJogador();
                    AdicionarPontuacao(inimigo.Pontuacao);
                    _paginaJogo.MostrarExplosao(x, y);
                    break;
                }
            }
            foreach (var barreira in _gerenciadorBarreiras.Barreiras
                                            .OrderBy(b => b.PosicaoX)
                                            .ToList())
            {
                var retBarreira = new Rect(barreira.PosicaoX, barreira.PosicaoY,
                                            barreira.Largura, barreira.Altura);

                var intersec = retProjetil;
                intersec.Intersect(retBarreira);

                if (!intersec.IsEmpty && projetilJogador.ElementoUI is Rectangle)
                {
                    double escalaX = (double)barreira.Bitmap!.PixelWidth / barreira.Largura;
                    double escalaY = (double)barreira.Bitmap.PixelHeight / barreira.Altura;

                    double impactoLocalXraw = (projetilJogador.X + projetilJogador.Largura / 2 - barreira.PosicaoX) * escalaX;
                    int impactoLocalX = Math.Clamp((int)Math.Round(impactoLocalXraw), 0, barreira.Bitmap.PixelWidth - 1);

                    double impactoLocalYraw = (projetilJogador.Y - barreira.PosicaoY) * escalaY;
                    int impactoLocalY = Math.Clamp((int)Math.Round(impactoLocalYraw), 0, barreira.Bitmap.PixelHeight - 1);

                    int largura = barreira.Bitmap.PixelWidth;
                    int altura = barreira.Bitmap.PixelHeight;

                    if (impactoLocalX < 0 || impactoLocalX >= largura || impactoLocalY < 0 || impactoLocalY >= altura)
                        continue;

                    using (var buffer = barreira.Bitmap.PixelBuffer.AsStream())
                    {
                        int tamanhoBuffer = largura * altura * 4;

                      
                        for (int y = impactoLocalY; y >= 0; y--)
                        {
                            int offset = (y * largura + impactoLocalX) * 4;
                            if (offset + 4 > tamanhoBuffer)
                                break;

                            buffer.Seek(offset, SeekOrigin.Begin);
                            byte[] pixel = new byte[4];
                            int bytesLidos = buffer.Read(pixel, 0, 4);
                            if (bytesLidos != 4)
                                break;

                            byte alpha = pixel[3];

                            if (alpha > 0)
                            {
                                var pontoImpactoNoLoop = new Point(impactoLocalX, y);

                                _gerenciadorBarreiras.ReceberDano(barreira, pontoImpactoNoLoop);
                                _gerenciadorProjeteis.RemoverProjetilJogador();
                                goto fimLoop;
                            }
                        }
                    }
                }
            fimLoop:;
            }
        }

        foreach (var projetil in _gerenciadorProjeteis.ProjeteisInimigos.ToList())
        {
            var retProjetil = ExpandirRetangulo(new Rect(projetil.X, projetil.Y,
                                                        projetil.Largura, projetil.Altura), 4);

            foreach (var barreira in _gerenciadorBarreiras.Barreiras
                                            .OrderByDescending(b => b.PosicaoX)
                                            .ToList())
            {
                var retBarreira = new Rect(barreira.PosicaoX, barreira.PosicaoY,
                                        barreira.Largura, barreira.Altura);

                var intersec = retProjetil;
                intersec.Intersect(retBarreira);

                if (!intersec.IsEmpty && projetil.ElementoUI is Rectangle)
                {
                    double escalaX = (double)barreira.Bitmap!.PixelWidth / barreira.Largura;
                    double escalaY = (double)barreira.Bitmap.PixelHeight / barreira.Altura;

                    double impactoLocalXraw = (projetil.X + projetil.Largura / 2 - barreira.PosicaoX) * escalaX;
                    int impactoLocalX = (int)Math.Round(impactoLocalXraw);
                    impactoLocalX = Math.Clamp(impactoLocalX, 0, barreira.Bitmap.PixelWidth - 1);

                    double impactoLocalYraw = (projetil.Y + projetil.Altura - barreira.PosicaoY) * escalaY;
                    int impactoLocalY = (int)Math.Round(impactoLocalYraw);
                    impactoLocalY = Math.Clamp(impactoLocalY, 0, barreira.Bitmap.PixelHeight - 1);

                    int largura = barreira.Bitmap.PixelWidth;
                    int altura = barreira.Bitmap.PixelHeight;

                    int x = (int)impactoLocalX;
                    int yStart = (int)impactoLocalY;

                    if (x < 0 || x >= largura)
                        continue;

                    if (yStart < 0 || yStart >= altura)
                        continue;

                    using (var buffer = barreira.Bitmap.PixelBuffer.AsStream())
                    {
                        int tamanhoBuffer = largura * altura * 4;

                        for (int y = yStart; y < altura; y++)
                        {
                            int offset = (y * largura + x) * 4;
                            if (offset + 4 > tamanhoBuffer)
                                break;

                            buffer.Seek(offset, SeekOrigin.Begin);
                            byte[] pixel = new byte[4];
                            int bytesLidos = buffer.Read(pixel, 0, 4);

                            if (bytesLidos != 4)
                                break;

                            byte alpha = pixel[3];

                            if (alpha > 0)
                            {
                                var pontoImpactoNoLoop = new Point(x, y);

                                _gerenciadorBarreiras.ReceberDano(barreira, pontoImpactoNoLoop);
                                _gerenciadorProjeteis.RemoverProjetilInimigo(projetil);
                                goto fimLoop;
                            }
                        }
                    }
                }
            fimLoop:;
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

    private Rect ExpandirRetangulo(Rect original, double pixels)
    {
        return new Rect(
            original.X - pixels,
            original.Y - pixels,
            original.Width + pixels * 2,
            original.Height + pixels * 2
        );
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