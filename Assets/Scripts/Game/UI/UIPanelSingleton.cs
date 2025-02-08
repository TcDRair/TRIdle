using UnityEngine;

namespace TRIdle.Game.UI
{
  using Logics.Extensions;

  public abstract class UIPanelSingleton<T> : MonoBehaviour where T : UIPanelSingleton<T>
  {
    public static T Panel { get; private set; }
    public static bool IsPanelActive => Panel != null;

    public RectTransform RectTransform => transform as RectTransform;

    protected virtual void Awake() {
      if (Panel != null) {
        this.LogAssertion($"This panel is already activated. Check the scene hierarchy if there are multiple panels.");
        Destroy(this);
      }
      else Panel = this as T;
    }

    protected virtual void OnDestroy() {
      if (Panel == this as T) Panel = null;
    }
  }
}