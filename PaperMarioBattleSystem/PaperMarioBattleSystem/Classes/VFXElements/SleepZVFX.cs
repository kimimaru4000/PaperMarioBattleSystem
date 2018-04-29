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
    /// The 'Z's that show up slightly above a character when the character is asleep.
    /// <para>Three increasingly larger 'Z's fade in, one at a time, in approximately 1.5 seconds.
    /// A shader is applied to make them wavy.</para>
    /// </summary>
    public sealed class SleepZVFX : VFXElement
    {
        private BattleEntity EntityOn = null;

        private Vector2 InitOffset = new Vector2(25, -30);
        private Vector2 ZOffset = new Vector2(15, -30);
        private int NumZs = 3;
        private float BaseScale = .4f;
        private float ScaleIncrease = .1f;

        private CroppedTexture2D ZGraphic = null;

        private double MaxAlphaTime = 1500d;
        private double AlphaTimePerChar = 0d;
        private double ElapsedTime = 0d;

        public SleepZVFX(BattleEntity entityOn)
        {
            EntityOn = entityOn;

            ZGraphic = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"),
                new Rectangle(393, 350, 30, 38));

            AlphaTimePerChar = MaxAlphaTime / (double)NumZs;
        }

        public override void CleanUp()
        {
            EntityOn = null;
            ZGraphic = null;
        }

        public override void Update()
        {
            ElapsedTime += Time.ElapsedMilliseconds;
        }

        public override void Draw()
        {
            float alpha = 1f;
            
            for (int i = 0; i < NumZs; i++)
            {
                //Calculate the alpha of this 'Z'
                double timeVal = (i + 1) * AlphaTimePerChar;
                if (ElapsedTime < timeVal)
                {
                    double timeDiff = timeVal - ElapsedTime;
                    if (timeDiff > AlphaTimePerChar) alpha = 0f;
                    else
                    {
                        alpha = (float)((ElapsedTime % AlphaTimePerChar) / AlphaTimePerChar);
                    }
                }

                SpriteRenderer.Instance.Draw(ZGraphic.Tex, EntityOn.Position + InitOffset + (ZOffset * i), ZGraphic.SourceRect, Color.White * alpha,
                    0f, Vector2.Zero, BaseScale + (ScaleIncrease * i), false, false, .5f);
            }
        }
    }
}
