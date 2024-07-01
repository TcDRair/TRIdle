using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

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

    private void OnGUI()
    {
      EditorGUILayout.BeginHorizontal();
      {
        GUI.enabled = false;
        EditorGUILayout.LabelField("Data File", GUILayout.Width(60));
        EditorGUILayout.ObjectField(GUIContent.none, data.KIAsset, typeof(TextAsset), false, GUILayout.ExpandWidth(true));
        EditorGUILayout.ObjectField(GUIContent.none, data.KwAsset, typeof(TextAsset), false, GUILayout.ExpandWidth(true));
        GUI.enabled = true;
        if (GUILayout.Button("Load", GUILayout.Width(50))) serialization.LoadData();
        GUI.enabled = data.KIAsset != null && data.KwAsset != null;
        if (GUILayout.Button("Save", GUILayout.Width(50))) serialization.SaveData();
        GUI.enabled = true;
      }
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space();
      EditorGUILayout.LabelField($"{data.Keywords.Count} Keywords / {data.Knowledge.Count} Knowledge");
    }
  }
}