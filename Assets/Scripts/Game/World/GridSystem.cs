using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.World
{
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
    [SerializeField, ReadonlyField] private State _State = State.NotInitialized;
    private readonly Dictionary<Vector2Int, CellData> _Grid = new();

    public void Setup() {
      if (_State is not State.NotInitialized) {
        this.Log("System has set before. Skipping setup process");
        return;
      }
      _State = State.Initializing;

      StartCoroutine(SetupAllFloorCells());
    }

    const float CellSize = 4, Margin = 0.01f;
    IEnumerator SetupAllFloorCells() {
      this.Log("Generating grid");

      // Variables used
      int floorLayer = LayerMask.NameToLayer("Floor"), floorMask = LayerMask.GetMask("Floor"), obstacleMask = LayerMask.GetMask("Obstacle");
      Vector3 half = (CellSize / 2).ToVector3(), flatHalf = half.SetY(0);

      Bounds bounds = GetAllRenderersBounds(floorLayer);
      RectInt gridBounds = GetGridBounds(bounds);
      float top = bounds.max.y + CellSize, bottom = bounds.min.y;
      this.Log($"Floor bounds set\n[{bounds}] -> [{gridBounds}]\nHeight: {bottom}~{top}");

      yield return GenerateCellData();
      this.Log($"{_Grid.Count} cells generated");

      // Debug Draw
      TDebug.DrawCube(bounds.center, bounds.extents, Color.cyan, 300);
      foreach (var cell in _Grid.Values)
        if (float.IsNaN(cell.height) is false)
          TDebug.DrawCube(cell.center.AddY(CellSize / 2), half, cell.hasObstacle ? Color.red : Color.green, 300);

      _State = State.Ready;
      yield break;

      // Local functions
      IEnumerator GenerateCellData() {
        _Grid.Clear();
        foreach (var cell in gridBounds.allPositionsWithin) {
          var ground = cell.X0Y() * CellSize + flatHalf; // cell center with y = 0
          var height = CellCast(ground.AddY(top), flatHalf, top - bottom, out var hit, floorMask) ? hit.point.y : float.NaN;
          bool hasObstacle = CellCast(ground.AddY(height + CellSize), flatHalf, CellSize, out _, obstacleMask);
          CellData data = new() {
            index = cell,
            center = ground.AddY(height),
            height = height,
            hasObstacle = hasObstacle
          };
          _Grid.Add(cell, data);
          if (Time.TickElapsed(this)) yield return null;
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
        int minX = Mathf.CeilToInt(bounds.min.x / CellSize),
          minZ = Mathf.CeilToInt(bounds.min.z / CellSize),
          maxX = Mathf.FloorToInt(bounds.max.x / CellSize),
          maxZ = Mathf.FloorToInt(bounds.max.z / CellSize);
        return new RectInt(minX, minZ, maxX - minX, maxZ - minZ); // Exclude last edges
      }
      static bool CellCast(Vector3 center, Vector3 half, float maxDistance, out RaycastHit hit, int layerMask) {
        // Shrink the box a bit to avoid hitting the grid's edges, except for the bottom face
        Vector3 centerS = center.AddY(-Margin / 2); // center is slightly below from the original
        Vector3 halfS = new(Mathf.Max(0, half.x - Margin), Mathf.Max(0, half.y - Margin), Mathf.Max(0, half.z - Margin));
        return Physics.BoxCast(centerS, halfS, Vector3.down, out hit, Quaternion.identity, maxDistance, layerMask);
      }
    }
  }
}