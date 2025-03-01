namespace TRIdle
{
  using Localization;

  /// <summary>
  /// Text data for localization. Deserialized from json file in StreamingAssets.<br/>
  /// Main purpose is to implement localization languages which (mainly) isn't supported by developers.<br/>
  /// Default language is Korean. <see cref="Current"/> is the current language data.
  /// </summary>
  public record TextLocale
  {
    public Text_Title Title { get; set; } = new();
    public Text_Settings Settings { get; set; } = new();
    public Text_Skills Skills { get; set; } = new();

    public static TextLocale Current { get; set; } = new();
  }

  namespace Localization
  {
    public record Text_Title
    {
      public string Title_StartGameButton { get; set; } = "게임 시작";
    }

    public record Text_Settings
    {
      public string Settings_Title { get; set; } = "설정";
      public string Settings_LanguageSelection { get; set; } = "언어";
    }

    public record Text_Skills
    {
      public Text_Skill_Common Common { get; set; } = new();
      public Text_Skill_Wildcrafting Wildcrafting { get; set; } = new();
    }
  }
}