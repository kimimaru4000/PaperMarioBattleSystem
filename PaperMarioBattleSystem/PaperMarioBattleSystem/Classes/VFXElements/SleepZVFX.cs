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

        private double MaxAlphaTime = 1500d;
        private double AlphaTimePerChar = 0d;
        private double ElapsedTime = 0d;

        private CroppedTexture2D ZGraphic = null;
        private Effect SleepShader = null;

        public SleepZVFX(BattleEntity entityOn)
        {
            EntityOn = entityOn;

            AlphaTimePerChar = MaxAlphaTime / (double)NumZs;

            int valAround = 3;
            Rectangle baseRect = new Rectangle(393, 350, 30, 38);
            baseRect.X -= valAround;
            baseRect.Y -= valAround;
            baseRect.Width += (valAround * 2);
            baseRect.Height += (valAround * 2);

            Debug.Log(baseRect);

            Texture2D tex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");
            ZGraphic = new CroppedTexture2D(tex, baseRect);

            SleepShader = AssetManager.Instance.LoadAsset<Effect>($"{ContentGlobals.ShaderRoot}Sleep");
            SleepShader.Parameters["textureSize"].SetValue(new Vector2(tex.Width, tex.Height));
            SleepShader.Parameters["intensity"].SetValue(220f);
            SleepShader.Parameters["moveAmtX"].SetValue((float)valAround - 1);
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
            //NOTE: Batch this somewhere eventually; may require more flexible rendering
            //This works for now, though
            SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.spriteBatch);

            float alpha = 1f;
            
            for (int i = 0; i < NumZs; i++)
            {
                //Start a new batch for each 'Z', as each has slightly different timing
                SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.spriteBatch, BlendState.AlphaBlend, null, SleepShader, Camera.Instance.Transform);

                //The shift amount is on a global timer
                //However, each 'Z's cycle is offset by a small amount
                float shiftTimeVal = RenderingGlobals.SleepShaderShiftOffset + (i * (float)Time.ElapsedMilliseconds);
                SleepShader.Parameters["shiftTime"].SetValue(shiftTimeVal);

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

                SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.spriteBatch);
            }

            SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.spriteBatch, BlendState.AlphaBlend, null, null, Camera.Instance.Transform);
        }
    }
}
