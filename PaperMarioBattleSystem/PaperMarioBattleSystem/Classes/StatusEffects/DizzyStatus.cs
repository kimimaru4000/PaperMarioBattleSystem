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
    /// The Dizzy Status Effect.
    /// The entity's Accuracy decreases until it ends
    /// </summary>
    public sealed class DizzyStatus : StatusEffect
    {
        private double AccuracyValue = .5d;
        private CroppedTexture2D DizzyIcon = null;

        public DizzyStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Dizzy;
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(658, 106, 38, 46));

            Duration = duration;

            AfflictedMessage = "Dizzy! Attacks might miss!";

            DizzyIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(404, 102, 32, 34));
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.AddAccuracyMod(AccuracyValue);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.RemoveAccuracyMod(AccuracyValue);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            OnEnd();
        }

        protected override void OnResume()
        {
            OnAfflict();
        }

        public override StatusEffect Copy()
        {
            return new DizzyStatus(Duration);
        }

        public override void DrawStatusInfo(Vector2 iconPos, float depth, float turnStringDepth)
        {
            base.DrawStatusInfo(iconPos, depth, turnStringDepth);

            //Draw the Dizzy symbol
            //This is stored as a separate graphic since it rotates

            //Rotate the Dizzy symbol
            const double remainder = Math.PI * 2d;
            const double rotTime = 100d;
            float rotation = -(float)((Time.ActiveMilliseconds / rotTime) % remainder);

            //The last Vector2 is an offset to the position
            Vector2 dizzyPos = iconPos + DizzyIcon.SourceRect.Value.GetCenterOrigin() + new Vector2(3, 1);
            float dizzyDepth = depth + .00001f;

            SpriteRenderer.Instance.Draw(DizzyIcon.Tex, dizzyPos, DizzyIcon.SourceRect, Color.White, rotation, new Vector2(.5f, .5f), 1f, false, dizzyDepth, true);
        }
    }
}
