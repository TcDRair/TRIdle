using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TRIdle.Editor
{
  using Knowledge;

  public class KnowledgeEditor : EditorWindow
  {
    [MenuItem("Window/Knowledge Editor")]
    public static void ShowWindow()
    {
      var window = GetWindow<KnowledgeEditor>("Knowledge Editor");
      window.minSize = new Vector2(600, 360);
      window.Show();

      window.serialization.LoadData();
    }

    #region Property
    private class Data
    {
      public string JsonDataPath => Application.persistentDataPath + "/knowledge.json";
      public string JsonKeywordPath => Application.persistentDataPath + "/keywords.json";
      public TextAsset KIAsset, KwAsset;
      public RP_Keyword Keywords => RP_Keyword.Instance;
      public RP_Knowledge Knowledge => RP_Knowledge.Instance;

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
      public Vector2 kiScroll, kwScroll;
      public Keyword kiSelected, kwSelected;
    }
    private readonly GUIState state = new();
    #endregion

    #region Layout Functions
    private int indent = 0;
    private void BeginIndent() {
      var style = EStyle.Background(++indent);
      style.padding = new RectOffset(8, 8, 8, 8);
      GUILayout.BeginVertical(style);
    }
    private void EndIndent() {
      GUILayout.EndVertical();
      indent--;
    }
    private GUIStyle KeyStyle => new(EditorStyles.toolbarButton) {
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
    int GM_Knowledge()
    {
      if (data.KIAsset == null)
      {
        EditorGUILayout.LabelField(
          "Load or Create Data to Start Editing",
          EStyle.BoldCenterLabel
        );
        return 0;
      }

      EditorGUILayout.LabelField(
        $"{data.Knowledge.Count} Knowledge Data Loaded",
        EStyle.BoldCenterLabel
      );

      EditorGUILayout.BeginHorizontal();
      {
        // Knowledge Key List (Keyword)
        state.kiScroll = GUILayout.BeginScrollView(
          state.kiScroll, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, // Scroll Bar Settings
          GUILayout.Width(200), GUILayout.ExpandHeight(true) // Background Settings
        );
        BeginIndent();
        {
          foreach (var key in data.Knowledge.Keys)
            if (GUILayout.Button(key.ToString(), KeyStyle))
              state.kiSelected = key;
        }
        GUILayout.FlexibleSpace();
        EndIndent();
        GUILayout.EndScrollView();

        // Knowledge Data (Selected)
        BeginIndent();
        {
          // if (state.kiSelected != Keyword.None)
          {
            var ki = data.Knowledge.GetData(state.kiSelected);
            EditorGUILayout.TextArea(
              ki.GetDescription(),
              EStyle.RichText
            );
          }
        }
        GUILayout.FlexibleSpace();
        EndIndent();
      }
      EditorGUILayout.EndHorizontal();
      return 0;
    }
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