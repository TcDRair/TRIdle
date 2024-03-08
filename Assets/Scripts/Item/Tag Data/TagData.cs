using UnityEngine;

namespace TRIdle.Game.Item
{
  public class TagData : MonoBehaviour
  {
    public TagInfo info;
    public TagScript script; // The script can be attached to this object, only if it exists.

    public override bool Equals(object other)
      => other is TagData data && data.info.uName == info.uName;
    public override int GetHashCode() => info.uName.GetHashCode();
  }
  /// <summary>
  /// <see cref="TagInfo"/>와 동적 멤버가 포함된 태그입니다.
  /// 실제 사용되는 태그는 이 구조체를 사용합니다.
  /// </summary>
  public struct Tag
  {
    public TagData data;
    public int level;
  }
  public enum TagType { General, Structure, Item, ItemPositive, ItemNegative, Unique, Type }
}
