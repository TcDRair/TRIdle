namespace TRIdle.Localization
{
  public record Text_Skill_Wildcrafting
  {
    public string Skill_Wildcrafting_Name { get; set; } = "생존 기술";
    public string Skill_Wildcrafting_Description { get; set; } = "자연 환경에서 생존하기 위한 기술군입니다.\n - 누적 숙련도 {0}\n - custom description here";
    public string Action_Wildcrafting_Search_Name { get; set; } = "탐색";
    public string Action_Wildcrafting_Search_DescriptionInfo { get; set; } = "주변 환경을 탐색하여 정보를 얻습니다. 다양한 자원, 식생, 위협 요소 등을 발견할 수 있습니다.";
    public string Action_Wildcrafting_Search_DetailedInfo { get; set; } = "숙달을 통해 탐색에 걸리는 시간이 {0:F1}% 감소합니다.";
  }
}