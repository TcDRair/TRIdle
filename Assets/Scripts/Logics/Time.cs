using System;
using System.Collections.Generic;

using UTime = UnityEngine.Time;

namespace TRIdle.Logics
{
  public static class Time
  {
    private static readonly Dictionary<object, float> m_Cache = new();
    public const float MaxTickDuration = 1f / 120f;
    /// <summary>
    /// Checks if the given object has been ticked recently. Can be used to check if specific calculations spent more than a tick.
    /// </summary>
    /// <returns><see cref="true"/> if the elapsed time from the last tick is more than <see cref="MaxTickDuration"/>.</returns>
    /// <exception cref="NullReferenceException">thrown if the given object is null.</exception>
    public static bool TickElapsed(object obj) {
      if (obj is null) throw new NullReferenceException();
      if (m_Cache.TryGetValue(obj, out var time)) {
        if (UTime.realtimeSinceStartup - time < MaxTickDuration)
          return true;
        m_Cache[obj] = UTime.realtimeSinceStartup;
        return false;
      }
      else {
        m_Cache.Add(obj, UTime.realtimeSinceStartup);
        return true;
      }
    }
  }
}