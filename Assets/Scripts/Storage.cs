

namespace TRIdle.Game
{
  public class Storage
  {
    static Storage _R;
    public static Storage R => _R ??= new();

    public struct R_Explore
    {
      public float Proficiency;
    }
    public R_Explore Explore;
  }
}