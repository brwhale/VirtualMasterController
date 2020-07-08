// Garrett Skelton 2020

using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace VirtualMasterController
{
    public partial class MainWindow : Window
    {
        List<string> episodePaths = new List<string>();
        int scrollIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void EpisodesTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void EpisodesTextBox_Drop(object sender, DragEventArgs e)
        {
            foreach (var fileName in (string[])e.Data.GetData(DataFormats.FileDrop, false))
            {
                if (Directory.Exists(fileName))
                {
                    foreach (var file2 in Directory.GetFiles(fileName))
                    {
                        EpisodesTextBox.Text += file2 + "\n";
                        episodePaths.Add(file2);
                    }
                }
                else
                {
                    EpisodesTextBox.Text += fileName + "\n";
                    episodePaths.Add(fileName);
                }
                
            }
        }

        private void EpisodesTextBox_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scrollIndex -= e.Delta/100;
            if (scrollIndex < 0) { scrollIndex = 0; }
            EpisodesTextBox.Text = "";
            for(int i = scrollIndex; i < episodePaths.Count; i++)
            {
                EpisodesTextBox.Text += episodePaths[i] + "\n";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string playlist = "";
            for (int i = 0; i < episodePaths.Count; i++)
            {
                playlist += "\"" + episodePaths[i] + "\" ";
            }
            System.Diagnostics.Process.Start("C:\\Program Files\\VideoLAN\\VLC\\vlc.exe", playlist);
        }
    }
}
