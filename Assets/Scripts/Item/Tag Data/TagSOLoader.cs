using System;
using System.IO;

using Newtonsoft.Json;

using TRIdle.Game.Item;

using UnityEditor;

using UnityEngine;

namespace TRIdle.System
{
  public class TagSOLoader : MonoBehaviour
  {
    int files = 0;
    int scriptCount = 0;
    /// <summary>
    /// 디렉터리 내 모든 Json으로부터 <see cref="Tag"/> 에셋을 생성합니다.
    /// Json 파일명을 기반으로 에셋 폴더를 생성합니다.
    /// </summary>
    /// <param name="source">추출하려는 Json 파일들이 위치한 디렉터리 경로입니다. <see cref="Resources"/>에서 접근할 수 있어야 합니다.</param>
    /// <param name="target">생성한 에셋 폴더가 위치할 디렉터리 경로입니다. <see cref="AssetDatabase"/>에서 접근할 수 있어야 합니다.</param>
    public void USOFromJsons(string source, string target) {
      Debug.Log($"Loading UniqueSO from {source} to {target}");
      foreach (var text in Resources.LoadAll<TextAsset>(source)) {
        #region Fetching
        files = 0;
        // check json
        var path = AssetDatabase.GetAssetPath(text);
        if (path.EndsWith(".json") is false) continue;
        #endregion

        #region Directory Checking
        // create assets
        string targetDirectory = $"{target}/{Path.GetFileNameWithoutExtension(path)}";
        if (Directory.Exists(targetDirectory)) {
          // clear directory by removing it
          AssetDatabase.DeleteAsset(path[..path.LastIndexOf('/')]); //TODO : Check if this is correct
          AssetDatabase.SaveAssets();
        }
        Directory.CreateDirectory(targetDirectory);
        #endregion

        #region Asset Creation
        foreach (var tag in JsonConvert.DeserializeObject<TagInfo[]>(text.text)) {
          var infoPath = $"{targetDirectory}/{tag.uName}.asset";
          AssetDatabase.CreateAsset(tag, infoPath);

          // Create TagInfo SO
          var asset = new GameObject(tag.uName);
          var data = asset.AddComponent<TagData>();
          data.info = tag;

          // Load TagScript if found
          var l = AssetDatabase.FindAssets($"TS_{tag.uName}");
          if (l.Length > 1) {
            Debug.LogError($"Multiple TagScripts found for TS_{tag.uName}. Only one TagScript is allowed.");
          } else if (l.Length == 1) {
            var type = Type.GetType($"{typeof(TagScript).Namespace}.TS_{tag.uName}");
            if (type != null) {
              var script = asset.AddComponent(type);
              data.script = script as TagScript;
              scriptCount++;
            }
          }

          var dataPath = $"{targetDirectory}/{tag.uName}.prefab";
          PrefabUtility.SaveAsPrefabAsset(asset, dataPath);
          DestroyImmediate(asset);
          files++;
        }
        #endregion
      }

      #region Saving
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      #endregion

      Debug.Log($"UniqueSOs created: {files}");
      Debug.Log($"Scripts: {scriptCount}");
    }
  }

  [CustomEditor(typeof(TagSOLoader))]
  public class TagSOLoaderEditor : Editor
  {
    public override void OnInspectorGUI() {
      DrawDefaultInspector();
      if (GUILayout.Button("(Test) Generate TagInfo Assets")) {
        var loader = target as TagSOLoader;
        loader.USOFromJsons(
          PathHolder.Resources.TagJson,
          PathHolder.Resources.TagSO.ResourcesPathToAssetPath()
        );
      }
    }
  }
}
