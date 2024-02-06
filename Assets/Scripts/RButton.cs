using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TRIdle.Game
{
  public class RButton : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    [SerializeField] private Image image, progress;

    public TextMeshProUGUI Text => text;
    public Button Button => button;
    public Image Image => image;
    public Image Progress => progress;
  }
}