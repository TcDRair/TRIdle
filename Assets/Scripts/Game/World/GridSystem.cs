using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.World
{
  using Logics;
  using Logics.Attributes;
  using Logics.Extensions;

  public record CellData
  {
    public Vector2Int index;
    public Vector3 center;
    public float height;
    public bool hasObstacle, hasProp;
  }

  public class GridSystem : MonoSingleton<GridSystem>
  {
    public enum State { NotInitialized, Initializing, Ready, Calculating }
    [SerializeField, ReadonlyField] private State _State = State.NotInitialized;
    private readonly Dictionary<Vector2Int, CellData> _Grid = new();
    private readonly List<Prop> _Props = new();

    #region Features
    public Vector2Int ToGridIndex(Vector3 vector) => new(Mathf.FloorToInt(vector.x / CellSize), Mathf.FloorToInt(vector.z / CellSize));

    #endregion


    #region Initialization

    public void Setup() {
      if (_State is not State.NotInitialized) {
        this.Log("System has set before. Skipping setup process");
        return;
      }
      _State = State.Initializing;

      StartCoroutine(ScanAndGenerateWorld());
    }

    #region Private Fixed Variables
    const float CellSize = 4, HalfSize = CellSize / 2, Margin = 0.01f;
    readonly static Vector3
      s_InnerPadding = Margin.ToVector3(),
      s_InnerFlatPadding = s_InnerPadding.SetY(0),
      s_Extents = HalfSize.ToVector3(),
      s_FlatExtents = s_Extents.SetY(0),
      s_PaddedExtents = s_Extents - s_InnerPadding,
      s_PaddedFlatExtents = s_PaddedExtents.SetY(0);
    #endregion

    IEnumerator ScanAndGenerateWorld() {
      this.Log("Generating grid");

      // Variables used
      int floorLayer = LayerMask.NameToLayer("Floor"), floorMask = LayerMask.GetMask("Floor"), obstacleMask = LayerMask.GetMask("Obstacle");


      Bounds bounds = GetAllRenderersBounds(floorLayer);
      RectInt gridBounds = GetGridBounds(bounds);
      float top = bounds.max.y + CellSize, bottom = bounds.min.y;
      // this.Log($"Floor bounds set\n[{bounds}] -> [{gridBounds}]\nHeight: {bottom}~{top}");

      yield return GenerateCellData();
      this.Log($"{_Grid.Count} cells generated");

      yield return ScanAllProps();
      this.Log($"{_Props.Count} props scanned");

      DebugDrawGrid();

      _State = State.Ready;
      yield break;

      // Local functions
      IEnumerator GenerateCellData() {
        _Grid.Clear();
        Collider[] col = new Collider[100];

        foreach (var cell in gridBounds.allPositionsWithin) {
          Vector3 ground = cell.X0Y() * CellSize + s_FlatExtents; // cell center with y = 0
          // Height : 
          float height =
            Physics.BoxCast(ground.SetY(top), s_PaddedFlatExtents, Vector3.down, out var hit, Quaternion.identity, top - bottom, floorMask)
            ? hit.point.y : float.NaN;
          bool hasObstacle = Physics.OverlapBoxNonAlloc(ground.SetY(height + HalfSize), s_PaddedExtents, col, Quaternion.identity, obstacleMask) > 0;
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
      IEnumerator ScanAllProps() {
        _Props.Clear();
        _Props.AddRange(FindObjectsByType<Prop>(FindObjectsSortMode.None));

        foreach (var prop in _Props) {
          // Check its position info (pivot index / cardinal direction)
          prop.gridTransform.index = ToGridIndex(prop.transform.position);
          Vector3 rot = prop.transform.rotation.eulerAngles;
          var angle = rot.y.PositiveRemainder(360) - 45; // -45 ~ 315
          (prop.gridTransform.cardinal, angle) = angle switch {
            < 45 => (Cardinal.North, 0),
            < 135 => (Cardinal.East, 90),
            < 225 => (Cardinal.South, 180),
            _ => (Cardinal.West, 270)
          };
          rot.y = angle;
          prop.transform.eulerAngles = rot;
          // Apply their occupation to grid. Maybe CellData should have a reference to prop.
          //TODO : for now, only prop's center index is applied.
          _Grid[prop.gridTransform.index].hasProp = true;
        }

        yield break;
      }

      void DebugDrawGrid() {
        TDebug.DrawCube(bounds.center, bounds.extents, Color.cyan, 300);
        Vector3 margin = 0.01f.ToVector3();
        foreach (var cell in _Grid.Values)
          if (float.IsNaN(cell.height) is false)
            TDebug.DrawCube(cell.center.AddY(HalfSize), s_PaddedExtents, cell.hasObstacle ? Color.red : cell.hasProp ? Color.yellow : Color.clear, 300);
        foreach (var prop in _Props)
          if (prop.TryGetComponent<Renderer>(out var renderer))
            TDebug.DrawCube(renderer.bounds.center, renderer.bounds.extents, Color.blue, 300);
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
    }
    #endregion
  }
}