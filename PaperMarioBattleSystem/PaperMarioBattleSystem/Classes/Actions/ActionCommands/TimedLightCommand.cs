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
    /// An Action Command for timed lights. As the bar fills up, press a button when the lights are lit.
    /// One move that uses this is Yoshi's Mini-Egg.
    /// <para>The bar fills over time. As such, the values passed in should be time-based. Lights are spaced out across the bar according to the LightDistribution used.</para>
    /// </summary>
    public class TimedLightCommand : FillBarCommand
    {
        /// <summary>
        /// How to distribute the lights.
        /// </summary>
        public enum LightDistributions
        {
            Even, LastLightAtEnd
        }

        protected Keys KeyToPress = Keys.Z;

        /// <summary>
        /// The number of lights to fill.
        /// </summary>
        protected int NumLights = 1;

        /// <summary>
        /// The current light you're on.
        /// If you press the button at the wrong time, this light is the one that'll fail.
        /// </summary>
        protected int CurLight = 0;

        /// <summary>
        /// The number of lights successfully filled.
        /// </summary>
        protected int LightsFilled = 0;

        /// <summary>
        /// The success range of pressing the button to fill the light.
        /// </summary>
        protected double LightRange = 200d;

        /// <summary>
        /// How much faster or slower to progress the bar.
        /// </summary>
        protected double SpeedScale = 1d;

        /// <summary>
        /// How to distribute the lights.
        /// </summary>
        protected LightDistributions LightDistribution = LightDistributions.Even;

        protected LightData[] LightRanges = null;

        /// <summary>
        /// Tells if the button was pressed for the current light.
        /// When we move onto the next light it gets reset to false.
        /// </summary>
        private bool PressedForLight = false;

        protected CroppedTexture2D UnlitLight = null;
        protected CroppedTexture2D LitLight = null;

        protected bool WithinRange
        {
            get
            {
                if (CurLight >= NumLights) return false;
                return (CurBarValue >= LightRanges[CurLight].StartRange && CurBarValue <= LightRanges[CurLight].EndRange);
            }
        }

        public TimedLightCommand(IActionCommandHandler commandAction, double maxBarValue, int numLights, double lightRange, double speedScale, LightDistributions lightDistribution) : base(commandAction, maxBarValue)
        {
            NumLights = numLights;
            LightRange = lightRange;
            SpeedScale = speedScale;
            LightDistribution = lightDistribution;

            //Space out the lights based on their LightDistribution
            if (LightDistribution == LightDistributions.Even)
            {
                SpaceOutLightsEvenly();
            }
            else if (LightDistribution == LightDistributions.LastLightAtEnd)
            {
                SpaceLastLightAtEnd();
            }
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            UnlitLight = new CroppedTexture2D(battleGFX, new Rectangle(390, 298, 44, 44));
            LitLight = new CroppedTexture2D(battleGFX, new Rectangle(341, 297, 44, 46));
        }

        public override void EndInput()
        {
            base.EndInput();

            UnlitLight = null;
            LitLight = null;
        }

        protected void SpaceOutLightsEvenly()
        {
            //Get the bar value to place the lights on the bar
            //For example, if MaxBarValue is 100 and there's 1 light, this will be 50, or the middle
            double barValue = (MaxBarValue / (1 + NumLights));

            //The lights will be placed in the center of their bar value
            //The start range will be the left half of the light range
            //The end range will be the right half of the light range
            double successRange = LightRange / 2d;

            LightRanges = new LightData[NumLights];

            for (int i = 0; i < NumLights; i++)
            {
                int index = (i + 1);
                double middle = barValue * index;
                double start = middle - successRange;
                double end = middle + successRange;
                LightRanges[i].StartRange = start;
                LightRanges[i].EndRange = end;
            }
        }

        protected void SpaceLastLightAtEnd()
        {
            //Space lights out evenly, then remove from the end of the bar to show the last light at the end
            SpaceOutLightsEvenly();
            
            double barValue = (MaxBarValue / (1 + NumLights));
            double successRange = LightRange / 2d;

            double barTrimValue = barValue - successRange;

            MaxBarValue -= barTrimValue;
        }

        protected override void ReadInput()
        {
            if (IsBarFull == true)
            {
                CommandResults result = CommandResults.Failure;

                if (LightsFilled > 0)
                    result = CommandResults.Success;

                OnComplete(result);
                return;
            }

            //Fill the bar
            FillBar(Time.ElapsedMilliseconds * SpeedScale);

            //Check for input for each light
            if (CurLight < NumLights)
            {
                if (CurBarValue > LightRanges[CurLight].EndRange)
                {
                    CurLight++;
                    if (CurLight >= NumLights) return;

                    PressedForLight = false;
                }

                //Check if the correct button was pressed
                if (PressedForLight == false && Input.GetKeyDown(KeyToPress) == true)
                {
                    //Check to see if the bar's value is within the next light's range
                    if (WithinRange == true)
                    {
                        LightsFilled++;
                    }

                    SendResponse(LightsFilled);

                    PressedForLight = true;
                }
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();

            string text = "NO!";
            Color color = Color.Red;
            if (WithinRange == true)
            {
                text = "OKAY!";
                color = Color.Green;
            }

            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, text, new Vector2(300, 100), color, .7f);

            Vector2 barScale = new Vector2(100f, 1f);
            Vector2 startPos = new Vector2(250, 150);
            Vector2 barStartPos = new Vector2(startPos.X, startPos.Y - (barScale.Y / 2f));

            DrawBar(startPos, barScale);
            DrawBarFill(startPos + new Vector2(0f, 5f), new Vector2(barScale.X, 18f));

            for (int i = 0; i < NumLights; i++)
            {
                CroppedTexture2D light = UnlitLight;

                //Draw the light as lit if the bar is in or past the light
                if (CurLight > i || (CurLight == i && WithinRange == true))
                    light = LitLight;

                //Get the start and end ranges
                float startScale = (float)(LightRanges[i].StartRange / MaxBarValue) * barScale.X;
                float endScale = (float)(LightRanges[i].EndRange / MaxBarValue) * barScale.X;

                Vector2 lightStartPos = startPos + new Vector2((int)startScale, 0f);
                Vector2 lightEndPos = startPos + new Vector2((int)endScale, 0f);

                //We know the start and end positions, so get the difference for the size
                int xDiff = (int)(lightEndPos.X - lightStartPos.X);

                //Get the midpoint
                Vector2 lightMidPos = new Vector2(lightStartPos.X + (xDiff / 2), lightStartPos.Y + 6f);

                //If the asset is 44x44, the range was 44, and the bar scale was 100, it would be 1
                //The asset should fit inside xDiff; that's the size it should be
                //diff / assetSize
                Vector2 lightScale = new Vector2(xDiff / (float)light.SourceRect.Value.Width);

                //Debug.DebugDrawLine(lightStartPos, lightStartPos + new Vector2(0, 24f), Color.White, .9f, 1, true);
                //Debug.DebugDrawLine(lightEndPos, lightEndPos + new Vector2(0, 24f), Color.White, .9f, 1, true);

                SpriteRenderer.Instance.DrawUI(light.Tex, lightMidPos, light.SourceRect, Color.White, 0f, new Vector2(.5f, 0), lightScale, false, false, .8f);
            }
        }

        protected struct LightData
        {
            public double StartRange;
            public double EndRange;

            public LightData(double startRange, double endRange)
            {
                StartRange = startRange;
                EndRange = endRange;
            }
        }
    }
}
