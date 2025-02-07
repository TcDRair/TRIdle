using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Math.Values
{
  public interface IDetailedValue<T>
  {
    T Value { get; }

    string ToString();
    string ToStringWithDetail();
  }

  public abstract class RangedValue<T> : IDetailedValue<T> where T : IComparable
  {
    public T Min { get; set; }
    public T Max { get; set; }
    public T Base { get; set; }
    public RangedValue(T @base, T min, T max) {
      if (@base.CompareTo(min) < 0 || @base.CompareTo(max) > 0)
        throw new ArgumentOutOfRangeException($"Base value {@base} is out of range [{min} ~ {max}]");
      if (min.CompareTo(max) > 0)
        throw new ArgumentOutOfRangeException($"Min value {min} is greater than Max value {max}");
      Base = @base;
      Min = min;
      Max = max;
    }

    public virtual T Value => Clamp(Base);
    protected T Clamp(T value) {
      T result = value;
      if (result.CompareTo(Min) < 0) result = Min;
      if (result.CompareTo(Max) > 0) result = Max;
      return result;
    }

    public override string ToString() => Value.ToString();
    public virtual string ToStringWithDetail() => $"{Value} <color=gray>[{Min} ~ {Max}]</color>";
  }

  public abstract class ModifiedRangedValue<T> : RangedValue<T> where T : IComparable
  {
    protected readonly Dictionary<object, T> m_adders = new();
    protected readonly Dictionary<object, T> m_multipliers = new();

    private const float UpdateInterval = 1 / 30f;
    private float lastUpdateTime = 0;
    protected T c_adder, c_multiplier, c_value;

    public ModifiedRangedValue(T @base, T min, T max) : base(@base, min, max) { }
    public override T Value {
      get {
        if (Time.time - lastUpdateTime > UpdateInterval) {
          lastUpdateTime = Time.time;
          CalculateValue();
        }
        return c_value;
      }
    }

    // Make non-abstract function if arithmetic interfaces are implemented (.Net 7+ features now)
    protected abstract void CalculateValue();

    public override string ToString() => Value.ToString();
    public override string ToStringWithDetail() => $"{Value} <color=gray>= ({Base} + {c_adder}) * {c_multiplier} [{Min} ~ {Max}]</color>";
  }
}

namespace TRIdle.Math
{
  using Values;

  public class RInt : ModifiedRangedValue<int>
  {
    public const int DefaultMax = (int)1e9; // almost half of int.MaxValue
    public RInt(int @base, int min = 0, int max = DefaultMax) : base(@base, min, max) { }

    protected override void CalculateValue() {
      c_adder = m_adders.Values.Sum();
      c_multiplier = m_multipliers.Values.Sum();
      c_value = Clamp(Clamp(Base + c_adder) * c_multiplier);
    }
  }
  public class RFloat : ModifiedRangedValue<float>
  {
    public const float DefaultMax = 1e9f;
    public RFloat(float @base, float min = 0, float max = DefaultMax) : base(@base, min, max) { }

    protected override void CalculateValue() {
      c_adder = m_adders.Values.Sum();
      c_multiplier = m_multipliers.Values.Sum();
      c_value = Clamp(Clamp(Base + c_adder) * c_multiplier);
    }
  }
}