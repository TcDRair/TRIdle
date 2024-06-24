using System;
using System.Linq;
using UnityEngine;

namespace TRIdle
{
  public class EventValue<T>
  {
    public T Base { get; set; }
    public event Func<T, T> OnGetValue;
    public EventValue(T @base) => Base = @base;

    private T cachedValue;
    private float lastUpdateTime = 0;
    private const float UpdateInterval = 1 / 30f;
    public virtual T Value
    {
      get
      {
        if (Time.time - lastUpdateTime > UpdateInterval)
        {
          lastUpdateTime = Time.time;
          UpdateValue();
        }
        return cachedValue;
      }
    }

    private void UpdateValue()
    {
      T result = Base;
      if (OnGetValue != null) 
        foreach (Func<T, T> f in OnGetValue.GetInvocationList().Cast<Func<T, T>>())
          result = f(result);
      cachedValue = result;
    }

    public override string ToString() => Value.ToString();
  }

  public class EInt : EventValue<int>
  {
    public EInt(int @base) : base(@base) { }
  }
  public class EFloat : EventValue<float>
  {
    public EFloat(float @base) : base(@base) { }
  }

  public class RangedEValue<T> : EventValue<T> where T : IComparable<T>
  {
    public T Min { get; set; }
    public T Max { get; set; }
    public override T Value
    {
      get
      {
        T result = base.Value;
        if (result.CompareTo(Min) < 0) result = Min;
        if (result.CompareTo(Max) > 0) result = Max;
        return result;
      }
    }

    public RangedEValue(T @base, T min, T max) : base(@base)
    {
      Min = min;
      Max = max;
    }

    public override string ToString() => $"{Value} ({Min} ~ {Max})";
  }
  public class REInt : RangedEValue<int>
  {
    public REInt(int @base, int min = 0, int max = int.MaxValue) : base(@base, min, max) { }
  }
  public class REFloat : RangedEValue<float>
  {
    public REFloat(float @base, float min = 0, float max = float.PositiveInfinity) : base(@base, min, max) { }
  }
}