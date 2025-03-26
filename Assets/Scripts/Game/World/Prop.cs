using System;

using UnityEngine;

namespace TRIdle.Game.World
{
  using Logics.Attributes;

  public enum Cardinal { North, South, East, West }
  [Serializable]
  public struct GridTransform
  {
    [ReadonlyField] public Vector2Int index;
    [ReadonlyField] public Cardinal cardinal;
  }

  public class Prop : MonoBehaviour
  {
    public GridTransform gridTransform;
    public virtual bool IsInteractable() => true;
  }
}