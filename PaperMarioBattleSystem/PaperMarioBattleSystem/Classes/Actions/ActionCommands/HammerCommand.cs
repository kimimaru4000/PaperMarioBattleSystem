using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

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

        protected CroppedTexture2D LightBarEnd = null;
        protected CroppedTexture2D LightBarMiddle = null;

        public HammerCommand(IActionCommandHandler commandAction, int maxLights, double timeBetweenLights) : base(commandAction, maxLights, timeBetweenLights)
        {

        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            LightsFilled = 0;
            PrevLightTime = (float)Time.ActiveMilliseconds + TimeBetweenLights;
            PrevEndTime = (float)Time.ActiveMilliseconds + EndTime;

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            LightBarEnd = new CroppedTexture2D(battleGFX, new Rectangle(795, 14, 15, 22));
            LightBarMiddle = new CroppedTexture2D(battleGFX, new Rectangle(813, 14, 1, 22));
        }

        public override void EndInput()
        {
            base.EndInput();

            LightBarEnd = null;
            LightBarMiddle = null;
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

        protected override void OnDraw()
        {
            base.OnDraw();

            string text = "NO!";
            Color color = Color.Red;
            if (AllLightsFilled)
            {
                text = "OKAY!";
                color = Color.Green;
            }

            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, text, new Vector2(300, 100), color, .7f);

            Vector2 startPos = new Vector2(250, 180);
            Vector2 barStartPos = startPos + new Vector2(-30, 0f);

            Vector2 barScale = new Vector2(130, 1f);

            //Draw the bar
            SpriteRenderer.Instance.DrawUI(LightBarEnd.Tex, barStartPos, LightBarEnd.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), Vector2.One, false, false, .69f);
            SpriteRenderer.Instance.DrawUI(LightBarMiddle.Tex, barStartPos + new Vector2((int)(LightBarEnd.WidthHeightToVector2().X / 2), 0f), LightBarMiddle.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), barScale, false, false, .69f);
            SpriteRenderer.Instance.DrawUI(LightBarEnd.Tex, barStartPos + new Vector2(barScale.X + (int)(LightBarEnd.WidthHeightToVector2().X - 1), 0f), LightBarEnd.SourceRect, Color.White, 0f, new Vector2(.5f, .5f), Vector2.One, true, false, .69f);

            //Draw the lights
            DrawLights(startPos, 0, true);
        }
    }
}
