

using System.Collections.Generic;

namespace TRIdle.Game.Item
{
  public class TItem {
    public string Name { get; protected set; }
    public string Tooltip { get; protected set; }

    public List<Tag> Tags { get; protected set; }
  }
}