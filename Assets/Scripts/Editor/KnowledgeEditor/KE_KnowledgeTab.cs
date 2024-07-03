using System;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TRIdle.Editor
{
  using System.Collections.Generic;
  using Knowledge;

  public partial class KnowledgeEditor
  {
    const string KIAssetPath = "Assets/Resources/Sprite/Knowledge/";


    void Initialize_KnowledgeRL()
    {
      data.RL_Knowledge = new(data.Knowledge.Keys.ToList(), typeof(Keyword), true, true, true, true)
      {
        drawHeaderCallback = rect =>
        {
          int count = data.Knowledge.Count;
          EditorGUI.LabelField(rect, $"{count} Keyword{(count > 1 ? "s" : string.Empty)}");
        },
        drawElementCallback = (rect, index, active, focused) =>
        {
          var key = (Keyword)data.RL_Knowledge.list[index];
          EditorGUI.LabelField(rect, key.ToString());
        },
        onAddCallback = list =>
        {
          data.Knowledge.TryAddKey(Keyword.None);
          list.list = data.Knowledge.Keys.ToList();
          list.index = list.list.Count - 1;

          SetKnowledgeRLIndex(list);
        },
        onCanAddCallback = list => data.Knowledge.TryGetData(Keyword.None, out _) is false,
        onRemoveCallback = list =>
        {
          data.Knowledge.Remove((Keyword)list.list[list.index]);
          list.list = data.Knowledge.Keys.ToList();
          list.index = Math.Min(list.index, list.list.Count - 1);

          SetKnowledgeRLIndex(list);
        },
        onSelectCallback = SetKnowledgeRLIndex
      };
    }

    void SetKnowledgeRLIndex(ReorderableList list)
    {
      state.Knowledge.Selected = (Keyword)list.list[list.index];
      state.Knowledge.EditData = data.Knowledge.GetData(state.Knowledge.Selected);
    }

    int GM_Knowledge()
    {
      // Data Check
      if (data.KIAsset == null)
      {
        EditorGUILayout.LabelField(
          "Load or Create Data to Start Editing",
          EStyle.BoldCenterLabel
        );
        GUILayout.FlexibleSpace();
        return 0;
      }

      EditorGUILayout.BeginHorizontal();
      {
        #region List Area
        BeginIndent();
        {
          data.RL_Knowledge.DoLayoutList();
          EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(200));
          EStyle.FlexibleHeight();
        }
        EndIndent();
        #endregion

        #region Edit Area
        BeginIndent();
        if (state.Knowledge.EditData != null)
        {
          KnowledgeEditMenu();
          GUILayout.FlexibleSpace();
          KnowledgeButtonMenu();
        }
        else
        {
          EditorGUILayout.LabelField("Select a Keyword to Edit", EStyle.BoldCenterLabel);
          // Layout margins (Don't know why it works)
          GUILayout.BeginHorizontal();
          GUILayout.FlexibleSpace();
          GUILayout.EndHorizontal();

          EStyle.FlexibleHeight();
        }
        EndIndent();
        #endregion
      }
      EditorGUILayout.EndHorizontal();
      return 0;
    }

    void KnowledgeEditMenu()
    {
      var edit = state.Knowledge.EditData;

      // Keyword
      edit.Keyword = (Keyword)EditorGUILayout.EnumPopup("Keyword", edit.Keyword);

      // Type
      var type = (KeywordType)EditorGUILayout.EnumPopup("Type", edit.Type);
      if (type != edit.Type)
        if (EditorUtility.DisplayDialog("Type Change", "Changing the type will discard specific data. Are you sure?", "Yes", "No"))
          edit = state.Knowledge.EditData = edit.Convert(type);

      // Description
      edit.FlatDescription = EditorGUILayout.TextArea(edit.FlatDescription);

      // Icon
      // TODO : Resource Load IconReference
      var assetPath = $"Assets/Resources/{edit.ImagePath}";
      if (state.Knowledge.IconRef == null) state.Knowledge.IconRef = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
      state.Knowledge.IconRef = EditorGUILayout.ObjectField("Icon", state.Knowledge.IconRef, typeof(Sprite), false, GUILayout.ExpandWidth(true)) as Sprite;
      var path = AssetDatabase.GetAssetPath(state.Knowledge.IconRef);
      if (path?.Length > 0)
      {
        var newPath = $"{KIAssetPath}{edit.Keyword}.png";
        // remove "Assets/Resources/" 
        edit.ImagePath = newPath[17..];
        if (path != newPath && AssetDatabase.CopyAsset(path, newPath)) {
          if (path.StartsWith(KIAssetPath)) AssetDatabase.DeleteAsset(path);
          AssetDatabase.Refresh();
          state.Knowledge.IconRef = AssetDatabase.LoadAssetAtPath<Sprite>(newPath);
        }
        else Debug.LogWarning($"Failed to copy the icon({path}) to {newPath}");
      }
      else edit.ImagePath = "";

      // Specific Data
      switch (edit.Type)
      {
        case KeywordType.Trait:
          var trait = edit as KI_Trait;
          break;
        case KeywordType.Item:
          var item = edit as KI_Item;
          break;
      }
    }

    void KnowledgeButtonMenu()
    {
      GUILayout.BeginHorizontal();
      {
        GUILayout.FlexibleSpace();
        GUI.enabled = state.Knowledge.EditData != null;
        bool trySave = GUILayout.Button("Save", GUILayout.Width(50));
        GUI.enabled = true;
        if (trySave && EditorUtility.DisplayDialog("Save Changes", "Are you sure to save the changes?", "Yes", "No"))
        {
          if (state.Knowledge.Selected != state.Knowledge.EditData.Keyword)
          {
            data.Knowledge.Set(state.Knowledge.EditData.Keyword, state.Knowledge.EditData);
            data.Knowledge.Remove(state.Knowledge.Selected);
            state.Knowledge.Selected = state.Knowledge.EditData.Keyword;
            data.RL_Knowledge.list = data.Knowledge.Keys.ToList();
          }
          else data.Knowledge.Replace(state.Knowledge.Selected, state.Knowledge.EditData);
          SetKnowledgeRLIndex(data.RL_Knowledge);
        }
        if (GUILayout.Button("Cancel", GUILayout.Width(50))) state.Knowledge.EditData = data.Knowledge.GetData(state.Knowledge.Selected);
      }
      GUILayout.EndHorizontal();
    }

  }
}