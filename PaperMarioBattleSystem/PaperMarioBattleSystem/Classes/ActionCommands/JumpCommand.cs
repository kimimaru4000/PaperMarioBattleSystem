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

        private bool WithinRange
        {
            get
            {
                float windowPressed = (float)Time.ActiveMilliseconds;
                return (windowPressed >= StartRange && windowPressed < Action.SequenceEndTime);
            }
        }

        public JumpCommand(BattleAction battleAction) : base(battleAction)
        {
            
        }

        public override void StartInput()
        {
            StartRange = Action.SequenceEndTime - Leniency;
        }

        protected override void ReadInput()
        {
            //Failure if it wasn't pressed
            if (Action.IsSequenceBaseEnded == true)
            {
                Action.OnActionCommandFinish(0);
                return;
            }

            if (Input.GetKeyDown(Keys.Z))
            {
                //Success if within range
                if (WithinRange == true)
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
