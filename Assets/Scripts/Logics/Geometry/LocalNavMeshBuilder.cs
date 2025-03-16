// https://github.com/Unity-Technologies/NavMeshComponents/blob/master/Assets/Examples/Scripts/LocalNavMeshBuilder.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

namespace TRIdle.Logics.Geometry
{
  // Build and update a localized navmesh from the sources marked by NavMeshSourceTag
  [DefaultExecutionOrder(-102)]
  public class LocalNavMeshBuilder : MonoBehaviour
  {
    // The center of the build
    [SerializeField] private Transform _Tracked;

    // The size of the build bounds
    [SerializeField] private Vector3 _Size = new(80.0f, 20.0f, 80.0f);

    NavMeshData _NavMesh;
    AsyncOperation _Operation;
    NavMeshDataInstance _Instance;

    private static List<NavMeshBuildSource> Sources => NavMeshSourceTag.Sources;

    IEnumerator Start() {
      while (true) {
        UpdateNavMesh(true);
        yield return _Operation;
      }
    }

    void OnEnable() {
      // Construct and add navmesh
      _NavMesh = new NavMeshData();
      _Instance = NavMesh.AddNavMeshData(_NavMesh);
      if (_Tracked == null) _Tracked = transform;
      UpdateNavMesh(false);
    }

    void OnDisable() {
      // Unload navmesh and clear handle
      _Instance.Remove();
    }

    void UpdateNavMesh(bool asyncUpdate = false) {
      NavMeshSourceTag.Collect();
      var defaultBuildSettings = NavMesh.GetSettingsByID(0);
      var bounds = QuantizedBounds();

      if (asyncUpdate)
        _Operation = NavMeshBuilder.UpdateNavMeshDataAsync(_NavMesh, defaultBuildSettings, Sources, bounds);
      else
        NavMeshBuilder.UpdateNavMeshData(_NavMesh, defaultBuildSettings, Sources, bounds);
    }

    static Vector3 Quantize(Vector3 v, Vector3 quant) {
      float x = quant.x * Mathf.Floor(v.x / quant.x);
      float y = quant.y * Mathf.Floor(v.y / quant.y);
      float z = quant.z * Mathf.Floor(v.z / quant.z);
      return new Vector3(x, y, z);
    }

    Bounds QuantizedBounds() {
      // Quantize the bounds to update only when theres a 10% change in size
      var center = _Tracked ? _Tracked.position : transform.position;
      return new Bounds(Quantize(center, 0.1f * _Size), _Size);
    }

    void OnDrawGizmosSelected() {
      if (_NavMesh) {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_NavMesh.sourceBounds.center, _NavMesh.sourceBounds.size);
      }

      Gizmos.color = Color.yellow;
      var bounds = QuantizedBounds();
      Gizmos.DrawWireCube(bounds.center, bounds.size);

      Gizmos.color = Color.green;
      var center = _Tracked ? _Tracked.position : transform.position;
      Gizmos.DrawWireCube(center, _Size);
    }
  }
}