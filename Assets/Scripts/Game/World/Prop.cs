using System;

using UnityEngine;

namespace TRIdle.Game.World
{
  using Logics.Attributes;

  public enum Cardinal { North, South, East, West }
  [Serializable]
  public struct PositionInfo
  {
    [ReadonlyField] public Vector2Int index;
    [ReadonlyField] public Cardinal cardinal;
  }

  public class Prop : MonoBehaviour
  {
    // What does prop do?
    // - Is it a building? (e.g. house, shop) : Yes. All props are buildings.
    // - Is it an obstacle? (e.g. wall, tree) : Depends on the prop types. i.e. a simple sign is not an obstacle
    // - Can it be interacted with? : Almost Yes. Without some exceptions succh as skill level limit, all props can be interacted.


    public PositionInfo position;
  }
}