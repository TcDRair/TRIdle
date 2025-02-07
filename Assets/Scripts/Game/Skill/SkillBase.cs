using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.Skill
{
  public abstract class SkillBase
  {
    public abstract string ID { get; } // ID는 각 액션의 고유 식별자로 사용됨
    public abstract string Name { get; } // Link to Text.Current

    public int Exp;


  }

  public sealed class Wildcrafting : SkillBase
  {
    public override string ID => "wildcrafting";
    public override string Name => Text.Current.Skill.Skill_Wildcrafting_Name;
  }
}