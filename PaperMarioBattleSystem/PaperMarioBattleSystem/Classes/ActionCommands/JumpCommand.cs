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
        private float Leniency = 200f;
        private float StartRange = 0f;

        //TEMPORARY
        private float SuccessTimeWait = 0f;

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

        protected override void OnSuccess(int successRate)
        {
            Action.OnCommandSuccess(successRate);

            SuccessTimeWait = (float)Time.ActiveMilliseconds + 1500f;
        }

        protected override void OnFailure()
        {
            Action.OnCommandFailed();
        }

        protected override void ReadInput()
        {
            //TEMPORARY
            if (SuccessTimeWait > 0f)
            {
                if ((float)Time.ActiveMilliseconds >= SuccessTimeWait)
                    OnFailure();
                return;
            }

            //Failure if it wasn't pressed
            if (Action.IsSequenceBaseEnded == true)
            {
                OnFailure();
                return;
            }

            if (Input.GetKeyDown(Keys.Z))
            {
                //Success if within range
                if (WithinRange == true)
                {
                    OnSuccess(1);
                }
                //Otherwise failure
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
            if (WithinRange == true)
            {
                text = "OKAY!";
                color = Color.Green;
            }

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, text, new Vector2(300, 100), color, .7f);
        }
    }
}
