using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.Skill
{
  [JsonDerivedType(typeof(SB_WoodCutting), typeDiscriminator: "WoodCutting")]
  public abstract class SkillBase
  {
    #region Fixed Properties (Override Required)
    [JsonIgnore]
    public virtual string Name => Const.PLACEHOLDER;
    [JsonIgnore]
    public virtual string Description => Const.PLACEHOLDER;
    [JsonIgnore]
    public Sprite Icon => Resources.Load<Sprite>(IconPath);
    #endregion

    #region Serialized Properties (Re-assignable in Constructor)
    [JsonInclude]
    protected virtual string IconPath => "Sprite/PlaceHolder";
    [JsonInclude]
    public ActionBase[] Actions { get; protected set; }
    [JsonInclude]
    public int MaxLevel { get; protected set; } = -1;
    [JsonInclude]
    public int Level { get; protected set; } = 0;
    [JsonInclude]
    public float Proficiency { get; set; } = 0;

    [JsonInclude]
    public float ProficiencyBase { get; protected set; } = 10;
    #endregion

    #region State Properties (Override Optional)
    [JsonIgnore]
    public virtual ProgressType Progress => ProgressType.None;
    //  PlayerOld.State.FocusSkill == this ? ProgressType.Focused : ProgressType.None;
    public enum ProgressType
    {
      /// <summary>Idle. Default state.</summary>
      None,
      /// <summary>Indicates the main panel is focusing this skill. Has lower priority than Tasks.</summary>
      Focused,
      /// <summary>In progress of performing a task.</summary>
      TaskActive,
      /// <summary>Task is completed. Only used for repeatable tasks. Otherwise, use <see cref="Focused"/>.</summary>
      TaskCompleted,
      /// <summary>Task is interrupted by non-player action. Otherwise, use <see cref="Focused"/>.</summary>
      TaskInterrupted
    }
    
    /// <summary>
    /// Derive this to provide additional information for the skill.<br/>
    /// Strongly recommended to name the derived record as <see langword="Stat"/>.
    /// </summary>
    public abstract record StatBase;
    #endregion

    #region Runtime Properties (+ Related Methods)
    
    #region Initialization
    /// <summary>
    /// Initialize the skill. Called when the skill is first created.<br/>
    /// Caution: This process resets the skill to its initial state.
    /// </summary>
    public abstract void Initialize();
    /// <summary>
    /// Setup references for the skill. Called after all skills are loaded from the file.<br/>
    /// Override this to setup custom references, except for actions.
    /// </summary>
    public virtual void SetupReference()
    {
      foreach (var action in Actions)
        action.SetupReference(this);
    }
    #endregion

    #region Default Action
    [JsonIgnore]
    private ActionBase _defaultAction;
    [JsonIgnore]
    public ActionBase DefaultAction
    {
      get => _defaultAction;
      set
      {
        if (CanBeDefaultAction(value))
          _defaultAction = value;
      }
    }
    /// <summary>
    /// Check if the action can be always performed, so that it can be set as the default action.
    /// </summary>
    public bool CanBeDefaultAction<T>(T action) where T : ActionBase
      => action.GetType().GetMethod("CanPerform").DeclaringType == typeof(ActionBase);
    #endregion

    #region Get Action
    /// <summary>
    /// Get the first action that matches the type.
    /// </summary>
    /// <typeparam name="T">Type of the action to find.</typeparam>
    /// <returns>First action that matches the type. Null if not found.</returns>
    public T GetAction<T>() where T : ActionBase => Actions.FirstOrDefault(a => a is T) as T;
    /// <summary>
    /// Get the first action that matches the type.
    /// </summary>
    /// <typeparam name="T">Type of the action to find.</typeparam>
    /// <param name="action">First action that matches the type.</param>
    /// <returns>True if found, false otherwise.</returns>
    public bool GetAction<T>(out T action) where T : ActionBase
    {
      action = GetAction<T>();
      return action != null;
    }
    #endregion

    #endregion
  }
}