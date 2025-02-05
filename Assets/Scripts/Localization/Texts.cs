

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
    public Settings Settings { get; set; } = new();
    public Menu Menu { get; set; } = new();

    public static Text Current { get; private set; } = new();
  }

  namespace Texts
  {
    public record Settings
    {
      public string Settings_LanguageSelection { get; set; } = "언어";
    }

    public record Menu
    {
      public string Menu_StartGameButton { get; set; } = "게임 시작";
    }
  }
}