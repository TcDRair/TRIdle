using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game.Skill
{
  using Action;

  public class SkillMainPanel : MonoBehaviour
  {
    public static SkillMainPanel Panel { get; private set; }

    void Awake() {
      if (Panel != null) {
        Debug.LogAssertion($"Multiple panel({nameof(SkillMainPanel)}) activation is invalid.");
        Destroy(this);
      } else Panel = this;
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

    public void ToggleDetailButton() {
      // TODO : Toggle Canvas Group of Detail Panel
    }

    public void DrawSkill(SkillBase skill) {
      if (skill == currentSkill) return;

      currentSkill = skill;
      title.Label.text = skill.Name;
      title.Icon.sprite = skill.Icon;

      foreach (var a in skill.Actions) {
        var p = Instantiate(main.ActionButton, main.ButtonPanel).GetComponent<ProgressButton>();
        p.Button.onClick.AddListener(() => DrawDescription(a));
      }
    }

    void DrawDescription(ActionBase action) {
      main.Description.text = action.Description;
    }
  }
}