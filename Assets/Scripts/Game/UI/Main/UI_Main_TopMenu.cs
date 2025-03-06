using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game.UI
{
  using Skill;

  public partial class UI_MainSceneController
  {
    [Serializable]
    public class UI_Main_TopMenu : IMainUIContent
    {
      public TextMeshProUGUI Title;

      public GameObject MenuPopupWindow;
      public TextMeshProUGUI MenuPopupDescription;
      public Button PopupButton;

      public void Update() {

      }
      public void Refresh(SkillBase skill) {
        Title.text = skill.Name;
        MenuPopupDescription.text = skill.Description;
      }
    }
  }
}