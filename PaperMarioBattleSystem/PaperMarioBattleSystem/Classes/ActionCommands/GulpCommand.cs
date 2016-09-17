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
        /// How much to progress of the bar
        /// </summary>
        private double SpeedScale = 1f;

        private double EndTime = 0d;

        private Keys KeyToHold = Keys.R;

        private bool StartedHolding = false;

        private Texture2D CircleImage = null;

        private bool WithinRange => (CurBarValue >= SuccessStartValue && CurBarValue < MaxBarValue);

        public GulpCommand(ICommandAction commandAction, double totalDuration, double successRange, double speedScale) : base(commandAction, totalDuration)
        {
            SuccessRange = successRange;
            SpeedScale = speedScale;

            CircleImage = AssetManager.Instance.LoadAsset<Texture2D>($"UI/Circle");
        }

        public override void StartInput()
        {
            base.StartInput();

            SuccessStartValue = MaxBarValue - SuccessRange;

            EndTime = Time.ActiveMilliseconds + MaxBarValue;
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
            Color circleColor = Color.Black;
            if (WithinRange == true)
            {
                text = "OKAY!";
                color = Color.Green;
                circleColor = Color.White;
            }

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, text, new Vector2(300, 100), color, .7f);

            Vector2 barScale = new Vector2(100f, 30f);
            Vector2 startPos = new Vector2(250, 150);
            Vector2 barStartPos = new Vector2(startPos.X, startPos.Y - (barScale.Y / 2f));

            DrawBar(barStartPos, barScale, SuccessStartValue);
            
            SpriteRenderer.Instance.Draw(CircleImage, new Vector2(startPos.X + 100f, startPos.Y), null, circleColor, 0f, CircleImage.GetOrigin(0f, .5f), 2f, false, .8f, true);
        }
    }
}
