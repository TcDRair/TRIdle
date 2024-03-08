namespace TRIdle
{
  public static class PathHolder
  {
    public static class Resources
    {
      public const string TagSO = "SO/Tags";
      public const string Attributes = "SO/Attributes";
      public const string TagJson = "Json/Tags";
    }

    public static string ResourcesPathToAssetPath(this string path) => $"Assets/Resources/{path}";
  }
}
