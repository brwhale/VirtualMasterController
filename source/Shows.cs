// Garrett Skelton 2020

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VirtualMasterController {
    public class ShowListing {
        public string Title { get; set; }
        public string CountStr { get { return "" + Count + " episodes"; } }
        public int Count { get { return episodePaths.Count; } }
        public List<string> episodePaths = new List<string>();
        public int showIndex = 0;

        public static string getTitle(string filepath) {
            var choices = filepath.Split('\\');
            var index = choices.Length - 1;
            while (Regex.Match(choices[index].ToLower(), "s(\\d|e)(\\d|ason)").Success) {
                --index;

                if (index < 0) { index = 0; break; }
            }
            return choices[index];
        }
    }
}
