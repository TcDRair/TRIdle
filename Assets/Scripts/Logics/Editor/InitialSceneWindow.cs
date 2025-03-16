using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace TRIdle.Logics.Editor
{
  public class InitialSceneWindow : EditorWindow
  {
    bool _Enabled;
    SceneAsset _StartScene;
    void OnGUI() {
      if (!_Initialized) Initialize();

      EditorGUI.BeginChangeCheck();

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Enable", GUILayout.Width(50));
      GUI.enabled = _Enabled = EditorGUILayout.Toggle(_Enabled, GUILayout.Width(20));
      _StartScene = (SceneAsset)EditorGUILayout.ObjectField(_StartScene, typeof(SceneAsset), false);
      GUI.enabled = true;
      EditorGUILayout.EndHorizontal();

      if (EditorGUI.EndChangeCheck()) {
        Set();
        Save();
      }
    }

    [MenuItem("Window/Initial Scene")]
    static void Open() => GetWindow<InitialSceneWindow>("Initial Scene Setting");


    const string EnabledKey = "InitialSceneWindow.Enabled";
    const string SceneKey = "InitialSceneWindow.Scene";
    bool _Initialized;
    void Initialize() {
      _Enabled = EditorPrefs.GetBool("InitialSceneWindow.Enabled", false);
      _StartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorPrefs.GetString("InitialSceneWindow.Scene", ""));
      _Initialized = true;
      Set();
    }
    void Set() => EditorSceneManager.playModeStartScene = _Enabled ? _StartScene : null;
    void Save() {
      EditorPrefs.SetBool(EnabledKey, _Enabled);
      EditorPrefs.SetString(SceneKey, _StartScene ? AssetDatabase.GetAssetPath(_StartScene) : "");
    }
  }
}