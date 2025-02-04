using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace TRIdle.Game.Skill
{
  using Knowledge;
  using Math;

  #region Derived Attributes
  [JsonDerivedType(typeof(SB_WoodCutting.SA_WoodCutting), typeDiscriminator: "WoodCutting")]
  [JsonDerivedType(typeof(SB_WoodCutting.SA_StickGathering), typeDiscriminator: "StickGathering")]
  #endregion
  public abstract class ActionBase
  {
    public ActionBase()
    {
      Callbacks = new ActionEvents(DefaultCallbacks, CustomCallbacks);
    }

    #region Fixed Properties (Override Required)
    [JsonIgnore]
    public abstract string Name { get; }
    [JsonIgnore]
    public abstract string Description { get; }
    [JsonIgnore]
    /// <summary>Duration of the action in seconds</summary>
    public virtual float Duration => DefaultDuration;
    #endregion

    #region Serialized Properties (Re-assignable in Constructor)
    [JsonInclude] public float DefaultDuration { get; protected set; } = 4;
    [JsonInclude] public bool Repeatable { get; protected set; } = true;
    [JsonInclude] public bool Pausable { get; protected set; } = false;

    [JsonInclude] public Keyword[] RequiredKnowledge { get; protected set; }

    [JsonInclude] public RFloat SkillProficiencyMultiplier { get; protected set; } = new(1);
    #endregion

    #region State Properties (Override Optional)
    public virtual bool CanPerform => true;
    public virtual string StackInfo => "";

    /// <summary>
    /// Derive this to provide additional information for the action.<br/>
    /// Strongly recommended to name the derived record as <see langword="Modifier"/>.
    /// </summary>
    public abstract record ModifierBase;
    #endregion

    #region Event Callbacks 
    public ActionEvents Callbacks { get; }

    [JsonIgnore] protected virtual ActionCallbacks CustomCallbacks => new();
    [JsonIgnore] private ActionCallbacks DefaultCallbacks => new()
    {
      OnStart = () => IsPerforming = true,
      OnProgress = deltaTime => CurrentProgress += deltaTime,
      OnPerform = () =>
      {
        Skill.Proficiency += Skill.ProficiencyBase * SkillProficiencyMultiplier.Value;
        CurrentProgress = 0;
        if (Repeatable) Callbacks.Repeat();
        else Callbacks.End();
      },
      OnRepeat = () => CurrentProgress = 0,
      OnPause = () => CurrentProgress = Pausable ? CurrentProgress : 0,
      OnEnd = () => IsPerforming = false
    };
    #endregion

    #region Runtime Reference
    [JsonInclude] public float CurrentProgress { get; set; }


    public void SetupReference(SkillBase skill) => Skill = skill;
    [JsonIgnore] public SkillBase Skill { get; private set; }
    [JsonIgnore] public ActionButton Button { get; private set; }
    [JsonIgnore] public bool IsPerforming { get; private set; }
    #endregion
  }

  #region Callback Definitions
  public delegate void ActionCallback();
  public delegate void ProgressCallback(float deltaTime);
  /// <summary>Stores event callbacks for <see cref="ActionBase"/> Instances.</summary>
  public class ActionEvents
  {
    public event ActionCallback OnStart;
    public void Start() => OnStart?.Invoke();
    public event ProgressCallback OnProgress;
    public void Progress(float deltaTime) => OnProgress?.Invoke(deltaTime);
    public event ActionCallback OnPerform;
    public void Perform() => OnPerform?.Invoke();
    public event ActionCallback OnRepeat;
    public void Repeat() => OnRepeat?.Invoke();
    public event ActionCallback OnPause;
    public void Pause() => OnPause?.Invoke();
    public event ActionCallback OnEnd;
    public void End() => OnEnd?.Invoke();

    /// <summary>Create a new instance without any callbacks.</summary>
    public ActionEvents() { }
    /// <summary>Create a new instance with predefined callbacks.</summary>
    public ActionEvents(params ActionCallbacks[] delegates)
    {
      foreach (var d in delegates)
      {
        OnStart += d.OnStart;
        OnProgress += d.OnProgress;
        OnPerform += d.OnPerform;
        OnRepeat += d.OnRepeat;
        OnPause += d.OnPause;
        OnEnd += d.OnEnd;
      }
    }
  }

  /// <summary>Stores a set of predefined callbacks for <see cref="ActionBase"/> instances.</summary>
  public class ActionCallbacks
  {
    public ActionCallback OnStart { get; set; } = () => { };
    public ProgressCallback OnProgress { get; set; } = _ => { };
    public ActionCallback OnPerform { get; set; } = () => { };
    public ActionCallback OnRepeat { get; set; } = () => { };
    public ActionCallback OnPause { get; set; } = () => { };
    public ActionCallback OnEnd { get; set; } = () => { };
  }
  #endregion
}