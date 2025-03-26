using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.Controller
{
  using Skill;
  using PlayerInternal;

  using Logics.Math;
  using Logics.Extensions;
  using Logics.Serialization;

  public class Player : LoaderBase
  {
    static Player s_Instance;
    public static Player Instance => s_Instance ??= new();

    public PlayerData Data { get; private set; } = new();


    private static IEnumerable<SkillBase> Skills => typeof(Skills)
      .GetProperties(BindingFlags.Public | BindingFlags.Static)
      .Where(property => property.PropertyType.IsSubclassOf(typeof(SkillBase)))
      .Select(property => property.GetValue(null) as SkillBase); // null(owner) = static
    public override IEnumerator Load() {
      this.Log($"Loading player data...");
      if (TryDeserializeDynamic($"{FilePath}/player.json", out var node)) {
        Data.FromJson(node["player"]);

        var skillNode = node["skills"];
        foreach (var skill in Skills)
          skill.LoadData(skillNode[skill.ID.ToString()]);
      }
      yield break;
    }
    public override IEnumerator Save() {
      this.Log($"Saving player data...");
      JsonObject node = new(), skillNode = new();

      foreach (var skill in Skills)
        skillNode[skill.ID.ToString()] = skill.SaveData();

      node["player"] = Data.ToJson();
      node["skills"] = skillNode;

      if (TrySerializeDynamic($"{FilePath}/player.json", node) is false)
        this.Log($"Failed to save player data.");

      yield break;
    }

    #region Skill Actions
    PlayerMono _Mono;
    PlayerMono Mono {
      get {
        if (_Mono == null) {
          _Mono = PlayerMono.GetInstance();
          // _Mono.Setup(); <- 모델 프리팹 필요 : ID값 저장 후 Resources.Load로 불러올 것
          // Find any Player gameobject in scene, else make one
          // TODO : 직렬화되는 데이터에 아래 목록을 저장한 뒤, 메인 씬 로드 시 해당 내역으로 초기화
          // 1. 커마 - 플레이어 이름, "캐릭터 모델" <- 일단 프리팹:id로 저장
          // 2. 월드 - 트랜스폼 등
          // 위 요소를 Mono에 이식해서 한 장 마무리
        }
        return _Mono;
      }
    }

    // Start Action Delay, but if the same action is focused, stop the delay instead.
    public void ActivateAction(ActionBase action) {
      Mono.StartActionDelay(Data.currentAction = (Data.currentAction == action) ? null : action);
    }

    #endregion
  }

  public record PlayerData
  {
    public SkillBase currentSkill;
    public ActionBase currentAction;

    public RFloat actionSpeed = new(1);

    public JsonObject ToJson() => new() {
      ["Placeholder"] = "Placeholder"
    };
    public void FromJson(JsonNode node) {
      node["Placeholder"] = "Placeholder";
    }
  }
}