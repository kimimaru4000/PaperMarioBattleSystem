using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Hold R until the light lights up.
    /// <para>The amount the bar increases is based on elapsed time and the speed scale.
    /// The MaxBarValue represents the total duration of the Action Command.</para>
    /// </summary>
    public sealed class GulpCommand : FillBarCommand
    {
        /// <summary>
        /// The value the success range starts
        /// </summary>
        private double SuccessStartValue = 0d;

        /// <summary>
        /// The amount of time the light is lit up for the success to be valid
        /// </summary>
        private double SuccessRange = 0d;

        /// <summary>
        /// How much faster or slower to progress the bar
        /// </summary>
        private double SpeedScale = 1f;

        private double EndTime = 0d;

        private Keys KeyToHold = Keys.R;

        private bool StartedHolding = false;

        private CroppedTexture2D UnlitLight = null;
        private CroppedTexture2D LitLight = null;

        private bool WithinRange => (CurBarValue >= SuccessStartValue && CurBarValue < MaxBarValue);

        public GulpCommand(IActionCommandHandler commandAction, double totalDuration, double successRange, double speedScale, Keys buttonToHold) : base(commandAction, totalDuration)
        {
            SuccessRange = successRange;
            SpeedScale = speedScale;

            KeyToHold = buttonToHold;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            UnlitLight = new CroppedTexture2D(battleGFX, new Rectangle(390, 298, 44, 44));
            LitLight = new CroppedTexture2D(battleGFX, new Rectangle(341, 297, 44, 46));

            SuccessStartValue = MaxBarValue - SuccessRange;

            EndTime = Time.ActiveMilliseconds + MaxBarValue;
        }

        public override void EndInput()
        {
            base.EndInput();

            UnlitLight = null;
            LitLight = null;
        }

        protected override void ReadInput()
        {
            //If the command is going past the total duration, stop
            if (Time.ActiveMilliseconds > EndTime)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            //If you started holding then stopped, check when the player stopped holding
            if (StartedHolding == true && Input.GetKey(KeyToHold) == false)
            {
                if (WithinRange == true)
                {
                    SendCommandRank(CommandRank.Nice);
                    OnComplete(CommandResults.Success);
                }
                else
                {
                    OnComplete(CommandResults.Failure);
                }
                return;
            }

            //Have the bar keep increasing, and make the light light up at a certain value range
            //The bar value can increase further than the light, but it won't show past the light
            if (Input.GetKey(KeyToHold) == true)
            {
                StartedHolding = true;

                FillBar(Time.ElapsedMilliseconds * SpeedScale);
                //If the button was held too long, it's a failure
                if (CurBarValue > MaxBarValue)
                {
                    OnComplete(CommandResults.Failure);
                }
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();

            string text = "NO!";
            Color color = Color.Red;
            CroppedTexture2D light = UnlitLight;
            if (WithinRange == true)
            {
                text = "OKAY!";
                color = Color.Green;
                light = LitLight;
            }

            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, text, new Vector2(300, 100), color, .7f);

            Vector2 barScale = new Vector2(100f, 1f);
            Vector2 startPos = new Vector2(250, 150);
            Vector2 barStartPos = new Vector2(startPos.X, startPos.Y - (barScale.Y / 2f));

            //Get the start and end ranges
            float startScale = (float)(SuccessStartValue / MaxBarValue) * barScale.X;
            float endScale = barScale.X;

            Vector2 lightStartPos = startPos + new Vector2((int)startScale, 0f);
            Vector2 lightEndPos = startPos + new Vector2((int)endScale, 0f);

            int diff = (int)(lightEndPos.X - startPos.X);

            DrawBar(barStartPos, barScale, SuccessStartValue);
            DrawBarFill(barStartPos + new Vector2(0f, 5f), new Vector2(barScale.X, 18f), SuccessStartValue);

            SpriteRenderer.Instance.DrawUI(light.Tex, lightStartPos + new Vector2((diff / 2) - BarEnd.WidthHeightToVector2().X, 12f), light.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), 1f, false, false, .8f);
        }
    }
}
