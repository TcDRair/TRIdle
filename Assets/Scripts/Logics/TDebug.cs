using UnityEngine;

namespace TRIdle.Logics
{
  public static class TDebug
  {
    public static void DrawCube(Vector3 center, Vector3 halfExtents, Color color, float duration = 5, bool depthTest = true) {
      Vector3 h = halfExtents,
        p1 = new(h.x, h.y, h.z),
        p2 = new(-h.x, h.y, h.z),
        p3 = new(-h.x, -h.y, h.z),
        p4 = new(h.x, -h.y, h.z),
        p5 = new(h.x, h.y, -h.z),
        p6 = new(-h.x, h.y, -h.z),
        p7 = new(-h.x, -h.y, -h.z),
        p8 = new(h.x, -h.y, -h.z);

      // Upper surface
      Debug.DrawLine(center + p1, center + p2, color, duration, depthTest);
      Debug.DrawLine(center + p2, center + p3, color, duration, depthTest);
      Debug.DrawLine(center + p3, center + p4, color, duration, depthTest);
      Debug.DrawLine(center + p4, center + p1, color, duration, depthTest);
      // Lower surface
      Debug.DrawLine(center + p5, center + p6, color, duration, depthTest);
      Debug.DrawLine(center + p6, center + p7, color, duration, depthTest);
      Debug.DrawLine(center + p7, center + p8, color, duration, depthTest);
      Debug.DrawLine(center + p8, center + p5, color, duration, depthTest);
      // Side edges
      Debug.DrawLine(center + p1, center + p5, color, duration, depthTest);
      Debug.DrawLine(center + p2, center + p6, color, duration, depthTest);
      Debug.DrawLine(center + p3, center + p7, color, duration, depthTest);
      Debug.DrawLine(center + p4, center + p8, color, duration, depthTest);
    }
  }
}