using System.IO;

namespace DotNetNuke.Services.Devices.Core.Resource
{
    internal class FlatWURFLResource : WURFLResource
    {
        public FlatWURFLResource(string path) : base(path)
        {
        }

        internal override Stream GetStream()
        {
            return GetFileStream();
        }
    }
}