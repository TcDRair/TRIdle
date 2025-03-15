using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.World
{
  using System.Linq;
  using Logics;
  using Logics.Attributes;
  using Logics.Extensions;

  public struct CellData
  {
    public Vector2Int index;
    public Vector3 center;
    public float height;
    public bool hasObstacle;
  }

  public class GridSystem : MonoSingleton<GridSystem>
  {
    public enum State { NotInitialized, Initializing, Ready, Calculating }
    [SerializeField, ReadonlyField] private State m_State = State.NotInitialized;
    private readonly Dictionary<Vector2Int, CellData> m_Grid = new();

    public void Setup() {
      if (m_State is not State.NotInitialized) {
        this.Log("System has set before. Skipping setup process");
        return;
      }
      m_State = State.Initializing;

      StartCoroutine(SetupAllFloorCells());
    }

    const float CellSize = 4, Margin = 0.01f;
    IEnumerator SetupAllFloorCells() {
      yield return new WaitForSeconds(5);
      this.Log("Generating grid");

      // Variables used
      LayerMask floorLayer = LayerMask.NameToLayer("Floor"), obstacleLayer = LayerMask.NameToLayer("Obstacle");
      Vector3 half = (CellSize / 2).ToVector3(), flatHalf = half.SetY(0), offset = new(-Margin, Margin, -Margin);

      Bounds bounds = GetAllRenderersBounds(floorLayer);
      RectInt gridBounds = GetGridBounds(bounds);
      float top = bounds.max.y, bottom = bounds.min.y;
      this.Log($"Floor bounds set\n[{bounds}] -> [{gridBounds}]\nHeight: {bottom}~{top}");

      GenerateCellData();
      this.Log($"{m_Grid.Count} cells generated");
      this.Log(string.Join('\n', m_Grid.Values.Select(d => $"{d.index}: {d.center} / {d.height} / {d.hasObstacle}")));

      m_State = State.Ready;
      yield break;

      // Local functions
      void GenerateCellData() {
        m_Grid.Clear();
        foreach (var cell in gridBounds.allPositionsWithin) {
          var center = cell.X0Y() * CellSize + flatHalf;
          // this.Log($"{center.AddY(top)} / {flatHalf + offset} / {top - bottom} / {floorLayer}");
          var height = CellCast(center.AddY(top), flatHalf + offset, top - bottom, out var hit, floorLayer) ? hit.point.y : float.MinValue;
          bool hasObstacle = CellCast(center.AddY(height), half + offset, 0.01f, out _, obstacleLayer);
          CellData data = new() {
            index = cell,
            center = center,
            height = height,
            hasObstacle = hasObstacle
          };
          m_Grid.Add(cell, data);
        }
      }
      static Bounds GetAllRenderersBounds(LayerMask layer) {
        Bounds bounds = new();
        var renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        foreach (var renderer in renderers)
          if (renderer.gameObject.layer == layer)
            bounds.Encapsulate(renderer.bounds);
        return bounds;
      }
      static RectInt GetGridBounds(Bounds bounds) {
        int minX = Mathf.FloorToInt(bounds.min.x / CellSize),
          minZ = Mathf.FloorToInt(bounds.min.z / CellSize),
          maxX = Mathf.CeilToInt(bounds.max.x / CellSize),
          maxZ = Mathf.CeilToInt(bounds.max.z / CellSize);
        return new RectInt(minX, minZ, maxX - minX + 1, maxZ - minZ + 1);
      }
      static bool CellCast(Vector3 center, Vector3 half, float maxDistance, out RaycastHit hit, int layerMask)
        => Physics.BoxCast(center, half, Vector3.down, out hit, Quaternion.identity, maxDistance/*, layerMask*/);
    }
  }
}