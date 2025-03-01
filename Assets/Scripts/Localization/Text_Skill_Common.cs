namespace TRIdle.Localization
{
  public record Text_Skill_Common
  {
    public string Skill_Common_Name { get; set; } = "공통";
    public string Skill_Common_Description { get; set; } = "공통 기술의 설명입니다.";

    public string Effect_Internalization_Name { get; set; } = "체득";
    public string Effect_Internalization_Description { get; set; } = "몸이 기억하는 행위는 쉽게 잊히지 않는다.\n장시간의 수련을 통해 단순 반복 작업의 효율을 상승시킨다.";
  }
}