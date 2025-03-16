// custom edit
// original link : https://github.com/Unity-Technologies/NavMeshComponents/blob/master/Assets/Examples/Scripts/NavMeshSourceTag.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// Tagging component for use with the LocalNavMeshBuilder
// Supports mesh-filter and terrain - can be extended to physics and/or primitives
[DefaultExecutionOrder(-200)]
public class NavMeshSourceTag : MonoBehaviour
{
  // Global containers for all active mesh/terrain tags
  public readonly static HashSet<MeshFilter> Meshes = new();
  public readonly static HashSet<Terrain> Terrains = new();
  public readonly static List<NavMeshBuildSource> Sources = new();

  private MeshFilter[] _MeshComponents;
  private Terrain[] _TerrainComponents;
  void OnEnable() {
    _MeshComponents = GetComponentsInChildren<MeshFilter>();
    foreach (var m in _MeshComponents)
      if (Meshes.Contains(m) is false)
        Meshes.Add(m);
    _TerrainComponents = GetComponentsInChildren<Terrain>();
    foreach (var t in _TerrainComponents)
      if (Terrains.Contains(t) is false)
        Terrains.Add(t);
  }

  void OnDisable() {
    foreach (var m in _MeshComponents)
      Meshes.Remove(m);
    foreach (var t in _TerrainComponents)
      Terrains.Add(t);
  }

  // Collect all the navmesh build sources for enabled objects tagged by this component
  public static void Collect() {
    Sources.Clear();
    foreach (var mf in Meshes) {
      if (mf == null) continue;
      var m = mf.sharedMesh;
      if (m == null) continue;

      var s = new NavMeshBuildSource {
        shape = NavMeshBuildSourceShape.Mesh,
        sourceObject = m,
        transform = mf.transform.localToWorldMatrix,
        area = 0
      };
      Sources.Add(s);
    }

    foreach (var t in Terrains) {
      if (t == null) continue;

      var s = new NavMeshBuildSource {
        shape = NavMeshBuildSourceShape.Terrain,
        sourceObject = t.terrainData,
        // Terrain system only supports translation - so we pass translation only to back-end
        transform = Matrix4x4.TRS(t.transform.position, Quaternion.identity, Vector3.one),
        area = 0
      };
      Sources.Add(s);
    }
  }
}