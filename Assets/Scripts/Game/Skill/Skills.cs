using System.Linq;
using System.Reflection;
using System.Collections;

namespace TRIdle.Game.Skill
{
  using Logics.Serialization;

  // maybe some user-defined skills will be added here
  public partial class Skills
  {
    public static Skill_Wildcrafting Wildcrafting => Skill_Wildcrafting.Instance;
  }
}