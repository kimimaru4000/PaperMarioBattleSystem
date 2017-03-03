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
    /// The Allergic Status Effect.
    /// The Entity afflicted cannot be inflicted with any new Status Effects.
    /// </summary>
    public sealed class AllergicStatus : StatusEffect
    {
        private CroppedTexture2D AllergicIcon = null;

        public AllergicStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Allergic;
            Alignment = StatusAlignments.Neutral;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(658, 106, 38, 46));

            Duration = duration;

            AfflictedMessage = "Status hasn't changed!";

            AllergicIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(350, 355, 25, 25));
        }

        protected override void OnAfflict()
        {
            //Make the entity immune to all StatusEffects
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.PositiveStatusImmune, true);
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.NeutralStatusImmune, true);
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.NegativeStatusImmune, true);
        }

        protected override void OnEnd()
        {
            //Remove the StatusEffect immunities
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.PositiveStatusImmune);
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.NeutralStatusImmune);
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.NegativeStatusImmune);
        }

        protected override void OnPhaseCycleStart()
        {
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
            return new AllergicStatus(Duration);
        }

        public override void DrawStatusInfo(Vector2 iconPos, float depth, float turnStringDepth)
        {
            base.DrawStatusInfo(iconPos, depth, turnStringDepth);

            //Draw the Allergic symbol
            //It's stored as a separate graphic since it blinks

            //The Allergic icon initially starts out visible and toggles visibility roughly every 1/3 of a second
            const double drawInterval = 334d;
            const double blinkInterval = drawInterval * 2d;
            double range = (Time.ActiveMilliseconds % blinkInterval);

            //Don't draw the icon for the latter half of the blink interval
            if (range >= drawInterval)
                return;

            Vector2 allergicOrigin = AllergicIcon.SourceRect.Value.GetCenterOrigin();

            Vector2 allergicPos = iconPos + allergicOrigin;
            float allergicDepth = depth + .00001f;

            SpriteRenderer.Instance.Draw(AllergicIcon.Tex, allergicPos, AllergicIcon.SourceRect, Color.White, 0f, new Vector2(.25f, .25f), 1f, false, allergicDepth, true);
        }
    }
}
