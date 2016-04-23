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
    public class HammerCommand : ActionCommand
    {
        /// <summary>
        /// The max number of lights. The last light is the big one that Left must be released on
        /// </summary>
        protected readonly int MaxLights = 4;
        protected readonly float TimeEachLight = 500f;

        protected float EndTime = 1500f;
        protected float PrevEndTime = 0f;

        protected bool HeldLeft = false;
        protected int LightsLit = 0;
        protected float PrevLightTime = 0f;

        protected Texture2D CircleImage = null;

        public HammerCommand(BattleAction battleAction) : base(battleAction)
        {
            CircleImage = AssetManager.Instance.LoadAsset<Texture2D>($"UI/Circle");
        }

        public override void StartInput()
        {
            LightsLit = 0;
            PrevLightTime = (float)Time.ActiveMilliseconds + TimeEachLight;
            PrevEndTime = (float)Time.ActiveMilliseconds + EndTime;
        }

        protected override void OnSuccess()
        {
            Action.DealDamage(Action.BaseDamage * 2);
            Action.EndSequence();
        }

        protected override void OnFailure()
        {
            Action.DealDamage(Action.BaseDamage);
            Action.EndSequence();
        }

        protected override void ReadInput()
        {
            float time = (float)Time.ActiveMilliseconds;

            //Didn't hold Left in time
            if (HeldLeft == false && time >= PrevEndTime)
            {
                OnFailure();
                return;
            }

            //Holding Left
            if (Input.GetKey(Keys.Left))
            {
                //The first time you hold left, the first light instantly lights up
                if (time >= PrevLightTime || HeldLeft == false)
                {
                    LightsLit++;
                    PrevLightTime = time + TimeEachLight;

                    //Held Left too long (past the last light)
                    if (LightsLit > MaxLights)
                    {
                        OnFailure();
                    }
                }

                HeldLeft = true;
            }
            //Released Left
            else if (HeldLeft == true)
            {
                //Released Left at the right time
                if (LightsLit == MaxLights)
                {
                    OnSuccess();
                }
                //Released Left too early
                else
                {
                    OnFailure();
                }
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();

            string text = "NO!";
            Color color = Color.Red;
            if (LightsLit == MaxLights)
            {
                text = "OKAY!";
                color = Color.Green;
            }

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, text, new Vector2(300, 100), color, .7f);
            Vector2 startPos = new Vector2(250, 150);

            for (int i = 0; i < MaxLights; i++)
            {
                Vector2 newpos = startPos + new Vector2((i * CircleImage.Width) + 15, 0);
                float scale = 1f;
                if (i == (MaxLights - 1))
                {
                    newpos += new Vector2(CircleImage.Width / 2, -(CircleImage.Height / 2));
                    scale = 2f;
                }
                Color circleColor = i >= LightsLit ? Color.Black : Color.White;

                SpriteRenderer.Instance.Draw(CircleImage, newpos, null, circleColor, 0f, CircleImage.GetCenterOrigin(), scale, false, .7f, true);
            }
        }
    }
}
