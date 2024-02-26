using TMPro;

using UnityEngine;


namespace TRIdle.Game
{
  public class ActionUI : MonoBehaviour
  {
    public RButton RB_Explore;
    public TextMeshProUGUI T_ExploreProficiency;

    float m_exploreProgress;
    bool m_exploreActive;

    protected void Start() {
      m_exploreActive = true;
      RB_ExploreClicked(); // reset

      RB_Explore.Button.onClick.AddListener(RB_ExploreClicked);
    }

    protected void Update() {
      RB_Explore.Progress.fillAmount = m_exploreProgress / 10;
      if (m_exploreActive && (m_exploreProgress += Time.deltaTime) >= 10)
        RB_ExploreCompleted();
    }


    public void RB_ExploreClicked() {
      m_exploreProgress = 0;
      if (m_exploreActive = !m_exploreActive) {
        // start explore
        RB_Explore.Text.text = "Å½»ö Áß...";
        RB_Explore.Image.color = Color.gray;
      } else {
        // cancel explore
        RB_Explore.Text.text = "Å½»ö ½ÃÀÛ";
        RB_Explore.Image.color = Color.white;
      }
    }

    public void RB_ExploreCompleted() {
      // add reward + restart explore
      Storage.R.Professions.CommodityManagement.AddProficiency(1);
      T_ExploreProficiency.text = $"Prof: {Storage.R.Professions.CommodityManagement.Proficiency}";
      m_exploreProgress = 0;
    }
  }
}