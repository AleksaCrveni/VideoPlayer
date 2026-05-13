using System.ComponentModel.Design;
using VideoPlayer.Readers;

namespace VideoPlayer.Formats.MP4
{
  public class MP4File
  {
    public MP4_FileTypeBox FileType;
  }

  // we dont need to store size since we will read all data into boxes during parsing
  public class MP4_Box
  {
    public MP4_Box(MP4_BoxType boxType, sbyte[]? extended_type = null)
    {
      if (boxType == MP4_BoxType.uuid)
        IsUUID = true;
      UserType = extended_type;
    }

    public MP4_BoxType Type;
    public bool IsUUID = false;
    public sbyte[]? UserType; // only used when IsUUID is true, meaning Type == 'uuid'
  }

  /// <summary>
  /// Flags is actually 3 bytes (24 bits) shifted to the right but we use 4 bytes for convinience
  /// </summary>
  public class MP4_FullBox : MP4_Box
  {
    public byte version;
    public uint flags;
    public MP4_FullBox(MP4_BoxType boxType, byte v, uint f) : base(boxType)
    {
      version = v;
      flags = f;
    }

  }
  public class MP4_FileTypeBox : MP4_Box
  {
    public MP4_FileTypeBox() : base(MP4_BoxType.ftyp) { }
    public MP4_FtypMajorBrand MajorBrand;
    public uint MinorVersion;
    // this could probably be array since can calculate how many brands there may be based on size and curr pos
    public List<MP4_FtypMajorBrand> CompatibleBrands;
  }

  public class MP4_MediaDataBox : MP4_Box
  {
    byte[] Data;
    public MP4_MediaDataBox() : base(MP4_BoxType.uuid) { }


  }

  public class MP4_FreeSkipBox : MP4_Box
  {
    public MP4_FreeSkipBox() : base(MP4_BoxType.free) { }
  }

  public class MP4_ProgressiveDownloadInfoBox : MP4_FullBox
  {
    public List<(uint rate, uint initial_delay)> Data;
    public MP4_ProgressiveDownloadInfoBox() : base(MP4_BoxType.pdin, 0, 0) { }
  }

  public class MP4_MovieBox : MP4_Box
  {
    public MP4_MovieHeaderBox Header;
    public List<MP4_TrackBox> Tracks;
    public MP4_MovieBox(MP4_BoxType boxType, sbyte[]? extended_type = null) : base(boxType, extended_type)
    {

    }
  }
  public class MP4_MovieHeaderBox : MP4_FullBox
  {
    public MP4_MovieHeaderVData64 Data64;
    public MP4_MovieHeaderVData32 Data32;
    public double Rate = ISOParser.ParseFixed1616(0x00010000);
    public double Volume = 1.0;
    // skip 10 bytes of reserved data

    /*
     * (p q 1) * | a b u | = (m n z)
     *           | c d v | 
     *           | x y w |
     * m = ap + cq + x; n = bp + dq + y; z = up + vq + w;
     * p' = m/z; q' = n/z
     * 
     * All the values in a matrix are stored as 16.16 fixed-point values,
     * except for u, v and w, which are stored as 2.30 fixed-point values.
    */

    public double[,] Matrix = { { ISOParser.ParseFixed1616(0x00010000), 0, 0 }, { 0, ISOParser.ParseFixed1616(0x00010000), 0 }, { 0, 0, ISOParser.ParseFixed0230(0x40000000) } };
    // skip 6 * 4 bytes for pre_defined
    public uint NextTrackID;
    public MP4_MovieHeaderBox(byte v) : base(MP4_BoxType.mvhd, v, 0)
    {
      if (v == 0)
        Data32 = new MP4_MovieHeaderVData32();
      else if (v == 1)
        Data64 = new MP4_MovieHeaderVData64();
      else
        throw new InvalidDataException("Invalid version value!");
    }


  }

  public class MP4_MovieHeaderVData64
  {
    public ulong CreationTime;
    public ulong ModificationTime;
    public uint Timescale;
    public ulong Duration;
  }

  public class MP4_MovieHeaderVData32
  {
    public uint CreationTime;
    public uint ModificationTime;
    public uint Timescale;
    public uint Duration;
  }

  public class MP4_TrackBox : MP4_Box
  {
    public MP4_TrackHeaderBox Header;
    public MP4_EditBox EditBox;
    public MP4_MediaBox MediaBox;
    public MP4_TrackBox() : base(MP4_BoxType.trak)
    {
    }
  }

  public class MP4_TrackHeaderBox : MP4_FullBox
  {
    public MP4_TrackHeaderVData64 Data64;
    public MP4_TrackHeaderVData32 Data32;
    public short Layer;
    public short AlternateGroup;
    public double Volume = 1.0d;
    public double[,] Matrix = { { ISOParser.ParseFixed1616(0x00010000), 0, 0 }, { 0, ISOParser.ParseFixed1616(0x00010000), 0 }, { 0, 0, ISOParser.ParseFixed0230(0x40000000) } };
    public double Width;
    public double Height;
    public MP4_TrackHeaderBox(byte v, uint f) : base(MP4_BoxType.tkhd, v, f)
    {
      if (v == 1)
      {
        Data64 = new MP4_TrackHeaderVData64();
      }
      else if (v == 0)
      {
        Data32 = new MP4_TrackHeaderVData32();
      }
      else
      {
        throw new InvalidDataException("Version supported!");
      }
    }
  }

  public class MP4_TrackHeaderVData64
  {
    public ulong CreationTime;
    public ulong ModificationTime;
    public uint TrackID;
    public readonly uint Reserved = 0; // redundant?
    public ulong Duration;
  }

  public class MP4_TrackHeaderVData32
  {
    public uint CreationTime;
    public uint ModificationTime;
    public uint TrackID;
    public readonly uint Reserved = 0; // redundant?
    public uint Duration;
  }

  public class MP4_EditBox : MP4_Box
  {
    public MP4_EditListBox EditList;
    public MP4_EditBox() : base(MP4_BoxType.edts) { }
  }

  public class MP4_EditListBox : MP4_FullBox
  {
    public uint EntryCount;
    public MP4_EditListData64 Data64;
    public MP4_EditListData32 Data32;
    public short MediaRateInteger;
    public short MediaRateFraction = 0;
    public MP4_EditListBox(byte v) : base(MP4_BoxType.elst, v, 0)
    {

    }
  }

  public class MP4_EditListData64
  {
    public ulong SegmentDuration;
    public long MediaTime;
  }

  public class MP4_EditListData32
  {
    public uint SegmentDuration;
    public int MediaTime;
  }

  public class MP4_MediaBox : MP4_Box
  {
    public MP4_MediaHeaderBox Header;
    public MP4_HandlerRefBox Handler;
    public MP4_MediaInformationBox Info;
    public MP4_MediaBox() : base(MP4_BoxType.mdia) { }
  }

  public class MP4_MediaHeaderBox : MP4_FullBox
  {
    public MP4_MediaHeaderData64 Data64;
    public MP4_MediaHeaderData32 Data32;
    public ushort Language; // this is actually 3 5 bit ints and 1 bit of padding on the start
    public MP4_MediaHeaderBox(byte v) : base(MP4_BoxType.mdhd, v, 0)
    {
      if (v == 1)
        Data64 = new MP4_MediaHeaderData64();
      else if (v == 0)
        Data32 = new MP4_MediaHeaderData32();
      else
        throw new InvalidDataException("Invalid version!");
    }
  }

  public class MP4_MediaHeaderData64
  {
    public ulong CreationTime;
    public ulong ModificationTime;
    public uint Timescale;
    public ulong Duration;
  }

  public class MP4_MediaHeaderData32
  {
    public uint CreationTime;
    public uint ModificationTime;
    public uint Timescale;
    public uint Duration;
  }

  public class MP4_HandlerRefBox : MP4_FullBox
  {
    public MP4_HanlderType HandlerType;
    public string Name;
    public MP4_HandlerRefBox() : base(MP4_BoxType.hdlr, 0, 0) { }
  }

  public class MP4_MediaInformationBox : MP4_Box
  {
    public IMediaInformationHeader Header;
    public MP4_MediaInformationHeaderType HeaderType; // for casting IMediaInformationHeader
    public MP4_DataInformationBox DataInfo;
    public MP4_SampleTableBox SampleTable;
    public MP4_MediaInformationBox() : base(MP4_BoxType.minf) { }
  }
  public interface IMediaInformationHeader { }
  public class VideoMediaHeaderBox : MP4_FullBox, IMediaInformationHeader
  {
    public MP4_VideoMediaHeaderGraphicsMode GraphicsMode = MP4_VideoMediaHeaderGraphicsMode.copy;
    public ushort[] OpColor = [0, 0, 0];
    public VideoMediaHeaderBox() : base(MP4_BoxType.vmhd, 0, 1) { }
  }

  public class SoundMediaHeaderBox : MP4_FullBox, IMediaInformationHeader
  {
    // Fixed 88. 0 is centre, -1.0 is full left and 1.0 is full right
    public double Balanced = 0;
    public SoundMediaHeaderBox() : base(MP4_BoxType.smhd, 0, 0) { }
  }

  public class HintMediaHeaderBox : MP4_FullBox, IMediaInformationHeader
  {
    // PDU -> Protocol Data Unit
    public ushort MaxPDUSize;
    public ushort AvgPDUSize;
    public uint MaxBitrate;
    public uint AvgBitrate;
    public HintMediaHeaderBox() : base(MP4_BoxType.hmhd, 0, 0) { }
  }

  public class NullMediaHeaderBox : MP4_FullBox, IMediaInformationHeader
  {
    public NullMediaHeaderBox() : base(MP4_BoxType.nmhd, 0, 0) { }
  }

  // I am not sure if this HAS to be dref containing urn/url or it can just be single url/urn
  public class MP4_DataInformationBox : MP4_Box
  {
    public MP4_DataReferenceBox DataReference;
    public MP4_DataInformationBox() : base(MP4_BoxType.dinf) { }
  }
  public interface IDataEntry
  {
    public MP4_BoxType GetType();
  }

  public class MP4_DataReferenceBox : MP4_FullBox
  {
    IDataEntry[] Entries;
    public MP4_DataReferenceBox() : base(MP4_BoxType.dref, 0, 0) { }
  }

  public class MP4_DataEntryUrlBox : MP4_FullBox, IDataEntry
  {
    public string Location;
    public MP4_DataEntryUrlBox(uint f) : base(MP4_BoxType.url, 0, f) { }
    public new MP4_BoxType GetType() => base.Type;
  }
  public class MP4_DataEntryUrnBox : MP4_FullBox, IDataEntry
  {
    public string Name;
    public string Location;
    public MP4_DataEntryUrnBox(uint f) : base(MP4_BoxType.url, 0, f) { }
    public new MP4_BoxType GetType() => base.Type;
  }

  public class MP4_SampleTableBox : MP4_Box
  {
    public MP4_SampleTableBox() : base(MP4_BoxType.stbl)
    {
    }
  }

}
