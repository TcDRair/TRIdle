using System;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Logics.Math
{
  using UTime = UnityEngine.Time;

  public interface IDetailedValue<T>
  {
    T Value { get; }

    string ToString();
    string ToStringWithDetail();
  }

  /// <summary>A struct that holds a float value with additional properties</summary>
  public struct SFloat
  {
    public float value, min, max, adder, multiplier;

    public SFloat(float value, float min, float max) {
      if (value < min || value > max)
        throw new ArgumentOutOfRangeException($"Value {value} is out of range [{min} ~ {max}]");
      if (min > max)
        throw new ArgumentOutOfRangeException($"Min value {min} is greater than Max value {max}");
      this.value = value;
      this.min = min;
      this.max = max;
      adder = 0;
      multiplier = 1;
    }

    public readonly float Calculated => Clamp(Clamp(value + adder) * multiplier);
    private readonly float Clamp(float value) => Mathf.Clamp(value, min, max);
  }
  /// <summary>Delegate using SFloat as input/output</summary>
  public delegate SFloat Modifier(SFloat value);

  /// <summary>A class that holds a float value with modifiers</summary>
  public class RFloat : IDetailedValue<float>
  {
    private SFloat _Base, _Result;
    protected readonly Dictionary<object, Modifier> m_modifiers = new();

    public event Modifier Modifiers {
      add { m_modifiers.TryAdd(value.Target, value); }
      remove { if (m_modifiers.TryGetValue(value.Target, out _)) m_modifiers.Remove(value.Target); }
    }

    public RFloat(float value, float min = 0, float max = 1e9f) {
      if (min > max)
        throw new ArgumentOutOfRangeException($"Min value {min} is greater than Max value {max}");
      if (value < min || value > max)
        throw new ArgumentOutOfRangeException($"Given value {value} is out of range [{min} ~ {max}]");

      _Base = new SFloat(value, min, max);
      UpdateValue();
    }

    private float _CachedValue;
    public float Value {
      get {
        if (UTime.time - _CachedTime > UpdateInterval) {
          _CachedTime = UTime.time;
          UpdateValue();
        }
        return _CachedValue;
      }
    }
    private float _CachedTime = -1;
    private const float UpdateInterval = 1 / 30f;
    private void UpdateValue() {
      _Result = _Base;
      foreach (var modifier in m_modifiers.Values)
        _Result = modifier(_Result);
      _CachedValue = _Result.Calculated;
    }

    public override string ToString() => Value.ToString();
    public string ToStringWithDetail() => $"{Value} <color=gray>= ({_Result.value} + {_Result.adder}) * {_Result.multiplier} [{_Result.min} ~ {_Result.max}]</color>";
  }
}