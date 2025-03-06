using System;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.UI
{
  using Skill;

  public partial class UI_MainSceneController
  {
    [Serializable]
    private class UI_Main_SideMenu : IMainUIContent
    {
      public GameObject MenuElementPrefab;
      public RectTransform MenuPanel;

      private readonly Dictionary<SkillBase, UI_MenuElement> m_MenuElements = new();

      public void AddMenu(SkillBase skill) {
        var element = Instantiate(MenuElementPrefab).GetComponent<UI_MenuElement>();
        element.transform.SetParent(MenuPanel, false);
        element.Initialize(skill);
        m_MenuElements.Add(skill, element);
      }
      //todo public void AddMenu(OtherMenuObject something) { }
      // such as settings, etc.

      public void Update() {
        foreach (var element in m_MenuElements)
          element.Value.Refresh();
      }

      public void Refresh(SkillBase skill) {
        //TODO : m_MenuElements[skill].Highlight();
        Update();
      }
    }
  }
}