namespace VideoPlayer.Formats.MP4
{
  public enum MP4_BoxType : uint
  {
    NULL = 0,
    uuid = 1970628964,
    ftyp = 1718909296,
    mdat = 1835295092,
    free = 1718773093,
    skip = 1936419184,
    pdin = 1885628782,
    moov = 1836019574,
    mvhd = 1836476516,
    trak = 1953653099,
    tkhd = 1953196132,
    edts = 1701082227,
    elst = 1701606260,
    mdia = 1835297121,
    mdhd = 1835296868,
    hdlr = 1751411826,
    minf = 1835626086,
    vmhd = 1986881636,
    smhd = 1936549988,
    hmhd = 1752000612,
    nmhd = 1852663908,
    dinf = 1684631142,
    url = 1970433056,
    urn = 1970433568,
    dref = 1685218662,
    stbl = 1937007212,
    stsd = 1937011556,
    btrt = 1651798644,
    metx = 1835365496,
    mett = 1835365492,
    uri = 1970432288,
    uriI = 1970432329,
    urim = 1970432365,
    pasp = 1885434736,
    clap = 1668047216,
    colr = 1668246642,
    avc1 = 1635148593,
    avcC = 1635148611,
    stss = 1937011571,
    ctts = 1668576371,
    stsc = 1937011555,
    stsz = 1937011578,
    stz2 = 1937013298,
    sgpd = 1936158820,
    sbgp = 1935828848,
    udta = 1969517665,
    name = 1851878757,

  }

  public enum MP4_FtypMajorBrand : uint
  {
    _3g2a = 862401121,
    _3g2b = 862401122,
    _3g2c = 862401123,
    _3ge6 = 862414134,
    _3ge7 = 862414135,
    _3gg6 = 862414646,
    _3gp1 = 862416945,
    _3gp2 = 862416946,
    _3gp3 = 862416947,
    _3gp4 = 862416948,
    _3gp5 = 862416949,
    _3gp6 = 862416950,
    _3gs7 = 862417719,
    avc1 = 1635148593,
    CAEP = 1128351056,
    caqv = 1667330422,
    CDes = 1128555891,
    da0a = 1684090977,
    da0b = 1684090978,
    da1a = 1684091233,
    da1b = 1684091234,
    da2a = 1684091489,
    da2b = 1684091490,
    da3a = 1684091745,
    da3b = 1684091746,
    dmb1 = 1684890161,
    dmpf = 1684893798,
    drc1 = 1685218097,
    dv1a = 1685467489,
    dv1b = 1685467490,
    dv2a = 1685467745,
    dv2b = 1685467746,
    dv3a = 1685468001,
    dv3b = 1685468002,
    dvr1 = 1685484081,
    dvt1 = 1685484593,
    F4V = 1177835040,
    F4P = 1177833504,
    F4A = 1177829664,
    F4B = 1177829920,
    isc2 = 1769169714,
    iso2 = 1769172786,
    isom = 1769172845,
    JP2 = 1246769696,
    JP20 = 1246769712,
    jpm = 1785752864,
    jpx = 1785755680,
    KDDI = 1262765129,
    M4A = 1295270176,
    M4B = 1295270432,
    M4P = 1295274016,
    M4V = 1295275552,
    M4VH = 1295275592,
    M4VP = 1295275600,
    mj2s = 1835676275,
    mjp2 = 1835692082,
    mmp4 = 1835888692,
    mp21 = 1836069425,
    mp41 = 1836069937,
    mp42 = 1836069938,
    mp71 = 1836070705,
    MPPI = 1297109065,
    mqt = 1836151840,
    MSNV = 1297305174,
    NDAS = 1313096019,
    NDSC = 1313100611,
    NDSH = 1313100616,
    NDSM = 1313100621,
    NDSP = 1313100624,
    NDSS = 1313100627,
    NDXC = 1313101891,
    NDXH = 1313101896,
    NDXM = 1313101901,
    NDXP = 1313101904,
    NDXS = 1313101907,
    odcf = 1868850022,
    opf2 = 1869637170,
    opx2 = 1869641778,
    pana = 1885433441,
    qt = 1903435808,
    ROSS = 1380930387,
    sdv = 1935963680,
    ssc1 = 1936941873,
    ssc2 = 1936941874,

  }

  public enum MP4_HandlerType
  {
    NULL = 1853189228, // Just used to hold resources
    vide = 1986618469, // Video Track
    soun = 1936684398, // Audio Track
    hint = 1751740020, // Hint Track
    meta = 1835365473, // Time Metadata Track
    auxv = 1635088502, // Auxiliary Video Track
  }

  // subset of Mp4_BoxType that are allowed to be a Header type
  public enum MP4_MediaInformationHeaderType
  {
    vmhd = 1986881636,
    smhd = 1936549988,
    hmhd = 1752000612,
    nmhd = 1852663908,
  }

  // seems redundant bue w/e
  public enum MP4_VideoMediaHeaderGraphicsMode
  {
    copy = 0
  }
  // subset of Mp4_BoxType that are allowed to be a DataReference Type
  public enum MP4_DataEntryType
  {
    url = 1970433056,
    urn = 1970433568,
  }

  public enum MP4_ColorType
  {
    nclx = 1852009592,
    rICC = 1917403971,
    prof = 1886547814,
  }

  public enum MP4_CodingType
  {
    avc1 = 1635148593, // H.264/AVC
  }

  public enum MP4_SampleSizeBoxType
  {
    stsz = 1937011578,
    stz2 = 1937013298,
  }

  public enum MP4_TrackStatus
  {
    Enabled,
    InMovie,
    InPreview
  }

}
