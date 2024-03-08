using System;
using TRIdle.System;
using UnityEngine;

namespace TRIdle.Game.Item
{
  /// <summary>
  /// 태그가 가지는 기본 정보를 정의합니다.
  /// <see cref="TagScript"/>를 통해 동적으로 작용하는 효과를 정의할 수 있습니다.
  /// </summary>
  [CreateAssetMenu(fileName = "TagInfo", menuName = "TRIdle/Tags/Tag Info", order = 0)]
  public class TagInfo : ScriptableObject
  {
    public string uName;
    public string tooltip;
    public TagType type;
    [Range(1, 9)] public int maxLevel = 1;
  }
}
