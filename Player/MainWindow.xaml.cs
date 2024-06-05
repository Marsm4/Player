using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private IWavePlayer wavePlayer;
        private AudioFileReader audioFileReader;
        private List<string> musicFiles;
        private int currentTrackIndex;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            wavePlayer = new WaveOutEvent();
            musicFiles = new List<string>();
            currentTrackIndex = -1;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Music Files|*.mp3;*.wav;*.wma"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                musicFiles.AddRange(openFileDialog.FileNames);
                currentTrackIndex = 0;
                LoadCurrentTrack();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            wavePlayer.Play();
            timer.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            wavePlayer.Pause();
            timer.Stop();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (musicFiles.Count > 0)
            {
                currentTrackIndex = (currentTrackIndex + 1) % musicFiles.Count;
                LoadCurrentTrack();
                wavePlayer.Play();
            }
        }

        private void LoadCurrentTrack()
        {
            if (currentTrackIndex >= 0 && currentTrackIndex < musicFiles.Count)
            {
                audioFileReader?.Dispose();
                audioFileReader = new AudioFileReader(musicFiles[currentTrackIndex]);
                wavePlayer.Init(audioFileReader);
                CurrentSongText.Text = $"Playing: {System.IO.Path.GetFileName(musicFiles[currentTrackIndex])}";
                PositionSlider.Maximum = audioFileReader.TotalTime.TotalSeconds;
                TotalTimeText.Text = audioFileReader.TotalTime.ToString(@"mm\:ss");
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (audioFileReader != null)
            {
                PositionSlider.Value = audioFileReader.CurrentTime.TotalSeconds;
                CurrentTimeText.Text = audioFileReader.CurrentTime.ToString(@"mm\:ss");
            }
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (audioFileReader != null && Math.Abs(audioFileReader.CurrentTime.TotalSeconds - e.NewValue) > 1)
            {
                audioFileReader.CurrentTime = TimeSpan.FromSeconds(e.NewValue);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            wavePlayer.Dispose();
            audioFileReader?.Dispose();
            base.OnClosing(e);
        }
    }
}