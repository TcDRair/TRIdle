using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TRIdle.Game
{
  using System;

  using Skill.Action;

  public class ProgressButton : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    [SerializeField] private Image image, progress;

    public TextMeshProUGUI Text => text;
    public Button Button => button;
    public Image Image => image;
    public Image ProgressBar => progress;
    event Action<ProgressButton> OnStart;

    public ActionBase Action { get; private set; }
    public bool OnGoing { get; private set; }
    public float Progress => progressValue / progressDuration;
    float progressValue, progressDuration;


    public void SetAction(ActionBase action, params Action<ProgressButton>[] startActions)
    {
      progressValue = 0;
      progressDuration = action.Duration;
      Action = action;
      text.text = action.Name;
      foreach (var a in startActions) OnStart += a;
    }

    public void Toggle() => Toggle(!OnGoing);
    public void Toggle(bool value) {
      if (OnGoing = value) {
        if (Action.CanPerform()) OnStart?.Invoke(this);
      } else if (Action.Pausable is false) progressValue = 0;
    }

    void Update()
    {
      button.interactable = Action.CanPerform();

      if (Action is not null && OnGoing)
      {

        progressValue += Time.deltaTime;
        if (Progress >= 1)
        {
          Action.OnPerform?.Invoke();
          progressValue = 0;

          if (!Action.Repeatable || !Action.CanPerform())
          {
            // TODO : Inform Skill that the task is completed / interrupted.
            OnGoing = false;
          }
        }
      }

      progress.fillAmount = Progress;
    }
  }
}
