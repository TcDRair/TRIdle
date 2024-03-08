using System.Collections;

namespace TRIdle
{
  public abstract class ObjectLoader
  {
    public abstract IEnumerator Load();

    private static float _elapsed = 0;
    private const float TICK = .025f;
    /// <summary>Check if the maximum time has elapsed</summary>
    protected static bool Elapsed {
      get {
        if (UnityEngine.Time.timeSinceLevelLoad - _elapsed > TICK) {
          _elapsed = UnityEngine.Time.timeSinceLevelLoad;
          return true;
        }
        return false;
      }
    }
  }
}
