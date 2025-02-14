

using TRIdle.Texts;

namespace TRIdle
{
  /// <summary>
  /// Text data for localization. Deserialized from json file in StreamingAssets.<br/>
  /// Main purpose is to implement localization languages which (mainly) isn't supported by developers.<br/>
  /// Default language is Korean. <see cref="Current"/> is the current language data.
  /// </summary>
  public record Text
  {
    public Title Title { get; set; } = new();
    public Settings Settings { get; set; } = new();
    public Skill Skill { get; set; } = new();

    public static Text Current { get; set; } = new();
  }

  namespace Texts
  {
    public record Title
    {
      public string Title_StartGameButton { get; set; } = "게임 시작";
    }

    public record Settings
    {
      public string Settings_Title { get; set; } = "설정";
      public string Settings_LanguageSelection { get; set; } = "언어";
    }

    public record Skill
    {
      public string Skill_Wildcrafting_Name { get; set; } = "생존 기술";
      public string Action_Wildcrafting_Search_Name { get; set; } = "탐색";
      public string Action_Wildcrafting_Search_DescriptionInfo { get; set; } = "주변 환경을 탐색하여 정보를 얻습니다. 다양한 자원, 식생, 위협 요소 등을 발견할 수 있습니다.";
      public string Action_Wildcrafting_Search_DetailedInfo { get; set; } = "신규 자원 발견 확률: {0}%\n신규 식생 발견 확률: {1}%\n위협 요소 발견 확률: {2}%";
    }
  }
}