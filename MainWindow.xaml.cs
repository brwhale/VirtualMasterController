// Garrett Skelton 2020

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VirtualMasterController {
    public partial class MainWindow : Window {
        Playlist playlist = new Playlist();

        public MainWindow() {
            InitializeComponent();
        }

        private void EpisodesTextBox_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false)) {
                e.Effects = DragDropEffects.All;
            } else {
                e.Effects = DragDropEffects.None;
            }
        }

        int scrollIndex = 0;
        private void refreshTextReadout() {
            EpisodesTextBox.Text = playlist.GetPlaylistString("\n", scrollIndex);
        }

        string draggedName = "";
        private void EpisodesTextBox_Drop(object sender, DragEventArgs e) {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (files != null) {
                foreach (var fileName in files) {
                    playlist.AddShow(fileName);
                }
            } else {
                if (draggedName != "") {
                    playlist.MoveToEnd(draggedName);
                }
            }
            playlist.UpdateUI(ShowListBox);
            refreshTextReadout();
        }

        private void itemDrop(object sender, DragEventArgs e) {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (files != null) {
                foreach (var fileName in files) {
                    playlist.AddShow(fileName);
                    
                }
            } else {
                if (sender is StackPanel) {
                    var sendStack = sender as StackPanel;
                    var nameBlock = sendStack.Children[0] as TextBlock;
                    var name = nameBlock.Text;

                    if (name == draggedName) {
                        draggedName = "";
                        return;
                    } else { 
                        playlist.MoveToBefore(draggedName, name);
                    }
                    draggedName = "";
                }
            }
            playlist.UpdateUI(ShowListBox);
            refreshTextReadout();
        }

        private void EpisodesTextBox_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e) {
            scrollIndex -= e.Delta / 100;
            if (scrollIndex < 0) { scrollIndex = 0; }
            refreshTextReadout();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            // use cmd line to add videos to current playlist after starting instead of this
            System.Diagnostics.Process.Start("C:\\Program Files\\VideoLAN\\VLC\\vlc.exe", "--one-instance");
            var plist = playlist.GetPlaylist();
            foreach (var ep in plist) {
                System.Threading.Thread.Sleep(60);
                System.Diagnostics.Process.Start("C:\\Program Files\\VideoLAN\\VLC\\vlc.exe", "--one-instance --playlist-enqueue \"" + ep + "\"");
            }
        }

        private void ShowListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {

        }

        private void dragStart(object sender, MouseEventArgs e) {
            if (sender is StackPanel && e.LeftButton == MouseButtonState.Pressed) {
                var draggedItem = sender as StackPanel;
                var nameBlock = draggedItem.Children[0] as TextBlock;
                draggedName = nameBlock.Text;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
            }
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e) {
            var selected = ShowListBox.SelectedItems;
            List<int> indexes = new List<int>();
            foreach (ShowListing item in selected) {
                indexes.Add(item.showIndex);
            }
            playlist.Remove(indexes);

            playlist.UpdateUI(ShowListBox);
            refreshTextReadout();
        }
    }
}
