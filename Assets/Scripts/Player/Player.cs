using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using UnityEngine;

namespace TRIdle.Game
{
  using System.Text.Json.Serialization;
  using Skill;

  public static class Player
  {
    public static bool IsLoaded { get; private set; } = false;

    public static class Serializer
    {
      static string FilePath => Application.persistentDataPath + "/player.json";
      static readonly JsonSerializerOptions JSOptions = new()
      {
        WriteIndented = true,
        IgnoreReadOnlyProperties = true,
        ReferenceHandler = ReferenceHandler.Preserve,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
      };

      public static void Save()
      {
        if (!IsLoaded) return;
        JsonSerializer.Serialize(new FileStream(FilePath, FileMode.Create), Skill.All, JSOptions);
        State.LastSave = DateTime.Now;
      }
      public static void Load()
      {
        // Load player data
        try
        {
          _skills = JsonSerializer.Deserialize<SkillBase[]>(
            new FileStream(FilePath, FileMode.Open), JSOptions
          ) ?? throw new Exception();
        }
        // If failed to load, load default data
        catch
        {
          Debug.LogError("Failed to load player data. Loading default data.");

          _skills = new SkillBase[]
          {
            new SB_WoodCutting(),
          };
          foreach (var skill in _skills)
            skill.Initialize();
        }
        // Setup references and finish loading
        finally {
          foreach (var skill in _skills)
            skill.SetupReference();
          IsLoaded = true;
        }
      }
    }

    public static class State
    {
      public static DateTime LastSave { get; set; } = DateTime.MinValue;
      public static SkillBase FocusSkill { get; set; }
      public static ActionBase FocusAction { get; set; }
    }

    public static class Statistic
    {

    }

    private static SkillBase[] _skills;
    public static class Skill
    {
      public static SkillBase[] All => _skills;
      public static T GetSkill<T>() where T : SkillBase => All.FirstOrDefault(s => s is T) as T;
    }
  }
}