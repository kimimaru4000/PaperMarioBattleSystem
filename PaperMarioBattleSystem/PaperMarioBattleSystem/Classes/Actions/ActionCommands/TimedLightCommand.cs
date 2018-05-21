using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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
        public int NumLights { get; private set; } = 1;

        /// <summary>
        /// The current light you're on.
        /// If you press the button at the wrong time, this light is the one that'll fail.
        /// </summary>
        public int CurLight { get; protected set; } = 0;

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

        public LightData[] LightRanges { get; protected set; } = null;

        /// <summary>
        /// Tells if the button was pressed for the current light.
        /// When we move onto the next light it gets reset to false.
        /// </summary>
        private bool PressedForLight = false;

        public bool WithinRange
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
                if (PressedForLight == false && (Input.GetKeyDown(KeyToPress) == true || (AutoComplete == true && WithinRange == true)))
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

        public struct LightData
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
