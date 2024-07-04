using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using UnityEngine;

namespace TRIdle.Game {
  using Skill;

  public static class Player {
    public static bool IsLoaded { get; private set; } = false;

    public static class Serializer {
      static string FilePath => Application.persistentDataPath + "/player.json";

      public static void Save(Stream stream) {
        if (!IsLoaded) return;
        JsonSerializer.Serialize(stream, Skill.All, Const.JsonSerializerOption);
        State.LastSave = DateTime.Now;
      }
      public static async void Load(Stream stream) {
        // Load player data
        try {
          _skills = await JsonSerializer.DeserializeAsync<SkillBase[]>(stream, Const.JsonSerializerOption)
            ?? throw new NullReferenceException("Failed to load player data.");
        }
        // If failed to load, load default data
        catch (Exception e) {
          if (e is JsonException or NotSupportedException)
            Debug.LogWarning("Failed to load player data properly. Loading default data.");
          else if (e is NullReferenceException or ArgumentNullException)
            Debug.LogWarning("No player data found. Loading default data.");

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

    public static class State {
      public static DateTime LastSave { get; set; } = DateTime.MinValue;
      public static SkillBase FocusSkill { get; set; }
      public static ActionBase FocusAction { get; set; }
    }

    public static class Statistic {

    }

    private static SkillBase[] _skills;
    public static class Skill {
      public static SkillBase[] All => _skills;
      public static T GetSkill<T>() where T : SkillBase => All.FirstOrDefault(s => s is T) as T;
    }
  }
}