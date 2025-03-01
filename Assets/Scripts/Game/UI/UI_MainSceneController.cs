using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game.UI
{
  using Skill;
  using Logics.Extensions;

  public class UI_MainSceneController : UIPanelSingleton<UI_MainSceneController>
  {
    [SerializeField] private UIElements m_UIElements;

    private readonly Dictionary<SkillBase, UI_MenuElement> m_MenuElements = new();
    private readonly Dictionary<ActionBase, UI_ActionElement> m_ActionElements = new();

    #region Menu Elements
    public void Menu_AddElement(SkillBase skill) {
      var element = Instantiate(m_UIElements.SideMenu.MenuElementPrefab).GetComponent<UI_MenuElement>();
      element.transform.SetParent(m_UIElements.SideMenu.MenuPanel, false);
      element.Initialize(skill);
      m_MenuElements.Add(skill, element);
    }

    private SkillBase m_focusedSkill;
    public void Menu_Focus(SkillBase skill) {
      this.Log($"Focused on {skill.Name}");
      m_focusedSkill = skill;

      // Refresh Top Menu
      m_UIElements.TopMenu.Title.text = skill.Name;
      m_UIElements.TopMenu.MenuPopupDescription.text = skill.Description;

      // Refresh All Action Elements
      foreach (var actionUI in m_ActionElements.Values)
        Destroy(actionUI.gameObject);
      m_ActionElements.Clear();
      foreach (var action in skill.Actions) {
        var actionUI = Instantiate(m_UIElements.MainArea.ActionElementPrefab).GetComponent<UI_ActionElement>();
        actionUI.transform.SetParent(m_UIElements.MainArea.ActionPanel, false);
        actionUI.Initialize(action);
        m_ActionElements.Add(action, actionUI);
      }
    }

    public void Menu_EnablePopup(bool enable)
      => m_UIElements.TopMenu.MenuPopupWindow.SetActive(enable);
    #endregion

    #region Action Elements
    public void Action_Focus(ActionBase action) {
      // m_UIElements.MainArea.ActionPopupDescription.text = $"{action.DescriptionInfo}\n{action.DetailedInfo}";
    }
    public void Action_EnablePopup(bool enable)
      => m_UIElements.MainArea.ActionPopupWindow.gameObject.SetActive(enable);

    private bool Action_AddElement(ActionBase action) {
      if (m_ActionElements.ContainsKey(action)) return false;
      var actionUI = Instantiate(m_UIElements.MainArea.ActionElementPrefab).GetComponent<UI_ActionElement>();
      actionUI.transform.SetParent(m_UIElements.MainArea.ActionPanel, false);
      actionUI.Initialize(action);
      return m_ActionElements.TryAdd(action, actionUI);
    }
    private bool Action_RemoveElement(ActionBase action)
      => m_ActionElements.Remove(action);
    #endregion

    #region UI Callbacks
    public void UpdateElements() {
      // Invoked when some action is done

      // Update Side Menu
      //  -> Side Menu updates all existing elements always
      //     It refreshes only if specific method is called
      // Update Top Menu
      //  -> Top Menu updates its popup description only,
      //     It's refreshed if the skill is changed
      // Update Main Area
      //  -> Main Area updates its all contents(action elements, customs, etc)
      //     It's refreshed if the skill is changed

      var skill = m_focusedSkill;
      foreach (var action in skill.Actions.Except(m_ActionElements.Keys))
        Action_AddElement(action);
      foreach (var action in m_ActionElements.Keys.Except(skill.Actions))
        Action_RemoveElement(action);
    }
    #endregion

    #region Definitions
    [Serializable]
    class UIElements
    {
      [Serializable]
      public class SideMenuElements
      {
        public GameObject MenuElementPrefab;
        public RectTransform MenuPanel;
      }
      [Serializable]
      public class TopMenuElements
      {
        public TextMeshProUGUI Title;
        // Window : Scene object only

        public GameObject MenuPopupWindow;
        public TextMeshProUGUI MenuPopupDescription;
        public Button PopupButton;
      }
      [Serializable]
      public class MainAreaElements
      {
        public GameObject ActionElementPrefab;
        public RectTransform ActionPanel;
        public RectTransform ActionPopupWindow;
        public TextMeshProUGUI ActionPopupDescription;
      }

      public SideMenuElements SideMenu;
      public TopMenuElements TopMenu;
      public MainAreaElements MainArea;
    }
    #endregion
  }
}