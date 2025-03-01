using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game.UI
{
  using Skill;

  [RequireComponent(typeof(Button))]
  public class UI_MenuElement : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI m_Text;
    private SkillBase m_skill;

    public void Initialize(SkillBase skill) {
      m_skill = skill;
      m_Text.text = skill.Name;
    }

    public void OnClick()
      => UI_MainSceneController.Instance.Menu_Focus(m_skill);
  }
}