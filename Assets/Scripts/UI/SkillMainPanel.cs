using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game.Skill
{
  using Action;
  using UnityEditor.Experimental.GraphView;

  public class SkillMainPanel : MonoBehaviour
  {
    public static SkillMainPanel Panel { get; private set; }

    void Awake() {
      if (Panel != null) {
        Debug.LogAssertion($"Multiple panel({nameof(SkillMainPanel)}) activation is invalid.");
        Destroy(this);
      } else Panel = this;
    }

    void Update() {
      if (currentSkill != null) {
        main.Proficiency.text = $"Proficiency : {currentSkill.Proficiency}";
      }
    }

    #region Inspector
    [Serializable]
    class Title
    {
      public TextMeshProUGUI Label;
      public Image Icon;
      public Button DetailButton;

      //TODO : 
    }
    [SerializeField] Title title;

    [Serializable]
    class Main
    {
      public RectTransform ButtonPanel;
      public GameObject ActionButton;
      public TextMeshProUGUI Proficiency;
      public TextMeshProUGUI Description;
    }
    [SerializeField] Main main;
    #endregion

    private SkillBase currentSkill;
    private ProgressButton current;

    public void ToggleDetailButton() {
      // TODO : Toggle Canvas Group of Detail Panel
    }

    public void DrawSkill(SkillBase skill) {
      if (skill == currentSkill) return;
      currentSkill = skill;
      title.Label.text = skill.Name;
      title.Icon.sprite = skill.Icon;

      main.ButtonPanel.DestroyAllChildren();
      foreach (var a in skill.Actions) {
        var b = Instantiate(main.ActionButton, main.ButtonPanel).GetComponent<ProgressButton>();
        b.SetAction(a, b => ChangeCurrentAction(b));
      }
    }

    void ChangeCurrentAction(ProgressButton button) {
      if (button == current) return;

      if (current != null) current.Toggle();
      current = button;
      main.Description.text = button.Action.Description;
    }
  }
}