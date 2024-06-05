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

            public MainWindow()
            {
                InitializeComponent();
                wavePlayer = new WaveOutEvent();
                musicFiles = new List<string>();
                currentTrackIndex = -1;
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
            }

            private void PauseButton_Click(object sender, RoutedEventArgs e)
            {
                wavePlayer.Pause();
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
                }
            }

            protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
            {
                wavePlayer.Dispose();
                audioFileReader?.Dispose();
                base.OnClosing(e);
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }

