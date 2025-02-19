using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

namespace TRIdle
{
  public class GlobalConstants
  {
    public const string PLACEHOLDER = "PlaceHolder";
    public const string SPRITE_PLACEHOLDER_PATH = "Sprites/PlaceHolder";
    public static readonly JsonSerializerOptions JsonSerializerOption = new() {
      WriteIndented = true,
      IgnoreReadOnlyProperties = true,
      ReferenceHandler = ReferenceHandler.IgnoreCycles,
      NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
      Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };
  }
}

// Force allow init properties in C# 9.0
namespace System.Runtime.CompilerServices { public class IsExternalInit { } }