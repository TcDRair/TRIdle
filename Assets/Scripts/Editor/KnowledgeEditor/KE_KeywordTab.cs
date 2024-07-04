using UnityEngine;

namespace TRIdle.Editor {
  public class KE_KeywordTab : ITabLayout {
    public class StateProperties
    {
      public TextAsset DisplayAsset { get; set; }
    }
    public StateProperties State { get; set; } = new();


    public void Initialize() {
      State.DisplayAsset = new() { name = "keywords.json" };
    }

    public int DoLayout() {
      return 0;
    }
  }
}