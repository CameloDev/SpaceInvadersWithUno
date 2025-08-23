using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpaceInvadersWithUno.Models;

namespace SpaceInvadersWithUno;

public sealed partial class ScoresPage : Page
{

    public ScoresPage()
    {
        this.InitializeComponent();
        CarregarScores();
    }

    private void CarregarScores()
    {
        string caminho = "scores.txt";

        if (!File.Exists(caminho))
            return;

        var linhas = File.ReadAllLines(caminho);

        var lista = linhas
            .Select(linha =>
            {
                var partes = linha.Split('-');
                if (partes.Length < 3) return null;

                string data = partes[0].Trim();
                string nome = partes[1].Trim();
                string pontosStr = partes[2].Replace("pontos", "").Trim();

                int.TryParse(pontosStr, out int pontos);

                return new ScoreItem
                {
                    Data = data,
                    Nome = nome,
                    Pontuacao = pontos
                };
            })
            .Where(x => x != null)
            .OrderByDescending(x => x?.Pontuacao)
            .ToList();

        ScoresList.ItemsSource = lista;
    }

    private void Voltar_Click(object sender, RoutedEventArgs e)
    {
        Frame.GoBack();
    }
}
