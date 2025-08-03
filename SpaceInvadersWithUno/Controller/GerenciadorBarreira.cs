namespace SpaceInvadersWithUno;
    using Models;
    using Microsoft.UI.Xaml.Media.Imaging;
    using Windows.Foundation;
    using Windows.Storage.Streams;
    using System.IO;
    using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;

public class GerenciadorBarreiras
{
    private readonly PaginaJogo _paginaJogo;
    public List<Barreira> Barreiras { get; private set; }

    public GerenciadorBarreiras(PaginaJogo paginaJogo)
    {
        _paginaJogo = paginaJogo;
        Barreiras = new List<Barreira>();
    }

    public void InicializarBarreiras()
    {
        LimparBarreiras();

        double[] posicoesX = { 50, 120, 190, 260 };
        double posicaoY = 500;

        foreach (var posX in posicoesX)
        {
            var barreira = new Barreira
            {
                PosicaoX = posX,
                PosicaoY = posicaoY
            };

            var uiBarreira = CriarUIBarreira(barreira);
            _paginaJogo.AdicionarAoCanvas(uiBarreira, barreira.PosicaoX, barreira.PosicaoY);
            barreira.ElementoUI = uiBarreira;
            Barreiras.Add(barreira);
        }
    }

    public void ReceberDano(Barreira barreira, Point pontoImpactoLocal)
    {
        if (barreira.Bitmap == null)
            return;

        int x = (int)pontoImpactoLocal.X;
        int y = (int)pontoImpactoLocal.Y;

        int tamanhoDano = 10;
        int raio = tamanhoDano / 2;

        int larguraBitmap = barreira.Bitmap.PixelWidth;
        int alturaBitmap = barreira.Bitmap.PixelHeight;
        int stride = larguraBitmap * 4;
        var pixelsParaApagar = new List<(int dx, int dy)>();

        for (int dy = -raio; dy <= raio; dy++)
        {
            for (int dx = -raio; dx <= raio; dx++)
            {
                if (dx * dx + dy * dy <= raio * raio)
                {
                    pixelsParaApagar.Add((dx, dy));
                }
            }
        }

        pixelsParaApagar = pixelsParaApagar
            .OrderBy(p => p.dx * p.dx + p.dy * p.dy)
            .ToList();

        using (var buffer = barreira.Bitmap.PixelBuffer.AsStream())
        {
            foreach (var (dx, dy) in pixelsParaApagar)
            {
                int pixelX = x + dx;
                int pixelY = y + dy;

                if (pixelX < 0 || pixelX >= larguraBitmap || pixelY < 0 || pixelY >= alturaBitmap)
                    continue;

                int offset = pixelY * stride + pixelX * 4;
                buffer.Seek(offset, SeekOrigin.Begin);

                buffer.WriteByte(0);
                buffer.WriteByte(0);
                buffer.WriteByte(0);
                buffer.WriteByte(0);
            }
        }

        barreira.Bitmap.Invalidate();

        if (IsBitmapVazio(barreira.Bitmap))
        {
            RemoverBarreira(barreira);
        }
    }


    private bool IsBitmapVazio(WriteableBitmap bitmap)
    {
        if (bitmap == null)
            throw new ArgumentNullException(nameof(bitmap));

        using (var stream = bitmap.PixelBuffer.AsStream())
        {
            byte[] pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
            int totalLido = stream.Read(pixels, 0, pixels.Length);

            if (totalLido != pixels.Length)
                throw new IOException("A leitura do bitmap foi incompleta.");

            for (int i = 3; i < pixels.Length; i += 4)
            {
                if (pixels[i] != 0)
                    return false; 
            }
        }

        return true;
    }

    private void RemoverBarreira(Barreira barreira)
    {
        if (barreira.ElementoUI != null)
        {
            _paginaJogo.RemoverDoCanvas(barreira.ElementoUI);
        }
        Barreiras.Remove(barreira);
    }

    private Image CriarUIBarreira(Barreira barreira)
    {
        int width = barreira.Largura;
        int height = barreira.Altura;

        var bitmap = new WriteableBitmap(width, height);
        using (var buffer = bitmap.PixelBuffer.AsStream())
        {
            byte[] pixels = new byte[width * height * 4];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool pintarPixel = false;
                    if (y < height / 5)
                        pintarPixel = true;

                    if ((x < width / 5 || x > width - width / 5) && y < height * 0.8)
                        pintarPixel = true;

                    if (pintarPixel)
                    {
                        int index = (y * width + x) * 4;
                        pixels[index + 0] = 30;   
                        pixels[index + 1] = 255; 
                        pixels[index + 2] = 30;   
                        pixels[index + 3] = 255; 
                    }
                }
            }

            buffer.Write(pixels, 0, pixels.Length);
        }

        barreira.Bitmap = bitmap;

        return new Image
        {
            Source = bitmap,
            Width = width,
            Height = height,
        };
    }

    private void LimparBarreiras()
    {
        foreach (var barreira in Barreiras.ToList())
        {
            RemoverBarreira(barreira);
        }
    }
}