
using System.Buffers.Binary;
using System.Diagnostics;
using VideoPlayer.Readers;

namespace VideoFormatTests
{
  [TestClass]
  public class ISO
  {
    [TestMethod]
    public void Test1616()
    {
      int i = (int)Random.Shared.NextInt64();
      Span<byte> b = new byte[4];
      BinaryPrimitives.WriteInt32BigEndian(b, i);
      double myRes = ISOParser.Parse1616(b);
      double actualRes = ISOParser.Parse16_16(b);
      Assert.IsTrue(myRes == actualRes);
    }

    [TestMethod]
    public void Test0230()
    {
      int i = -1567776005;
      Span<byte> b = new byte[4];
      BinaryPrimitives.WriteInt32BigEndian(b, i);
      double myRes = ISOParser.Parse0230(b);
      double actualRes = ISOParser.Parse02_30(b);
      Assert.IsTrue(myRes == actualRes);
    }
  }
}
