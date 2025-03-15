using UnityEngine;

namespace TRIdle.Logics.Extensions
{
  // Almost all of this can be done without this extension in C# 10.0+
  public static class VectorExtensions
  {
    public static Vector3 ToVector3(this float f) => new(f, f, f);

    public static Vector3 SetX(this Vector3 vector, float x) => new(x, vector.y, vector.z);
    public static Vector3 SetY(this Vector3 vector, float y) => new(vector.x, y, vector.z);
    public static Vector3 SetZ(this Vector3 vector, float z) => new(vector.x, vector.y, z);
    public static Vector3 AddX(this Vector3 vector, float x) => new(vector.x + x, vector.y, vector.z);
    public static Vector3 AddY(this Vector3 vector, float y) => new(vector.x, vector.y + y, vector.z);
    public static Vector3 AddZ(this Vector3 vector, float z) => new(vector.x, vector.y, vector.z + z);


    public static Vector2 XZ(this Vector3 vector) => new(vector.x, vector.z);
    public static Vector3 X0Y(this Vector2 vector) => new(vector.x, 0, vector.y);
    public static Vector3 X0Y(this Vector2Int vector) => new(vector.x, 0, vector.y);
  }
}