using UnityEngine;

namespace TRIdle.Game.Item.Attributes
{
  // 코드 정리하지 말 것
  public enum Attr
  {
    Size,
      Small,
      Medium,
      Large,

    Shape,
      Fabric,
      FabricPiece,
      Plate,
      Column,
      Stick,
      String,
        Rope,

    #region Material
    Material,
      Plant,
        Wood,
        Stem,
        Cotton,
      Metal,
        Iron,
        Steel,
        Copper,
        Zinc,
        Silver,
        Gold,
        Lead,
      Fiber,
        CottonFiber,
        Linen,
        Wool,
        Hemp,
        Silk,
        CompositeFiber, //TODO
      Fluid,
        Water,
        Oil,
        Acid,
        Alcohol,
    #endregion

    #region Usage
    Usage,
      Tool,
        Hammer, //TODO
        Axe,
        Saw,
        Knife,
        Scissors,
        Needle,
        Thread,
        Nails,
        Screw,
        Bolt,
        Nut,
        Washer,
        Pliers,
        Wrench,
        Spanner,
        Clamp,
        Vise,
        File,
        Rasp,
        Sandpaper,
        Chisel,
        Plane,
        Spokeshave,
        Veneer,
        Router,
        Drill,
        Auger,
        Bit,
        Reamer,
        Tap,
        Die,
        Broach,
        Mandrel,
        Punch,
        Awl,
      RawMaterial,
        Ore,
        Glue,

    #endregion

    Unique,
  }
}
