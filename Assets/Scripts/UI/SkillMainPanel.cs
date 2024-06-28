using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System.Linq;

namespace TRIdle.Game.Skill
{

  public class SkillMainPanel : MonoBehaviour
  {
    public static SkillMainPanel Panel { get; private set; }


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
      public TextMeshProUGUI SkillDescription;
      public TextMeshProUGUI ActionDescription;
      public TextMeshProUGUI ActionRequiredKeyword;
    }
    [SerializeField] Main main;
    #endregion

    private SkillBase currentSkill;
    private ActionBase current;

    void Awake()
    {
      if (Panel != null)
      {
        Debug.LogAssertion($"Multiple panel({nameof(SkillMainPanel)}) activation is invalid.");
        Destroy(this);
      }
      else Panel = this;
    }

    void Update()
    {
      UpdateMainPanel();
    }

    public void ToggleDetailButton()
    {
      // TODO : Toggle Canvas Group of Detail Panel
    }

    public void DrawSkill(SkillBase skill)
    {
      if (skill == currentSkill) return;
      currentSkill = skill;
      title.Label.text = skill.Name;
      title.Icon.sprite = skill.Icon;

      main.ButtonPanel.DestroyAllChildren();
      foreach (var a in skill.Actions)
      {
        a.Callbacks.OnStart += () => ChangeCurrentAction(a);
        var b = Instantiate(main.ActionButton, main.ButtonPanel).GetComponent<ActionButton>();
        b.Action = a;
      }
    }
    void UpdateMainPanel()
    {
      if (currentSkill == null)
        main.Proficiency.text = main.SkillDescription.text = main.ActionDescription.text = main.ActionRequiredKeyword.text = "";
      else
      {
        main.Proficiency.text = $"Proficiency : {currentSkill.Proficiency}";
        main.SkillDescription.text = currentSkill.Description;
        main.ActionDescription.text = (current == null) ? "" : current.Description;
        main.ActionRequiredKeyword.text = (current == null) ? ""
          : string.Join('\n', current.RequiredKnowledge.Select(x => x.GetKnowledgeInfo().ToString()));
      }
    }

    void ChangeCurrentAction(ActionBase action)
    {
      if (action == current) return;
      current?.Callbacks.End();
      current = action;
    }
  }
}