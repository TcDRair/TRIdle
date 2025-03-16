using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

namespace TRIdle.Game.UI
{
  using Skill;
  using TRIdle.Logics.Extensions;

  public partial class UI_MainSceneController
  {
    [Serializable]
    public class UI_Main_MainArea : IMainUIContent
    {
      public TextMeshProUGUI SkillDescription;

      public GameObject ActionElementPrefab;
      public RectTransform ActionPanel;
      public GameObject ActionPopupWindow;
      public TextMeshProUGUI ActionPopupDescription;

      private SkillBase m_skill;
      private ActionBase m_FocusedAction;

      private readonly Dictionary<ActionBase, UI_ActionElement> m_ActionElements = new();

      private void AddElement(ActionBase action) {
        var actionUI = Instantiate(ActionElementPrefab).GetComponent<UI_ActionElement>();
        actionUI.transform.SetParent(ActionPanel, false);
        actionUI.Initialize(action);
        m_ActionElements.Add(action, actionUI);
      }
      private void RemoveElement(ActionBase action) {
        Destroy(m_ActionElements[action].gameObject);
        m_ActionElements.Remove(action);
      }



      public void Update() {
        foreach (var action in m_ActionElements.Keys.Except(m_skill.Actions))
          RemoveElement(action);
        foreach (var action in m_skill.Actions.Intersect(m_ActionElements.Keys))
          m_ActionElements[action].Refresh();
        foreach (var action in m_skill.Actions.Except(m_ActionElements.Keys))
          AddElement(action);
        if (m_FocusedAction is not null)
          ActionPopupDescription.text = m_FocusedAction.DescriptionInfo;
      }
      public void Refresh(SkillBase skill) {
        if (m_skill == skill) { Update(); return; }

        m_skill = skill;
        foreach (var action in m_ActionElements.Keys.ToArray())
          RemoveElement(action);
        foreach (var action in m_skill.Actions)
          AddElement(action);
        m_FocusedAction = null;
      }

      public void Focus(ActionBase action) {
        if (m_FocusedAction == action)
          return;
        this.Log($"Now focusing {action.Name}.");
        m_FocusedAction = action;
        ActionPopupDescription.text = action.DescriptionInfo;
      }
      public void ActionPopup(bool enable) {
        ActionPopupWindow.SetActive(enable);
        var pos = ActionPopupWindow.transform.position;
        ActionPopupWindow.transform.position = new Vector3(pos.x, m_ActionElements[m_FocusedAction].transform.position.y, pos.z);
      }
    }
  }
}