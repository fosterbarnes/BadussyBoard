using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using NAudio.Wave;
using System.Text;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace BadussyBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<SoundItem> SoundItems { get; set; }
        private List<(WaveOutEvent player, AudioFileReader reader)> currentlyPlaying = new List<(WaveOutEvent, AudioFileReader)>(); //list of currently playing sounds

        public MainWindow()
        {
            InitializeComponent();
            SoundItems = new ObservableCollection<SoundItem>();
            SoundDataGrid.ItemsSource = SoundItems;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Click: +");

            PickerWindow picker = new PickerWindow(this);
            picker.Owner = this;
            bool? result = picker.ShowDialog(); // blocks until window is closed
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Click: -");

            // Get the currently selected item in the DataGrid
            var selectedItem = SoundDataGrid.SelectedItem as SoundItem;
            if (selectedItem != null)
                SoundItems.Remove(selectedItem);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Click: Edit");

            var selectedItem = SoundDataGrid.SelectedItem as SoundItem;
            if (selectedItem != null)
            {
                var picker = new PickerWindow(this, selectedItem);
                picker.ShowDialog();
            } else
            {
                MessageBox.Show("Please select a sound item to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Click: Play");

            // simple way to play a sound while testing. we will need something more complex to be able to stop or layer sounds
            var selectedItem = SoundDataGrid.SelectedItem as SoundItem;
            if (selectedItem != null)
            {
                try
                {
                    var audioFile = new AudioFileReader(selectedItem.FilePath);
                    var output = new WaveOutEvent();

                    output.Init(audioFile); output.Play();
                    currentlyPlaying.Add((output, audioFile)); //add to list of playing sounds

                    //dispose objects after playback
                    output.PlaybackStopped += (s, args) =>
                    {
                        output.Dispose(); audioFile.Dispose();
                        currentlyPlaying.RemoveAll(x => x.player == output);
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error playing sound:\n{ex.Message}", "Playback Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Click: Stop");

            foreach (var (player, reader) in currentlyPlaying)
            {
                player.Stop(); player.Dispose(); reader.Dispose();
            }
            currentlyPlaying.Clear();
        }

        private void Levels_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Click: Levels"); 
            //TODO Add logic
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Click: Settings"); 
            //TODO Add logic
        }
    }
}