using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game.Skill
{
  using Logics.Extensions;
  using TRIdle.Game.UI;

  public class SkillMainPanel : UIPanelSingleton<SkillMainPanel>
  {
    // There will be a lot of UI Components, so use class to group them
    [SerializeField] private MainPanelComponents Components;

    public SkillBase FocusedSkill { get; private set; }
    public ActionBase FocusedAction { get; private set; }

    void Update() {

    }

    public void ToggleDetailButton() {
      // TODO : Toggle Canvas Group of Detail Panel
    }

    public void DrawSkill(SkillBase skill) {

    }

    [Serializable]
    private class MainPanelComponents {
      public TextMeshProUGUI Name, Level, Description;
      public Image Icon;
      public Button Button;
    }
  }
}