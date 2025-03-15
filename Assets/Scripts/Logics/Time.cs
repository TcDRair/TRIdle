using System;
using System.Collections.Generic;

using UTime = UnityEngine.Time;

namespace TRIdle.Logics
{
  public static class Time
  {
    private static readonly Dictionary<object, float> m_Cache = new();
    public const float MaxTickDuration = 1f / 120f;
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