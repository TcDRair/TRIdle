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
      EditorGUI.BeginChangeCheck();

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Enable", GUILayout.Width(50));
      GUI.enabled = m_enabled = EditorGUILayout.Toggle(m_enabled, GUILayout.Width(20));
      m_startScene = (SceneAsset)EditorGUILayout.ObjectField(m_startScene, typeof(SceneAsset), false);
      GUI.enabled = true;
      EditorGUILayout.EndHorizontal();

      if (EditorGUI.EndChangeCheck())
        EditorSceneManager.playModeStartScene = m_enabled ? m_startScene : null;
    }

    [MenuItem("Window/Initial Scene")]
    static void Open() => GetWindow<InitialSceneWindow>("Initial Scene Setting");
  }
}