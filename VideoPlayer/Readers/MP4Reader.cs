
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using VideoPlayer.Formats.MP4;

namespace VideoPlayer.Readers
{
  public static class MP4Reader
  {

    public static MP4File Parse(string path)
    {
      MP4File file = new MP4File();
      byte[] bytes = File.ReadAllBytes(path);
      ParseFromRoot(file, bytes);
    }

    public static void ParseFromRoot(MP4File file, ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      (uint size, MP4_BoxType type) Header = ParseBoxHeader(ref r);
      ReadOnlySpan<byte> boxData;
      while (Header.type != 0)
      {
        int noHeaderLen = (int)Header.size - 8;
        boxData = buffer.Slice(r.Pos, noHeaderLen);
        switch (Header.type)
        {

          case MP4_BoxType.ftyp:
            file.FileType = ParseFileBox(boxData);
            break;
          case MP4_BoxType.mdat:
            file.MediaData.Add(ParseMediaData(boxData));
            break;
          case MP4_BoxType.moov:
            file.Movie = ParseMovieBox(boxData);
            break;
          case MP4_BoxType.pdin:
            file.PDownloadInfo = ParsePDownloadInfo(boxData);
            break;
          default:
            throw new InvalidDataException($"Unknown box type: {Header.type.ToString()}");
        }
        // this just is so we have bounded box data and size is size of the total box including the header
        r.Skip(noHeaderLen);
        Header = ParseBoxHeader(ref r);
      }
    }

    public static MP4_MovieBox ParseMovieBox(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_MovieBox box = new MP4_MovieBox();
      (uint size, MP4_BoxType type) Header = ParseBoxHeader(ref r);
      ReadOnlySpan<byte> boxData;
      while (Header.type != 0)
      {
        int noHeaderLen = (int)Header.size - 8;
        boxData = buffer.Slice(r.Pos, noHeaderLen);
        switch (Header.type)
        {

          case MP4_BoxType.mvhd:
            box.Header = ParseMovieHeader(boxData);
            break;
          case MP4_BoxType.trak:
            file.MediaData.Add(ParseMediaData(boxData));
            break;
          case MP4_BoxType.udta:
            file.Movie = ParseMovieBox(boxData);
            break;
          default:
            throw new InvalidDataException($"Unknown box type: {Header.type.ToString()}");
        }
        // this just is so we have bounded box data and size is size of the total box including the header
        r.Skip(noHeaderLen);
        Header = ParseBoxHeader(ref r);
      }
      return box;
    }

    public static MP4_MovieHeaderBox ParseMovieHeader(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      (byte v, uint flags) header = ParseVersionAndFlags(ref r);
      MP4_MovieHeaderBox box = new MP4_MovieHeaderBox(header.v);
      if (header.v == 0)
      {
        box.Data32.CreationTime = r.ReadUInt32BE();
        box.Data32.ModificationTime = r.ReadUInt32BE();
        box.Data32.Timescale = r.ReadUInt32BE();
        box.Data32.Duration = r.ReadUInt32BE();
      }
      else
      {
        box.Data64.CreationTime = r.ReadUInt64BE();
        box.Data64.ModificationTime = r.ReadUInt64BE();
        box.Data64.Timescale = r.ReadUInt32BE();
        box.Data64.Duration = r.ReadUInt64BE();
      }

      box.Rate = Parse1616Int(ref r);
      box.Volume = Parse88Int(ref r);
      r.Skip(10);
      box.Matrix = ParseMatrix(ref r);
      if (box.Matrix[0, 2] != 0 || box.Matrix[1, 2] != 0 || box.Matrix[2, 2] != 1)
        throw new InvalidDataException("Invalid matrix uvw values!");
      r.Skip(6 * 4);
      box.NextTrackID = r.ReadUInt32BE();
      return box;
    }

    public static MP4_MediaDataBox ParseMediaData(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_MediaDataBox box = new MP4_MediaDataBox();
      box.Data = r.ReadNext(r.Len);
      return box;
    }

    public static MP4_FileTypeBox ParseFileBox(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_FileTypeBox box = new MP4_FileTypeBox();
      box.MajorBrand = (MP4_FtypMajorBrand)r.ReadUInt32BE();
      if (!Enum.IsDefined(box.MajorBrand))
        throw new InvalidDataException("Invalid MajorBrand Version!");

      box.MinorVersion = r.ReadUInt32BE();
      box.CompatibleBrands = GetNumList<MP4_FtypMajorBrand>(ref r, (r.Len - r.Pos) / 4);
      return box;
    }

    public static (byte version, uint flags) ParseVersionAndFlags(ref BinaryReader r)
    {
      byte v = r.ReadNextByte();
      uint flags = r.ReadNextByte();
      flags <<= 16;
      flags |= r.ReadNextByte();
      flags <<= 8;
      flags |= r.ReadNextByte();
      return (v, flags);
    }

    public static (uint size, MP4_BoxType type) ParseBoxHeader(ref BinaryReader r)
    {
      if (r.Pos >= r.Len)
        return (0, 0);
      uint size = r.ReadUInt32BE();
      // just checking for some weird states where im someshow offset after reading size;
      Debug.Assert(r.Pos < r.Len);
      MP4_BoxType bType = (MP4_BoxType)r.ReadUInt32BE();
      if (!Enum.IsDefined(bType))
        throw new NotImplementedException($"{(int)bType} boxType not found!");
      return (size, bType);
    }

    public static T[] GetNumList<T>(ref BinaryReader r, int count) where T : struct, Enum
    {
     
      T[] res = new T[count];
      for (int i = 0; i < count; i++)
      {
        T val = GetEnum<T>(ref r);
        if (!Enum.IsDefined<T>(val))
          throw new InvalidDataException("Invalid Enum!"); // this is jsut for duration of the development
        res[i] = val;
      }

      return res;
    }

    public static T GetEnum<T>(ref BinaryReader r) where T : struct, Enum
    {
      // this needs to be done because Enum.TryParse accepts only <char>
      Span<char> spanOfChars = stackalloc char[4];
      for (int i = 0; i < spanOfChars.Length; i++)
        spanOfChars[i] = (char)r._buffer[r.Pos + i]; 

      if (!Enum.TryParse(spanOfChars, out T result))
        return default;

      return result;
    }

    public static double Parse1616Int(ref BinaryReader r)
      => ISOParser.ParseFixed1616(r.ReadInt32BE());

    public static double Parse88Int(ref BinaryReader r)
      => ISOParser.ParseFixed88(r.ReadInt16BE());

    public static double[,] ParseMatrix(ref BinaryReader r)
    {
      double[,] matrix = new double[3, 3];
      matrix[0, 0] = Parse1616Int(ref r);
      matrix[0, 1] = Parse1616Int(ref r);
      matrix[0, 2] = Parse1616Int(ref r);
      matrix[1, 0] = Parse1616Int(ref r);
      matrix[1, 1] = Parse1616Int(ref r);
      matrix[1, 2] = Parse1616Int(ref r);
      matrix[2, 0] = Parse1616Int(ref r);
      matrix[2, 1] = Parse1616Int(ref r);
      matrix[2, 2] = Parse1616Int(ref r);
      return matrix;
    }
  }

  
}
