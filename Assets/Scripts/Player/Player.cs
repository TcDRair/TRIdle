using UnityEngine;
using System.Text.Json;
using System.IO;

namespace TRIdle.Game
{
  using System;
  using Skill;
  public static class Player
  {
    public static bool IsLoaded { get; private set; } = false;
    public static DateTime LastSave { get; private set; } = DateTime.Now;
    public static SkillBase[] AllSkills { get; private set; } = new SkillBase[] {
      woodCutting = new SMPSkill_WoodCutting()
    };

    public static SMPSkill_WoodCutting woodCutting;

    static string FilePath => Application.persistentDataPath + "/player.json";
    public static void Save()
    {
      JsonSerializer.Serialize(new FileStream(FilePath, FileMode.Create), AllSkills, new JsonSerializerOptions { WriteIndented = true });
      LastSave = DateTime.Now;
    }

    public static void Load()
    {
      if (File.Exists(FilePath) is false) { IsLoaded = true; return; }
      AllSkills = JsonSerializer.Deserialize<SkillBase[]>(new FileStream(FilePath, FileMode.Open));
      IsLoaded = true;
    }
  }
}