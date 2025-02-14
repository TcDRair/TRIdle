using UnityEngine;

namespace TRIdle.Game.UI
{
  using Logics.Extensions;

  public abstract class UIPanelSingleton<T> : MonoBehaviour where T : UIPanelSingleton<T>
  {
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
      if (Instance != null)
        this.LogAssertion("Instance already exists. Check the scene if there are multiple instances.");
      Instance = this as T;
    }

    public RectTransform RectTransform => transform as RectTransform;
  }
}