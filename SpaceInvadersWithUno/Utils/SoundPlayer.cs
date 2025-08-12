using NAudio.Wave;
using System;
using System.IO;

namespace SpaceInvadersWithUno;

    public class SoundPlayer
    {
        private WaveOutEvent? _waveOutLoop;
        private LoopStream? _loopStream;
        public void TocarSom(string caminho)
        {
            try
            {
                var ms = new MemoryStream(File.ReadAllBytes(caminho));
                var reader = new WaveFileReader(ms);
                var waveOut = new WaveOutEvent();

                waveOut.Init(reader);
                waveOut.Play();

                waveOut.PlaybackStopped += (s, e) =>
                {
                    waveOut.Dispose();
                    reader.Dispose();
                    ms.Dispose();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro '{caminho}': {ex.Message}");
            }
        }

        public void TocarSomEmLoop(string caminho)
        {
            try
            {
                var ms = new MemoryStream(File.ReadAllBytes(caminho));
                var reader = new WaveFileReader(ms);

                _loopStream = new LoopStream(reader);

                _waveOutLoop = new WaveOutEvent();
                _waveOutLoop.Init(_loopStream);
                _waveOutLoop.Play();

                _waveOutLoop.PlaybackStopped += (s, e) =>
                {
                    _waveOutLoop.Dispose();
                    _loopStream.Dispose();
                    reader.Dispose();
                    ms.Dispose();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao tocar som em loop '{caminho}': {ex.Message}");
            }
        }

        public void PararSomEmLoop()
        {
            _waveOutLoop?.Stop();
        }
    }
