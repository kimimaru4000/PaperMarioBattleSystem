using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.BattlePlayerGlobals;

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
            DefensiveActions.Add(new Superguard(this));
        }
    }
}
