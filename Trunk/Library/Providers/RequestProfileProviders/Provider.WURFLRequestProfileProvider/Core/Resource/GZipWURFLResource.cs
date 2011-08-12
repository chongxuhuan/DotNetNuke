using System;
using System.IO;
using System.IO.Compression;

namespace DotNetNuke.Services.Devices.Core.Resource
{
    internal class GZipWURFLResource : WURFLResource
    {
        public GZipWURFLResource(string path) : base(path)
        {
            if (!IsGZipFile(path))
            {
                throw new ArgumentException("The path you passed is not a valid Gzip File");
            }
        }


        internal override Stream GetStream()
        {
            Stream stream = GetFileStream();
            return new GZipStream(stream, CompressionMode.Decompress);
        }
    }
}