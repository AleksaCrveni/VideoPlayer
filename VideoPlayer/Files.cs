namespace VideoPlayer
{
  public static class Files
  {
    public static string RootFolder { get; set; }
    public static string HD_LessThan10MB_MP4 { get; set; }
    static Files()
    {
      RootFolder = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Files");
      HD_LessThan10MB_MP4 = Path.Combine(RootFolder, "HD_LessThan10MB.mp4");
    }
  }
}
