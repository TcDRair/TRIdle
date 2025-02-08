using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;

namespace TRIdle.Game.UI
{
  using Skill;

  public class SkillUI : MonoBehaviour
  {
    [Serializable]
    class CategoryPanel
    {
      public TextMeshProUGUI Name, Level;
      public Image Icon;
      public Button Button;
    }
    [SerializeField] private CategoryPanel Category;

    [Serializable]
    class MainPanel
    {
      public GameObject RButton;
      public RectTransform RButtonPanel;
    }
    [SerializeField] private MainPanel Main;

    public SkillBase Skill { get; private set; }

    public void Initialize(SkillBase skill) {
      Skill = skill;

      //TODO Init
      Category.Button.onClick.AddListener(() => SkillMainPanel.Panel.DrawSkill(Skill));

      Update(); // Invoke also at first frame
    }

    void Update() {

    }
  }
}