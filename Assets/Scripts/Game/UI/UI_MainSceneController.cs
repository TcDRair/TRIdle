using System;
using System.Collections.Generic;

using UnityEngine;

using TMPro;
using UnityEngine.UI;


namespace TRIdle.Game.UI
{
  using Skill;
  using Logics.Extensions;

  public class UI_MainSceneController : UIPanelSingleton<UI_MainSceneController>
  {
    [SerializeField] private UIElements m_UIElements;
    
    private readonly Dictionary<UI_MenuElement, SkillBase> m_MenuElements = new();
    private readonly Dictionary<ActionBase, UI_ActionElement> m_ActionElements = new();
    
    public void AddMenu(SkillBase skill) {
      var element = Instantiate(m_UIElements.SideMenu.MenuElement).GetComponent<UI_MenuElement>();
      element.transform.SetParent(m_UIElements.SideMenu.MenuPanel, false);
      element.Initialize(skill);
      m_MenuElements.Add(element, skill);
    }
    public void FocusMenu(UI_MenuElement element) {
      // Do sth
      if (m_MenuElements.TryGetValue(element, out var skill) is false) {
        this.LogError($"No skill found for {element}");
        return;
      }

      SetActions(skill);
      this.Log($"Focused on {skill.Name}");

      void SetActions(SkillBase skill) {
        ClearActions();
        foreach (var action in skill.Actions) {
          var actionUI = Instantiate(m_UIElements.MainArea.ActionElement).GetComponent<UI_ActionElement>();
          actionUI.transform.SetParent(m_UIElements.MainArea.ActionPanel, false);
          actionUI.Initialize(action);
          m_ActionElements.Add(action, actionUI);
        }
      }
      void ClearActions() {
        foreach (var actionUI in m_ActionElements.Values)
          Destroy(actionUI.gameObject);
        m_ActionElements.Clear();
      }
    }
    public void FocusAction(ActionBase action) {
      m_UIElements.MainArea.ActionDescription.text = $"{action.DescriptionInfo}\n{action.DetailedInfo}";
    }


    public void UpdateElements() {
      // Invoked when some action is done

      // Update Side Menu
      // Update Top Menu
      // Update MainArea
      if (Player.Instance.Data.CurrentAction is ActionBase action)
        FocusAction(action);
    }

    #region UI Elements Definition
    [Serializable]
    class UIElements
    {
      [Serializable] public class SideMenuElements
      {
        public GameObject MenuElement;
        public RectTransform MenuPanel; // contains the menu elements
      }
      [Serializable] public class TopMenuElements
      {
        public TextMeshProUGUI Title;
        public Button DetailButton;
      }
      [Serializable] public class MainAreaElements
      {
        public GameObject ActionElement;
        public RectTransform ActionPanel; // contains the action elements
        public TextMeshProUGUI ActionDescription;
      }

      public SideMenuElements SideMenu;
      public TopMenuElements TopMenu;
      public MainAreaElements MainArea;
    }
    #endregion
  }
}