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

  public interface IMainUIContent
  {
    void Update();
    void Refresh(SkillBase skill); // TODO : increase range with interface later
  }

  public partial class UI_MainSceneController : UIPanelSingleton<UI_MainSceneController>
  {
    [SerializeField] private UI_Main_SideMenu m_SideMenu;
    [SerializeField] private UI_Main_TopMenu m_TopMenu;
    [SerializeField] private UI_Main_MainArea m_MainArea;

    private SkillBase m_focusedSkill;

    #region Menu Elements
    public void Menu_AddElement(SkillBase skill) => m_SideMenu.AddMenu(skill);

    public void Menu_Focus(SkillBase skill) {
      if (m_focusedSkill == skill) { Menu_Update(); return; }
      // todo : null skill means (frontpage) dashboard. draw it instead

      this.Log($"Focused on {skill.Name}");
      m_focusedSkill = skill;

      m_SideMenu.Refresh(skill);
      m_TopMenu.Refresh(skill);
      m_MainArea.Refresh(skill);
    }

    public void Menu_EnablePopup(bool enable)
      => m_TopMenu.MenuPopupWindow.SetActive(enable);

    public void Menu_Update() {
      m_SideMenu.Update();
      m_TopMenu.Update();
      m_MainArea.Update();
    }
    #endregion

    public void Action_Focus(ActionBase action)
      => m_MainArea.Focus(action);

    public void Action_Popup(bool enable)
      => m_MainArea.ActionPopup(enable);
  }
}