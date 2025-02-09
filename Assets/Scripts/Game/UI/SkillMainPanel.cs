using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game.Skill
{
  using UI;
  using Skill;

  public class SkillMainPanel : UIPanelSingleton<SkillMainPanel>
  {
    // There will be a lot of UI Components, so use class to group them
    [SerializeField] private MainPanelComponents Components;

    public SkillBase FocusedSkill { get; private set; }
    public ActionBase FocusedAction { get; private set; }

    void Start() {
      DrawSkill(Skills.Wildcrafting);
    }

    public void ToggleDetailButton() {
      // TODO : Toggle Canvas Group of Detail Panel
    }

    public void DrawSkill(SkillBase skill) {
      FocusedSkill = skill;

      Components.Name.text = skill.Name;
      // Components.Level.text = $"Level {skill.Level}";
      // Components.Description.text = skill..Description;
      // Components.Icon.sprite = skill.Icon;
    }

    [Serializable]
    private class MainPanelComponents {
      public TextMeshProUGUI Name, Level, Description;
      public Image Icon;
      public Button Button;
    }
  }
}