
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
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
        if (noHeaderLen == -8)
          noHeaderLen = r.Len - r.Pos;
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
        if (noHeaderLen == -8)
          noHeaderLen = r.Len - r.Pos;
        boxData = buffer.Slice(r.Pos, noHeaderLen);
        switch (Header.type)
        {

          case MP4_BoxType.mvhd:
            box.Header = ParseMovieHeader(boxData);
            break;
          case MP4_BoxType.trak:
            box.Tracks.Add(ParseTrack(boxData));
            break;
          case MP4_BoxType.udta:
            box.UserData = ParseUserData(boxData);
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

    public static MP4_TrackBox ParseTrack(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_TrackBox box = new MP4_TrackBox();
      (uint size, MP4_BoxType type) Header = ParseBoxHeader(ref r);
      ReadOnlySpan<byte> boxData;
      while (Header.type != 0)
      {
        int noHeaderLen = (int)Header.size - 8;
        if (noHeaderLen == -8)
          noHeaderLen = r.Len - r.Pos;
        boxData = buffer.Slice(r.Pos, noHeaderLen);
        switch (Header.type)
        {
          case MP4_BoxType.tkhd:
            box.Header = ParseTrackHeader(boxData);
            break;
          case MP4_BoxType.edts:
            box.Edit = ParseEdit(boxData);
            break;
          case MP4_BoxType.mdia:
            box.TrackMedia = ParseTrackMedia(boxData);
            break;
          case MP4_BoxType.udta:
            box.UserData = ParseUserData(boxData);
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

    public static MP4_EditBox ParseEdit(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_EditBox box = new MP4_EditBox();
      (uint size, MP4_BoxType type) Header = ParseBoxHeader(ref r);
      ReadOnlySpan<byte> boxData;
      while (Header.type != 0)
      {
        int noHeaderLen = (int)Header.size - 8;
        if (noHeaderLen == -8)
          noHeaderLen = r.Len - r.Pos;
        boxData = buffer.Slice(r.Pos, noHeaderLen);
        switch (Header.type)
        {

          case MP4_BoxType.elst:
            box.EditList = ParseEditList(boxData);
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

    public static MP4_TrackMediaBox ParseTrackMedia(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_TrackMediaBox box = new MP4_TrackMediaBox();
      (uint size, MP4_BoxType type) Header = ParseBoxHeader(ref r);
      ReadOnlySpan<byte> boxData;
      while (Header.type != 0)
      {
        int noHeaderLen = (int)Header.size - 8;
        if (noHeaderLen == -8)
          noHeaderLen = r.Len - r.Pos;
        boxData = buffer.Slice(r.Pos, noHeaderLen);
        switch (Header.type)
        {

          case MP4_BoxType.mdhd:
            box.Header = ParseMediaHeader(boxData);
            break;
          case MP4_BoxType.hdlr:
            box.Handler = ParseHandler(boxData);
            break;
          case MP4_BoxType.minf:
            box.Info = ParseMediaInformation(boxData);
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
    public static MP4_MediaInformationBox ParseMediaInformation(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_MediaInformationBox box = new MP4_MediaInformationBox();
      (uint size, MP4_BoxType type) Header = ParseBoxHeader(ref r);
      ReadOnlySpan<byte> boxData;
      while (Header.type != 0)
      {
        int noHeaderLen = (int)Header.size - 8;
        if (noHeaderLen == -8)
          noHeaderLen = r.Len - r.Pos;
        boxData = buffer.Slice(r.Pos, noHeaderLen);
        switch (Header.type)
        {
          case MP4_BoxType.vmhd:
            box.Header = ParseVideoMediaHeader(boxData);
            box.HeaderType = MP4_MediaInformationHeaderType.vmhd;
            break;
          case MP4_BoxType.smhd:
            box.Header = ParseSoundMediaHeader(boxData);
            box.HeaderType = MP4_MediaInformationHeaderType.smhd;
            break;
          case MP4_BoxType.hmhd:
            box.Header = ParseHintMediaHeader(boxData);
            box.HeaderType = MP4_MediaInformationHeaderType.hmhd;
            break;
          case MP4_BoxType.nmhd:
            box.Header = ParseNullMediaHeader(boxData);
            box.HeaderType = MP4_MediaInformationHeaderType.nmhd;
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

    public static MP4_VideoMediaHeaderBox ParseVideoMediaHeader(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_VideoMediaHeaderBox box = new MP4_VideoMediaHeaderBox();
      box.GraphicsMode = (MP4_VideoMediaHeaderGraphicsMode)r.ReadUInt16BE();
      if (box.GraphicsMode != 0)
        throw new InvalidDataException("Invalid Graphics Mode!");
      box.OpColor[0] = r.ReadUInt16BE(); // r
      box.OpColor[1] = r.ReadUInt16BE(); // g
      box.OpColor[2] = r.ReadUInt16BE(); // b
      return box;
    }

    public static MP4_SoundMediaHeaderBox ParseSoundMediaHeader(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_SoundMediaHeaderBox box = new MP4_SoundMediaHeaderBox();
      box.Balance = Parse88Int(ref r);
      return box;
    }
    public static MP4_HintMediaHeaderBox ParseHintMediaHeader(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_HintMediaHeaderBox box = new MP4_HintMediaHeaderBox();
      box.MaxPDUSize = r.ReadUInt16BE();
      box.AvgPDUSize = r.ReadUInt16BE();
      box.MaxBitrate = r.ReadUInt32BE();
      box.AvgBitrate = r.ReadUInt32BE();
      return box;
    }
    public static MP4_NullMediaHeaderBox ParseNullMediaHeader(ReadOnlySpan<byte> buffer)
    {
      return new MP4_NullMediaHeaderBox();
    }
    public static MP4_HandlerRefBox ParseHandler(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_HandlerRefBox box = new MP4_HandlerRefBox();
      r.Skip(4 + 4);
      box.HandlerType = (MP4_HandlerType)r.ReadUInt32BE();
      if (!Enum.IsDefined(box.HandlerType))
        throw new InvalidDataException("Unknown HandlerType!");

      box.Name = r.ReadNullTerminatedString();
      return box;
    }
    public static MP4_MediaHeaderBox ParseMediaHeader(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      (byte v, uint flags) header = ParseVersionAndFlags(ref r);
      MP4_MediaHeaderBox box = new MP4_MediaHeaderBox(header.v);
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
      // skip language for now
      return box;
    }
    public static MP4_EditListBox ParseEditList(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader();
      (byte v, uint flags) header = ParseVersionAndFlags(ref r);
      MP4_EditListBox box = new MP4_EditListBox(header.v);
      uint entryCount = r.ReadUInt32BE();
      if (header.v == 0)
      {
        MP4_EditListData32[] Data = new MP4_EditListData32[entryCount + 1];
        // retarded arraays can sometime start at index 1
        for (int i = 1; i <= entryCount; i++)
        {
          MP4_EditListData32 d = new MP4_EditListData32();
          d.SegmentDuration = r.ReadUInt32BE();
          d.MediaTime = r.ReadInt32BE();
          d.MediaRateInteger = r.ReadInt16BE();
          d.MediaRateFraction = r.ReadInt16BE();
          Data[i] = d;
        }
        box.Data32 = Data;
      }
      else
      {
        MP4_EditListData64[] Data = new MP4_EditListData64[entryCount + 1];
        // retarded arraays can sometime start at index 1
        for (int i = 1; i <= entryCount; i++)
        {
          MP4_EditListData64 d = new MP4_EditListData64();
          d.SegmentDuration = r.ReadUInt64BE();
          d.MediaTime = r.ReadInt64BE();
          d.MediaRateInteger = r.ReadInt16BE();
          d.MediaRateFraction = r.ReadInt16BE();
          Data[i] = d;
        }
        box.Data64 = Data;
      }

      return box;
    }
    public static MP4_TrackHeaderBox ParseTrackHeader(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      (byte v, uint flags) header = ParseVersionAndFlags(ref r);
      MP4_TrackHeaderBox box = new MP4_TrackHeaderBox(header.v, header.flags);
      if (header.v == 0)
      {
        box.Data32.CreationTime = r.ReadUInt32BE();
        box.Data32.ModificationTime = r.ReadUInt32BE();
        box.Data32.TrackID = r.ReadUInt32BE();
        r.Skip(4);
        box.Data32.Duration = r.ReadUInt32BE();
      }
      else
      {
        box.Data64.CreationTime = r.ReadUInt64BE();
        box.Data64.ModificationTime = r.ReadUInt64BE();
        box.Data64.TrackID = r.ReadUInt32BE();
        r.Skip(4);
        box.Data64.Duration = r.ReadUInt64BE();
      }
      r.Skip(4 * 2);
      box.Layer = r.ReadInt16BE();
      box.AlternateGroup = r.ReadInt16BE();
      box.Volume = Parse88Int(ref r);
      r.Skip(2);
      box.Matrix = ParseMatrix(ref r);
      box.Width = Parse1616Int(ref r);
      box.Height = Parse1616Int(ref r);
      return box;
    }
    public static MP4_UserDataBox ParseUserData(ReadOnlySpan<byte> buffer)
    {
      // Skip for now
      BinaryReader r = new BinaryReader(buffer);
      MP4_UserDataBox box = new MP4_UserDataBox();
      return box;
    }
    public static MP4_ProgressiveDownloadInfoBox ParsePDownloadInfo(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_ProgressiveDownloadInfoBox box = new MP4_ProgressiveDownloadInfoBox();
      r.Skip(4); // skip flags and version
      box.Data = new (uint rate, uint delay)[(r.Len - r.Pos) / 8];
      for (int i = 0; i < box.Data.Length; i++)
      {
        box.Data[i].rate = r.ReadUInt32BE();
        box.Data[i].initial_delay = r.ReadUInt32BE();
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