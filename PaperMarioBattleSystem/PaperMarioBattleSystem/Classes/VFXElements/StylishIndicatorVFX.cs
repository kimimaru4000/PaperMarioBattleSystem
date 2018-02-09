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
    /// VFX for an indicator showing that a Stylish Move can be performed.
    /// </summary>
    public sealed class StylishIndicatorVFX : VFXElement
    {
        /// <summary>
        /// The BattleEntity to make the indicator follow.
        /// </summary>
        private BattleEntity Entity = null;

        /// <summary>
        /// The StylishData to check.
        /// </summary>
        private Sequence.StylishData StylishData = null;

        private CroppedTexture2D Indicator = null;

        public StylishIndicatorVFX(BattleEntity entity, Sequence.StylishData stylishData)
        {
            Entity = entity;
            StylishData = stylishData;

            Indicator = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"),
                new Rectangle(534, 907, 14, 30));
        }

        public override void CleanUp()
        {
            base.CleanUp();

            Entity = null;
            StylishData = null;
            Indicator = null;
        }

        public override void Update()
        {
            //If the BattleEntity or StylishData is null or the Stylish Move is finished, then mark this for removal since we're done
            if (Entity == null || StylishData == null || StylishData.Finished == true)
            {
                ReadyForRemoval = true;
                return;
            }
        }

        public override void Draw()
        {
            if (Entity == null || StylishData == null) return;

            //If we're within range, show the indicator
            if (StylishData.WithinRange == true)
            {
                SpriteRenderer.Instance.Draw(Indicator.Tex, Entity.Position + new Vector2(40f, -20f), Indicator.SourceRect, Color.Red, false, false, .95f);
            }
        }
    }
}
