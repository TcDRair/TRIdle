using UnityEngine;

namespace TRIdle.Logics.Extensions
{
  public static class LoggerExtension
  {
    /// <summary>Print Debug.Log with its class name as tag. Colorized by hash code.</summary>
    public static void Log(this object obj, string message) {
      var tag = $"[{obj.GetType().Name}]";
      Debug.Log($"{ColorByString(tag)} {message}");
    }
    public static void Log(this object obj, string message, Object context) {
      var tag = $"[{obj.GetType().Name}]";
      Debug.Log($"{ColorByString(tag)} {message}", context);
    }

    /// <summary>Print Debug.LogWarning with its class name as tag. Colorized by hash code.</summary>
    public static void LogWarning(this object obj, string message) {
      var tag = $"[{obj.GetType().Name}]";
      Debug.LogWarning($"{ColorByString(tag)} {message}");
    }
    public static void LogWarning(this object obj, string message, Object context) {
      var tag = $"[{obj.GetType().Name}]";
      Debug.LogWarning($"{ColorByString(tag)} {message}", context);
    }

    /// <summary>Print Debug.LogError with its class name as tag. Colorized by hash code.</summary>
    public static void LogError(this object obj, string message) {
      var tag = $"[{obj.GetType().Name}]";
      Debug.LogError($"{ColorByString(tag)} {message}");
    }
    public static void LogError(this object obj, string message, Object context) {
      var tag = $"[{obj.GetType().Name}]";
      Debug.LogError($"{ColorByString(tag)} {message}", context);
    }

    private static string ColorByString(string text) {
      float hashHue = $"%{text}%".GetHashCode() % 180 + 180; // Hash to Hue
      var color = Color.HSVToRGB(hashHue / 360f, 1, 1); // Hue to RGB
      return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
    }
  }
}