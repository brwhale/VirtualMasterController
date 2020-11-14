// Garrett Skelton 2020

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VirtualMasterController {
    class Playlist {
        private List<string> AllowedFormats = new List<string> { "avi", "mkv", "mp4", "m4v", "ogm", "divx", "m2ts" };
        private List<ShowListing> Shows = new List<ShowListing>();

        private bool IsFileAllowed(string path) {
            return AllowedFormats.Contains(path.Split(".").Last().ToLower());
        }
        public class countTotal {
            public int index = 0;
            public int total;
            public double completion { get { return (double)index / (double)total; } }

            public countTotal(int total_) {
                total = total_;
            }
        }

        public void MoveToBefore(string a, string b) {
            var indexA = Shows.FindIndex(s => s.Title == a);
            var temp = Shows[indexA];
            Shows.RemoveAt(indexA);
            var indexB = Shows.FindIndex(s => s.Title == b);
            if (indexB >= indexA) {
                ++indexB;
            }
            Shows.Insert(indexB, temp);
        }

        public void MoveToEnd(string a) {
            var indexA = Shows.FindIndex(s => s.Title == a);
            var temp = Shows[indexA];
            Shows.RemoveAt(indexA); 
            Shows.Add(temp);
        }

        public void Remove(List<int> indexes) {
            indexes.Sort();
            indexes.Reverse();
            foreach(var index in indexes) {
                Shows.RemoveAt(index);
            }
        }

        public void Merge(List<int> indexes) {
            indexes.Sort();

            var show = new ShowListing();
            foreach (var index in indexes) {
                show.Title += " + " + Shows[index].Title;
                show.episodePaths.AddRange(Shows[index].episodePaths);
            }
            show.Title = show.Title.Substring(3);
            indexes.Reverse();            

            foreach (var index in indexes) {
                Shows.RemoveAt(index);
            }

            Shows.Insert(indexes.Last(), show);
        }

        public ShowListing AddShow(string filename) {
            var name = ShowListing.getTitle(filename);
            var matches = Shows.Where(s => s.Title == name).ToList();
            if (matches.Count > 0) {
                AddItemOrDirtectory(matches.First(), filename);
                return matches.First();
            }

            var show = new ShowListing {
                Title = name,
                showIndex = Shows.Count
            };            

            AddItemOrDirtectory(show, filename);

            if (show.Count > 0) {
                Shows.Add(show);
            }

            return show;
        }

        // sync the listbox that displays the shows
        public void UpdateUI(System.Windows.Controls.ListBox listBox) {
            listBox.Items.Clear();

            var index = 0;
            foreach (var show in Shows) {
                show.showIndex = index;
                ++index;
                listBox.Items.Add(show);
            }
        }

        // generate a playlist into a list of the file paths
        public List<string> GetPlaylist(int skipN = 0) {
            List<string> playlist = new List<string>();
            if (Shows.Count > 0) {
                List<countTotal> showIndex = new List<countTotal>();
                foreach (var show in Shows) {
                    showIndex.Add(new countTotal(show.Count));
                }
                double avgCompletion = 0.0;
                double epsilon = 0.00000000001;
                while (avgCompletion != 1.0) {
                    for (int i = 0; i < Shows.Count; i++) {
                        if (showIndex[i].index < showIndex[i].total && showIndex[i].completion <= avgCompletion + epsilon) {
                            if (skipN == 0) {
                                playlist.Add(Shows[i].episodePaths[showIndex[i].index]);
                            } else {
                                skipN--;
                            }
                            showIndex[i].index++;
                        }
                    }

                    double complete = 0;
                    foreach (var show in showIndex) {
                        complete += show.completion;
                    }
                    avgCompletion = complete / showIndex.Count;
                }
            }
            return playlist;
        }

        // Get text to display in the playlist box
        public string GetPlaylistString(string delim, int skipN = 0) {
            string playlist = "";
            var eps = GetPlaylist(skipN);
            foreach (var ep in eps) {
                playlist += ep + delim;
            }
            return playlist;
        }

        // Handle paths drag-dropped into the program
        // if it's a file, we add it, if it's a folder we add it's files recursively
        public void AddItemOrDirtectory(ShowListing show, string fileName) {
            if (Directory.Exists(fileName)) {
                foreach (var file in Directory.GetFiles(fileName)) {
                    if (IsFileAllowed(file)) {
                        show.episodePaths.Add(file);
                    }
                }

                foreach (var folder in Directory.GetDirectories(fileName)) {
                    AddItemOrDirtectory(show, folder);
                }
            } else {
                if (IsFileAllowed(fileName)) {
                    show.episodePaths.Add(fileName);
                }
            }
        }
    }
}
