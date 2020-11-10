﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace VirtualMasterController {
    class Playlist {
        private List<string> AllowedFormats = new List<string> { "avi", "mkv", "mp4", "m4v", "ogm", "divx" };
        private List<ShowListing> Shows = new List<ShowListing>();

        public ShowListing GetShow(string name) {
            return Shows.Where(s => s.Title == name).FirstOrDefault();
        }

        public void MoveToBefore(string a, string b) {
            //if (a == b) return;
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

        public ShowListing AddShow(string filename) {
            var name = TVShow.getTitle(filename);
            var matches = Shows.Where(s => s.Title == name).ToList();
            if (matches.Count > 0) {
                AddItemOrDirtectory(matches.First(), filename);
                return matches.First();
            }

            var show = new ShowListing {
                Title = name,
                showIndex = Shows.Count
            };

            Shows.Add(show);

            AddItemOrDirtectory(show, filename);

            return show;
        }

        public void UpdateUI(System.Windows.Controls.ListBox listBox) {
            listBox.Items.Clear();

            var index = 0;
            foreach (var show in Shows) {
                show.showIndex = index;
                ++index;
                listBox.Items.Add(show);
            }
        }

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

        public string GetPlaylistString(string delim, int skipN = 0) {
            string playlist = "";
            var eps = GetPlaylist(skipN);
            foreach (var ep in eps) {
                playlist += ep + delim;
            }
            return playlist;
        }

        public void AddItemOrDirtectory(ShowListing show, string fileName) {
            if (Directory.Exists(fileName)) {
                foreach (var file2 in Directory.GetFiles(fileName)) {
                    if (AllowedFormats.Contains(file2.Split(".").Last().ToLower())) {
                        show.episodePaths.Add(file2);
                    }
                }

                foreach (var file2 in Directory.GetDirectories(fileName)) {
                    AddItemOrDirtectory(show, file2);
                }
            } else {
                if (AllowedFormats.Contains(fileName.Split(".").Last().ToLower())) {
                    show.episodePaths.Add(fileName);
                }
            }
        }
    }
}