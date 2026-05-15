using System.Buffers.Binary;

namespace VideoPlayer.Readers
{
  /// <summary>
  /// Big endian, but can easily be extended to be little endian
  /// Generally we wont care if we read outside of bounds since,
  /// we will use these across each box and outside called should always
  /// check bounds in some way or expect an exception
  /// I think that this will be better to debug wether its my code or bad data
  /// </summary>
  /// This is also limited to int which means 2GB boxes
  /// I think that we will want to stream main data otherwise and use this reader 
  /// only for "normal" non large boxes
  public ref struct BinaryReader
  {
    // this doesnt really have a getter becuase we dont want its ref struct and we cant return it by reference and i dont want to copy on get
    public ReadOnlySpan<byte> _buffer;
    private int _readPos;
    public int Pos => _readPos;
    public int Len => _buffer.Length;
    public BinaryReader(ReadOnlySpan<byte> buffer)
    {
      _buffer = buffer;
    }

    public short ReadInt16BE()
    {
      short res = BinaryPrimitives.ReadInt16BigEndian(_buffer.Slice(_readPos, 2));
      _readPos += 2;
      return res;
    }

    public uint ReadUInt32BE()
    {
      uint res = BinaryPrimitives.ReadUInt32BigEndian(_buffer.Slice(_readPos, 4));
      _readPos += 4;
      return res;
    }
    public int ReadInt32BE()
    {
      int res = BinaryPrimitives.ReadInt32BigEndian(_buffer.Slice(_readPos, 4));
      _readPos += 4;
      return res;
    }
    public ulong ReadUInt64BE()
    {
      uint res = BinaryPrimitives.ReadUInt32BigEndian(_buffer.Slice(_readPos, 8));
      _readPos += 8;
      return res;
    }

    public void Skip(int size) => _readPos += size;
    public byte[] ReadNext(int size)
    {
      byte[] res = _buffer.Slice(_readPos, size).ToArray();
      _readPos += size;
      return res;
    }
    public byte ReadNextByte() => _buffer[_readPos++];
  }
}
