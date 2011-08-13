using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace DotNetNuke.Services.ClientCapability.Resource
{
    internal class ZipWURFLResource : WURFLResource
    {
        public ZipWURFLResource(string path) : base(path)
        {
            if (!IsZipFile(path))
            {
                throw new ArgumentException("the file you passed is not a valid zip file.");
            }
        }

        internal override Stream GetStream()
        {
            Stream stream = GetFileStream();
            ZipInputStream zipInputStream = new ZipInputStream(stream);

            if (!ZipFileHasAnyEntry(zipInputStream))
            {
                throw new Exception("The Zipped File you passed has no files inside");
            }

            return GetZippedStream(zipInputStream);
        }

        private bool ZipFileHasAnyEntry(ZipInputStream zipInputStream)
        {
            return zipInputStream.GetNextEntry() != null;
        }

        private Stream GetZippedStream(ZipInputStream zipInputStream)
        {
            byte[] buffer = new byte[zipInputStream.Length];
            zipInputStream.Read(buffer, 0, (int) zipInputStream.Length);
            return new MemoryStream(buffer);
        }
    }
}