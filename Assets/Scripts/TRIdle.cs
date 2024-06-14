using UnityEngine;

namespace TRIdle
{
  public class Const
  {
    public const string PLACEHOLDER = "PlaceHolder";
  }


  public static class Ext
  {
    public static void DestroyAllChildren(this GameObject obj) => DestroyAllChildren(obj.transform);
    public static void DestroyAllChildren(this Transform tr) {
      for (int i = tr.childCount - 1; i >= 0; i--) {
        Object.Destroy(tr.GetChild(i).gameObject);
      }
    }
  }
}