using System.Collections.Generic;
using TRIdle.Game.Item;

namespace TRIdle.Game
{
  public partial class Storage
  {
    static Storage _R;
    public static Storage R => _R ??= new();


    public struct R_Inventory {
      //? �±׺��� �κ��丮�� ������... �Ƹ� ���� �±װ� �ǰ���
      public List<TItem> Structure;
      public List<TItem> Food;
    }
    public R_Inventory Inventory;

  }
}