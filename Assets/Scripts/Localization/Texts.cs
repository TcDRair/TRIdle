

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
    }
  }
}