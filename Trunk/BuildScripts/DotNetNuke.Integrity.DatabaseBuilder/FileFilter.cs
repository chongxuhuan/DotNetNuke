using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace DotNetNuke.Integrity.DatabaseBuilder {
    public abstract class FileFilter {
        public abstract bool Matches(FileInfo file);
    }

    public class RegexFileFilter : FileFilter {
        private Regex _matchString;

        public Regex MatchString {
            get { return _matchString; }
        }

        public RegexFileFilter(Regex matchString) {
            _matchString = matchString;
        }

        public override bool Matches(FileInfo file) {
            return MatchString.IsMatch(file.FullName);
        }
    }
}
