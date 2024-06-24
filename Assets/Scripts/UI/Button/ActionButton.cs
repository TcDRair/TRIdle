using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game
{
  using Skill;

  public class ActionButton : MonoBehaviour
  {
    #region Inspector
    [SerializeField] private ActionButton_Ref references;

    [Header("Main")]
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button button;
    [SerializeField] private Image image, progress;

    [Header("Stack")]
    [SerializeField] private TextMeshProUGUI stack;
    #endregion

    public ActionBase Action { get; set; }

    #region Indicator
    public float Progress => Action.CurrentProgress / Action.Duration;
    #endregion

    #region Control Methods
    public void Toggle() // Assigned to Button.onClick
    {
      if (Action.IsPerforming) Action.Callbacks.End();
      else Action.Callbacks.Start();
    }
    #endregion

    #region Update Methods
    void Update()
    {
      Update_Progress();

      Invoke_Callbacks();

      Update_Display();
    }

    void Update_Progress()
    {
      if (Performable is false) return;

      if (Action.IsPerforming) Action.Callbacks.Progress(Time.deltaTime);
    }
    void Invoke_Callbacks()
    {
      if (Progress >= 1) {
        Action.Callbacks.Perform();
      }
    }
    void Update_Display()
    {
      button.interactable = Action.CanPerform;
      label.text = Action.Name;
      stack.text = Action.StackInfo;
      progress.fillAmount = Progress;
    }
    #endregion

    #region Internal Utility
    bool Performable => Action is not null && Action.CanPerform;
    bool Repetable => Performable && Action.Repeatable;
    #endregion
  }
}
