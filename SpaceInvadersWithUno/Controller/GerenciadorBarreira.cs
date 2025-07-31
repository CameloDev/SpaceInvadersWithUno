namespace SpaceInvadersWithUno;
    using Models;
    using Microsoft.UI.Xaml.Media.Imaging;
    using Windows.Foundation;
    using Windows.Storage.Streams;
    using System.IO;
    using System.Runtime.InteropServices.WindowsRuntime;
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
        int x = (int)pontoImpactoLocal.X;
        int y = (int)pontoImpactoLocal.Y;

        if (barreira.Bitmap == null)
            return;

        int tamanhoDano = 5;  // Largura/altura do quadrado de pixels que serão apagados
        int raio = tamanhoDano / 2;

        int larguraBitmap = barreira.Bitmap.PixelWidth;
        int alturaBitmap = barreira.Bitmap.PixelHeight;
        int stride = larguraBitmap * 4;

        using (var buffer = barreira.Bitmap.PixelBuffer.AsStream())
        {
            for (int dy = -raio; dy <= raio; dy++)
            {
                int pixelY = y + dy;
                if (pixelY < 0 || pixelY >= alturaBitmap)
                    continue;

                for (int dx = -raio; dx <= raio; dx++)
                {
                    int pixelX = x + dx;
                    if (pixelX < 0 || pixelX >= larguraBitmap)
                        continue;

                    int offset = pixelY * stride + pixelX * 4;
                    buffer.Seek(offset, SeekOrigin.Begin);

                    // Apaga pixel RGBA (preto transparente)
                    buffer.WriteByte(0); // R
                    buffer.WriteByte(0); // G
                    buffer.WriteByte(0); // B
                    buffer.WriteByte(0); // A
                }
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
            byte[] whitePixels = Enumerable.Repeat((byte)255, width * height * 4).ToArray(); // RGBA
            buffer.Write(whitePixels, 0, whitePixels.Length);
        }

        barreira.Bitmap = bitmap;

        return new Image
        {
            Source = bitmap,
            Width = width,
            Height = height
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