using System.Buffers.Binary;

namespace VideoPlayer.Readers
{
  public static class ISOParser
  {
    public static double ParseFixed88(short val)
      => val / 256d;
    public static double ParseFixed88(ReadOnlySpan<byte> bytes)
      => BinaryPrimitives.ReadInt16BigEndian(bytes) / 256d;

    public static double ParseFixed1616(ReadOnlySpan<byte> bytes)
     => BinaryPrimitives.ReadInt32BigEndian(bytes) / 65536d;

    public static double ParseFixed1616(int val)
      => val / 65536d;

    public static double ParseFixed0230(ReadOnlySpan<byte> bytes)
     => BinaryPrimitives.ReadInt32BigEndian(bytes) / (double)(1 << 30);

    public static double ParseFixed0230(int val)
     => (double)val / (1 << 30);

    // bellow are functions used only for testing guaranteed to work, so that we can compare to our impl
    public static double readFixedPoint1616Test(ReadOnlySpan<byte> bytes)
    {
      int result = 0;
      result |= (int)(bytes[0] << 24 & 0xFF000000);
      result |= bytes[1] << 16 & 0xFF0000;
      result |= bytes[2] << 8 & 0xFF00;
      result |= bytes[3] & 0xFF;
      return (double)result / 65536;
    }

    public static double readFixedPoint0230Test(ReadOnlySpan<byte> bytes)
    {
      int result = 0;
      result |= (int)(bytes[0] << 24 & 0xFF000000);
      result |= bytes[1] << 16 & 0xFF0000;
      result |= bytes[2] << 8 & 0xFF00;
      result |= bytes[3] & 0xFF;
      return (double)result / (1 << 30);
    }

    public static double readFixedPoint88Test(ReadOnlySpan<byte> bytes)
    {
      short result = 0;
      result |= (short)(bytes[0] << 8 & 0xFF00);
      result |= (short)(bytes[1] & 0xFF);
      return (float)result / 256;
    }

  }
}
