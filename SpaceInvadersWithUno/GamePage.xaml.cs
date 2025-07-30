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

        public PaginaJogo()
        {
            this.InitializeComponent();
            _gerenciadorProjeteis = new GerenciadorProjeteis(this, som);
            _gerenciadorJogador = new GerenciadorJogador(this, som, _gerenciadorProjeteis);
            _gerenciadorInimigos = new GerenciadorInimigos(this, som, _gerenciadorProjeteis);
            _gerenciadorBarreiras = new GerenciadorBarreiras(this);
            _gerenciadorJogo = new GerenciadorJogo(this, _gerenciadorJogador, _gerenciadorInimigos, _gerenciadorProjeteis, _gerenciadorBarreiras);

            this.Loaded += (s, e) => CanvasJogo.Focus(FocusState.Programmatic);
            this.KeyDown += Pagina_TeclaPressionada;
            this.KeyUp += Pagina_TeclaLiberada;

            InicializarJogo();
        }

        public void InicializarJogo()
        {
            _gerenciadorJogador.Inicializar();
            _gerenciadorJogo.InicializarJogo();
            IniciarLoopJogo();
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
    }
