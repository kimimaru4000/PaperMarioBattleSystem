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
    /// Press the jump button once before Mario almost hits the enemy
    /// </summary>
    public sealed class JumpCommand : ActionCommand
    {
        private float Leniency = 600f;
        private float StartRange = 0f;
        private float EndRange = 0f;

        private bool WithinRange
        {
            get
            {
                float windowPressed = (float)Time.ActiveMilliseconds;
                return (windowPressed >= StartRange && windowPressed < EndRange);
            }
        }

        public JumpCommand(BattleAction battleAction) : base(battleAction)
        {
            
        }

        public override void StartInput()
        {
            EndRange = (float)Time.ActiveMilliseconds + Action.BaseLength;
            StartRange = EndRange - Leniency;
        }

        protected override void ReadInput()
        {
            float windowPressed = (float)Time.ActiveMilliseconds;

            //Failure if it wasn't pressed
            if (windowPressed > EndRange)
            {
                Action.OnActionCommandFinish(0);
                return;
            }

            if (Input.GetKeyDown(Keys.Z))
            {
                //Success if within range
                if (windowPressed >= StartRange && windowPressed < EndRange)
                {
                    Action.OnActionCommandFinish(1);
                }
                //Otherwise failure
                else
                {
                    Action.OnActionCommandFinish(0);
                }
            }
        }

        public override void Draw()
        {
            base.Draw();

            string text = "NO!";
            Color color = Color.Red;
            if (WithinRange == true)
            {
                text = "OKAY!";
                color = Color.Green;
            }

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, text, new Vector2(300, 100), color, .7f);
        }
    }
}
