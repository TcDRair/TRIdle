using System.IO;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TRIdle.Editor {
  using Knowledge;

  public class KnowledgeEditor : EditorWindow {
    #region Initialization
    public static KnowledgeEditor Instance { get; private set; }

    [MenuItem("Window/Knowledge Editor")]
    public static void ShowWindow() {
      var window = GetWindow<KnowledgeEditor>("Knowledge Editor");
      window.minSize = new Vector2(600, 360);
      window.Show();
    }

    void InitializeTab() {
      KnowledgeTab.Initialize();
      KeywordTab.Initialize();
      SettingTab.Initialize();
    }

    void OnEnable() {
      Instance = this;
    }
    #endregion

    #region Property
    public KE_KnowledgeTab KnowledgeTab { get; } = new();
    public KE_KeywordTab KeywordTab { get; } = new();
    public KE_SettingTab SettingTab { get; } = new();

    public string JsonKeywordPath => Application.persistentDataPath + "/keywords.json";
    public string JsonDataPath => Application.persistentDataPath + "/knowledge.json";

    private class SerializationProperties {
      public void LoadData() {
        using var keywordStream = new FileStream(Instance.JsonKeywordPath, FileMode.Open);
        RP_Keyword.Instance.Load(keywordStream);

        using var knowledgeStream = new FileStream(Instance.JsonDataPath, FileMode.Open);
        RP_Knowledge.Instance.Load(knowledgeStream);

        Instance.InitializeTab();
      }

      public void SaveData() {
        if (EditorUtility.DisplayDialog("Save Data", "Are you sure to save the data?", "Yes", "No")) {
          using var kwStream = new FileStream(Instance.JsonKeywordPath, FileMode.Create);
          RP_Keyword.Instance.Save(kwStream);

          using var kiStream = new FileStream(Instance.JsonDataPath, FileMode.Create);
          RP_Knowledge.Instance.Save(kiStream);
        }
      }
    }
    private SerializationProperties Serialization { get; } = new();

    private class StateProperties {
      public enum Tab { Knowledge, Keywords, Setting, None }
      public Tab tab = Tab.Setting;
    }
    private StateProperties State { get; } = new();
    #endregion

    private void OnGUI() {
      ELayout.BeginIndent();

      TabLayout();
      MainLayout();

      ELayout.EndIndent();
    }

    void TabLayout() {
      EditorGUILayout.BeginHorizontal();
      State.tab =
        GUILayout.Button("Knowledge", State.tab == StateProperties.Tab.Knowledge ? EStyle.HeaderTabOn : EStyle.HeaderTabOff) ? StateProperties.Tab.Knowledge :
        GUILayout.Button("Keywords", State.tab == StateProperties.Tab.Keywords ? EStyle.HeaderTabOn : EStyle.HeaderTabOff) ? StateProperties.Tab.Keywords :
        GUILayout.Button("Setting", State.tab == StateProperties.Tab.Setting ? EStyle.HeaderTabOn : EStyle.HeaderTabOff) ? StateProperties.Tab.Setting :
        State.tab;
      EditorGUILayout.EndHorizontal();
    }

    void MainLayout() {
      ELayout.BeginIndent();
      var _ = State.tab switch {
        StateProperties.Tab.Knowledge => KnowledgeTab.DoLayout(),
        StateProperties.Tab.Keywords => GM_Keyword(),
        StateProperties.Tab.Setting => GM_Setting(),
        _ => throw new System.NotImplementedException()
      };
      ELayout.EndIndent();
    }

    int GM_Keyword() {
      return 0;
    }

    int GM_Setting() {
      TextAsset kwA = KeywordTab.State.DisplayAsset, kiA = KnowledgeTab.State.DisplayAsset;

      EditorGUILayout.BeginHorizontal();
      {
        EditorGUILayout.LabelField("Files:", GUILayout.Width(40));
        GUI.enabled = false;
        EditorGUILayout.ObjectField(kwA, typeof(TextAsset), false);
        EditorGUILayout.ObjectField(kiA, typeof(TextAsset), false);
        GUI.enabled = true;
        if (GUILayout.Button("Load", GUILayout.Width(50))) Serialization.LoadData();
        GUI.enabled = kwA != null && kiA != null;
        if (GUILayout.Button("Save", GUILayout.Width(50))) Serialization.SaveData();
        GUI.enabled = true;
      }
      EditorGUILayout.EndHorizontal();

      GUILayout.FlexibleSpace();
      return 0;
    }
  }


  public interface ITabLayout {
    void Initialize();
    int DoLayout();
  }
}