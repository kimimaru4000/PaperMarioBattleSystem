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
        private float Leniency = 500f;
        private float StartRange = 0f;
        private float EndRange = 0f;
        private float TotalRange = 0f;

        private bool WithinRange
        {
            get
            {
                float windowPressed = (float)Time.ActiveMilliseconds;
                return (windowPressed >= StartRange && windowPressed < EndRange);
            }
        }

        public JumpCommand(BattleAction battleAction, float totalRange, float leniency) : base(battleAction)
        {
            TotalRange = totalRange;
            Leniency = leniency;
        }

        public override void StartInput()
        {
            base.StartInput();

            StartRange = (float)Time.ActiveMilliseconds + Leniency;
            EndRange = (float)Time.ActiveMilliseconds + TotalRange;
        }

        protected override void OnSuccess()
        {
            Action.OnCommandSuccess();
            EndInput();
        }

        protected override void OnFailure()
        {
            Action.OnCommandFailed();
            EndInput();
        }

        protected override void ReadInput()
        {
            //Failure if it wasn't pressed
            if ((float)Time.ActiveMilliseconds >= EndRange)
            {
                OnFailure();
                return;
            }

            if (Input.GetKeyDown(Keys.Z))
            {
                //Success if within range
                if (WithinRange == true)
                {
                    OnSuccess();
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
