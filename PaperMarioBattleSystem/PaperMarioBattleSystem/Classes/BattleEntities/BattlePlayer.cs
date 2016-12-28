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

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            int itemTurns = EntityProperties.GetAdditionalProperty<int>(Enumerations.AdditionalProperty.DipItemTurns);
            if (itemTurns > 0)
            {
                BattleUIManager.Instance.PushMenu(new ItemSubMenu(1, 0, true));
            }
            else
            {
                BattleUIManager.Instance.PushMenu(GetMainBattleMenu());
            }
        }

        /// <summary>
        /// Gets the BattleMenu the BattlePlayer uses at the start of its turn.
        /// </summary>
        /// <returns></returns>
        protected abstract BattleMenu GetMainBattleMenu();
    }
}
