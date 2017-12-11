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
    /// The Dodgy Status Effect.
    /// The entity's Evasion increases until it ends.
    /// </summary>
    public class DodgyStatus : StatusEffect
    {
        /// <summary>
        /// The Evasion modifier.
        /// </summary>
        protected double EvasionValue = .5d;

        public DodgyStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Dodgy;
            Alignment = StatusAlignments.Positive;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(658, 253, 38, 46));

            Duration = duration;

            AfflictedMessage = "Dodgy! Some attacks will\nautomatically be dodged!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.AddEvasionMod(EvasionValue);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.RemoveEvasionMod(EvasionValue);
        }

        protected override void OnPhaseCycleStart()
        {
            ProgressTurnCount();
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                OnEnd();
            }
        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                OnAfflict();
            }
        }

        public override StatusEffect Copy()
        {
            return new DodgyStatus(Duration);
        }
    }
}
