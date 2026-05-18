
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
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
            box.Info = ParseMediaInformation(boxData, box.Handler.HandlerType);
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
    public static MP4_MediaInformationBox ParseMediaInformation(ReadOnlySpan<byte> buffer, MP4_HandlerType handlerType)
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
          case MP4_BoxType.stbl:
            box.SampleTable = ParseSampleTable(boxData, handlerType);
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

    public static MP4_SampleTableBox ParseSampleTable(ReadOnlySpan<byte> buffer, MP4_HandlerType handlerType)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_SampleTableBox box = new MP4_SampleTableBox();
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
          case MP4_BoxType.stsd:
            box.Description = ParseSampleDescription(boxData, handlerType);
            break;
          case MP4_BoxType.stts:
            box.STTS = ParseTimeToSample(boxData);
            break;
          case MP4_BoxType.stss:
            box.SyncSample = ParseSyncSample(boxData);
            break;
          case MP4_BoxType.ctts:
            box.CTTS = ParseCompositionToSample(boxData);
            break;
          case MP4_BoxType.stsc:
            box.STSC = ParseSampleToChunk(boxData);
            break;
          case MP4_BoxType.stsz:
            box.SampleSize = ParseSampleSize(boxData);
            break;
          case MP4_BoxType.stz2:
            box.SampleSize = ParseCompactSampleSize(boxData);
            break;
          case MP4_BoxType.sbgp:
            box.SampleToGroups.Add(ParseSampleToGroup(boxData));
            break;
          case MP4_BoxType.sgpd:
            box.GroupDescriptions.Add(ParseSampleGroupDescription(boxData, handlerType));
            break;
          default:
            throw new InvalidDataException($"Unknown box type: {Header.type.ToString()}");
        }
        // this just is so we have bounded box data and size is size of the total box including the header
        r.Skip(noHeaderLen);
        Header = ParseBoxHeader(ref r);
      }

      if (box.SampleToGroups.Count != box.GroupDescriptions.Count)
        throw new InvalidDataException("Invalid Sample group and group description count!");
      return box;
    }

    public static MP4_SampleGroupDescriptionBox ParseSampleGroupDescription(ReadOnlySpan<byte> buffer, MP4_HandlerType handlerType)
    {
      BinaryReader r = new BinaryReader(buffer);
      (byte v, uint flags) header = ParseVersionAndFlags(ref r);
      MP4_SampleGroupDescriptionBox box = new MP4_SampleGroupDescriptionBox(header.v, handlerType);
      box.GroupingType = (MP4_GroupingType)r.ReadUInt32BE();
      if (!Enum.IsDefined(box.GroupingType))
        throw new InvalidDataException($"Unkown grouping type: {box.GroupingType.ToString()}")
      if (header.v == 1)
        box.DefaultLength = r.ReadUInt32BE();
      box.EntryCount = r.ReadUInt32BE();
      box.Entries = new MP4_SampleGroupDescriptionEntry[box.EntryCount + 1];
      for (int i = 1; i <= box.EntryCount; i++)
      {
        uint len = box.DefaultLength;
        if (header.v == 1 && box.DefaultLength == 0)
          len = r.ReadUInt32BE();

        switch (handlerType)
        {
          case MP4_HandlerType.vide:
            box.Entries[i] = ParseAbstractVisualSampleEntry(buffer, box.GroupingType);
            break;
          case MP4_HandlerType.soun:
            box.Entries[i] = ParseAbstractAudioSampleEntry(buffer, box.GroupingType);
            break;
          case MP4_HandlerType.hint:
            throw new NotSupportedException("Handler type for sample group description parsing not supported yet!");
            break;
          default:
            throw new InvalidDataException($"Unsupported handler for sample group description: {handlerType.ToString()}");
            break;
        }
      }
      return box;
    }
    public static MP4_SampleGroupDescriptionEntry ParseAbstractVisualSampleEntry(ReadOnlySpan<byte> buffer, MP4_GroupingType groupingType)
    {
      switch (groupingType)
      {
        case MP4_GroupingType.roll:
          return ParseVisualRollSampleGroupEntry(buffer, groupingType);
        default:
          throw new NotSupportedException("Grouping type not supported yet!");
      }
    }
    public static MP4_SampleGroupDescriptionEntry ParseAbstractAudioSampleEntry(ReadOnlySpan<byte> buffer, MP4_GroupingType groupingType)
    {
      switch (groupingType)
      {
        case MP4_GroupingType.roll:
          return ParseAudioRollSampleGroupEntry(buffer, groupingType);
        default:
          throw new NotSupportedException("Grouping type not supported yet!");
      }
    }
    public static MP4_VisualRollSampleGroupEntryBox ParseVisualRollSampleGroupEntry(ReadOnlySpan<byte> buffer, MP4_GroupingType groupingType)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_VisualRollSampleGroupEntryBox box = new MP4_VisualRollSampleGroupEntryBox(groupingType);
      box.RollDistance = r.ReadInt16BE();
      return box;
    }
    public static MP4_AudioRollSampleGroupEntryBox ParseAudioRollSampleGroupEntry(ReadOnlySpan<byte> buffer, MP4_GroupingType groupingType)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_AudioRollSampleGroupEntryBox box = new MP4_AudioRollSampleGroupEntryBox(groupingType);
      box.RollDistance = r.ReadInt16BE();
      return box;
    }
    public static MP4_SampleToGroupBox ParseSampleToGroup(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      (byte v, uint flags) header = ParseVersionAndFlags(ref r);
      MP4_SampleToGroupBox box = new MP4_SampleToGroupBox(header.v);
      box.GroupingType = r.ReadUInt32BE();
      if (header.v == 1)
        box.GroupingTypeParameter = r.ReadUInt32BE();
      box.EntryCount = r.ReadUInt32BE();
      box.Entries = new (uint, uint)[box.EntryCount + 1];
      for (int i = 1; i <= box.EntryCount; i++)
      {
        box.Entries[i].SampleCount = r.ReadUInt32BE();
        box.Entries[i].GroupDescriptionIndex = r.ReadUInt32BE();
      }
      return box;
    }
    public static MP4_SampleSizeBox ParseSampleSize(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_SampleSizeBox box = new MP4_SampleSizeBox();
      r.Skip(4);
      box.SampleSize = r.ReadUInt32BE();
      box.SampleCount = r.ReadUInt32BE();
      if (box.SampleSize == 0)
      {
        box.SampleSizes = new uint[box.SampleCount + 1];
        for (int i = 1; i <= box.SampleCount; i++)
          box.SampleSizes[i] = r.ReadUInt32BE();
      }
      return box;
    }
    public static MP4_CompactSampleSizeBox ParseCompactSampleSize(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_CompactSampleSizeBox box = new MP4_CompactSampleSizeBox();
      r.Skip(4 + 3);
      box.FieldSize = r.ReadByte();
      box.SampleCount = r.ReadUInt32BE();
      box.SampleSizes = new uint[box.SampleCount + 1];
      if (box.FieldSize == 4)
      {
        for (int i = 1; i <= box.SampleCount; i+= 2)
        {
          byte val = r.ReadByte();
          box.SampleSizes[i] = (uint)val >> 4;
          if (i + 1 <= box.SampleCount)
          {
            box.SampleSizes[i + 1] = (uint)val & 15;
          }
        }
      }
      else if (box.FieldSize == 8)
      {
        for (int i = 1; i <= box.SampleCount; i++)
        {
          box.SampleSizes[i] = r.ReadByte();
        }
      }
      else if (box.FieldSize == 16)
      {
        for (int i = 1; i <= box.SampleCount; i++)
        {
          box.SampleSizes[i] = r.ReadUInt16BE();
        }
      }
      else
      {
        throw new InvalidDataException("Invalid field size value!");
      }
      return box;

    }
    public static MP4_SampleToChunkBox ParseSampleToChunk(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_SampleToChunkBox box = new MP4_SampleToChunkBox();
      r.Skip(4);
      box.EntryCount = r.ReadUInt32BE();
      box.Data = new (uint, uint, uint)[box.EntryCount + 1];
      for (int i = 1; i <= box.EntryCount; i++)
      {
        box.Data[i].FirstChunk = r.ReadUInt32BE();
        box.Data[i].SamplesPerChunk = r.ReadUInt32BE();
        box.Data[i].SampleDescriptionIndex = r.ReadUInt32BE();
      }
      return box;
    }
    public static MP4_CompositionToSampleBox ParseCompositionToSample(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      (byte v, uint flags) header = ParseVersionAndFlags(ref r);
      MP4_CompositionToSampleBox box = new MP4_CompositionToSampleBox(header.v);
      uint entryCount = r.ReadUInt32BE();
      if (header.v == 0)
      {
        box.UnsignedData = new (uint, uint)[entryCount];
        for (int i = 0; i < entryCount; i++)
        {
          box.UnsignedData[i].SampleCount = r.ReadUInt32BE();
          box.UnsignedData[i].SampleOffset = r.ReadUInt32BE();
        }
      }
      else
      {
        box.UnsignedData = new (uint, uint)[entryCount];
        for (int i = 0; i < entryCount; i++)
        {
          box.SignedData[i].SampleCount = r.ReadUInt32BE();
          box.SignedData[i].SampleOffset = r.ReadInt32BE();
        }
      }
      return box;
    }

    public static MP4_SyncSampleBox ParseSyncSample(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_SyncSampleBox box = new MP4_SyncSampleBox();
      r.Skip(4);
      uint entryCount = r.ReadUInt32BE();
      box.Samples = new uint[entryCount];
      for (int i = 0; i < entryCount; i++)
        box.Samples[i] = r.ReadUInt32BE();
      return box;
    }

    public static MP4_TimeToSampleBox ParseTimeToSample(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_TimeToSampleBox box = new MP4_TimeToSampleBox();
      r.Skip(4);
      uint entryCount = r.ReadUInt32BE();
      box.Data = new (uint SampleCount, uint SampleDelta)[entryCount];
      for (int i = 0; i < entryCount; i++)
      {
        box.Data[i].SampleCount = r.ReadUInt32BE();
        box.Data[i].SampleDelta = r.ReadUInt32BE();
      }
      return box;
    }

    public static MP4_SampleDescriptionBox ParseSampleDescription(ReadOnlySpan<byte> buffer, MP4_HandlerType handlerType)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_SampleDescriptionBox box = new MP4_SampleDescriptionBox(handlerType);
      r.Skip(4);
      uint entryCount = r.ReadUInt32BE();
      MP4_SampleEntry[] sampleEntries = new MP4_SampleEntry[entryCount];
      ReadOnlySpan<byte> boxData;
      for (int i = 0; i <= entryCount; i++)
      {
        uint boxSize = r.ReadUInt32BE();
        int noSizeBoxLen = (int)boxSize - 4;
        if (noSizeBoxLen == -4)
          noSizeBoxLen = r.Len - r.Pos;
        boxData = buffer.Slice(r.Pos, noSizeBoxLen);

        switch (handlerType)
        {
          case MP4_HandlerType.soun:
            throw new NotImplementedException();
            break;
          case MP4_HandlerType.vide:
            sampleEntries[i] = ParseVisualSampleEntry(boxData);
            break;
          case MP4_HandlerType.hint:
            throw new NotImplementedException();
            break;
          case MP4_HandlerType.meta:
            throw new NotImplementedException();
            break;
          // not sure if NULL is required to be instancieeted
          default:
            throw new InvalidDataException("Unknown HandlerType!");
            break;
        }
        r.Skip(noSizeBoxLen);
      }
      return box;
    }
    /// <summary>
    /// Buffer starts after size entry just before the name of the codec
    /// so first 32bit read woul be codec name
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static MP4_SampleEntry ParseVisualSampleEntry(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader();
      MP4_CodingType codecType = (MP4_CodingType)r.ReadUInt32BE();
      if (!Enum.IsDefined(codecType))
        throw new InvalidDataException("Unknown codec!");
      // this cast is guaranteed
      MP4_VisualSampleEntryBox box = new MP4_VisualSampleEntryBox((MP4_BoxType)codecType);
      r.Skip(6); // skip 6 resevred from SampleEntry default
      box.DataReferenceIndex = r.ReadUInt16BE();
      r.Skip(2 + 2 + 12);
      box.Width = r.ReadUInt16BE();
      box.Height = r.ReadUInt16BE();
      box.HorizResolution = Parse1616Int(ref r);
      box.VertResolution = Parse1616Int(ref r);
      r.Skip(4);
      box.FrameCount = r.ReadUInt16BE();
      box.CompressorName = r.ReadAsString(32);
      box.Depth = r.ReadUInt16BE();
      r.Skip(2);
      
      if (r.Pos >= r.Len)
        return box;
      // do while loop for extra boxes and do special cases for clap and pasp
      // if its not check if box name matches extra data code for MP4_coding type and put it in box.SampleExtraData
      ReadOnlySpan<byte> boxData;
      (uint size, MP4_BoxType type) Header = ParseBoxHeader(ref r);
      while (Header.type != 0)
      {
        int noHeaderLen = (int)Header.size - 8;
        if (noHeaderLen == -8)
          noHeaderLen = r.Len - r.Pos;
        boxData = buffer.Slice(r.Pos, noHeaderLen);
        switch (Header.type)
        {
          case MP4_BoxType.clap:
            throw new NotImplementedException();
            break;
          case MP4_BoxType.pasp:
            throw new NotImplementedException();
            break;
          case MP4_BoxType.avcC:
            if (codecType != MP4_CodingType.avc1)
              throw new InvalidDataException($"Uknown extra sample data: {Header.type.ToString()} for avc1");
            box.SampleExtraData = ParseAVCConfigurationBox(buffer);
            break;
          default:
            throw new InvalidDataException($"Uknown extra sample data: {Header.type.ToString()}");
            break;
        }
        // this just is so we have bounded box data and size is size of the total box including the header
        r.Skip(noHeaderLen);
        Header = ParseBoxHeader(ref r);
      }

      return box;
    }
    public static MP4_ICodecCustomBox ParseAVCConfigurationBox(ReadOnlySpan<byte> buffer)
    {
      BinaryReader r = new BinaryReader(buffer);
      MP4_AVCConfigurationBox box = new MP4_AVCConfigurationBox();
      box.Version = r.ReadByte();
      box.AVCProfile = r.ReadByte();
      box.AVCCompatibility = r.ReadByte();
      box.AVCLevel = r.ReadByte();
      /*
       * This field is supposed to tell
       * us how many bytes to use to store the length of each NALU.
       * So, if NALULengthSizeMinusOne is set to 0,
       * then each NALU is preceded with a single byte indicating its length, etc
       * 
       * BUT! In reality size sps and sps len field appears to be 2 bytes even though NALULenghtSize is set to 3 ( means 4 bytes).
       * This might be only MP4 derived thing, meaning its different in some cases than raw h264?
       * There isn't any official specification out there other than some questions on SO, since latest official spec is chained behind
       * 300e paywall :)
       * 
       * So for now always assume that psp and sps NALU sizes are in 2 bytes and after reading all of it see if we are at the end of the box.
       * If we arent throw so I can come up with better solution and maybe do some calculations and read ahead etc
       * 
       * References:
       * https://stackoverflow.com/questions/24884827/possible-locations-for-sequence-picture-parameter-sets-for-h-264-stream
       * https://stackoverflow.com/questions/17541153/how-to-find-sps-and-pps-string-in-h264-codec-from-mp4
      */
      box.NALULengthSize = (byte)(r.ReadByte() & 3 + 1);
      byte SPS_NALU_COUNT = (byte)(r.ReadByte() & 31);
      box.SPSData = new List<byte[]>(SPS_NALU_COUNT);
      for (int i = 0; i < SPS_NALU_COUNT; i++)
      {
        uint size = r.ReadUInt16BE();
        box.SPSData.Add(r.Read((int)size));
      }
      byte PPS_NALU_COUNT = r.ReadByte();
      box.PPSData = new List<byte[]>(PPS_NALU_COUNT);
      for (int i = 0; i < PPS_NALU_COUNT; i++)
      {
        uint size = r.ReadUInt16BE();
        box.PPSData.Add(r.Read((int)size));
      }
      Debug.Assert(r.Pos == r.Len);
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
      box.EntryCount = r.ReadUInt32BE();
      if (header.v == 0)
      {
        MP4_EditListData32[] Data = new MP4_EditListData32[box.EntryCount + 1];
        // retarded arraays can sometime start at index 1
        for (int i = 1; i <= box.EntryCount; i++)
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
        MP4_EditListData64[] Data = new MP4_EditListData64[box.EntryCount + 1];
        // retarded arraays can sometime start at index 1
        for (int i = 1; i <= box.EntryCount; i++)
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
      box.Data = r.Read(r.Len);
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
      byte v = r.ReadByte();
      uint flags = r.ReadByte();
      flags <<= 16;
      flags |= r.ReadByte();
      flags <<= 8;
      flags |= r.ReadByte();
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