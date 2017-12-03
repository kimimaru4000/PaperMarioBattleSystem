using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Mini-Yux. It is created by a Yux, which the Mini-Yux protects with a shield that makes it invincible.
    /// </summary>
    public class MiniYux : BattleEnemy, ITattleableEntity
    {
        public MiniYux() : base(new Stats(0, 1, 0, 0, 0))
        {
            Name = "Mini-Yux";

            //All types of Mini-Yuxes don't take turns
            BaseTurns = 0;

            //The Helper AdditionalProperty is added when the Yux creates the Mini-Yux
            //This helps allow Mini-Yuxes to be standalone enemies if desired

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(80, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Confused, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Immobilized, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.DEFDown, new StatusPropertyHolder(95, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Burn, new StatusPropertyHolder(0, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Frozen, new StatusPropertyHolder(0, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(0, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.KO, new StatusPropertyHolder(100, 0));
        }

        #region Tattle Information

        public string[] GetTattleLogEntry()
        {
            return new string[]
            {
                "Each one can divide into two. As long as you clear these out first, ordinary Yux are nothing to be feared."
            };
        }

        public string[] GetTattleDescription()
        {
            return new string[]
            {
                "That's a Mini-Yux. A creature made to protect a Yux, it can split into two. Max HP is 1, Attack is 0, and Defense is 0." +
                "These twerps are the reason you sometimes can't do any damage to the main Yux." +
                "They're a pain, but you HAVE to beat them before the Yux. Flurrie's pretty effective..."
            };
        }

        #endregion
    }
}
