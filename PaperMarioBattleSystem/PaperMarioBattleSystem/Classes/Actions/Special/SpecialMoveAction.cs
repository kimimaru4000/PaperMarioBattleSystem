using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.StarPowerGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for Special Moves, which use up Star Power.
    /// <para>Special Moves are unaffected by All Or Nothing; make sure AllOrNothingAffected is false when defining damage data.</para>
    /// <para>Note that all Crystal Star Special Moves can hit entities with the Invisible Status Effect.</para>
    /// </summary>
    public class SpecialMoveAction : MoveAction
    {
        /// <summary>
        /// The type of Star Power this Special Move uses.
        /// </summary>
        public StarPowerTypes SPType { get; protected set; } = StarPowerTypes.StarSpirit;

        /// <summary>
        /// The amount of Star Power the Special Move costs.
        /// </summary>
        public float SPCost { get; protected set; } = 0f;

        /// <summary>
        /// Tells if this Special Move costs SP.
        /// </summary>
        public bool CostsSP => (SPCost > 0f);

        protected SpecialMoveAction()
        {
            
        }

        public SpecialMoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, StarPowerTypes spType, float spCost) : base(name, moveProperties, moveSequence)
        {
            SPType = spType;
            SPCost = spCost;
        }

        public SpecialMoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, HealingData healingData, StarPowerTypes spType, float spCost) : base(name, moveProperties, moveSequence, healingData)
        {
            SPType = spType;
            SPCost = spCost;
        }

        public SpecialMoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, ActionCommand actionCommand, HealingData healingData, StarPowerTypes spType, float spCost) : base(name, moveProperties, moveSequence, actionCommand, healingData)
        {
            SPType = spType;
            SPCost = spCost;
        }

        public SpecialMoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, DamageData damageInfo, StarPowerTypes spType, float spCost) : base(name, moveProperties, moveSequence, damageInfo)
        {
            SPType = spType;
            SPCost = spCost;
        }

        public SpecialMoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, ActionCommand actionCommand, DamageData damageInfo, StarPowerTypes spType, float spCost) : base(name, moveProperties, moveSequence, actionCommand, damageInfo)
        {
            SPType = spType;
            SPCost = spCost;
        }

        public sealed override void Initialize()
        {
            float starPower = 0f;

            //Check if the entity has enough SP to perform this Special Move
            //Only Mario has Star Power to perform Special Moves
            MarioStats marioStats = User.BattleStats as MarioStats;

            if (marioStats != null)
            {
                starPower = marioStats.GetStarPowerFromType(SPType).SPU;
            }

            //If the entity has insufficient SP to use this move, disable it
            if (SPCost > starPower)
            {
                Disabled = true;
                DisabledString = "Not enough Star Power.";
                return;
            }

            //Check base aspects to determine whether the move should be disabled or not
            base.Initialize();
        }

        public sealed override void OnActionStarted()
        {
            base.OnActionStarted();

            //Subtract Star Power if the Special Move costs SP (Focus, Star Beam, and Peach Beam are Special Moves that don't cost SP)
            //The BattleEntity must have SP, and enough of it, at this point, as the action is not selectable from the menu otherwise
            if (CostsSP == true)
            {
                //Subtract Star Power
                MarioStats marioStats = User.BattleStats as MarioStats;
                StarPowerBase starPower = marioStats.GetStarPowerFromType(SPType);
                starPower.LoseStarPower(SPCost);
            }
        }

        public override void DrawMenuInfo(Vector2 position, Color color, float alphaMod)
        {
            //Draw icon
            if (MoveInfo.Icon != null && MoveInfo.Icon.Tex != null)
            {
                SpriteRenderer.Instance.DrawUI(MoveInfo.Icon.Tex, position - new Vector2(32, 0), MoveInfo.Icon.SourceRect, color * alphaMod, false, false, .39f);
            }

            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, Name, position, color * alphaMod, 0f, Vector2.Zero, 1f, .4f);

            //Show SP count if the Special Move costs SP
            if (CostsSP == true && MoveProperties.CostDisplayType != Enumerations.CostDisplayTypes.Hidden)
            {
                Color spColor = color;

                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, GetCostString(), position + new Vector2(200, 0), spColor * alphaMod, 0f, Vector2.Zero, 1f, .4f);
            }
        }

        public override string GetCostString()
        {
            return $"{SPCost/SPUPerStarPower} SP";
        }
    }
}
