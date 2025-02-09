using System;

using UnityEngine;

using TMPro;
using UnityEngine.UI;


namespace TRIdle.Game.UI
{
  public class MainUIController : UIPanelSingleton<MainUIController>
  {
    [SerializeField] private UIElements m_UIElements;



    #region UI Elements Definition
    [Serializable]
    class UIElements
    {
      [Serializable] public class SideMenuElements
      {
        public GameObject MenuElement;
        public RectTransform MenuPanel; // contains the menu elements
      }
      [Serializable] public class TopMenuElements
      {
        public TextMeshProUGUI Title;
        public Button DetailButton;
      }
      [Serializable] public class MainAreaElements
      {
        public GameObject ActionElement;
        public RectTransform ActionPanel; // contains the action elements
        public TextMeshProUGUI ActionDescription;
      }

      public SideMenuElements SideMenu;
      public TopMenuElements TopMenu;
      public MainAreaElements MainArea;
    }
    #endregion
  }
}