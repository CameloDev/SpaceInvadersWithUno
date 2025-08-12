namespace SpaceInvadersWithUno;
    using SpaceInvadersWithUno.Models;
    using Microsoft.UI.Xaml.Shapes;
    using Microsoft.UI;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media.Imaging;

public class GerenciadorInimigos
{
    private readonly PaginaJogo _paginaJogo;
    private readonly SoundPlayer _som;
    public List<Inimigo> Inimigos { get; private set; }
    private Dictionary<Inimigo, Rectangle> _mapaUIInimigos;
    private double _direcaoInimigos = 1;
    private double _velocidadeInimigos = 0.5;
    private GerenciadorProjeteis _gerenciadorProjeteis;

    public GerenciadorInimigos(PaginaJogo paginaJogo, SoundPlayer som, GerenciadorProjeteis gerenciadorProjeteis)
    {
        _paginaJogo = paginaJogo;
        _som = som;
        Inimigos = new List<Inimigo>();
        _mapaUIInimigos = new Dictionary<Inimigo, Rectangle>();
        _gerenciadorProjeteis = gerenciadorProjeteis;
    }

    public void InicializarOnda(int numeroOnda)
    {
        LimparInimigos();

        for (int linha = 0; linha < 5; linha++)
        {
            for (int coluna = 0; coluna < 7; coluna++)
            {
                var inimigo = CriarInimigo(linha, coluna, numeroOnda);
                var inimigoUI = CriarUIInimigo(inimigo);

                _paginaJogo.AdicionarAoCanvas(inimigoUI, inimigo.PosicaoX, inimigo.PosicaoY);

                Inimigos.Add(inimigo);
                _mapaUIInimigos[inimigo] = inimigoUI;
            }
        }
    }

    public void Atualizar()
    {
        MoverInimigos();
        TentarAtirar();
    }

    private void MoverInimigos()
    {
        bool deveMoverParaBaixo = false;
        double novaDirecao = _direcaoInimigos;

        foreach (var inimigo in Inimigos)
        {
            var inimigoUI = _mapaUIInimigos[inimigo];
            if ((inimigo.PosicaoX + inimigo.Largura > _paginaJogo.LarguraCanvas && _direcaoInimigos > 0) ||
                (inimigo.PosicaoX < 0 && _direcaoInimigos < 0))
            {
                deveMoverParaBaixo = true;
                novaDirecao = -_direcaoInimigos;
                break;
            }
        }

        foreach (var inimigo in Inimigos)
        {
            var inimigoUI = _mapaUIInimigos[inimigo];

            double novaPosicaoX = inimigo.PosicaoX + _velocidadeInimigos * _direcaoInimigos;
            double novaPosicaoY = inimigo.PosicaoY;

            if (deveMoverParaBaixo)
            {
                novaPosicaoY += 20;
            }

            inimigo.PosicaoX = novaPosicaoX;
            inimigo.PosicaoY = novaPosicaoY;
            _paginaJogo.DefinirPosicaoUI(inimigoUI, inimigo.PosicaoX, inimigo.PosicaoY);
        }


        if (deveMoverParaBaixo)
        {
            _direcaoInimigos = novaDirecao;
        }
    }

    public bool InimigosChegaramAbaixo()
    {
        return Inimigos.Any(i => i.PosicaoY + i.Altura > _paginaJogo.PosicaoYJogador);
    }

    public void RemoverInimigo(Inimigo inimigo)
    {
        if (_mapaUIInimigos.TryGetValue(inimigo, out var uiElement))
        {
            _paginaJogo.RemoverDoCanvas(uiElement);
            _mapaUIInimigos.Remove(inimigo);
        }
        Inimigos.Remove(inimigo);
    }

    private Inimigo CriarInimigo(int linha, int coluna, int numeroOnda)
    {
        return new Inimigo
        {
            PosicaoX = 50 + coluna * 50,
            PosicaoY = 50 + linha * 30 + (numeroOnda * 10),
            Pontuacao = linha == 0 ? 50 : (linha == 1 || linha == 2 ? 20 : 10),
            Largura = 40,
            Altura = 20
        };

    }
    private Inimigo CriarInimigoOVNI(int linha, int coluna, int numeroOnda)
    {
        Random rdn = new Random();
        int randomNumber = rdn.Next(100, 200);
        return new Inimigo
        {
            PosicaoX = coluna,
            PosicaoY = linha,
            Pontuacao = randomNumber,
            Largura = 60,
            Altura = 30
        };
    } // desenvolvendo

    private Rectangle CriarUIInimigo(Inimigo inimigo)
    {
        return new Rectangle
        {
            Width = inimigo.Largura,
            Height = inimigo.Altura,
            Fill = inimigo.Pontuacao == 50 ? new SolidColorBrush(Colors.Red) :
                  (inimigo.Pontuacao == 20 ? new SolidColorBrush(Colors.Yellow) :
                   new SolidColorBrush(Colors.Green)),
            Tag = inimigo.Pontuacao.ToString()
        };
    }
    private Rectangle CriarUIInimigoOVNI(Inimigo inimigo)
    {
        return new Rectangle
        {
            Width = inimigo.Largura,
            Height = inimigo.Altura,
            Fill = new SolidColorBrush(Colors.Gray),
            Tag = inimigo.Pontuacao.ToString()
        };
        // desenvolvendo
    }

    private void TentarAtirar()
    {
        if (new Random().Next(100) > 98)
        {
            Atirar();
        }
    }

    private void Atirar()
    {
        if (Inimigos.Count == 0) return;

        var atirador = Inimigos[new Random().Next(Inimigos.Count)];
        _gerenciadorProjeteis.AdicionarProjetilInimigo(atirador);
        _som.TocarSom("Assets/shootInimigo.wav");
    }

    private void LimparInimigos()
    {
        foreach (var inimigo in Inimigos.ToList())
        {
            RemoverInimigo(inimigo);
        }
    }
}