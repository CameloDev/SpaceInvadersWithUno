using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Microsoft.UI;

namespace SpaceInvadersWithUno;

public class GerenciadorParticulas
{
    private readonly Canvas _canvas;
    private readonly List<Particula> _particulas = new();
    private readonly Random _aleatorio = new();
    private readonly int _quantidade;

    public GerenciadorParticulas(Canvas canvas, int quantidade = 15)
    {
        _canvas = canvas;
        _quantidade = quantidade;
        CriarParticulas();
    }

    private void CriarParticulas()
    {
        for (int i = 0; i < _quantidade; i++)
        {
            double angulo = _aleatorio.NextDouble() * 2 * Math.PI;
            double velocidade = 2;
            var particula = new Particula()
            {
                Posicao = new Point(_aleatorio.Next(0, (int)_canvas.Width), _aleatorio.Next(0, (int)_canvas.Height)),
                Velocidade = new Point(Math.Cos(angulo) * velocidade, Math.Sin(angulo) * velocidade),
                Raio = _aleatorio.Next(2, 4),
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
            _particulas.Add(particula);
            _canvas.Children.Add(circulo);
        }
    }

    public void Atualizar(bool movendoEsquerda, bool movendoDireita)
    {
        double largura = _canvas.Width;
        double altura = _canvas.Height;

        double forcaX = 0;
        if (movendoEsquerda) forcaX = 1.5;
        else if (movendoDireita) forcaX = -1.5;

        foreach (var particula in _particulas)
        {
            if (forcaX == 0)
            {
                double ruidoX = (_aleatorio.NextDouble() - 0.5) * 0.3;
                double ruidoY = (_aleatorio.NextDouble() - 0.5) * 0.3;
                particula.Posicao = new Point(
                    particula.Posicao.X + particula.Velocidade.X * 0.3 + ruidoX,
                    particula.Posicao.Y + particula.Velocidade.Y * 0.3 + ruidoY
                );
            }
            else
            {
                particula.Posicao = new Point(
                    particula.Posicao.X + particula.Velocidade.X + forcaX,
                    particula.Posicao.Y + particula.Velocidade.Y
                );
            }

            if (particula.Posicao.X < 0) particula.Posicao = new Point(largura, particula.Posicao.Y);
            if (particula.Posicao.X > largura) particula.Posicao = new Point(0, particula.Posicao.Y);
            if (particula.Posicao.Y < 0) particula.Posicao = new Point(particula.Posicao.X, altura);
            if (particula.Posicao.Y > altura) particula.Posicao = new Point(particula.Posicao.X, 0);

            Canvas.SetLeft(particula.Elemento, particula.Posicao.X - particula.Raio);
            Canvas.SetTop(particula.Elemento, particula.Posicao.Y - particula.Raio);
        }
    }
}
