using System.Collections.Generic;
using TRIdle.Game.Item;

namespace TRIdle.Game
{
  public partial class Storage
  {
    static Storage _R;
    public static Storage R => _R ??= new();


    public struct R_Inventory {
      //? 태그별로 인벤토리를 나눌까... 아마 메인 태그가 되겠지
      public List<TItem> Structure;
      public List<TItem> Food;
    }
    public R_Inventory Inventory;

  }
}