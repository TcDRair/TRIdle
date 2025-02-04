

using TRIdle.Texts;

namespace TRIdle
{
  /// <summary>
  /// Text data for localization. Deserialized from json file in StreamingAssets.<br/>
  /// Main purpose is to implement localization languages which (mainly) isn't supported by developers.<br/>
  /// Default language is Korean.
  /// </summary>
  public record Text
  {
    public string Language { get; set; } = "한국어";
    public Settings Settings { get; set; } = new();
    public Menu Menu { get; set; } = new();
  }

  namespace Texts
  {
    public record Settings
    {
      public string Settings_SelectedLanguage { get; set; } = "ko-KR";
    }

    public record Menu
    {
      public string Menu_StartGameButton { get; set; } = "게임 시작";

    }
  }
}