// Garrett Skelton 2020

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace VirtualMasterController
{
    public class ShowListing
    {
        public string Title { get; set; }
        public string CountStr { get { return "" + episodePaths.Count + " episodes"; } }
        public int Count { get { return episodePaths.Count; } }
        public List<string> episodePaths = new List<string>();
        public int showIndex = 0;
    }

    public class countTotal
    {
        public int index = 0;
        public int total;
        public double completion { get { return (double)index / (double)total; } }

        public countTotal(int total_)
        {
            total = total_;
        }
    }

    public partial class MainWindow : Window
    {
        public List<ShowListing> Shows = new List<ShowListing>();
        int scrollIndex = 0;

        public List<string> AllowedFormats = new List<string>{ "avi", "mkv", "mp4", "m4v", "ogm", "divx" };

        public MainWindow()
        {
            InitializeComponent();
        }

        private string getPlaylist(string delim, int skipN = 0)
        {            
            string playlist = "";            
            if (Shows.Count > 0)
            {
                List<countTotal> showIndex = new List<countTotal>();
                foreach (var show in Shows)
                {
                    showIndex.Add(new countTotal(show.Count));
                }
                double avgCompletion = 0.0;
                while (avgCompletion != 1.0)
                {
                    for (int i = 0; i < Shows.Count; i++)
                    {
                        if (showIndex[i].index < showIndex[i].total && showIndex[i].completion <= avgCompletion)
                        {
                            if (skipN == 0)
                            {
                                playlist += "\"" + Shows[i].episodePaths[showIndex[i].index] + "\"" + delim;
                            }
                            else
                            {
                                skipN--;
                            }
                            showIndex[i].index++;
                        }
                    }

                    double complete = 0;
                    foreach (var show in showIndex)
                    {
                        complete += show.completion;
                    }
                    avgCompletion = complete / showIndex.Count;
                }
            }
            return playlist;
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

        private void addItemOrDirtectory(ShowListing show, string fileName)
        {
            if (Directory.Exists(fileName))
            {
                foreach (var file2 in Directory.GetFiles(fileName))
                {
                    if (AllowedFormats.Contains(file2.Split(".").Last().ToLower()))
                    {
                        show.episodePaths.Add(file2);
                    }
                }

                foreach (var file2 in Directory.GetDirectories(fileName))
                {
                    addItemOrDirtectory(show, file2);
                }
            }
            else
            {
                if (AllowedFormats.Contains(fileName.Split(".").Last().ToLower()))
                {
                    show.episodePaths.Add(fileName);
                }                
            }
        }

        private void refreshTextReadout()
        {
            EpisodesTextBox.Text = getPlaylist("\n", scrollIndex);
        }

        private void EpisodesTextBox_Drop(object sender, DragEventArgs e)
        {
            foreach (var fileName in (string[])e.Data.GetData(DataFormats.FileDrop, false))
            {
                var show = new ShowListing();
                show.Title = fileName.Split('\\').Last();
                show.showIndex = Shows.Count;

                addItemOrDirtectory(show, fileName);

                if (show.Count > 0)
                {
                    Shows.Add(show);
                    ShowListBox.Items.Add(show);
                }
            }

            refreshTextReadout();
        }

        private void EpisodesTextBox_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scrollIndex -= e.Delta/100;
            if (scrollIndex < 0) { scrollIndex = 0; }
            refreshTextReadout();
  
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // use cmd line to add videos to current playlist after starting instead of this
            System.Diagnostics.Process.Start("C:\\Program Files\\VideoLAN\\VLC\\vlc.exe", getPlaylist(" "));
        }

        private void ShowListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ShowListBox.SelectedItems;
            List<int> indexes = new List<int>();
            foreach (ShowListing item in selected)
            {
                indexes.Add(item.showIndex);
            }
            indexes.Sort();
            for (int i = indexes.Count - 1; i >= 0; i--)
            {
                Shows.RemoveAt(indexes[i]);
                ShowListBox.Items.RemoveAt(indexes[i]);
            }

            refreshTextReadout();
        }

        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ShowListBox.SelectedItems;
            List<int> indexes = new List<int>();
            foreach (ShowListing item in selected)
            {
                indexes.Add(item.showIndex);
            }

            var show = new ShowListing();
            show.Title = "";
           
            indexes.Sort();
            for (int i = indexes.Count - 1; i >= 0; i--)
            {
                foreach (var episode in Shows[indexes[i]].episodePaths)
                {
                    show.episodePaths.Add(episode);
                }

                if (i != indexes.Count - 1)
                {
                    show.Title += " + ";
                }
                show.Title += Shows[indexes[i]].Title;

                Shows.RemoveAt(indexes[i]);
                ShowListBox.Items.RemoveAt(indexes[i]);
            }

            show.showIndex = Shows.Count;

            Shows.Add(show);
            ShowListBox.Items.Add(show);

            refreshTextReadout();
        }
    }
}
