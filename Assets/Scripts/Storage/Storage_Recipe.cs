

using System.Collections.Generic;

namespace TRIdle.Game
{
  public partial class Storage {
    public readonly Dictionary<string, bool> RecipeUnlocked = new();
  }

  //TODO Recipe class
  public struct Recipe {
    public string Name;
    public string Tooltip;
    public Requirement[] Requirements;

    //TODO 공용 등급, 소속 기술 등...

    public struct Requirement {
      public (string tag, int level)[] Tags;
    }
  }
}