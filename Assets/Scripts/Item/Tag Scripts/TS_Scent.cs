using System;

namespace TRIdle.Game.Item
{
  public class TS_Scent : TagScript {
    bool HasEffect =>
      EnvironmentEffect.GetPersistentEventCount() > 0 ||
      StructureEffect.GetPersistentEventCount() > 0 ||
      ItemEffect.GetPersistentEventCount() > 0;
    bool KnownEffect => false; // TODO

    string D_base => Tag.level switch {
      1 => "희미한 향기",
      2 => "그윽한 향기",
      3 => "풍성한 향기",
      4 => "강렬한 향기",
      5 => "아찔한 향기",
      _ => throw new ArgumentOutOfRangeException()
    };
    string D_unknown => Tag.level switch {
      1 or 2 => "미묘하고 ",
      3 or 4 => "알 수 없는 ",
      5 => "형용할 수 없는",
      _ => throw new ArgumentOutOfRangeException()
    };
    string D_hasEffect => HasEffect ? "가 느껴집니다." : Tag.level switch {
      1 or 2 => "가 감돕니다.",
      3 or 4 => "가 주위를 감쌉니다.",
      5 => "에 사로잡힙니다.",
      _ => throw new ArgumentOutOfRangeException()
    };

    public override string Description => D_unknown + D_base + D_hasEffect;
    public override Affix Prefix => new() {
      text = Tag.level >= 3 ? "향기로운 " : "",
      order = -2
    };
  }
}
