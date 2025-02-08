using UnityEngine;

namespace TRIdle.Logics.Extensions
{
  public static class LoggerExtension
  {
    /// <summary>Print Debug.Log with its class name as tag. Colorized by hash code.</summary>
    public static void Log(this object obj, string message) 
      => Debug.Log($"{ColorByString(obj.NameWithBrackets())} {message}");
    /// <summary>Print Debug.Log with its class name as tag. Colorized by hash code.</summary>
    public static void Log(this object obj, string message, Object context)
      => Debug.Log($"{ColorByString(obj.NameWithBrackets())} {message}", context);

    /// <summary>Print Debug.LogWarning with its class name as tag. Colorized by hash code.</summary>
    public static void LogWarning(this object obj, string message)
      => Debug.LogWarning($"{ColorByString(obj.NameWithBrackets())} {message}");
    /// <summary>Print Debug.LogWarning with its class name as tag. Colorized by hash code.</summary>
    public static void LogWarning(this object obj, string message, Object context) 
      => Debug.LogWarning($"{ColorByString(obj.NameWithBrackets())} {message}", context);

    /// <summary>Print Debug.LogError with its class name as tag. Colorized by hash code.</summary>
    public static void LogError(this object obj, string message) 
      => Debug.LogError($"{ColorByString(obj.NameWithBrackets())} {message}");
    /// <summary>Print Debug.LogError with its class name as tag. Colorized by hash code.</summary>
    public static void LogError(this object obj, string message, Object context)
      => Debug.LogError($"{ColorByString(obj.NameWithBrackets())} {message}", context);

    /// <summary>Print Debug.LogAssertion with its class name as tag. Colorized by hash code.</summary>
    public static void LogAssertion(this object obj, string message)
      => Debug.LogAssertion($"{ColorByString(obj.NameWithBrackets())} {message}");

    public static void LogAssertion(this object obj, string message, Object context)
      => Debug.LogAssertion($"{ColorByString(obj.NameWithBrackets())} {message}", context);

    private static string NameWithBrackets(this object obj) => $"[{obj.GetType().Name}]";
    private static string ColorByString(string text) {
      float hashHue = $"%{text}%".GetHashCode() % 180 + 180; // Hash to Hue
      var color = Color.HSVToRGB(hashHue / 360f, 1, 1); // Hue to RGB
      return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
    }
  }
}