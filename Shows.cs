using System.Collections.Generic;

namespace VirtualMasterController {
    public class ShowListing {
        public string Title { get; set; }
        public string CountStr { get { return "" + Count + " episodes"; } }
        public int Count { get { return episodePaths.Count; } }
        public List<string> episodePaths = new List<string>();
        public int showIndex = 0;
    }

    public class countTotal {
        public int index = 0;
        public int total;
        public double completion { get { return (double)index / (double)total; } }

        public countTotal(int total_) {
            total = total_;
        }
    }

    public class TVShow {
        public static string getTitle(string filepath) {
            var choices = filepath.Split('\\');
            var index = choices.Length - 1;
            while (choices[index].ToLower().Contains("season")) {
                --index;

                if (index < 0) { index = 0; break; }
            }
            return choices[index];
        }
    }
}
