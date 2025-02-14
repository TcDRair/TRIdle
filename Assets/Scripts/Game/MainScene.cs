using UnityEngine;

namespace TRIdle.Game
{
  public class MainScene : MonoBehaviour
  {
    private void Start() {
      UI.UI_MainSceneController.Instance.AddMenu(Skill.Skills.Wildcrafting);
    }
  }
}