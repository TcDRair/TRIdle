using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace TRIdle.Game
{
  using Skill.Action;

  public class ActionButton : MonoBehaviour
  {
    #region Inspector
    [SerializeField] private ProgressButton_Ref references;

    [Header("Main")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    [SerializeField] private Image image, progress;

    [Header("Stack")]
    [SerializeField] private TextMeshProUGUI stack;
    #endregion

    #region Callbacks
    public delegate void BCallback(ActionButton button);

    /// <summary>Invoked when the progress starts by any means.</summary>
    public event BCallback OnStart;
    /// <summary>Invoked when the progress is repeated.</summary>
    public event BCallback OnRepeat;
    /// <summary>Invoked when the progress is performed.</summary>
    public event BCallback OnPerform;
    /// <summary>Invoked when the progress ends by any means.</summary>
    public event Action<ActionButton> OnEnd;
    #endregion

    public ActionBase Action { get; private set; }

    #region Default Logics
    void Start()
    {
      OnPerform += (button) =>
      {
        button.progressValue = 0;
        if (button.Repetable) button.OnRepeat?.Invoke(button);
        else button.Off();
      };
    }
    #endregion

    #region Indicator
    public bool OnGoing { get; private set; }
    public float Progress => progressValue / progressDuration;
    float progressValue, progressDuration;
    #endregion

    #region Control Methods
    public void SetAction(ActionBase action)
    {
      progressValue = 0;
      progressDuration = action.Duration;
      Action = action;
      text.text = action.Name;
    }

    public void Toggle()
    {
      if (OnGoing) Off();
      else On();
    }
    public void On()
    {
      if (Action.CanPerform() is false) return;
      OnStart?.Invoke(this);
      OnGoing = true;
    }
    public void Off()
    {
      if (OnGoing is false) return;
      OnEnd?.Invoke(this);
      OnGoing = false;

      if (Action.Pausable is false) progressValue = 0;
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

      if (OnGoing) progressValue += Time.deltaTime;
    }
    void Invoke_Callbacks()
    {
      if (Progress >= 1) {
        OnPerform?.Invoke(this);
        Action?.OnPerform?.Invoke();
      }
    }
    void Update_Display()
    {
      button.interactable = Action.CanPerform();
      stack.text = Action.StackInfo();
      progress.fillAmount = Progress;
    }
    #endregion

    #region Internal Utility
    bool Performable => Action is not null && Action.CanPerform();
    bool Repetable => Performable && Action.Repeatable;
    #endregion
  }
}
