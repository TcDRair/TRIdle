using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace TRIdle.Game.Skill
{
  public class SkillCategoryPanel : MonoBehaviour
  {
    public static SkillCategoryPanel Panel { get; private set; }
    [SerializeField] RectTransform Content;
    [SerializeField] GameObject SkillUIPanel;

    void Awake() {
      if (Panel != null) {
        Debug.LogAssertion($"Multiple panel({nameof(SkillCategoryPanel)}) activation is invalid.");
        Destroy(this);
      } else Panel = this;

      //DEBUG
      if (Player.IsLoaded is false) Player.Load();
      Initialize(Player.AllSkills);
    }

    //! DEBUG
    void OnDestroy() {
      Player.Save();
    }

    public void Initialize(IEnumerable<SkillBase> skills) {// TODO : Fetch SkillCategory data
      foreach (var skill in skills) {
        var ui = Instantiate(SkillUIPanel, Content).GetComponent<SkillUI>();
        ui.Initialize(skill);
        // Need to save reference?
      }
    }
  }
}