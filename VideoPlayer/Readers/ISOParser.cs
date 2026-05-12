using System.Buffers.Binary;

namespace VideoPlayer.Readers
{
  public static class ISOParser
  {
    public static double ParseFixed1616(ReadOnlySpan<byte> bytes)
    {
      short num = BinaryPrimitives.ReadInt16BigEndian(bytes.Slice(0, 2));
      ushort dec = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(2, 2));
      return num + (dec / 65536d);
    }

    public static double ParseFixed0230(ReadOnlySpan<byte> bytes)
    {
      int result = 0;
      result |= (int)(bytes[0] << 24 & 0xFF000000);
      result |= bytes[1] << 16 & 0xFF0000;
      result |= bytes[2] << 8 & 0xFF00;
      result |= bytes[3] & 0xFF;
      return (double)result / (1 << 30);
    }

   
  }
}
