using MbUnit.Framework;
using Gallio.Framework;

namespace DotNetNuke.MSBuild.Tasks.Tests
{

    [TestFixture]
    class TfsCommentFixture
    {

        [Test]
        public void TfsComment_Test()
        {
            var tfsComment = new TfsComments { };
            tfsComment.Execute();
        }
    }
}
