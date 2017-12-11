using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Confused Status Effect.
    /// The BattleEntity afflicted has a 50% chance of doing something other than what it was intended or told to do.
    /// <para>There is a slight pause before the BattleEntity does something different.</para>
    /// </summary>
    /*Possible results (from MarioWiki):
     * Mario or his partner may attack each other with a basic attack which is not Action Commanded (such as a jump, hammer, or no-FP move),
     *  which the player can try to block.
     * Mario or his partner may use the Defend, Appeal, or Run Away actions.
     * Mario or his partner may switch to a random partner. This move ignores the Quick Change badge if Mario has it equipped, so in effect,
     *  ending their turn.
     * Enemies may attack each other, or use healing moves or items on Mario and/or his partner. If an enemy is alone and attempts to attack an
     *  ally in confusion (or it cannot reach its ally), it simply does nothing.*/
    public sealed class ConfusedStatus : StatusEffect
    {
        /// <summary>
        /// The chance of the BattleEntity doing something other than what it intended when it's Confused
        /// </summary>
        private int ConfusionPercent = 50;

        public ConfusedStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Confused;
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(708, 253, 38, 46));

            Duration = duration;

            AfflictedMessage = "Confused! Movement will be\nhindered for a while!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.AddIntAdditionalProperty(Enumerations.AdditionalProperty.ConfusionPercent, ConfusionPercent);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.SubtractIntAdditionalProperty(Enumerations.AdditionalProperty.ConfusionPercent, ConfusionPercent, false);
        }

        protected override void OnPhaseCycleStart()
        {
            ProgressTurnCount();
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.SubtractIntAdditionalProperty(Enumerations.AdditionalProperty.ConfusionPercent, ConfusionPercent, false);
            }
        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.AddIntAdditionalProperty(Enumerations.AdditionalProperty.ConfusionPercent, ConfusionPercent);
            }
        }

        public override StatusEffect Copy()
        {
            return new ConfusedStatus(Duration);
        }
    }
}
