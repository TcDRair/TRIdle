using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game.UI
{
  using Skill;

  [RequireComponent(typeof(Button))]
  public class UI_MenuElement : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _Text; // SubText 
    private SkillBase _Skill;

    public void Initialize(SkillBase skill) {
      if (skill is null) throw new System.NullReferenceException("Null Skill has been passed to the menu element");
      _Skill = skill;
      _Text.text = skill.Name;
    }

    public void Refresh() {
      _Text.text = _Skill.Name;
    }

    public void OnClick()
      => UI_MainSceneController.Instance.Menu_Focus(_Skill);
  }
}