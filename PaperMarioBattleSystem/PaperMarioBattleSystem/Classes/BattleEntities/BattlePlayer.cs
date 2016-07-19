using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for playable characters in battle (Ex. Mario and his Partners)
    /// </summary>
    public abstract class BattlePlayer : BattleEntity
    {
        public BattlePlayer()
        {
            
        }

        public BattlePlayer(Stats stats) : base(stats)
        {
            
        }

        public override void BraceAttack(BattleEntity attacker)
        {
            base.BraceAttack(attacker);
        }

        public override void StopBracing()
        {
            base.StopBracing();
        }

        public override bool HasBadge(BadgeGlobals.BadgeTypes badgeType)
        {
            //TEMPORARY
            return false;
        }
    }
}
