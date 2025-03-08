using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game.UI
{
  using Skill;
  using TRIdle.Logics.Extensions;

  /// <summary>Default UI element for action. Derive this class to create custom Action UI.</summary>
  public class UI_ActionElement : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private Image m_Progress;
    [SerializeField] private LongClickButton m_LongClickButton;

    private ActionBase m_action;
    public void Initialize(ActionBase action) {
      if (action == null) { this.LogError("Action element instantiated without a valid action."); return; }
      m_action = action;
      m_Text.text = action.Name;
    }
    public void Refresh() {
      m_Text.text = m_action.Name;
    }
    // TODO : divide focusing and activating action
    public void Activate() => Player.Instance.ActivateAction(m_action);
    public void Focus() => UI_MainSceneController.Instance.Action_Focus(m_action);
    public void ShowPopup() => UI_MainSceneController.Instance.Action_Popup(true);

    protected virtual void Update() {
      m_Progress.fillAmount = m_action.Progress;
    }
  }
}