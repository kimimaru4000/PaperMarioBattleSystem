using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for playable characters in battle (Ex. Mario and his Partners)
    /// </summary>
    public abstract class BattlePlayer : BattleEntity
    {
        public PlayerTypes PlayerType { get; protected set; } = PlayerTypes.Mario;

        public BattlePlayer(Stats stats) : base(stats)
        {
            DefensiveActions.Add(new Guard(this));
        }

        public override void BraceAttack(BattleEntity attacker)
        {
            base.BraceAttack(attacker);
        }

        public override void StopBracing()
        {
            base.StopBracing();
        }
    }
}
