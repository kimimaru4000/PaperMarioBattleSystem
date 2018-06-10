using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Class for global values dealing with Badges
    /// </summary>
    public static class BadgeGlobals
    {
        #region Fields

        /// <summary>
        /// The max number of Simplifiers that can affect ActionCommands.
        /// </summary>
        public const int MaxSimplifierCount = 3;

        /// <summary>
        /// The max number of Unsimplifiers that can affect ActionCommands.
        /// </summary>
        public const int MaxUnsimplifierCount = 3;

        #endregion

        #region Enums

        /// <summary>
        /// The various types of Badges (what the actual Badges are).
        /// <para>The values are defined by each Badge type's Type Number.
        /// If Badges exist in the same spot and aren't in both games, Badges with lower alphabetical values will be placed first.
        /// In cases where one Badge is before another Badge in one game and after that Badge in the other game, the Badge is grouped
        /// with similar Badges around it.</para>
        /// <para>If adding brand new badges, put them at the bottom.</para>
        /// </summary>
        public enum BadgeTypes
        {
            //Default value
            None = 0,
            PowerJump = 1, MegaJump = 2, Multibounce = 3, JumpCharge = 4, SJumpCharge = 5, ShrinkStomp = 6,
            SleepStomp = 7, DizzyStomp = 8, SoftStomp = 9, DDownJump = 10, TornadoJump = 11,
            PowerBounce = 12, PowerSmash = 13, MegaSmash = 14, PiercingBlow = 14,
            SmashCharge = 15, SSmashCharge = 16, SpinSmash = 17, HammerThrow = 18,
            HeadRattle = 19, IceSmash = 20,
            QuakeHammer = 21, PowerQuake = 22, MegaQuake = 23, DDownPound = 24,
            FireDrive = 25, Charge = 26, ChargeP = 27,
            DoubleDip = 28, DoubleDipP = 29, TripleDip = 30, GroupFocus = 31,
            DodgeMaster = 32, DeepFocus = 33, HPPlus = 34, HPPlusP = 35, FPPlus = 36,
            PowerPlus = 37, PowerPlusP = 38, AllOrNothing = 39, Jumpman = 40, Hammerman = 41,
            PUpDDown = 42, PUpDDownP = 43, PDownDUp = 44, PDownDUpP = 45,
            DefendPlus = 46, DefendPlusP = 47, DamageDodge = 48, DamageDodgeP = 49,
            DoublePain = 50, PowerRush = 51, PowerRushP = 52, LastStand = 53, LastStandP = 54,
            MegaRush = 55, MegaRushP = 56, CloseCall = 57, CloseCallP = 58,
            PrettyLucky = 59, PrettyLuckyP = 60, LuckyDay = 61, LuckyStart = 62,
            HappyHeart = 63, HappyHeartP = 64, HappyFlower = 65,
            FlowerSaver = 66, FlowerSaverP = 67, PityFlower = 68, HPDrain = 69, HPDrainP = 70,
            FPDrain = 71, HeartFinder = 72, FlowerFinder = 73, ItemHog = 74, RunawayPay = 75,
            Refund = 76, PayOff = 77, MoneyMoney = 78,
            IcePower = 79, FireShield = 80, SpikeShield = 81,
            ZapTap = 82, ReturnPostage = 83,
            FeelingFine = 84, FeelingFineP = 85, SuperAppeal = 86, SuperAppealP = 87,
            Peekaboo = 88, ISpy = 89, QuickChange = 90, TimingTutor = 91,
            Simplifier = 92, Unsimplifier = 93, ChillOut = 94,
            SpeedySpin = 95, DizzyAttack = 96, SpinAttack = 97, FirstAttack = 98, BumpAttack = 99,
            LEmblem = 100, WEmblem = 101, SlowGo = 102,
            AttackFXA = 103, AttackFXB = 104, AttackFXC = 105, AttackFXD = 106, AttackFXE = 107,
            AttackFXR = 108, AttackFXY = 109, AttackFXG = 110, AttackFXP = 111,
            
            //Unused Badges
            AttackFXF = 112, AngersPower = 113, RightOn = 114
            
            //New badges
        }

        /// <summary>
        /// Who the Badge affects.
        /// <para>For Players, Self refers to Mario, and Partner refers to Partners.
        /// Both is for Badges such as Simplifier and Unsimplifier that affect both Mario and Partners.
        /// For Enemies, all types affect the Enemy equipped.</para>
        /// </summary>
        public enum AffectedTypes
        {
            Self, Partner, Both
        }

        /// <summary>
        /// Filter options for finding Badges.
        /// </summary>
        public enum BadgeFilterType
        {
            All, Equipped, UnEquipped
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a non-Partner BadgeTypes corresponding to a particular BadgeTypes.
        /// </summary>
        /// <param name="pBadgeType">The Partner version of the BadgeTypes to find a non-Partner version for.</param>
        /// <returns>A non-Partner version of the BadgeTypes passed in. If already a non-Partner version, it will be returned. null if none was found.</returns>
        public static BadgeTypes? GetNonPartnerBadgeType(BadgeTypes pBadgeType)
        {
            string pBadgeName = pBadgeType.ToString();

            //Check the last character for a "P"
            string checkP = pBadgeName.Substring(pBadgeName.Length - 1);

            //This is the non-Partner version, so return it
            if (checkP != "P")
                return pBadgeType;

            //Remove the "P" and see if there is a corresponding value
            string nonPBadgeName = pBadgeName.Substring(0, pBadgeName.Length - 1);

            BadgeTypes nonPBadgeType;
            bool success = Enum.TryParse(nonPBadgeName, out nonPBadgeType);

            if (success == true) return nonPBadgeType;
            return null;
        }

        /// <summary>
        /// Returns a Partner BadgeTypes corresponding to a particular non-Partner BadgeTypes.
        /// </summary>
        /// <param name="pBadgeType">The non-Partner version of the BadgeTypes to find a Partner version for.</param>
        /// <returns>A Partner version of the BadgeTypes passed in. If already a Partner version, it will be returned. null if none was found.</returns>
        public static BadgeTypes? GetPartnerBadgeType(BadgeTypes badgeType)
        {
            string badgeName = badgeType.ToString();

            //Check the last character for a "P"
            string checkP = badgeName.Substring(badgeName.Length - 1);

            //This is the Partner version, so return it
            if (checkP == "P")
                return badgeType;

            //Add a "P" and see if there is a corresponding value
            string pBadgeName = badgeName + "P";

            BadgeTypes pBadgeType;
            bool success = Enum.TryParse(pBadgeName, out pBadgeType);

            if (success == true) return pBadgeType;
            return null;
        }

        #endregion

        #region Badge Sort Methods

        /// <summary>
        /// A Comparison method used to sort Badges by their Type Numbers
        /// </summary>
        /// <param name="badge1">The first Badge to compare</param>
        /// <param name="badge2">The second Badge to compare</param>
        /// <returns>-1 if badge1 has a lower TypeNumber, 1 if badge2 has a lower TypeNumber, 0 if they have the same TypeNumber</returns>
        public static int BadgeTypeNumberSort(Badge badge1, Badge badge2)
        {
            if (badge1 == null && badge2 == null) return 0;
            if (badge1 == null) return 1;
            if (badge2 == null) return -1;

            if (badge1.TypeNumber < badge2.TypeNumber)
                return -1;
            if (badge1.TypeNumber > badge2.TypeNumber)
                return 1;

            return 0;
        }

        /// <summary>
        /// A Comparison method used to sort Badges alphabetically (ABC)
        /// </summary>
        /// <param name="badge1">The first Badge to compare</param>
        /// <param name="badge2">The second Badge to compare</param>
        /// <returns>-1 if badge1 has a lower alphabetical value, 1 if badge2 has a lower alphabetical value, 0 if they have the same alphabetical value</returns>
        public static int BadgeAlphabeticalSort(Badge badge1, Badge badge2)
        {
            if (badge1 == null && badge2 == null) return 0;
            if (badge1 == null) return 1;
            if (badge2 == null) return -1;

            return string.Compare(badge1.Name, badge2.Name, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// A Comparison method used to sort Badges by BP cost (BP Needed)
        /// </summary>
        /// <param name="badge1">The first Badge to compare</param>
        /// <param name="badge2">The second Badge to compare</param>
        /// <returns>-1 if badge1 has a lower BP cost, 1 if badge2 has a lower BP cost, 0 if they have the same BP cost and TypeNumber</returns>
        public static int BadgeBPSort(Badge badge1, Badge badge2)
        {
            if (badge1 == null && badge2 == null) return 0;
            if (badge1 == null) return 1;
            if (badge2 == null) return -1;

            if (badge1.BPCost < badge2.BPCost)
                return -1;
            if (badge1.BPCost > badge2.BPCost)
                return 1;

            //Resort to their TypeNumbers if they have the same BP cost
            return BadgeTypeNumberSort(badge1, badge2);
        }

        #endregion
    }
}
