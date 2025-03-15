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
    public float floor;
    public bool hasObstacle;
  }

  public class GridSystem : MonoSingleton<GridSystem>
  {
    public enum State { NotInitialized, Initializing, Ready, Calculating }
    [SerializeField, ReadonlyField] private State m_State = State.NotInitialized;

    public void Setup() {
      if (m_State is not State.NotInitialized) {
        this.Log("System has set before. Skipping setup process");
        return;
      }
      m_State = State.Initializing;

      StartCoroutine(SetupAllFloorCells());
    }

    const float CellSize = 4, InnerMargin = .001f; // TODO : need to be configurable?
    static readonly Vector3 CellOffset = new(CellSize / 2f, 0, CellSize / 2f);
    static readonly Vector3 CellFlatHalfExtents = CellOffset + new Vector3(-InnerMargin, 0, InnerMargin);
    static readonly Vector3 CellHalfExtents = CellFlatHalfExtents + new Vector3(0, CellSize, 0);

    private readonly Dictionary<Vector2Int, CellData> m_FloorCells = new();
    IEnumerator SetupAllFloorCells() {
      yield return null;

      // Default variables
      LayerMask floorLayer = LayerMask.NameToLayer("Floor"), obstacleLayer = LayerMask.NameToLayer("Obstacle");

      // Find All renderers with given layer
      var renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
      Bounds bounds = new();
      foreach (var renderer in renderers)
        if (renderer.gameObject.layer == floorLayer)
          bounds.Encapsulate(renderer.bounds);

      // Setup cell checking variables
      RectInt gridBounds = GetGridBounds(bounds);
      this.Log($"{gridBounds}");
      float topHeight = bounds.max.y;
      this.Log($"All floor objects({renderers.Length}) found.\nFull bounds: {bounds}\nGrid bounds: {gridBounds}\nTop height: {topHeight}\n");

      // Check each cell height
      m_FloorCells.Clear();
      foreach (var cell in gridBounds.allPositionsWithin) {
        var center = new Vector3(cell.x * CellSize, 0, cell.y * CellSize) + CellOffset;
        m_FloorCells.Add(cell, new CellData() {
          index = cell,
          floor = CellCast(center + new Vector3(0, topHeight, 0), CellFlatHalfExtents, out var hit, topHeight, floorLayer) ? hit.point.y : -1,
          hasObstacle = CellCast(center, CellHalfExtents, out _, 0, obstacleLayer)
        });
        if (Time.TickElapsed(this)) yield return null;
      }
      this.Log($"All floor cells({m_FloorCells.Count}) checked.");
      this.Log(string.Join('\n', m_FloorCells.Values.Select(x => $"{x.index}: {x.floor}, {x.hasObstacle}")));
      m_State = State.Ready;

      static RectInt GetGridBounds(Bounds bounds) {
        int mx = Mathf.FloorToInt(bounds.min.x / CellSize);
        int mz = Mathf.FloorToInt(bounds.min.z / CellSize);
        int Mx = Mathf.CeilToInt(bounds.max.x / CellSize);
        int Mz = Mathf.CeilToInt(bounds.max.z / CellSize);
        return new(mx, mz, Mx - mx + 1, Mz - mz + 1);
      }

      static bool CellCast(Vector3 center, Vector3 halfExtents, out RaycastHit hit, float distance, int layer)
        => Physics.BoxCast(center, halfExtents, Vector3.down, out hit, Quaternion.identity, distance, layer);

    }
  }
}