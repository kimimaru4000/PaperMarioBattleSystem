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
    /// Hold Left until the fourth light lights up
    /// </summary>
    public class HammerCommand : FillLightsCommand
    {
        protected float EndTime = 1500f;
        protected float PrevEndTime = 0f;

        protected bool HeldLeft = false;

        protected Keys ButtonToHold = Keys.Left;

        public HammerCommand(IActionCommandHandler commandAction, int maxLights, double timeBetweenLights) : base(commandAction, maxLights, timeBetweenLights)
        {

        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            LightsFilled = 0;
            PrevLightTime = (float)Time.ActiveMilliseconds + TimeBetweenLights;
            PrevEndTime = (float)Time.ActiveMilliseconds + EndTime;
        }

        protected override void ReadInput()
        {
            float time = (float)Time.ActiveMilliseconds;

            //Didn't hold Left in time
            if (HeldLeft == false && time >= PrevEndTime)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            //Holding Left
            if (AutoComplete == true || Input.GetKey(ButtonToHold))
            {
                //The first time you hold left, the first light instantly lights up
                if (time >= PrevLightTime || HeldLeft == false)
                {
                    FillNextLight();

                    //Send the number of lights lit
                    SendResponse(LightsFilled);

                    //Auto complete
                    if (AllLightsFilled == true)
                        AutoComplete = false;

                    //Held Left too long (past the last light)
                    if (LightsFilled > MaxLights)
                    {
                        OnComplete(CommandResults.Failure);
                    }
                }

                HeldLeft = true;
            }
            //Released Left
            else if (HeldLeft == true)
            {
                //Released Left at the right time
                if (AllLightsFilled == true)
                {
                    SendCommandRank(CommandRank.Nice);
                    OnComplete(CommandResults.Success);
                }
                //Released Left too early
                else
                {
                    OnComplete(CommandResults.Failure);
                }
            }
        }
    }
}
