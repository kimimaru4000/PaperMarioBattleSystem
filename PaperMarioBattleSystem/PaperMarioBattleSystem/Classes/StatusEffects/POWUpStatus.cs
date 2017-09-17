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
    /// The POWUp Status Effect.
    /// The entity's Attack is raised by a certain value until it ends.
    /// </summary>
    public class POWUpStatus : StatusEffect
    {
        /// <summary>
        /// The amount to raise the entity's Attack by.
        /// </summary>
        protected int AttackValue = 0;

        public POWUpStatus(int attackValue, int duration)
        {
            StatusType = Enumerations.StatusTypes.POWUp;
            Alignment = StatusAlignments.Positive;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(658, 155, 38, 46));

            AttackValue = attackValue;
            Duration = duration;

            AfflictedMessage = "Attack is boosted!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.RaiseAttack(AttackValue);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.LowerAttack(AttackValue);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.LowerAttack(AttackValue);
            }
        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.RaiseAttack(AttackValue);
            }
        }

        public override StatusEffect Copy()
        {
            return new POWUpStatus(AttackValue, Duration);
        }
    }
}
