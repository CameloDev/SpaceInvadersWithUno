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
    private Inimigo? _ovni;
    private Rectangle? _ovniUI;
    private DispatcherTimer _timerOVNI;
    private double _direcaoOVNI = 1;
    public Inimigo? ObterOVNI() => _ovni;

    public GerenciadorInimigos(PaginaJogo paginaJogo, SoundPlayer som, GerenciadorProjeteis gerenciadorProjeteis)
    {
        _paginaJogo = paginaJogo;
        _som = som;
        Inimigos = new List<Inimigo>();
        _mapaUIInimigos = new Dictionary<Inimigo, Rectangle>();
        _gerenciadorProjeteis = gerenciadorProjeteis;
        _timerOVNI = new DispatcherTimer();
        _timerOVNI.Interval = TimeSpan.FromSeconds(10);
        _timerOVNI.Tick += (s, e) => SpawnOVNI();
        _timerOVNI.Start();
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
        AtualizarOVNI(); 
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
            Altura = 40
        };
    }
    public void RemoverOVNI()
    {
        if (_ovni != null && _ovniUI != null)
        {
            _paginaJogo.RemoverDoCanvas(_ovniUI);
            _ovni = null;
            _ovniUI = null;
        }
    }

    private Rectangle CriarUIInimigo(Inimigo inimigo)
    {
        return new Rectangle
        {
            Width = inimigo.Largura,
            Height = inimigo.Altura,
            Fill = inimigo.Pontuacao == 50 
                ? new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/alien3.png")) }
                : (inimigo.Pontuacao == 20
                    ? new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/alien2.png")) } 
                    : new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/alien1.png")) }), 
            Tag = inimigo.Pontuacao.ToString()
        };
    }
    private Inimigo CriarInimigoOVNI(double y, bool daEsquerda, double larguraCanvas)
    {
        Random rdn = new Random();
        int randomNumber = rdn.Next(100, 300);

        return new Inimigo
        {
            PosicaoX = daEsquerda ? -60 : larguraCanvas + 60,
            PosicaoY = y,
            Pontuacao = randomNumber,
            Largura = 60,
            Altura = 30,
        };
    }
    private void SpawnOVNI()
    {
        if (_ovni != null) return;
        Random rdn = new Random();
       
        double larguraCanvas = _paginaJogo.LarguraCanvas;
        bool daEsquerda = rdn.Next(0, 2) == 0;
        _ovni = CriarInimigoOVNI(20, daEsquerda, larguraCanvas);
        _direcaoOVNI = daEsquerda ? 1 : -1;
        _ovniUI = CriarUIInimigoOVNI(_ovni);

        _paginaJogo.AdicionarAoCanvas(_ovniUI, _ovni.PosicaoX, _ovni.PosicaoY);
    }
    private Rectangle CriarUIInimigoOVNI(Inimigo inimigo)
    {
        string caminhoImagem = "ms-appx:///Assets/alien4.png";

        return new Rectangle
        {
            Width = inimigo.Largura,
            Height = inimigo.Altura,
            Fill =new ImageBrush { ImageSource = new BitmapImage(new Uri(caminhoImagem)) },
            Tag = inimigo.Pontuacao.ToString()
        };
    }
    private void AtualizarOVNI()
    {
        if (_ovni == null) return;

        double velocidade = 2;

        _ovni.PosicaoX += velocidade * _direcaoOVNI;
        Canvas.SetLeft(_ovniUI, _ovni.PosicaoX);

        if (_ovni.PosicaoX < -_ovni.Largura || _ovni.PosicaoX > _paginaJogo.LarguraCanvas + _ovni.Largura)
        {
            _paginaJogo.RemoverDoCanvas(_ovniUI!);
            _ovni = null;
            _ovniUI = null;
        }
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