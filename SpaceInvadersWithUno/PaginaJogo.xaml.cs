using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Input;
using Windows.UI;
using Windows.Foundation;
using Microsoft.UI;
using SpaceInvadersWithUno.Models;
using System.Diagnostics;
using Windows.Media.Core;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;

namespace SpaceInvadersWithUno;

public sealed partial class PaginaJogo : Page
{
    private GerenciadorJogo _gerenciadorJogo;
    private DispatcherTimer? _timerJogo;
    private GerenciadorJogador _gerenciadorJogador;
    private GerenciadorProjeteis _gerenciadorProjeteis;
    private GerenciadorInimigos _gerenciadorInimigos;
    private GerenciadorBarreiras _gerenciadorBarreiras;
    private SoundPlayer som = new SoundPlayer();

    public double LarguraCanvas => CanvasJogo.ActualWidth;
    public double AlturaCanvas => CanvasJogo.ActualHeight;
    public double PosicaoYJogador => Canvas.GetTop(RetanguloJogador);


    // in test -- particulas
    private const int quantidadeParticulas = 15; // adicionar ao model de particulas 
    private readonly Random aleatorio = new Random();
    private readonly List<Particula> particulas = new List<Particula>();

    public PaginaJogo()
    {
        this.InitializeComponent();

        // in test-- particulas
         CriarParticulas();

        _gerenciadorProjeteis = new GerenciadorProjeteis(this, som);
        _gerenciadorJogador = new GerenciadorJogador(this, som, _gerenciadorProjeteis);
        _gerenciadorInimigos = new GerenciadorInimigos(this, som, _gerenciadorProjeteis);
        _gerenciadorBarreiras = new GerenciadorBarreiras(this);
        _gerenciadorJogo = new GerenciadorJogo(this, _gerenciadorJogador, _gerenciadorInimigos, _gerenciadorProjeteis, _gerenciadorBarreiras);

        this.Loaded += (s, e) =>
        {
            CanvasJogo.Focus(FocusState.Programmatic);

            // in test -- particulas
            CriarParticulas();

        };
        this.KeyDown += Pagina_TeclaPressionada;
        this.KeyUp += Pagina_TeclaLiberada;

        InicializarJogo();
    }

    public void InicializarJogo()
    {
        _gerenciadorJogador.Inicializar();
        _gerenciadorJogo.InicializarJogo();
        IniciarLoopJogo();
        som.TocarSomEmLoop("Assets/musica_ambiente.wav");
    }

    private void IniciarLoopJogo()
    {
        _timerJogo = new DispatcherTimer();
        _timerJogo.Interval = TimeSpan.FromMilliseconds(16);
        _timerJogo.Tick += (s, e) =>
        {
            _gerenciadorJogo.LoopJogo();
            _gerenciadorJogador.Atualizar();
            _gerenciadorProjeteis.Atualizar();


            // in test -- particulas
            AtualizarParticulas();
        };
        _timerJogo.Start();
    }

    public void DefinirPosicaoJogador(double x, double y)
    {
        Canvas.SetLeft(RetanguloJogador, x);
        Canvas.SetTop(RetanguloJogador, y);
    }

    public void AdicionarAoCanvas(UIElement elemento, double x, double y)
    {
        if (!CanvasJogo.Children.Contains(elemento))
        {
            CanvasJogo.Children.Add(elemento);
        }
        Canvas.SetLeft(elemento, x);
        Canvas.SetTop(elemento, y);
    }

    public void RemoverDoCanvas(UIElement elemento)
    {
        if (CanvasJogo.Children.Contains(elemento))
        {
            CanvasJogo.Children.Remove(elemento);
        }
    }

    public void DefinirPosicaoUI(UIElement elemento, double x, double y)
    {
        Canvas.SetLeft(elemento, x);
        Canvas.SetTop(elemento, y);
    }

    public void AtualizarTextoPontuacao(int pontuacao)
    {
        TextoPontuacao.Text = $"PONTUAÇÃO: {pontuacao}";
    }

    public void AtualizarTextoVidas(int vidas)
    {
        TextoVidas.Text = $"VIDAS: {vidas}";
    }

    public void AtualizarTextoOnda(int onda)
    {
        TextoOnda.Text = $"ONDA: {onda}";
    }

    public async void PiscarJogador()
    {
        for (int i = 0; i < 3; i++)
        {
            RetanguloJogador.Opacity = 0.3;
            await Task.Delay(100);
            RetanguloJogador.Opacity = 1;
            await Task.Delay(100);
        }
    }

    public async void MostrarDialogoFimDeJogo(string motivo, int pontuacao)
    {
        if (_timerJogo != null) _timerJogo.Stop();

        var dialogo = new ContentDialog
        {
            Title = "Fim de Jogo",
            Content = $"Motivo: {motivo}\nPontuação final: {pontuacao}",
            PrimaryButtonText = "Jogar Novamente",
            CloseButtonText = "Sair",
        };
        if (this.Content is FrameworkElement frameworkElement)
        {
            dialogo.XamlRoot = frameworkElement.XamlRoot;
        }
        som.PararSomEmLoop();
        var resultado = await dialogo.ShowAsync();
        if (resultado == ContentDialogResult.Primary)
            Frame.Navigate(typeof(PaginaJogo));
        else
            Frame.Navigate(typeof(MainPage));
    }

    private void Pagina_TeclaPressionada(object sender, KeyRoutedEventArgs e)
    {
        _gerenciadorJogador.TeclaPressionada(e.Key);
        e.Handled = true;
    }

    private void Pagina_TeclaLiberada(object sender, KeyRoutedEventArgs e)
    {
        _gerenciadorJogador.TeclaLiberada(e.Key);
        e.Handled = true;
    }
    public async void MostrarExplosao(double x, double y)
    {
        var explosao = new Image
        {
            Width = 40,
            Height = 40,
            Source = new BitmapImage(new Uri("ms-appx:///Assets/explosao.png")),
            Stretch = Stretch.UniformToFill
        };

        AdicionarAoCanvas(explosao, x, y);

        som.TocarSom("Assets/explosao.wav");

        await Task.Delay(800);

        CanvasJogo.Children.Remove(explosao);
    }

    // nao faz parte do escopo do projeto -- extra
     private void CriarParticulas()
        {
            for (int i = 0; i < quantidadeParticulas; i++)
            {
                double angulo = aleatorio.NextDouble() * 2 * Math.PI;
                double velocidade = 2; 
                var particula = new Particula()
                {
                    Posicao = new Point(aleatorio.Next(0, (int)CanvasJogo.Width), aleatorio.Next(0, (int)CanvasJogo.Height)),
                    Velocidade = new Point(Math.Cos(angulo) * velocidade, Math.Sin(angulo) * velocidade),
                    Raio = aleatorio.Next(2, 4),
                    Cor = Colors.Gray
                };

                var circulo = new Ellipse
                {
                    Width = particula.Raio * 2,
                    Height = particula.Raio * 2,
                    Fill = new SolidColorBrush(particula.Cor)
                };

                Canvas.SetLeft(circulo, particula.Posicao.X - particula.Raio);
                Canvas.SetTop(circulo, particula.Posicao.Y - particula.Raio);

                particula.Elemento = circulo;
                particulas.Add(particula);
                CanvasJogo.Children.Add(circulo);
            }
        }

        private void AtualizarParticulas()
        {
           double largura = CanvasJogo.Width;
           double altura = CanvasJogo.Height;
            foreach (var particula in particulas)
            {
                particula.Posicao = new Point(
                    particula.Posicao.X + particula.Velocidade.X,
                    particula.Posicao.Y + particula.Velocidade.Y
                );

                if (particula.Posicao.X < 0) particula.Posicao = new Point(largura, particula.Posicao.Y);
                if (particula.Posicao.X > largura) particula.Posicao = new Point(0, particula.Posicao.Y);
                if (particula.Posicao.Y < 0) particula.Posicao = new Point(particula.Posicao.X, altura);
                if (particula.Posicao.Y > altura) particula.Posicao = new Point(particula.Posicao.X, 0);

                Canvas.SetLeft(particula.Elemento, particula.Posicao.X - particula.Raio);
                Canvas.SetTop(particula.Elemento, particula.Posicao.Y - particula.Raio);
            }
        }
}
