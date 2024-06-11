using TMPro;

using UnityEngine;


namespace TRIdle.Game.Skill.Action
{
  public class ActionUI : MonoBehaviour
  {
    public ProgressButton RB_WoodCutting;
    public TextMeshProUGUI T_ExploreProficiency;

    float proficiency = 0;
    float m_cuttingProgress;
    bool m_cuttingActive;
    const float CUT_TIME = 5;

    protected void Start() {
      m_cuttingActive = true;
      RB_WCClicked(); // reset

      RB_WoodCutting.Button.onClick.AddListener(RB_WCClicked);
    }

    protected void Update() {
      RB_WoodCutting.Progress.fillAmount = m_cuttingProgress / CUT_TIME;
      if (m_cuttingActive && (m_cuttingProgress += Time.deltaTime) >= CUT_TIME)
        RB_ExploreCompleted();
    }


    public void RB_WCClicked() {
      m_cuttingProgress = 0;
      if (m_cuttingActive = !m_cuttingActive) {
        // start explore
        RB_WoodCutting.Text.text = "탐색 진행 중...";
        RB_WoodCutting.Image.color = Color.gray;
      } else {
        // cancel explore
        RB_WoodCutting.Text.text = "탐색 시작";
        RB_WoodCutting.Image.color = Color.white;
      }
    }

    public void RB_ExploreCompleted() {
      // add reward + restart explore
      proficiency += 1;
      T_ExploreProficiency.text = $"Prof: {proficiency}";
      m_cuttingProgress = 0;
    }
  }
}
