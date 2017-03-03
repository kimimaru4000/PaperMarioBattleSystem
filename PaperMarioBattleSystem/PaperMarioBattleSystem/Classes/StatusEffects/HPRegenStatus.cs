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
    /// The HPRegen Status Effect.
    /// The entity heals an amount of HP each turn until it ends.
    /// </summary>
    public sealed class HPRegenStatus : StatusEffect
    {
        /// <summary>
        /// The amount of HP to heal each turn.
        /// </summary>
        private int AmountHealed = 0;

        private CroppedTexture2D HPIcon = null;

        public HPRegenStatus(int amountHealed, int duration)
        {
            StatusType = Enumerations.StatusTypes.HPRegen;
            Alignment = StatusAlignments.Positive;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(658, 106, 38, 46));

            AmountHealed = amountHealed;
            Duration = duration;

            AfflictedMessage = "HP will briefly recover!";

            HPIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(503, 60, 35, 24));
        }

        protected override void OnAfflict()
        {
            
        }

        protected override void OnEnd()
        {
            
        }

        protected override void OnPhaseCycleStart()
        {
            EntityAfflicted.HealHP(AmountHealed);
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            
        }

        protected override void OnResume()
        {
            
        }

        public override StatusEffect Copy()
        {
            return new HPRegenStatus(AmountHealed, Duration);
        }

        public override void DrawStatusInfo(Vector2 iconPos, float depth, float turnStringDepth)
        {
            base.DrawStatusInfo(iconPos, depth, turnStringDepth);

            //The HP icon tweens between yellow (about the same yellow as Electrified's Spark icon) and dark pink, starting with dark pink
        }
    }
}
