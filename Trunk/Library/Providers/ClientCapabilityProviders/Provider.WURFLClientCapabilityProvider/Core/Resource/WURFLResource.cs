using System;
using System.IO;

namespace DotNetNuke.Services.ClientCapability.Resource
{
    public abstract class WURFLResource
    {
        private readonly string _path;

        public WURFLResource(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new Exception("path must be valid.");
            }

            _path = path;
        }

        public String Path
        {
            get { return _path; }
        }

        protected Stream GetFileStream()
        {
            return new FileStream(Path, FileMode.Open, FileAccess.Read);
        }

        internal abstract Stream GetStream();


        public static bool IsZipFile(string path)
        {
            return path.ToLower().EndsWith(".zip");
        }

        public static bool IsGZipFile(string path)
        {
            return path.ToLower().EndsWith(".gz");
        }
    }
}