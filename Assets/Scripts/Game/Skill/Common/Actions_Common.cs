namespace TRIdle.Game.Skill
{
  public class Action_Common_Learning : ActionBase<Action_Common_Learning>
  {
    public override string Name => Text.Current.Skill.Action_Common_Learning_Name;
    public override float Delay => 1.0f;
    public override string DescriptionInfo => Text.Current.Skill.Action_Common_Learning_DescriptionInfo;
    public override string DetailedInfo => string.Format(Text.Current.Skill.Action_Common_Learning_DetailedInfo, -1, -1);

    protected override void OnActivated() {
      // 여기에 체득 액션의 로직을 작성하자.
      // 아 맞다 이거 패시브지 어떡하지
    }
  }
}