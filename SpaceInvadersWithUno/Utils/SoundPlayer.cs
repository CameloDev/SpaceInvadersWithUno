using NAudio.Wave;
namespace SpaceInvadersWithUno;
    public class SoundPlayer : IDisposable
    {
        private IWavePlayer? outputDevice;
        private AudioFileReader? audioFile;

        public void Play(string arquivo)
        {
            try
            {
                outputDevice?.Dispose();
                audioFile?.Dispose();

                outputDevice = new WaveOutEvent();
                audioFile = new AudioFileReader(arquivo);
                outputDevice.Init(audioFile);
                outputDevice.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Dispose()
        {
            outputDevice?.Dispose();
            audioFile?.Dispose();
        }
    }
