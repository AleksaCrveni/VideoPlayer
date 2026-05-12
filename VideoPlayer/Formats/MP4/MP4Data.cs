

using System.Numerics;

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
    public MP4_MovieBox(MP4_BoxType boxType, sbyte[]? extended_type = null) : base(boxType, extended_type)
    {

    }
  }
  public class MP4_MovieHeaderBox : MP4_FullBox
  {
    public MP4_MovieHeaderVData64 Data64;
    public MP4_MovieHeaderVData32 Data32;
    public double Rate;
    public double Volume;
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

    public double[,] Matrix = { { 0x00010000, 0, 0 }, { 0, 0x00010000, 0 }, { 0, 0, 0x40000000 } };
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
}
