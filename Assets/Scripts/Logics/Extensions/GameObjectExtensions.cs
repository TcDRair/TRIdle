using UnityEngine;

namespace TRIdle.Logics.Extensions
{
  public static class GameObjectExtensions
  {
    /// <summary>
    /// Try to get the first component of the given type from the object to its root parent.
    /// </summary>
    /// <typeparam name="T"><see cref="Component"/> type to get</typeparam>
    /// <param name="component">The first found component</param>
    /// <returns><see cref="true"/> if found, otherwise <see cref="false"/></returns>
    public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component) where T : Component
      => gameObject.transform.TryGetComponentInParent(out component);
    
    /// <summary>Variation of <see cref="TryGetComponentInParent{T}"/></summary>
    public static bool TryGetComponentInParent<T>(this Transform transform, out T component) where T : Component {
      if (transform == null) { component = null; return false; }
      if (transform.TryGetComponent(out component)) return true;
      if (transform.parent != null && transform.parent.gameObject.TryGetComponent(out component)) return true;
      return false;
    }

    /// <summary>
    /// Get the bounding box of a game object and its children.<br/>
    /// This method uses <see cref="Renderer.bounds"/> to calculate the bounding box.
    /// </summary>
    /// <returns>The bounding box encompassing the object and its children. default if no renderer found.</returns>
    public static Bounds GetBounds(this GameObject gameObject, bool includeInactive) {
      Bounds bounds = default;
      foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>(includeInactive))
        bounds.Encapsulate(renderer.bounds);
      return bounds;
    }
  }
}