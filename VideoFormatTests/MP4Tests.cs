using VideoPlayer;
using VideoPlayer.Formats.MP4;
using VideoPlayer.Readers;

namespace VideoFormatTests
{
  [TestClass]
  public sealed class MP4Tests
  {
    [TestMethod]
    public void HD_LessThan10MB()
    {
      MP4File file = MP4Reader.Parse(Files.HD_LessThan10MB_MP4);
    }
  }
}
