using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TRIdle.Editor {
  using Knowledge;

  public class KE_KnowledgeTab : ITabLayout {
    const string KIAssetPath = "Assets/Resources/Sprite/Knowledge/";
    public RP_Knowledge Data => RP_Knowledge.Instance;

    public class StateProperties {
      public ReorderableList DisplayList;
      public TextAsset DisplayAsset { get; set; }

      // Key Menu State
      public Keyword KeywordMenu { get; set; }
      public Vector2 KeyMenuScroll { get; set; }

      // Knowledge Data
      public IKnowledgeInfo EditData { get; set; }
      public Keyword KeywordPopup { get; set; }

      // GUI Layout Data (temporary)
      public bool IconLoaded { get; set; }
      public Sprite SelectedIcon { get; set; }

      public void UpdateIndex(int index) {
        if (index < 0) { KeywordMenu = default; EditData = default; return; }
        KeywordMenu = (Keyword)DisplayList.list[index];
        EditData = RP_Knowledge.Instance.GetData(KeywordMenu);

        IconLoaded = false;
        SelectedIcon = null;
      }
    }
    public StateProperties State { get; set; } = new();

    #region Init / Update
    public void Initialize() {
      State.DisplayAsset = new() { name = "knowledge.json" };

      State.DisplayList = new(Data.Keys.ToList(), typeof(Keyword), true, true, true, true) {
        drawHeaderCallback = rect => {
          int count = Data.Count;
          EditorGUI.LabelField(rect, $"{count} Keyword{(count > 1 ? "s" : string.Empty)}");
        },
        drawElementCallback = (rect, index, active, focused) => {
          var key = (Keyword)State.DisplayList.list[index];
          EditorGUI.LabelField(rect, key.ToString());
        },
        onAddCallback = list => {
          Data.TryAddDefault();
          list.list = Data.Keys.ToList();
          if (list.index < 0) State.UpdateIndex(list.index = 0);
        },
        onCanAddCallback = list => Data.TryGetData(Keyword.None, out _) is false,
        onRemoveCallback = list => {
          Data.Remove((Keyword)list.list[list.index]);
          list.list.RemoveAt(list.index);
          State.UpdateIndex(--list.index);
        },
        onSelectCallback = list => State.UpdateIndex(list.index),
      };
    }
    public void UpdateDisplayList() => UpdateDisplayList(State.DisplayList);
    public void UpdateDisplayList(ReorderableList list) {
      // Update the list
      list.list = Data.Keys.ToList();

      // Get the current index keyword
      list.index = Math.Min(list.index, list.count - 1);
      var displayedKeyword = (Keyword)list.list[list.index];

      // If target keyword is different
      if (State.KeywordMenu != displayedKeyword) {
        // 1. If the keyword is moved to the new index, update the index
        if (Data.TryGetData(displayedKeyword, out _))
          list.index = list.list.IndexOf(displayedKeyword);
        // 2. If the keyword does not exist in the data, get the closest one instead.
        else {
          State.KeywordMenu = displayedKeyword;

          // Refresh state. Layout will be updated in the next frame.
          State.IconLoaded = false;
          State.SelectedIcon = null;

          State.EditData = Data.GetData(State.KeywordMenu);
          return;
        }
      }
      // Update the data only if the keyword not has been set
      State.EditData ??= Data.GetData(State.KeywordMenu);
    }
    #endregion

    #region Layout
    public int DoLayout() {
      // Data Check
      if (State.DisplayAsset == null) {
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
        ELayout.BeginIndent();
        {
          State.DisplayList.DoLayoutList();
          EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(200));
          ELayout.FlexibleHeight();
        }
        ELayout.EndIndent();
        #endregion

        #region Edit Area
        ELayout.BeginIndent();
        EditLayout();
        ELayout.EndIndent();
        #endregion
      }
      EditorGUILayout.EndHorizontal();
      return 0;
    }

    void EditLayout() {
      var edit = State.EditData;
      if (edit == null) {
        EditorGUILayout.LabelField("Select a Keyword to Edit", EStyle.BoldCenterLabel);
        GUILayout.FlexibleSpace();
        ButtonLayout();
        return;
      }

      #region Layout Body
      EditorGUILayout.BeginHorizontal();
      {
        EditorGUILayout.BeginVertical();
        {
          SelectKeyLayout();
          SelectTypeLayout();
        }
        EditorGUILayout.EndVertical();
        SelectIconLayout();
      }
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.Space();

      DescriptionLayout();
      // TODO : 생각 좀 해보자
      // Knowledge에 Keyword 요소를 추가하는 것은 기본적으로 Description에 영향이 가야 한다.
      // 당연히 이 때 Serialized되는 텍스트는 Regex로 처리할 수 있는데, 에디터상에서는 어떻게 표현해야 하는가
      // 1. Description에 특정 키워드를 입력하면 해당 키워드를 선택할 수 있는 버튼이 나타난다.
      //    - 중요한 점 : 커서 바로 밑에 오버레이가 나타나야 한다. 유니티상에서 가능할까?

      // Specific Data
      switch (edit.Type) {
        case KeywordType.Trait:
          var trait = edit as KI_Trait;
          break;
        case KeywordType.Item:
          var item = edit as KI_Item;
          break;
      }

      GUILayout.FlexibleSpace();

      ButtonLayout();
      #endregion

      #region Partial Layouts
      void SelectKeyLayout() {
        var validKeys = GetAvailableKeywords(State.KeywordMenu).ToArray();
        edit.Keyword = ELayout.Popup("Keyword", edit.Keyword, validKeys);
      }

      void SelectTypeLayout() {
        var type = (KeywordType)EditorGUILayout.EnumPopup("Type", edit.Type);
        if (type != edit.Type)
          if (EditorUtility.DisplayDialog("Type Change", "Type change will discard specific Main.data. Are you sure?", "Yes", "No"))
            edit = State.EditData = edit.Convert(type);
      }

      void SelectIconLayout() {
        if (State.IconLoaded is false) {
          var savedPath = $"Assets/Resources/{edit.IconPath}";
          if (AssetDatabase.LoadAssetAtPath<Sprite>(savedPath) is Sprite savedSprite) {
            State.SelectedIcon = savedSprite;
          }
          State.IconLoaded = true;
        }

        State.SelectedIcon = EditorGUILayout.ObjectField(State.SelectedIcon, typeof(Sprite), false, GUILayout.Width(64), GUILayout.Height(64)) as Sprite;
        var selectedIconPath = AssetDatabase.GetAssetPath(State.SelectedIcon);

        // Check if the icon is changed
        if (selectedIconPath?.Length > 0) {
          var newPath = $"{KIAssetPath}{edit.Keyword}.png";
          // Remove "Assets/Resources/" from the path (AssetPath => ResourcePath)
          edit.IconPath = newPath[17..];
          // Try to copy the icon to the new path
          if (selectedIconPath != newPath && AssetDatabase.CopyAsset(selectedIconPath, newPath)) {
            if (selectedIconPath.StartsWith(KIAssetPath)) AssetDatabase.DeleteAsset(selectedIconPath);
            AssetDatabase.Refresh();
            State.SelectedIcon = AssetDatabase.LoadAssetAtPath<Sprite>(newPath);
          }
        }
        else edit.IconPath = "";
      }

      void DescriptionLayout() {
        GUILayout.BeginHorizontal();
        {
          EditorGUILayout.LabelField("Description", GUILayout.Width(150));

          // var text = EKeywordUtility.ParseToDynamic(edit.FlatDescription);
          edit.FlatDescription = ELayout.DynamicTextArea("text", edit.FlatDescription);
        }
        GUILayout.EndHorizontal();
      }


      void ButtonLayout() {
        GUILayout.BeginHorizontal();
        {
          GUILayout.FlexibleSpace();
          GUI.enabled = State.EditData != null
            ? Data.TryGetData(State.EditData.Keyword, out var prevData)
              ? State.EditData.Same(prevData) is false // 기존 데이터가 존재하지 않거나, 변경된 경우에만 저장 가능
              : true
            : false;
          bool trySave = GUILayout.Button("Save", GUILayout.Width(50));
          GUI.enabled = true;
          if (trySave && EditorUtility.DisplayDialog("Save Changes", "Are you sure to save the changes?", "Yes", "No")) {
            if (State.KeywordMenu != State.EditData.Keyword && Data.Add(State.EditData.Keyword, State.EditData)) {
              Debug.Log("Keyword Changed");
              Data.Remove(State.KeywordMenu);
              State.KeywordMenu = State.EditData.Keyword;
            }
            else Data.Replace(State.KeywordMenu, State.EditData);

            UpdateDisplayList();
          }
          GUI.enabled = State.EditData != null;
          if (GUILayout.Button("Cancel", GUILayout.Width(50))) State.EditData = Data.GetData(State.KeywordMenu);
          GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
      }
      #endregion
    }


    #endregion

    #region Misc
    /// <summary>
    /// Get Available Keywords that are not used in the data.<br/>
    /// Excludes <see cref="Keyword.None"/>  but include current selected keyword.
    /// (So if current is <see cref="Keyword.None"/> , it will be included)
    /// </summary>
    IEnumerable<Keyword> GetAvailableKeywords(Keyword current = Keyword.None)
      => from Keyword key in Enum.GetValues(typeof(Keyword))
         where key == current || (key > 0 && Data.TryGetData(key, out _) is false)
         select key;

    #endregion
  }
}