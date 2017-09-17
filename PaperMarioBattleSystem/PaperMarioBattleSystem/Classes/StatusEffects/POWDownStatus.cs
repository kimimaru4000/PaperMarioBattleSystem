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
    /// The POWDown Status Effect.
    /// The entity's Attack is reduced by a certain value until it ends.
    /// </summary>
    public class POWDownStatus : StatusEffect
    {
        /// <summary>
        /// The amount to reduce the entity's Attack by.
        /// </summary>
        protected int AttackValue = 0;

        public POWDownStatus(int attackValue, int duration)
        {
            StatusType = Enumerations.StatusTypes.POWDown;
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(707, 154, 40, 48));

            AttackValue = attackValue;
            Duration = duration;

            AfflictedMessage = "Attack has dropped!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.LowerAttack(AttackValue);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.RaiseAttack(AttackValue);
        }

        protected override void OnPhaseCycleStart()
        {
            ProgressTurnCount();
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.RaiseAttack(AttackValue);
            }
        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.LowerAttack(AttackValue);
            }
        }

        public override StatusEffect Copy()
        {
            return new POWDownStatus(AttackValue, Duration);
        }
    }
}
