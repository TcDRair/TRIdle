using UnityEngine;

namespace TRIdle.Logics.Extensions
{
  public static class MiscExtensions
  {
    public static int ToMask(this LayerMask layer) => 1 << layer;
  }
}