using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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
            DefensiveActions.Add(new Superguard(this));
            DefensiveActions.Add(new Guard(this));
        }

        public override void OnBattleStart()
        {
            base.OnBattleStart();

            //Set battle position
            //Players follow the same BattleIndex rules, but their positions are from right to left instead of left to right
            Vector2 battlepos = Vector2.Zero;

            if (PlayerType == PlayerTypes.Mario)
            {
                battlepos = BattleManager.Instance.MarioPos;
            }
            else if (PlayerType == PlayerTypes.Partner)
            {
                battlepos = BattleManager.Instance.PartnerPos;
            }

            if (HeightState == HeightStates.Airborne) battlepos.Y -= BattleManager.Instance.AirborneY;
            else if (HeightState == HeightStates.Ceiling) battlepos.Y -= BattleManager.Instance.CeilingY;

            SetBattlePosition(battlepos);
            Position = BattlePosition;

            bool hasLuckyStar = (Inventory.Instance.FindItem(LuckyStar.LuckyStarName, true) != null);
            bool autoComplete = EntityProperties.HasAdditionalProperty(AdditionalProperty.AutoActionCommands);

            //Check for enabling and auto-completing Action Commands for Defensive Actions
            for (int i = 0; i < DefensiveActions.Count; i++)
            {
                DefensiveActions[i].EnableActionCommand = hasLuckyStar;

                if (DefensiveActions[i].actionCommand != null)
                {
                    DefensiveActions[i].actionCommand.AutoComplete = autoComplete;
                }
            }
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            if (IsInDanger == true || EntityProperties.HasStatus(StatusTypes.Poison) == true)
            {
                AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.DangerChoosingActionName);
            }
            else
            {
                AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.ChoosingActionName);
            }

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

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();

            AnimManager.PlayAnimation(GetIdleAnim());
        }

        public override string GetIdleAnim()
        {
            switch (HealthState)
            {
                case HealthStates.Danger:
                case HealthStates.Peril:
                    return AnimationGlobals.PlayerBattleAnimations.DangerName;
            }

            //This is hacky for now - fix this once we have priorities for idle animations
            if (PreviousAction?.Name == "Defend")
                return AnimationGlobals.PlayerBattleAnimations.GuardName;

            return base.GetIdleAnim();
        }

        public override Item GetItemOfType(Item.ItemTypes itemTypes)
        {
            return Inventory.Instance.FindItem(Item.ItemCategories.Standard, itemTypes);
        }

        /// <summary>
        /// Gets the BattleMenu the BattlePlayer uses at the start of its turn.
        /// </summary>
        /// <returns></returns>
        protected abstract BattleMenu GetMainBattleMenu();

        /// <summary>
        /// Gets the BattlePlayer's Star Power.
        /// Mario returns his own, while Partners use Mario's.
        /// </summary>
        /// <returns>A StarPowerBase with the StarPower the BattlePlayer uses.</returns>
        public abstract StarPowerBase GetStarPower(StarPowerGlobals.StarPowerTypes starPowerType);
    }
}
