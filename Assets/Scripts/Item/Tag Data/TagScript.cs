using System;
using UnityEngine;
using UnityEngine.Events;

namespace TRIdle.Game.Item
{
  /// <summary>
  /// <see cref="Tag"/> 내에 정의할 수 있는 태그 스크립트입니다.
  /// 이 스크립트를 할당하여 한 개 이상의 태그 정보를 동적으로 적용할 수 있습니다.
  /// 태그 자체 및 부가 효과가 존재할 경우 정의할 수 있습니다.
  /// </summary>
  [Serializable]
  public class TagScript : MonoBehaviour
  {
    public Tag Tag { get; set; }
    /// <summary>태그의 설명을 정의합니다.</summary>
    public virtual string Description => Tag.data.info.tooltip;

    #region Affix
    /// <summary>
    /// 태그가 적용될 경우 가능한 접두사를 정의합니다.
    /// 실제 적용은 우선순위에 따라 결정됩니다.
    /// </summary>
    public virtual Affix Prefix => new();
    /// <summary>
    /// 태그가 적용될 경우 가능한 접미사를 정의합니다.
    /// 실제 적용은 우선순위에 따라 결정됩니다.
    /// </summary>
    public virtual Affix Suffix => new();
    #endregion

    #region Events
    /// <summary>태그가 인근에 작용하는 효과를 정의합니다.</summary>
    public UnityEvent<Tag> EnvironmentEffect = new();
    /// <summary>태그가 구조물에 동적으로 작용하는 효과를 정의합니다.</summary>
    public UnityEvent<Tag/*, Structure*/> StructureEffect = new();
    /// <summary>태그가 아이템 내에 동적으로 작용하는 효과를 정의합니다.</summary>
    public UnityEvent<Tag, TItem> ItemEffect = new();
    #endregion
  }
}
