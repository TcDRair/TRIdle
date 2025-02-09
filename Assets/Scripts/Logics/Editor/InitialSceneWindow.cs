using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace TRIdle.Logics.Editor
{
  public class InitialSceneWindow : EditorWindow
  {

    bool m_enabled;
    SceneAsset m_startScene;
    void OnGUI() {
      if (!m_initialized) Initialize();

      EditorGUI.BeginChangeCheck();

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Enable", GUILayout.Width(50));
      GUI.enabled = m_enabled = EditorGUILayout.Toggle(m_enabled, GUILayout.Width(20));
      m_startScene = (SceneAsset)EditorGUILayout.ObjectField(m_startScene, typeof(SceneAsset), false);
      GUI.enabled = true;
      EditorGUILayout.EndHorizontal();

      if (EditorGUI.EndChangeCheck()) {
        EditorSceneManager.playModeStartScene = m_enabled ? m_startScene : null;
        Save();
      }
    }

    [MenuItem("Window/Initial Scene")]
    static void Open() => GetWindow<InitialSceneWindow>("Initial Scene Setting");


    const string kEnabledKey = "InitialSceneWindow.Enabled";
    const string kSceneKey = "InitialSceneWindow.Scene";
    bool m_initialized;
    void Initialize() {
      m_enabled = EditorPrefs.GetBool("InitialSceneWindow.Enabled", false);
      m_startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorPrefs.GetString("InitialSceneWindow.Scene", ""));
      m_initialized = true;
    }
    void Save() {
      EditorPrefs.SetBool(kEnabledKey, m_enabled);
      EditorPrefs.SetString(kSceneKey, m_startScene ? AssetDatabase.GetAssetPath(m_startScene) : "");
    }
  }
}