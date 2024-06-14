using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TRIdle.Game
{
  using Skill.Action;

  public class ProgressButton : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    [SerializeField] private Image image, progress;

    public TextMeshProUGUI Text => text;
    public Button Button => button;
    public Image Image => image;
    public Image ProgressBar => progress;

    public float Progress => progressValue / progressDuration;
    float progressValue, progressDuration;
    ActionBase current;
    public void StartProgress(ActionBase action) {
      progressValue = 0;
      progressDuration =action.Duration;
      current = action;
    }
    public void StopProgress() {
      progressValue = 0;
      progress.fillAmount = 0;
      current = null;
    }

    void Update() {
      if (current != null) {
        progressValue += Time.deltaTime;
        if ((progress.fillAmount = Progress) >= 1) {
          current.OnPerform?.Invoke();
          progressValue = 0;
          if (current.Repeatable is false) current = null;
        }
      }
    }
  }
}
