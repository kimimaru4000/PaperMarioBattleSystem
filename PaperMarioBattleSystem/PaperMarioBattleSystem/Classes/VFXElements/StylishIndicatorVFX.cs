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

        /// <summary>
        /// The indicator icon for showing when the Stylish Move can be performed.
        /// </summary>
        private CroppedTexture2D Indicator = null;
        
        /// <summary>
        /// The bubble backing the indicator icon to make it more visible.
        /// </summary>
        private CroppedTexture2D IndicatorBubble = null;

        public StylishIndicatorVFX(BattleEntity entity, Sequence.StylishData stylishData)
        {
            Entity = entity;
            StylishData = stylishData;

            Texture2D tex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            Indicator = new CroppedTexture2D(tex, new Rectangle(534, 907, 14, 30));
            IndicatorBubble = new CroppedTexture2D(tex, new Rectangle(729, 797, 76, 76));
        }

        public override void CleanUp()
        {
            base.CleanUp();

            Entity = null;
            StylishData = null;

            Indicator = null;
            IndicatorBubble = null;
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
                //Draw this a bit in front and above the entity
                Vector2 offset = new Vector2(40, -40f);

                //For non-players, show it to the left
                if (Entity.EntityType != Enumerations.EntityTypes.Player)
                    offset.X = -offset.X;

                Vector2 drawPos = Entity.Position + offset;
            
                SpriteRenderer.Instance.Draw(Indicator.Tex, drawPos, Indicator.SourceRect, Color.Red, false, false, .95f);

                Rectangle posAndSize = new Rectangle((int)drawPos.X - 10, (int)drawPos.Y - 2, 34, 34);

                SpriteRenderer.Instance.Draw(IndicatorBubble.Tex, posAndSize, IndicatorBubble.SourceRect, Color.White, 0f, Vector2.Zero, false, false, .94f);
            }
        }
    }
}
