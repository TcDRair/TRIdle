using UnityEngine;

namespace TRIdle.Logics.Extensions
{
  public static class GameObjectExtensions
  {
    public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component) where T : Component
      => gameObject.transform.TryGetComponentInParent<T>(out component);
    public static bool TryGetComponentInParent<T>(this Transform transform, out T component) where T : Component {
      if (transform == null) { component = null; return false; }
      if (transform.TryGetComponent(out component)) return true;
      if (transform.parent != null && transform.parent.gameObject.TryGetComponent(out component)) return true;
      return false;
    }
  }
}