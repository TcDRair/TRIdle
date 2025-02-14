using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game.UI
{
  using Skill;

  /// <summary>Default UI element for action. Derive this class to create custom Action UI.</summary>
  public class UI_ActionElement : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private Image m_progress;

    private ActionBase m_action;
    public void Initialize(ActionBase action) {
      m_action = action;
      m_text.text = action.Name;
    }

    public void OnClick() => Player.Instance.FocusAction(m_action);

    protected virtual void Update() {
      if (m_action is null) return;

      m_progress.fillAmount = m_action.Progress;
    }
  }
}