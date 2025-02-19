namespace TRIdle.Game.Skill
{
  using Logics.Math;
  using Logics.Extensions;

  public class Action_Wildcrafting_Search : ActionBase<Action_Wildcrafting_Search>
  {
    public override string Name => Text.Action_Wildcrafting_Search_Name;
    public override string DescriptionInfo => Text.Action_Wildcrafting_Search_DescriptionInfo;
    public override string DetailedInfo => string.Format(Text.Action_Wildcrafting_Search_DetailedInfo, 10 * Fx, 20 * Fx, 30 * Fx);
   
    public override ValueData Data => new() {
      Duration = new(3)
    };
    private float Fx => UnityEngine.Mathf.Log10(Proficiency + 1) + 1;

    protected override void OnActivated() {
      // 여기에 탐색 액션의 로직을 작성하자.
      Proficiency += 1;
      this.Log($"Action has been activated.");
    }
  }
}