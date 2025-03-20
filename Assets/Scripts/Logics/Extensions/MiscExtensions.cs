using UnityEngine;

namespace TRIdle.Logics.Extensions
{
  public static class MiscExtensions
  {
    public static float PositiveRemainder(this float f, float divisor) => (f % divisor + divisor) % divisor;

    public static int PositiveRemainder(this int i, int divisor) => (i % divisor + divisor) % divisor;
  }
}