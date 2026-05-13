using VideoPlayer;

namespace VideoFormatTests
{
  [TestClass]
  public sealed class MP4Tests
  {
    [TestMethod]
    public void HD_LessThan10MB()
    {
      Stream stream = File.OpenRead(Files.HD_LessThan10MB_MP4);
    }
  }
}
