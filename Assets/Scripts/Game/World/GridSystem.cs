using System.Collections;

using UnityEngine;

namespace TRIdle.Game.World
{
  using Logics;
  using Logics.Attributes;
  using Logics.Extensions;

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

    const float m_CellSize = 4; // TODO : need to manage?
    IEnumerator SetupAllFloorCells() {
      yield return null;

      // Find All renderers with given layer
      Bounds bounds = new();
      LayerMask floorLayer = LayerMask.NameToLayer("Floor");

      foreach (var renderer in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        if (renderer.gameObject.layer == floorLayer)
          bounds.Encapsulate(renderer.bounds);
          
      this.Log($"All floor bounds: {bounds}");
      int minX = Mathf.FloorToInt(bounds.min.x / m_CellSize),
        minZ = Mathf.FloorToInt(bounds.min.z / m_CellSize),
        maxX = Mathf.CeilToInt(bounds.max.x / m_CellSize),
        maxZ = Mathf.CeilToInt(bounds.max.z / m_CellSize);
      RectInt gridBounds = new(minX, minZ, maxX, maxZ);
      float topY = bounds.max.y;

      // TODO : Create All cell info in 2d data structure

      m_State = State.Ready;
    }
  }
}