using UnityEngine;

namespace TRIdle.Game
{
  public class ResponsiveCanvas : MonoBehaviour
  {
    public Canvas Canvas;
    public GameObject WideMenu, NarrowMenu, MenuButton;

    bool m_responsive;
    void Update() {
      m_responsive = Canvas.renderingDisplaySize.x <= 900;
      WideMenu.SetActive(!m_responsive);
      MenuButton.SetActive(m_responsive);
      if (!m_responsive) NarrowMenu.SetActive(false);
    }

    public void OnMenuButtonClicked() => NarrowMenu.SetActive(true);
    public void OnNarrowMenuButtonClicked() => NarrowMenu.SetActive(false);
  }
}
