namespace TRIdle.Game.Skill
{
  using Logics.Extensions;

  public class Action_WildCrafting_Search : ActionBase<Action_WildCrafting_Search>
  {
    public override string Name => Texts.Action_Wildcrafting_Search_Name;
    public override float Delay => 1.0f;
    public override string DescriptionInfo => Texts.Action_Wildcrafting_Search_DescriptionInfo;
    public override string DetailedInfo => string.Format(Texts.Action_Wildcrafting_Search_DetailedInfo, 10 * Fx, 20 * Fx, 30 * Fx);
    private float Fx => UnityEngine.Mathf.Log10(Proficiency + 1) + 1;

    protected override void OnActivated() {
      // 여기에 탐색 액션의 로직을 작성하자.
      Proficiency += 1;
      this.Log($"Action has been activated.");
    }
  }
}