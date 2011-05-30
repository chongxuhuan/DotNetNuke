using MbUnit.Framework;
using Gallio.Framework;

namespace DotNetNuke.MSBuild.Tasks.Tests
{
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Linq;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    class CodePlexFixture
    {

        [Test]
        public void CodePlex_Finds_Table_In_HTML()
        {
            var codePlex = new CodePlexDelete();
            codePlex.FileName = "compare.html";
            codePlex.FileRootPath = @"C:\CodePlex\";
            codePlex.SvnLocation = @"C:\Program Files\csvn\bin";
            Assert.IsTrue(codePlex.Execute());
        }
    }
}
