
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
      int i = (int)Random.Shared.NextInt64(Int32.MinValue, Int32.MaxValue);
      Span<byte> b = new byte[4];
      BinaryPrimitives.WriteInt32BigEndian(b, i);
      double myRes = ISOParser.ParseFixed1616(b);
      double actualRes = ISOParser.readFixedPoint1616Test(b);
      double intRes = ISOParser.ParseFixed1616(i);
      Assert.IsTrue(myRes == actualRes);
      Assert.IsTrue(intRes == actualRes);
    }

    [TestMethod]
    public void Test0230()
    {
      int i = (int)Random.Shared.NextInt64(Int32.MinValue, Int32.MaxValue);
      Span<byte> b = new byte[4];
      BinaryPrimitives.WriteInt32BigEndian(b, i);
      double myRes = ISOParser.ParseFixed0230(b);
      double actualRes = ISOParser.readFixedPoint0230Test(b);
      double intRes = ISOParser.ParseFixed0230(i);
      Assert.IsTrue(myRes == actualRes);
      Assert.IsTrue(intRes == actualRes);
    }

    [TestMethod]
    public void Test88()
    {
      short i = (short)Random.Shared.NextInt64(Int16.MinValue, Int16.MaxValue);
      Span<byte> b = new byte[2];
      BinaryPrimitives.WriteInt16BigEndian(b, i);
      double myRes = ISOParser.ParseFixed88(b);
      double actualRes = ISOParser.readFixedPoint88Test(b);
      double intRes = ISOParser.ParseFixed88(i);
      Assert.IsTrue(myRes == actualRes);
      Assert.IsTrue(intRes == actualRes);
    }
  }
}
