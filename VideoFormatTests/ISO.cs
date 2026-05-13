
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
      double myRes = ISOParser.ParseFixed1616(b);
      double actualRes = ISOParser.readFixedPoint1616(b);
      double intRes = ISOParser.ParseFixed1616(i);
      Assert.IsTrue(myRes == actualRes);
      Assert.IsTrue(intRes == actualRes);
    }

    [TestMethod]
    public void Test0230()
    {
      int i = -1567776005;
      Span<byte> b = new byte[4];
      BinaryPrimitives.WriteInt32BigEndian(b, i);
      double myRes = ISOParser.ParseFixed0230(b);
      double actualRes = ISOParser.readFixedPoint0230(b);
      double intRes = ISOParser.ParseFixed0230(i);
    }
  }
}
