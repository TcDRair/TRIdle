using System.IO;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TRIdle.Editor
{
  using Knowledge;

  public partial class KnowledgeEditor : EditorWindow
  {
    [MenuItem("Window/Knowledge Editor")]
    public static void ShowWindow()
    {
      var window = GetWindow<KnowledgeEditor>("Knowledge Editor");
      window.minSize = new Vector2(600, 360);
      window.Show();

      window.serialization.LoadData();
    }

    void OnEnable() { Initialize_KnowledgeRL(); }

    #region Property
    private class Data
    {
      public string JsonDataPath => Application.persistentDataPath + "/knowledge.json";
      public string JsonKeywordPath => Application.persistentDataPath + "/keywords.json";
      public TextAsset KIAsset, KwAsset;
      public RP_Keyword Keywords => RP_Keyword.Instance;
      public RP_Knowledge Knowledge => RP_Knowledge.Instance;

      public ReorderableList RL_Knowledge;

      public enum FileExistence
      {
        None,
        Main,
        Keyword,
        Both
      }
      public FileExistence CheckFileExistence()
      {
        return (File.Exists(JsonDataPath), File.Exists(JsonKeywordPath)) switch
        {
          (true, true) => FileExistence.Both,
          (true, false) => FileExistence.Main,
          (false, true) => FileExistence.Keyword,
          _ => FileExistence.None
        };
      }
    }
    private static readonly Data data = new();

    private class Serialization
    {
      public void LoadData()
      {
        using var kwStream = new FileStream(data.JsonKeywordPath, FileMode.Open);
        data.Keywords.Load(kwStream);
        data.KwAsset = new() { name = "keywords.json" };
        data.RL_Knowledge.list = data.Keywords.Keys.ToList();

        using var kiStream = new FileStream(data.JsonDataPath, FileMode.Open);
        data.Knowledge.Load(kiStream);
        data.KIAsset = new() { name = "knowledge.json" };

        // DEBUG
        /*data.Add(
          Keyword.None,
          new KI_Trait(new Kw_Trait() {
            Key = Keyword.None,
            Description = "Keyword for Nothing",
          })
        );*/
      }
      public void SaveData()
      {
        if (EditorUtility.DisplayDialog("Save Data", "Are you sure to save the data?", "Yes", "No"))
        {
          using var kwStream = new FileStream(data.JsonKeywordPath, FileMode.Create);
          data.Keywords.Save(kwStream);

          using var kiStream = new FileStream(data.JsonDataPath, FileMode.Create);
          data.Knowledge.Save(kiStream);
        }
      }
      public enum GenerateType { Both, Main, Keyword }
      /*
      private void TryGenerateNewFile(string title, GenerateType type = GenerateType.Both)
      {
        switch (type)
        {
          case GenerateType.Both:
            if (EditorUtility.DisplayDialog(
              title,
              "Need to generate data file for start editing. Do you want to generate a new file?",
              "Generate",
              "Cancel"
            ))
            {
              data.keyword = new();
              data.knowledge = new();
              SaveData();

              data.KwAsset = new();
              data.KIAsset = new();
            }
            break;
          case GenerateType.Main:
            switch (EditorUtility.DisplayDialogComplex(
              title,
              "Need to generate data file for start editing. Do you want to generate a new file?",
              "Generate",
              "Cancel",
              "Data Only"
            ))
            {
              case 0:
                data.knowledge = new();
                SaveData();
                data.KIAsset = new();
                break;
              case 1:
                break;
              case 2:
                data.knowledge = new();
                data.KIAsset = new();
                break;
              default: break;
            }
            break;
          case GenerateType.Keyword:
            switch (EditorUtility.DisplayDialogComplex(
              title,
              "Need to generate data file for start editing. Do you want to generate a new file?",
              "Generate",
              "Cancel",
              "Keyword Only"
            ))
            {
              case 0:
                data.keyword = new();
                SaveData();
                data.KwAsset = new();
                break;
              case 1:
                break;
              case 2:
                data.keyword = new();
                data.KwAsset = new();
                break;
              default: break;
            }
            break;
          default: break;
        }
      }
      */
    }
    private readonly Serialization serialization = new();

    private class GUIState
    {
      public enum Tab { Knowledge, Keywords, Settings, None }
      public Tab tab = Tab.Settings;
      public class KnowledgeState
      {
        public Vector2 Scroll { get; set; }
        public Keyword Selected { get; set; }

        // Knowledge Data
        public IKnowledgeInfo EditData { get; set; }
        public Sprite IconRef { get; set; }
      }
      public KnowledgeState Knowledge { get; } = new();
    }
    private readonly GUIState state = new();
    #endregion

    #region Layout Functions
    private int indent = 0;
    private void BeginIndent(bool horizontalPadding = true, bool verticalPadding = true)
    {
      var style = EStyle.Background(++indent);
      style.padding = (horizontalPadding, verticalPadding) switch
      {
        (true, true) => new(8, 8, 8, 8),
        (true, false) => new(8, 8, 0, 0),
        (false, true) => new(0, 0, 8, 8),
        _ => new(0, 0, 0, 0)
      };
      GUILayout.BeginVertical(style);
    }
    private void EndIndent()
    {
      GUILayout.EndVertical();
      indent--;
    }
    private GUIStyle KeyStyle => new("RL Element")
    {
      alignment = TextAnchor.MiddleCenter,
      fontStyle = FontStyle.Bold,
      fontSize = 16,
      fixedHeight = 32,
    };
    #endregion

    private void OnGUI()
    {
      BeginIndent();
      {
        G_TabMenu();
        G_Main();
      }
      EndIndent();
    }

    #region Main Layout
    void G_TabMenu()
    {
      EditorGUILayout.BeginHorizontal();
      {
        state.tab =
          GUILayout.Button("Knowledge", state.tab == GUIState.Tab.Knowledge ? EStyle.HeaderTabOn : EStyle.HeaderTabOff) ? GUIState.Tab.Knowledge :
          GUILayout.Button("Keywords", state.tab == GUIState.Tab.Keywords ? EStyle.HeaderTabOn : EStyle.HeaderTabOff) ? GUIState.Tab.Keywords :
          GUILayout.Button("Settings", state.tab == GUIState.Tab.Settings ? EStyle.HeaderTabOn : EStyle.HeaderTabOff) ? GUIState.Tab.Settings :
          state.tab;
      }
      EditorGUILayout.EndHorizontal();
    }
    void G_Main()
    {
      BeginIndent();
      {
        var _ = state.tab switch
        {
          GUIState.Tab.Knowledge => GM_Knowledge(),
          GUIState.Tab.Keywords => GM_Keyword(),
          GUIState.Tab.Settings => GM_Settings(),
          _ => G_()
        };
      }
      EndIndent();
    }
    #endregion

    int GM_Keyword()
    {
      if (data.KwAsset == null)
      {
        EditorGUILayout.LabelField(
          "Load or Create Data to Start Editing",
          EStyle.BoldCenterLabel
        );
        return 0;
      }

      EditorGUILayout.LabelField(
        $"{data.Keywords.Count} Keywords Loaded",
        EStyle.BoldCenterLabel
      );

      GUILayout.FlexibleSpace();
      return 0;
    }
    
    int GM_Settings()
    {
      EditorGUILayout.BeginHorizontal();
      {
        EditorGUILayout.LabelField("Files:", GUILayout.Width(40));
        GUI.enabled = false;
        EditorGUILayout.ObjectField(GUIContent.none, data.KIAsset, typeof(TextAsset), false);
        EditorGUILayout.ObjectField(GUIContent.none, data.KwAsset, typeof(TextAsset), false);
        GUI.enabled = true;
        if (GUILayout.Button("Load", GUILayout.Width(50))) serialization.LoadData();
        GUI.enabled = data.KIAsset != null && data.KwAsset != null;
        if (GUILayout.Button("Save", GUILayout.Width(50))) serialization.SaveData();
        GUI.enabled = true;
      }
      EditorGUILayout.EndHorizontal();

      GUILayout.FlexibleSpace();
      return 0;
    }

    int G_() => 0;
  }
}