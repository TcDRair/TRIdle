using System.Collections.Generic;
using TRIdle.Game.Item;

namespace TRIdle.Game
{
  public partial class Storage
  {
    static Storage _R;
    public static Storage R => _R ??= new();

    
  }
}
