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

        /// <summary>
        /// Returns Mario's FP for BattlePlayers, as they share the same FP pool for Mario.
        /// </summary>
        public override int CurFP => BattleManager.Instance.GetMario().BattleStats.FP;

        /// <summary>
        /// While any BattleEntity can have FP, only Mario actually uses it.
        /// Both Partners and Mario add to Mario's FP pool.
        /// </summary>
        /// <param name="fp"></param>
        public override void HealFP(int fp)
        {
            BattleMario mario = BattleManager.Instance.GetMario();
            mario.BattleStats.FP = UtilityGlobals.Clamp(mario.BattleStats.FP + fp, 0, mario.BattleStats.MaxFP);
            Debug.Log($"{mario.Name} healed {fp} FP!");
        }

        /// <summary>
        /// While any BattleEntity can have FP, only Mario actually uses it.
        /// Both Partners and Mario subtract from Mario's FP pool.
        /// </summary>
        public override void LoseFP(int fp)
        {
            BattleMario mario = BattleManager.Instance.GetMario();
            mario.BattleStats.FP = UtilityGlobals.Clamp(mario.BattleStats.FP - fp, 0, mario.BattleStats.MaxFP);
            Debug.Log($"{mario.Name} healed {fp} FP!");
        }

        public override int GetEquippedBadgeCount(BadgeGlobals.BadgeTypes badgeType)
        {
            //NOTE: This isn't entity-specific right now, so it technically doesn't work properly.
            //For example, if a Partner had Mario's Jump, it could use Power Bounce if Mario had
            //the Badge equipped even if the Partner didn't.

            //NOTE 2: The problem is that all Active Badges are in the same list, and it's impossible
            //to differentiate Badges that only affect Mario from ones that affect both Mario and his Partner
            //without finding the Badge first. I feel there needs to be a new approach to how this is handled.

            BadgeGlobals.BadgeTypes newBadgeType = badgeType;

            if (PlayerType == PlayerTypes.Mario)
            {
                BadgeGlobals.BadgeTypes? tempBadgeType = BadgeGlobals.GetNonPartnerBadgeType(badgeType);
                if (tempBadgeType.HasValue == true) newBadgeType = tempBadgeType.Value;
            }
            else if (PlayerType == PlayerTypes.Partner)
            {
                BadgeGlobals.BadgeTypes? tempBadgeType = BadgeGlobals.GetPartnerBadgeType(badgeType);
                if (tempBadgeType.HasValue == true) newBadgeType = tempBadgeType.Value;
            }

            return Inventory.Instance.GetActiveBadgeCount(badgeType);
        }
    }
}
