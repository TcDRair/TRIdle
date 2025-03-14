using UnityEngine;

namespace TRIdle.Logics
{
  public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
  {
    private static readonly object lockObject = new();
    private static T instance = null;
    private static bool destroyed = false;

    public static T Instance {
      get {
        lock (lockObject) {
          if (destroyed) return null;
          if (instance == null) {
            instance = new GameObject().AddComponent<T>();
            DontDestroyOnLoad(instance.gameObject);
          }
          return instance;
        }
      }
    }

    private void OnDestroy() {
      instance = null;
      destroyed = true;
    }
  }
}